using UnityEngine;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Utilities
{
    public class ObservableCommonGridTrigger : ComponentBase, IScenentersect
    {
        private ITriggerModule _triggerModule;
        private ObservableCommonTrigger _trigger;

        public ObservableCommonTrigger Observable => _trigger ??= new ObservableCommonTrigger();

        public override void OnInit(Entity entity, object data)
        {
            base.OnInit(entity, data);
            _triggerModule = (ITriggerModule)data;
        }

        public override void OnStart()
        {
            base.OnStart();
            _triggerModule.Register(this);
        }

        void IScenentersect.OnMoving(Vector2 screenPos)
        {
            _trigger.OnMoving(screenPos);
        }

        void IScenentersect.OnPointDown(Vector2 screenPoint)
        {
            _trigger.OnPointDown(screenPoint);
        }

        void IScenentersect.OnPointing(Vector2 pos)
        {
            _trigger.OnPointing(pos);
        }

        void IScenentersect.OnPointUp(Vector2 pos)
        {
            _trigger.OnPointUp(pos);
        }

        void IScenentersect.OnScale(float value)
        {
            _trigger.OnScale(value);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _triggerModule.Unregister(this);
            _trigger.OnDestroy();
            _triggerModule = null;
            _trigger = null;
        }
    }
}
