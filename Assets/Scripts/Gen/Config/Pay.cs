
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class PayTable
    {
        public List<Pay> DataList { get; set; }
        public Dictionary<int, Pay> DataDict { get; set; } = new Dictionary<int, Pay>();

		public bool TryGetById(int id, out Pay value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Pay GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static PayTable Parse(string json)
        {
            var t = new PayTable
            {
                DataList = JsonConvert.DeserializeObject<List<Pay>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Pay {
		//ID
		public int id {get;set;}
		//关联活动id
		public int activity_id {get;set;}
		//商店类型
		public int pay_type {get;set;}
		//人民币价格
		public int price_rmb {get;set;}
		//商品id（国服）
		public string product_id_china {get;set;}
		//性价比倍数
		public int efficient {get;set;}
		//IOS商品ID
		public string ios_pay_rmb {get;set;}

	
	}
}
