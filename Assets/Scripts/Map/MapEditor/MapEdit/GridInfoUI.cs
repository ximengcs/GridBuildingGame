#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.UIs.MapEdit
{
    public class GridInfoUI : MonoBehaviour
    {
        [SerializeField]
        private Transform layerNode;

        [SerializeField]
        private GameObject layerPrefab;

        [SerializeField]
        private TextMeshProUGUI title;

        private RectTransform _uiRect;
        private float _height;
        private MapEditorUI _editorUI;
        private List<GameObject> _layers;
        private Vector2Int _index;

        public void OnInit(MapEditorUI editorUI)
        {
            _editorUI = editorUI;
            _layers = new List<GameObject>();
            layerPrefab.SetActive(false);
            _uiRect = GetComponent<RectTransform>();
            _height = _uiRect.sizeDelta.y;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void UpdateInfo(string selectLayer, Vector2Int index)
        {
            if (index == this._index)
                return;
            _index = index;
            foreach (GameObject layer in _layers)
                GameObject.Destroy(layer);
            _layers.Clear();

            title.text = $"{index.x}, {index.y}";
            IGridEntity grid = _editorUI.Scene.GetGrid(index);
            if (grid != null)
            {
                Show();

                int layerCount = 0;
                if (selectLayer != null)
                {
                    IItemEntity item = grid.GetItem(selectLayer);
                    if (item != null)
                    {
                        GenItem(item);
                        layerCount = 1;
                    }
                }
                else
                {
                    foreach (IItemEntity item in grid.Items)
                    {
                        GenItem(item);
                    }
                    layerCount = grid.LayerCount;
                }

                _uiRect.sizeDelta = new Vector2(_uiRect.sizeDelta.x, _height + 110 * (layerCount - 1));
            }
            else
            {
                Hide();
            }
        }

        private void GenItem(IItemEntity item)
        {
            GameObject inst = GameObject.Instantiate(layerPrefab);
            inst.SetActive(true);
            inst.transform.SetParent(layerNode.transform);
            GridInfoLayerItem layerItem = inst.GetComponent<GridInfoLayerItem>();
            _ = layerItem.Set(item);
            _layers.Add(inst);
        }
    }
}
#endif