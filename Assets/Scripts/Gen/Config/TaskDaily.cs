
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class TaskDailyTable
    {
        public List<TaskDaily> DataList { get; set; }
        public Dictionary<int, TaskDaily> DataDict { get; set; } = new Dictionary<int, TaskDaily>();

		public bool TryGetById(int id, out TaskDaily value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public TaskDaily GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static TaskDailyTable Parse(string json)
        {
            var t = new TaskDailyTable
            {
                DataList = JsonConvert.DeserializeObject<List<TaskDaily>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class TaskDaily {
		//每日任务id
		public int id {get;set;}
		//完成条件
		public TaskCondition condition {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//每日任务多语言
		public string task_desc {get;set;}
		//显示排序
		public int order {get;set;}
		//跳转ID
		public int jump_id {get;set;}

	
	}
}
