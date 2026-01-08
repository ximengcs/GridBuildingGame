using Cysharp.Threading.Tasks;
using SgFramework.Audio;
using SgFramework.Machine;
using SgFramework.Net;
using SgFramework.RedPoint;
using SgFramework.Utility;
using UI;
using UnityEngine.SceneManagement;

namespace State
{
    /// <summary>
    /// 卸载游戏运行环境，并重新开始
    /// </summary>
    public class Restart : IStateNode
    {
        public void OnCreate(StateMachine machine)
        {
        }

        public async UniTask OnEnter()
        {
            AudioManager.Instance.Dispose();
            await UIBlockAllClick.Ref();
            NetManager.Dispose();
            RedPointManager.Instance.Dispose();
            CheckingChain.Instance.Dispose();
            UIToast.Dispose();
            GameMain.Instance.Dispose();
            await UniTask.WaitForSeconds(0.5f);
            SceneManager.LoadScene(0);
        }

        public void OnUpdate()
        {
        }

        public UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }
    }
}