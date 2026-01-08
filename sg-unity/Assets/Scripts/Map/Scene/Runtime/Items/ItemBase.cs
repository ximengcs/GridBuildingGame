using System;
using UnityEngine;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Entities
{
    public abstract class ItemBase : Entity, IItemEntity
    {
        private int _itemId;
        private string _layer;
        private int _direction;
        private Vector2Int _size;
        private IGridEntity _mainGrid;
        private List<IGridEntity> _grids;
        private Action _gridChangeEvent;

        public int ItemId => _itemId;

        public string Layer => _layer;

        public Vector2Int Size => _size;

        public int Direction => _direction;

        public IReadOnlyCollection<IGridEntity> Grids => _grids;

        public IGridEntity MainGrid => _mainGrid;

        public event Action GridChangeEvent
        {
            add { _gridChangeEvent += value; }
            remove { _gridChangeEvent -= value; }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            ItemGenParam param = (ItemGenParam)data;
            _itemId = param.ItemId;
            _layer = param.Layer;
            _size = param.Size;
            _direction = param.Direction;
            _grids = new List<IGridEntity>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearGrid();
        }

        internal void SetGrid(IGridEntity grid, IGridEntity mainGrid)
        {
            _grids.Add(grid);
            _mainGrid = mainGrid;
        }

        internal void TriggerChange()
        {
            OnGridChange();
            _gridChangeEvent?.Invoke();
        }

        protected virtual void OnGridChange()
        {

        }

        internal void ClearGrid()
        {
            _grids.Clear();
        }

        internal void SetDirection(int direction)
        {
            _direction = direction;
        }
    }
}
