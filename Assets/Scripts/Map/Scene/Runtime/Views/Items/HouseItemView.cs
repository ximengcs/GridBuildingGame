using Cysharp.Threading.Tasks;
using UI.UIScenes;
using UnityEngine;

namespace MH.GameScene.Runtime.Views
{
    public class HouseItemView : ItemSpriteView, IGridSelectable
    {
        private HouseBubble _bubbleUI;

        public void OnSelect()
        {
            if (!_initFinish)
                return;

            if (_bubbleUI == null)
                OpenUI().Forget();
        }

        public void OnUnSelect()
        {
            if (!_initFinish)
                return;

            if (_bubbleUI != null)
            {
                Entity.World.UIScene.Close(_bubbleUI);
                _bubbleUI = null;
            }
        }

        private async UniTaskVoid OpenUI()
        {
            IUISceneBinder binder = (IUISceneBinder)Entity;
            _bubbleUI = await Entity.World.UIScene.Open<HouseBubble>(binder);
            _bubbleUI.SetIcon(_spriteRender.sprite);
        }
    }
}
