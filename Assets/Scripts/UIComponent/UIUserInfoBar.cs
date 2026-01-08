using Common;
using Config;
using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.RedPoint;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UIUserInfoBar : MonoBehaviour
    {

        [SerializeField] private AvatarComp avatar;
        [SerializeField] private UIImageProgress expProgress;
        [SerializeField] private RedPointComponent redPoint;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtLevel;
        [SerializeField] private Button btnHead;

        // Start is called before the first frame update
        private void Start()
        {
            btnHead.BindClick(() => { UIManager.Open<UIPopPlayerInfo>().Forget(); });
            var playerInfo = DataController.UserInfo;
            avatar.SetData(playerInfo.AvatarFrame, playerInfo.Avatar, playerInfo.CustomAvatar, true);
            UpdateExp(playerInfo.Level, DataController.GetCurrency(SgConst.CurrencyPlayerExp));
            txtName.text = playerInfo.Name;
            DataController.OnUserInfoText(txtName, userInfo => userInfo.Name).AddTo(this);
            DataController.OnUserInfo(OnSubscribeLevel).AddTo(this);
        }

        private void UpdateExp(int level, long exp)
        {
            txtLevel.text = level.ToString();
            expProgress.progress = (float)exp / Table.PlayerLevelTable.GetById(level).need_exp;
        }

        private void OnSubscribeLevel(UserInfo rsp)
        {
            var exp = rsp.Exp;
            var level = rsp.Level;
            txtLevel.text = level.ToString();
            expProgress.progress = (float)exp / Table.PlayerLevelTable.GetById(level).need_exp;
        }

    }
}