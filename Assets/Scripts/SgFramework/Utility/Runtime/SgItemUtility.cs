using System;
using Common;
using Config;

namespace SgFramework.Utility
{
    public static class SgItemUtility
    {
        /// <summary>
        /// 品质背景图片
        /// </summary>
        public static readonly string[] QualityImg =
        {
            "lobby_hero_avatar_bg_0", "lobby_hero_avatar_bg_1", "lobby_hero_avatar_bg_2", "lobby_hero_avatar_bg_3",
            "lobby_hero_avatar_bg_4", "lobby_hero_avatar_bg_5"
        };

        /// <summary>
        /// item 资源所在目录
        /// </summary>
        public const string ResRootPath = "Assets/GameRes/Sprite/Item/";


        /// <summary>
        /// 获取物品信息的公共接口返回
        /// </summary>
        public class RewardInfo
        {
            public int Type;
            public int Id;

            /// <summary>
            /// 名字多语言
            /// </summary>
            public string NameLau;

            /// <summary>
            /// 描述多语言
            /// </summary>
            public string DescLau;

            public string Icon;
            public string Quality;

            /// <summary>
            /// 价格 [Reward类型，id，数量]
            /// </summary>
            public int[] Price;

            /// <summary>
            /// 跳转到获取途径，暂时没有实现
            /// </summary>
            public Action<int> GetWay;
        }

        /// <summary>
        /// 用物品表的 类型和 id 来获取物品信息
        /// </summary>
        /// <param name="type">物品类型</param>
        /// <param name="goodsId">物品表中的 id</param>
        /// <param name="info">输出信息</param>
        /// <returns></returns>
        public static bool TryGetRewardID(int type, int goodsId, out RewardInfo info)
        {
            if (type == Table.Global.RewardTypeItem)
            {
                return TryGetItemInfo(goodsId, out info);
            }

            if (type == Table.Global.RewardTypeCurrency)
            {
                return TryGetCurrencyInfo(goodsId, out info);
            }

            info = null;
            return false;
        }

        /// <summary>
        /// 用 item 表的 id 获取物品信息
        /// </summary>
        /// <param name="itemId">item 表中的id</param>
        /// <param name="info">输出的物品信息</param>
        /// <returns></returns>
        private static bool TryGetItemInfo(int itemId, out RewardInfo info)
        {
            var config = Table.ItemTable.GetById(itemId);
            if (config == null)
            {
                info = null;
                return false;
            }

            info = new RewardInfo()
            {
                Type = Table.Global.RewardTypeItem,
                Id = itemId,
                NameLau = config.item_name,
                DescLau = config.item_desc,
                Icon = ResRootPath + config.icon,
                Quality = ResRootPath + QualityImg[config.color],
                Price = config.buy_use == null
                    ? null
                    : new[] { config.buy_use.type, config.buy_use.confId, config.buy_use.num },
            };
            return true;
        }

        /// <summary>
        /// 用 item 表的 id 获取物品信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="info">输出的物品信息</param>
        /// <returns></returns>
        private static bool TryGetCurrencyInfo(int goodsId, out RewardInfo info)
        {
            var config = Table.CurrencyTable.GetById(goodsId);
            if (config == null)
            {
                info = null;
                return false;
            }

            info = new RewardInfo()
            {
                Type = Table.Global.RewardTypeCurrency,
                Id = goodsId,
                NameLau = config.currency_name,
                DescLau = config.currency_desc,
                Icon = ResRootPath + config.currency_icon,
                Quality = ResRootPath + QualityImg[config.color],
                Price = new[] { Table.Global.RewardTypeCurrency, goodsId, 1 },
            };
            return true;
        }
    }
}