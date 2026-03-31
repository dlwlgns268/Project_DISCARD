using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoxCollider2D attackCollider;
        [SerializeField] private PlayerAttackHitbox attackHitbox;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const float AttackDuration = 0.15f;
        private const float AttackCooldown = 0.1f;
        private const float ComboResetTime = 0.65f;
        private const int MaxComboStep = 3;

        public bool IsAttacking { get; private set; }

        private float _lastAttackTime = -999f;
        private float _nextComboResetTime = -999f;
        private Coroutine _attackCoroutine;
        private int _comboStep;
        private Vector2 _originalOffset;
        private float _originalOffsetAbsX;
        private bool _isFacingRight = true;
        private WaitForSeconds _attackWait;

        private static readonly int ComboStepHash = Animator.StringToHash("ComboStep");
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

        private void Awake()
        {
            if (attackCollider)
            {
                _originalOffset = attackCollider.offset;
                _originalOffsetAbsX = Mathf.Abs(_originalOffset.x);
                attackCollider.enabled = false;
            }

            if (!attackHitbox && attackCollider)
            {
                attackHitbox = attackCollider.GetComponent<PlayerAttackHitbox>();
            }

            _attackWait = new WaitForSeconds(AttackDuration);

            UpdateFacingState();
            UpdateAttackDirection();
        }

        private void Update()
        {
            bool previousFacing = _isFacingRight;
            UpdateFacingState();

            if (previousFacing != _isFacingRight)
            {
                UpdateAttackDirection();
            }

            if (_comboStep > 0 && !IsAttacking && Time.time >= _nextComboResetTime)
            {
                _comboStep = 0;
            }
        }

        private void OnDisable()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }

            if (attackCollider)
            {
                attackCollider.enabled = false;
            }

            IsAttacking = false;
            _comboStep = 0;
        }

        public void OnAttack(InputValue value)
        {
            if (!value.isPressed || IsAttacking)
            {
                return;
            }

            float currentTime = Time.time;

            if (currentTime < _lastAttackTime + AttackCooldown)
            {
                return;
            }

            if (currentTime >= _nextComboResetTime)
            {
                _comboStep = 1;
            }
            else
            {
                _comboStep = _comboStep >= MaxComboStep ? 1 : _comboStep + 1;
            }

            if (animator)
            {
                animator.SetInteger(ComboStepHash, _comboStep);
                animator.SetTrigger(AttackTriggerHash);
            }

            _attackCoroutine = StartCoroutine(AttackCoroutine());

            _lastAttackTime = currentTime;
            _nextComboResetTime = currentTime + ComboResetTime;
        }

        private IEnumerator AttackCoroutine()
        {
            IsAttacking = true;

            UpdateFacingState();
            UpdateAttackDirection();

            if (attackHitbox)
            {
                attackHitbox.ResetHitTargets();
            }

            if (attackCollider)
            {
                attackCollider.enabled = true;
            }

            yield return _attackWait;

            if (attackCollider)
            {
                attackCollider.enabled = false;
            }

            IsAttacking = false;
            _attackCoroutine = null;
        }

        private void UpdateFacingState()
        {
            if (spriteRenderer)
            {
                _isFacingRight = !spriteRenderer.flipX;
                return;
            }

            _isFacingRight = transform.localScale.x >= 0f;
        }

        private void UpdateAttackDirection()
        {
            if (!attackCollider)
            {
                return;
            }

            attackCollider.offset = new Vector2(_originalOffsetAbsX * (_isFacingRight ? 1f : -1f), _originalOffset.y);
        }

        private void OnDrawGizmosSelected()
        {
            if (attackCollider == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.matrix = attackCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(attackCollider.offset, attackCollider.size);
        }
    }
}