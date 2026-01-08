using Common;
using Pt;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopMailContent.prefab")]
    public class UIPopMailContent : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnGetReward;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtContent;
        [SerializeField] private RectTransform rewardRoot;
        [SerializeField] private RewardItem reward;

        private Mail Mail => UserData[0] as Mail;

        private bool DateValid => Mail.ExpiredAt > SgUtility.Now;

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopMailContent>);
            btnGetReward.BindClick(async () =>
            {
                if (!DateValid)
                {
                    UIToast.Instance.ShowToast("mail is expired").Forget();
                    return;
                }

                await DataController.MailClaim(Mail.Uuid);
                btnGetReward.interactable = !Mail.IsClaimed;
            });
        }

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);
            SetData(Mail);
        }

        private void SetData(Mail mail)
        {
            //本地化文本的邮件
            if (mail.MailId > 0)
            {
            }
            else
            {
                txtTitle.SetText(mail.Title);
                txtContent.SetText(mail.Describe);
            }

            foreach (var r in mail.Rewards)
            {
                var item = Instantiate(reward, rewardRoot);
                item.SetData(r);
            }

            DataController.MailRead(mail.Uuid).Forget();

            btnGetReward.gameObject.SetActive(mail.Rewards.Count > 0);
            btnGetReward.interactable = !mail.IsClaimed;
        }
    }
}