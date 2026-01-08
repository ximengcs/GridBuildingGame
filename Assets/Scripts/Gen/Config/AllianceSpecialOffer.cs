
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceSpecialOfferTable
    {
        public List<AllianceSpecialOffer> DataList { get; set; }
        public Dictionary<int, AllianceSpecialOffer> DataDict { get; set; } = new Dictionary<int, AllianceSpecialOffer>();

		public bool TryGetById(int id, out AllianceSpecialOffer value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceSpecialOffer GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceSpecialOfferTable Parse(string json)
        {
            var t = new AllianceSpecialOfferTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceSpecialOffer>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceSpecialOffer {
		//砍价商品序号id
		public int id {get;set;}
		//商品内容
		public RewardStruct reward {get;set;}
		//初始售价
		public BuyCostInitialStruct buy_cost_initial {get;set;}
		//单次可砍灵玉下限
		public int cost_cut_min {get;set;}
		//单次可砍灵玉上限
		public int cost_cut_max {get;set;}

	
		public class RewardStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

		public class BuyCostInitialStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

	}
}
