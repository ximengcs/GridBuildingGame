
using UnityEngine;

namespace MH.GameScene.Runtime.Views
{
    public interface ITile
    {
        Color Color { get; set; }

        void Clear();
    }
}
