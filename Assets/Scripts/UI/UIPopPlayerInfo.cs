using Common;
using Config;
using Pt;
using R3;
using SgFramework.RedPoint;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopPlayerInfo.prefab")]
    public class UIPopPlayerInfo : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private AvatarComp avatar;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private Button btnChangeAvatar;
        [SerializeField] private RedPointComponent reddot;
        [SerializeField] private TextMeshProUGUI txtPlayerId;
        [SerializeField] private Button btnChangeName;
        [SerializeField] private Button btnCopy;
        [SerializeField] private GameObject expRoot;
        [SerializeField] private TextMeshProUGUI txtLevel;
        [SerializeField] private TextMeshProUGUI expProgress;
        [SerializeField] private UIImageProgress imgExp;


        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopPlayerInfo>);
            btnChangeAvatar.BindClick(() => UIManager.Open<UIPopAvatarOperate>());
            btnChangeName.BindClick(() => UIManager.Open<UIPopRename>());
            btnCopy.BindClick(() =>
            {
                GUIUtility.systemCopyBuffer = txtPlayerId.text;
            });
            DataController.OnUserInfoText(txtName, userInfo => userInfo.Name).AddTo(this);
        }

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);

            if (args == null || args.Length == 0)
            {
                SetSelfInfo(DataController.UserInfo);
                return;
            }

            switch (args[0])
            {
                case UserPublicInfo info:
                    if (info.PlayerId.Equals(DataController.UserInfo.PlayerId))
                    {
                        SetSelfInfo(info);
                    }
                    else
                    {
                        SetOtherInfo(info);
                    }
                    break;
                case UserInfo info:
                    SetSelfInfo(info);
                    break;
            }
        }

        private void SetSelfInfo(UserPublicInfo info)
        {
            ShowItemBySelf(true);
            avatar.SetData(info.AvatarFrame, info.Avatar, info.CustomAvatarUrl);
            txtName.text = info.Name;
            reddot.gameObject.SetActive(DataController.UserInfo.CurLevelTipShow);
            txtPlayerId.text = info.PlayerId;
            UpdateExp(info.Level, DataController.UserInfo.Exp);
        }

        private void SetSelfInfo(UserInfo info)
        {
            ShowItemBySelf(true);
            avatar.SetData(info.AvatarFrame, info.Avatar, info.CustomAvatar);
            txtName.text = info.Name;
            reddot.gameObject.SetActive(info.CurLevelTipShow);
            txtPlayerId.text = info.PlayerId;
            UpdateExp(info.Level, DataController.GetCurrency(SgConst.CurrencyPlayerExp));
            DataController.OnUserInfo(OnArchiveChange).AddTo(this);
        }

        private void SetOtherInfo(UserPublicInfo info)
        {
            ShowItemBySelf(false);
            avatar.SetData(info.AvatarFrame, info.Avatar, info.CustomAvatarUrl);
            txtName.text = info.Name;
            reddot.gameObject.SetActive(false);
            txtPlayerId.text = info.PlayerId;
            UpdateExp(info.Level, 0);
        }

        private void ShowItemBySelf(bool isSelf)
        {
            expRoot.gameObject.SetActive(isSelf);
            btnChangeAvatar.gameObject.SetActive(isSelf);
            btnChangeName.gameObject.SetActive(isSelf);
        }

        private void UpdateExp(int level, long exp)
        {
            txtLevel.text = level.ToString();
            var needExp = Table.PlayerLevelTable.GetById(level).need_exp;
            expProgress.text = exp + "/" + needExp;
            imgExp.progress = (float)exp / needExp;
        }

        private void OnArchiveChange(UserInfo rsp)
        {
            var level = rsp.Level;
            var exp = rsp.Exp;
            txtLevel.text = level.ToString();
            var needExp = Table.PlayerLevelTable.GetById(level).need_exp;
            expProgress.text = exp + "/" + needExp;
            imgExp.progress = (float)exp / needExp;
        }

    }
}