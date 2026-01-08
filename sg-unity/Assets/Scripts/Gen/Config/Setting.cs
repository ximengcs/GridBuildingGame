
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class SettingTable
    {
        public List<Setting> DataList { get; set; }
        public Dictionary<int, Setting> DataDict { get; set; } = new Dictionary<int, Setting>();

		public bool TryGetById(int id, out Setting value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Setting GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static SettingTable Parse(string json)
        {
            var t = new SettingTable
            {
                DataList = JsonConvert.DeserializeObject<List<Setting>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Setting {
		//id
		public int id {get;set;}
		//文本
		public string setting_text {get;set;}
		//图片
		public string setting_icon {get;set;}
		//默认开启状态
		public bool is_open {get;set;}
		//是否显示
		public bool is_show {get;set;}

	
	}
}
