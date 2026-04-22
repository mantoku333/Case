using UnityEngine;

namespace GameName.Enemy
{
    /// <summary>
    /// ステージボス専用の突進攻撃ロジック。
    /// ボスエリア開始時に外部から有効化されると、
    /// 振動→突進→クールダウンをループする。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class StageBossAttack : MonoBehaviour
    {
        [Header("Activation")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool startActiveOnPlay = false;

        [Header("Wind Up")]
        [SerializeField, Min(0.1f)] private float vibrationDuration = 2f;
        [SerializeField, Min(0f)] private float vibrationAmplitude = 0.05f;
        [SerializeField, Min(1f)] private float vibrationFrequency = 35f;

        [Header("Charge")]
        [SerializeField, Min(0.1f)] private float chargeSpeed = 8f;
        [SerializeField, Min(0.05f)] private float chargeDistance = 3.6f;
        [SerializeField, Min(0f)] private float chargeCooldown = 0.4f;
        [SerializeField] private bool stopChargeOnWall = true;
        [SerializeField] private bool stopChargeByDistance = false;
        [SerializeField] private bool stopChargeWhenBlocked = true;
        [SerializeField, Min(0.001f)] private float blockedMoveThreshold = 0.01f;
        [SerializeField, Min(0.02f)] private float blockedStopDelay = 0.1f;

        [Header("Debug")]
        [SerializeField] private bool drawDebugGizmo = true;

        private enum AttackState
        {
            Idle,
            Vibration,
            Charging,
            Cooldown
        }

        private EnemyController enemyController;
        private Transform playerTransform;

        private AttackState attackState = AttackState.Idle;
        private bool encounterActive;
        private float stateTimer;
        private int chargeDirection;
        private float vibrationBaseX;
        private float vibrationElapsed;
        private float chargeStartX;
        private float previousChargeX;
        private float blockedTimer;

        public bool IsEncounterActive => encounterActive;

        private void Awake()
        {
            enemyController = GetComponent<EnemyController>();
            if (enemyController == null)
            {
                Debug.LogWarning("StageBossAttack requires EnemyController on the same GameObject.", this);
                enabled = false;
                return;
            }

            encounterActive = startActiveOnPlay;
        }

        private void OnDisable()
        {
            if (enemyController == null)
            {
                return;
            }

            encounterActive = false;
            attackState = AttackState.Idle;
            stateTimer = 0f;
            enemyController.PauseMovement(false);
            enemyController.StopHorizontalMotion();
        }

        public void ActivateEncounter()
        {
            if (enemyController == null)
            {
                return;
            }

            encounterActive = true;
            if (attackState == AttackState.Idle)
            {
                EnterVibrationState();
            }
        }

        public void DeactivateEncounter()
        {
            if (enemyController == null)
            {
                return;
            }

            encounterActive = false;
            attackState = AttackState.Idle;
            stateTimer = 0f;
            blockedTimer = 0f;
            enemyController.StopHorizontalMotion();
            enemyController.PauseMovement(false);
            enemyController.ResetPatrolOrigin();
        }

        private void FixedUpdate()
        {
            if (!encounterActive)
            {
                return;
            }

            switch (attackState)
            {
                case AttackState.Idle:
                    EnterVibrationState();
                    break;
                case AttackState.Vibration:
                    UpdateVibrationState();
                    break;
                case AttackState.Charging:
                    UpdateChargingState();
                    break;
                case AttackState.Cooldown:
                    UpdateCooldownState();
                    break;
            }
        }

        private void EnterVibrationState()
        {
            enemyController.PauseMovement(true);
            enemyController.StopHorizontalMotion();

            attackState = AttackState.Vibration;
            stateTimer = vibrationDuration;
            vibrationElapsed = 0f;
            chargeDirection = ResolveChargeDirectionTowardsPlayer();
            vibrationBaseX = enemyController.CurrentX;
        }

        private void UpdateVibrationState()
        {
            stateTimer -= Time.fixedDeltaTime;
            vibrationElapsed += Time.fixedDeltaTime;

            float offset = Mathf.Sin(vibrationElapsed * vibrationFrequency * Mathf.PI * 2f) * vibrationAmplitude;
            enemyController.SetHorizontalPosition(vibrationBaseX + offset);
            enemyController.StopHorizontalMotion();

            if (stateTimer <= 0f)
            {
                BeginCharge();
            }
        }

        private void BeginCharge()
        {
            enemyController.SetHorizontalPosition(vibrationBaseX);
            enemyController.FaceDirection(chargeDirection);

            chargeStartX = enemyController.CurrentX;
            previousChargeX = chargeStartX;
            blockedTimer = 0f;
            attackState = AttackState.Charging;
        }

        private void UpdateChargingState()
        {
            if (stopChargeOnWall && enemyController.IsWallAhead())
            {
                EnterCooldownState();
                return;
            }

            enemyController.SetHorizontalVelocity(chargeSpeed);

            if (stopChargeWhenBlocked && IsChargeBlockedThisFrame())
            {
                EnterCooldownState();
                return;
            }

            if (stopChargeByDistance && HasReachedChargeDistance())
            {
                EnterCooldownState();
            }
        }

        private bool IsChargeBlockedThisFrame()
        {
            float currentX = enemyController.CurrentX;
            float movedDistance = Mathf.Abs(currentX - previousChargeX);

            if (movedDistance <= blockedMoveThreshold)
            {
                blockedTimer += Time.fixedDeltaTime;
            }
            else
            {
                blockedTimer = 0f;
            }

            previousChargeX = currentX;
            return blockedTimer >= blockedStopDelay;
        }

        private bool HasReachedChargeDistance()
        {
            float targetDistance = Mathf.Max(0f, chargeDistance);
            if (targetDistance <= 0f)
            {
                return true;
            }

            float traveledDistance = Mathf.Abs(enemyController.CurrentX - chargeStartX);
            return traveledDistance >= targetDistance;
        }

        private void EnterCooldownState()
        {
            attackState = AttackState.Cooldown;
            stateTimer = chargeCooldown;
            enemyController.StopHorizontalMotion();
        }

        private void UpdateCooldownState()
        {
            stateTimer -= Time.fixedDeltaTime;
            enemyController.StopHorizontalMotion();

            if (stateTimer > 0f)
            {
                return;
            }

            attackState = AttackState.Idle;
        }

        private int ResolveChargeDirectionTowardsPlayer()
        {
            int fallbackDirection = enemyController.FacingDirection >= 0 ? 1 : -1;
            if (!TryGetPlayerTransform(out Transform player))
            {
                return fallbackDirection;
            }

            float deltaX = player.position.x - transform.position.x;
            if (Mathf.Abs(deltaX) <= 0.05f)
            {
                return fallbackDirection;
            }

            return deltaX >= 0f ? 1 : -1;
        }

        private bool TryGetPlayerTransform(out Transform player)
        {
            if (playerTransform != null)
            {
                player = playerTransform;
                return true;
            }

            player = null;

            if (string.IsNullOrWhiteSpace(playerTag))
            {
                return false;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject == null)
            {
                return false;
            }

            playerTransform = playerObject.transform;
            player = playerTransform;
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawDebugGizmo)
            {
                return;
            }

            int direction = chargeDirection != 0 ? chargeDirection : 1;
            float previewDistance = Mathf.Max(0.2f, chargeDistance);

            Vector3 start = transform.position;
            Vector3 end = start + (Vector3.right * direction * previewDistance);

            Gizmos.color = new Color(1f, 0.3f, 0.15f, 0.9f);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, 0.12f);
        }
    }
}