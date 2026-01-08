using System;
using System.Threading;
using System.Collections.Generic;

namespace MH.GameScene.Core.Entites
{
    public abstract class Entity : IEntity
    {
        private int _id;
        private bool _active;
        private bool _start;
        private World _world;
        private Entity _parent;
        private List<Entity> _children;
        private List<IComponent> _componentList;
        private Dictionary<Type, IComponent> _components;
        protected CancellationTokenSource _destroyTokenSource;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public int Id => _id;

        public IEntity Parent => _parent;

        public World World => _world;

        public Entity()
        {
            _components = new Dictionary<Type, IComponent>();
            _componentList = new List<IComponent>();
            _children = new List<Entity>();
        }

        public T GetCom<T>() where T : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }
            return default;
        }

        public T AddCom<T>(object data = null) where T : IComponent, new()
        {
            T com = _world._comPool.Require<T>();
            _components.Add(typeof(T), com);
            _componentList.Add(com);
            com.OnInit(this, data);
            return com;
        }

        public T FindCom<T>() where T : IComponent
        {
            Type findType = typeof(T);
            foreach (IComponent com in _componentList)
            {
                Type comType = com.GetType();
                if (findType.IsAssignableFrom(comType) || comType == findType)
                {
                    return (T)com;
                }
            }

            return default;
        }

        public void RemoveCom<T>() where T : IComponent
        {
            Type type = typeof(T);
            if (_components.TryGetValue(type, out IComponent component))
            {
                _componentList.Remove(component);
                _components.Remove(type);
                component.OnDestroy();
                _world._comPool.Release(component);
            }
        }

        public T FindEntity<T>() where T : IEntity
        {
            Type findType = typeof(T);
            for (int i = 0; i < _children.Count; i++)
            {
                IEntity entity = _children[i];
                if (entity != null)
                {
                    Type eType = entity.GetType();
                    if (findType.IsAssignableFrom(eType) || eType == findType)
                    {
                        return (T)entity;
                    }
                }
            }

            return default;
        }


        public void Init(World world, IEntity pEntity, int id, object data)
        {
            this._id = id;
            this._world = world;
            _parent = (Entity)pEntity;
            if (_parent != null)
                _parent._children.Add(this);
            _active = true;
            _destroyTokenSource = new CancellationTokenSource();
            OnInit(data);
        }

        public void Start()
        {
            if (_start)
                return;

            OnStart();

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Start();
            }

            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].OnStart();
            }
            _start = true;
        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (!_start) return;

            if (_componentList.Count <= 0)
                return;

            for (int i = 0; i < _children.Count; i++)
            {
                IEntity child = _children[i];
                child.OnUpdate(deltaTime);
            }

            for (int i = 0; i < _componentList.Count; i++)
            {
                IComponent component = _componentList[i];
                if (component.Active && component is IUpdate updater)
                {
                    updater.OnUpdate(deltaTime);
                }
            }
        }

        public void Destroy()
        {
            _start = false;
            OnDestroy();

            List<Entity> cache = new List<Entity>(_children);
            for (int i = cache.Count - 1; i >= 0; i--)
            {
                Entity child = cache[i];
                child.Destroy();
                _world._entityPool.Release(child);
            }

            for (int i = _componentList.Count - 1; i >= 0; i--)
            {
                IComponent component = _componentList[i];
                component.OnDestroy();
                _world._comPool.Release(component);
                _components.Remove(component.GetType());
            }

            if (_parent != null)
            {
                if (_parent._children.Contains(this))
                    _parent._children.Remove(this);
                _parent = null;
            }

            _children.Clear();
            _components.Clear();
            _componentList.Clear();
            _destroyTokenSource.Dispose();
        }

        protected virtual void OnInit(object data) { }
        protected virtual void OnStart() { }
        protected virtual void OnDestroy() { }
    }
}
