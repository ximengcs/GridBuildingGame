using System;
using Common;
using SgFramework.Net;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UIPopNoticeListNoticeItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtDate;
        [SerializeField] private GameObject goRead;

        private HttpApi.NavNotice _bindData;

        private void Awake()
        {
            GetComponent<Button>().BindClick(async () =>
            {
                await UIManager.Open<UIPopNotice>(_bindData);
                goRead.SetActive(LocalStorage.GetNoticeStatus(_bindData.Id) == ENoticeStatus.IsRead);
            });
        }

        public void SetData(HttpApi.NavNotice notice)
        {
            _bindData = notice;
            txtTitle.SetText(notice.Title);

            var date = DateTimeOffset.FromUnixTimeSeconds(notice.StartTime);
            txtDate.SetText(date.ToString("yyyy-MM-dd"));

            goRead.SetActive(LocalStorage.GetNoticeStatus(notice.Id) == ENoticeStatus.IsRead);
        }
    }
}