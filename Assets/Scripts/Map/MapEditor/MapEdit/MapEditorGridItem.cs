#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using MH.GameScene.Runtime;
using MH.GameScene.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorGridItem : MonoBehaviour
    {
        [SerializeField]
        private Button btn;

        [SerializeField]
        private Image bg;

        [SerializeField]
        private Image icon;

        public int ItemId { get; private set; }

        public MapEditorSelectItem Data { get; private set; }

        public string Layer { get; private set; }

        public MapEditorUI EditorUI { get; set; }

        private Color _originColor;
        private Vector2 _iconSize;

        public void Init(int itemId, string layer, string res, MapEditorSelectItem data)
        {
            ItemId = itemId;
            Data = data;
            Layer = layer;

            _iconSize = icon.rectTransform.sizeDelta;
            _ = LoadRes();
        }

        public void OnClick(UnityAction handler)
        {
            btn.onClick.AddListener(handler);
        }

        private async UniTask LoadRes()
        {
            string resLayer = Layer;
            if (ItemId == -1)
                resLayer = GameConst.COMMON_LAYER;

            icon.sprite = await EditorUI.World.Resource.GetSprite(ItemId, resLayer, GameConst.DIRECTION_RT);
            if (icon.sprite == null)
                Debug.LogError($" item {ItemId} {resLayer} sprite is null");
            if (icon)
                icon.FitSize(_iconSize);
            _originColor = bg.color;
        }

        public void OnSelect()
        {
            bg.color = Color.yellow;
            icon.color = Color.yellow;
        }

        public void OnUnselect()
        {
            bg.color = _originColor;
            icon.color = Color.white;
        }
    }
}
#endif