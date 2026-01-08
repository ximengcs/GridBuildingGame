#if UNITY_EDITOR
using UnityEngine;
using MM.MapEditors;
using MH.GameScene.Configs;
using MH.GameScene.Runtime.Utilities;
using MH.GameScene.Runtime;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.UIs.MapEdit
{
    public class PaintFeature : FeatureBase
    {
        private MapEditorGridEditPanel _layerUI;
        private int _curDirection;

        public override void SelectFeature()
        {
            _layerUI = _editorUI.ShowPanel<MapEditorGridEditPanel>();
            _layerUI.SelectChangeEvent += SelectItemChangeHandler;
            base.SelectFeature();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ActiveSelector();
            RefreshSelector();
        }

        private void Update()
        {
            if (!_featureActive)
                return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                MapEditorGridItem item = _layerUI.Current;
                if (item != null)
                {
                    ItemConfig itemConfig = item.Data.Config;
                    _curDirection = MathUtility.RotateDirection(itemConfig.Size, _curDirection, itemConfig.Directions);
                    _selector.SetSize(itemConfig.Size, _curDirection);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_layerUI != null)
                _layerUI.SelectChangeEvent -= SelectItemChangeHandler;
        }

        private void SelectItemChangeHandler(MapEditorGridItem change)
        {
            _curDirection = GameConst.DIRECTION_RT;
            RefreshSelector();
        }

        private void RefreshSelector()
        {
            MapEditorGridItem item = _layerUI.Current;
            if (item == null)
            {
                _selector.SetSize(Vector2Int.one);
                _selector.GetCom<EditSpritePreview>().ClearItem();
                return;
            }

            switch (item.Data.SelectType)
            {
                case SelectItemType.Common:
                    {
                        ItemConfig itemConfig = item.Data.Config;
                        _selector.SetPreviewItem(item.ItemId);
                        _selector.SetSize(itemConfig.Size, _curDirection);
                    }
                    break;

                case SelectItemType.Delete:
                    {
                        _selector.SetSize(Vector2Int.one);
                        _selector.ClearPreviewItem();
                    }
                    break;
            }
        }

        protected override void MoveSelectorHandler(Vector2Int index)
        {
            base.MoveSelectorHandler(index);

            string selectLayer = null;
            var cur = _layerUI.Current;
            if (cur != null)
            {
                switch (cur.Data.SelectType)
                {
                    case SelectItemType.Common:
                        _selector.Color = Color.green;
                        selectLayer = cur.Layer;
                        break;

                    case SelectItemType.Delete:
                        _selector.Color = Color.red;
                        selectLayer = cur.Layer;
                        break;
                }
            }
            else
            {
                _selector.Color = Color.cyan;
            }

            _selector.SetIndex(index);

            _editorUI.UpdateInfo(selectLayer, index);
        }

        protected override void SelectRepeatSelectorHandler(Vector2Int index)
        {
            base.SelectRepeatSelectorHandler(index);
            MapEditorGridItem item = _layerUI.Current;
            if (item == null)
                return;

            var cur = _layerUI.Current;
            if (cur != null)
            {
                switch (cur.Data.SelectType)
                {
                    case SelectItemType.Common:
                        SelectCommonItemHandler(index);
                        break;

                    case SelectItemType.Delete:
                        SelectDeleteHandler(index);
                        break;
                }
            }
        }

        private void SelectDeleteHandler(Vector2Int index)
        {
            MapEditorGridItem item = _layerUI.Current;
            string layer = item.Layer;
            _editorUI.Scene.RemoveItem(index, layer);
        }

        private void SelectCommonItemHandler(Vector2Int index)
        {
            MapEditorGridItem item = _layerUI.Current;
            ItemGenParam param = new ItemGenParam();
            param.Index = index;
            param.ItemId = item.ItemId;
            ItemConfig itemConfig = item.Data.Config;
            param.Layer = itemConfig.Layer;
            param.Size = itemConfig.Size;
            param.Direction = _curDirection;
            ItemSetResult check = _editorUI.Scene.CheckItemSet(param);

            switch (check.Type)
            {
                case ItemSetResultType.Ok:
                    _editorUI.Scene.SetItem(param, false);
                    break;

                case ItemSetResultType.NoGrid:
                    _editorUI.Scene.SetItem(param, true);
                    break;
            }
        }
    }
}
#endif