using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pt;
using SgFramework.Res;
using SgFramework.UI;
using UIComponent;
using UnityEngine;

namespace UI
{
    [UIConfig(UILayer.Toast, "Assets/GameRes/Prefabs/UI/UIToast.prefab")]
    public class UIToast : UIForm
    {
        [SerializeField] private RectTransform lampRoot;

        public override bool CanReuse => false;

        public static UIToast Instance { get; private set; }
        private static ResourceGroup _group;

        public static async UniTask Initialize()
        {
            Instance = await UIManager.Open<UIToast>();
        }

        public static void Dispose()
        {
            UIManager.Close<UIToast>().Forget();
        }

        private void Awake()
        {
            _group = ResourceManager.GetGroup("Toast").AddTo(this);
        }

        public async UniTaskVoid ShowToast(string key)
        {
            var token = await _group.GetObject("Assets/GameRes/Prefabs/UIComponent/UIToastItem.prefab", transform,
                Vector3.zero, Quaternion.identity);
            var item = token.GetComponent<UIToastItem>();
            item.SetData(key);
            await UniTask.WaitForSeconds(1f);
            _group.ReleaseObject(token);
        }


        private readonly Queue<PushLampMsg> _lampQueue = new Queue<PushLampMsg>();

        public async UniTaskVoid ShowLamp(PushLampMsg rsp)
        {
            if (_lampQueue.Count > 15)
            {
                Debug.LogError("跑马灯队列满了。");
                return;
            }

            _lampQueue.Enqueue(rsp);
            if (_lampQueue.Count > 1)
            {
                return;
            }

            while (_lampQueue.TryPeek(out var cur))
            {
                var token = await _group.GetObject("Assets/GameRes/Prefabs/UIComponent/UILampItem.prefab", lampRoot,
                    Vector3.zero, Quaternion.identity);
                var item = token.GetComponent<UILampItem>();
                item.SetData(cur);
                await item.ShowAnim();
                if (destroyCancellationToken.IsCancellationRequested)
                {
                    _lampQueue.Clear();
                    break;
                }

                _group.ReleaseObject(token);
                _lampQueue.Dequeue();
            }
        }
    }
}