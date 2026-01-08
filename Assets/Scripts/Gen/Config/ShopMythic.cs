
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ShopMythicTable
    {
        public List<ShopMythic> DataList { get; set; }
        public Dictionary<int, ShopMythic> DataDict { get; set; } = new Dictionary<int, ShopMythic>();

		public bool TryGetById(int id, out ShopMythic value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ShopMythic GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ShopMythicTable Parse(string json)
        {
            var t = new ShopMythicTable
            {
                DataList = JsonConvert.DeserializeObject<List<ShopMythic>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ShopMythic {
		//物品id
		public int id {get;set;}
		//物品备注
		public string name {get;set;}
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
