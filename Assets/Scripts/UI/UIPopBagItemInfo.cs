using System;
using Common;
using Config;
using Cysharp.Threading.Tasks;
using SgFramework.Language;
using SgFramework.Res;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;
using Item = Pt.Item;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopBagItemInfo.prefab")]
    public class UIPopBagItemInfo : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Image imgIcon;
        [SerializeField] private GameObject imgRare;
        [SerializeField] private Image imgCurrency;
        [SerializeField] private Slider sliderUse;
        [SerializeField] private Image imgUseRaycast;
        [SerializeField] private Button btnAdd;
        [SerializeField] private Button btnSub;
        [SerializeField] private LanguageText txtName;
        [SerializeField] private LanguageText txtDesc;
        [SerializeField] private TextMeshProUGUI txtUseCount;
        [SerializeField] private TextMeshProUGUI txtPrice;
        [SerializeField] private GameObject txtNoSell;
        [SerializeField] private GameObject useRoot;
        [SerializeField] private Button btnUse;
        [SerializeField] private Button btnSell;
        [SerializeField] private GameObject useItemCtx;
        [SerializeField] private GameObject useItem;
        [SerializeField] private GameObject txtRand;
        [SerializeField] private GameObject txtNoRand;
        [SerializeField] private GameObject countRoot;

        private Item _data;
        private int _useCount = 1;
        private ResourceGroup _group;
        private bool _notRefreshBar;
        private int _perPrice;


        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopBagItemInfo>);
            btnSub.BindClick(() => { AddUseCount(-1); });
            btnAdd.BindClick(() => { AddUseCount(1); });
            btnUse.BindClick(async () =>
            {
                await DataController.ItemUseMsg(_data.ConfId, _useCount);
                RefreshUseCount();
            });
            btnSell.BindClick(async () =>
            {
                await DataController.ItemSaleMsg(_data.ConfId, _useCount);
                RefreshUseCount();
            });
            sliderUse.onValueChanged.AddListener(value =>
            {
                if (_notRefreshBar)
                {
                    return;
                }

                if (_useCount != (int)value)
                {
                    AddUseCount((int)value - _useCount, value > 0);
                }
            });
        }

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);
            if (args[0] is not Item info)
            {
                return;
            }

            _data = info;

            _group = ResourceManager.GetGroup("ItemInfo").AddTo(this);
            var config = Table.ItemTable.GetById(_data.ConfId);
            txtName.SetKey(config.item_name);
            txtDesc.SetKey(config.item_desc);
            imgRare.gameObject.SetActive(false);
            var canUse = config.type is (int)UIPopBag.ItemType.FixBox or (int)UIPopBag.ItemType.RandBox
                or (int)UIPopBag.ItemType.FixRandBox;
            useRoot.SetActive(canUse);
            countRoot.SetActive(canUse || config.sell_value != null);
            txtNoSell.SetActive(!canUse && config.sell_value == null);
            imgCurrency.gameObject.SetActive(config.sell_value != null);
            _perPrice = config.sell_value?.num ?? 0;

            if (canUse)
            {
                txtRand.gameObject.SetActive(config.type != (int)UIPopBag.ItemType.RandBox);
                txtNoRand.gameObject.SetActive(config.type == (int)UIPopBag.ItemType.RandBox);
                RefreshUseView(config);
            }

            if (canUse || config.sell_value != null)
            {
                sliderUse.maxValue = _data.Amount;
                imgUseRaycast.raycastTarget = _data.Amount > 1;
            }

            AddUseCount(0);
            RefreshView(config);
        }

        private async void RefreshView(Config.Item config)
        {
            try
            {
                var iconSprite = await _group.GetSprite(SgItemUtility.ResRootPath + config.icon);
                if (config.id != _data.ConfId)
                {
                    return;
                }

                imgIcon.sprite = iconSprite;

                if (config.sell_value == null)
                {
                    return;
                }

                SgItemUtility.TryGetRewardID(config.sell_value.type, config.sell_value.confId, out var goods);
                var currency = await _group.GetSprite(goods.Icon);
                if (config.id == _data.ConfId)
                {
                    imgCurrency.sprite = currency;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void RefreshUseView(Config.Item config)
        {
            useItem.SetActive(false);
            if (config.reward != null)
            {
                foreach (var reward in config.reward)
                {
                    var item = Instantiate(useItem, useItemCtx.transform, false);
                    item.SetActive(true);
                    item.GetComponent<UIBagItem>().SetRewardID(_group, reward.type, reward.confId, reward.num);
                }
            }

            if (config.rand_reward != null)
            {
                foreach (var reward in config.rand_reward)
                {
                    var item = Instantiate(useItem, useItemCtx.transform, false);
                    item.SetActive(true);
                    item.GetComponent<UIBagItem>().SetRewardID(_group, reward.type, reward.confId, reward.num);
                }
            }
        }

        private void AddUseCount(int count, bool notBarFlush = false)
        {
            _useCount = Math.Clamp(count + _useCount, 1, _data.Amount);
            txtUseCount.text = $"<color=#00FF00>{_useCount}</color>/{_data.Amount}";
            btnSub.interactable = _useCount > 1;
            btnAdd.interactable = _useCount < _data.Amount;
            txtPrice.text = (_perPrice * _useCount).ToString();

            if (notBarFlush)
            {
                return;
            }

            _notRefreshBar = true;
            sliderUse.value = _useCount;
            _notRefreshBar = false;
        }

        private void RefreshUseCount()
        {
            var allCount = DataController.GetItem(_data.ConfId);
            if (allCount <= 0)
            {
                UIManager.Close<UIPopBagItemInfo>().Forget();
                return;
            }

            _useCount = 1;
            sliderUse.maxValue = _data.Amount;
            imgUseRaycast.raycastTarget = _data.Amount > 1;
            AddUseCount(0);
        }
    }
}