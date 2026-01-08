using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using R3;
using SgFramework.Env;
using SgFramework.Net;
using SgFramework.UI;
using SgFramework.Utility;
using State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Default, "Assets/GameRes/Prefabs/UI/UILogin.prefab")]
    public class UILogin : UIForm
    {
        [SerializeField] private GameObject panelGuest;
        [SerializeField] private TMP_InputField inputGuestAccount;
        [SerializeField] private Button btnGuestLogin;
        [SerializeField] private Button btnNotice;
        [SerializeField] private TMP_Dropdown serverList;
        public override bool CanReuse => false;

        private async void Start()
        {
            UIPopNoticeList.RequestNotice(true);
            inputGuestAccount.text = LocalStorage.GetString("last_account");

            btnGuestLogin.BindClick(OnClickGuestLogin);
            btnNotice.BindClick(OnClickNotice);
            Observable.EveryValueChanged(NetManager.Shared, net => net is { IsSessionReady: true })
                .Subscribe(v => panelGuest.SetActive(!v)).AddTo(gameObject);

            //设置默认服务器
            NetManager.Host = SgAppEnv.Shared.HttpServer;
            var options = new List<TMP_Dropdown.OptionData> { new("默认服务器") };
            serverList.ClearOptions();
            serverList.AddOptions(options);

            //请求服务器列表
            var resp = await NetManager.QueryServerList(SgAppEnv.Shared.HttpServer);
            options.Clear();

            foreach (var config in resp.Servers)
            {
                if (string.IsNullOrEmpty(config.GateURL))
                {
                    config.GateURL = SgAppEnv.Shared.HttpServer;
                }

                options.Add(new TMP_Dropdown.OptionData(config.Name));
            }

            serverList.ClearOptions();
            serverList.AddOptions(options);
            serverList.onValueChanged.AddListener(index =>
            {
                var selection = resp.Servers[index];
                NetManager.Host = selection.GateURL;
                NetManager.Uuid = selection.Uuid;
                LocalStorage.SetString("last_server", selection.Uuid);
                Debug.Log($"选择服务器：{selection.Name} [{selection.Uuid}] : {selection.GateURL}");
            });

            var index = resp.Servers.FindIndex(x => x.Uuid == LocalStorage.GetString("last_server"));
            if (index == 0)
            {
                index = -1;
            }

            serverList.value = index;
        }

        private async UniTask OnClickNotice()
        {
            await UIManager.Open<UIPopNoticeList>();
        }

        private async UniTask OnClickGuestLogin()
        {
            serverList.interactable = false;
            btnGuestLogin.interactable = false;
            inputGuestAccount.interactable = false;
            var session = NetManager.Create(NetManager.Host, inputGuestAccount.text, NetManager.Uuid);
            session.Notice += NetManager.DefaultNotice;
            var result = await session.GuestLogin();
            switch (result)
            {
                case ESessionCode.Maintain:
                {
                    inputGuestAccount.interactable = true;
                    btnGuestLogin.interactable = true;
                    serverList.interactable = true;
                    break;
                }
                case ESessionCode.Failure:
                {
                    Debug.LogError("登录失败");
                    UIToast.Instance.ShowToast("登录失败").Forget();

                    inputGuestAccount.interactable = true;
                    btnGuestLogin.interactable = true;
                    serverList.interactable = true;
                    break;
                }
                case ESessionCode.Success:
                {
                    session.Error += NetManager.DefaultError;
                    GameMain.Instance.StateMachine.ChangeState<Main>().Forget();
                    LocalStorage.SetString("last_account", inputGuestAccount.text);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}