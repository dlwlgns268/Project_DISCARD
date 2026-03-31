using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly IPoolable _prefab;
    private readonly Transform _parent;
    private readonly Stack<IPoolable> _inactiveObjects = new();

    public ObjectPool(IPoolable prefab, Transform parent, int initialSize)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private IPoolable CreateNewObject()
    {
        IPoolable obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        obj.SetOriginPool(this);
        _inactiveObjects.Push(obj);
        return obj;
    }

    public T Get<T>(Vector3 position, Quaternion rotation) where T : IPoolable
    {
        if (_inactiveObjects.Count == 0)
        {
            CreateNewObject();
        }

        IPoolable obj = _inactiveObjects.Pop();
        Transform tr = obj.transform;
        tr.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        obj.OnSpawn();

        return obj as T;
    }

    public void Return(IPoolable obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        _inactiveObjects.Push(obj);
    }
}