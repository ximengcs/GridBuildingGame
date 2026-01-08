#if UNITY_EDITOR
using System;
using UnityEngine;
using MH.GameScene.Configs;
using MH.GameScene.Runtime;
using System.Collections.Generic;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorGridEditPanel : FeaturePanelBase
    {
        [SerializeField]
        private RectTransform layersNode;

        [SerializeField]
        private GameObject layerPrefab;

        private Action<MapEditorGridItem> _selectChangeEvent;
        private Dictionary<string, MapEditorGridEditPanelItem> layers;

        public MapEditorGridItem Current { get; private set; }

        public event Action<MapEditorGridItem> SelectChangeEvent
        {
            add => _selectChangeEvent += value;
            remove => _selectChangeEvent -= value;
        }

        private void Start()
        {
            layers = new Dictionary<string, MapEditorGridEditPanelItem>();
            layerPrefab.SetActive(false);
            foreach (var config in _editorUI.World.Resource.GetConfigs<ItemConfig>())
            {
                switch (config.Type)
                {
                    case GameConst.TYPE_SURFACE:
                    case GameConst.TYPE_SURFACE_DECORATE:
                    case GameConst.TYPE_COMMON:
                        AddItem(config.Id, config);
                        break;
                }
            }
        }

        public void AddItem(int id, ItemConfig itemTable)
        {
            switch (itemTable.Layer)
            {
                case GameConst.SURFACE_FEAT_LAYER:
                case GameConst.COMMON_FEAT_LAYER: return;
            }

            if (!layers.TryGetValue(itemTable.Layer, out MapEditorGridEditPanelItem layerUI))
            {
                GameObject child = GameObject.Instantiate(layerPrefab);
                child.SetActive(true);
                child.transform.SetParent(layersNode);
                layerUI = child.GetComponent<MapEditorGridEditPanelItem>();
                layerUI.SetTile(itemTable.Layer);
                AddDeleteItem(itemTable.Layer, layerUI);
                layers.Add(itemTable.Layer, layerUI);
            }

            MapEditorGridItem item = layerUI.AddItem(id, itemTable.Layer, itemTable);
            item.OnClick(() => InnerClickItemHandler(item));
        }

        private void AddDeleteItem(string layer, MapEditorGridEditPanelItem layerUI)
        {
            layerUI.EditorUI = _editorUI;
            MapEditorGridItem item = layerUI.AddItem(-1, layer, "x", new MapEditorSelectItem(SelectItemType.Delete, null));
            item.OnClick(() => InnerClickItemHandler(item));
        }

        private void InnerClickItemHandler(MapEditorGridItem item)
        {
            Current?.OnUnselect();
            if (item == Current)
            {
                Current = null;
            }
            else
            {
                Current = item;
                Current.OnSelect();
            }

            _selectChangeEvent?.Invoke(Current);
        }
    }
}
#endif