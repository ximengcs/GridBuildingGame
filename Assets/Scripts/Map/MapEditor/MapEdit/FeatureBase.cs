#if UNITY_EDITOR
using UnityEngine;
using MM.MapEditors;
using UnityEngine.UI;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.UIs.MapEdit
{
    public class FeatureBase : MonoBehaviour, IEditFeature
    {
        private Image _icon;
        private Button _selectBtn;

        protected bool _featureActive;
        protected MapEditorUI _editorUI;
        protected EditSelector _selector;

        public virtual void SelectFeature()
        {
            _editorUI.OnSelectFeature(this);
        }

        public virtual void OnInit(MapEditorUI editorUI)
        {
            _icon = GetComponent<Image>();
            _selectBtn = GetComponent<Button>();
            _editorUI = editorUI;
            _selectBtn.onClick.AddListener(SelectFeature);
        }

        protected void ActiveSelector()
        {
            if (_editorUI.Scene != null)
            {
                _selector = _editorUI.Scene.FindEntity<EditSelector>();
                if (_selector != null)
                {
                    _selector.SetSize(Vector2Int.one);
                    _selector.moveEvent += MoveSelectorHandler;
                    _selector.selectEvent += SelectSelectorHandler;
                    _selector.Show();
                }
                else
                {
                    Debug.LogError("selector is null");
                }
            }
            else
            {
                Debug.LogError("editorUI scene is null");
            }
        }

        protected virtual void MoveSelectorHandler(Vector2Int index)
        {

        }

        private void SelectSelectorHandler(SelectorTouchType touchType, Vector2Int index)
        {
            switch (touchType)
            {
                case SelectorTouchType.Repeat:
                    SelectRepeatSelectorHandler(index);
                    break;

                case SelectorTouchType.Click:
                    SelectClickSelectorHandler(index);
                    break;
            }
        }

        protected virtual void SelectRepeatSelectorHandler(Vector2Int index)
        {
        }

        protected virtual void SelectClickSelectorHandler(Vector2Int index)
        {
        }

        public virtual void OnEnter()
        {
            _icon.color = Color.cyan;
            _featureActive = true;
        }

        public virtual void OnExit()
        {
            _featureActive = false;
            _icon.color = Color.white;

            if (_selector != null)
            {
                _selector.GetCom<EditSpritePreview>().ClearItem();
                _selector.Hide();
                _selector.moveEvent -= MoveSelectorHandler;
                _selector.selectEvent -= SelectSelectorHandler;
                _selector = null;
            }
        }

        public virtual void OnSceneLoad(MapEditorEntity scene)
        {

        }

        public virtual void OnSceneExit()
        {
            _selector = null;
        }

        public virtual void OnSelectGrid(GridEntity grid)
        {

        }

        public void SetEnable(bool enable)
        {
            gameObject.SetActive(enable);
        }
    }
}
#endif