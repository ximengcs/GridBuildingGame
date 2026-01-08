#if UNITY_EDITOR
using UnityEngine;
using MH.GameScene.Runtime.Entities;
using MM.MapEditors;

namespace MH.GameScene.UIs.MapEdit
{
    public class AreaFeature : FeatureBase
    {
        private AreasEditPanel _panel;

        public override void OnEnter()
        {
            base.OnEnter();
            _editorUI.ShowPanel<AreasEditPanel>();
            ActiveSelector();
        }

        public override void OnSceneLoad(MapEditorEntity scene)
        {
            base.OnSceneLoad(scene);
            _panel = _editorUI.GetPanel<AreasEditPanel>();
            _panel.RefreshAreas();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_selector != null)
            {
                _selector.Hide();
                _selector.ClearPreviewItem();
            }
        }

        protected override void MoveSelectorHandler(Vector2Int index)
        {
            base.MoveSelectorHandler(index);
            _selector.SetIndex(index);
        }

        protected override void SelectRepeatSelectorHandler(Vector2Int index)
        {
            base.SelectRepeatSelectorHandler(index);
            AreaItem areaItem = _panel.Current;
            if (areaItem != null)
            {
                IGridEntity grid = _editorUI.Scene.GetGrid(index);
                if (grid != null)
                {
                    if (_panel.PaintMode)
                        areaItem.Area.Add(grid);
                    else
                        areaItem.Area.Remove(grid);
                }
            }
        }
    }
}
#endif