#if UNITY_EDITOR
using MM.MapEditors;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.UIs.MapEdit
{
    public interface IEditFeature
    {
        void SelectFeature();
        void OnInit(MapEditorUI editorUI);
        void OnExit();
        void OnEnter();
        void SetEnable(bool enable);
        void OnSceneLoad(MapEditorEntity scene);
        void OnSceneExit();
        void OnSelectGrid(GridEntity grid);
    }
}
#endif