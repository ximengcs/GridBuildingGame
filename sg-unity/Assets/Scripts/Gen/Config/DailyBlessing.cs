
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class DailyBlessingTable
    {
        public List<DailyBlessing> DataList { get; set; }
        public Dictionary<int, DailyBlessing> DataDict { get; set; } = new Dictionary<int, DailyBlessing>();

		public bool TryGetById(int id, out DailyBlessing value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public DailyBlessing GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static DailyBlessingTable Parse(string json)
        {
            var t = new DailyBlessingTable
            {
                DataList = JsonConvert.DeserializeObject<List<DailyBlessing>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class DailyBlessing {
		//ID
		public int id {get;set;}
		//奖励类型
		public int type {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//效果描述
		public string attr_desc {get;set;}
		//属性类型
		public string attr_type {get;set;}
		//属性值
		public float attr_para {get;set;}
		//权重
		public int weight {get;set;}

	
	}
}
