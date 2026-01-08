
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ActivityEntranceTable
    {
        public List<ActivityEntrance> DataList { get; set; }
        public Dictionary<int, ActivityEntrance> DataDict { get; set; } = new Dictionary<int, ActivityEntrance>();

		public bool TryGetById(int id, out ActivityEntrance value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ActivityEntrance GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ActivityEntranceTable Parse(string json)
        {
            var t = new ActivityEntranceTable
            {
                DataList = JsonConvert.DeserializeObject<List<ActivityEntrance>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ActivityEntrance {
		//ID
		public int id {get;set;}
		//名称
		public string name {get;set;}
		//主界面图标
		public string static_entrance_path {get;set;}
		//位置
		public int position {get;set;}
		//排序
		public int order {get;set;}
		//组件类型
		public int module_type {get;set;}
		//名称
		public string entrance_name {get;set;}
		//动效路径
		public string anim_entrance_path {get;set;}

	
	}
}
