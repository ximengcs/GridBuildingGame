#if UNITY_EDITOR
using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorGridScannerPanel : FeaturePanelBase
    {
        [SerializeField]
        private TextMeshProUGUI indexText;

        [SerializeField]
        private RectTransform itemNode;

        [SerializeField]
        private GameObject itemPrefab;

        private GameObject _indexInst;
        private List<GameObject> _items;
        private Action _itemChangeEvent;

        public MapEditorGridScannerPanelItem Current { get; set; }

        public event Action ItemChangeEvent
        {
            add => _itemChangeEvent += value;
            remove => _itemChangeEvent -= value;
        }

        private void Start()
        {
            _items = new List<GameObject>();
            _indexInst = indexText.transform.parent.gameObject;
            _indexInst.SetActive(false);
            itemPrefab.SetActive(false);
        }

        public void UpdateInfo(IGridEntity grid)
        {
            foreach (GameObject inst in _items)
                GameObject.Destroy(inst);
            _items.Clear();

            if (grid != null)
            {
                _indexInst.SetActive(true);
                indexText.text = $"{grid.Index.x}, {grid.Index.y}";
                foreach (IItemEntity entity in grid.Items)
                {
                    GameObject inst = GameObject.Instantiate(itemPrefab);
                    inst.SetActive(true);
                    inst.transform.SetParent(itemNode.transform);
                    MapEditorGridScannerPanelItem item = inst.GetComponent<MapEditorGridScannerPanelItem>();
                    item.Init(entity, InnerItemSelectHandler);
                    _items.Add(inst);
                }
            }
            else
            {
                _indexInst.SetActive(false);
            }
        }

        private void InnerItemSelectHandler(MapEditorGridScannerPanelItem item)
        {
            if (Current == item)
            {
                Current.OnUnselect();
                Current = null;
            }
            else
            {
                Current?.OnUnselect();
                Current = item;
                Current.OnSelect();
            }

            _itemChangeEvent?.Invoke();
        }
    }
}
#endif