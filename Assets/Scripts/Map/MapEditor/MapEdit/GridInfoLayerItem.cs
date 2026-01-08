#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;
using Cysharp.Threading.Tasks;

namespace MH.GameScene.UIs.MapEdit
{
    public class GridInfoLayerItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nameCom;

        [SerializeField]
        private Image icon;

        private Vector2 _originSize;

        public async UniTask Set(IItemEntity item)
        {
            nameCom.text = item.Layer;
            _originSize = icon.rectTransform.sizeDelta;
            icon.sprite = await item.World.Resource.GetSprite(item.ItemId, item.Layer, item.Direction);
            if (icon)
                icon.FitSize(_originSize);
        }
    }
}
#endif