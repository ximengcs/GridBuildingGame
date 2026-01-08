using R3;
using R3.Triggers;
using SgFramework.UI;
using SgFramework.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.GM, "Assets/GameRes/Prefabs/UI/UIGMFloatingBall.prefab")]
    public class UIGMFloatingBall : UIForm
    {
        [SerializeField] private Button btnEnter;

        private RectTransform RectTransform => transform as RectTransform;
        private RectTransform BtnRectTransform => btnEnter.transform as RectTransform;

        private void Start()
        {
            btnEnter.BindClick(() => UIManager.Open<UIGM>());

            btnEnter.OnBeginDragAsObservable().Subscribe(b => { btnEnter.interactable = false; });
            btnEnter.OnDragAsObservable().Subscribe(b =>
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, b.position, null,
                        out var lp))
                {
                    return;
                }

                var rect = RectTransform.rect;
                lp.x = Mathf.Clamp(lp.x, rect.xMin, rect.xMax);
                lp.y = Mathf.Clamp(lp.y, rect.yMin, rect.yMax);
                BtnRectTransform.anchoredPosition = lp;
            });
            btnEnter.OnEndDragAsObservable().Subscribe(b => { btnEnter.interactable = true; });
        }
    }
}