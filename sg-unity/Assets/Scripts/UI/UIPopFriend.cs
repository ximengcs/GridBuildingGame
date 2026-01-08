using System.Collections.Generic;
using Common;
using Config;
using Cysharp.Threading.Tasks;
using Pt;
using SgFramework.UI;
using SgFramework.Utility;
using SuperScrollView;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;
using R3;
using SgFramework.Language;

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopFriend.prefab")]
    public class UIPopFriend : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnSearch;
        [SerializeField] private Button btnRefresh;
        [SerializeField] private Button btnRefuseAll;
        [SerializeField] private Button btnApplyAll;

        [SerializeField] private LoopListView2 playerList;
        [SerializeField] private TMP_InputField inputPlayerId;

        [SerializeField] private GameObject goSearch;
        [SerializeField] private GameObject goApply;

        [SerializeField] private GameObject goRecommendTitle;

        [SerializeField] private TextMeshProUGUI txtUserId;
        [SerializeField] private TextMeshProUGUI txtCount;

        [SerializeField] private List<Toggle> pageTabs;

        private List<UserPublicInfo> _data;
        private int Count => _data?.Count ?? 0;

        private int CurrentPage { get; set; } = -1;

        private void Awake()
        {
            playerList.InitListView(Count, OnGetItemByIndex);
        }

        private void OnEnable()
        {
            DataController.OnUserInfoText(txtUserId, info => info.PlayerId);
            ShowPage(0);
        }

        private void Start()
        {
            for (var i = 0; i < pageTabs.Count; i++)
            {
                var page = i;
                pageTabs[i].onValueChanged.AddListener(v =>
                {
                    if (!v)
                    {
                        return;
                    }

                    ShowPage(page);
                });
            }

            btnClose.BindClick(UIManager.Close<UIPopFriend>);
            btnSearch.BindClick(OnClickSearch);
            btnRefresh.BindClick(async () => { await ShowPageAsync(1, true); });
            btnRefuseAll.BindClick(DataController.FriendRefuseApplyAll);
            btnApplyAll.BindClick(DataController.FriendAgreeApplyAll);

            DataController.FriendUpdate.Subscribe(page =>
            {
                if (CurrentPage != page)
                {
                    return;
                }

                ShowPageAsync(page, true).Forget();
            }).AddTo(this);
        }

        private void CleanData()
        {
            _data = null;
            playerList.SetListItemCount(Count);
            playerList.ResetListView();
        }

        public void ShowPage(int pageId)
        {
            ShowPageAsync(pageId).Forget();
        }

        public async UniTask ShowPageAsync(int pageId, bool force = false)
        {
            if (!force && CurrentPage == pageId)
            {
                return;
            }

            CurrentPage = pageId;
            goSearch.SetActive(false);
            goApply.SetActive(false);

            goRecommendTitle.SetActive(pageId == 1);

            switch (pageId)
            {
                case 0:
                {
                    await ShowPageList();
                    break;
                }
                case 1:
                {
                    await ShowPageAdd();
                    break;
                }
                case 2:
                {
                    await ShowPageApply();
                    break;
                }
                case 3:
                {
                    await ShowPageBlock();
                    break;
                }
            }

            txtCount.SetText($"{LanguageManager.Get("UI_Text_Friend2")}{Count}/{Table.Global.FriendLimit}");
        }

        private async UniTask ShowPageList()
        {
            //await DataController.RefreshFriendList(1);
            _data = DataController.GetFriendList(1);
            playerList.SetListItemCount(Count);
            playerList.RefreshAllShownItem();
        }

        private async UniTask ShowPageAdd()
        {
            goSearch.SetActive(true);

            CleanData();
            _data = await DataController.GetRecommendFriendList();
            playerList.SetListItemCount(Count);
            playerList.RefreshAllShownItem();
        }

        private async UniTask ShowPageApply()
        {
            goApply.SetActive(true);

            //await DataController.RefreshFriendList(2);
            _data = DataController.GetFriendList(2);
            playerList.SetListItemCount(Count);
            playerList.RefreshAllShownItem();
        }

        private async UniTask ShowPageBlock()
        {
            //await DataController.RefreshFriendList(3);
            _data = DataController.GetFriendList(3);
            playerList.SetListItemCount(Count);
            playerList.RefreshAllShownItem();
        }

        private async UniTask OnClickSearch()
        {
            if (string.IsNullOrEmpty(inputPlayerId.text))
            {
                UIToast.Instance.ShowToast("请输入玩家ID进行搜索").Forget();
                return;
            }

            _data = await DataController.FriendSearch(inputPlayerId.text);
            playerList.SetListItemCount(Count);
            playerList.RefreshAllShownItem();
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 list, int index)
        {
            if (index < 0 || index > Count)
            {
                return null;
            }

            var item = list.NewListViewItem("FriendItem");
            if (item.TryGetComponent(out FriendItem friendItem))
            {
                friendItem.SetData(_data[index], CurrentPage);
            }

            return item;
        }
    }
}