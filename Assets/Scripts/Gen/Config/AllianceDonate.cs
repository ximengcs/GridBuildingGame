
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceDonateTable
    {
        public List<AllianceDonate> DataList { get; set; }
        public Dictionary<int, AllianceDonate> DataDict { get; set; } = new Dictionary<int, AllianceDonate>();

		public bool TryGetById(int id, out AllianceDonate value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceDonate GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceDonateTable Parse(string json)
        {
            var t = new AllianceDonateTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceDonate>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceDonate {
		//公会捐献类型
		public int id {get;set;}
		//单日可捐献次数
		public int num {get;set;}
		//捐赠消耗资源
		public CostStruct cost {get;set;}
		//产出奖励
		public RewardStruct reward {get;set;}
		//公会经验
		public AllianceRewardStruct alliance_reward {get;set;}

	
		public class CostStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

		public class RewardStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

		public class AllianceRewardStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

	}
}
