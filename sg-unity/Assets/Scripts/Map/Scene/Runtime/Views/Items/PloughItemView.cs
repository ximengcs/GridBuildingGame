using DG.Tweening;
using UnityEngine;

namespace MH.GameScene.Runtime.Views
{
    public class PloughItemView : ItemSpriteView, IGridSelectable, IItemView
    {
        private Vector3 _originPos;
        private int _sortOrder;

        protected override void OnIndexChange()
        {
            base.OnIndexChange();
            _originPos = _obj.transform.localPosition;
        }

        public void OnSelect()
        {
            if (!_initFinish)
                return;

            _obj.transform.DOLocalMove(_originPos + new Vector3(0, 0.4f), 0.5f);
            _sortOrder = SortingOrder;
            SortingOrder = GameConst.MAX_ORDER;
            _spriteRender.DOColor(Color.red, 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnUnSelect()
        {
            if (!_initFinish)
                return;

            _spriteRender.DOKill();
            _obj.transform.DOLocalMove(_originPos, 0.5f);
            SortingOrder = _sortOrder;
            Color = Color.white;
        }
    }
}
