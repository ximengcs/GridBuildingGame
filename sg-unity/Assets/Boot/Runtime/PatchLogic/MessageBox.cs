using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PatchLogic
{
    /// <summary>
    /// 对话框封装类
    /// </summary>
    public class MessageBox : MonoBehaviour
    {
        public TextMeshProUGUI txtContent;
        public Button btnOk;
        private Action _clickOk;

        private void Awake()
        {
            btnOk.onClick.AddListener(OnClickYes);
        }

        public void Show(string content, Action clickOk)
        {
            txtContent.text = content;
            _clickOk = clickOk;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            txtContent.text = string.Empty;
            _clickOk = null;
            gameObject.SetActive(false);
        }

        private void OnClickYes()
        {
            _clickOk?.Invoke();
            Hide();
        }
    }
}