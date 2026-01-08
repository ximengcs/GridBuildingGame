using UnityEngine;

namespace MH.GameScene.Runtime.Entities
{
    public struct ItemGenParam
    {
        public int ItemId;
        public string Layer;
        public Vector2Int Index;
        public Vector2Int Size;
        public int Direction;

        public ItemGenParam(IItemEntity itemEntity)
        {
            ItemId = itemEntity.ItemId;
            Layer = itemEntity.Layer;
            Size = itemEntity.Size;
            Direction = itemEntity.Direction;
            Index = itemEntity.MainGrid.Index;
        }

        public ItemGenParam(Vector2Int index)
        {
            ItemId = 0;
            Layer = GameConst.COMMON_LAYER;
            Size = Vector2Int.one;
            Direction = GameConst.DIRECTION_RT;
            this.Index = index;
        }

        public bool Equals(IItemEntity itemEntity)
        {
            if (ItemId != itemEntity.ItemId) return false;
            if (Layer != itemEntity.Layer) return false;
            if (Size != itemEntity.Size) return false;
            if (Direction != itemEntity.Direction) return false;
            if (Index != itemEntity.MainGrid.Index) return false;
            return true;
        }
    }
}
