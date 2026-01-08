#if UNITY_EDITOR
using MH.GameScene.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.UIs.MapEdit
{
    public class NpcItemsPanel : FeaturePanelBase
    {
        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private GameObject prefab;

        private List<NpcItemsPanelItem> _items;
        private Action _selectChangeEvent;

        public NpcItemsPanelItem Current { get; set; }

        public event Action SelectChangeEvent
        {
            add => _selectChangeEvent += value; 
            remove => _selectChangeEvent -= value;
        }

        private void Start()
        {
            prefab.SetActive(false);
            _items = new List<NpcItemsPanelItem>();
            var configs = _editorUI.World.Resource.GetConfigs<NpcConfig>();
            foreach (NpcConfig config in configs)
            {
                GameObject inst = GameObject.Instantiate(prefab);
                inst.transform.SetParent(content);
                inst.SetActive(true);
                NpcItemsPanelItem item = inst.GetComponent<NpcItemsPanelItem>();
                item.EditorUI = _editorUI;
                item.Init(config);
                item.OnClick(() => OnClickItem(item));
                _items.Add(item);
            }
        }

        public override void OnShow()
        {
            base.OnShow();
            Current = null;
        }

        private void OnClickItem(NpcItemsPanelItem item)
        {
            Current?.OnUnselect();
            if (Current != item)
            {
                Current = item;
                Current.OnSelect();
            }
            else
            {
                Current = null;
            }

            _selectChangeEvent?.Invoke();
        }
    }
}
#endif