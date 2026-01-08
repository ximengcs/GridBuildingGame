using System;
using Best.HTTP;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SgFramework.UI;
using UI;
using UnityEngine;

namespace SgFramework.Net
{
    public static class NetManager
    {
        /// <summary>
        /// 现在只需要一条连接，暂时使用静态对象保存
        /// </summary>
        public static NetSession Shared { get; private set; }

        public static string Host { get; set; }
        public static string Uuid { get; set; }
        public static string Account { get; set; }

        public static NetSession Create()
        {
            return Shared = Create(Host, Account, Uuid);
        }

        public static NetSession Create(string host, string account, string uuid)
        {
            Dispose();
            return Shared = new NetSession(host, account, uuid);
        }

        public static void Dispose()
        {
            Shared?.Dispose();
            Shared = null;
        }

        public static void DefaultError(Exception e)
        {
            Debug.LogError(e);
            UIManager.Open<UIPopNetError>().Forget();
        }

        public static async UniTask<HttpApi.QueryServerListResp> QueryServerList(string host)
        {
            try
            {
                Debug.Log($"query server list at {host}");
                const string path = "/sg/servers";
                var request = HTTPRequest.CreateGet($"{host}{path}");
                request.TimeoutSettings.Timeout = TimeSpan.FromSeconds(5f);

                var resp = await request.GetHTTPResponseAsync();
                if (resp.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<HttpApi.QueryServerListResp>(resp.DataAsText);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        public static void DefaultNotice(HttpApi.MaintainInfo info)
        {
            UIManager.Open<UIPopNotice>(info).Forget();
        }
    }
}