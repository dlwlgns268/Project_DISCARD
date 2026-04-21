using UnityEngine;

namespace GameLogic.Entity.Player
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;

        public bool IsGrounded { get; private set; }

        private const float JumpForce = 15f;
        private static readonly Vector2 GroundCheckBoxSize = new(0.5f, 0.1f);

        private void Update()
        {
            IsGrounded = Physics2D.OverlapBox(transform.position, GroundCheckBoxSize, 0f, groundLayer);
        }

        private void FixedUpdate()
        {
            if (!Player.Instance.playerInputManager.JumpRequest) return;
            Player.Instance.playerInputManager.JumpRequest = false;
            if (!IsGrounded) return;
            Player.Instance.rb.linearVelocityY = JumpForce;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, GroundCheckBoxSize);
        }
    }
}
