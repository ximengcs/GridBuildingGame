#if UNITY_EDITOR
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MH.GameScene.Runtime.Entities;
using MM.MapEditors;
using SgFramework.Utility;

namespace MH.GameScene.UIs.MapEdit
{
    public class AreaItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image bg;
        [SerializeField] private Button selectBtn;
        [SerializeField] private RectTransform colorsNode;
        [SerializeField] private Button deleteBtn;

        private Color _originColor;
        private Action _deleteHandler;
        private Action _clickHandler;
        private Image _curColorIcon;
        private Action<Color> _colorChangeEvent;

        public event Action<Color> ColorChangeEvent
        {
            add { _colorChangeEvent += value; }
            remove { _colorChangeEvent -= value; }
        }

        public IArea Area { get; private set; }

        public Color SelectColor => _curColorIcon.color;

        public void OnInit(IArea area)
        {
            Area = area;
            title.text = $"{area.AreaId}";
            _originColor = bg.color;

            AreaView view = area.FindEntity<AreaView>();
            Color currentColor = view.Color;
            currentColor.a = 1;
            bool findColor = false;

            foreach (Transform child in colorsNode)
            {
                Button btn = child.GetComponent<Button>();
                Image img = btn.GetComponent<Image>();
                btn.onClick.AddListener(() =>
                {
                    Image selectIcon = _curColorIcon.transform.GetChild(0).GetComponent<Image>();
                    selectIcon.enabled = false;
                    _curColorIcon = img;
                    selectIcon = _curColorIcon.transform.GetChild(0).GetComponent<Image>();
                    selectIcon.enabled = true;
                    _colorChangeEvent?.Invoke(_curColorIcon.color);
                });

                if (_curColorIcon == null)
                {
                    _curColorIcon = img;
                }

                if (currentColor == img.color)
                {
                    _curColorIcon = img;
                    findColor = true;
                }
            }

            if (!findColor)
                view.Color = _curColorIcon.color;
            Image selectIcon = _curColorIcon.transform.GetChild(0).GetComponent<Image>();
            selectIcon.enabled = true;

            selectBtn.onClick.AddListener(() => _clickHandler?.Invoke());
            deleteBtn.onClick.AddListener(() => _deleteHandler?.Invoke());
        }

        public void RegisterDelete(Action handler)
        {
            _deleteHandler = handler;
        }

        public void RegisterClick(Action handler)
        {
            _clickHandler = handler;
        }

        public void OnSelect()
        {
            bg.color = Color.cyan;
        }

        public void OnUnselect()
        {
            bg.color = _originColor;
        }
    }
}
#endif