#if UNITY_EDITOR
namespace MH.GameScene.UIs.MapEdit
{
    public class SettingFeature : FeatureBase
    {
        public override void SelectFeature()
        {
            base.SelectFeature();
            _editorUI.ShowPanel<MapEditorSystemPanel>();
        }
    }
}
#endif