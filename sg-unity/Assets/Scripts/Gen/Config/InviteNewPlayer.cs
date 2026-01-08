
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class InviteNewPlayerTable
    {
        public List<InviteNewPlayer> DataList { get; set; }
        public Dictionary<int, InviteNewPlayer> DataDict { get; set; } = new Dictionary<int, InviteNewPlayer>();

		public bool TryGetById(int id, out InviteNewPlayer value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public InviteNewPlayer GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static InviteNewPlayerTable Parse(string json)
        {
            var t = new InviteNewPlayerTable
            {
                DataList = JsonConvert.DeserializeObject<List<InviteNewPlayer>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class InviteNewPlayer {
		//任务id
		public int id {get;set;}
		//完成条件
		public TaskCondition condition {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//任务多语言
		public string task_desc {get;set;}

	
	}
}
