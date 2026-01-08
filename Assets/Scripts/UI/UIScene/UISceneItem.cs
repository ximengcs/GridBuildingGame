using MH.GameScene.Core.Entites;
using SgFramework.Res;
using UnityEngine;

namespace UI.UIScenes
{
    public class UISceneItem : ResourceToken
    {
        private RectTransform _tf;
        private IUISceneBinder _binder;
        private IWorldCamera _cam;
        private Vector2 _rectRate;

        public virtual void OnItemCreate()
        {
            _tf = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public virtual void OnInit(IWorldCamera cam, IUISceneBinder binder, Vector2 rectRate, object args)
        {
            _cam = cam;
            _rectRate = rectRate;
            _binder = binder;
            _binder.PosChangeEvent += BinderChangeHandler;
            _cam.PosChangeEvent += BinderChangeHandler;
            gameObject.SetActive(true);
            BinderChangeHandler();
        }

        private void BinderChangeHandler()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_binder.WorldPos);
            _tf.anchoredPosition = screenPos * _rectRate;
        }

        public virtual void OnRecycle()
        {
            gameObject.SetActive(false);
            _binder.PosChangeEvent -= BinderChangeHandler;
            _cam.PosChangeEvent -= BinderChangeHandler;
            _binder = null;
        }

        public virtual void OnOpen() { }

        public virtual void OnClose() { }
    }
}
