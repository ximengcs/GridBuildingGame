using System;
using Pt;
using R3;
using SgFramework.UI;
using SgFramework.Utility;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UIMailItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtDate;
        [SerializeField] private TextMeshProUGUI txtExpire;

        [SerializeField] private GameObject goRead;
        [SerializeField] private GameObject goClaim;
        [SerializeField] private GameObject goReward;

        private Mail _bindData;

        private void Awake()
        {
            GetComponent<Button>().BindClick(() => UIManager.Open<UIPopMailContent>(_bindData));
        }

        private void Start()
        {
            Observable.EveryValueChanged(_bindData, mail => mail.IsRead).Subscribe(goRead.SetActive)
                .AddTo(this);
            Observable.EveryValueChanged(_bindData, mail => mail.IsClaimed).Subscribe(goClaim.SetActive)
                .AddTo(this);
            Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                txtExpire.SetText(SgUtility.ExpireString(_bindData.ExpiredAt));
            }).AddTo(this);
        }

        public void SetData(Mail mail)
        {
            _bindData = mail;
            goRead.SetActive(mail.IsRead);
            goClaim.SetActive(mail.IsClaimed);
            goReward.SetActive(mail.Rewards.Count > 0);
            txtTitle.SetText(mail.Title);
            txtDate.SetText(DateTimeOffset.FromUnixTimeSeconds(mail.CreatedAt).ToString("yyyy-MM-dd hh-mm-ss"));
            txtExpire.SetText(SgUtility.ExpireString(mail.ExpiredAt));
        }
    }
}