using System.Collections.Generic;
using UnityEngine;

public class StrongDashHitbox : MonoBehaviour
{
    private const float Damage = 60f;
    private readonly HashSet<Enemy> _hitTargets = new();

    public void ResetHitTargets()
    {
        _hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (!enemy)
        {
            return;
        }

        if (_hitTargets.Contains(enemy))
        {
            return;
        }

        _hitTargets.Add(enemy);
        enemy.TakeDamage(Damage);
    }
}