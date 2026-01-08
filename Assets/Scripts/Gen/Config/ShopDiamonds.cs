
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ShopDiamondsTable
    {
        public List<ShopDiamonds> DataList { get; set; }
        public Dictionary<int, ShopDiamonds> DataDict { get; set; } = new Dictionary<int, ShopDiamonds>();

		public bool TryGetById(int id, out ShopDiamonds value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ShopDiamonds GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ShopDiamondsTable Parse(string json)
        {
            var t = new ShopDiamondsTable
            {
                DataList = JsonConvert.DeserializeObject<List<ShopDiamonds>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ShopDiamonds {
		//物品id
		public int id {get;set;}
		//物品备注
		public string name {get;set;}
		//奖励内容
		public Reward reward {get;set;}
		//首次购买额外奖励
		public Reward reward_first {get;set;}
		//关联payid
		public int pay_id {get;set;}
		//商品名称多语言
		public string goods_name {get;set;}
		//广告语多语言
		public string goods_slogan {get;set;}
		//icon
		public string goods_icon {get;set;}

	
	}
}
