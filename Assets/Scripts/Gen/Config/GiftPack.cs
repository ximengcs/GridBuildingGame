
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class GiftPackTable
    {
        public List<GiftPack> DataList { get; set; }
        public Dictionary<int, GiftPack> DataDict { get; set; } = new Dictionary<int, GiftPack>();

		public bool TryGetById(int id, out GiftPack value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public GiftPack GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static GiftPackTable Parse(string json)
        {
            var t = new GiftPackTable
            {
                DataList = JsonConvert.DeserializeObject<List<GiftPack>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class GiftPack {
		//礼包id
		public int id {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//奖励位置
		public int gift_position {get;set;}
		//IOS商品ID
		public string ios_pay_rmb {get;set;}

	
	}
}
