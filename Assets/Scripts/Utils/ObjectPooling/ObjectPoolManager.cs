using System.Collections.Generic;
using UnityEngine;

namespace Utils.ObjectPooling
{
    public class ObjectPoolManager : SingleMono<ObjectPoolManager>
    {
        private readonly Dictionary<Poolable, ObjectPool> _pools = new();
        private readonly Dictionary<string, Poolable> _registeredPrefabs = new();

        private Transform _poolRoot;

        public override void Awake()
        {
            base.Awake();
            _poolRoot = new GameObject("PoolRoot").transform;
            _poolRoot.SetParent(transform);
        }

        private ObjectPool Register(Poolable prefab, int initialSize)
        {
            if (_pools.TryGetValue(prefab, out var pool)) return pool;
            
            var poolGroup = new GameObject(prefab.name + "_Pool");
            poolGroup.transform.SetParent(_poolRoot);
            pool = new ObjectPool(prefab, poolGroup.transform, initialSize);
            _pools[prefab] = pool;
            _registeredPrefabs[prefab.name] = prefab;
            return pool;
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Poolable
        {
            if (_pools.TryGetValue(prefab, out var pool)) return pool.Get(position, rotation) as T;
            pool = Register(prefab, 1);

            return pool.Get(position, rotation) as T;
        }

        public T Spawn<T>(string prefabName, Vector3 position, Quaternion rotation) where T : Poolable
        {
            return !_registeredPrefabs.TryGetValue(prefabName, out var prefab) ? null : Spawn(prefab as T, position, rotation);
        }
    }
}