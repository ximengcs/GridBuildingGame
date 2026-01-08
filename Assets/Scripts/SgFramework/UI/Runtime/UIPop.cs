using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SgFramework.UI
{
    public class UIPop : UIForm
    {
        [SerializeField] private Image imgMask;
        [SerializeField] private RectTransform rootPanel;

        public override bool CanReuse => false;

        private const float InTime = 0.25f;
        private const float OutTime = 0.05f;

        public override async UniTask ShowOpen()
        {
            if (!rootPanel)
            {
                return;
            }

            if (imgMask)
            {
                var c = imgMask.color;
                var t = c;
                c.a = 0f;
                imgMask.color = c;
                imgMask.DOColor(t, InTime);
            }
            
            rootPanel.localScale = Vector3.zero;
            await rootPanel.DOScale(Vector3.one, InTime).AsyncWaitForCompletion();
        }

        public override async UniTask ShowClose()
        {
            await base.ShowClose();
            if (!rootPanel || OutTime < 0.03f)
            {
                return;
            }

            if (imgMask)
            {
                var t = imgMask.color;
                t.a = 0f;
                imgMask.DOColor(t, OutTime);
            }

            await rootPanel.DOScale(Vector3.zero, OutTime).AsyncWaitForCompletion();
        }
    }
}