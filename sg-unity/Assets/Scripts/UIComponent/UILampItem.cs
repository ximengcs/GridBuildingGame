using System;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pt;
using R3;
using SgFramework.Res;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UILampItem : ResourceToken
    {
        [SerializeField] private TextMeshProUGUI txtInfo;

        private IDisposable _disposable;
        private PushLampMsg _bindData;
        private const float OffsetDistance = 100f;

        private void OnEnable()
        {
            _disposable = DataController.LanguageChanged.Subscribe(OnLangChanged);
        }

        private void OnDisable()
        {
            _disposable.Dispose();
        }

        public override void OnGet()
        {
            if (!TryGetComponent(out RectTransform rectTransform))
            {
                return;
            }

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        public void SetData(PushLampMsg rsp)
        {
            txtInfo.rectTransform.anchoredPosition = new Vector2(10000f, 0f);
            _bindData = rsp;

            if (!_bindData.Content.TryGetValue(DataController.GetLanguageSetting(), out var content)
                && !_bindData.Content.TryGetValue(3, out content))
            {
                content = _bindData.Content.FirstOrDefault().Value;
            }

            txtInfo.SetText(content);
            var rectTransform = txtInfo.rectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private bool _wait;

        private async void OnLangChanged(int lang)
        {
            _wait = true;
            _tweener.Kill();
            SetData(_bindData);
            await UniTask.WaitForSeconds(0.5f);
            _tweener = CreateAnim();
            _wait = false;
        }

        private Tweener _tweener;

        private Tweener CreateAnim()
        {
            txtInfo.rectTransform.anchoredPosition =
                new Vector2(txtInfo.rectTransform.sizeDelta.x + OffsetDistance, 0f);
            var rect = txtInfo.rectTransform.parent as RectTransform;
            var distance = rect!.rect.width + txtInfo.rectTransform.rect.width + OffsetDistance * 2f;
            const float speed = 100f;
            return txtInfo.rectTransform.DOAnchorPosX(-rect!.rect.width - OffsetDistance, distance / speed)
                .SetEase(Ease.Linear);
        }

        public async UniTask ShowAnim()
        {
            _tweener = CreateAnim();
            await UniTask.WaitUntil(() => !_wait && !_tweener.active || _tweener.IsComplete(),
                cancellationToken: destroyCancellationToken);
            if (destroyCancellationToken.IsCancellationRequested)
            {
                txtInfo.rectTransform.DOKill();
            }
        }
    }
}