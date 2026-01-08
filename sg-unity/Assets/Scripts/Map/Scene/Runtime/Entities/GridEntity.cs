using UnityEngine;
using MH.GameScene.Datas;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class GridEntity : Entity, IGridEntity
    {
        private GridData _data;
        private Dictionary<string, IItemEntity> _items;
        private IMapScene _scene;

        public Vector2Int Index { get; private set; }

        public Dictionary<string, ItemData> Objects => _data.Objects;

        public int LayerCount => _items.Count;

        public IMapScene Scene => _scene;

        public IReadOnlyCollection<IItemEntity> Items => _items.Values;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _scene = (IMapScene)Parent;
            Index = (Vector2Int)data;
            _items = new Dictionary<string, IItemEntity>();

            AddCom<GridViewCom>();
        }

        public bool HasItem(string layer)
        {
            return _items.ContainsKey(layer);
        }

        public bool InGrid(IItemEntity item)
        {
            return GetItem(item.Layer) == item;
        }

        public IItemEntity GetItem(string layer)
        {
            if (_items.TryGetValue(layer, out IItemEntity item))
            {
                return item;
            }
            return null;
        }

        internal void SetItem(IItemEntity item, IGridEntity mainGrid)
        {
            _items[item.Layer] = item;
            ItemBase itemBase = (ItemBase)item;
            itemBase.SetGrid(this, mainGrid);
            item.Start();
        }

        internal void RemoveItem(string layer)
        {
            if (_items.TryGetValue(layer, out IItemEntity item))
            {
                _items.Remove(layer);
            }
        }
    }
}
