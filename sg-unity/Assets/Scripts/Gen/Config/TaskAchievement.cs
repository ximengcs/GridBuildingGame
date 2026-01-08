
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class TaskAchievementTable
    {
        public List<TaskAchievement> DataList { get; set; }
        public Dictionary<int, TaskAchievement> DataDict { get; set; } = new Dictionary<int, TaskAchievement>();

		public bool TryGetById(int id, out TaskAchievement value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public TaskAchievement GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static TaskAchievementTable Parse(string json)
        {
            var t = new TaskAchievementTable
            {
                DataList = JsonConvert.DeserializeObject<List<TaskAchievement>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class TaskAchievement {
		//成就任务id
		public int id {get;set;}
		//任务类型
		public TaskCondition achievement_type {get;set;}
		//任务参数2
		public List<int> achievement_para2 {get;set;}
		//奖励内容
		public List<Reward> achievement_reward {get;set;}
		//成就任务标题多语言
		public string title_name {get;set;}
		//成就任务描述多语言
		public string achievement_desc {get;set;}
		//排序
		public int order {get;set;}
		//跳转ID
		public int jump_id {get;set;}

	
	}
}
