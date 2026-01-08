using System.Collections.Generic;
using Common;
using Config;
using Cysharp.Threading.Tasks;
using R3;
using SgFramework.Language;
using SgFramework.RedPoint;
using SgFramework.Res;
using SgFramework.UI;
using SgFramework.Utility;
using SuperScrollView;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;
using Item = Pt.Item;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopBag.prefab")]
    public class UIPopBag : UIPop
    {
        /// <summary>
        /// 背包页签类型
        /// </summary>
        private enum ItemBagType
        {
            None,
            All,
            Flower,
            Product,
            Other,

            // 结束标记用于遍历
            End,
        }

        public enum ItemType
        {
            None,

            // 固定奖励宝箱
            FixBox,

            // 随机奖励宝箱
            RandBox,

            // 固定+随机奖励宝箱
            FixRandBox,

            // 头像框
            AvatarFrame,

            // 常规消耗道具
            UsageTool,

            // 头像
            Avatar,
        }

        [SerializeField] private Button btnClose;
        [SerializeField] private LoopListView2 loopList;
        [SerializeField] private Button btnCapacity;
        [SerializeField] private GameObject bagItemsRow;
        [SerializeField] private TextMeshPro txtCapacity;
        [SerializeField] private Transform togPageGroup;
        [SerializeField] private GameObject togItem;

        private readonly List<List<Item>> _list = new();
        private ItemBagType _currPage = ItemBagType.None;

        private int ItemCount => _list?.Count ?? 0;

        private ResourceGroup _group;


        private void Start()
        {
            _group = ResourceManager.GetGroup("Bag").AddTo(this);

            btnClose.BindClick(UIManager.Close<UIPopBag>);
            btnCapacity.BindClick(() => { });

            loopList.InitListView(0, OnGetItemByIndex);
            InitTogPage();

            DataController.ItemUpdate.Subscribe(OnItemUpdate).AddTo(this);
        }

        public override UniTask ShowClose()
        {
            loopList.SetListItemCount(0);
            return base.ShowClose();
        }

        private void InitTogPage()
        {
            var item = togItem;
            for (var i = 1; i < (int)ItemBagType.End; i++)
            {
                if (i != 1)
                {
                    item = Instantiate(togItem, togPageGroup, false);
                    item.transform.Find("RedPoint").GetComponent<RedPointComponent>().SetPath($"bag/{i}");
                }
                else
                {
                    item.transform.Find("RedPoint").GetComponent<RedPointComponent>().SetPath("bag");
                }

                item.transform.Find("txtName").GetComponent<LanguageText>()
                    .SetKey(Table.WarehouseTypeTable.GetById(i).warehouse_type);
                var i1 = i;
                item.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        RefreshView((ItemBagType)i1);
                    }
                });
            }

            togItem.GetComponent<Toggle>().isOn = true;
        }

        private void RefreshView(ItemBagType view, bool force = false)
        {
            if (!force && view == _currPage)
            {
                return;
            }

            _currPage = view;
            _list.Clear();
            var sortList = new List<Item>();
            foreach (var (key, value) in DataController.Items)
            {
                if (value.Amount <= 0)
                {
                    continue;
                }

                var config = Table.ItemTable.GetById(key);
                if (config != null && (view == ItemBagType.All || config.in_warehouse == (int)view))
                {
                    sortList.Add(value);
                }
            }

            sortList.Sort((x, y) => Table.ItemTable.GetById(x.ConfId).order_id
                .CompareTo(Table.ItemTable.GetById(y.ConfId).order_id));

            var index = 0;
            var grid = new List<Item>();
            _list.Add(grid);
            foreach (var value in sortList)
            {
                if (index == 4)
                {
                    index = 0;
                    grid = new List<Item>();
                    _list.Add(grid);
                }

                index++;
                grid.Add(value);
            }

            loopList.SetListItemCount(ItemCount);
            loopList.RefreshAllShownItem();
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 list, int index)
        {
            if (index < 0 || index >= ItemCount)
            {
                return null;
            }

            var item = list.NewListViewItem("BagItemsRow");
            var info = _list[index];
            for (var i = 0; i < 4; i++)
            {
                var cell = item.transform.Find("BagItem_" + i);
                cell.gameObject.SetActive(i < info.Count);
                if (i < info.Count)
                {
                    cell.GetComponent<UIBagGridItem>().SetData(_group, info[i]);
                }
            }

            return item;
        }

        private void OnItemUpdate(Item item)
        {
            RefreshView(_currPage, true);
        }
    }
}