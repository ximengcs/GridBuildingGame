using UnityEngine;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Entities
{
    public class AreaModule<T> : Entity, IAreaModule where T : IArea, new()
    {
        private Dictionary<int, IArea> _areas;
        private HashSet<Vector2Int> _indexes;

        public IReadOnlyCollection<IArea> Areas => _areas.Values;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _areas = new Dictionary<int, IArea>();
            _indexes = new HashSet<Vector2Int>();
        }

        public bool Register(Vector2Int index)
        {
            if (!_indexes.Contains(index))
            {
                _indexes.Add(index);
                return true;
            }
            return false;
        }

        public void UnRegister(Vector2Int index)
        {
            if (_indexes.Contains(index))
                _indexes.Remove(index);
        }

        public bool Contains(int areaId)
        {
            return _areas.ContainsKey(areaId);
        }

        public IArea GetArea(int id)
        {
            if (_areas.TryGetValue(id, out IArea area))
                return area;
            return null;
        }

        public IArea AddArea(int id)
        {
            if (_areas.TryGetValue(id, out IArea area))
                return area;

            area = this.AddEntity<T>(id);
            _areas.Add(id, area);
            area.Start();
            return area;
        }

        public void RemoveArea(int id)
        {
            if (_areas.TryGetValue(id, out IArea area))
            {
                AreaBase mapArea = area as AreaBase;
                mapArea.Destroy();
                _areas.Remove(id);
            }
        }
    }
}
