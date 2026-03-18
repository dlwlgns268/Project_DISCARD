using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private readonly Dictionary<IPoolable, ObjectPool> _pools = new();
    private readonly Dictionary<string, IPoolable> _registeredPrefabs = new();

    private Transform _poolRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _poolRoot = new GameObject("PoolRoot").transform;
        _poolRoot.SetParent(transform);
    }

    public void Register(IPoolable prefab, int initialSize)
    {
        if (_pools.ContainsKey(prefab))
            return;

        GameObject poolGroup = new GameObject(prefab.name + "_Pool");
        poolGroup.transform.SetParent(_poolRoot);

        ObjectPool pool = new ObjectPool(prefab, poolGroup.transform, initialSize);
        _pools.Add(prefab, pool);
        _registeredPrefabs.Add(prefab.name, prefab);
    }

    public T Spawn<T>(IPoolable prefab, Vector3 position, Quaternion rotation) where T : IPoolable
    {
        if (!_pools.TryGetValue(prefab, out ObjectPool pool))
        {
            Register(prefab, 1);
            pool = _pools[prefab];
        }

        return pool.Get<T>(position, rotation);
    }

    public T Spawn<T>(string prefabName, Vector3 position, Quaternion rotation) where T : IPoolable
    {
        if (!_registeredPrefabs.TryGetValue(prefabName, out IPoolable prefab))
            return null;

        return Spawn<T>(prefab, position, rotation);
    }
}