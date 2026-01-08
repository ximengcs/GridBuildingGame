
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;
using System.Threading;

namespace MH.GameScene.Runtime.Views
{
    public class CommonCropView : ItemSpriteView
    {
        private CommonCrop _crop;
        private int _level;
        private bool _canCrop;
        private Tween _tween;

        public bool CanCrop => _canCrop;

        protected override void OnLoadFinish()
        {
            base.OnLoadFinish();
            _obj.transform.localScale = Vector3.zero;
            _crop = (CommonCrop)Entity;
            _crop.RegisterTime(TimeHandler);
            _crop.RegisterFinish(FinishHandler);
            _crop.PloughChangeEvent += PloughChangeHandler;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _crop.PloughChangeEvent -= PloughChangeHandler;
            _tween?.Kill();
            _tween = null;
        }

        private void PloughChangeHandler()
        {
            Vector2Int curIndex = _crop.Plough.MainGrid.Index;
            SetIndex(curIndex);
        }

        public Tween ToRemoveEffect()
        {
            _obj.transform.DOKill();
            _canCrop = false;
            _spriteRender.DOFade(0, 1);
            Vector3 target = _obj.transform.position + new Vector3(0, 1);
            _tween = _obj.transform.DOMove(target, 1);
            return _tween;
        }

        private Tween ToSmallEffect()
        {
            _obj.transform.DOKill();
            float time = 1;
            if (_obj.transform.localScale == Vector3.zero)
                time = 0;

            _tween = _obj.transform.DOScale(Vector3.zero, time);
            _tween.OnKill(() => _tween = null);
            return _tween;
        }

        private Tween ToNormalEffect()
        {
            _obj.transform.DOKill();
            float time = 1;
            if (_obj.transform.localScale == Vector3.one)
                time = 0;

            _tween = _obj.transform.DOScale(Vector3.one, time);
            _tween.OnKill(() => _tween = null);
            return _tween;
        }

        private Tween ToBreathEffect()
        {
            _obj.transform.DOKill();
            _tween = _obj.transform.DOScale(1.1f, 1).SetLoops(-1, LoopType.Yoyo);
            _tween.OnKill(() => _tween = null);
            return _tween;
        }

        private void TimeHandler(float time)
        {
            int curLv = 0;
            if (time < 5)
                curLv = 1;
            else if (time < 10)
                curLv = 2;
            else
                curLv = 3;

            if (curLv != _level)
            {
                _level = curLv;
                _ = LoadLvSprite(_level);
            }
        }

        private async void FinishHandler()
        {
            await UniTask.CompletedTask;
            _canCrop = true;
        }

        private async UniTask LoadLvSprite(int level)
        {
            CancellationToken token = _destroyTokenSource.Token;
            string resKey = null;
            if (level == 1)
                resKey = $"common/{_crop.ItemId}.png";
            else
                resKey = $"common/{_crop.ItemId}_lv{level}.png";
            await ToSmallEffect().Waiter();
            token.ThrowIfCancellationRequested();
            _spriteRender.sprite = await Entity.World.Resource.GetSprite(resKey);
            token.ThrowIfCancellationRequested();
            await ToNormalEffect().Waiter();
            token.ThrowIfCancellationRequested();
            ToBreathEffect();
        }
    }
}
