using TileMode = UnityEngine.Tilemaps.TilemapRenderer.Mode;

namespace MH.GameScene.Runtime.Views
{
    public interface IGameLayer
    {
        void Show();

        void Hide();

        IObjectLayer GetObjectLayer(string name = GameConst.DEFAULT_OBJNAME);

        IObjectLayer AddObjectLayer(string name = GameConst.DEFAULT_OBJNAME, int sortingOrder = GameConst.DEFAULT_ORDER);

        ITilemapLayer GetTilemapLayer(string name = GameConst.DEFAULT_TILENAME);

        ITilemapLayer AddTilemapLayer(string name = GameConst.DEFAULT_TILENAME, TileMode mode = TileMode.Chunk, int sortingOrder = GameConst.DEFAULT_ORDER);

        void RemoveLayer(string name);
    }
}
