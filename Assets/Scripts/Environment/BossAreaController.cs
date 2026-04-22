using GameName.Enemy;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// ボスエリア侵入で戦闘を開始し、
/// 撃破まで壁とカメラをロックするコントローラー。
/// </summary>
[DisallowMultipleComponent]
public sealed class BossAreaController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool disableTriggerAfterStart = true;

    [Header("Boss")]
    [SerializeField] private StageBossAttack stageBossAttack;
    [SerializeField] private Transform bossRoot;
    [SerializeField] private string bossDefeatedFlagKey = GameProgressKeys.Boss01Defeated;

    [Header("Walls")]
    [SerializeField] private ShutterWallBlockRise[] wallsCloseOnStart = new ShutterWallBlockRise[0];
    [SerializeField] private ShutterWallBlockRise[] wallsOpenOnStart = new ShutterWallBlockRise[0];

    [Header("Camera")]
    [SerializeField] private CinemachineCamera fixedBossCamera;
    [SerializeField] private int activeCameraPriority = 50;
    [SerializeField] private int inactiveCameraPriority = 0;

    [Header("Confinement")]
    [SerializeField] private bool confineInsideArea = true;
    [SerializeField] private bool confinePlayerInsideArea = true;
    [SerializeField] private bool confineBossInsideArea = true;
    [SerializeField] private bool confineX = true;
    [SerializeField] private bool confineY = true;
    [SerializeField] private bool confineYForDynamicBodies = false;

    [Header("Debug")]
    [SerializeField] private bool verboseLogging;

    // Change this to true to re-enable shutter wall lock/unlock behavior.
    private static readonly bool enableWallMechanic = false;

    private bool encounterStarted;
    private bool encounterCompleted;
    private bool hasConfinementBounds;
    private Bounds confinementBounds;
    private Transform playerRoot;
    private Rigidbody2D playerRigidbody2D;
    private Rigidbody2D bossRigidbody2D;

    private void Awake()
    {
        CacheConfinementBounds();

        if (stageBossAttack == null && bossRoot != null)
        {
            stageBossAttack = bossRoot.GetComponent<StageBossAttack>();
        }

        if (bossRoot == null && stageBossAttack != null)
        {
            bossRoot = stageBossAttack.transform;
        }

        if (bossRoot != null)
        {
            bossRigidbody2D = bossRoot.GetComponent<Rigidbody2D>();
        }

        CachePlayerReferences();

        DeactivateBossCamera();

        if (!string.IsNullOrWhiteSpace(bossDefeatedFlagKey) && GameProgressFlags.Get(bossDefeatedFlagKey))
        {
            encounterCompleted = true;

            if (enableWallMechanic)
            {
                UnlockArea();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!encounterStarted || encounterCompleted || !confineInsideArea)
        {
            return;
        }

        ConfineTargetsInsideArea();
    }

    private void Update()
    {
        if (!encounterStarted || encounterCompleted)
        {
            return;
        }

        if (!IsBossAlive())
        {
            CompleteEncounter();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            TryStartEncounter();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            TryStartEncounter();
        }
    }

    private void TryStartEncounter()
    {
        if (encounterStarted || encounterCompleted)
        {
            return;
        }

        CachePlayerReferences();

        encounterStarted = true;

        if (enableWallMechanic)
        {
            LockArea();
        }

        ActivateBossCamera();

        if (stageBossAttack != null)
        {
            stageBossAttack.ActivateEncounter();
        }

        if (disableTriggerAfterStart)
        {
            DisableTriggerComponents();
        }

        if (verboseLogging)
        {
            Debug.Log($"[BossAreaController] Encounter started on {gameObject.name}", this);
        }
    }

    private void CompleteEncounter()
    {
        if (encounterCompleted)
        {
            return;
        }

        encounterCompleted = true;
        encounterStarted = false;

        if (stageBossAttack != null)
        {
            stageBossAttack.DeactivateEncounter();
        }

        if (!string.IsNullOrWhiteSpace(bossDefeatedFlagKey))
        {
            GameProgressFlags.Set(bossDefeatedFlagKey, true);
        }

        if (enableWallMechanic)
        {
            UnlockArea();
        }

        DeactivateBossCamera();

        if (verboseLogging)
        {
            Debug.Log($"[BossAreaController] Encounter completed on {gameObject.name}", this);
        }
    }

    private bool IsBossAlive()
    {
        if (stageBossAttack != null)
        {
            bossRoot = stageBossAttack.transform;

            if (bossRoot != null && bossRigidbody2D == null)
            {
                bossRigidbody2D = bossRoot.GetComponent<Rigidbody2D>();
            }
        }

        return bossRoot != null;
    }

    private void ConfineTargetsInsideArea()
    {
        if (!hasConfinementBounds)
        {
            return;
        }

        if (confinePlayerInsideArea)
        {
            CachePlayerReferences();
            ConstrainTransform(playerRoot, playerRigidbody2D);
        }

        if (confineBossInsideArea)
        {
            ConstrainTransform(bossRoot, bossRigidbody2D);
        }
    }

    private void ConstrainTransform(Transform target, Rigidbody2D targetRigidbody2D)
    {
        if (target == null)
        {
            return;
        }

        Vector2 extents = ResolveColliderExtents(target);
        Vector3 current = target.position;

        float clampedX = current.x;
        float clampedY = current.y;

        if (confineX)
        {
            float minX = confinementBounds.min.x + extents.x;
            float maxX = confinementBounds.max.x - extents.x;
            clampedX = minX <= maxX ? Mathf.Clamp(current.x, minX, maxX) : confinementBounds.center.x;
        }

        bool canConfineY = confineY && ShouldConfineYForTarget(targetRigidbody2D);
        if (canConfineY)
        {
            float minY = confinementBounds.min.y + extents.y;
            float maxY = confinementBounds.max.y - extents.y;
            clampedY = minY <= maxY ? Mathf.Clamp(current.y, minY, maxY) : confinementBounds.center.y;
        }

        if (Mathf.Approximately(clampedX, current.x) && Mathf.Approximately(clampedY, current.y))
        {
            return;
        }

        if (targetRigidbody2D != null)
        {
            targetRigidbody2D.position = new Vector2(clampedX, clampedY);
            return;
        }

        target.position = new Vector3(clampedX, clampedY, current.z);
    }

    private bool ShouldConfineYForTarget(Rigidbody2D targetRigidbody2D)
    {
        if (targetRigidbody2D == null)
        {
            return true;
        }

        if (targetRigidbody2D.bodyType != RigidbodyType2D.Dynamic)
        {
            return true;
        }

        return confineYForDynamicBodies;
    }

    private static Vector2 ResolveColliderExtents(Transform target)
    {
        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(includeInactive: false);
        if (colliders == null || colliders.Length == 0)
        {
            return Vector2.zero;
        }

        bool hasBounds = false;
        Bounds merged = default;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D collider = colliders[i];
            if (collider == null || !collider.enabled)
            {
                continue;
            }

            if (!hasBounds)
            {
                merged = collider.bounds;
                hasBounds = true;
            }
            else
            {
                merged.Encapsulate(collider.bounds);
            }
        }

        return hasBounds ? (Vector2)merged.extents : Vector2.zero;
    }

    private void CacheConfinementBounds()
    {
        Collider2D area2D = GetComponent<Collider2D>();
        if (area2D != null)
        {
            hasConfinementBounds = true;
            confinementBounds = area2D.bounds;
            return;
        }

        Collider area3D = GetComponent<Collider>();
        if (area3D != null)
        {
            hasConfinementBounds = true;
            confinementBounds = area3D.bounds;
            return;
        }

        hasConfinementBounds = false;
    }

    private void CachePlayerReferences()
    {
        if (playerRoot != null)
        {
            if (playerRigidbody2D == null)
            {
                playerRigidbody2D = playerRoot.GetComponent<Rigidbody2D>();
            }

            return;
        }

        if (string.IsNullOrWhiteSpace(playerTag))
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject == null)
        {
            return;
        }

        playerRoot = playerObject.transform;
        playerRigidbody2D = playerObject.GetComponent<Rigidbody2D>();
    }

    private void LockArea()
    {
        ApplyWallCommands(wallsCloseOnStart, open: false);
        ApplyWallCommands(wallsOpenOnStart, open: true);
    }

    private void UnlockArea()
    {
        ApplyWallCommands(wallsCloseOnStart, open: true);
        ApplyWallCommands(wallsOpenOnStart, open: false);
    }

    private static void ApplyWallCommands(ShutterWallBlockRise[] walls, bool open)
    {
        if (walls == null || walls.Length == 0)
        {
            return;
        }

        for (int i = 0; i < walls.Length; i++)
        {
            ShutterWallBlockRise wall = walls[i];
            if (wall == null)
            {
                continue;
            }

            if (open)
            {
                wall.TryOpen();
            }
            else
            {
                wall.TryClose();
            }
        }
    }

    private void ActivateBossCamera()
    {
        if (fixedBossCamera == null)
        {
            return;
        }

        fixedBossCamera.Priority.Value = activeCameraPriority;
        fixedBossCamera.Priority.Enabled = true;
    }

    private void DeactivateBossCamera()
    {
        if (fixedBossCamera == null)
        {
            return;
        }

        fixedBossCamera.Priority.Value = inactiveCameraPriority;
        fixedBossCamera.Priority.Enabled = true;
    }

    private void DisableTriggerComponents()
    {
        Collider2D trigger2D = GetComponent<Collider2D>();
        if (trigger2D != null)
        {
            trigger2D.enabled = false;
        }

        Collider trigger3D = GetComponent<Collider>();
        if (trigger3D != null)
        {
            trigger3D.enabled = false;
        }
    }
}