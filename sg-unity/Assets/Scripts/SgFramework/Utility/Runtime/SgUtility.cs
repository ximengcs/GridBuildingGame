using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;
using Cysharp.Threading.Tasks;
using R3;
using SgFramework.Audio;
using SgFramework.Net;
using UnityEngine;
using UnityEngine.UI;

namespace SgFramework.Utility
{
    public static class SgUtility
    {
        private static readonly HashSet<string> ButtonGroupBusyRef = new HashSet<string>();

        public static IDisposable BindClick(this Button button, Action action, string clickSfx = "sfx_click_01")
        {
            var observable = button.OnClickAsObservable();
            return observable.Subscribe(_ =>
            {
                AudioManager.Instance.PlaySfx(clickSfx);
                action?.Invoke();
            });
        }

        public static IDisposable BindClick(this Button button, Func<UniTask> action, string group = default,
            string clickSfx = "sfx_click_01")
        {
            var observable = button.OnClickAsObservable();
            return observable.Subscribe(OnNext);

            async void OnNext(Unit _)
            {
                AudioManager.Instance.PlaySfx(clickSfx);
                if (ButtonGroupBusyRef.Contains(group))
                {
                    return;
                }

                try
                {
                    ButtonGroupBusyRef.Add(group);
                    Debug.Log("busy true");
                    await action();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                finally
                {
                    ButtonGroupBusyRef.Remove(group);
                    Debug.Log("busy false");
                }
            }
        }

        public static long Now => NetManager.Shared.TimeNow;

        public static string ExpireString(long expireTime)
        {
            var expire = expireTime - Now;
            if (expire > 86400)
            {
                return $"{expire / 86400} days";
            }

            if (expire > 3600)
            {
                return $"{expire / 3600} hours";
            }

            if (expire > 60)
            {
                return $"{expire / 60} minutes";
            }


            if (expire > 0)
            {
                return $"{expire} seconds";
            }

            return "Expired";
        }

        public static int GetSystemLanguage()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.ChineseSimplified => 1,
                SystemLanguage.Chinese or SystemLanguage.ChineseTraditional => 2,
                SystemLanguage.English => 3,
                _ => 3
            };
        }

        public static int GetLanguage()
        {
            return LocalStorage.GetInt("system_language", GetSystemLanguage());
        }

        public static void SetLanguage(int lanId)
        {
            LocalStorage.SetInt("system_language", lanId);
        }

        // 正则表达式匹配富文本标签
        private static readonly Regex RichTextTagRegex = new Regex(@"<[^>]*>", RegexOptions.Compiled);

        /// <summary>
        /// 移除字符串中的富文本标签
        /// </summary>
        /// <param name="input">包含富文本标签的字符串</param>
        /// <returns>移除标签后的字符串</returns>
        public static string RemoveRichTextTags(string input)
        {
            return string.IsNullOrEmpty(input)
                ? input
                : RichTextTagRegex.Replace(input, string.Empty);
        }
    }
}