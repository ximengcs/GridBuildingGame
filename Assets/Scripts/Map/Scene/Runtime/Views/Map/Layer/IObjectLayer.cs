
using UnityEngine;

namespace MH.GameScene.Runtime.Views
{
    public interface IObjectLayer
    {
        Transform Root { get; }

        void SetProp(IObjectView view, Vector2Int index);

        void Add(IObjectView view);

        void Remove(IObjectView view);
    }
}
