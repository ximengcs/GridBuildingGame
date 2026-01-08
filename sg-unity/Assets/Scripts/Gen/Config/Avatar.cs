
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AvatarTable
    {
        public List<Avatar> DataList { get; set; }
        public Dictionary<int, Avatar> DataDict { get; set; } = new Dictionary<int, Avatar>();

		public bool TryGetById(int id, out Avatar value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Avatar GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AvatarTable Parse(string json)
        {
            var t = new AvatarTable
            {
                DataList = JsonConvert.DeserializeObject<List<Avatar>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Avatar {
		//ID序列
		public int id {get;set;}
		//解锁类型
		public int unlock_type {get;set;}
		//完成条件
		public TaskCondition condition {get;set;}
		//名称
		public string avatar_name {get;set;}
		//获取途径描述
		public string avatar_desc {get;set;}
		//图片
		public string picture {get;set;}
		//排序
		public int order {get;set;}
		//动态头像
		public string dynamics_picture {get;set;}

	
	}
}
