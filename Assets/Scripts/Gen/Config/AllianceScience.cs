
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceScienceTable
    {
        public List<AllianceScience> DataList { get; set; }
        public Dictionary<int, AllianceScience> DataDict { get; set; } = new Dictionary<int, AllianceScience>();

		public bool TryGetById(int id, out AllianceScience value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceScience GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceScienceTable Parse(string json)
        {
            var t = new AllianceScienceTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceScience>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceScience {
		//公会科技id(类型*10000+科技等级)
		public int id {get;set;}
		//科技类型
		public int science_type {get;set;}
		//科技等级
		public int science_lv {get;set;}
		//逻辑ID
		public int alone {get;set;}
		//行为特殊参数
		public string special_para {get;set;}
		//单级属性值%
		public float num {get;set;}
		//当前等级属性值%
		public float num_total {get;set;}
		//升至此级单次消耗
		public BuyCostStruct buy_cost {get;set;}
		//升至此级所需公会等级
		public int need_alliance_lv {get;set;}
		//图标
		public string icon {get;set;}

	
		public class BuyCostStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

	}
}
