using SgFramework.Font;
using TMPro;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class FontManagerEditor
{
    static FontManagerEditor()
    {
        var f = ResetFontSetting();
        SgTMP_Settings.AssetLoader = () => f;
        EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
    }

    public static TMP_FontAsset ResetFontSetting()
    {
        var f = AssetDatabase.LoadAssetAtPath<FontConfig>("Assets/GameRes/FontConfig/en/FontConfig.asset").fontAsset;
        f.fallbackFontAssetTable.Clear();
        f.fallbackFontAssetTable.Add(
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/GameRes/FontConfig/_no_edit/Alibaba-PuHuiTi-Editor.asset"));
        return f;
    }

    private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
    {
        if (obj != PlayModeStateChange.EnteredEditMode)
        {
            return;
        }

        //清理字体材质
        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/GameRes/FontConfig" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("_no_auto_edit"))
            {
                continue;
            }

            var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            asset.ClearFontAssetData(true);
            asset.characterLookupTable.Clear();

            if (!path.Contains("_no_edit"))
            {
                asset.atlasPopulationMode = AtlasPopulationMode.Static;
            }

            asset.fallbackFontAssetTable.Clear();
            if (path.Contains("LiberationSans SDF"))
            {
                asset.fallbackFontAssetTable.Add(
                    AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                        "Assets/GameRes/FontConfig/_no_edit/Alibaba-PuHuiTi-Editor.asset"));
            }

            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("自动清理字体缓存");
    }
}