using System.Collections.Generic;
using UnityEngine;

namespace Utils.ObjectPooling
{
    public class ObjectPool
    {
        private readonly Poolable _prefab;
        private readonly Transform _parent;
        private readonly Queue<Poolable> _inactiveObjects = new();

        public ObjectPool(Poolable prefab, Transform parent, int initialSize)
        {
            _prefab = prefab;
            _parent = parent;
            for (var i = 0; i < initialSize; i++) CreateNewObject();
        }

        private void CreateNewObject()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            obj.SetOriginPool(this);
            _inactiveObjects.Enqueue(obj);
        }

        public Poolable Get(Vector3 position, Quaternion rotation)
        {
            if (_inactiveObjects.Count == 0) CreateNewObject();

            var obj = _inactiveObjects.Dequeue();
            var tr = obj.transform;
            tr.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
            obj.OnSpawn();

            return obj;
        }

        public void Return(Poolable obj)
        {
            obj.OnDespawn();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parent);
            _inactiveObjects.Enqueue(obj);
        }
    }
}