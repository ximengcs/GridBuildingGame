using UnityEngine;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Views
{
    public class CloudView : ComponentBase, IObjectView
    {
        private GameObject _inst;
        private Transform _tf;
        private SpriteRenderer _spriteRender;
        private Tweener _tween;
        private IObjectLayer _layer;

        private string _sortingLayer;
        private int _sortingOrder;

        public Color Color
        {
            get => _spriteRender.color;
            set => _spriteRender.color = value;
        }

        public string SortingLayer
        {
            get => _sortingLayer;
            set
            {
                _sortingLayer = value;
                if (_spriteRender)
                    _spriteRender.sortingLayerName = _sortingLayer;
            }
        }

        public int SortingOrder
        {
            get => _sortingOrder;
            set
            {
                _sortingOrder = value;
                if (_spriteRender)
                    _spriteRender.sortingOrder = _sortingOrder;
            }
        }

        private async UniTask LoadAsync(CancellationToken token)
        {
            _inst = new GameObject(nameof(CloudView));
            _tf = _inst.transform;
            _tween = _tf.DOMove(new Vector3(40, 0), 10).SetLoops(-1, LoopType.Restart);
            _spriteRender = _inst.AddComponent<SpriteRenderer>();
            _spriteRender.sprite = await Entity.World.Resource.GetSprite(400001, GameConst.SKY_LAYER, GameConst.DIRECTION_RT);
            token.ThrowIfCancellationRequested();

            SceneViewCom sceneView = Entity.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(GameConst.SKY_LAYER);
            _layer = gameLayer.GetObjectLayer();
            _layer.Add(this);
            _layer.SetProp(this, new Vector2Int(-60, 0));
            RefreshProp();
        }

        private void RefreshProp()
        {
            _spriteRender.sortingLayerName = _sortingLayer;
            _spriteRender.sortingOrder = _sortingOrder;
        }

        public override void OnStart()
        {
            base.OnStart();
            UniTask.Create(LoadAsync, _destroyTokenSource.Token);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_layer != null)
            {
                _layer.Remove(this);
            }

            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }

        public void SetIndex(Vector2Int index)
        {
            _tf.position = MathUtility.IndexToGamePos(index);
        }

        public void SetParent(Transform layerRoot)
        {
            _tf.SetParent(layerRoot);
        }
    }
}
