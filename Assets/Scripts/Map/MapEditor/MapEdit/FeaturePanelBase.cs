#if UNITY_EDITOR
using UnityEngine;

namespace MH.GameScene.UIs.MapEdit
{
    public class FeaturePanelBase : MonoBehaviour, IFeaturePanel
    {
        protected MapEditorUI _editorUI;

        public virtual void OnInit(MapEditorUI editorUI)
        {
            _editorUI = editorUI;
        }

        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnShow()
        {
            gameObject.SetActive(true);
        }
    }
}
#endif