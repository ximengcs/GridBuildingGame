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

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopItemsLack.prefab")]
    public class UIPopItemsLack : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private GameObject itemContent;
        [SerializeField] private GameObject itemGo;
        [SerializeField] private Button btnOk;
        [SerializeField] private TextMeshProUGUI txtPrice;

        private ResourceGroup _group;
        private int _gemPrice;
        private Pt.Item[] _items;


        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopItemsLack>);
            btnOk.BindClick(async () =>
            {
                if (DataController.GetCurrency(SgConst.CurrencyGem) < _gemPrice)
                {
                    var str = string.Format(LanguageManager.Get("Warehouse_Tips2"),
                        LanguageManager.Get(Table.CurrencyTable.GetById(SgConst.CurrencyGem).currency_name));
                    UIToast.Instance.ShowToast(str).Forget();
                    return;
                }

                await DataController.ItemBuyMsg(_items);
                UIManager.Close<UIPopItemsLack>().Forget();
            });
        }

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);
            if (args is not Pt.Item[] info)
            {
                return;
            }

            _items = info;
            _group = ResourceManager.GetGroup("GoodsLack").AddTo(this);
            itemGo.SetActive(false);
            _gemPrice = 0;
            var itemType = Table.Global.RewardTypeItem;
            foreach (var reward in info)
            {
                var item = Instantiate(itemGo, itemContent.transform, false);
                item.SetActive(true);
                item.GetComponent<UIBagItem>().SetRewardID(_group, itemType, reward.ConfId, reward.Amount);

                if (SgItemUtility.TryGetRewardID(itemType, reward.ConfId, out var price))
                {
                    if (price.Price[0] == Table.Global.RewardTypeCurrency &&
                        price.Price[1] == SgConst.CurrencyGem)
                    {
                        _gemPrice += price.Price[2] * reward.Amount;
                    }
                }
            }

            txtPrice.text = _gemPrice.ToString();
        }
    }
}