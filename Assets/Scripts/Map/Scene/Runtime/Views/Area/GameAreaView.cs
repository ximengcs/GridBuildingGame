using R3;
using DG.Tweening;
using UnityEngine;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Runtime.Utilities;

namespace MM.MapEditors
{
    public class GameAreaView : AreaView
    {
        protected override string Res => "other/cloud.png";

        protected override void OnInitLayer(ITilemapLayer layer)
        {
            base.OnInitLayer(layer);
            layer.AddCollider();
            layer.Root.OnSceneClickAsObservable().Subscribe(ClickHandler);
        }

        private void ClickHandler(Vector2 screenPos)
        {
            if (_areaLayer != null)
            {
                _areaLayer.Root.DOShakeScale(1.0f, 0.2f, 20).OnComplete(() =>
                {
                    _areaLayer.Root.localScale = Vector3.one;
                });
            }
        }
    }
}
