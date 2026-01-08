#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using MH.GameScene.Configs;
using MH.GameScene.Runtime;
using MH.GameScene.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MH.GameScene.UIs.MapEdit
{
    public class NpcItemsPanelItem : MonoBehaviour
    {
        [SerializeField]
        private Image bg;

        [SerializeField]
        private Image icon;

        [SerializeField]
        private Button btn;

        private Vector2 _rectSize;
        private Color _originColor;

        public NpcConfig Config { get; private set; }
        public MapEditorUI EditorUI { get; set; }

        public void Init(NpcConfig config)
        {
            this.Config = config;
            _rectSize = icon.rectTransform.sizeDelta;
            _ = LoadRes();
        }

        public void OnClick(UnityAction handler)
        {
            btn.onClick.AddListener(handler);
        }

        private async UniTask LoadRes()
        {
            icon.sprite = await EditorUI.World.Resource.GetSprite(Config.Id, Config.Layer, GameConst.DIRECTION_RT);
            icon.FitSize(_rectSize);
            _originColor = bg.color;
        }

        public void OnSelect()
        {
            bg.color = Color.green;
        }

        public void OnUnselect()
        {
            bg.color = _originColor;
        }
    }
}
#endif