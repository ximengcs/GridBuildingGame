
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class TaskDailyRewardTable
    {
        public List<TaskDailyReward> DataList { get; set; }
        public Dictionary<int, TaskDailyReward> DataDict { get; set; } = new Dictionary<int, TaskDailyReward>();

		public bool TryGetById(int id, out TaskDailyReward value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public TaskDailyReward GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static TaskDailyRewardTable Parse(string json)
        {
            var t = new TaskDailyRewardTable
            {
                DataList = JsonConvert.DeserializeObject<List<TaskDailyReward>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class TaskDailyReward {
		//每日任务档位id
		public int id {get;set;}
		//所需积分
		public int need_point {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}

	
	}
}
