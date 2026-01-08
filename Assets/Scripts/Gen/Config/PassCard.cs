
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class PassCardTable
    {
        public List<PassCard> DataList { get; set; }
        public Dictionary<int, PassCard> DataDict { get; set; } = new Dictionary<int, PassCard>();

		public bool TryGetById(int id, out PassCard value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public PassCard GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static PassCardTable Parse(string json)
        {
            var t = new PassCardTable
            {
                DataList = JsonConvert.DeserializeObject<List<PassCard>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class PassCard {
		//自增id
		public int id {get;set;}
		//活动id
		public int activity_id {get;set;}
		//条件类型
		public int type {get;set;}
		//条件参数
		public int condition {get;set;}
		//通行证等级
		public int level {get;set;}
		//奖励内容
		public List<Reward> reward_free {get;set;}
		//奖励内容
		public List<Reward> reward_pay {get;set;}

	
	}
}
