
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AvatarFrameTable
    {
        public List<AvatarFrame> DataList { get; set; }
        public Dictionary<int, AvatarFrame> DataDict { get; set; } = new Dictionary<int, AvatarFrame>();

		public bool TryGetById(int id, out AvatarFrame value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AvatarFrame GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AvatarFrameTable Parse(string json)
        {
            var t = new AvatarFrameTable
            {
                DataList = JsonConvert.DeserializeObject<List<AvatarFrame>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AvatarFrame {
		//ID序列
		public int id {get;set;}
		//解锁类型
		public int unlock_type {get;set;}
		//完成条件
		public TaskCondition condition {get;set;}
		//名称
		public string avatarframe_name {get;set;}
		//获取途径描述
		public string avatarframe_desc {get;set;}
		//图片
		public string picture {get;set;}
		//排序
		public int order {get;set;}
		//动态头像框图片
		public string dynamics_picture {get;set;}
		//头像框名次标签图片
		public string picture_mark {get;set;}
		//头像框未获取是否展示（1展示0不展示）
		public int is_show {get;set;}

	
	}
}
