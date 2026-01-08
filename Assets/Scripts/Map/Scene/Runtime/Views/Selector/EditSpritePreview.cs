using UnityEngine;
using System.Threading;
using MH.GameScene.Configs;
using MH.GameScene.Runtime;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Views
{
    public class EditSpritePreview : ComponentBase, IObjectView, IEditPreviewItem
    {
        private GameObject _obj;
        private ItemGenParam _genParam;
        private bool _dirty;

        protected SpriteRenderer _spriteRender;

        private string _sortingLayer;
        private int _sortingOrder;

        public SpriteRenderer SpriteRender => _spriteRender;

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

        public ItemGenParam GenParam => _genParam;

        public void SetParent(Transform layerRoot)
        {
            _obj.transform.SetParent(layerRoot);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            ClearItem();
        }

        public void SetItem(int itemId)
        {
            if (itemId == _genParam.ItemId)
                return;
            if (_obj != null)
                ClearItem();

            _genParam.ItemId = itemId;
        }

        public void ClearItem(bool resetItem = true)
        {
            if (_obj)
            {
                IMapScene scene = (IMapScene)Entity.Parent;
                SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
                IGameLayer gameLayer = sceneView.GetLayer(_genParam.Layer);
                IObjectLayer objLayer = gameLayer.GetObjectLayer();
                objLayer.Remove(this);
                GameObject.Destroy(_obj);
                _spriteRender = null;
                _obj = null;
            }

            if (resetItem)
                _genParam.ItemId = default;
            _dirty = true;
        }

        public void SetIndex(Vector2Int index)
        {
            if (_obj != null)
            {
                Vector3 targetPos = MathUtility.GetGamePos(index, _genParam.Size, _genParam.Direction);
                targetPos.y += _spriteRender.GetOffset();
                _obj.transform.position = targetPos;
            }
        }

        public async UniTask Refresh(Vector2Int index, Vector2Int size, int direction, CancellationToken token)
        {
            if (_genParam.ItemId == default)
                return;

            if (_dirty || _genParam.Size != size || _genParam.Direction != direction)
            {
                ClearItem(false);

                ItemConfig config = Entity.World.Resource.GetConfig<ItemConfig>(_genParam.ItemId);
                _genParam.Layer = config.Layer;
                _genParam.Direction = direction;
                _genParam.Size = size;

                _obj = new GameObject(nameof(ItemSpriteView));
                _spriteRender = _obj.AddComponent<SpriteRenderer>();
                _spriteRender.sprite = await Entity.World.Resource.GetSprite(_genParam.ItemId, _genParam.Layer, direction);
                token.ThrowIfCancellationRequested();
                _spriteRender.spriteSortPoint = SpriteSortPoint.Pivot;
                _spriteRender.color = new Color(1, 1, 1, 0.5f);

                IMapScene scene = (IMapScene)Entity.Parent;
                SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
                IGameLayer gameLayer = sceneView.GetLayer(_genParam.Layer);
                IObjectLayer objLayer = gameLayer.GetObjectLayer();
                objLayer.Add(this);
                objLayer.SetProp(this, Vector2Int.zero);
            }

            _genParam.Index = index;
            SetIndex(index);
            _dirty = false;
        }
    }
}