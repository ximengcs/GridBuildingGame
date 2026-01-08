using Cysharp.Threading.Tasks;
using SgFramework.Machine;
using UnityEngine;
using YooAsset;

/// <summary>
/// 更新资源版本号
/// </summary>
internal class FsmUpdatePackageVersion : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    UniTask IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("Check Version.");
        UpdatePackageVersion().Forget();
        return UniTask.CompletedTask;
    }
    void IStateNode.OnUpdate()
    {
    }
    UniTask IStateNode.OnExit()
    {
        return UniTask.CompletedTask;
    }

    private async UniTaskVoid UpdatePackageVersion()
    {
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
        await operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
        }
        else
        {
            Debug.Log($"Request package version : {operation.PackageVersion}");
            _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
    }
}