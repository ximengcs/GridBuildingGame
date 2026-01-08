using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SgFramework.Utility
{
    internal struct CheckData
    {
        public Func<UniTask<bool>> Checking;
        public int Priority;
    }

    public class CheckingChain
    {
        private readonly List<CheckData> _checkData = new List<CheckData>();
        public static CheckingChain Instance { get; } = new CheckingChain();
        public bool IsChecking { get; private set; }

        public void Register(Func<UniTask<bool>> checking, int priority)
        {
            _checkData.Add(new CheckData
            {
                Checking = checking,
                Priority = priority
            });

            _checkData.Sort((a, b) =>
            {
                if (a.Priority == b.Priority)
                {
                    return 0;
                }

                return a.Priority < b.Priority ? -1 : 1;
            });
        }

        public async void Check()
        {
            try
            {
                if (IsChecking)
                {
                    Debug.LogWarning("检查链运行中");
                    return;
                }

                IsChecking = true;
                foreach (var data in _checkData)
                {
                    var ret = await data.Checking();
                    if (!ret)
                    {
                        break;
                    }
                }

                Debug.Log("[GAME]Checking Finish");
                IsChecking = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Dispose()
        {
            IsChecking = false;
            _checkData.Clear();
        }
    }
}