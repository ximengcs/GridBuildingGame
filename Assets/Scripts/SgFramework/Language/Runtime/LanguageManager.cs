using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config;
using SgFramework.Utility;

namespace SgFramework.Language
{
    public static class LanguageManager
    {
        public static readonly Dictionary<List<string>, LanguageConfig> Lang2Font =
            new Dictionary<List<string>, LanguageConfig>
            {
                {
                    new List<string>
                    {
                        "en"
                    },
                    new LanguageConfig
                    {
                        Name = "English",
                        FontKey = "en"
                    }
                },
                {
                    new List<string>
                    {
                        "cn",
                        "tw",
                    },
                    new LanguageConfig
                    {
                        Name = "简体中文",
                        FontKey = "cn"
                    }
                }
            };

        private static readonly Dictionary<string, Func<string, string>> ParseFunc =
            new Dictionary<string, Func<string, string>>();

        private static bool _init;

        public static LangType CurrentLang { get; set; }

        public static void Initialize()
        {
            if (!Table.LangTypeTable.TryGetById(SgUtility.GetLanguage(), out var type))
            {
                throw new InvalidDataException("本地化配置有误");
            }

            CurrentLang = type;
            if (_init)
            {
                return;
            }

            foreach (var lang in Table.LangTypeTable.DataList.Select(langType => langType.lang_type))
            {
                ParseFunc[lang] = key => Parse(key, lang, out var content) ? content : key;
            }

            _init = true;
        }

        private static bool Parse(string key, string langType, out string content)
        {
            content = key;
            if (!Table.LangTable.DataDict.TryGetValue(key, out var lang))
            {
                return false;
            }

            //此处需要根据Lang表的列扩展而扩展
            content = langType switch
            {
                "cn" => lang.cn,
                _ => content
            };
            return true;
        }

        public static string Get(string key)
        {
            return ParseFunc.TryGetValue(CurrentLang.lang_type, out var func) ? func?.Invoke(key) : key;
        }
    }
}