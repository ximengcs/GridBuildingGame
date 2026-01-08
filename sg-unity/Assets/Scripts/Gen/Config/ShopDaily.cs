
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ShopDailyTable
    {
        public List<ShopDaily> DataList { get; set; }
        public Dictionary<int, ShopDaily> DataDict { get; set; } = new Dictionary<int, ShopDaily>();

		public bool TryGetById(int id, out ShopDaily value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ShopDaily GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ShopDailyTable Parse(string json)
        {
            var t = new ShopDailyTable
            {
                DataList = JsonConvert.DeserializeObject<List<ShopDaily>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ShopDaily {
		//物品id
		public int id {get;set;}
		//物品名称
		public string name {get;set;}
		//商品类型
		public int goods_type {get;set;}
		//货币id
		public int cost_id {get;set;}
		//货币价格
		public int cost_price {get;set;}
		//刷新权重
		public int goods_weight {get;set;}
		//奖励内容
		public Reward reward {get;set;}
		//招募池
		public int recruitpool_id {get;set;}
		//随机数量
		public int unit_num {get;set;}
		//icon
		public string goods_icon {get;set;}
		//免费次数
		public int free_times {get;set;}
		//购买广告id
		public int ads_id {get;set;}

	
	}
}
