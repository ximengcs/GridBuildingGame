#if UNITY_EDITOR
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;
using Cysharp.Threading.Tasks;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorGridScannerPanelItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI layerText;

        [SerializeField]
        private Image icon;

        [SerializeField]
        private Image selectIcon;

        [SerializeField]
        private TextMeshProUGUI itemIdText;

        [SerializeField]
        private TextMeshProUGUI sizeText;

        [SerializeField]
        private TextMeshProUGUI directionText;

        [SerializeField]
        private Button selectBtn;

        public IItemEntity ItemEntity { get; private set; }

        private Action<MapEditorGridScannerPanelItem> _callback;
        private Color _originColor;
        private Vector2 _originSize;

        public void Init(IItemEntity itemEntity, Action<MapEditorGridScannerPanelItem> callback)
        {
            ItemEntity = itemEntity;
            _callback = callback;
            layerText.text = itemEntity.Layer;

            _ = LoadRes();

            itemIdText.text = $"{itemEntity.ItemId}";
            sizeText.text = $"{itemEntity.Size.x} x {itemEntity.Size.y}";
            directionText.text = $"{itemEntity.Direction}";
            _originColor = selectIcon.color;
            selectBtn = GetComponent<Button>();
            selectBtn.onClick.AddListener(ClickHandler);
        }

        private async UniTask LoadRes()
        {
            _originSize = icon.rectTransform.sizeDelta;
            icon.sprite = await ItemEntity.World.Resource.GetSprite(ItemEntity.ItemId, ItemEntity.Layer, ItemEntity.Direction);
            icon.FitSize(_originSize);
        }

        private void ClickHandler()
        {
            _callback?.Invoke(this);
        }

        public void OnSelect()
        {
            selectIcon.color = Color.cyan;
        }

        public void OnUnselect()
        {
            selectIcon.color = _originColor;
        }
    }
}
#endif