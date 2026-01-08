
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ShopBaseTable
    {
        public List<ShopBase> DataList { get; set; }
        public Dictionary<int, ShopBase> DataDict { get; set; } = new Dictionary<int, ShopBase>();

		public bool TryGetById(int id, out ShopBase value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ShopBase GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ShopBaseTable Parse(string json)
        {
            var t = new ShopBaseTable
            {
                DataList = JsonConvert.DeserializeObject<List<ShopBase>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ShopBase {
		//id
		public int id {get;set;}
		//【消耗】每日商店重置消耗金币
		public int daily_shop_reset_cost {get;set;}
		//每日商店-每日金币重置次数
		public int daily_shop_cost_reset_count {get;set;}
		//每日商店-每日重置广告id
		public int daily_shop_reset_ads_id {get;set;}
		//每日商店随机商品数量
		public int daily_shop_max_count {get;set;}
		//每日商店折扣百分比
		public List<int> daily_discount {get;set;}
		//每日商店折扣权重
		public List<int> daily_discount_weight {get;set;}

	
	}
}
