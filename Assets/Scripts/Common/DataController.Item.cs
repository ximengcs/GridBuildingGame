using System.Collections.Generic;
using Config;
using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.Net;
using SgFramework.RedPoint;
using Item = Pt.Item;

namespace Common
{
    public partial class DataController
    {
        public static readonly Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public static readonly Subject<Item> ItemUpdate = new Subject<Item>();


        private static void InitItemData()
        {
            Items.Clear();
            foreach (var item in Archive.Items)
            {
                Items.Add(item.ConfId, item);
            }
        }

        public static long GetItem(int confId)
        {
            return Items.TryGetValue(confId, out var item) ? item.Amount : 0;
        }

        public static void SetItem(Item newItem)
        {
            if (!Items.TryGetValue(newItem.ConfId, out var item))
            {
                Archive.Items.Add(newItem);
                Items.Add(newItem.ConfId, item = newItem);
            }
            else
            {
                item.Amount = newItem.Amount;
            }

            ItemUpdate.OnNext(item);
        }

        public static void SetItemRed(int confId, bool red)
        {
            var config = Table.ItemTable.GetById(confId);
            if (config != null && config.in_warehouse != 0)
            {
                RedPointManager.Instance.FindNode($"bag/{config.in_warehouse}/item{confId}").SetValue(red ? 1 : 0);
            }
        }

        public static async UniTask<bool> ItemBuyMsg(Item[] item)
        {
            var msg = new ItemBuyMsg();
            foreach (var it in item)
            {
                msg.Items.Add(it);
            }

            var rsp = await NetManager.Shared.Request(msg);
            if (rsp is not Fail)
            {
                return false;
            }

            return true;
        }

        public static async UniTask<bool> ItemUseMsg(int confId, int amount)
        {
            var rsp = await NetManager.Shared.Request(new ItemUseMsg()
            {
                ConfId = confId,
                Amount = amount
            });
            if (rsp is not Fail)
            {
                return false;
            }

            return true;
        }

        public static async UniTask<bool> ItemSaleMsg(int confId, int amount)
        {
            var rsp = await NetManager.Shared.Request(new ItemSaleMsg()
            {
                ConfId = confId,
                Amount = amount
            });
            if (rsp is not Fail)
            {
                return false;
            }

            return true;
        }
    }
}