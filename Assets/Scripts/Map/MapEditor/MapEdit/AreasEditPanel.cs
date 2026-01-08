#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MH.GameScene.Runtime.Entities;
using TMPro;
using MM.MapEditors;

namespace MH.GameScene.UIs.MapEdit
{
    public class AreasEditPanel : FeaturePanelBase
    {
        [SerializeField] private RectTransform itemNode;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Button addBtn;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button paintBtn;
        [SerializeField] private Image paintBtnIcon;
        [SerializeField] private Button eraseBtn;
        [SerializeField] private Image eraseBtnIcon;
        [SerializeField] private Color selectColor;
        [SerializeField] private Color unselectColor;

        private bool _paintMode;
        private Dictionary<int, AreaItem> _areas;

        public bool PaintMode => _paintMode;

        public AreaItem Current { get; private set; }

        public override void OnInit(MapEditorUI editorUI)
        {
            base.OnInit(editorUI);

            _areas = new Dictionary<int, AreaItem>();
            itemPrefab.SetActive(false);
            addBtn.onClick.AddListener(AddItemHandler);
            paintBtn.onClick.AddListener(() =>
            {
                if (_paintMode) return;
                _paintMode = true;
                RefreshMode();
            });
            eraseBtn.onClick.AddListener(() =>
            {
                if (!_paintMode) return;
                _paintMode = false;
                RefreshMode();
            });
            _paintMode = true;
            RefreshMode();
        }

        public void RefreshAreas()
        {
            foreach (AreaItem item in _areas.Values)
                GameObject.Destroy(item.gameObject);
            _areas.Clear();

            IAreaModule areaModule = _editorUI.Scene.FindEntity<IAreaModule>();
            foreach (IArea area in areaModule.Areas)
            {
                InitArea(area);
            }
        }

        private void RefreshMode()
        {
            if (_paintMode)
            {
                paintBtnIcon.color = selectColor;
                eraseBtnIcon.color = unselectColor;
            }
            else
            {
                paintBtnIcon.color = unselectColor;
                eraseBtnIcon.color = selectColor;
            }
        }

        private void AddItemHandler()
        {
            if (int.TryParse(input.text, out int areaId))
            {
                IMapScene mapScene = _editorUI.World.FindEntity<IMapScene>();
                IAreaModule areaModule = mapScene.FindEntity<IAreaModule>();
                if (!areaModule.Contains(areaId))
                {
                    IArea area = areaModule.AddArea(areaId);
                    InitArea(area);
                }
            }
        }

        private void InitArea(IArea area)
        {
            GameObject areaInst = GameObject.Instantiate(itemPrefab);
            areaInst.SetActive(true);
            AreaItem areaItem = areaInst.GetComponent<AreaItem>();
            areaInst.transform.SetParent(itemNode.transform);

            areaItem.OnInit(area);
            areaItem.RegisterClick(() => SelectItemHandler(areaItem));
            areaItem.RegisterDelete(() => DeleteItemHandler(areaItem));

            AreaView view = area.FindEntity<AreaView>();
            areaItem.ColorChangeEvent += (color) => view.Color = color;

            _areas.Add(area.AreaId, areaItem);
        }

        private void DeleteItemHandler(AreaItem item)
        {
            int areaId = item.Area.AreaId;
            _areas.Remove(areaId);
            IAreaModule areaModule = _editorUI.Scene.FindEntity<IAreaModule>();
            areaModule.RemoveArea(areaId);
            GameObject.Destroy(item.gameObject);
        }

        private void SelectItemHandler(AreaItem item)
        {
            if (Current == item)
            {
                Current?.OnUnselect();
                Current = null;
            }
            else
            {
                Current?.OnUnselect();
                Current = item;
                Current?.OnSelect();
            }
        }
    }
}
#endif