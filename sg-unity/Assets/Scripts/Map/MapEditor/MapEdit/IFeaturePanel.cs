#if UNITY_EDITOR
using MM.MapEditors;

namespace MH.GameScene.UIs.MapEdit
{
    public interface IFeaturePanel
    {
        void OnInit(MapEditorUI editorUI);

        void OnShow();

        void OnHide();
    }
}
#endif