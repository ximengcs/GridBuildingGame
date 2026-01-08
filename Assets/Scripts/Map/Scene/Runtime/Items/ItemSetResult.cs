
using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.Runtime.Entities
{
    public struct ItemSetResult
    {
        public ItemSetResultType Type;
        public Dictionary<ItemSetResultType, HashSet<Vector2Int>> CheckGrids;

        public void Add(ItemSetResultType type, Vector2Int index)
        {
            if (CheckGrids == null)
                CheckGrids = new Dictionary<ItemSetResultType, HashSet<Vector2Int>>();
            if (!CheckGrids.TryGetValue(type, out HashSet<Vector2Int> list))
            {
                list = new HashSet<Vector2Int>();
                CheckGrids.Add(type, list);
            }

            list.Add(index);
        }
    }
}
