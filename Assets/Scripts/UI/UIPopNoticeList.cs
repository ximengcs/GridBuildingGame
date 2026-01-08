using System;
using System.Collections.Generic;
using System.Linq;
using Best.HTTP;
using Common;
using Newtonsoft.Json;
using SgFramework.Env;
using SgFramework.Net;
using SgFramework.RedPoint;
using SgFramework.UI;
using SgFramework.Utility;
using SuperScrollView;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopNoticeList.prefab")]
    public class UIPopNoticeList : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private LoopListView2 noticeList;

        public static List<HttpApi.NavNotice> Data { get; set; }
        private static int Count => Data?.Count ?? 0;

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopNoticeList>);
            noticeList.InitListView(Count, OnGetItemByIndex);

            noticeList.SetListItemCount(Count);
            noticeList.ResetListView();
        }

        private static void RefreshRedPoint()
        {
            if (Data == null)
            {
                return;
            }

            foreach (var notice in Data)
            {
                var status = LocalStorage.GetNoticeStatus(notice.Id);
                RedPointManager.Instance.FindNode($"notice/data/{notice.Id}").SetValue(status == 0 ? 1 : 0);
            }
        }

        public static async void RequestNotice(bool force = false)
        {
            try
            {
                if (Count != 0 && !force)
                {
                    return;
                }

                var host = SgAppEnv.Shared.HttpServer;
                Debug.Log($"request notice {host}");
                var path = $"/sg/notice?langCode={SgUtility.GetLanguage()}";
                var request = HTTPRequest.CreatePost($"{host}{path}");
                request.TimeoutSettings.Timeout = TimeSpan.FromSeconds(5f);
                var resp = await request.GetHTTPResponseAsync();
                if (!resp.IsSuccess)
                {
                    return;
                }

                var noticeDict =
                    JsonConvert.DeserializeObject<Dictionary<string, HttpApi.NavNotice>>(resp.DataAsText);
                if (noticeDict == null)
                {
                    Debug.LogWarning("公告数据无效");
                    return;
                }

                Data = noticeDict.Values.ToList();
                RefreshRedPoint();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 list, int index)
        {
            if (index < 0 || index > Count)
            {
                return null;
            }

            var item = list.NewListViewItem("NoticeItem");
            if (item.TryGetComponent(out UIPopNoticeListNoticeItem noticeItem))
            {
                noticeItem.SetData(Data[index]);
            }

            return item;
        }
    }
}