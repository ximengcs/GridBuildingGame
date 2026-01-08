using System;
using UnityEngine;
using MH.GameScene.Configs;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Utilities;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Entities
{
    public abstract class MapScene : Entity, IMapScene
    {
        private bool _inited;
        private Dictionary<Vector2Int, GridEntity> _grids;
        private Dictionary<Type, List<IItemEntity>> _items;

        private Action _initFinishEvent;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _grids = new Dictionary<Vector2Int, GridEntity>();
            _items = new Dictionary<Type, List<IItemEntity>>();

            OnSceneInit(data);

            _initFinishEvent?.Invoke();
            _initFinishEvent = null;
            _inited = true;
        }

        protected virtual void OnSceneInit(object data) { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _grids = null;
        }

        public IReadOnlyCollection<IGridEntity> Grids
        {
            get
            {
                return new List<IGridEntity>(_grids.Values);
            }
        }

        public void RegisterInitFinish(Action handler)
        {
            if (_inited)
                handler();
            else
                _initFinishEvent += handler;
        }

        public bool HasGrid(Vector2Int index)
        {
            return _grids.ContainsKey(index);
        }

        public void GetAdjacentGrid(Vector2Int index, ICollection<object> result, Func<IGridEntity, bool> filter = null)
        {
            IGridEntity grid = GetGrid(new Vector2Int(index.x - 1, index.y));
            if (grid != null && !result.Contains(grid) && (filter == null || filter(grid))) result.Add(grid);
            grid = GetGrid(new Vector2Int(index.x + 1, index.y));
            if (grid != null && !result.Contains(grid) && (filter == null || filter(grid))) result.Add(grid);
            grid = GetGrid(new Vector2Int(index.x, index.y - 1));
            if (grid != null && !result.Contains(grid) && (filter == null || filter(grid))) result.Add(grid);
            grid = GetGrid(new Vector2Int(index.x, index.y + 1));
            if (grid != null && !result.Contains(grid) && (filter == null || filter(grid))) result.Add(grid);
        }

        public IGridEntity GetGrid(Vector2Int index)
        {
            if (_grids.TryGetValue(index, out GridEntity entity))
            {
                return entity;
            }
            return default;
        }

        public IGridEntity AddGrid(Vector2Int index)
        {
            if (_grids.TryGetValue(index, out GridEntity entity))
                return entity;

            entity = this.AddEntity<GridEntity>(index);
            _grids.Add(entity.Index, entity);
            return entity;
        }

        public ItemSetResult CheckItemSet(ItemGenParam param, bool checkSelf = false, Vector2Int index = default, GridCheckFilter filter = null)
        {
            ItemSetResult check = new ItemSetResult();
            IItemEntity targetItem = null;

            if (checkSelf)
            {
                IGridEntity targetGrid = GetGrid(index);
                if (targetGrid != null)
                {
                    targetItem = targetGrid.GetItem(param.Layer);
                    if (targetItem != null && param.Equals(targetItem))
                    {
                        check.Type = ItemSetResultType.Self;
                        return check;
                    }
                }
            }

            // 获取item配置
            ItemConfig config = World.Resource.GetConfig<ItemConfig>(param.ItemId);

            MathUtility.GetDirectionRange(param.Index, param.Size, param.Direction,
                out Vector2Int xRange, out Vector2Int yRange);

            check.Type = ItemSetResultType.Ok;
            HashSet<IItemEntity> sortList = new HashSet<IItemEntity>();
            List<IItemEntity> cacheList = null;

            // 检查size
            for (int y = yRange.x; y < yRange.y; y++)
            {
                for (int x = xRange.x; x < xRange.y; x++)
                {
                    Vector2Int checkIndex = new Vector2Int(x, y);
                    IGridEntity grid = GetGrid(checkIndex);
                    if (grid == null)
                    {
                        if (check.Type == ItemSetResultType.Ok || check.Type == ItemSetResultType.SelfItemOk)
                            check.Type = ItemSetResultType.NoGrid;
                        check.Add(ItemSetResultType.NoGrid, checkIndex);
                        continue;
                    }

                    if (filter == null)
                    {
                        IItemEntity item = grid.GetItem(param.Layer);
                        ItemSetResultType checkResult = CheckItemSetState(item, targetItem, param, sortList);
                        if (checkResult != ItemSetResultType.Ok)
                        {
                            if (check.Type == ItemSetResultType.Ok || check.Type == ItemSetResultType.NoGrid)
                                check.Type = checkResult;
                            check.Add(checkResult, checkIndex);
                        }
                    }
                    else
                    {
                        if (cacheList == null)
                            cacheList = new List<IItemEntity>(4);

                        cacheList.Clear();
                        filter(targetItem, grid, param, cacheList);
                        foreach (IItemEntity item in cacheList)
                        {
                            ItemSetResultType checkResult = CheckItemSetState(item, targetItem, param, sortList);
                            if (checkResult != ItemSetResultType.Ok)
                            {
                                if (check.Type == ItemSetResultType.Ok || check.Type == ItemSetResultType.NoGrid)
                                    check.Type = checkResult;
                                check.Add(checkResult, checkIndex);
                            }
                        }
                    }
                }
            }

            if (sortList.Count > 1)
                check.Type = ItemSetResultType.MultiItem;

            return check;
        }

        private ItemSetResultType CheckItemSetState(IItemEntity checkItem, IItemEntity originTargetItem, ItemGenParam param, HashSet<IItemEntity> sortList)
        {
            ItemSetResultType result = ItemSetResultType.Ok;

            if (checkItem != null)
            {
                if (param.Layer == checkItem.Layer && !sortList.Contains(checkItem))
                {
                    sortList.Add(checkItem);
                }

                if (checkItem.ItemId == param.ItemId)
                {
                    if (originTargetItem != null && originTargetItem == checkItem)
                        result = ItemSetResultType.SelfItemOk;
                    else
                        result = ItemSetResultType.SameTypeItem;
                }
                else
                {
                    result = ItemSetResultType.DiffTypeItem;
                }
            }
            return result;
        }

        public void EnsureGrid(ItemGenParam param)
        {
            MathUtility.GetDirectionRange(param.Index, param.Size, param.Direction,
                        out Vector2Int xRange, out Vector2Int yRange);

            for (int y = yRange.x; y < yRange.y; y++)
            {
                for (int x = xRange.x; x < xRange.y; x++)
                {
                    Vector2Int gridIndex = new Vector2Int(x, y);
                    if (!HasGrid(gridIndex))
                    {
                        AddGrid(gridIndex);
                    }
                }
            }
        }

        public void RemoveItem(Vector2Int index, string layer)
        {
            IGridEntity grid = GetGrid(index);
            if (grid != null)
            {
                IItemEntity item = grid.GetItem(layer);
                if (item != null)
                {
                    foreach (GridEntity bindGrid in item.Grids)
                    {
                        bindGrid.RemoveItem(item.Layer);
                    }
                    World.RemoveEntity(item);

                    Type itemType = item.GetType();
                    if (!_items.TryGetValue(itemType, out List<IItemEntity> itemEntities))
                    {
                        itemEntities = new List<IItemEntity>();
                        _items.Add(itemType, itemEntities);
                    }
                    itemEntities.Remove(item);
                }
            }
        }

        public T GetFirstItem<T>() where T : IItemEntity
        {
            if (_items.TryGetValue(typeof(T), out List<IItemEntity> entities))
            {
                if (entities.Count > 0)
                    return (T)entities[0];
            }

            return default;
        }

        public IReadOnlyCollection<T> GetItems<T>() where T : IItemEntity
        {
            if (_items.TryGetValue(typeof(T), out List<IItemEntity> entities))
            {
                List<T> result = new List<T>();
                foreach (IItemEntity item in entities)
                    result.Add((T)item);
                return result;
            }
            return null;
        }

        public void MoveItem(IItemEntity item, ItemGenParam param, bool ensureGrid)
        {
            if (ensureGrid)
                EnsureGrid(param);

            foreach (GridEntity bindGrid in item.Grids)
                bindGrid.RemoveItem(item.Layer);
            ItemBase itemBase = (ItemBase)item;
            itemBase.ClearGrid();

            IGridEntity grid = GetGrid(param.Index);
            BindItemToGrid(item, grid, param);
            itemBase.TriggerChange();
        }

        public IItemEntity SetItem(ItemGenParam param, bool ensureGrid)
        {
            if (ensureGrid)
                EnsureGrid(param);

            RemoveItem(param.Index, param.Layer);

            IGridEntity grid = GetGrid(param.Index);
            IItemEntity item = grid.GenerateItem(param);
            if (item != null)
            {
                BindItemToGrid(item, grid, param);

                Type itemType = item.GetType();
                if (!_items.TryGetValue(itemType, out List<IItemEntity> itemEntities))
                {
                    itemEntities = new List<IItemEntity>();
                    _items.Add(itemType, itemEntities);
                }

                itemEntities.Add(item);
                return item;
            }
            else
            {
                Debug.LogError("item type is null " + param.ItemId);
                return null;
            }
        }

        private void BindItemToGrid(IItemEntity item, IGridEntity grid, ItemGenParam param)
        {
            MathUtility.GetDirectionRange(param.Index, param.Size, param.Direction,
                        out Vector2Int xRange, out Vector2Int yRange);

            ItemBase itemBase = (ItemBase)item;
            itemBase.SetDirection(param.Direction);

            for (int y = yRange.x; y < yRange.y; y++)
            {
                for (int x = xRange.x; x < xRange.y; x++)
                {
                    Vector2Int targetIndex = new Vector2Int(x, y);
                    GridEntity bindGrid = (GridEntity)GetGrid(targetIndex);
                    if (bindGrid.HasItem(item.Layer))
                        throw new Exception($"check this grid item");
                    bindGrid.SetItem(item, grid);
                }
            }
        }
    }
}
