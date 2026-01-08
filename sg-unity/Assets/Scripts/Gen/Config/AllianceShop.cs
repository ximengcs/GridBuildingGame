
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceShopTable
    {
        public List<AllianceShop> DataList { get; set; }
        public Dictionary<int, AllianceShop> DataDict { get; set; } = new Dictionary<int, AllianceShop>();

		public bool TryGetById(int id, out AllianceShop value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceShop GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceShopTable Parse(string json)
        {
            var t = new AllianceShopTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceShop>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceShop {
		//物品id
		public int id {get;set;}
		//物品备注
		public string name {get;set;}
		//成员可购买限制类型(1每日2每周3不限购)
		public int limit_type {get;set;}
		//可购买次数上限
		public int buy_limit {get;set;}
		//购买消耗
		public BuyCostStruct buy_cost {get;set;}
		//奖励内容
		public RewardStruct reward {get;set;}
		//商品解锁所需公会等级
		public int need_alliance_lv {get;set;}
		//商品展示序号(越小越靠前)
		public int order {get;set;}

	
		public class BuyCostStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

		public class RewardStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

	}
}
