
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class FiveDayGoalTable
    {
        public List<FiveDayGoal> DataList { get; set; }
        public Dictionary<int, FiveDayGoal> DataDict { get; set; } = new Dictionary<int, FiveDayGoal>();

		public bool TryGetById(int id, out FiveDayGoal value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public FiveDayGoal GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static FiveDayGoalTable Parse(string json)
        {
            var t = new FiveDayGoalTable
            {
                DataList = JsonConvert.DeserializeObject<List<FiveDayGoal>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class FiveDayGoal {
		//id
		public int id {get;set;}
		//开放天数
		public int day {get;set;}
		//当前TAB
		public int tab {get;set;}
		//完成条件
		public TaskCondition condition {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//TAB描述
		public string tab_desc {get;set;}
		//任务描述
		public string trans_desc {get;set;}

	
	}
}
