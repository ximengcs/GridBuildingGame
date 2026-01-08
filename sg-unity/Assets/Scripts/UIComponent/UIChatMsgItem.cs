using Pt;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UIChatMsgItem : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI txtContent;
        
        public void SetData(ChatNotice notice)
        {
            txtContent.text = notice.Content;
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}