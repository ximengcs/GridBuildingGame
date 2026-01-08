using Common;
using R3;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UISettings.prefab")]
    public class UISettings : UIPop
    {
        [SerializeField] private TextMeshProUGUI txtUid;

        [SerializeField] private Button btnClose;

        private void Start()
        {
            DataController.OnUserInfoText(txtUid, info => txtUid.text = $"Uid:{info.PlayerId}").AddTo(this);
            btnClose.BindClick(UIManager.Close<UISettings>);
        }
    }
}