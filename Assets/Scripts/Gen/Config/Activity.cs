
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ActivityTable
    {
        public List<Activity> DataList { get; set; }
        public Dictionary<int, Activity> DataDict { get; set; } = new Dictionary<int, Activity>();

		public bool TryGetById(int id, out Activity value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Activity GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ActivityTable Parse(string json)
        {
            var t = new ActivityTable
            {
                DataList = JsonConvert.DeserializeObject<List<Activity>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Activity {
		//ID序号
		public int id {get;set;}
		//名称
		public string name {get;set;}
		//解锁等级
		public int open_level {get;set;}
		//是否显示
		public bool is_show {get;set;}
		//开启时间条件
		public List<ConditionStruct> condition {get;set;}
		//每期持续时间（秒）
		public int periods_last_time {get;set;}
		//每期间隔时间（秒）
		public int periods_between_time {get;set;}
		//循环期数
		public int periods_num {get;set;}
		//未领取的奖励发送邮件id
		public int mail_id {get;set;}
		//排序
		public int order {get;set;}
		//名称
		public string activity_name {get;set;}
		//banner页活动描述
		public string banner_desc {get;set;}
		//活动规则（感叹号里的）
		public string rule {get;set;}
		//主Banner
		public string activity_banner {get;set;}
		//子Banner
		public string extra_banner {get;set;}
		//特效
		public string effect {get;set;}

	
		public class ConditionStruct{
			public int type {get;set;}
			public int time {get;set;}
		}

	}
}
