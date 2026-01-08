using Cysharp.Threading.Tasks;
using SgFramework.Res;
using UnityEngine;
using UnityEngine.UI;

namespace SgFramework.UI
{
    public class UIForm : ResourceToken
    {
        protected object[] UserData;

        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;

        public virtual void OnCreate(object[] args)
        {
            UserData = args;
            _canvas = GetComponent<Canvas>();
            _graphicRaycaster = GetComponent<GraphicRaycaster>();

            if (!TryGetComponent(out RectTransform rectTransform))
            {
                return;
            }

            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }

        public virtual void SetLayer(int sortingOrder)
        {
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = sortingOrder;
        }

        public virtual UniTask ShowOpen()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask ShowClose()
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnOpen()
        {
            _graphicRaycaster.enabled = true;
        }

        public virtual void OnClose()
        {
            _graphicRaycaster.enabled = false;
            UserData = null;
        }
    }
}