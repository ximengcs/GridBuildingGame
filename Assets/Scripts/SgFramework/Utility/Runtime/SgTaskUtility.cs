using System;
using System.Collections.Generic;
using Config;
using Pt;
using SgFramework.Language;

namespace SgFramework.Utility
{
    public static class SgTaskUtility
    {
        /// <summary>
        /// 定义任务条件的具体条件描述的文本生成方式
        /// </summary>
        private static readonly Dictionary<int, Func<int, string>> TaskParaConverter =
            new Dictionary<int, Func<int, string>>
            {
                {
                    3009,
                    id => Table.CurrencyTable.TryGetById(id, out var config)
                        ? LanguageManager.Get(config.currency_name)
                        : default
                }
            };

        public static string GetTaskDesc(string key, TaskCondition condition)
        {
            var content = LanguageManager.Get(key);
            var para1 = string.Empty;
            if (condition.para1 != 0 && TaskParaConverter.TryGetValue(condition.type, out var func))
            {
                para1 = func.Invoke(condition.para1);
            }

            return string.Format(content, para1, condition.para2);
        }

        public static string GetAchievementDesc(TaskAchievement config, Task bindData)
        {
            var content = LanguageManager.Get(config.achievement_desc);
            var para1 = string.Empty;
            var condition = config.achievement_type;
            if (condition.para1 != 0 && TaskParaConverter.TryGetValue(condition.type, out var func))
            {
                para1 = func.Invoke(condition.para1);
            }

            var para2 = bindData.FinishTimes < config.achievement_para2.Count
                ? $"{config.achievement_para2[bindData.FinishTimes]}"
                : $"{config.achievement_para2[^1]}";

            return string.Format(content, para1, para2);
        }
    }
}