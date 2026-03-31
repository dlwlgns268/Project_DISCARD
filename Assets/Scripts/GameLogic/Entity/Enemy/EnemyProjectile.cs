using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : IPoolable
{
    private float speed = 12f;
    private float lifeTime = 1.25f;

    private Rigidbody2D _rb;
    private float _damage;
    private float _lifeTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float damage)
    {
        _damage = damage;
        _lifeTimer = 0f;
        _rb.linearVelocity = direction.normalized * speed;
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    public override void OnSpawn()
    {
        _lifeTimer = 0f;
    }

    public override void OnDespawn()
    {
        _rb.linearVelocity = Vector2.zero;
        _damage = 0f;
        _lifeTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO 플레이어 데미지 처리
            ReturnToPool();
            return;
        }

        if (other.CompareTag("Ground"))
        {
            ReturnToPool();
        }
    }
}