#if UNITY_EDITOR
using UnityEngine;
using MM.MapEditors;
using MH.GameScene.Configs;
using MH.GameScene.Runtime;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.UIs.MapEdit
{
    public class MoveFeature : FeatureBase
    {
        private MapEditorGridScannerPanel _panel;
        private int _curDirection;
        private IItemEntity _current;

        public override void SelectFeature()
        {
            base.SelectFeature();
            _panel = _editorUI.ShowPanel<MapEditorGridScannerPanel>();
            _panel.ItemChangeEvent += ItemChangeHandler;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ActiveSelector();
            InnerRefreshCurrentSelector();
        }

        private void Update()
        {
            if (!_featureActive)
                return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_current != null)
                {
                    ItemConfig config = _editorUI.World.Resource.GetConfig<ItemConfig>(_current.ItemId);
                    _curDirection = MathUtility.RotateDirection(_current.Size, _curDirection, config.Directions);
                    _selector.SetPreviewItem(_current.ItemId);
                    _selector.SetSize(_current.Size, _curDirection);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            _current = null;
            if (_panel != null)
            {
                _panel.UpdateInfo(null);
                _panel.Current = null;
                _panel.ItemChangeEvent -= ItemChangeHandler;
            }
        }

        public override void OnSceneExit()
        {
            base.OnSceneExit();
            _current = null;
        }

        private void ItemChangeHandler()
        {
            _curDirection = GameConst.DIRECTION_RT;
            if (_panel.Current != null)
            {
                _current = _panel.Current.ItemEntity;
                InnerRefreshCurrentSelector();
            }
            else
            {
                _current = null;
                _selector.SetSize(Vector2Int.one);
                _selector.ClearPreviewItem();
            }
        }

        private void InnerRefreshCurrentSelector()
        {
            if (_current != null)
            {
                _curDirection = _current.Direction;
                _selector.SetPreviewItem(_current.ItemId);
                _selector.SetSize(_current.Size, _curDirection);
            }
        }

        protected override void MoveSelectorHandler(Vector2Int index)
        {
            base.MoveSelectorHandler(index);
            _selector.Color = Color.yellow;
            _selector.SetIndex(index);
            _editorUI.UpdateInfo(null, index);
        }

        protected override void SelectClickSelectorHandler(Vector2Int index)
        {
            base.SelectClickSelectorHandler(index);
            if (_current != null)
            {
                EditSpritePreview selectorPreview = _selector.GetCom<EditSpritePreview>();
                ItemGenParam param = selectorPreview.GenParam;
                IMapScene scene = _current.MainGrid.Scene;
                ItemSetResult check = scene.CheckItemSet(param);

                Debug.Log($"Move Check Result {check.Type}");

                switch (check.Type)
                {
                    case ItemSetResultType.Ok:
                        {
                            scene.MoveItem(_current, param, false);
                        }
                        break;
                }
            }
            else
            {
                UpdatePanelInfo(index);
            }
        }

        private void UpdatePanelInfo(Vector2Int index)
        {
            IGridEntity grid = _editorUI.Scene.GetGrid(index);
            _panel.UpdateInfo(grid);
        }
    }
}
#endif