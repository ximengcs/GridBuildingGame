using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SgFramework.UI;
using UI;
using UnityEngine;

namespace SgFramework.Net
{
    public static class ErrorCenter
    {
        private static readonly HashSet<string> PopCode;
        private static readonly Queue<string> PopQueue = new Queue<string>();

        static ErrorCenter()
        {
            PopCode = new HashSet<string>
            {
            };
        }

        public static async void OnError(string key)
        {
            try
            {
                Debug.LogError(key);
                if (PopCode.Contains(key))
                {
                    PopQueue.Enqueue(key);
                    if (PopQueue.Count > 1)
                    {
                        return;
                    }

                    while (PopQueue.Count > 0)
                    {
                        var ui = await UIManager.Open<UIPopMessage>();
                        ui.SetData(key);
                        await UniTask.WaitUntilCanceled(ui.destroyCancellationToken);
                    }

                    return;
                }

                UIToast.Instance.ShowToast(key).Forget();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}