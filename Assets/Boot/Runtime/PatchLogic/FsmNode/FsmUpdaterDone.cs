using Cysharp.Threading.Tasks;
using SgFramework.Machine;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmUpdaterDone : IStateNode
{
    private PatchOperation _owner;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _owner = machine.Owner as PatchOperation;
    }
    UniTask IStateNode.OnEnter()
    {
        _owner.SetFinish();
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