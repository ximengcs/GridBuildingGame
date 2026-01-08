using Cysharp.Threading.Tasks;
using SgFramework.Machine;
using SgFramework.Res;
using SgFramework.UI;
using UI;

namespace State
{
    public class Login : IStateNode
    {
        private ResourceGroup _resourceGroup;
        public void OnCreate(StateMachine machine)
        {
            
        }

        public async UniTask OnEnter()
        {
            await UIManager.Open<UILogin>();
        }

        public void OnUpdate()
        {
        }

        public async UniTask OnExit()
        {
            await UIManager.Close<UILogin>();
        }
    }
}