using System;
using Common;
using Cysharp.Threading.Tasks;
using SgFramework.Net;
using SgFramework.RedPoint;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopNotice.prefab")]
    public class UIPopNotice : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtContent;

        private HttpApi.NavNotice _bindData;

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopNotice>);
        }

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);
            switch (UserData[0])
            {
                case HttpApi.MaintainInfo info:
                {
                    SetData(info.Notice);
                    break;
                }
                case HttpApi.NavNotice notice:
                {
                    SetData(notice);
                    break;
                }
                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private void SetData(HttpApi.NavNotice notice)
        {
            _bindData = notice;
            txtTitle.SetText(notice.Title);
            txtContent.SetText(notice.Content);
        }

        public override UniTask ShowOpen()
        {
            LocalStorage.SetNoticeStatus(_bindData.Id, ENoticeStatus.IsRead);
            RedPointManager.Instance.FindNode($"notice/data/{_bindData.Id}").ResetValue();
            return base.ShowOpen();
        }
    }
}