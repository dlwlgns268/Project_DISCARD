using UnityEngine;

namespace Utils.ObjectPooling
{
    public abstract class Poolable : MonoBehaviour
    {
        private ObjectPool _originPool;

        public void SetOriginPool(ObjectPool pool)
        {
            _originPool = pool;
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDespawn()
        {
        }

        public void ReturnToPool()
        {
            if (_originPool == null)
            {
                Destroy(gameObject);
                return;
            }

            _originPool.Return(this);
        }
    }
}