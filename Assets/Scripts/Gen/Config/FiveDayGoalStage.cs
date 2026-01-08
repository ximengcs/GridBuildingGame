
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class FiveDayGoalStageTable
    {
        public List<FiveDayGoalStage> DataList { get; set; }
        public Dictionary<int, FiveDayGoalStage> DataDict { get; set; } = new Dictionary<int, FiveDayGoalStage>();

		public bool TryGetById(int id, out FiveDayGoalStage value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public FiveDayGoalStage GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static FiveDayGoalStageTable Parse(string json)
        {
            var t = new FiveDayGoalStageTable
            {
                DataList = JsonConvert.DeserializeObject<List<FiveDayGoalStage>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class FiveDayGoalStage {
		//id
		public int id {get;set;}
		//所需积分
		public int need_score {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//任务名称
		public string name {get;set;}
		//任务描述
		public string desc {get;set;}

	
	}
}
