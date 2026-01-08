using System;
using System.Collections.Generic;
using Config;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SgFramework.Language;
using SgFramework.Res;
using SgFramework.Utility;
using TMPro;
using YooAsset;
using Object = UnityEngine.Object;

namespace SgFramework.Font
{
    public class FontManager
    {
        public static FontManager Instance { get; } = new FontManager();

        private FontConfig _defaultFont;

        private FontConfig _currentFont;

        public string DefaultFont { get; private set; }
        public string CurrentFont { get; private set; }

        private static readonly Dictionary<string, string> Cache = new Dictionary<string, string>();
        private readonly Dictionary<string, AssetHandle> _assetHandles = new Dictionary<string, AssetHandle>();

        private AssetHandle _staticAsset;

        private bool _initialized;

        public async UniTask Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _staticAsset =
                ResourceManager.LoadAssetAsync<TMP_FontAsset>(
                    "Assets/GameRes/FontConfig/_no_auto_edit/Alibaba-PuHuiTi-Static.asset");
            await _staticAsset;

            TMP_Text.OnFontAssetRequest += TMP_TextOnOnFontAssetRequest;
            TMP_Text.OnFontMaterialRequest += TMP_TextOnOnFontMaterialRequest;
            await ChangeFont(3);
            await ChangeFont(SgUtility.GetLanguage());
            _initialized = true;
        }

        private Material TMP_TextOnOnFontMaterialRequest(string code)
        {
            Debug.Log($"font mat:{code}");
            return null;
        }

        private TMP_FontAsset TMP_TextOnOnFontAssetRequest(int fontHashCode, string code)
        {
            return _defaultFont.fontAsset;
        }

        private static string GetResourceKey(string key)
        {
            if (Cache.TryGetValue(key, out var value))
            {
                return value;
            }

            value = $"Assets/GameRes/FontConfig/{key}/FontConfig.asset";
            Cache.Add(key, value);
            return value;
        }

        public bool IsFontReady(string key)
        {
            var resKey = GetResourceKey(key);
            return ResourceManager.CheckLocationValid(resKey) && !ResourceManager.IsNeedDownloadFromRemote(resKey);
        }

        public UniTask ChangeFont(int lanId)
        {
            if (!Table.LangTypeTable.DataDict.TryGetValue(lanId, out var langType))
            {
                Debug.LogError($"不支持这种语言{lanId}");
                return UniTask.CompletedTask;
            }

            Debug.Log($"根据语言{langType.lang_type_name}，加载字体");
            foreach (var (list, config) in LanguageManager.Lang2Font)
            {
                if (!list.Contains(langType.lang_type))
                {
                    continue;
                }

                return LoadFont(config.FontKey);
            }

            return LoadFont("en");
        }

        public async UniTask LoadFont(string key)
        {
            Debug.Log($"加载字体{key}");
            if (!IsFontReady(key))
            {
                Debug.LogError($"字体需要下载{key}");
                return;
            }

            if (CurrentFont == key)
            {
                return;
            }

            var resKey = GetResourceKey(key);
            if (!_assetHandles.TryGetValue(key, out var handle))
            {
                handle = ResourceManager.LoadAssetAsync<FontConfig>(resKey);
                _assetHandles.Add(key, handle);
            }

            await handle;
            var config = handle.GetAssetObject<FontConfig>();
            if (_defaultFont == null)
            {
                SetupFont(config);
                _defaultFont = config;
                DefaultFont = key;
                SetFallback(null);
            }
            else if (_defaultFont == config && _currentFont != null && _currentFont != config)
            {
                CleanFont(_currentFont);
                _currentFont = null;

                SetupFont(_defaultFont);
                SetFallback(null);
            }
            else if (_currentFont == null || _currentFont != config)
            {
                CleanFont(_defaultFont);

                SetupFont(config);
                _currentFont = config;
                SetFallback(config);
            }
            else
            {
                return;
            }

            foreach (var languageText in Object.FindObjectsByType<LanguageText>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                languageText.RefreshText();
            }

            foreach (var tmpText in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                tmpText.SetAllDirty();
            }

            var unloadKey = CurrentFont;
            CurrentFont = key;
            if (DefaultFont == unloadKey || string.IsNullOrEmpty(unloadKey))
            {
                return;
            }

            if (_assetHandles.Remove(unloadKey, out var unloadHandle))
            {
                unloadHandle.Release();
                ResourceManager.TryUnloadUnusedAsset(GetResourceKey(unloadKey));
            }

            Debug.Log($"unload font {unloadKey}");
        }

        private static void CleanFont(FontConfig fontConfig)
        {
            fontConfig.fontAsset.ClearFontAssetData(true);
            fontConfig.fontAsset.characterLookupTable.Clear();
            fontConfig.fontAsset.atlasPopulationMode = AtlasPopulationMode.Static;
        }

        private static void SetupFont(FontConfig fontConfig)
        {
            fontConfig.fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            fontConfig.fontAsset.sourceFontFile = fontConfig.font;
        }

        private void SetFallback(FontConfig fontConfig)
        {
            _defaultFont.fontAsset.fallbackFontAssetTable.Clear();
            //todo add static font
            _defaultFont.fontAsset.fallbackFontAssetTable.Add(_staticAsset.GetAssetObject<TMP_FontAsset>());

            if (fontConfig == null)
            {
                return;
            }

            _defaultFont.fontAsset.fallbackFontAssetTable.Insert(0, fontConfig.fontAsset);
        }
    }
}