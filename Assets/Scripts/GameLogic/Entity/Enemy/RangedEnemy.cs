using UnityEngine;
using Utils.ObjectPooling;

namespace GameLogic.Entity.Enemy
{
    public abstract class RangedEnemy : Enemy
    {
        [SerializeField] protected float attackRange = 12f;
        [SerializeField] protected float attackDelay = 2f;
        [SerializeField] protected Transform firePoint;
        [SerializeField] protected EnemyProjectile projectilePrefab;

        protected float currentAttackDelay;

        protected virtual void FixedUpdate()
        {
            if (currentAttackDelay > 0f) currentAttackDelay -= Time.fixedDeltaTime;
        }

        protected bool CanAttack => currentAttackDelay <= 0f;

        protected bool IsTargetInRange()
        {
            if (!target) return false;
            return Vector2.Distance(transform.position, target.position) <= attackRange;
        }

        protected void ResetAttackDelay()
        {
            currentAttackDelay = attackDelay;
        }

        protected virtual void FireProjectile()
        {
            if (!projectilePrefab || !firePoint || !target) return;
            var projectile = ObjectPoolManager.Instance.Spawn(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.Initialize(target.position - firePoint.position, atkPower);
        }
    }
}
