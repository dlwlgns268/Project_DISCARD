using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float dashForce = 15f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 3f;

        public bool IsDashing { get; private set; }

        private bool _dashRequested;
        private float _dashYPosition;
        private float _lastDashTime = -999f;
        private Vector2 _moveInput;

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        public void OnDash(InputValue value)
        {
            if (value.isPressed && !IsDashing && Time.time >= _lastDashTime + dashCooldown)
            {
                _dashRequested = true;
            }
        }

        private void Update()
        {
            if (_dashRequested)
            {
                _dashRequested = false;
                StartCoroutine(DashCoroutine());
            }
        }

        private IEnumerator DashCoroutine()
        {
            IsDashing = true;
            _lastDashTime = Time.time;

            float originalGravityScale = rb.gravityScale;
            rb.gravityScale = 0f;

            _dashYPosition = transform.position.y;

            float dashDirection;
        
            // 입력이 있으면 입력 방향으로, 없으면 현재 보는 방향으로 대쉬
            if (Mathf.Abs(_moveInput.x) > 0.01f)
            {
                dashDirection = _moveInput.x > 0 ? 1f : -1f;
            }
            else
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                {
                    dashDirection = spriteRenderer.flipX ? -1f : 1f;
                }
                else
                {
                    dashDirection = transform.localScale.x > 0 ? 1f : -1f;
                }
            }

            rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                Vector3 currentPos = transform.position;
                currentPos.y = _dashYPosition;
                transform.position = currentPos;

                rb.linearVelocityY = 0f;

                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.gravityScale = originalGravityScale;
            IsDashing = false;
        }
    }
}