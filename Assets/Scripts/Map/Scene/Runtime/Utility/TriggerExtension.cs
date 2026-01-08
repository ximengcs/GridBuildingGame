using R3;
using UnityEngine;
using MH.GameScene.Core.Entites;

namespace MH.GameScene.Runtime.Utilities
{
    public static class TriggerExtension
    {
        private static ITriggerModule s_TriggerModule;

        public static ITriggerModule TriggerModule => s_TriggerModule;

        public static void Initialize(ITriggerModule triggerModule)
        {
            s_TriggerModule = triggerModule;
        }

        public static Observable<Vector2> OnScenePointDownAsObservable(this IComponent component)
        {
            return OnScenePointDownAsObservable(component.Entity);
        }

        public static Observable<Vector2> OnScenePointDownAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnPointDownAsObservable();
        }

        public static Observable<Vector2> OnScenePointingAsObservable(this IComponent component)
        {
            return OnScenePointingAsObservable(component.Entity);
        }

        public static Observable<Vector2> OnSceneMovingAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnMovingAsObservable();
        }

        public static Observable<Vector2> OnScenePointingAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnPointingAsObservable();
        }

        public static Observable<Vector2> OnScenePointUpAsObservable(this IComponent component)
        {
            return OnScenePointUpAsObservable(component.Entity);
        }

        public static Observable<Vector2> OnScenePointUpAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnPointUpAsObservable();
        }

        public static Observable<Vector2> OnSceneLongPressStartAsObservable(this IComponent component, float time = 0.5f)
        {
            return OnSceneLongPressStartAsObservable(component.Entity, time);
        }

        public static Observable<Vector2> OnSceneLongPressingAsObservable(this IComponent component, float time = 0.5f)
        {
            return OnSceneLongPressingAsObservable(component.Entity, time);
        }

        public static Observable<Vector2> OnSceneLongPressEndAsObservable(this IComponent component, float time = 0.5f)
        {
            return OnSceneLongPressEndAsObservable(component.Entity, time);
        }

        public static Observable<Vector2> OnSceneLongPressStartAsObservable(this IEntity entity, float time = 0.5f)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            trigger.Observable.LongPressTime = time;
            return trigger.Observable.OnLongPressStartAsObservable();
        }

        public static Observable<Vector2> OnSceneLongPressingAsObservable(this IEntity entity, float time = 0.5f)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            trigger.Observable.LongPressTime = time;
            return trigger.Observable.OnLongPressingAsObservable();
        }

        public static Observable<Vector2> OnSceneLongPressEndAsObservable(this IEntity entity, float time = 0.5f)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            trigger.Observable.LongPressTime = time;
            return trigger.Observable.OnLongPressEndAsObservable();
        }

        public static Observable<Vector2> OnSceneShortPressStartAsObservable(this IComponent component)
        {
            return OnSceneShortPressStartAsObservable(component.Entity);
        }

        public static Observable<Vector2> OnSceneShortPressingAsObservable(this IComponent component)
        {
            return OnSceneShortPressingAsObservable(component.Entity);
        }

        public static Observable<Vector2> OnSceneShortPressEndAsObservable(this IComponent component)
        {
            return OnSceneShortPressEndAsObservable(component.Entity);
        }


        public static Observable<Vector2> OnSceneShortPressStartAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnShortPressStartAsObservable();
        }

        public static Observable<Vector2> OnSceneShortPressingAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnShortPressingAsObservable();
        }

        public static Observable<Vector2> OnSceneShortPressEndAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnShortPressEndAsObservable();
        }

        public static Observable<Vector2> OnSceneClickAsObservable(this IComponent component)
        {
            return OnSceneClickAsObservable(component.Entity);
        }

        public static Observable<Vector2> OnSceneClickAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<Vector2>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnClickAsObservable();
        }

        public static Observable<float> OnSceneScaleAsObservable(this IComponent component)
        {
            return OnSceneScaleAsObservable(component.Entity);
        }

        public static Observable<float> OnSceneScaleAsObservable(this IEntity entity)
        {
            if (entity == null) return Observable.Empty<float>();
            ObservableCommonGridTrigger trigger = GetTrigger(entity);
            return trigger.Observable.OnScaleAsObservable();
        }

        public static Observable<Vector2> OnScenePointDownAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnPointDownAsObservable();
        }

        public static Observable<Vector2> OnScenePointingAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnPointingAsObservable();
        }

        public static Observable<Vector2> OnScenePointUpAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnPointUpAsObservable();
        }

        public static Observable<Vector2> OnSceneLongPressStartAsObservable(this Component component, float time = 0.5f)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            touchCom.Observable.LongPressTime = time;
            return touchCom.Observable.OnLongPressStartAsObservable();
        }

        public static Observable<Vector2> OnSceneLongPressingAsObservable(this Component component, float time = 0.5f)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            touchCom.Observable.LongPressTime = time;
            return touchCom.Observable.OnLongPressingAsObservable();
        }

        public static Observable<Vector2> OnSceneLongPressEndAsObservable(this Component component, float time = 0.5f)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            touchCom.Observable.LongPressTime = time;
            return touchCom.Observable.OnLongPressEndAsObservable();
        }

        public static Observable<Vector2> OnSceneShortPressStartAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnShortPressStartAsObservable();
        }

        public static Observable<Vector2> OnSceneShortPressingAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnShortPressingAsObservable();
        }

        public static Observable<Vector2> OnSceneShortPressEndAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnShortPressEndAsObservable();
        }

        public static Observable<Vector2> OnSceneClickAsObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Vector2>();
            ObservableCommonColliderTrigger touchCom = GetTrigger(component);
            return touchCom.Observable.OnClickAsObservable();
        }

        private static ObservableCommonColliderTrigger GetTrigger(Component component)
        {
            GameObject gameObject = component.gameObject;
            ObservableCommonColliderTrigger touchCom = gameObject.GetComponent<ObservableCommonColliderTrigger>();
            if (touchCom == null)
                touchCom = gameObject.AddComponent<ObservableCommonColliderTrigger>();
            return touchCom;
        }

        private static ObservableCommonGridTrigger GetTrigger(IEntity entity)
        {
            ObservableCommonGridTrigger triggerCom = entity.GetCom<ObservableCommonGridTrigger>();
            if (triggerCom == null)
                triggerCom = entity.AddCom<ObservableCommonGridTrigger>(s_TriggerModule);
            return triggerCom;
        }
    }
}
