
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class PlayerLevelTable
    {
        public List<PlayerLevel> DataList { get; set; }
        public Dictionary<int, PlayerLevel> DataDict { get; set; } = new Dictionary<int, PlayerLevel>();

		public bool TryGetById(int id, out PlayerLevel value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public PlayerLevel GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static PlayerLevelTable Parse(string json)
        {
            var t = new PlayerLevelTable
            {
                DataList = JsonConvert.DeserializeObject<List<PlayerLevel>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class PlayerLevel {
		//等级id
		public int id {get;set;}
		//升至下一级所需单级经验
		public int need_exp {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}

	
	}
}
