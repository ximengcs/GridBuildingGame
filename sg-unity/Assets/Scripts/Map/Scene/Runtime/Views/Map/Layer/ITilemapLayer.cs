
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MH.GameScene.Runtime.Views
{
    public interface ITilemapLayer
    {
        Transform Root { get; }

        ITile Add(Tile tile, Vector2Int index);

        void Remove(Vector2Int index);

        void Clear();

        void AddCollider();

        void SetBaseIndex(Vector2Int baseIndex);
    }
}
