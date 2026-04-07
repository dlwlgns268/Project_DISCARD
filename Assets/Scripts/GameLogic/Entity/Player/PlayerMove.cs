using UnityEngine;

namespace GameLogic.Entity.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 7f;
    
        private void FixedUpdate()
        {
            var playerDash = Player.Instance.playerDash;
            if (playerDash && playerDash.IsDashing) return;
            
            var input = Player.Instance.playerInputManager.MoveInput;
            Player.Instance.rb.linearVelocityX = input.x * moveSpeed;

            var spriteRenderer = Player.Instance.spriteRenderer;
            spriteRenderer.flipX = input.x switch
            {
                > 0 => false,
                < 0 => true,
                _ => spriteRenderer.flipX
            };
        }
    }
}
