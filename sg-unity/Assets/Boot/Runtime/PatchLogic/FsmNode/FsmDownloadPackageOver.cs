using Cysharp.Threading.Tasks;
using SgFramework.Machine;

/// <summary>
/// 下载完毕
/// </summary>
internal class FsmDownloadPackageOver : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    UniTask IStateNode.OnEnter()
    {
        _machine.ChangeState<FsmClearPackageCache>();
        return UniTask.CompletedTask;
    }
    void IStateNode.OnUpdate()
    {
    }
    UniTask IStateNode.OnExit()
    {
        return UniTask.CompletedTask;
    }
}