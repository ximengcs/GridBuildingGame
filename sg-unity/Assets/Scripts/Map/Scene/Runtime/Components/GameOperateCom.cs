using R3;
using UnityEngine;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Views;
using System.Collections.Generic;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Entities
{
    public class GameOperateCom : ComponentBase
    {
        private Vector2 _lastPos;
        private Vector2 _direciton;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        private float _edgeThreshold = 0.01f;
#elif UNITY_IPHONE || UNITY_ANDROID
        private float _edgeThreshold = 0.1f;
#endif

        private float _edgeMoveSpeed = 0.01f;
        private IWorldCamera _cam;
        private EditSelector _selector;
        private IMapScene _mapScene;
        private IGridSelectable _curSelectItem;

        public override void OnStart()
        {
            base.OnStart();
            _mapScene = (IMapScene)Entity;
            _cam = Entity.World.FindEntity<IWorldCamera>();
            _selector = Entity.AddEntity<EditSelector>();
            _selector.Start();

            _edgeThreshold = _edgeThreshold * Screen.width;

            this.OnSceneLongPressStartAsObservable().Subscribe(TriggerClickItem);
            this.OnSceneLongPressingAsObservable().Subscribe((screenPoint) =>
            {
                if (_curSelectItem == null)
                {
                    MoveCamera(screenPoint);
                    return;
                }

                Vector2Int index = _selector.GetOffsetIndex(screenPoint);
                _selector.SetIndex(index);

                IItemEntity item = _curSelectItem.Item;
                IGridEntity grid = item.MainGrid;
                ItemGenParam param = GetGenParam(screenPoint);
                ItemSetResult check = _mapScene.CheckItemSet(param, true, grid.Index, GridCheckHandler);
                switch (check.Type)
                {
                    case ItemSetResultType.SelfItemOk:
                        _selector.SetColors(null);
                        break;

                    default:
                        _selector.SetColors(GenEditorColor(check));
                        break;
                }

                if (CheckScreenEdge(screenPoint, out Vector2 direction))
                {
                    _cam.SetPos(direction);
                }
            });
            this.OnSceneLongPressEndAsObservable().Subscribe(TriggerPutItem);
            this.OnSceneShortPressStartAsObservable().Subscribe(RecordScreenPoint);
            this.OnSceneShortPressingAsObservable().Subscribe(MoveCamera);
            this.OnSceneShortPressEndAsObservable().Subscribe(MoveCameraEnd);
            this.OnSceneClickAsObservable().Subscribe(TriggerClickItem);
            this.OnSceneScaleAsObservable().Subscribe(_cam.SetSize);
        }

        private bool CheckScreenEdge(Vector2 screenPos, out Vector2 direction)
        {
            if (_lastPos != screenPos)
                _direciton = _lastPos - screenPos;
            direction = _direciton * Time.deltaTime * _edgeMoveSpeed;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 检查是否在屏幕边缘
            if (screenPos.x <= _edgeThreshold)
                return true;
            else if (screenPos.x >= screenWidth - _edgeThreshold)
                return true;

            if (screenPos.y <= _edgeThreshold)
                return true;
            else if (screenPos.y >= screenHeight - _edgeThreshold)
                return true;

            return false;
        }

        private void MoveCamera(Vector2 screenPoint)
        {
            Vector3 camPos = Camera.main.ScreenToWorldPoint(screenPoint);
            camPos -= Camera.main.ScreenToWorldPoint(_lastPos);
            camPos.z = 0;
            if (camPos != Vector3.zero)
                _cam.SetPos(camPos);

            RecordScreenPoint(screenPoint);
        }

        private void MoveCameraEnd(Vector2 screenPoint)
        {
            Vector3 camPos = Camera.main.ScreenToWorldPoint(screenPoint);
            camPos -= Camera.main.ScreenToWorldPoint(_lastPos);
            camPos.z = 0;
            _cam.SetPosEnd(camPos);
        }

        private Dictionary<Vector2Int, Color> GenEditorColor(ItemSetResult check)
        {
            if (check.CheckGrids == null)
                return null;

            Dictionary<Vector2Int, Color> colors = new Dictionary<Vector2Int, Color>();
            foreach (var entry in check.CheckGrids)
            {
                if (entry.Key == ItemSetResultType.SelfItemOk)
                    continue;

                foreach (Vector2Int index in entry.Value)
                {
                    if (!colors.ContainsKey(index))
                        colors.Add(index, Color.red);
                }
            }
            return colors;
        }

        private void RecordScreenPoint(Vector2 screenPoint)
        {
            _lastPos = screenPoint;
        }

        private void TriggerClickItem(Vector2 screenPoint)
        {
            IGridSelectable selectable = GetSelectable(screenPoint);
            if (_curSelectItem == null || _curSelectItem != selectable)
            {
                UncancelCurrent();
                _curSelectItem = selectable;
            }

            if (_curSelectItem != null)
            {
                SelectCurrent();
                RefreshPreview();
            }
            else
            {
                _selector.Hide();
                _selector.ClearPreviewItem();
            }
            RecordScreenPoint(screenPoint);
        }

        private void TriggerPutItem(Vector2 screenPoint)
        {
            if (_curSelectItem == null)
                return;

            IItemEntity item = _curSelectItem.Item;
            IGridEntity grid = item.MainGrid;

            ItemGenParam param = GetGenParam(screenPoint);
            ItemSetResult check = _mapScene.CheckItemSet(param, true, grid.Index, GridCheckHandler);
            Debug.Log($"check type {check.Type}");
            switch (check.Type)
            {
                case ItemSetResultType.Ok:
                case ItemSetResultType.SelfItemOk:
                    _mapScene.MoveItem(item, param, false);
                    break;
            }

            _selector.Hide();
            _selector.ClearPreviewItem();
            UncancelCurrent();
        }

        private ItemGenParam GetGenParam(Vector2 screenPoint)
        {
            Vector2Int index = _selector.GetOffsetIndex(screenPoint);
            IItemEntity item = _curSelectItem.Item;
            ItemGenParam param = new ItemGenParam(item);
            param.Index = index;
            return param;
        }

        private void GridCheckHandler(IItemEntity selfItem, IGridEntity grid, ItemGenParam genParam, ICollection<IItemEntity> result)
        {
            if (grid.InGrid(selfItem))
            {
                result.Add(selfItem);
            }
            else
            {
                IItemEntity item = grid.GetItem(GameConst.SURFACEDECORATE_LAYER);
                if (item != null) result.Add(item);
                item = grid.GetItem(GameConst.COMMON_LAYER);
                if (item != null) result.Add(item);
            }
        }

        private IGridSelectable GetSelectable(Vector2 screenPoint)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(screenPoint);
            Vector2Int index = MathUtility.GamePosToIndex(pos);
            IMapScene mapScene = (IMapScene)Entity;
            IGridEntity grid = mapScene.GetGrid(index);

            IGridSelectable selectable = null;
            foreach (IItemEntity item in grid.Items)
            {
                selectable = item.FindCom<IGridSelectable>();
                if (selectable != null)
                    break;
            }

            return selectable;
        }

        private void UncancelCurrent()
        {
            _curSelectItem?.OnUnSelect();
            _curSelectItem = null;
        }

        private void SelectCurrent()
        {
            _curSelectItem.OnSelect();
        }

        private void RefreshPreview()
        {
            if (_curSelectItem == null)
                return;

            IItemEntity item = _curSelectItem.Item;
            _selector.SetPreviewItem(item.ItemId);
            _selector.SetSize(item.Size, item.Direction);
            _selector.SetIndex(item.MainGrid.Index);
            _selector.Show();
        }
    }
}
