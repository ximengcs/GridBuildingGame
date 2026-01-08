using Cysharp.Threading.Tasks;
using SgFramework.Audio;
using SgFramework.Language;
using SgFramework.Machine;
using SgFramework.Net;
using SgFramework.RedPoint;
using SgFramework.UI;
using UI;

namespace State
{
    /// <summary>
    /// 初始化游戏运行环境
    /// </summary>
    public class InitEnv : IStateNode
    {
        private StateMachine _machine;
        public void OnCreate(StateMachine machine)
        {
            _machine = machine;
        }

        public async UniTask OnEnter()
        {
            LanguageManager.Initialize();
            AudioManager.Instance.Initialize();
            RedPointManager.Instance.Initialize();
            await UIManager.Open<UIInitEnv>();
            await UIToast.Initialize();
            NetUtility.Initialize();

            _machine.ChangeState<Login>().Forget();
        }

        public void OnUpdate()
        {
            
        }

        public async UniTask OnExit()
        {
            await UIManager.Close<UIInitEnv>();
        }
    }
}