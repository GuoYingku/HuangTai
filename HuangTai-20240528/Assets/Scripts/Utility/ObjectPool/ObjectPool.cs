using System.Collections.Generic;
using UnityEngine;
namespace Utility.ObjectPool
{
    public class ObjectPool
    {
        private List<GameObject> _usedPool;
        private Stack<GameObject> _waitPool;
        private GameObject _poolPrefab;
        private Transform _poolParent;
        string name;
        public ObjectPool(string poolName, GameObject prefab, int poolSize, GameObject poolParent = null, bool preload = true)
        {
            if (poolSize == 0 || prefab == null)
            {
                throw new System.Exception("参数错误。");
            }
            _usedPool = new List<GameObject>();
            _waitPool = new Stack<GameObject>();
            _poolPrefab = prefab;
            name = poolName;

            if (poolParent == null)
            {
                poolParent = new GameObject(name + "PoolParent");
            }

            _poolParent = poolParent.transform;

            if (preload)
            {
                for (int i = 0; i < poolSize; ++i)
                {
                    GameObject g = GameObject.Instantiate(_poolPrefab);
                    g.transform.SetParent(this._poolParent);
                    g.SetActive(false);
                    _waitPool.Push(g);
                }
            }
            else
            {
                for (int i = 0; i < poolSize; ++i)
                {
                    _waitPool.Push(null);
                }
            }
        }
        public GameObject GetFreeObject()
        {
            if (_waitPool.Count == 0)
            {
                Debug.LogError("对象池已满。(" + name + ")");
                return null;
            }
            GameObject g = _waitPool.Pop();
            if (g == null)
            {
                g = GameObject.Instantiate(_poolPrefab);
                g.transform.SetParent(_poolParent);
            }
            _usedPool.Add(g);
            g.SetActive(true);
            return g;
        }
        public void FreeObject(ref GameObject g)
        {
            if (!_usedPool.Contains(g))
            {
                Debug.LogError("未找到对象。(" + name + ")");
            }
            g.SetActive(false);
            g.transform.SetParent(_poolParent);
            _usedPool.Remove(g);
            _waitPool.Push(g);
            g = null;
        }
        public void FreeAll()
        {
            for(int i=0;i<_usedPool.Count;++i)
            {
                _usedPool[i].SetActive(false);
                _usedPool[i].transform.SetParent(_poolParent);
                _waitPool.Push(_usedPool[i]);
            }
            _usedPool.Clear();
        }
    }
}