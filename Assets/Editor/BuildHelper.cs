using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.AOT;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BuildHelper
{
    [MenuItem("项目/Open Launcher &1", false, -100)]
    public static void OpenLauncher()
    {
        EditorApplication.delayCall += () =>
        {
            EditorSceneManager.OpenScene("Assets/Boot/launcher.unity");
        };
    }
    
    //[MenuItem("项目/构建程序集", false, 1)]
    public static void BuildDll()
    {
        HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
    }

    //[MenuItem("项目/拷贝dll", false, 3)]
    public static void CopyDll()
    {
        var env = HybridCLRSettings.Instance;
        const string dllPackagePath = "Assets/GameRes/HotUpdateDlls";
        var dllPackageRoot = $"{Application.dataPath}/../{dllPackagePath}";
        if (Directory.Exists(dllPackageRoot))
        {
            Directory.Delete(dllPackageRoot, true);
        }

        Directory.CreateDirectory(dllPackageRoot);

        var path = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget);
        Debug.Log(path);

        foreach (var name in env.hotUpdateAssemblies)
        {
            File.Copy($"{path}/{name}.dll", $"{dllPackageRoot}/{name}.bytes");
            Debug.Log(name);
        }

        path =
            $"{SettingsUtil.HybridCLRDataDir}/StrippedAOTAssembly2/{EditorUserBuildSettings.activeBuildTarget}";
        Debug.Log(path);

        foreach (var name in env.patchAOTAssemblies)
        {
            Debug.Log(name);
            File.Copy($"{path}/{name}.dll", $"{dllPackageRoot}/{name}.bytes");
        }

        AssetDatabase.Refresh();
    }

    //[MenuItem("项目/剔除AOT dll", false, 2)]
    public static void StripAOTAssembly()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        var srcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        var dstDir = $"{SettingsUtil.HybridCLRDataDir}/StrippedAOTAssembly2/{target}";
        foreach (var src in Directory.GetFiles(srcDir, "*.dll"))
        {
            var dllName = Path.GetFileName(src);
            var dstFile = $"{dstDir}/{dllName}";
            AOTAssemblyMetadataStripper.Strip(src, dstFile);
        }
    }
}