
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MH.GameScene.Runtime.Views
{
    public partial class TilemapLayer
    {
        private class TileCommon : ITile
        {
            private Color _color;
            private Tile _tile;
            private Tilemap _map;
            private Vector3Int _index;

            public Color Color
            {
                get { return _tile.color; }
                set
                {
                    if (_color != value)
                    {
                        _color = value;
                        _tile.color = value;
                        _map.RefreshTile(_index);
                    }
                }
            }

            public TileCommon(Tilemap map, Vector3Int index)
            {
                _tile = (Tile)map.GetTile(index);
                _map = map;
                _index = index;
            }

            public void Clear()
            {
                _map.SetTile(_index, null);
            }
        }
    }
}
