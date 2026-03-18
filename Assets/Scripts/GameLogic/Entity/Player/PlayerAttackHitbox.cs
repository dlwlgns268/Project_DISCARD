using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private readonly float _attackDamage = 25f;
    private readonly Vector2 _offset = new Vector2(0.1f, 0f);

    private readonly HashSet<Collider2D> _hitTargets = new();
    private Transform _playerTransform;

    private void Awake()
    {
        _playerTransform = transform.parent;
    }

    public void ResetHitTargets()
    {
        _hitTargets.Clear();
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!_playerTransform)
        {
            return;
        }

        SpriteRenderer spriteRenderer = _playerTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            float direction = spriteRenderer.flipX ? -1f : 1f;
            transform.localPosition = new Vector3(_offset.x * direction, _offset.y, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        if (_hitTargets.Contains(other))
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (!enemy) return;
        

        _hitTargets.Add(other);
       
        enemy.TakeDamage(_attackDamage);
    }
}