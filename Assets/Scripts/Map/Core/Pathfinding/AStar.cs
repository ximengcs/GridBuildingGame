using System.Collections.Generic;

namespace MH.GameScene.Core.PathFinding
{
    public partial class AStar
    {
        private IAStarHelper _helper;
        private NodeCollection _openList;
        private NodeCollection _closeList;
        private HashSet<object> _cache;
        private ObjectPool<AStarNode> _nodePool;

        public AStar(IAStarHelper helper)
        {
            _helper = helper;
            _nodePool = new ObjectPool<AStarNode>();
            _openList = new NodeCollection(this._helper, _nodePool);
            _closeList = new NodeCollection(this._helper, _nodePool);
            _cache = new HashSet<object>();
        }

        public IPath<T> Execute<T>(object startItem, object endItem)
        {
            AStarNode startNode = _nodePool.Require<AStarNode>();
            startNode.Init(startItem, _helper.GetHValue(startItem, endItem));
            startNode.OriginGValue = 1;
            startNode.GValue = 1;
            AStarNode endNode = null;
            _openList.Add(startNode);
            while (!_openList.Empty)
            {
                AStarNode itemNode = _openList.RemoveMinimum();

                if (itemNode.Item == endItem)
                {
                    endNode = itemNode;
                    break;
                }

                _closeList.Add(itemNode);

                _cache.Clear();
                _helper.GetItemRound(itemNode.Item, _cache);

                foreach (object child in _cache)
                {
                    if (_closeList.Contains(child))
                        continue;
                    if (!_openList.TryGet(child, out AStarNode childNode))
                    {
                        int hValue = _helper.GetHValue(child, endItem);
                        childNode = _nodePool.Require<AStarNode>();
                        childNode.Init(child, hValue);
                        _openList.Add(childNode);
                    }

                    if (childNode != null)
                    {
                        int gValue = _helper.GetGValue(itemNode.Item, childNode.Item);
                        if (gValue < childNode.OriginGValue)
                        {
                            childNode.OriginGValue = gValue;
                        }

                        gValue += itemNode.GValue;
                        if (gValue < childNode.GValue)
                        {
                            childNode.Parent = itemNode;
                            childNode.GValue = gValue;
                        }
                    }
                }
            }

            AStarPath<T> path = null;
            if (endNode != null)
            {
                path = new AStarPath<T>(endNode);
            }

            _closeList.Clear();
            _openList.Clear();

            return path;
        }
    }
}
