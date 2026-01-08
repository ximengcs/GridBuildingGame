#if UNITY_EDITOR
using UnityEngine;
using MH.GameScene.Core;
using System.Collections.Generic;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.UIs.MapEdit
{
    public interface IEditorLayerHelper
    {
        Dictionary<string, bool> LayerShowState { get; }

        Dictionary<string, bool> LayerSelectState { get; }

        void OnShowStateChange(string layer, bool state);
        void OnSelectStateChange(string layer, bool state);
    }

    public class MapEditorLayerPanel : FeaturePanelBase
    {
        [SerializeField]
        private Transform layerNode;

        [SerializeField]
        private GameObject layerPrefab;

        private IEditorLayerHelper _layerHelper;
        private Dictionary<string, MapEditorLayerPanelItem> _items;

        public override void OnInit(MapEditorUI editorUI)
        {
            base.OnInit(editorUI);
            _items = new Dictionary<string, MapEditorLayerPanelItem>();
            layerPrefab.SetActive(false);
        }

        public void ShowLayer(string layer)
        {
            SceneViewCom sceneView = _editorUI.Scene.GetCom<SceneViewCom>();
            sceneView.ShowLayer(layer);
        }

        public void HideLayer(string layer)
        {
            SceneViewCom sceneView = _editorUI.Scene.GetCom<SceneViewCom>();
            sceneView.HideLayer(layer);
        }

        public void SetLayerHelper(IEditorLayerHelper helper)
        {
            _layerHelper = helper;

            foreach (var entry in _items)
            {
                entry.Value.OnClear();
            }
            _items.Clear();

            if (helper != null)
            {
                if (helper.LayerShowState != null)
                {
                    foreach (var entry in helper.LayerShowState)
                    {
                        string layer = entry.Key;
                        if (entry.Value)
                            ShowLayer(layer);
                        else
                            HideLayer(layer);

                        GameObject inst = GenItem();
                        MapEditorLayerPanelItem item = inst.GetComponent<MapEditorLayerPanelItem>();
                        item.Init(layer, helper);
                        _items.Add(layer, item);
                    }
                }
            }
            else
            {
                SceneViewCom sceneView = _editorUI.Scene.GetCom<SceneViewCom>();
                sceneView.ShowLayer();
            }
        }

        private GameObject GenItem()
        {
            GameObject result = GameObject.Instantiate(layerPrefab);
            result.transform.SetParent(layerNode);
            result.SetActive(true);
            return result;
        }
    }
}
#endif