using System;
using Common;
using SgFramework.Language;
using SgFramework.Res;
using SgFramework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UIBagItem : MonoBehaviour
    {
        [SerializeField] private Image imgBg;
        [SerializeField] private Image icon;
        [SerializeField] private LanguageText itemName;
        [SerializeField] private TextMeshProUGUI txtCount;
        [SerializeField] private Button btnGetWay;

        private SgItemUtility.RewardInfo _rewardInfo;

        // Start is called before the first frame update
        void Start()
        {
            btnGetWay.BindClick(() => { _rewardInfo?.GetWay?.Invoke(_rewardInfo.Id); });
        }

        public async void SetRewardID(ResourceGroup group, int type, int goodsID, int count)
        {
            try
            {
                var rst = SgItemUtility.TryGetRewardID(type, goodsID, out var info);
                if (!rst)
                {
                    Debug.LogError($"GoodsID {type}:{goodsID} not found");
                    return;
                }

                _rewardInfo = info;
                if (txtCount)
                {
                    txtCount.gameObject.SetActive(count > 1);
                    txtCount.text = count.ToString();
                }
                if (itemName)
                {
                    itemName.SetKey(_rewardInfo.NameLau);
                }

                var bgSprite = await group.GetSprite(info.Quality);
                var iconSprite = await group.GetSprite(info.Icon);
                if (type != _rewardInfo.Type || goodsID != _rewardInfo.Id)
                {
                    return;
                }

                imgBg.sprite = bgSprite;
                icon.sprite = iconSprite;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}