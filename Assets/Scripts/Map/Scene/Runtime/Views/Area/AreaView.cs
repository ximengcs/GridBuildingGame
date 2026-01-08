using UnityEngine;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Runtime;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using MH.GameScene.Runtime.Utilities;

namespace MM.MapEditors
{
    public abstract partial class AreaView : Entity
    {
        protected Tile _tile;
        protected IArea _area;
        protected string _layerName;
        protected ITilemapLayer _areaLayer;
        protected Dictionary<Vector2Int, ITile> _tiles;
        protected Color _tileColor;

        protected abstract string Res { get; }

        public Color Color
        {
            get => _tileColor;
            set
            {
                _tileColor = value;
                if (_tiles != null)
                {
                    foreach (ITile tile in _tiles.Values)
                        tile.Color = _tileColor;
                }
            }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _area = (IArea)Parent;
            _area.GridChangeEvnet += GridChangeHandler;
            _tiles = new Dictionary<Vector2Int, ITile>();
            _tileColor = Color.white;
            _layerName = $"Area_{_area.AreaId}";
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _area.GridChangeEvnet -= GridChangeHandler;
            _areaLayer.Clear();
            IMapScene mapScene = Parent.World.FindEntity<IMapScene>();
            SceneViewCom sceneView = mapScene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(GameConst.COMMON_FEAT_LAYER);
            gameLayer.RemoveLayer(_layerName);
            _areaLayer = null;
            _tiles = null;
            _area = null;
            _layerName = null;
        }

        protected override void OnStart()
        {
            base.OnStart();
            UniTask.Create(InitAsync);
        }

        private void GridChangeHandler()
        {
            Refresh();
        }

        private async UniTask InitAsync()
        {
            _tile = ScriptableObject.CreateInstance<Tile>();
            _tile.sprite = await Parent.World.Resource.GetSprite(Res);
            _tile.color = _tileColor;
            Refresh();
        }

        private void Refresh()
        {
            IMapScene mapScene = Parent.World.FindEntity<IMapScene>();
            SceneViewCom sceneView = mapScene.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(GameConst.COMMON_FEAT_LAYER);
            if (_areaLayer == null)
            {
                _areaLayer = gameLayer.AddTilemapLayer(_layerName);
                Vector2Int centerIndex = MathUtility.GamePosToIndex(_area.Center);
                _areaLayer.SetBaseIndex(centerIndex);
                OnInitLayer(_areaLayer);
            }

            _areaLayer.Clear();
            _tiles.Clear();

            foreach (IGridEntity grid in _area.Grids)
            {
                ITile tile = _areaLayer.Add(_tile, grid.Index);
                tile.Color = Color;
                _tiles[grid.Index] = tile;

                OnInitItem(tile);
            }
        }

        protected virtual void OnInitLayer(ITilemapLayer layer)
        {

        }

        protected virtual void OnInitItem(ITile view)
        {
        }
    }
}
