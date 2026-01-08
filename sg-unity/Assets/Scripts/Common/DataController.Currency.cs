using System;
using System.Collections.Generic;
using Pt;
using R3;
using SgFramework.Utility;

namespace Common
{
    public partial class DataController
    {
        private static readonly Dictionary<int, CurrencyAmount> CurrencyAmounts = new Dictionary<int, CurrencyAmount>();
        private static readonly Subject<CurrencyAmount> CurrencyUpdate = new Subject<CurrencyAmount>();

        public static long GetCurrency(int confId)
        {
            return CurrencyAmounts.TryGetValue(confId, out var currency) ? currency.Amount : 0;
        }

        public static IDisposable OnCurrency(Action<CurrencyAmount> action)
        {
            return CurrencyUpdate.Subscribe(action);
        }

        public static void SetCurrency(CurrencyAmount newCurrency)
        {
            if (newCurrency.ConfId == SgConst.CurrencyPlayerExp)
            {
                SetExp((int)newCurrency.Amount);
            }

            if (!CurrencyAmounts.TryGetValue(newCurrency.ConfId, out var currency))
            {
                Archive.Currency.Add(newCurrency);
                CurrencyAmounts.Add(newCurrency.ConfId, currency = newCurrency);
            }
            else
            {
                currency.Amount = newCurrency.Amount;
            }

            CurrencyUpdate.OnNext(currency);
        }
    }
}