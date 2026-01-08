
using System;
using System.Collections.Generic;

namespace MH.GameScene.Core
{
    internal class ObjectPool<T>
    {
        private int _defaultCapacity;
        private Dictionary<Type, Queue<T>> _cache;

        public ObjectPool(int defaultCapacity = 32)
        {
            _defaultCapacity = defaultCapacity;
            _cache = new Dictionary<Type, Queue<T>>();
        }

        public int Count<SUBT>() where SUBT : T
        {
            Type type = typeof(SUBT);
            if (_cache.TryGetValue(type, out Queue<T> queue))
                return queue.Count;
            return 0;
        }

        public SUBT Require<SUBT>() where SUBT : T, new()
        {
            Type type = typeof(SUBT);
            if (!_cache.TryGetValue(type, out Queue<T> queue))
            {
                queue = new Queue<T>(_defaultCapacity);
                _cache[type] = queue;
            }

            if (queue.Count == 0)
            {
                return new SUBT();
            }
            else
            {
                return (SUBT)queue.Dequeue();
            }
        }

        public void Release(T obj)
        {
            Type type = obj.GetType();
            if (!_cache.TryGetValue(type, out Queue<T> queue))
            {
                queue = new Queue<T>(_defaultCapacity);
                _cache[type] = queue;
            }
            queue.Enqueue(obj);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
