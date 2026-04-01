using GameName.Player;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの基本操作を管理するクラス
/// 左右移動、ジャンプ、向き制御、銃反動、傘の開け閉め、敵への攻撃
/// 入力を受け取り、それぞれの機能へ処理を振り分ける
/// </summary>
public class PlayerController : MonoBehaviour
{
    //--------------ステータス関連------------------
    [Header("プレイヤーステータス")]
    [SerializeField] private PlayerStatsData playerStatsData;

    //--------------移動関連------------------
    private Rigidbody2D m_RigidBody2D;
    private float m_MoveInput;
    private bool m_JumpInput;
    private bool m_IsGround;

    //--------------各種コンポーネント参照関連------------------
    private GroundCheck groundCheck;
    private GunController gunController;
    private UmbrellaController umbrellaController;
    private UmbrellaAttackController umbrellaAttackController;
    private UmbrellaParryController umbrellaParryController;
    private ParryHitbox parryHitbox;
    private DodgeController dodgeController;

    private void Awake()
    {
        m_RigidBody2D = GetComponent<Rigidbody2D>();

        if (m_RigidBody2D == null)
        {
            Debug.LogError("Rigidbody2Dが見つかっていません！");
        }
    }

    private void Start()
    {
        FindComponents();
        ApplyStats();
    }

    private void Update()
    {
        if (groundCheck == null)
        {
            return;
        }

        m_IsGround = groundCheck.IsGround();

        GetInput();
        Flip();
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    //--------Set関数-------
    public void SetPlayerStatsData(PlayerStatsData playerStatsData)
    {
        playerStatsData = playerStatsData;
        ApplyStats();
    }

    //--------Get関数-------
    public PlayerStatsData GetPlayerStatsData()
    {
        return playerStatsData;
    }

    //--------------初期化関連------------------
    private void FindComponents()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheckが見つかっていません！");
        }

        gunController = GetComponentInChildren<GunController>();
        if (gunController == null)
        {
            Debug.LogError("GunControllerが見つかっていません！");
        }

        umbrellaController = GetComponentInChildren<UmbrellaController>();
        if (umbrellaController == null)
        {
            Debug.LogError("UmbrellaControllerが見つかっていません！");
        }

        umbrellaAttackController = GetComponentInChildren<UmbrellaAttackController>();
        if (umbrellaAttackController == null)
        {
            Debug.LogError("UmbrellaAttackControllerが見つかっていません！");
        }

        umbrellaParryController = GetComponentInChildren<UmbrellaParryController>();
        if (umbrellaParryController == null)
        {
            Debug.LogError("UmbrellaParryControllerが見つかっていません！");
        }

        parryHitbox = GetComponentInChildren<ParryHitbox>();
        if (parryHitbox == null)
        {
            Debug.LogError("ParryHitboxが見つかっていません！");
        }

        dodgeController = GetComponent<DodgeController>();
        if (dodgeController == null)
        {
            Debug.LogError("DodgeControllerが見つかっていません！");
        }
    }

    private void ApplyStats()
    {
        if (playerStatsData == null)
        {
            Debug.LogWarning("PlayerStatsDataが設定されていません。Inspectorから設定してください。");
            return;
        }

        if (umbrellaController != null)
        {
            umbrellaController.SetGlideMoveSpeed(playerStatsData.GlideMoveSpeed);
            umbrellaController.SetFallSpeed(playerStatsData.FallSpeed);
        }

        if (gunController != null)
        {
            gunController.SetAirRecoilPower(playerStatsData.GunRecoilForce);
            gunController.SetCoolTime(playerStatsData.ReloadSeconds);
        }

        if (umbrellaAttackController != null)
        {
            umbrellaAttackController.SetAttackPerSecond(playerStatsData.AttackPerSecond);
        }
    }

    //--------------入力関連------------------
    private void GetInput()
    {
        if (umbrellaController == null)
        {
            return;
        }

        bool isGliding = (umbrellaController.GetUmbrellaState() == UmbrellaController.UmbrellaState.Open);

        m_MoveInput = 0.0f;

        if (Keyboard.current.aKey.isPressed)
        {
            m_MoveInput = -1.0f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            m_MoveInput = 1.0f;
        }

        if (
            Keyboard.current.leftShiftKey.wasPressedThisFrame ||
            (
                Keyboard.current.leftShiftKey.isPressed &&
                (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            )
        )
        {
            Vector2 dodgeDirection = Vector2.zero;

            if (Keyboard.current.aKey.isPressed)
            {
                dodgeDirection = Vector2.left;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                dodgeDirection = Vector2.right;
            }

            if (dodgeDirection != Vector2.zero)
            {
                if (dodgeController != null)
                {
                    dodgeController.Dodge(dodgeDirection).Forget();
                }
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            m_JumpInput = true;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleRightClick();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick(isGliding);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (m_IsGround)
            {
                if (gunController != null)
                {
                    gunController.JumpRecoil();
                }
            }
        }
    }

    private void HandleRightClick()
    {
        if (umbrellaController == null)
        {
            return;
        }

        if (parryHitbox != null && parryHitbox.HasEnemyAttack())
        {
            if (umbrellaParryController != null)
            {
                umbrellaParryController.Parry().Forget();
            }

            var bullets = parryHitbox.GetEnemyAttacks();

            foreach (var bullet in bullets)
            {
                if (bullet != null)
                {
                    Destroy(bullet);
                }
            }

            parryHitbox.ClearEnemyAttacks();
        }
        else
        {
            umbrellaController.ToggleUmbrella();
        }
    }

    private void HandleLeftClick(bool isGliding)
    {
        if (!m_IsGround)
        {
            if (gunController == null)
            {
                return;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));
            mouseWorldPos.z = 0.0f;

            Vector2 shootDirection = (mouseWorldPos - transform.position).normalized;
            gunController.Shoot(shootDirection);
            return;
        }

        if (!isGliding)
        {
            if (umbrellaAttackController != null)
            {
                umbrellaAttackController.Attack().Forget();
            }
        }
    }

    //--------------移動関連------------------
    private void Move()
    {
        if (m_RigidBody2D == null)
        {
            return;
        }

        if (playerStatsData == null)
        {
            return;
        }

        if (umbrellaController == null)
        {
            return;
        }

        Vector2 velocity = m_RigidBody2D.linearVelocity;
        bool isGliding = (umbrellaController.GetUmbrellaState() == UmbrellaController.UmbrellaState.Open);

        if (m_MoveInput != 0.0f)
        {
            float moveSpeed = playerStatsData.MoveSpeed;

            if (isGliding)
            {
                moveSpeed = umbrellaController.GetGlideMoveSpeed();
            }

            velocity.x = m_MoveInput * moveSpeed;
        }
        else
        {
            if (isGliding)
            {
                velocity.x *= 0.95f;
            }
            else
            {
                velocity.x = 0.0f;
            }
        }

        m_RigidBody2D.linearVelocity = velocity;
    }

    private void Jump()
    {
        if (!m_JumpInput)
        {
            return;
        }

        if (m_RigidBody2D == null)
        {
            m_JumpInput = false;
            return;
        }

        if (playerStatsData == null)
        {
            m_JumpInput = false;
            return;
        }

        if (m_IsGround)
        {
            m_RigidBody2D.linearVelocity = new Vector2(m_RigidBody2D.linearVelocity.x, playerStatsData.JumpForce);
        }

        m_JumpInput = false;
    }

    //--------------向き関連------------------
    private void Flip()
    {
        if (umbrellaController == null)
        {
            return;
        }

        bool isGliding = (umbrellaController.GetUmbrellaState() == UmbrellaController.UmbrellaState.Open);

        if (m_IsGround)
        {
            if (m_MoveInput > 0.0f)
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (m_MoveInput < 0.0f)
            {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }

            return;
        }

        if (isGliding)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));
            mouseWorldPos.z = 0.0f;

            if (mouseWorldPos.x > transform.position.x)
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else
            {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }

            return;
        }

        if (m_MoveInput > 0.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else if (m_MoveInput < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
    }
}
