using System;
using R3;
using TMPro;

namespace SgFramework.Utility
{
    
    public static partial class ClassExtern
    {
        public static IDisposable SubscribeToText(this Observable<string> source, TMP_Text text)
        {
            return source.Subscribe(text, static (x, t) => t.text = x);
        }

        public static IDisposable SubscribeToText<T>(this Observable<T> source, TMP_Text text)
        {
            return source.Subscribe(text, static (x, t) => t.text = x.ToString());
        }

        public static IDisposable SubscribeToText<T>(this Observable<T> source, TMP_Text text, Func<T, string> selector)
        {
            return source.Subscribe((text, selector), static (x, state) => state.text.text = state.selector(x));
        }
    }
}