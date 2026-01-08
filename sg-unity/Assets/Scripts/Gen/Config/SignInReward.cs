
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class SignInRewardTable
    {
        public List<SignInReward> DataList { get; set; }
        public Dictionary<int, SignInReward> DataDict { get; set; } = new Dictionary<int, SignInReward>();

		public bool TryGetById(int id, out SignInReward value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public SignInReward GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static SignInRewardTable Parse(string json)
        {
            var t = new SignInRewardTable
            {
                DataList = JsonConvert.DeserializeObject<List<SignInReward>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class SignInReward {
		//自增id
		public int id {get;set;}
		//活动id
		public int activity_id {get;set;}
		//天数排序
		public int order {get;set;}
		//首次奖励内容
		public List<Reward> reward_first {get;set;}
		//常规奖励内容
		public List<Reward> reward {get;set;}
		//天数奖励品质
		public List<int> color {get;set;}
		//补签消耗
		public Reward sign_cost {get;set;}

	
	}
}
