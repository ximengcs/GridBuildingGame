using R3.Triggers;
using UnityEngine;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Utilities
{
    [DisallowMultipleComponent]
    public class ObservableCommonColliderTrigger : ObservableTriggerBase, IIntersect
    {
        private ObservableCommonTrigger _trigger;

        public ObservableCommonTrigger Observable => _trigger ??= new ObservableCommonTrigger();

        void IIntersect.OnMoving(Vector2 screenPos)
        {
            _trigger.OnMoving(screenPos);
        }

        void IIntersect.OnPointDown(Vector2 screenPos)
        {
            _trigger.OnPointDown(screenPos);
        }

        void IIntersect.OnPointing(Vector2 screenPos)
        {
            _trigger.OnPointing(screenPos);
        }

        void IIntersect.OnPointUp(Vector2 screenPos)
        {
            _trigger.OnPointUp(screenPos);
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _trigger.OnDestroy();
            _trigger = null;
        }
    }
}