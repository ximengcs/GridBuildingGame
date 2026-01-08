using System;
using System.Collections.Generic;
using System.Diagnostics;
using Common;
using Config;
using Cysharp.Threading.Tasks;
using HybridCLR;
using Newtonsoft.Json;
using SgFramework.Env;
using SgFramework.Font;
using SgFramework.Machine;
using SgFramework.Net;
using SgFramework.Res;
using SgFramework.UI;
using State;
using UI;
using UnityEngine;
using YooAsset;
using Debug = UnityEngine.Debug;

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行

public class GameMain : MonoBehaviour
{
    public static GameMain Instance { get; private set; }
    [SerializeField] private GameObject uiRoot;

    public StateMachine StateMachine { get; private set; }
    private static bool _initialized;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize().Forget();
    }

    private async UniTask LoadMetadataForAOTAssembly()
    {
        if (Application.isEditor || _initialized)
        {
            return;
        }

        var jsonHandle = YooAssets.LoadAssetAsync<TextAsset>("Assets/GameRes/aot_list.json");
        await jsonHandle;
        var aotDllList = JsonConvert.DeserializeObject<List<string>>(jsonHandle.GetAssetObject<TextAsset>().text);
        jsonHandle.Release();

        foreach (var aotDllName in aotDllList)
        {
            var key = $"Assets/GameRes/HotUpdateDlls/{aotDllName}";
            var handle = YooAssets.LoadAssetAsync<TextAsset>(key);
            await handle;
            var err = RuntimeApi.LoadMetadataForAOTAssembly(handle.GetAssetObject<TextAsset>().bytes,
                HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            handle.Release();
        }
    }

    private async UniTaskVoid Initialize()
    {
        Debug.Log("游戏初始化");
        await LoadMetadataForAOTAssembly();

        Application.targetFrameRate = SgAppEnv.Shared.FrameRate;

        Debug.Log("初始化运行配置");
        await RuntimeConfig.Initialize();
        Debug.Log("加载游戏配置");
        await InitializeConfig((c, t) => { Debug.Log($"Config load {c}/{t}"); });

        Debug.Log("初始化字体管理器");
        await FontManager.Instance.Initialize();

        Debug.Log("初始化UI管理器");
        await UIManager.Initialize();

        Debug.Log("初始化完成，启动游戏");
        _initialized = true;
        StateMachine = new StateMachine(this)
        {
            OnStateChangeStart = OnStateChangeStart,
            OnStateChangeEnd = OnStateChangeEnd
        };

        StateMachine.AddNode<InitEnv>();
        StateMachine.AddNode<Login>();
        StateMachine.AddNode<Main>();
        StateMachine.AddNode<Restart>();
        StateMachine.Run<InitEnv>();
    }

    private async UniTask OnStateChangeStart(IStateNode cur, IStateNode next)
    {
        if (cur is Login && next is Main)
        {
            await UIManager.Open<UILoading>();
        }
    }

    private async UniTask OnStateChangeEnd(IStateNode cur, IStateNode next)
    {
        if (cur is Login && next is Main)
        {
            await UIManager.Close<UILoading>();
        }
    }

    private async UniTask InitializeConfig(Action<int, int> onProgress = null)
    {
        if (_initialized)
        {
            return;
        }

        var sw = new Stopwatch();
        sw.Start();
        var handle = ResourceManager.LoadAllAssetsAsync<TextAsset>("Assets/GameRes/Config/Lang.json");
        await handle;
        var dict = new Dictionary<string, string>();
        foreach (var obj in handle.AllAssetObjects)
        {
            dict[obj.name] = ((TextAsset)obj).text;
        }

        handle.Release();

        sw.Stop();
        Debug.Log($"load config cost {sw.ElapsedTicks / Stopwatch.Frequency: 0.00}s");
        await UniTask.SwitchToThreadPool();
        sw.Restart();
        Table.Initialize(key => dict[key], onProgress);
        sw.Stop();
        Debug.Log($"parse config cost {sw.ElapsedTicks / Stopwatch.Frequency: 0.00}s");
        await UniTask.SwitchToMainThread();
    }

    private void OnDestroy()
    {
        NetManager.Dispose();
    }

    /// <summary>
    /// 重启游戏
    /// </summary>
    public void Dispose()
    {
        Destroy(gameObject);
    }
}