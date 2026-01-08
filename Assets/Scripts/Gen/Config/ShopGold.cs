
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ShopGoldTable
    {
        public List<ShopGold> DataList { get; set; }
        public Dictionary<int, ShopGold> DataDict { get; set; } = new Dictionary<int, ShopGold>();

		public bool TryGetById(int id, out ShopGold value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ShopGold GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ShopGoldTable Parse(string json)
        {
            var t = new ShopGoldTable
            {
                DataList = JsonConvert.DeserializeObject<List<ShopGold>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ShopGold {
		//物品id
		public int id {get;set;}
		//物品备注
		public string name {get;set;}
		//免费次数
		public int free_times {get;set;}
		//购买广告id
		public int ads_id {get;set;}
		//货币id
		public int cost_id {get;set;}
		//货币价格
		public int cost_price {get;set;}
		//每日购买次数
		public int buy_limit {get;set;}
		//奖励内容
		public Reward reward {get;set;}
		//商品名称多语言
		public string goods_name {get;set;}
		//icon
		public string goods_icon {get;set;}

	
	}
}
