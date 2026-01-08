
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceChallengeRewardTable
    {
        public List<AllianceChallengeReward> DataList { get; set; }
        public Dictionary<int, AllianceChallengeReward> DataDict { get; set; } = new Dictionary<int, AllianceChallengeReward>();

		public bool TryGetById(int id, out AllianceChallengeReward value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceChallengeReward GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceChallengeRewardTable Parse(string json)
        {
            var t = new AllianceChallengeRewardTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceChallengeReward>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceChallengeReward {
		//奖励档位id
		public int id {get;set;}
		//档位积分需求（所有人共同积分）
		public int challenge_score {get;set;}
		//奖励内容
		public List<RewardStruct> reward {get;set;}

	
		public class RewardStruct{
			public int type {get;set;}
			public int confId {get;set;}
			public int num {get;set;}
		}

	}
}
