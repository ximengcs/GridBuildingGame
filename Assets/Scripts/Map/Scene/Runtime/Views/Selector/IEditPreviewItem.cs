using UnityEngine;
using MH.GameScene.Runtime.Entities;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MH.GameScene.Runtime.Views
{
    public interface IEditPreviewItem
    {
        SpriteRenderer SpriteRender { get; }
        ItemGenParam GenParam { get; }
        void SetItem(int itemId);
        void ClearItem(bool resetItem = true);
        UniTask Refresh(Vector2Int index, Vector2Int size, int direction, CancellationToken token);
    }
}