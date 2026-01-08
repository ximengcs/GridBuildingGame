using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Views
{
    public class NpcItemView : CharacterSpriteView
    {
        private Npc _npcEntity;

        public override void OnStart()
        {
            base.OnStart();
            _npcEntity = (Npc)Entity;
            _npcEntity.PositionChangeEvent += PositionChangeHandler;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _npcEntity.PositionChangeEvent -= PositionChangeHandler;
        }

        private void PositionChangeHandler()
        {
            SetIndex(_npcEntity.Index);
        }
    }
}
