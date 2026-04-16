using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Entity.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class DashHitbox : MonoBehaviour
    {
        private const float Damage = 60f;
        private readonly HashSet<Enemy.Enemy> _hitTargets = new();
        public Collider2D selfCollider;
        public bool IsStrongDash { get; set; }

        public void ResetHitTargets()
        {
            _hitTargets.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponentInParent<Enemy.Enemy>();
            if (!enemy) return;
            Player.Instance.playerDash.IsDashAvailable = true;
            if (!_hitTargets.Add(enemy)) return;
            Player.Instance.playerDash.StrongDashCount++;
            if (!IsStrongDash || enemy.IsExecutable) return;
            enemy.TakeDamage(Damage);
        }
    }
}