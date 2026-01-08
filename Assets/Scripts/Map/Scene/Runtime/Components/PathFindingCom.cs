using UnityEngine;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MH.GameScene.Core.PathFinding;

namespace MH.GameScene.Runtime.Entities
{
    public class PathFindingCom : ComponentBase, IAStarHelper
    {
        private AStar _algorithm;
        private IMapScene _scene;
        private IGridEntity _from;
        private IGridEntity _to;

        public override void OnInit(Entity entity, object data)
        {
            base.OnInit(entity, data);
            _scene = (IMapScene)Entity;
            _algorithm = new AStar(this);
        }

        public IPath<IGridEntity> Find(Vector2Int from, Vector2Int to)
        {
            return Find(_scene.GetGrid(from), _scene.GetGrid(to));
        }

        public IPath<IGridEntity> Find(IGridEntity from, IGridEntity to)
        {
            if (from == null || to == null)
                return null;

            _from = from;
            _to = to;
            IPath<IGridEntity> path = _algorithm.Execute<IGridEntity>(from, to);
            return path;
        }

        int IAStarHelper.GetGValue(object from, object to)
        {
            IGridEntity fromGrid = (IGridEntity)from;
            IGridEntity toGrid = (IGridEntity)to;
            return (int)Vector2Int.Distance(fromGrid.Index, toGrid.Index);
        }

        int IAStarHelper.GetHValue(object start, object end)
        {
            IGridEntity fromGrid = (IGridEntity)start;
            IGridEntity toGrid = (IGridEntity)end;
            return (int)Vector2Int.Distance(fromGrid.Index, toGrid.Index) * 2;
        }

        void IAStarHelper.GetItemRound(object item, HashSet<object> result)
        {
            IGridEntity grid = (IGridEntity)item;
            _scene.GetAdjacentGrid(grid.Index, result, (grid) =>
            {
                if (grid == _from || grid == _to)
                    return true;

                IItemEntity item = grid.GetItem(GameConst.SURFACE_LAYER);
                if (item != null)
                {
                    switch (item.ItemId)
                    {
                        case 100004: return false; // 海面
                    }
                }
                
                item = grid.GetItem(GameConst.SURFACEDECORATE_LAYER);
                if (item != null)
                {
                    switch (item.ItemId)
                    {
                        case 200004: return false; // 耕地
                    }
                }
                
                return !grid.HasItem(GameConst.COMMON_LAYER);
            });
        }

        int IAStarHelper.GetUniqueId(object item)
        {
            return item.GetHashCode();
        }
    }
}
