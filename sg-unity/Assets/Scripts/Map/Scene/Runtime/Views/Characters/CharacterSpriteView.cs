using UnityEngine;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;
using MH.GameScene.Runtime.Characters;

namespace MH.GameScene.Runtime.Views
{
    public class CharacterSpriteView : ComponentBase, IObjectView
    {
        private GameObject _obj;
        private IMapScene _mapScene;

        protected SpriteRenderer _spriteRender;
        protected ICharacter _entity;

        public Color Color
        {
            get => _spriteRender.color;
            set => _spriteRender.color = value;
        }

        public string SortingLayer
        {
            get => _spriteRender.sortingLayerName;
            set => _spriteRender.sortingLayerName = value;
        }
        public int SortingOrder
        {
            get => _spriteRender.sortingOrder;
            set => _spriteRender.sortingOrder = value;
        }

        public void SetParent(Transform layerRoot)
        {
            _obj.transform.SetParent(layerRoot);
        }

        public void SetIndex(Vector2Int index)
        {
            _obj.transform.position = MathUtility.GetGamePos(index, Vector2Int.one, GameConst.DIRECTION_RT);
        }

        public override void OnStart()
        {
            base.OnStart();
            _mapScene = Entity.World.FindEntity<IMapScene>();
            _entity = (ICharacter)Entity;
            //entity.GridChangeEvent += GridChangeHandler;
            LoadSprite();
        }

        private void GridChangeHandler()
        {
            //IGridEntity grid = itemEntity.MainGrid;
            //IMapScene scene = (IMapScene)grid.Parent;
            //SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
            //sceneView.SetObjectProp(itemEntity.Layer, this, grid.Index);
        }

        protected async void LoadSprite()
        {
            _obj = new GameObject(nameof(CharacterSpriteView));
            _spriteRender = _obj.AddComponent<SpriteRenderer>();
            _spriteRender.sprite = await Entity.World.Resource.GetSprite(300012, GameConst.COMMON_LAYER, GameConst.DIRECTION_RT);
            _spriteRender.spriteSortPoint = SpriteSortPoint.Pivot;

            SceneViewCom sceneView = _mapScene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(GameConst.COMMON_LAYER);
            IObjectLayer objLayer = gameLayer.GetObjectLayer();
            objLayer.Add(this);
            objLayer.SetProp(this, _entity.Index);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            //itemEntity.GridChangeEvent -= GridChangeHandler;

            if (_mapScene != null)
            {
                SceneViewCom sceneView = _mapScene.GetCom<SceneViewCom>();
                if (sceneView != null)
                {
                    IGameLayer gameLayer = sceneView.GetLayer(GameConst.COMMON_LAYER);
                    IObjectLayer objLayer = gameLayer.GetObjectLayer();
                    objLayer.Remove(this);
                }
            }

            GameObject.Destroy(_obj);
            _spriteRender = null;
            _obj = null;
            _mapScene = null;
        }
    }
}
