using UnityEngine;

namespace GameLogic.Entity.Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected float enemyHp;
        [SerializeField] protected float atkPower;
        [SerializeField] protected float moveSpeed;
        [SerializeField] [Range(0f, 1f)] protected float executionThreshold = 0.5f;
    
        protected Transform target;
        [SerializeField] protected float currentEnemyHp;

        public bool isSpawned = false;

        public bool IsExecutable => currentEnemyHp <= enemyHp * executionThreshold;

        protected virtual void Start()
        {
            currentEnemyHp = enemyHp;
        
            if (Player.Player.Instance) target = Player.Player.Instance.transform;
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
            // TODO 스폰 시 실행할 로직 (애니메이션, 사운드 등)
        }
    
        public virtual void TakeDamage(float damage)
        {
            if ((currentEnemyHp -= damage) <= 0f) Die();
        }
        
        public virtual void Die()
        {
            Destroy(gameObject);
        }

        public virtual void Execute()
        {
            if (IsExecutable) Die();
        }
    }
}