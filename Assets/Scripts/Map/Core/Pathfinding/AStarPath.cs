using System.Collections;
using System.Collections.Generic;

namespace MH.GameScene.Core.PathFinding
{
    public class AStarPath<T> : IPath<T>
    {
        private List<T> _items;

        public int Count => _items.Count;

        T IReadOnlyList<T>.this[int index] => (T)_items[index];

        internal AStarPath(AStarNode node)
        {
            _items = new List<T>();
            InnerRecursiveAdd(node);
            _items.Reverse();
        }

        private void InnerRecursiveAdd(AStarNode node)
        {
            if (node == null) return;
            _items.Add((T)node.Item);
            InnerRecursiveAdd(node.Parent);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
