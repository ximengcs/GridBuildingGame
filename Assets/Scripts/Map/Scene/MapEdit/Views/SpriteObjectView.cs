#if UNITY_EDITOR
using MH.GameScene.Runtime.Utilities;
using MH.GameScene.Runtime.Views;
using R3;
using System;
using UnityEngine;

namespace MM.MapEditors
{
    public class SpriteObjectView : IObjectView
    {
        private GameObject _obj;
        private Sprite _sprite;
        private SpriteRenderer _spriteRender;

        private Vector2Int _size;
        private int _direction;
        private Color _color;
        private string _sortingLayer;
        private int _sortingOrder;
        private Action _clickHandler;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                if (_spriteRender)
                    _spriteRender.color = _color;
            }
        }

        public string SortingLayer
        {
            get => _sortingLayer;
            set
            {
                _sortingLayer = value;
                if (_spriteRender)
                    _spriteRender.sortingLayerName = value;
            }
        }
        public int SortingOrder
        {
            get => _sortingOrder;
            set
            {
                _sortingOrder = value;
                if (_spriteRender)
                    _spriteRender.sortingOrder = value;
            }
        }

        public SpriteObjectView(Sprite sprite, Vector2Int size, int direction)
        {
            _obj = new GameObject();
            _spriteRender = _obj.AddComponent<SpriteRenderer>();
            _sprite = sprite;
            _size = size;
            _direction = direction;
            _spriteRender.sprite = _sprite;
            _obj.AddComponent<PolygonCollider2D>();
            _obj.transform.OnSceneClickAsObservable().Subscribe((unit) =>
            {
                _clickHandler?.Invoke();
            });
        }

        public void RegisterClick(Action handler)
        {
            _clickHandler = handler;
        }

        public void SetParent(Transform layerRoot)
        {
            _obj.transform.SetParent(layerRoot);
        }

        public void SetIndex(Vector2Int index)
        {
            _obj.transform.position = MathUtility.GetGamePos(index, _size, _direction);
        }

        public void OnDestroy()
        {
            GameObject.Destroy(_obj);
        }
    }
}
#endif