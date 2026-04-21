using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameLogic.Entity.Player
{
    public class PlayerDash : MonoBehaviour
    {
        private static readonly int Thickness = Shader.PropertyToID("_Thickness");
        [SerializeField] private DashHitbox dashHitbox;
        [SerializeField] private SpriteRenderer targetIndicator;
        [SerializeField] private Material playerOutlineMaterial;

        [SerializeField] private float strongDashForce = 37.5f;
        [SerializeField] private float strongDashDuration = 0.2f;

        private const float DashForce = 37.5f;
        private const float DashDuration = 0.2f;

        public bool IsDashing { get; private set; }
        public bool IsInvincible { get; set; }
        public bool IsStrongDashReady => StrongDashCount >= 2;

        private Coroutine _dashCoroutine;
        private float _originalGravityScale;

        public bool IsDashAvailable { get; set; }
        public int StrongDashCount { get; set; }

        private void Awake()
        {
            dashHitbox.selfCollider.enabled = false;
        }

        private void Update()
        {
            playerOutlineMaterial.SetFloat(Thickness, IsStrongDashReady ? 1 : 0);
            if (IsStrongDashReady)
            {
                var target = GetNearestNonExecutionTarget();
                targetIndicator.gameObject.SetActive(target.Length > 0);
                if (target.Length <= 0) goto label;
                targetIndicator.transform.position = target[0].transform.position;
            }
            else
            {
                targetIndicator.gameObject.SetActive(false);
            }

            label:
            if (Player.Instance.playerJump.IsGrounded && !IsDashing) IsDashAvailable = true;
            if (!Player.Instance.playerInputManager.DashRequest) return;
            Player.Instance.playerInputManager.DashRequest = false;
            if (!IsDashAvailable) return;
            IsDashAvailable = false;
            if (_dashCoroutine != null)
            {
                var rb = Player.Instance.rb;
                rb.gravityScale = _originalGravityScale;
                rb.linearVelocity *= 0.1f;
                StopCoroutine(_dashCoroutine);
            }

            _dashCoroutine = StartCoroutine(DashCoroutine());
        }

        private IEnumerator DashCoroutine()
        {
            IsDashing = true;

            var useStrongDash = IsStrongDashReady;
            dashHitbox.IsStrongDash = useStrongDash;

            var currentDashForce = useStrongDash ? strongDashForce : DashForce;
            var currentDashDuration = useStrongDash ? strongDashDuration : DashDuration;

            if (useStrongDash)
            {
                StrongDashCount = 0;
                IsInvincible = true;
            }

            dashHitbox.ResetHitTargets();
            dashHitbox.selfCollider.enabled = true;

            var rb = Player.Instance.rb;
            _originalGravityScale = rb.gravityScale;
            rb.gravityScale = 0f;

            var dashDirection = GetDashDirection(useStrongDash);
            rb.linearVelocity = dashDirection * currentDashForce;

            yield return new WaitForSeconds(currentDashDuration);

            rb.gravityScale = _originalGravityScale;
            rb.linearVelocity *= 0.3f;

            if (useStrongDash) IsInvincible = false;
            dashHitbox.selfCollider.enabled = false;

            IsDashing = false;
            _dashCoroutine = null;
        }

        private Vector2 GetDashDirection(bool isStrongDash)
        {
            var moveInput = Player.Instance.playerInputManager.MoveInput;
            var enemy = GetNearestNonExecutionTarget();
            if (!isStrongDash || enemy.Length <= 0)
                return Mathf.Abs(moveInput.magnitude) > 0.01f
                    ? moveInput.normalized
                    : new Vector2(Player.Instance.spriteRenderer.flipX ? -1f : 1f, 0);

            var dir = enemy[0].transform.position - Player.Instance.spriteRenderer.bounds.center;
            return (dir / 7.5f).magnitude >= 1f ? dir / 7.5f + dir.normalized / 10f : dir.normalized * 1.1f;
        }

        private void OnDisable()
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
                _dashCoroutine = null;
            }

            dashHitbox.selfCollider.enabled = false;
            IsInvincible = false;
            IsDashing = false;
        }

        private Enemy.Enemy[] GetNearestNonExecutionTarget()
        {
            var moveInput = Player.Instance.playerInputManager.MoveInput;
            var direction = Mathf.Abs(moveInput.magnitude) > 0.01f
                ? moveInput.normalized
                : new Vector2(Player.Instance.spriteRenderer.flipX ? -1f : 1f, 0);

            var e = FindObjectsByType<Enemy.Enemy>(FindObjectsSortMode.None);
            return e.Where(x => !x.IsExecutable)
                .Where(x =>
                {
                    var toEnemy = (x.transform.position - Player.Instance.spriteRenderer.bounds.center).normalized;
                    var angle = Vector2.Angle(direction, toEnemy);
                    return angle <= 45f;
                })
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !IsStrongDashReady) return;

            var moveInput = Player.Instance.playerInputManager.MoveInput;
            var direction = Mathf.Abs(moveInput.magnitude) > 0.01f
                ? moveInput.normalized
                : new Vector2(Player.Instance.spriteRenderer.flipX ? -1f : 1f, 0);

            var origin = Player.Instance.spriteRenderer.bounds.center;

            Gizmos.color = Color.yellow;

            const float coneAngle = 45f;
            const int segments = 1;
            const float angleStep = coneAngle * 2f / segments;
            var baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            for (var i = 0; i <= segments; i++)
            {
                var angle = (baseAngle - coneAngle + angleStep * i) * Mathf.Deg2Rad;
                var coneDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                Gizmos.DrawRay(origin, coneDir * 7f);
            }

            var enemy = GetNearestNonExecutionTarget();
            if (enemy.Length <= 0) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, enemy[0].transform.position);
        }
    }
}