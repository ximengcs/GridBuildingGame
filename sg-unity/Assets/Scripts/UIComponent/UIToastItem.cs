using SgFramework.Language;
using SgFramework.Res;
using UnityEngine;

namespace UIComponent
{
    public class UIToastItem : ResourceToken
    {
        [SerializeField] private LanguageText txtInfo;

        public override void OnGet()
        {
            if (!TryGetComponent(out RectTransform rectTransform))
            {
                return;
            }

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        public void SetData(string key)
        {
            txtInfo.SetKey(key);
        }
    }
}