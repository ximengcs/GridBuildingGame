#if UNITY_EDITOR
using UnityEngine;
using MM.MapEditors;
using System.Collections.Generic;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.UIs.MapEdit
{
    public class ScanLayerFeature : FeatureBase, IEditorLayerHelper
    {
        private MapEditorLayerPanel _layerUI;
        private Dictionary<string, bool> _layerSelectState;
        private Dictionary<string, bool> _layerShowState;

        Dictionary<string, bool> IEditorLayerHelper.LayerShowState => _layerShowState;

        Dictionary<string, bool> IEditorLayerHelper.LayerSelectState => _layerSelectState;

        public override void OnInit(MapEditorUI editorUI)
        {
            base.OnInit(editorUI);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _layerUI.SetLayerHelper(this);
            ActiveSelector();
        }

        protected override void MoveSelectorHandler(Vector2Int index)
        {
            base.MoveSelectorHandler(index);
            _selector.Color = Color.blue;
            _selector.SetIndex(index);
            _editorUI.UpdateInfo(null, index);
        }

        public override void OnExit()
        {
            base.OnExit();
            _layerUI?.SetLayerHelper(null);
        }

        public override void OnSceneExit()
        {
            base.OnSceneExit();
            _layerSelectState.Clear();
            _layerShowState.Clear();
        }

        public override void OnSceneLoad(MapEditorEntity scene)
        {
            base.OnSceneLoad(scene);
            _layerShowState = new Dictionary<string, bool>();
            _layerSelectState = new Dictionary<string, bool>();

            IEnumerable<string> layers = scene.GetCom<SceneViewCom>().GetLayers();
            foreach (string layer in layers)
            {
                _layerShowState.Add(layer, true);
                _layerSelectState.Add(layer, true);
            }

            if (_layerUI != null)
            {
                _layerUI.SetLayerHelper(this);
            }
        }

        public void OnShowStateChange(string layer, bool state)
        {
            Debug.Log("show state " + layer);
            _layerShowState[layer] = state;
            if (state)
                _layerUI.ShowLayer(layer);
            else
                _layerUI.HideLayer(layer);
        }

        public void OnSelectStateChange(string layer, bool state)
        {
            Debug.Log("select state " + layer);
            _layerSelectState[layer] = state;
        }

        public override void SelectFeature()
        {
            _layerUI = _editorUI.ShowPanel<MapEditorLayerPanel>();
            base.SelectFeature();
        }

    }
}
#endif