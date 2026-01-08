using UnityEngine;
using System.Threading;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Views
{
    public abstract class ItemTileViewBase : ComponentBase
    {
        protected Tile _tile;

        private Vector2Int _index;
        private IItemEntity _itemEntity;

        public override void OnStart()
        {
            base.OnStart();
            _itemEntity = (IItemEntity)Entity;
            UniTask.Create(LoadAsync, _destroyTokenSource.Token);
        }

        private async UniTask LoadAsync(CancellationToken token)
        {
            _tile = ScriptableObject.CreateInstance<Tile>();
            _tile.sprite = await Entity.World.Resource.GetSprite(_itemEntity.ItemId, _itemEntity.Layer, _itemEntity.Direction);
            token.ThrowIfCancellationRequested();

            IGridEntity grid = _itemEntity.MainGrid;
            _index = grid.Index;
            IMapScene scene = (IMapScene)grid.Parent;
            SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(_itemEntity.Layer);
            ITilemapLayer tilemapLayer = gameLayer.GetTilemapLayer();
            tilemapLayer.Add(_tile, _index);

            _itemEntity.GridChangeEvent += GridChangeHandler;
        }

        private void GridChangeHandler()
        {
            IGridEntity grid = _itemEntity.MainGrid;
            IMapScene scene = (IMapScene)grid.Parent;
            SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(_itemEntity.Layer);
            ITilemapLayer tilemapLayer = gameLayer.GetTilemapLayer();
            tilemapLayer.Remove(_index);
            _index = grid.Index;
            tilemapLayer.Add(_tile, _index);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _itemEntity.GridChangeEvent -= GridChangeHandler;

            IGridEntity grid = _itemEntity.MainGrid;
            IMapScene scene = (IMapScene)grid.Parent;
            SceneViewCom sceneView = scene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(_itemEntity.Layer);
            ITilemapLayer tilemapLayer = gameLayer.GetTilemapLayer();
            tilemapLayer.Remove(grid.Index);
            ScriptableObject.Destroy(_tile);
            _tile = null;
            _itemEntity = null;
        }
    }
}
