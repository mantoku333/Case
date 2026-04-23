using System.Collections;
using Metroidvania.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public sealed class PrologueAutoStarter : MonoBehaviour
{
    private const string RuntimeObjectName = "[PrologueAutoStarter]";

    [SerializeField] private string targetSceneName = "Story_Mantoku";
    [SerializeField] private string prologueNodeName = "Prologue";
    [SerializeField] private DialogueStyle dialogueStyle = DialogueStyle.ADV;
    [SerializeField] private float startDelaySeconds = 0.2f;

    private bool startedByThisRuntime;
    private Coroutine startRoutine;
    private DialogueRunner activeRunner;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        var existing = FindFirstObjectByType<PrologueAutoStarter>(FindObjectsInactive.Include);
        if (existing != null)
        {
            return;
        }

        var runtimeObject = new GameObject(RuntimeObjectName);
        DontDestroyOnLoad(runtimeObject);
        runtimeObject.AddComponent<PrologueAutoStarter>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryStartForCurrentScene();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DetachRunner();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryStartForCurrentScene();
    }

    private void TryStartForCurrentScene()
    {
        if (!string.Equals(SceneManager.GetActiveScene().name, targetSceneName, System.StringComparison.Ordinal))
        {
            return;
        }

        if (GameProgressFlags.Get(GameProgressKeys.PrologueCompleted))
        {
            return;
        }

        if (GameProgressFlags.Get(GameProgressKeys.PrologueStarted))
        {
            return;
        }

        if (startRoutine != null)
        {
            StopCoroutine(startRoutine);
        }

        startRoutine = StartCoroutine(StartPrologueWhenReady());
    }

    private IEnumerator StartPrologueWhenReady()
    {
        if (startDelaySeconds > 0f)
        {
            float elapsed = 0f;
            while (elapsed < startDelaySeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        const int maxFrames = 300;
        int waitedFrames = 0;
        DialogueManager manager = null;

        while (waitedFrames < maxFrames)
        {
            manager = FindFirstObjectByType<DialogueManager>();
            if (manager != null && manager.Runner != null && !manager.Runner.IsDialogueRunning)
            {
                break;
            }

            waitedFrames++;
            yield return null;
        }

        startRoutine = null;

        if (manager == null || manager.Runner == null)
        {
            return;
        }

        if (manager.Runner.Dialogue == null || !manager.Runner.Dialogue.NodeExists(prologueNodeName))
        {
            return;
        }

        GameProgressFlags.Set(GameProgressKeys.PrologueStarted, true);

        startedByThisRuntime = true;
        activeRunner = manager.Runner;
        activeRunner.onDialogueComplete?.AddListener(OnDialogueComplete);
        manager.StartConversation(prologueNodeName, dialogueStyle);
    }

    private void OnDialogueComplete()
    {
        if (!startedByThisRuntime)
        {
            return;
        }

        GameProgressFlags.Set(GameProgressKeys.PrologueCompleted, true);
        startedByThisRuntime = false;
        DetachRunner();
    }

    private void DetachRunner()
    {
        if (activeRunner != null)
        {
            activeRunner.onDialogueComplete?.RemoveListener(OnDialogueComplete);
            activeRunner = null;
        }
    }
}
