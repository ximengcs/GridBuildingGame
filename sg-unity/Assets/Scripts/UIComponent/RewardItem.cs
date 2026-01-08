using SgFramework.Res;
using TMPro;
using UnityEngine;

namespace UIComponent
{
    public class RewardItem : ResourceToken
    {
        [SerializeField] private TextMeshProUGUI txtAmount;

        public void SetData(Reward reward)
        {
            txtAmount.SetText($"{reward.num}");
        }

        public void SetData(Pt.Reward reward)
        {
            txtAmount.SetText($"{reward.Amount}");
        }
    }
}