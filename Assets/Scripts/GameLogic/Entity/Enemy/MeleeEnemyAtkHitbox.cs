using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Entity.Enemy
{
    public class EnemyAttackHitbox : MonoBehaviour
    {
        private float _damage;
        private bool _isActive;
        private readonly HashSet<Collider2D> _hitTargets = new();

        public void Activate(float damage)
        {
            _damage = damage;
            _isActive = true;
            _hitTargets.Clear();
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            _isActive = false;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isActive) return;
            if (!other.CompareTag("Player")) return;
            if (!_hitTargets.Add(other)) return;
            Debug.Log("Hit!");

            /* TODO var playerHealth = Player.Instance.playerHealth;
        if (playerHealth != null) playerHealth.TakeDamage(_damage);
        */
        }
    }
}
