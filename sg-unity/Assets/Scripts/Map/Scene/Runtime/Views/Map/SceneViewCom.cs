using UnityEngine;
using UnityEngine.Tilemaps;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Views
{
    public class SceneViewCom : ComponentBase
    {
        private Vector3 _cellSize;
        private IMapScene _entity;
        private GameObject _mapObject;
        private Dictionary<string, IGameLayer> _layerViews;

        public override void OnInit(Entity e, object data)
        {
            base.OnInit(e, data);
            _entity = (IMapScene)e;

            _cellSize = new Vector3(1, 0.5f);
            _mapObject = new GameObject(nameof(SceneViewCom));
            Entity.World.FindEntity<WorldView>().AddChild(_mapObject);
            Grid grid = _mapObject.AddComponent<Grid>();
            grid.cellSize = _cellSize;
            grid.cellLayout = GridLayout.CellLayout.Isometric;
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;

            _layerViews = new Dictionary<string, IGameLayer>();
            AddLayer(GameConst.SURFACE_LAYER, true, TilemapRenderer.Mode.Chunk);
            AddLayer(GameConst.SURFACEDECORATE_LAYER);
            AddLayer(GameConst.SURFACE_FEAT_LAYER, true, TilemapRenderer.Mode.Chunk);
            AddLayer(GameConst.COMMON_LAYER, true, TilemapRenderer.Mode.Individual);
            AddLayer(GameConst.COMMON_FEAT_LAYER, true, TilemapRenderer.Mode.Chunk);
            AddLayer(GameConst.SKY_LAYER, false, TilemapRenderer.Mode.Chunk);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            List<IGameLayer> layers = new List<IGameLayer>(_layerViews.Values);
            _layerViews.Clear();
            foreach (GameLayer layer in layers)
                layer.Destroy();

            GameObject.Destroy(_mapObject);
            _entity = null;
            _mapObject = null;
            _layerViews = null;
        }

        public IEnumerable<string> GetLayers()
        {
            return _layerViews.Keys;
        }

        public void ShowLayer(string layerName = null)
        {
            if (layerName != null)
            {
                if (_layerViews.TryGetValue(layerName, out IGameLayer gameLayer))
                {
                    gameLayer.Show();
                }
            }
            else
            {
                foreach (var entry in _layerViews)
                    entry.Value.Show();
            }
        }

        public void HideLayer(string layerName)
        {
            if (_layerViews.TryGetValue(layerName, out IGameLayer gameLayer))
            {
                gameLayer.Hide();
            }
        }

        public IGameLayer GetLayer(string layerName)
        {
            if (_layerViews.TryGetValue(layerName, out IGameLayer gameLayer))
            {
                return gameLayer;
            }
            return null;
        }

        private void AddLayer(string layerName, bool addTileLayer = false, TilemapRenderer.Mode mode = TilemapRenderer.Mode.Chunk)
        {
            IGameLayer layer = new GameLayer(_mapObject, layerName);
            if (addTileLayer)
                layer.AddTilemapLayer(GameConst.DEFAULT_TILENAME, mode);
            layer.AddObjectLayer();
            _layerViews[layerName] = layer;
        }
    }
}
