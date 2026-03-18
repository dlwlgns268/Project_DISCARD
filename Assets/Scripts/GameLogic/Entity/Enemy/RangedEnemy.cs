using UnityEngine;

public abstract class RangedEnemy : Enemy
{
    [SerializeField] protected float attackRange = 12f;
    [SerializeField] protected float attackDelay = 2f;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected EnemyProjectile projectilePrefab;

    protected float CurrentAttackDelay;

    protected virtual void FixedUpdate()
    {
        if (CurrentAttackDelay > 0f)
            CurrentAttackDelay -= Time.fixedDeltaTime;
    }

    protected bool CanAtk()
    {
        return CurrentAttackDelay <= 0f;
    }

    protected bool IsTargetInRange()
    {
        if (!Target) return false;
        return Vector2.Distance(transform.position, Target.position) <= attackRange;
    }

    protected void ResetAttackDelay()
    {
        CurrentAttackDelay = attackDelay;
    }

    protected virtual void FireProjectile()
    {
        if (!projectilePrefab || !firePoint || !Target)
            return;

        EnemyProjectile projectile = ObjectPoolManager.Instance.Spawn<EnemyProjectile>(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.Initialize(Target.position - firePoint.position, atkPower);
    }
}

