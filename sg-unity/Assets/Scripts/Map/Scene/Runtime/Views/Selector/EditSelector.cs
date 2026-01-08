using R3;
using System;
using UnityEngine;
using System.Threading;
using MH.GameScene.Runtime;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Views
{
    public class EditSelector : Entity
    {
        private GameObject _root;
        private GameObject _renderPrefab;
        private Color _renderColor;
        private Dictionary<Vector2Int, SpriteRenderer> _renderers;
        private Dictionary<Vector2Int, Color> _renderColors;
        private Vector2Int _size;
        private Vector2Int _index;
        private int _direction;
        private bool _dirty;
        private bool _showState;
        private IEditPreviewItem _previewItem;
        private Action<SelectorTouchType, Vector2Int> _selectEvent;
        private Action<Vector2Int> _moveEvent;

        public Color Color
        {
            get => _renderColor;
            set
            {
                _renderColor = value;
                RefreshRenderColor();
            }
        }

        public event Action<SelectorTouchType, Vector2Int> selectEvent
        {
            add => _selectEvent += value;
            remove => _selectEvent -= value;
        }

        public event Action<Vector2Int> moveEvent
        {
            add => _moveEvent += value;
            remove => _moveEvent -= value;
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _previewItem = AddCom<EditSpritePreview>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            UniTask.Create(InitAsync, _destroyTokenSource.Token);
        }

        public void SetColors(Dictionary<Vector2Int, Color> colors)
        {
            _renderColors = colors;
            RefreshRenderColor();
        }

        private async UniTask InitAsync(CancellationToken token)
        {
            _root = await World.Resource.LoadObject("Selector");
            token.ThrowIfCancellationRequested();
            SpriteRenderer r = _root.GetComponentInChildren<SpriteRenderer>();
            _renderPrefab = r.gameObject;
            _renderPrefab.SetActive(false);
            _renderers = new();
            _renderColor = r.color;
            if (_dirty)
                SetSize(_size, _direction);
            RefreshIndex();

            this.OnSceneClickAsObservable().Subscribe((screenPos) =>
            {
                _selectEvent?.Invoke(SelectorTouchType.Click, GetOffsetIndex(screenPos));
            });
            this.OnScenePointingAsObservable().Subscribe((screenPos) =>
            {
                _selectEvent?.Invoke(SelectorTouchType.Repeat, GetOffsetIndex(screenPos));
            });

            TriggerExtension.TriggerModule.CheckMove = true;
            this.OnSceneMovingAsObservable().Subscribe((screenPos) =>
            {
                _moveEvent?.Invoke(GetOffsetIndex(screenPos));
            });
        }

        public Vector2Int GetOffsetIndex(Vector2 screenPos)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.y -= _previewItem.SpriteRender.GetOffset();
            Vector2Int index = MathUtility.GamePosToIndex(worldPos);
            return index;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TriggerExtension.TriggerModule.CheckMove = false;
            if (_root != null)
            {
                GameObject.Destroy(_root);
                _root = null;
            }

            _renderPrefab = null;
            _renderers = null;
        }

        public void SetSize(Vector2Int size, int direction = GameConst.DIRECTION_RT)
        {
            if (!_dirty && _size == size && _direction == direction)
                return;
            _size = size;
            _direction = direction;

            if (_renderers == null)
                return;

            _dirty = false;
            foreach (var renderer in _renderers.Values)
            {
                GameObject inst = renderer.gameObject;
                GameObject.Destroy(inst);
            }

            _renderers.Clear();

            MathUtility.GetDirectionRange(Vector2Int.zero, size, direction,
                out Vector2Int xRange, out Vector2Int yRange);

            for (int y = yRange.x; y < yRange.y; y++)
            {
                for (int x = xRange.x; x < xRange.y; x++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    GameObject inst = GameObject.Instantiate(_renderPrefab);
                    inst.SetActive(true);
                    inst.transform.SetParent(_root.transform);
                    inst.transform.localPosition = MathUtility.IndexToGamePos(index) + new Vector2(0, -0.25f);
                    SpriteRenderer render = inst.GetComponent<SpriteRenderer>();
                    _renderers.Add(index, render);
                }
            }

            RefreshRenderColor();
            _previewItem.Refresh(_index, size, direction, _destroyTokenSource.Token);
        }

        private void RefreshRenderColor()
        {
            if (_renderers == null)
                return;

            if (_renderColors != null)
            {
                foreach (var entry in _renderers)
                {
                    if (_renderColors.TryGetValue(entry.Key + _index, out Color color))
                        entry.Value.color = color;
                    else
                        entry.Value.color = _renderColor;
                }
            }
            else
            {
                foreach (var entry in _renderers)
                {
                    entry.Value.color = _renderColor;
                }
            }
        }

        public void SetIndex(Vector2Int index)
        {
            if (_index == index) return;

            _index = index;
            if (_root == null)
                return;

            RefreshIndex();
        }

        private void RefreshIndex()
        {
            _root.SetActive(_showState);
            Vector3 worldPos = MathUtility.IndexToGamePos(_index);
            worldPos.z = 0;
            var tf = _root.transform;
            tf.position = worldPos;

            _previewItem.Refresh(_index, _size, _direction, _destroyTokenSource.Token);
        }

        public void SetPreviewItem(int itemId)
        {
            _dirty = true;
            _previewItem.SetItem(itemId);
        }

        public void ClearPreviewItem()
        {
            _previewItem.ClearItem();
        }

        public void Show()
        {
            _showState = true;
            if (_root)
                _root.SetActive(_showState);
        }

        public void Hide()
        {
            _showState = false;
            if (_root)
                _root.SetActive(_showState);
        }
    }
}