
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIScenes
{
    public class HouseBubble : UISceneItem
    {
        [SerializeField]
        private Image icon;

        private Vector2 _iconSize;

        private void Awake()
        {
            _iconSize = icon.rectTransform.sizeDelta;
        }

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
            icon.FitSize(_iconSize);
        }
    }
}
