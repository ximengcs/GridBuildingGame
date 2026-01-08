using System;
using Pt;
using R3;
using TMPro;

namespace Common
{
    public static partial class DataController
    {
        private static LoginRsp Archive { get; set; }
        public static bool ArchiveReady => Archive != null;

        public static void Initialize(LoginRsp archive)
        {
            Archive = archive;

            CurrencyAmounts.Clear();
            foreach (var currencyAmount in Archive.Currency)
            {
                CurrencyAmounts.Add(currencyAmount.ConfId, currencyAmount);
            }

            TriggerUserInfo();
            InitItemData();
        }

        public static void Dispose()
        {
            Archive = null;
            CurrencyAmounts.Clear();
        }
    }
}