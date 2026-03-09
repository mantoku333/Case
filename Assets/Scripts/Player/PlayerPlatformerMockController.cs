using UnityEngine;
using UnityEngine.InputSystem;

namespace Metroidvania.Player
{
    /// <summary>
    /// Minimal 2D platformer controller for metroidvania mock:
    /// horizontal movement (A/D) and jump (Space) via Input System.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerPlatformerMockController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float moveSpeed = 6f;
        [SerializeField, Min(0f)] private float jumpImpulse = 10f;

        [Header("Physics")]
        [SerializeField] private bool freezeRotationZ = true;
        [SerializeField] private bool applyNoFrictionMaterial = true;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private Transform groundCheck;
        [SerializeField, Min(0.01f)] private float groundCheckRadius = 0.1f;

        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _jumpAction;

        private float _moveInputX;
        private bool _isGrounded;
        private bool _jumpQueued;
        private PhysicsMaterial2D _runtimeNoFrictionMaterial;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _playerInput = GetComponent<PlayerInput>();

            if (freezeRotationZ)
            {
                _rigidbody2D.constraints |= RigidbodyConstraints2D.FreezeRotation;
            }

            if (applyNoFrictionMaterial && _collider2D != null)
            {
                ApplyNoFrictionMaterial();
            }
        }

        private void OnEnable()
        {
            BindActions();
        }

        private void OnDisable()
        {
            if (_jumpAction != null)
            {
                _jumpAction.performed -= OnJumpPerformed;
            }

            _moveInputX = 0f;
            _jumpQueued = false;
        }

        private void Update()
        {
            if (_moveAction != null)
            {
                var move = _moveAction.ReadValue<Vector2>();
                _moveInputX = Mathf.Clamp(move.x, -1f, 1f);
            }

            _isGrounded = CheckGrounded();
        }

        private void FixedUpdate()
        {
            var velocity = _rigidbody2D.linearVelocity;
            velocity.x = _moveInputX * moveSpeed;
            _rigidbody2D.linearVelocity = velocity;

            if (_jumpQueued && _isGrounded)
            {
                velocity = _rigidbody2D.linearVelocity;
                velocity.y = 0f;
                _rigidbody2D.linearVelocity = velocity;
                _rigidbody2D.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            }

            _jumpQueued = false;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _jumpQueued = true;
            }
        }

        private void BindActions()
        {
            if (_playerInput == null || _playerInput.actions == null)
            {
                Debug.LogWarning("PlayerInput or Actions asset is missing.", this);
                return;
            }

            _moveAction = _playerInput.actions.FindAction("Move", false);
            _jumpAction = _playerInput.actions.FindAction("Jump", false);

            if (_moveAction == null || _jumpAction == null)
            {
                Debug.LogWarning("Input Actions 'Move' and/or 'Jump' not found.", this);
                return;
            }

            _jumpAction.performed -= OnJumpPerformed;
            _jumpAction.performed += OnJumpPerformed;
        }

        private bool CheckGrounded()
        {
            if (groundCheck != null)
            {
                return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers) != null;
            }

            if (_collider2D == null)
            {
                return false;
            }

            var bounds = _collider2D.bounds;
            var center = new Vector2(bounds.center.x, bounds.min.y + (groundCheckRadius * 0.5f));
            var size = new Vector2(bounds.size.x * 0.9f, groundCheckRadius);

            return Physics2D.OverlapBox(center, size, 0f, groundLayers) != null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            if (groundCheck != null)
            {
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
            else
            {
                var collider2D = GetComponent<Collider2D>();
                if (collider2D == null)
                {
                    return;
                }

                var bounds = collider2D.bounds;
                var center = new Vector2(bounds.center.x, bounds.min.y + (groundCheckRadius * 0.5f));
                var size = new Vector2(bounds.size.x * 0.9f, groundCheckRadius);
                Gizmos.DrawWireCube(center, size);
            }
        }

        private void ApplyNoFrictionMaterial()
        {
            if (_collider2D.sharedMaterial != null)
            {
                _runtimeNoFrictionMaterial = new PhysicsMaterial2D($"{_collider2D.sharedMaterial.name}_RuntimeNoFriction")
                {
                    friction = 0f,
                    bounciness = 0f
                };
                _collider2D.sharedMaterial = _runtimeNoFrictionMaterial;
                return;
            }

            _runtimeNoFrictionMaterial = new PhysicsMaterial2D("Runtime_Player_NoFriction")
            {
                friction = 0f,
                bounciness = 0f
            };
            _collider2D.sharedMaterial = _runtimeNoFrictionMaterial;
        }
    }
}
