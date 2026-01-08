using Cysharp.Threading.Tasks;
using SgFramework.Machine;
using YooAsset;

/// <summary>
/// 下载更新文件
/// </summary>
public class FsmDownloadPackageFiles : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    UniTask IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("Download Files.");
        BeginDownload().Forget();
        return UniTask.CompletedTask;
    }
    void IStateNode.OnUpdate()
    {
    }
    UniTask IStateNode.OnExit()
    {
        return UniTask.CompletedTask;
    }

    private async UniTaskVoid BeginDownload()
    {
        var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
        downloader.DownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
        downloader.DownloadUpdateCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
        downloader.BeginDownload();
        await downloader;

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
        {
            return;
        }

        _machine.ChangeState<FsmDownloadPackageOver>();
    }
}