using System;
using Common;
using Config;
using Cysharp.Threading.Tasks;
using SgFramework.Language;
using SgFramework.RedPoint;
using SgFramework.Res;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Item = Pt.Item;

namespace UIComponent
{
    public class UIBagGridItem : MonoBehaviour
    {
        [SerializeField] private Image imgBg;
        [SerializeField] private Image icon;
        [SerializeField] private RedPointComponent redPoint;
        [SerializeField] private GameObject imgSign;
        [SerializeField] private Image imgSignLeft;
        [SerializeField] private Image imgSignRight;
        [SerializeField] private TextMeshProUGUI txtSign;
        [SerializeField] private TextMeshProUGUI txtCount;
        [SerializeField] private LanguageText txtName;
        [SerializeField] private Button btnClick;

        private Item _data;
        private ResourceGroup _group;


        // Start is called before the first frame update
        void Start()
        {
            btnClick.BindClick(() =>
            {
                DataController.SetItemRed(_data.ConfId, false);
                UIManager.Open<UIPopBagItemInfo>(_data).Forget();
            });
        }

        public async void SetData(ResourceGroup group, Item info)
        {
            try
            {
                _group = group;
                _data = info;
                var config = Table.ItemTable.GetById(info.ConfId);
                var hasMask = !string.IsNullOrEmpty(config.picture_mark);
                imgSign.gameObject.SetActive(config.type == (int)UIPopBag.ItemType.FixBox ||
                                             config.type == (int)UIPopBag.ItemType.RandBox);
                imgSignLeft.gameObject.SetActive(false);
                imgSignRight.gameObject.SetActive(false);
                redPoint.SetPath($"bag/{config.in_warehouse}/item{info.ConfId}");
                txtCount.text = info.Amount.ToString();
                txtName.SetKey(config.item_name);
                if (config.type == (int)UIPopBag.ItemType.RandBox)
                {
                    // txtSign.text = LanguageManager.Get("随机");
                }

                var bgSprite =
                    await _group.GetSprite(SgItemUtility.ResRootPath + SgItemUtility.QualityImg[config.color]);
                var iconSprite = await _group.GetSprite(SgItemUtility.ResRootPath + config.icon);
                Sprite signSprite = null;
                if (hasMask)
                {
                    signSprite = await _group.GetSprite(SgItemUtility.ResRootPath + config.picture_mark);
                }

                if (info.ConfId != _data.ConfId)
                {
                    return;
                }

                imgBg.sprite = bgSprite;
                icon.sprite = iconSprite;
                if (!hasMask)
                {
                    return;
                }

                var isLeft = imgSign.gameObject.activeSelf;
                imgSignLeft.gameObject.SetActive(isLeft);
                imgSignRight.gameObject.SetActive(!isLeft);
                if (isLeft)
                {
                    imgSignLeft.sprite = signSprite;
                }
                else
                {
                    imgSignRight.sprite = signSprite;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}