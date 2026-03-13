using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D _rb;
    private float _damage;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float damage)
    {
        _damage = damage;
        _rb.linearVelocity = direction.normalized * speed;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO 플레이어 데미지 처리
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
