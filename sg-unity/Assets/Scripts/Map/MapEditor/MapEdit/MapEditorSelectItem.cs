#if UNITY_EDITOR
using MH.GameScene.Configs;

namespace MH.GameScene.UIs.MapEdit
{
    public enum SelectItemType
    {
        Common,
        Delete
    }

    public class MapEditorSelectItem
    {
        public ItemConfig Config { get; private set; }

        public SelectItemType SelectType { get; private set; }

        public MapEditorSelectItem(SelectItemType type, object data)
        {
            SelectType = type;
            Config = (ItemConfig)data;
        }
    }
}
#endif