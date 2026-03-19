using System;
using System.Threading;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using Cysharp.Threading.Tasks;

#nullable enable

namespace Metroidvania.UI
{
    /// <summary>
    /// キャラクターの頭上に吹き出し（ポップアップ）としてテキストを表示するYarn Spinner用カスタムビュー。
    /// 対象のTransformにScreenSpaceで追従します。
    /// </summary>
    public class BubbleDialogueView : DialoguePresenterBase
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject bubblePanel = null!;
        [SerializeField] private TextMeshProUGUI dialogueText = null!;

        [Header("Settings")]
        [SerializeField] private float textSpeed = 30f;
        [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0); // キャラの頭上へのオフセット

        private Transform? _currentTarget = null;
        private Camera? _mainCamera;
        private CancellationTokenSource? _currentLineCts;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (bubblePanel != null) bubblePanel.SetActive(false);
            if (dialogueText != null) dialogueText.text = "";
        }

        public void SetTarget(Transform? target)
        {
            _currentTarget = target;
        }

        private void LateUpdate()
        {
            if (_currentTarget == null || _mainCamera == null || bubblePanel == null || !bubblePanel.activeSelf) return;

            // 対象のワールド座標にオフセットを足してスクリーン座標に変換
            Vector3 worldPos = _currentTarget.position + offset;
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
            
            // カメラの後ろにいる場合は表示しない
            if (screenPos.z < 0)
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
            if (!gameObject.activeSelf) return YarnTask.CompletedTask;

            gameObject.SetActive(true);
            if (bubblePanel != null) bubblePanel.SetActive(true);
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            if (bubblePanel != null) bubblePanel.SetActive(false);
            gameObject.SetActive(false);
            return YarnTask.CompletedTask;
        }

        public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            if (!gameObject.activeSelf) return YarnTask.CompletedTask;

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
            var mergedToken = linkedTokenSource.Token;

            if (dialogueText != null) dialogueText.text = "";
            var text = line.TextWithoutCharacterName.Text;

            try
            {
                int textLength = text.Length;
                float delayBetweenChars = 1f / textSpeed;

                for (int i = 0; i < textLength; i++)
                {
                    if (dialogueText != null)
                    {
                        dialogueText.text = text.Substring(0, i + 1);
                    }

                    if (token.HurryUpToken.IsCancellationRequested)
                    {
                        if (dialogueText != null) dialogueText.text = text;
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
                if (dialogueText != null) dialogueText.text = text;
            }

            try
            {
                await UniTask.WaitUntilCanceled(mergedToken);
            }
            catch (OperationCanceledException)
            {
                // Proceed
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
            // Bubbleダイアログでの選択肢は非対応とする（ADV形式を使用する想定）
            return YarnTask.FromResult<DialogueOption?>(dialogueOptions.Length > 0 ? dialogueOptions[0] : null);
        }
    }
}
