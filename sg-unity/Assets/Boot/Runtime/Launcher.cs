using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SgFramework.Env;
using SgFramework.Event;
using UnityEngine;
using YooAsset;

public class Launcher : MonoBehaviour
{
    public string packageName = "DefaultPackage";

    public GameObject patcher;

    private static bool _init;

    private async void Start()
    {
        try
        {
            Debug.Log("开始启动");
            if (!_init)
            {
                Debug.Log("启动器初始化");
                if (!SgAppEnv.Initialize())
                {
                    Debug.LogError("app配置加载失败。");
                    PatchEventDefine.PatchStatesChange.SendEventMessage("App Env Initialize Error.");
                    return;
                }

                {
                    var reporter = Instantiate(Resources.Load<GameObject>("Reporter")).GetComponent<Reporter>();
                    reporter.enabled = SgAppEnv.Shared.LogReportEnable;
                }

                SgEvent.Initialize();

                // 初始化资源框架
                YooAssets.Initialize();
                
#if UNITY_EDITOR
                var playMode = EPlayMode.EditorSimulateMode;
#else
                var playMode = (EPlayMode)SgAppEnv.Shared.PlayMode;
#endif

                // 开始补丁更新流程
                var operation = new PatchOperation(
                    packageName,
                    playMode,
                    SgAppEnv.Shared.CdnServer,
                    SgAppEnv.Shared.CdnServer,
                    SgAppEnv.Shared.Version
                );
                YooAssets.StartOperation(operation);
                await operation;

                // 设置默认的资源包
                var gamePackage = YooAssets.GetPackage(packageName);
                YooAssets.SetDefaultPackage(gamePackage);

#if !UNITY_EDITOR
                // 加载代码
                {
                    Debug.Log("加载程序集");
                    var handle = YooAssets.LoadAssetAsync("Assets/GameRes/HotUpdateDlls/Assembly-CSharp.bytes");
                    await handle;
                    System.Reflection.Assembly.Load(handle.GetAssetObject<TextAsset>().bytes);
                }
#endif

                _init = true;
            }

            await Enter();
        }
        catch (Exception e)
        {
            Debug.LogError($"游戏启动失败{e.Message}");
        }
    }

    private static AssetHandle _enterHandle;

    private async ValueTask Enter()
    {
        Debug.Log("进入游戏");
        patcher.SetActive(false);
        if (_enterHandle != null)
        {
            //GC
            await YooAssets.GetPackage(packageName).UnloadUnusedAssetsAsync();
            GC.Collect();
        }

        _enterHandle ??= YooAssets.LoadAssetAsync<GameObject>("Assets/GameRes/GameMain.prefab");
        await _enterHandle;
        _enterHandle.InstantiateSync().name = $"[GameMain]";
        Destroy(gameObject);
    }
}