using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 7f;
        private float _moveInput;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
    

        [SerializeField] private PlayerDash playerDash;
    
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>().x;
        }
    
        private void FixedUpdate()
        {
            if (playerDash != null && playerDash.IsDashing) return;
        
            rb.linearVelocityX = _moveInput * moveSpeed;
        
            if (_moveInput > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (_moveInput < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}