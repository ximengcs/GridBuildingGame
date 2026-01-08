using System;
using TMPro;
using UnityEngine;

namespace SgFramework.Language
{
    public class LanguageText : MonoBehaviour
    {
        public string languageKey;
        private object[] _args;

        private TMP_Text _txtTarget;

        private void Awake()
        {
            _txtTarget = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            RefreshText();
        }

        private void OnDestroy()
        {
            _args = null;
        }

        public void SetKey(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            languageKey = key.Trim();
            _args = args;
            var content = LanguageManager.Get(languageKey);
            try
            {
                if (_args is { Length: > 0 })
                {
                    content = string.Format(content, _args);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"本地化：{languageKey}，Format出错，检查配置。{e}");
            }

            _txtTarget.text = content;
        }

        public void SetText(string content)
        {
            _txtTarget.text = content;
            _args = null;
            languageKey = null;
        }

        public void RefreshText()
        {
            SetKey(languageKey, _args);
        }
    }
}