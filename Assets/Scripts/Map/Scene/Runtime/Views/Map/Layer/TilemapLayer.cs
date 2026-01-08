using MH.GameScene.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.TilemapRenderer;

namespace MH.GameScene.Runtime.Views
{
    public partial class TilemapLayer : LayerBase, ITilemapLayer
    {
        private string _name;
        private GameObject _root;
        private GameObject _layerObj;
        private Tilemap _tilemap;
        private TilemapRenderer _tilemapRenderer;
        private Vector2Int _baseIndex;

        public Transform Root => _layerObj.transform;

        public override string SortingLayer => _tilemapRenderer.sortingLayerName;

        public override int SortingOrder => _tilemapRenderer.sortingOrder;

        public override string Name => _name;

        public TilemapLayer(GameObject root, string layerName, string name, int order, Mode renderMode)
        {
            _root = root;
            _name = name;
            _layerObj = new GameObject($"{nameof(TilemapLayer)}_{name}");
            _layerObj.transform.parent = root.transform;
            _tilemap = _layerObj.AddComponent<Tilemap>();
            _tilemapRenderer = _layerObj.AddComponent<TilemapRenderer>();

            _tilemap.tileAnchor = new Vector3(0.5f, 0.5f);
            _tilemap.orientation = Tilemap.Orientation.XY;
            _tilemapRenderer.sortOrder = SortOrder.TopRight;
            _tilemapRenderer.mode = renderMode;
            _tilemapRenderer.sortingLayerName = layerName;
            _tilemapRenderer.sortingOrder = order;
            _tilemapRenderer.mode = Mode.Individual;
        }

        public void SetBaseIndex(Vector2Int baseIndex)
        {
            _baseIndex = baseIndex;
            _layerObj.transform.position = MathUtility.IndexToGamePos(baseIndex) + new Vector2(0, -0.25f);
        }

        public ITile Add(Tile tile, Vector2Int index)
        {
            if (_tilemap)
            {
                Vector3Int pos = new Vector3Int(index.x - _baseIndex.x, index.y - _baseIndex.y);
                _tilemap.SetTile(pos, tile);
                ITile wrapper = new TileCommon(_tilemap, pos);
                return wrapper;
            }
            return null;
        }

        public void Remove(Vector2Int index)
        {
            if (_tilemap)
                _tilemap.SetTile(new Vector3Int(index.x - _baseIndex.x, index.y - _baseIndex.y), null);
        }

        public void Clear()
        {
            if (_tilemap)
                _tilemap.ClearAllTiles();
        }

        public void AddCollider()
        {
            if (_layerObj)
                _layerObj.AddComponent<TilemapCollider2D>();
        }

        public override void OnDestroy()
        {
            GameObject.Destroy(_layerObj);
            _root = null;
            _layerObj = null;
            _tilemap = null;
            _tilemapRenderer = null;
        }
    }
}
