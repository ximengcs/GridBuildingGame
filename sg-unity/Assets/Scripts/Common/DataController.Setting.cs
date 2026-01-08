using System.Collections.Generic;
using Config;
using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.Audio;
using SgFramework.Font;
using SgFramework.Language;
using SgFramework.Net;
using SgFramework.Utility;
using UnityEngine;

namespace Common
{
    public abstract class SettingKey
    {
        public const string BgmSetting = "bgm_setting";
        public const string BgmSettingValue = "bgm_setting_value";
        public const string SfxSetting = "sfx_setting";
        public const string SfxSettingValue = "sfx_setting_value";
        public const string LanguageSetting = "language_setting";
    }

    public partial class DataController
    {
        public static Subject<int> LanguageChanged { get; } = new Subject<int>();

        private static bool _settingDirty;

        public static void SetupSetting()
        {
            AudioManager.Instance.BgmOn = GetSettingAsBool(SettingKey.BgmSetting);
            AudioManager.Instance.BgmVolume = GetSettingAsFloat(SettingKey.BgmSettingValue);
            AudioManager.Instance.SfxOn = GetSettingAsBool(SettingKey.SfxSetting);
            AudioManager.Instance.SfxVolume = GetSettingAsFloat(SettingKey.SfxSettingValue);
            LanguageManager.CurrentLang = GetCurrentLangType();
            FontManager.Instance.ChangeFont(GetLanguageSetting());
        }

        public static void UploadSetting()
        {
            if (!_settingDirty)
            {
                return;
            }

            NetManager.Shared.Send(new ChangeSetUpMsg
            {
                Setting = { Archive.Setting }
            });
            _settingDirty = false;
            Debug.Log("上传设置信息");
        }

        public static string GetSetting(string key)
        {
            return Archive.Setting.GetValueOrDefault(key);
        }

        public static bool GetSettingAsBool(string key)
        {
            if (Archive == null)
            {
                return default;
            }

            return Archive.Setting.TryGetValue(key, out var value) && bool.TryParse(value, out var result) && result;
        }

        public static int GetSettingAsInt(string key)
        {
            if (Archive == null)
            {
                return default;
            }

            return Archive.Setting.TryGetValue(key, out var value) && int.TryParse(value, out var result) ? result : 0;
        }

        public static float GetSettingAsFloat(string key)
        {
            if (Archive == null)
            {
                return default;
            }

            return Archive.Setting.TryGetValue(key, out var value) && float.TryParse(value, out var result)
                ? result
                : 0f;
        }

        public static LangType GetCurrentLangType()
        {
            return Table.LangTypeTable.TryGetById(GetLanguageSetting(), out var type) ? type : null;
        }

        public static int GetLanguageSetting()
        {
            if (!Archive.Setting.TryGetValue(SettingKey.LanguageSetting, out var value) ||
                !int.TryParse(value, out var result))
            {
                return SgUtility.GetSystemLanguage();
            }

            return result;
        }

        public static void SetLanguageSetting(int value)
        {
            SetSetting(SettingKey.LanguageSetting, value);
            SgUtility.SetLanguage(value);

            NetManager.Shared.Send(new SetDeviceLanguageMsg
            {
                Language = value
            });
            LanguageManager.CurrentLang = GetCurrentLangType();
            FontManager.Instance.ChangeFont(value).Forget();
            LanguageChanged.OnNext(value);
        }

        public static void SetSetting(string key, object value)
        {
            Archive.Setting[key] = value.ToString();
            _settingDirty = true;

            UploadSetting();
        }
    }
}