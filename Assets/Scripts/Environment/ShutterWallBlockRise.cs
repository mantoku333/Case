using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterWallBlockRise : MonoBehaviour
{
    [SerializeField] private List<Transform> blocksFromBottom = new List<Transform>();
    [SerializeField] private bool autoCollectChildren = true;

    [Header("Motion")]
    [SerializeField] private bool startsOpened = false;
    [SerializeField, Min(0.01f)] private float riseDurationPerBlock = 0.12f;
    [SerializeField, Min(0f)] private float intervalBetweenBlocks = 0.06f;
    [SerializeField] private bool lockAfterOpen = true;

    private readonly List<Vector3> targetLocalPositions = new List<Vector3>();

    private Coroutine openRoutine;
    private bool isOpen;
    private bool initialized;

    private void Awake()
    {
        InitializeIfNeeded();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && autoCollectChildren && (blocksFromBottom == null || blocksFromBottom.Count == 0))
        {
            RebuildBlockList();
        }
    }
#endif

    public void Open()
    {
        InitializeIfNeeded();

        if (openRoutine != null)
        {
            return;
        }

        if (lockAfterOpen && isOpen)
        {
            return;
        }

        openRoutine = StartCoroutine(OpenRoutine());
    }

    public void Close()
    {
        InitializeIfNeeded();

        if (openRoutine != null)
        {
            return;
        }

        if (lockAfterOpen || !isOpen)
        {
            return;
        }

        openRoutine = StartCoroutine(CloseRoutine());
    }

    private void InitializeIfNeeded()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (autoCollectChildren || blocksFromBottom.Count == 0)
        {
            RebuildBlockList();
        }

        targetLocalPositions.Clear();

        for (int i = 0; i < blocksFromBottom.Count; i++)
        {
            Transform block = blocksFromBottom[i];
            Vector3 target = block != null ? block.localPosition : Vector3.zero;
            targetLocalPositions.Add(target);
        }

        isOpen = startsOpened;
    }

    private IEnumerator OpenRoutine()
    {
        int blockCount = blocksFromBottom.Count;
        if (blockCount == 0)
        {
            openRoutine = null;
            yield break;
        }

        List<Vector3> openStartPositions = new List<Vector3>(blockCount);
        for (int i = 0; i < blockCount; i++)
        {
            Transform block = blocksFromBottom[i];
            openStartPositions.Add(block != null ? block.localPosition : Vector3.zero);
        }

        float stepHeight = ResolveStepHeight(openStartPositions);

        if (blockCount == 1)
        {
            yield return MoveBlocksToY(1, openStartPositions[0].y + stepHeight);
            isOpen = true;
            openRoutine = null;
            yield break;
        }

        // Stage 1: move only the bottom block to overlap the second block,
        // Stage 2: move bottom 2 blocks to overlap the third block, ...
        for (int stage = 1; stage < blockCount; stage++)
        {
            float overlapY = openStartPositions[stage].y;
            yield return MoveBlocksToY(stage, overlapY);

            if (intervalBetweenBlocks > 0f)
            {
                yield return new WaitForSeconds(intervalBetweenBlocks);
            }
        }

        // Final stage: once all blocks overlap at the top, move all blocks one more step up.
        float finalOpenY = openStartPositions[blockCount - 1].y + stepHeight;
        yield return MoveBlocksToY(blockCount, finalOpenY);

        isOpen = true;
        openRoutine = null;
    }

    private IEnumerator CloseRoutine()
    {
        int blockCount = blocksFromBottom.Count;
        if (blockCount == 0)
        {
            openRoutine = null;
            yield break;
        }

        if (targetLocalPositions.Count < blockCount)
        {
            openRoutine = null;
            yield break;
        }

        if (blockCount == 1)
        {
            yield return MoveBlocksToY(1, targetLocalPositions[0].y);
            isOpen = false;
            openRoutine = null;
            yield break;
        }

        // Reverse of opening: first undo the final full-stack lift,
        // then peel layers back down from top overlap to original positions.
        float topClosedY = targetLocalPositions[blockCount - 1].y;
        yield return MoveBlocksToY(blockCount, topClosedY);

        if (intervalBetweenBlocks > 0f)
        {
            yield return new WaitForSeconds(intervalBetweenBlocks);
        }

        for (int stage = blockCount - 1; stage >= 1; stage--)
        {
            float targetY = targetLocalPositions[stage - 1].y;
            yield return MoveBlocksToY(stage, targetY);

            if (stage > 1 && intervalBetweenBlocks > 0f)
            {
                yield return new WaitForSeconds(intervalBetweenBlocks);
            }
        }

        isOpen = false;
        openRoutine = null;
    }

    private IEnumerator MoveBlocksToY(int countFromBottom, float targetY)
    {
        if (countFromBottom <= 0)
        {
            yield break;
        }

        List<Transform> movingBlocks = new List<Transform>(countFromBottom);
        List<Vector3> startPositions = new List<Vector3>(countFromBottom);

        for (int i = 0; i < countFromBottom && i < blocksFromBottom.Count; i++)
        {
            Transform block = blocksFromBottom[i];
            if (block == null)
            {
                continue;
            }

            movingBlocks.Add(block);
            startPositions.Add(block.localPosition);
        }

        if (movingBlocks.Count == 0)
        {
            yield break;
        }

        float duration = Mathf.Max(0.01f, riseDurationPerBlock);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < movingBlocks.Count; i++)
            {
                Vector3 start = startPositions[i];
                Vector3 target = new Vector3(start.x, targetY, start.z);
                movingBlocks[i].localPosition = Vector3.LerpUnclamped(start, target, eased);
            }

            yield return null;
        }

        for (int i = 0; i < movingBlocks.Count; i++)
        {
            Vector3 start = startPositions[i];
            movingBlocks[i].localPosition = new Vector3(start.x, targetY, start.z);
        }
    }

    private float ResolveStepHeight(List<Vector3> referencePositions)
    {
        float step = FindMinPositiveYStep(referencePositions);

        if (!float.IsFinite(step) || step <= 0.001f)
        {
            step = FindMinPositiveYStep(targetLocalPositions);
        }

        if (!float.IsFinite(step) || step <= 0.001f)
        {
            step = 1f;
        }

        return step;
    }

    private static float FindMinPositiveYStep(List<Vector3> positions)
    {
        if (positions == null || positions.Count < 2)
        {
            return float.PositiveInfinity;
        }

        float minStep = float.PositiveInfinity;

        for (int i = 1; i < positions.Count; i++)
        {
            float step = Mathf.Abs(positions[i].y - positions[i - 1].y);
            if (step > 0.001f)
            {
                minStep = Mathf.Min(minStep, step);
            }
        }

        return minStep;
    }

    private void RebuildBlockList()
    {
        blocksFromBottom.Clear();

        foreach (Transform child in transform)
        {
            blocksFromBottom.Add(child);
        }

        blocksFromBottom.Sort((a, b) =>
        {
            float ay = a != null ? a.localPosition.y : float.PositiveInfinity;
            float by = b != null ? b.localPosition.y : float.PositiveInfinity;
            return ay.CompareTo(by);
        });
    }
}
