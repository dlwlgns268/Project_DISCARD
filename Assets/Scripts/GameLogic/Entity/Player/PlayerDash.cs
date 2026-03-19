using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDash : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D strongDashAttackCollider;
        [SerializeField] private StrongDashHitbox strongDashHitbox;

        [Header("Normal Dash")]
        private readonly float _dashForce = 37.5f;
        private readonly float _dashDuration = 0.2f;
        private readonly float _dashCooldown = 1f;

        [Header("Strong Dash")]
        [SerializeField] private float strongDashForce = 37.5f;
        [SerializeField] private float strongDashDuration = 0.2f;
        [SerializeField] private float strongDashChargeTime = 3f;

        public bool IsDashing { get; private set; }
        public bool IsInvincible { get; private set; }
        public bool IsStrongDashReady => Time.time >= _lastStrongDashTime + strongDashChargeTime;

        private bool _dashRequested;
        private float _dashYPosition;
        private float _lastStrongDashTime = -999f;
        private Vector2 _moveInput;
        private Coroutine _dashCoroutine;
        private float _nextDashAvailableTime = -999f;
        private WaitForFixedUpdate _waitForFixed;

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        public void OnDash(InputValue value)
        {
            if (value.isPressed && !IsDashing && Time.time >= _nextDashAvailableTime)
            {
                _dashRequested = true;
            }
        }

        private void Awake()
        {
            if (!rb)
            {
                rb = GetComponent<Rigidbody2D>();
            }

            if (!spriteRenderer)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (strongDashHitbox == null && strongDashAttackCollider != null)
            {
                strongDashHitbox = strongDashAttackCollider.GetComponent<StrongDashHitbox>();
            }

            _waitForFixed = new WaitForFixedUpdate();
            DisableStrongDashAttack();
        }

        private void Update()
        {
            if (_dashRequested)
            {
                _dashRequested = false;

                if (_dashCoroutine != null)
                {
                    StopCoroutine(_dashCoroutine);
                }

                _dashCoroutine = StartCoroutine(DashCoroutine());
            }
        }

        private IEnumerator DashCoroutine()
        {
            IsDashing = true;

            float currentTime = Time.time;
            _nextDashAvailableTime = currentTime + _dashCooldown;

            bool useStrongDash = IsStrongDashReady;

            float currentDashForce = useStrongDash ? strongDashForce : _dashForce;
            float currentDashDuration = useStrongDash ? strongDashDuration : _dashDuration;

            if (useStrongDash)
            {
                _lastStrongDashTime = currentTime;
                IsInvincible = true;

                if (strongDashHitbox)
                {
                    strongDashHitbox.ResetHitTargets();
                }

                if (strongDashAttackCollider)
                {
                    strongDashAttackCollider.enabled = true;
                }
            }

            float originalGravityScale = rb.gravityScale;
            rb.gravityScale = 0f;

            _dashYPosition = transform.position.y;
            float dashDirection = GetDashDirection();

            rb.linearVelocity = new Vector2(dashDirection * currentDashForce, 0f);

            float elapsed = 0f;

            while (elapsed < currentDashDuration)
            {
                Vector3 currentPos = transform.position;
                currentPos.y = _dashYPosition;
                transform.position = currentPos;

                rb.linearVelocityY = 0f;

                elapsed += Time.fixedDeltaTime;
                yield return _waitForFixed;
            }

            rb.gravityScale = originalGravityScale;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (useStrongDash)
            {
                DisableStrongDashAttack();
                IsInvincible = false;
            }

            IsDashing = false;
            _dashCoroutine = null;
        }

        private float GetDashDirection()
        {
            if (Mathf.Abs(_moveInput.x) > 0.01f)
            {
                return Mathf.Sign(_moveInput.x);
            }

            if (spriteRenderer)
            {
                return spriteRenderer.flipX ? -1f : 1f;
            }

            return transform.localScale.x >= 0f ? 1f : -1f;
        }

        private void DisableStrongDashAttack()
        {
            if (strongDashAttackCollider)
            {
                strongDashAttackCollider.enabled = false;
            }
        }

        private void OnDisable()
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
                _dashCoroutine = null;
            }

            DisableStrongDashAttack();
            IsInvincible = false;
            IsDashing = false;
        }
    }
}