using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class TutorialTriggerZone : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private TutorialOverlayController tutorialOverlay;
    [SerializeField] private string completedFlagKey = GameProgressKeys.TutorialAttackShown;
    [SerializeField] private bool markCompletedOnOpen;
    [SerializeField] private bool disableAfterCompletion = true;

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";

    private bool triggered;

    private void Reset()
    {
        EnsureTriggerCollider();
    }

    private void Awake()
    {
        EnsureTriggerCollider();

        if (IsAlreadyCompleted())
        {
            triggered = true;
            DisableIfConfigured();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(playerTag) && !other.CompareTag(playerTag))
        {
            return;
        }

        if (IsAlreadyCompleted())
        {
            triggered = true;
            DisableIfConfigured();
            return;
        }

        if (tutorialOverlay == null)
        {
            Debug.LogWarning($"[TutorialTriggerZone] TutorialOverlayController is missing on '{name}'.");
            return;
        }

        triggered = true;

        if (markCompletedOnOpen)
        {
            MarkCompleted();
        }

        tutorialOverlay.Show(OnTutorialClosed);
    }

    private void OnTutorialClosed()
    {
        if (!markCompletedOnOpen)
        {
            MarkCompleted();
        }

        DisableIfConfigured();
    }

    private bool IsAlreadyCompleted()
    {
        if (string.IsNullOrWhiteSpace(completedFlagKey))
        {
            return false;
        }

        return GameProgressFlags.Get(completedFlagKey);
    }

    private void MarkCompleted()
    {
        if (string.IsNullOrWhiteSpace(completedFlagKey))
        {
            return;
        }

        GameProgressFlags.Set(completedFlagKey, true);
    }

    private void DisableIfConfigured()
    {
        if (disableAfterCompletion)
        {
            enabled = false;
        }
    }

    private void EnsureTriggerCollider()
    {
        Collider2D target = GetComponent<Collider2D>();
        if (target != null)
        {
            target.isTrigger = true;
        }
    }
}
