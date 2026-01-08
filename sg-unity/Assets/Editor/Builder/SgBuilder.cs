using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HybridCLR.Editor;
using HybridCLR.Editor.AOT;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using YooAsset;
using YooAsset.Editor;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Settings;
using Newtonsoft.Json;
using SgFramework.Env;
using TMPro;
using UnityEditor.Build;

namespace Builder
{
    public class SgBuilder : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;

        private BuildConfig _buildConfig;

        private readonly List<Action> _setActions = new List<Action>();

        [MenuItem("项目/项目构建窗口 &2")]
        public static void ShowWindow()
        {
            GetWindow<SgBuilder>().titleContent = new GUIContent("SgBuilder");
        }

        private void BindToggle(VisualElement root, string key, EventCallback<ChangeEvent<bool>> action,
            Func<bool> setter)
        {
            var t = root.Q<Toggle>(key);
            t.SetValueWithoutNotify(setter());
            t.RegisterValueChangedCallback(action);
            _setActions.Add(SetAction);
            return;

            void SetAction()
            {
                t.SetValueWithoutNotify(setter());
            }
        }

        private void BindDropdown(VisualElement root, string key, EventCallback<ChangeEvent<Enum>> action,
            Func<Enum> setter)
        {
            var t = root.Q<EnumField>(key);
            t.SetValueWithoutNotify(setter());
            t.RegisterValueChangedCallback(action);
            _setActions.Add(SetAction);
            return;

            void SetAction()
            {
                t.SetValueWithoutNotify(setter());
            }
        }

        private void BindDropdownList(VisualElement root, string key, EventCallback<ChangeEvent<string>> action,
            Func<string> setter, List<string> data)
        {
            var t = root.Q<DropdownField>(key);
            t.choices = data;
            SetAction();
            t.RegisterValueChangedCallback(action);
            _setActions.Add(SetAction);
            return;

            void SetAction()
            {
                t.index = Mathf.Max(0, data.IndexOf(setter()));
            }
        }

        private void BindButton(VisualElement root, string key, Action action)
        {
            root.Q<Button>(key).clicked += action;
        }

        private void BindInput(VisualElement root, string key, EventCallback<ChangeEvent<string>> action,
            Func<string> setter)
        {
            var t = root.Q<TextField>(key);
            t.SetValueWithoutNotify(setter());
            t.RegisterValueChangedCallback(action);
            _setActions.Add(SetAction);
            return;

            void SetAction()
            {
                t.SetValueWithoutNotify(setter());
            }
        }

        public void CreateGUI()
        {
            _buildConfig = new BuildConfig();
            var root = rootVisualElement;
            root.Add(visualTreeAsset.Instantiate());
            BindInput(root, "input-version", v => { _buildConfig.Version = v.newValue; }, () => _buildConfig.Version);

            var fileList = Directory.GetFiles("GameConfig", "*.json").ToList();
            BindDropdownList(root, "dropdown-game-config", e => { _buildConfig.GameConfig = e.newValue; },
                () => _buildConfig.GameConfig, fileList);

            BindDropdown(root, "dd-build-target", v => { _buildConfig.BuildTarget = (BuildTarget)v.newValue; },
                () => _buildConfig.BuildTarget);

            BindToggle(root, "tg-gen-proto", v => { _buildConfig.GenProto = v.newValue; },
                () => _buildConfig.GenProto);

            BindToggle(root, "tg-gen-config", v => { _buildConfig.GenConfig = v.newValue; },
                () => _buildConfig.GenConfig);

            BindToggle(root, "tg-generate-all", v => { _buildConfig.GenerateAll = v.newValue; },
                () => _buildConfig.GenerateAll);

            BindToggle(root, "tg-compile-target", v => { _buildConfig.CompileTarget = v.newValue; },
                () => _buildConfig.CompileTarget);

            BindToggle(root, "tg-build-res", v => { _buildConfig.BuildRes = v.newValue; },
                () => _buildConfig.BuildRes);

            BindToggle(root, "tg-copy-res", v => { _buildConfig.CopyRes = v.newValue; },
                () => _buildConfig.CopyRes);

            BindInput(root, "input-copy-tags", v => { _buildConfig.CopyTags = v.newValue; },
                () => _buildConfig.CopyTags);

            BindToggle(root, "tg-build-player", v => { _buildConfig.BuildPlayer = v.newValue; },
                () => _buildConfig.BuildPlayer);

            BindButton(root, "btn-build",
                () => { EditorApplication.delayCall += () => { ExecuteBuild(_buildConfig); }; });

            BindButton(root, "btn-save",
                () =>
                {
                    var path = EditorUtility.SaveFilePanel("Save",
                        Path.Combine(Application.dataPath, "../BuildScripts"),
                        "build_config", "json");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    var json = JsonConvert.SerializeObject(_buildConfig);
                    File.WriteAllText(path, json);
                });

            BindButton(root, "btn-load",
                () =>
                {
                    Debug.Log(Path.Combine(Application.dataPath, "../BuildScripts"));
                    var path = EditorUtility.OpenFilePanel("Load",
                        Path.Combine(Application.dataPath, "../BuildScripts"), "json");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    var json = File.ReadAllText(path);
                    _buildConfig = JsonConvert.DeserializeObject<BuildConfig>(json);
                    foreach (var action in _setActions)
                    {
                        action();
                    }
                });
        }

        private static void GenerateAll(BuildConfig config)
        {
            var installer = new InstallerController();
            if (!installer.HasInstalledHybridCLR())
            {
                throw new BuildFailedException(
                    $"You have not initialized HybridCLR, please install it via menu 'HybridCLR/Installer'");
            }

            var target = config.BuildTarget;
            CompileDllCommand.CompileDll(target);
            Il2CppDefGeneratorCommand.GenerateIl2CppDef();

            // 这几个生成依赖HotUpdateDlls
            LinkGeneratorCommand.GenerateLinkXml(target);

            // 生成裁剪后的aot dll
            StripAOTDllCommand.GenerateStripedAOTDlls(target);

            // 桥接函数生成依赖于AOT dll，必须保证已经build过，生成AOT dll
            MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(target);
            AOTReferenceGeneratorCommand.GenerateAOTGenericReference(target);
        }

        private static void CopyGameConfig(BuildConfig config)
        {
            Debug.Log($"复制{config.GameConfig}");
            var from = Path.Combine(Application.dataPath, $"../{config.GameConfig}");
            var to = Path.Combine(Application.dataPath, $"Boot/Resources/game_config.json");
            var json = File.ReadAllText(from);
            File.WriteAllText(to, json);
            AssetDatabase.Refresh();
        }

        private static void ExecuteBuild(BuildConfig config)
        {
            CopyGameConfig(config);
            if (config.GenProto)
            {
                var c = SgImporter.Load();
                SgImporter.GenProto(c);
            }

            if (config.GenConfig)
            {
                var c = SgImporter.Load();
                SgImporter.GenConfig(c);
            }

            if (config.GenerateAll)
            {
                Debug.Log("generate all");
                GenerateAll(config);
            }
            else if (config.CompileTarget)
            {
                Debug.Log("compile dll");
                CompileDllCommand.CompileDll(config.BuildTarget);
            }

            //要构建资源
            if (config.BuildRes)
            {
                //清理字体材质
                var guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/GameRes/FontConfig" });
                foreach (var guid in guids)
                {
                    var fontAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (fontAssetPath.Contains("_no_edit"))
                    {
                        continue;
                    }

                    var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
                    if (fontAssetPath.Contains("_no_auto_edit"))
                    {
                        //asset.sourceFontFile = null;
                        EditorUtility.SetDirty(asset);
                        continue;
                    }

                    asset.ClearFontAssetData(true);
                    asset.characterLookupTable.Clear();
                    asset.atlasPopulationMode = AtlasPopulationMode.Static;
                    asset.fallbackFontAssetTable.Clear();
                    EditorUtility.SetDirty(asset);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                StripAOTAssembly();
                CopyDll();

                var copyMode = EBuildinFileCopyOption.None;
                if (config.CopyRes)
                {
                    copyMode = EBuildinFileCopyOption.ClearAndCopyByTags;

                    var asset = Resources.Load<TextAsset>("game_config");
                    var runConf = JsonConvert.DeserializeObject<SgAppEnv>(asset.text);
                    Debug.Log($"运行模式为：{(EPlayMode)runConf.PlayMode}");
                    if (runConf.PlayMode == (int)EPlayMode.OfflinePlayMode)
                    {
                        copyMode = EBuildinFileCopyOption.ClearAndCopyAll;
                    }
                }

                var buildParameters = new BuiltinBuildParameters
                {
                    BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(),
                    BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot(),
                    BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString(),
                    BuildBundleType = (int)EBuildBundleType.AssetBundle,
                    BuildTarget = config.BuildTarget,
                    PackageName = "DefaultPackage",
                    PackageVersion = GetDefaultPackageVersion(),
                    EnableSharePackRule = true,
                    VerifyBuildingResult = true,
                    FileNameStyle = EFileNameStyle.BundleName_HashName,
                    BuildinFileCopyOption = copyMode,
                    BuildinFileCopyParams = config.CopyTags,
                    CompressOption = ECompressOption.Uncompressed,
                    ClearBuildCacheFiles = true,
                    UseAssetDependencyDB = true,
                    EncryptionServices = null
                };

                var pipeline = new BuiltinBuildPipeline();
                var buildResult = pipeline.Run(buildParameters, true);
                if (!buildResult.Success)
                {
                    Debug.LogError("构建失败");
                    return;
                }

                if (!config.BuildPlayer)
                {
                    EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
                    Debug.Log("构建结束");
                    FontManagerEditor.ResetFontSetting();
                }
            }

            if (!config.BuildPlayer)
            {
                return;
            }

            Debug.Log("构建Player");
            switch (config.BuildTarget)
            {
                case BuildTarget.iOS:
                {
                    break;
                }
                case BuildTarget.Android:
                {
                    BuildPlayer.BuildApk(config);
                    break;
                }
                case BuildTarget.StandaloneWindows64:
                {
                    BuildPlayer.BuildWin64(config);
                    break;
                }
                case BuildTarget.WebGL:
                {
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FontManagerEditor.ResetFontSetting();
        }

        private static string GetDefaultPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }


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

            var sb = new List<string>();
            foreach (var name in env.patchAOTAssemblies)
            {
                Debug.Log(name);
                File.Copy($"{path}/{name}.dll", $"{dllPackageRoot}/{name}.bytes");
                sb.Add(name);
            }

            var json = JsonConvert.SerializeObject(sb);
            File.WriteAllText($"Assets/GameRes/aot_list.json", json);

            AssetDatabase.Refresh();
        }

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

        public static void BuildWithPath()
        {
            var args = Environment.GetCommandLineArgs();
            var path = args[8];
            Console.WriteLine($"使用配置文件打包:{path}");
            if (!File.Exists(path))
            {
                Console.WriteLine($"配置路径错误:{path}");
                return;
            }

            var json = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<BuildConfig>(json);
            ExecuteBuild(config);
        }
    }
}