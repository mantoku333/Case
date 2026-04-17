using System.Collections.Generic;
using Metroidvania.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public sealed class StoryEventRunner : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager = null!;

    private readonly Queue<StoryEventDefinition> queuedEvents = new Queue<StoryEventDefinition>();
    private StoryEventDefinition activeEvent;
    private DialogueRunner activeDialogueRunner;

    public bool HasPendingEvents => activeEvent != null || queuedEvents.Count > 0;

    public void Enqueue(StoryEventDefinition definition)
    {
        if (definition == null)
        {
            return;
        }

        queuedEvents.Enqueue(definition);
        TryStartNextEvent();
    }

    public void ClearQueue()
    {
        queuedEvents.Clear();
    }

    private void OnDisable()
    {
        UnsubscribeFromDialogueComplete();
        activeEvent = null;
        queuedEvents.Clear();
    }

    private void TryStartNextEvent()
    {
        if (activeEvent != null)
        {
            return;
        }

        while (queuedEvents.Count > 0)
        {
            StoryEventDefinition nextEvent = queuedEvents.Dequeue();
            if (TryStartEvent(nextEvent))
            {
                return;
            }
        }
    }

    private bool TryStartEvent(StoryEventDefinition definition)
    {
        if (definition == null)
        {
            return false;
        }

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (!definition.MatchesScene(activeSceneName))
        {
            return false;
        }

        if (!definition.CanRunByFlags())
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(definition.dialogueNodeName))
        {
            Debug.LogWarning("[StoryEventRunner] dialogueNodeName is empty.");
            return false;
        }

        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }

        if (dialogueManager == null || dialogueManager.Runner == null)
        {
            Debug.LogWarning($"[StoryEventRunner] DialogueManager not found. eventId='{definition.eventId}'");
            return false;
        }

        DialogueRunner runner = dialogueManager.Runner;
        if (runner.Dialogue == null || !runner.Dialogue.NodeExists(definition.dialogueNodeName))
        {
            Debug.LogWarning(
                $"[StoryEventRunner] Dialogue node not found. eventId='{definition.eventId}', node='{definition.dialogueNodeName}'");
            return false;
        }

        if (runner.IsDialogueRunning && definition.skipWhenDialogueRunning)
        {
            Debug.LogWarning(
                $"[StoryEventRunner] Dialogue is already running. eventId='{definition.eventId}' was skipped.");
            return false;
        }

        if (runner.IsDialogueRunning && !definition.skipWhenDialogueRunning)
        {
            runner.Stop();
        }

        activeEvent = definition;
        activeDialogueRunner = runner;
        activeDialogueRunner.onDialogueComplete?.AddListener(OnDialogueComplete);

        activeEvent.onStartMutations?.Apply();
        dialogueManager.StartConversation(activeEvent.dialogueNodeName, activeEvent.dialogueStyle);

        return true;
    }

    private void OnDialogueComplete()
    {
        if (activeEvent == null)
        {
            return;
        }

        StoryEventDefinition completedEvent = activeEvent;
        activeEvent = null;
        UnsubscribeFromDialogueComplete();

        completedEvent.onCompleteMutations?.Apply();

        if (!string.IsNullOrWhiteSpace(completedEvent.runOnceFlagKey))
        {
            GameProgressFlags.Set(completedEvent.runOnceFlagKey.Trim(), true);
        }

        if (completedEvent.autoSaveOnComplete)
        {
            SaveManager.TrySaveCurrentGame();
        }

        TryStartNextEvent();
    }

    private void UnsubscribeFromDialogueComplete()
    {
        if (activeDialogueRunner != null)
        {
            activeDialogueRunner.onDialogueComplete?.RemoveListener(OnDialogueComplete);
            activeDialogueRunner = null;
        }
    }
}
