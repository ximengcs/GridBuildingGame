
using Cysharp.Threading.Tasks;

namespace SgFramework.Machine
{
    public interface IStateNode
    {
        void OnCreate(StateMachine machine);
        UniTask OnEnter();
        void OnUpdate();
        UniTask OnExit();

        virtual void OnStart()
        {
        }
    }
}