
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class TaskMainTable
    {
        public List<TaskMain> DataList { get; set; }
        public Dictionary<int, TaskMain> DataDict { get; set; } = new Dictionary<int, TaskMain>();

		public bool TryGetById(int id, out TaskMain value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public TaskMain GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static TaskMainTable Parse(string json)
        {
            var t = new TaskMainTable
            {
                DataList = JsonConvert.DeserializeObject<List<TaskMain>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class TaskMain {
		//主线任务id
		public int id {get;set;}
		//下个任务id
		public int next_task_id {get;set;}
		//完成条件
		public TaskCondition condition {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//主线任务多语言
		public string task_desc {get;set;}
		//任务完成进度最大值
		public int task_complete_num_max {get;set;}
		//跳转ID
		public int jump_id {get;set;}

	
	}
}
