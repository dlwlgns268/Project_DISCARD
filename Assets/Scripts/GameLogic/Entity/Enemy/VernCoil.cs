using UnityEngine;

namespace GameLogic.Entity.Enemy
{
    public class VernCoil : MeleeEnemy
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D attackPoint;
        private bool _facingRight = true;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isSpawned || !target) return;

            var diffX = target.position.x - transform.position.x;
            UpdateFacing(diffX);

            if (IsTargetInRange())
            {
                rb.linearVelocityX = 0f;
                if (CanAtk()) Attack();
            }
            else
            {
                var dirX = Mathf.Abs(diffX) < 1f ? diffX : Mathf.Sign(diffX);
                rb.linearVelocityX = dirX * moveSpeed;
            }
        }

        protected override void Attack()
        {
            ResetAtkDelay();

            // TODO animator.SetTrigger("Attack");
            var hitColliders = Physics2D.OverlapBox(attackPoint.transform.position, attackPoint.size, 0f, LayerMask.GetMask("Player"));
            if (!hitColliders) return;
            /* TODO var playerHealth = Player.Instance.playerHealth;
            if (playerHealth != null) playerHealth.TakeDamage(_damage);*/
        }

        private void UpdateFacing(float diffX)
        {
            _facingRight = diffX switch
            {
                > 0.05f => false,
                < -0.05f => true,
                _ => _facingRight
            };

            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1f : -1f);
            transform.localScale = scale;
        }
    }
}