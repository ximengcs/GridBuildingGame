using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopMessage.prefab")]
    public class UIPopMessage : UIPop
    {
        [SerializeField] private TextMeshProUGUI txtInfo;
        [SerializeField] private Button btnClose;

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopMessage>);
        }

        public void SetData(string info)
        {
            txtInfo.SetText(info);
        }
    }
}