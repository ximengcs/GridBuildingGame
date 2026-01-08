using UnityEngine;

namespace MH.GameScene.Runtime.Views
{
    public interface IObjectView
    {
        Color Color { get; set; }

        string SortingLayer { get; set; }

        int SortingOrder { get; set; }

        void SetParent(Transform layerRoot);

        void SetIndex(Vector2Int index);
    }
}
