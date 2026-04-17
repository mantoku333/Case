using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Yarn.Unity;

#nullable enable

namespace Metroidvania.UI
{
    [Serializable]
    public struct BubbleSpeakerAnchor
    {
        public string characterName;
        public Transform target;
        public Vector3 offset;
    }

    /// <summary>
    /// Bubble-style dialogue presenter.
    /// - Follows a speaker transform in world space.
    /// - Resizes the bubble based on the full line length.
    /// </summary>
    public class BubbleDialogueView : DialoguePresenterBase
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject bubblePanel = null!;
        [SerializeField] private TextMeshProUGUI dialogueText = null!;

        [Header("Settings")]
        [SerializeField] private float textSpeed = 30f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);

        [Header("Speaker Anchors")]
        [SerializeField] private List<BubbleSpeakerAnchor> speakerAnchors = new();
        [SerializeField] private bool tryAutoResolveSpeakerByObjectName = true;
        [SerializeField] private bool logUnresolvedSpeaker = true;

        [Header("Auto Size")]
        [SerializeField] private bool autoResizeBubble = true;
        [SerializeField] private Vector2 bubblePadding = new Vector2(56f, 36f);
        [SerializeField] private float minBubbleWidth = 180f;
        [SerializeField] private float maxBubbleWidth = 520f;
        [SerializeField] private float minBubbleHeight = 72f;
        [SerializeField] private float maxBubbleHeight = 280f;
        [SerializeField] private float maxTextWidth = 420f;

        private readonly Dictionary<string, Transform?> _speakerTargetCache =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _warnedUnresolvedSpeakers =
            new(StringComparer.OrdinalIgnoreCase);

        private Transform? _conversationDefaultTarget;
        private Transform? _currentTarget;
        private Vector3 _currentOffset;

        private Camera? _mainCamera;
        private RectTransform? _bubbleRectTransform;
        private RectTransform? _textRectTransform;
        private CancellationTokenSource? _currentLineCts;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _bubbleRectTransform = bubblePanel != null ? bubblePanel.GetComponent<RectTransform>() : null;
            _textRectTransform = dialogueText != null ? dialogueText.rectTransform : null;
            _currentOffset = offset;

            if (bubblePanel != null)
            {
                bubblePanel.SetActive(false);
            }

            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
            }
        }

        public void SetTarget(Transform? target)
        {
            _conversationDefaultTarget = target;
            _currentTarget = target;
            _currentOffset = offset;
        }

        private void LateUpdate()
        {
            if (bubblePanel == null)
            {
                return;
            }

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            if (_currentTarget == null || _mainCamera == null)
            {
                return;
            }

            Vector3 worldPos = _currentTarget.position + _currentOffset;
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0f)
            {
                bubblePanel.SetActive(false);
                return;
            }

            if (!bubblePanel.activeSelf && gameObject.activeInHierarchy)
            {
                bubblePanel.SetActive(true);
            }

            bubblePanel.transform.position = screenPos;
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            if (!gameObject.activeSelf)
            {
                return YarnTask.CompletedTask;
            }

            gameObject.SetActive(true);
            if (bubblePanel != null)
            {
                bubblePanel.SetActive(true);
            }

            if (_currentTarget == null)
            {
                _currentTarget = _conversationDefaultTarget;
                _currentOffset = offset;
            }

            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            if (bubblePanel != null)
            {
                bubblePanel.SetActive(false);
            }

            _currentTarget = null;
            _currentOffset = offset;
            gameObject.SetActive(false);
            return YarnTask.CompletedTask;
        }

        public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            if (!gameObject.activeSelf)
            {
                return YarnTask.CompletedTask;
            }

            var taskCompletionSource = new YarnTaskCompletionSource();
            RunLineInternalAsync(line, token, taskCompletionSource).Forget();
            return taskCompletionSource.Task;
        }

        private async UniTaskVoid RunLineInternalAsync(LocalizedLine line, LineCancellationToken token, YarnTaskCompletionSource tcs)
        {
            _currentLineCts?.Cancel();
            _currentLineCts?.Dispose();
            _currentLineCts = new CancellationTokenSource();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                token.NextContentToken,
                token.HurryUpToken,
                _currentLineCts.Token
            );
            CancellationToken mergedToken = linkedTokenSource.Token;

            ApplySpeakerTarget(line.CharacterName);

            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
            }

            string text = line.TextWithoutCharacterName.Text;
            UpdateBubbleSizeForText(text);

            try
            {
                int textLength = text.Length;
                float delayBetweenChars = 1f / Mathf.Max(1f, textSpeed);

                for (int i = 0; i < textLength; i++)
                {
                    if (dialogueText != null)
                    {
                        dialogueText.text = text.Substring(0, i + 1);
                    }

                    if (token.HurryUpToken.IsCancellationRequested)
                    {
                        if (dialogueText != null)
                        {
                            dialogueText.text = text;
                        }
                        break;
                    }

                    if (mergedToken.IsCancellationRequested)
                    {
                        break;
                    }

                    await UniTask.WaitForSeconds(delayBetweenChars, ignoreTimeScale: true, cancellationToken: mergedToken);
                }
            }
            catch (OperationCanceledException)
            {
                if (dialogueText != null)
                {
                    dialogueText.text = text;
                }
            }

            try
            {
                await UniTask.WaitUntilCanceled(mergedToken);
            }
            catch (OperationCanceledException)
            {
                // continue
            }
            finally
            {
                _currentLineCts?.Dispose();
                _currentLineCts = null;
                tcs.TrySetResult();
            }
        }

        public void OnContinueClicked()
        {
            _currentLineCts?.Cancel();
        }

        public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken cancellationToken)
        {
            return YarnTask.FromResult<DialogueOption?>(dialogueOptions.Length > 0 ? dialogueOptions[0] : null);
        }

        private void ApplySpeakerTarget(string? characterName)
        {
            if (TryResolveSpeakerTarget(characterName, out Transform? target, out Vector3 speakerOffset))
            {
                _currentTarget = target;
                _currentOffset = speakerOffset;
                return;
            }

            string? speakerAlias = ResolveSpeakerAlias(characterName);
            if (!string.IsNullOrEmpty(speakerAlias) &&
                !string.Equals(characterName?.Trim(), speakerAlias, StringComparison.OrdinalIgnoreCase) &&
                TryResolveSpeakerTarget(speakerAlias, out target, out speakerOffset))
            {
                _currentTarget = target;
                _currentOffset = speakerOffset;
                return;
            }

            if (IsNarrationSpeaker(characterName))
            {
                _currentTarget = _conversationDefaultTarget;
                if (_currentTarget == null)
                {
                    _currentTarget = FindPlayerTransform();
                }
                _currentOffset = offset;
                return;
            }

            if (logUnresolvedSpeaker && !string.IsNullOrWhiteSpace(characterName))
            {
                string speaker = characterName.Trim();
                if (_warnedUnresolvedSpeakers.Add(speaker))
                {
                    Debug.LogWarning($"[BubbleDialogueView] Speaker target not resolved for '{speaker}'. Falling back to default target/player.");
                }
            }

            _currentTarget = _conversationDefaultTarget;
            if (_currentTarget == null)
            {
                _currentTarget = FindPlayerTransform();
            }
            _currentOffset = offset;
        }

        private bool TryResolveSpeakerTarget(string? characterName, out Transform? target, out Vector3 speakerOffset)
        {
            target = null;
            speakerOffset = offset;

            if (string.IsNullOrWhiteSpace(characterName))
            {
                return false;
            }

            string speaker = characterName.Trim();

            for (int i = 0; i < speakerAnchors.Count; i++)
            {
                BubbleSpeakerAnchor anchor = speakerAnchors[i];
                if (!string.Equals(anchor.characterName?.Trim(), speaker, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (anchor.target == null)
                {
                    continue;
                }

                target = anchor.target;
                speakerOffset = offset + anchor.offset;
                return true;
            }

            if (_speakerTargetCache.TryGetValue(speaker, out Transform? cached) && cached != null)
            {
                target = cached;
                return true;
            }
            if (_speakerTargetCache.ContainsKey(speaker))
            {
                return false;
            }

            if (!tryAutoResolveSpeakerByObjectName)
            {
                return false;
            }

            Transform? resolved = FindSpeakerTransformByName(speaker);
            _speakerTargetCache[speaker] = resolved;

            if (resolved == null)
            {
                return false;
            }

            target = resolved;
            return true;
        }

        private static Transform? FindSpeakerTransformByName(string speaker)
        {
            if (string.IsNullOrWhiteSpace(speaker))
            {
                return null;
            }

            string trimmedSpeaker = speaker.Trim();
            string alias = ResolveSpeakerAlias(trimmedSpeaker) ?? trimmedSpeaker;

            if (string.Equals(alias, "player", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(alias, "iris", StringComparison.OrdinalIgnoreCase))
            {
                Transform? player = FindPlayerTransform();
                if (player != null)
                {
                    return player;
                }
            }

            if (string.Equals(alias, "nox", StringComparison.OrdinalIgnoreCase))
            {
                Transform? noxTransform = FindTransformByExactName("PG_ACTOR_nox", "_PG_ACTOR_nox", "Nox", "nox");
                if (noxTransform != null)
                {
                    return noxTransform;
                }

                // Nox is often represented by the umbrella in the prologue scene.
                Transform? umbrellaMarker = FindTransformByExactName("PG_MARKER_umbrella", "_PG_MARKER_umbrella");
                if (umbrellaMarker != null)
                {
                    return umbrellaMarker;
                }
            }

            Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            for (int i = 0; i < transforms.Length; i++)
            {
                Transform tf = transforms[i];
                if (tf != null && string.Equals(tf.name, trimmedSpeaker, StringComparison.OrdinalIgnoreCase))
                {
                    return tf;
                }
            }

            string prefixed = "PG_ACTOR_" + alias;
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform tf = transforms[i];
                if (tf != null &&
                    (string.Equals(tf.name, prefixed, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(tf.name, "_" + prefixed, StringComparison.OrdinalIgnoreCase)))
                {
                    return tf;
                }
            }

            return null;
        }

        private static Transform? FindTransformByExactName(params string[] candidateNames)
        {
            if (candidateNames == null || candidateNames.Length == 0)
            {
                return null;
            }

            Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            for (int i = 0; i < candidateNames.Length; i++)
            {
                string name = candidateNames[i];
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                for (int j = 0; j < transforms.Length; j++)
                {
                    Transform tf = transforms[j];
                    if (tf != null && string.Equals(tf.name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return tf;
                    }
                }
            }

            return null;
        }

        private static bool IsNarrationSpeaker(string? speaker)
        {
            string? alias = ResolveSpeakerAlias(speaker);
            return string.Equals(alias, "narration", StringComparison.OrdinalIgnoreCase);
        }

        private static string? ResolveSpeakerAlias(string? speaker)
        {
            if (string.IsNullOrWhiteSpace(speaker))
            {
                return null;
            }

            string trimmed = speaker.Trim();
            if (string.Equals(trimmed, "イリス", StringComparison.OrdinalIgnoreCase))
            {
                return "iris";
            }

            if (string.Equals(trimmed, "ノクス", StringComparison.OrdinalIgnoreCase))
            {
                return "nox";
            }

            if (string.Equals(trimmed, "ナレーション", StringComparison.OrdinalIgnoreCase))
            {
                return "narration";
            }

            return trimmed;
        }

        private static Transform? FindPlayerTransform()
        {
            global::PlayerController player =
                UnityEngine.Object.FindFirstObjectByType<global::PlayerController>(FindObjectsInactive.Include);
            return player != null ? player.transform : null;
        }

        private void UpdateBubbleSizeForText(string text)
        {
            if (!autoResizeBubble || dialogueText == null || _bubbleRectTransform == null)
            {
                return;
            }

            string measureText = string.IsNullOrEmpty(text) ? " " : text;

            float clampedMaxTextWidth = Mathf.Clamp(
                maxTextWidth,
                32f,
                Mathf.Max(32f, maxBubbleWidth - bubblePadding.x));

            Vector2 preferredAtMaxWidth = dialogueText.GetPreferredValues(measureText, clampedMaxTextWidth, 0f);
            float desiredTextWidth = Mathf.Min(preferredAtMaxWidth.x, clampedMaxTextWidth);
            float bubbleWidth = Mathf.Clamp(desiredTextWidth + bubblePadding.x, minBubbleWidth, maxBubbleWidth);

            float finalTextWidth = Mathf.Max(8f, bubbleWidth - bubblePadding.x);
            Vector2 preferredAtFinalWidth = dialogueText.GetPreferredValues(measureText, finalTextWidth, 0f);
            float bubbleHeight = Mathf.Clamp(preferredAtFinalWidth.y + bubblePadding.y, minBubbleHeight, maxBubbleHeight);

            _bubbleRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bubbleWidth);
            _bubbleRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bubbleHeight);

            if (_textRectTransform != null)
            {
                _textRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalTextWidth);
            }
        }
    }
}
