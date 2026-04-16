using UnityEngine;

namespace GameLogic.Entity.Enemy
{
    public class VernTurret : RangedEnemy
    {
        private bool _facingRight = true;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        
            if (!isSpawned || !target) return;
        
            var diffX = target.position.x - transform.position.x;
            UpdateFacing(diffX);

            if (IsTargetInRange() && CanAttack) Attack();
        }

        private void Attack()
        {
            ResetAttackDelay();
        
            //TODO 얘도 공격 모션이나 연출 넣기
            FireProjectile();
        }
    
        private void UpdateFacing(float diffX)
        {
            _facingRight = diffX switch
            {
                > 0.05f => true,
                < -0.05f => false,
                _ => _facingRight
            };

            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1f : -1f);
            transform.localScale = scale;
        }
    }
}
