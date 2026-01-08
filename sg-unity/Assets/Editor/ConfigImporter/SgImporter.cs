using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class SgImporter : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset = default;

    private ImportConfig _importConfig;
    private const string FilePath = "UserSettings/sg_importer.json";

    [MenuItem("项目/项目资源导入窗口 &3")]
    public static void ShowExample()
    {
        GetWindow<SgImporter>().titleContent = new GUIContent("SgImporter");
    }

    public void CreateGUI()
    {
        _importConfig = Load();

        var root = rootVisualElement;
        root.Add(visualTreeAsset.Instantiate());
        RefreshView();
        root.Q<Button>("btn-proto-path").clicked += () =>
        {
            var path = EditorUtility.OpenFolderPanel("Proto Folder", Application.dataPath, default);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            _importConfig.ProtoPath = path;
            Save();
        };
        root.Q<Button>("btn-config-path").clicked += () =>
        {
            var path = EditorUtility.OpenFolderPanel("Config Folder", Application.dataPath, default);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            _importConfig.ConfigPath = path;
            Save();
        };
        root.Q<Button>("btn-proto-update").clicked += () => { GitUpdate(_importConfig); };
        root.Q<Button>("btn-proto-gen").clicked += () => { GenProto(_importConfig); };
        root.Q<Button>("btn-config-update").clicked += () => { SvnUpdate(_importConfig); };
        root.Q<Button>("btn-config-gen").clicked += () => { GenConfig(_importConfig); };
        root.Q<Button>("btn-scv-gen").clicked += () =>
        {
            EditorUtility.DisplayProgressBar("变体搜集", "搜集中", 0.9f);
            ShaderVariantCollector.Run(_importConfig.ScvPath, "DefaultPackage", 100, () =>
            {
                BuildHelper.OpenLauncher();
                EditorUtility.ClearProgressBar();
            });
        };
    }

    private void RefreshView()
    {
        var root = rootVisualElement;
        root.Q<TextField>("proto-path").SetValueWithoutNotify(_importConfig.ProtoPath);
        root.Q<TextField>("config-path").SetValueWithoutNotify(_importConfig.ConfigPath);
        root.Q<TextField>("scv-path").SetValueWithoutNotify(_importConfig.ScvPath);
    }

    public static ImportConfig Load()
    {
        var config = new ImportConfig();
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(config));
        }
        else
        {
            config = JsonConvert.DeserializeObject<ImportConfig>(File.ReadAllText(FilePath));
        }

        return config;
    }

    private void Save()
    {
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(_importConfig));
        RefreshView();
    }

    public static void GenProto(ImportConfig config)
    {
        Execute(Path.Combine(Application.dataPath, "../Tools/protoc.exe"),
            $"-I{config.ProtoPath} --csharp_out=Scripts/Gen/Proto {config.ProtoPath}/*.proto");
        AssetDatabase.Refresh();
    }

    private static void GitUpdate(ImportConfig config)
    {
        Execute("git", $"-C {config.ProtoPath} reset --hard");
        Execute("git", $"-C {config.ProtoPath} pull");
    }

    private static void SvnUpdate(ImportConfig config)
    {
        Execute("svn", $"revert {config.ConfigPath} -R");
        Execute("svn", $"update {config.ConfigPath}");
    }

    public static void GenConfig(ImportConfig config)
    {
        Execute(Path.Combine(Application.dataPath, "../Tools/genConfig.exe"),
            $"export_csharp {config.ConfigPath} GameRes/Config Scripts/Gen/Config");
        AssetDatabase.Refresh();
    }

    private static void Execute(string path, string arg)
    {
        try
        {
            // 创建进程对象
            var process = new Process();

            // 配置进程启动信息
            var startInfo = new ProcessStartInfo
            {
                FileName = path, // 文件路径
                WorkingDirectory = Application.dataPath, // 设置工作目录
                Arguments = arg,
                UseShellExecute = false, // 不使用操作系统shell启动
                RedirectStandardOutput = true, // 重定向标准输出
                RedirectStandardError = true, // 重定向标准错误
                CreateNoWindow = true // 不创建新窗口
            };

            // 启动进程
            process.StartInfo = startInfo;
            process.Start();

            // 读取输出
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            // 等待进程结束
            process.WaitForExit();

            // 输出结果
            if (string.IsNullOrEmpty(output))
            {
                Debug.Log($"{path}执行结束没有输出信息。");
            }
            else
            {
                Debug.Log("输出: " + output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Debug.Log("输出错误: " + error);
            }

            // 关闭进程
            process.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("执行时出错: " + ex.Message);
        }
    }
}