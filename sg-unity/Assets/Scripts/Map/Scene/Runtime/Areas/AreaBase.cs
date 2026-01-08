using System;
using UnityEngine;
using MM.MapEditors;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Entities
{
    public class AreaBase : Entity, IArea
    {
        private int _areaId;
        private Action _gridChangeEvent;
        private IAreaModule _areaModule;
        private Vector2 _innerCenter;
        private Dictionary<Vector2Int, IGridEntity> _grids;

        public int AreaId => _areaId;

        public IReadOnlyCollection<IGridEntity> Grids => _grids.Values;

        public Vector2 Center
        {
            get => _innerCenter;
            set => _innerCenter = value;
        }

        public event Action GridChangeEvnet
        {
            add { _gridChangeEvent += value; }
            remove { _gridChangeEvent -= value; }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _areaModule = (IAreaModule)Parent;
            _areaId = (int)data;
            _grids = new Dictionary<Vector2Int, IGridEntity>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (Vector2Int index in _grids.Keys)
                _areaModule.UnRegister(index);
            _grids = null;
            _areaModule = null;
            _gridChangeEvent = null;
        }

        public void Add(IEnumerable<IGridEntity> list)
        {
            foreach (var grid in list)
            {
                Vector2Int index = grid.Index;
                if (!_grids.ContainsKey(index))
                {
                    if (_areaModule.Register(index))
                    {
                        _grids[grid.Index] = grid;
                    }
                }
            }
            _gridChangeEvent?.Invoke();
        }

        public void Add(IGridEntity grid)
        {
            Vector2Int index = grid.Index;
            if (!_grids.ContainsKey(index))
            {
                if (_areaModule.Register(index))
                {
                    _grids[grid.Index] = grid;
                    _gridChangeEvent?.Invoke();
                }
            }
        }

        public void Remove(IGridEntity grid)
        {
            Vector2Int index = grid.Index;
            if (_grids.ContainsKey(index))
            {
                _grids.Remove(index);
                _areaModule.UnRegister(index);
                _gridChangeEvent?.Invoke();
            }
        }

        public void CalculateCenterPos()
        {
            List<Vector2> points = new List<Vector2>();
            foreach (Vector2Int index in _grids.Keys)
            {
                points.Add(MathUtility.IndexToGamePos(index));
            }
            _innerCenter = PointsUtility.GetCenter(points);
        }
    }
}
