using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VERN_DRONE : RangedEnemy
{
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private bool _facingRight = true;
    private bool _isAttacking;
    private float _hoverSeed;
    private float _cachedGroundY;
    private float _lastGroundCheckTime;

    private static readonly Vector2 MaxSpeed = new Vector2(3.2f, 2.2f);
    private static readonly Vector2 Acceleration = new Vector2(12f, 8f);
    private static readonly Vector3 HeightRange = new Vector3(1.2f, 1.2f, 2.5f);
    private static readonly Vector2 FollowTolerance = new Vector2(0.35f, 0.2f);
    private static readonly Vector3 HoverConfig = new Vector3(0.25f, 0.18f, 1.8f);
    private static readonly Vector2 FireDelay = new Vector2(0.15f, 0.2f);

    private const float GroundCheckDistance = 10f;
    private const float GroundCheckInterval = 0.5f;
    private const float DescendSpeedMultiplier = 0.7f;
    private const int BurstCount = 3;
    private const float BurstInterval = 0.1f;

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
        _hoverSeed = Random.Range(0f, 10f);

        // 테스트 중이면 자동 활성화
        Spawn();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isSpawned || !Target)
            return;

        if (_isAttacking)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 playerPos = Target.position;

        float hoverPhase = (Time.time + _hoverSeed) * HoverConfig.z;
        Vector2 sway = new Vector2(
            Mathf.Sin(hoverPhase * 0.7f) * HoverConfig.x,
            Mathf.Sin(hoverPhase) * HoverConfig.y
        );

        float minAllowedY = GetMinimumAllowedY();
        Vector2 desired = new Vector2(
            playerPos.x + sway.x,
            Mathf.Clamp(
                playerPos.y + HeightRange.y + sway.y,
                minAllowedY,
                playerPos.y + HeightRange.z
            )
        );

        Vector2 diff = desired - currentPos;

        bool inAttackRange = IsTargetInRange();
        Vector2 targetVel = Vector2.zero;

        if (!inAttackRange)
        {
            if (Mathf.Abs(diff.x) > FollowTolerance.x)
                targetVel.x = Mathf.Clamp(diff.x, -1f, 1f) * MaxSpeed.x;

            if (diff.y > FollowTolerance.y)
                targetVel.y = Mathf.Clamp(diff.y, 0f, 1f) * MaxSpeed.y;
            else if (diff.y < -FollowTolerance.y)
                targetVel.y = -Mathf.Clamp(-diff.y, 0f, 1f) * MaxSpeed.y * DescendSpeedMultiplier;
        }

        if (currentPos.y < minAllowedY)
            targetVel.y = MaxSpeed.y;

        Vector2 nextVel = new Vector2(
            Mathf.MoveTowards(_rb.linearVelocity.x, targetVel.x, Acceleration.x * Time.fixedDeltaTime),
            Mathf.MoveTowards(_rb.linearVelocity.y, targetVel.y, Acceleration.y * Time.fixedDeltaTime)
        );

        _rb.linearVelocity = nextVel;
        UpdateFacing(nextVel.x, diff.x);

        if (inAttackRange && CanAtk())
        {
            Atk();
        }
    }

    private void Atk()
    {
        ResetAttackDelay();
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(FireDelay.x);

        for (int i = 0; i < BurstCount; i++)
        {
            FireProjectile();
            if (i < BurstCount - 1)
                yield return new WaitForSeconds(BurstInterval);
        }

        yield return new WaitForSeconds(FireDelay.y);
        _isAttacking = false;
    }

    private float GetMinimumAllowedY()
    {
        if (Time.time - _lastGroundCheckTime >= GroundCheckInterval)
        {
            _lastGroundCheckTime = Time.time;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDistance, groundLayer);
            _cachedGroundY = hit.collider ? hit.point.y + HeightRange.x : transform.position.y - 100f;
        }
        return _cachedGroundY;
    }

    private void UpdateFacing(float xVelocity, float rawDiffX)
    {
        bool newFacing = Mathf.Abs(xVelocity) > 0.05f
            ? xVelocity > 0
            : Mathf.Abs(rawDiffX) > FollowTolerance.x && rawDiffX > 0;

        if (newFacing != _facingRight)
        {
            _facingRight = newFacing;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1f : -1f);
            transform.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * GroundCheckDistance);
    }
}