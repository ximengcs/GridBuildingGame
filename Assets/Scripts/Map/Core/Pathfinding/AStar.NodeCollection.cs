using System.Collections.Generic;

namespace MH.GameScene.Core.PathFinding
{
    public partial class AStar
    {
        private class NodeCollection
        {
            private IAStarHelper _helper;
            private ObjectPool<AStarNode> _pool;
            private Dictionary<int, AStarNode> _nodes;

            public bool Empty
            {
                get
                {
                    return _nodes.Count == 0;
                }
            }

            public int Count => _nodes.Count;

            public NodeCollection(IAStarHelper helper, ObjectPool<AStarNode> pool)
            {
                this._helper = helper;
                this._pool = pool;
                _nodes = new Dictionary<int, AStarNode>();
            }

            public void Clear()
            {
                foreach (var node in _nodes.Values)
                    _pool.Release(node);
                _nodes.Clear();
            }

            public AStarNode RemoveMinimum()
            {
                AStarNode minimum = null;
                foreach (var node in _nodes.Values)
                {
                    if (minimum == null || minimum.FValue > node.FValue)
                    {
                        minimum = node;
                    }
                }

                if (minimum != null)
                {
                    _nodes.Remove(_helper.GetUniqueId(minimum.Item));
                }
                return minimum;
            }

            public bool Contains(object item)
            {
                int id = _helper.GetUniqueId(item);
                return _nodes.ContainsKey(id);
            }

            public bool TryGet(object item, out AStarNode node)
            {
                int id = _helper.GetUniqueId(item);
                return _nodes.TryGetValue(id, out node);
            }

            public void Add(AStarNode node)
            {
                int id = _helper.GetUniqueId(node.Item);
                _nodes[id] = node;
            }
        }
    }
}
