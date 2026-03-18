using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheck;

        private Rigidbody2D rb;
        private bool _isGrounded;
        private bool _jumpRequested;

        private const float JumpForce = 15f;
        private const float FallMultiplier = 2.5f;
        private const float LowJumpMultiplier = 2f;
        private static readonly Vector2 GroundCheckBoxSize = new Vector2(0.5f, 0.1f);

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _jumpRequested = true;
            }
        }

        private void Update()
        {
            _isGrounded = Physics2D.OverlapBox(groundCheck.position, GroundCheckBoxSize, 0f, groundLayer);
        
            if (_jumpRequested && !_isGrounded && rb.linearVelocity.y < 0)
            {
                _jumpRequested = false;
            }
        }

        private void FixedUpdate()
        {
            if (_jumpRequested && _isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
                _jumpRequested = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(groundCheck.position, GroundCheckBoxSize);
            }
        }
    }
}