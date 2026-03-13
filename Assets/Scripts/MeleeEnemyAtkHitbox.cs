using System.Collections.Generic;
using UnityEngine;

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
        if (_hitTargets.Contains(other)) return;

        _hitTargets.Add(other);

        //TODO other.GetComponent<플레이어 체력 스크립트>().TakeDamage(_damage);
    }
}
