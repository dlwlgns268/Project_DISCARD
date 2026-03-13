using UnityEngine;
using UnityEngine.Serialization;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyHp;
    [SerializeField] protected float atkPower;
    [SerializeField] protected float moveSpeed;
    [SerializeField] [Range(0f, 1f)] protected float executionThreshold = 0.5f;
    
    protected Transform Target;
    protected float CurrentEnemyHp;

    public bool isSpawned = false;

    public bool IsExecutable => CurrentEnemyHp <= enemyHp * executionThreshold;

    protected virtual void Start()
    {
        CurrentEnemyHp = enemyHp;
        
        if (Player.Instance)
        {
            Target = Player.Instance.transform;
        }
        
        Spawn(); //방 시스템 제작시 삭제해야함
    }

    public virtual void Spawn()
    {
        if (isSpawned) return;
        
        isSpawned = true;
        gameObject.SetActive(true);
        OnSpawned();
    }

    protected virtual void OnSpawned()
    {
        //TODO 스폰 시 실행할 로직 (애니메이션, 사운드 등)
    }
    
    public virtual void TakeDamage(float damage)
    {
        CurrentEnemyHp -= damage;

        if (CurrentEnemyHp <= 0f)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void Execute()
    {
        if (IsExecutable)
        {
            Die();
        }
    }
}