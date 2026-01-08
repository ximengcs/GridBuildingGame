using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Views
{
    public class ItemSpriteView : ComponentBase, IObjectView, IItemView
    {
        protected GameObject _obj;
        protected bool _initFinish;
        protected SpriteRenderer _spriteRender;
        protected IItemEntity _itemEntity;

        private Color _color;
        private string _sortingLayer;
        private int _sortingOrder;

        public IItemEntity Item => _itemEntity;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                if (_spriteRender)
                    _spriteRender.color = _color;
            }
        }

        public string SortingLayer
        {
            get => _sortingLayer;
            set
            {
                _sortingLayer = value;
                if (_spriteRender)
                    _spriteRender.sortingLayerName = value;
            }
        }
        public int SortingOrder
        {
            get => _sortingOrder;
            set
            {
                _sortingOrder = value;
                if (_spriteRender)
                    _spriteRender.sortingOrder = value;
            }
        }

        public void SetParent(Transform layerRoot)
        {
            _obj.transform.SetParent(layerRoot);
        }

        public void SetIndex(Vector2Int index)
        {
            Vector3 targetPos = MathUtility.GetGamePos(index, _itemEntity.Size, _itemEntity.Direction);
            targetPos.y += _spriteRender.GetOffset();
            _obj.transform.position = targetPos;
            OnIndexChange();
        }

        protected virtual void OnIndexChange()
        {

        }

        public override void OnStart()
        {
            base.OnStart();
            _itemEntity = (IItemEntity)Entity;
            UniTask.Create(LoadAsync, _destroyTokenSource.Token);
        }

        private async UniTask LoadAsync(CancellationToken token)
        {
            Sprite sprite = await Entity.World.Resource.GetSprite(_itemEntity.ItemId, _itemEntity.Layer, _itemEntity.Direction);
            token.ThrowIfCancellationRequested();
            _obj = new GameObject($"{_itemEntity.GetType().Name}_View_{_itemEntity.ItemId}");
            _spriteRender = _obj.AddComponent<SpriteRenderer>();
            _spriteRender.spriteSortPoint = SpriteSortPoint.Pivot;
            _spriteRender.sprite = sprite;

            IGridEntity grid = _itemEntity.MainGrid;
            IMapScene scene = (IMapScene)grid.Parent;
            SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(_itemEntity.Layer);
            IObjectLayer objLayer = gameLayer.GetObjectLayer();
            objLayer.Add(this);
            objLayer.SetProp(this, grid.Index);
            _itemEntity.GridChangeEvent += GridChangeHandler;
            _initFinish = true;

            OnLoadFinish();
        }

        protected virtual void OnLoadFinish() { }

        private void GridChangeHandler()
        {
            IGridEntity grid = _itemEntity.MainGrid;
            IMapScene scene = (IMapScene)grid.Parent;
            SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(_itemEntity.Layer);
            IObjectLayer objLayer = gameLayer.GetObjectLayer();
            objLayer.SetProp(this, grid.Index);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _itemEntity.GridChangeEvent -= GridChangeHandler;
            IGridEntity grid = _itemEntity.MainGrid;
            IMapScene scene = (IMapScene)grid.Parent;
            if (scene != null)
            {
                SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
                IGameLayer gameLayer = sceneView.GetLayer(_itemEntity.Layer);
                IObjectLayer objectLayer = gameLayer.GetObjectLayer();
                objectLayer.Remove(this);
            }

            GameObject.Destroy(_obj);
            _spriteRender = null;
            _obj = null;
            _itemEntity = null;
            _sortingLayer = null;
            _sortingOrder = default;
        }
    }
}
