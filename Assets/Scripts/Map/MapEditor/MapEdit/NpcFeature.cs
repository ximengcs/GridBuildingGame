#if UNITY_EDITOR
using MH.GameScene.Runtime.Characters;
using MH.GameScene.Runtime.Entities;
using UnityEngine;

namespace MH.GameScene.UIs.MapEdit
{
    public class NpcFeature : FeatureBase
    {
        private NpcItemsPanel _panel;

        public override void OnEnter()
        {
            base.OnEnter();
            ActiveSelector();
            _panel = _editorUI.ShowPanel<NpcItemsPanel>();
            if (_panel != null)
            {
                _panel.SelectChangeEvent += SelectChangeHandler;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (_panel != null)
            {
                _panel.SelectChangeEvent -= SelectChangeHandler;
            }
        }

        private void SelectChangeHandler()
        {

        }

        protected override void SelectClickSelectorHandler(Vector2Int index)
        {
            base.SelectClickSelectorHandler(index);

            if (_panel.Current != null)
            {
                CharacterModule module = _editorUI.World.GetEntity<CharacterModule>();
                CharacterGenParam param = new CharacterGenParam();
                param.Index = index;
                param.NpcId = _panel.Current.Config.Id;
                module.AddCharacter<Npc>(param);
            }
        }

        protected override void MoveSelectorHandler(Vector2Int index)
        {
            base.MoveSelectorHandler(index);
            _selector.SetIndex(index);
            _selector.Color = new Color(1, 0.25f, 0);
        }
    }
}
#endif