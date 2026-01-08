using System;
using Common;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using SgFramework.Audio;
using SgFramework.Machine;
using SgFramework.UI;
using SgFramework.Utility;
using UI;

namespace State
{
    public class Main : IStateNode
    {
        public void OnCreate(StateMachine machine)
        {
        }

        public async UniTask OnEnter()
        {
            DataController.SetupSetting();
            AudioManager.Instance.PlayBgm("bgm_01");
            DataController.RefreshFriendAll().Forget();
            await UIManager.Open<UIMain>();
            await UIManager.Open<UIChat>();
            await UIManager.Open<UIGMFloatingBall>();
            GC.Collect();
            await UIManager.UnloadUnusedAssets();
            await World.Create(1010);
        }

        public void OnUpdate()
        {
        }

        public async UniTask OnExit()
        {
            await UIManager.Close<UIGMFloatingBall>();
            await UIManager.Close<UIChat>();
            await UIManager.Close<UIMain>();
            World.Destory(1010);
        }

        public void OnStart()
        {
            CheckingChain.Instance.Check();
        }
    }
}