using System;
using UnityEngine;
using System.Collections.Generic;
using UI.UIScenes;

namespace MH.GameScene.Core.Entites
{
    public sealed partial class World
    {
        private IDGenerator _IDGen;
        private Dictionary<int, IEntity> _entities;
        private List<IEntity> _entitiesList;
        private GameObject _sceneObj;

        internal ObjectPool<IEntity> _entityPool;
        internal ObjectPool<IComponent> _comPool;

        public IResourceModule Resource { get; private set; }

        public UIScene UIScene { get; private set; }

        private World() { }

        public T AddEntity<T>(object data = null) where T : IEntity, new()
        {
            return AddEntity<T>(null, data);
        }

        public T AddEntity<T>(IEntity parent, object data = null) where T : IEntity, new()
        {
            T entity = _entityPool.Require<T>();
            int entityId = _IDGen.Next;
            if (parent == null)
            {
                _entitiesList.Add(entity);
                _entities.Add(entityId, entity);
            }

            entity.Init(this, parent, entityId, data);
            if (parent == null)
                entity.Start();
            return entity;
        }

        public void RemoveEntity(IEntity entity)
        {
            if (entity == null)
                return;

            if (_entities.ContainsKey(entity.Id))
            {
                _entitiesList.Remove(entity);
                _entities.Remove(entity.Id);

            }

            entity.Destroy();
            _entityPool.Release(entity);
        }

        public T FindEntity<T>() where T : IEntity
        {
            Type findType = typeof(T);
            foreach (IEntity e in _entitiesList)
            {
                Type eType = e.GetType();
                if (findType.IsAssignableFrom(eType) || eType == findType)
                {
                    return (T)e;
                }
            }

            return default;
        }

        public T GetEntity<T>(int entityId = default) where T : IEntity
        {
            if (entityId == default)
            {
                foreach (IEntity e in _entitiesList)
                {
                    if (e.GetType() == typeof(T))
                        return (T)e;
                }
            }

            if (_entities.TryGetValue(entityId, out var entity))
                return (T)entity;
            return default;
        }

        public void Initialize()
        {
            _IDGen = new IDGenerator(Const.ID_START);
            _entities = new Dictionary<int, IEntity>();
            _entityPool = new ObjectPool<IEntity>();
            _comPool = new ObjectPool<IComponent>();
            _entitiesList = new List<IEntity>();
        }

        public void Update(float deltaTime)
        {
            if (_entitiesList.Count <= 0)
                return;

            for (int i = _entitiesList.Count - 1; i >= 0; i--)
            {
                IEntity entity = _entitiesList[i];
                if (entity.Active)
                    entity.OnUpdate(deltaTime);
            }
        }

        public void Destroy()
        {
            foreach (Entity entity in _entitiesList)
            {
                entity.Destroy();
            }
            GameObject.Destroy(_sceneObj);
            Resource.Dispose();
            _entitiesList.Clear();
            _entities.Clear();
            _entityPool.Clear();
            _IDGen.Dispose();
            _entitiesList = null;
            _entities = null;
            _entityPool = null;
            _IDGen = null;
            Resource = null;
            _sceneObj = null;
        }
    }
}
