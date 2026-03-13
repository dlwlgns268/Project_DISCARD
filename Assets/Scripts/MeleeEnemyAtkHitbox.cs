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
        
        Debug.Log("Hit!");
        
        _hitTargets.Add(other);

        /*TODO PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(_damage);
        }*/
    }
}
