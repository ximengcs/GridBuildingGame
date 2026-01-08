using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Pt;
using SgFramework.Net;
using SgFramework.UI;
using SgFramework.Utility;
using SuperScrollView;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopMail.prefab")]
    public class UIPopMail : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnReadAll;
        [SerializeField] private Button btnClaimAll;
        [SerializeField] private Button btnDeleteAll;

        [SerializeField] private LoopListView2 loopList;

        private List<Mail> _list;

        private int MailCount => _list?.Count ?? 0;

        private void Start()
        {
            _list = DataController.GetMailList();

            btnClose.BindClick(UIManager.Close<UIPopMail>);
            loopList.InitListView(MailCount, OnGetItemByIndex);

            btnReadAll.BindClick(async () =>
            {
                await DataController.MailReadAll();
                loopList.RefreshAllShownItem();

                RefreshView();
            });

            btnClaimAll.BindClick(async () =>
            {
                await DataController.MailClaimAll();
                loopList.RefreshAllShownItem();

                RefreshView();
            });

            btnDeleteAll.BindClick(async () =>
            {
                await DataController.MailDeleteAll();
                _list = DataController.GetMailList();
                loopList.SetListItemCount(MailCount);
                loopList.RefreshAllShownItem();

                RefreshView();
            });

            RefreshView();
        }

        private void RefreshView()
        {
            btnReadAll.gameObject.SetActive(DataController.GetMailNoClaimCount() == 0);
            btnClaimAll.gameObject.SetActive(DataController.GetMailNoClaimCount() > 0);
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 list, int index)
        {
            if (index < 0 || index >= MailCount)
            {
                return null;
            }

            var item = list.NewListViewItem("MailItem");
            if (item.TryGetComponent(out UIMailItem mailItem))
            {
                mailItem.SetData(_list[index]);
            }

            return item;
        }

        public override UniTask ShowClose()
        {
            loopList.SetListItemCount(0);
            return base.ShowClose();
        }
    }
}