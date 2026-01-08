
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class SystemNoticeTable
    {
        public List<SystemNotice> DataList { get; set; }
        public Dictionary<int, SystemNotice> DataDict { get; set; } = new Dictionary<int, SystemNotice>();

		public bool TryGetById(int id, out SystemNotice value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public SystemNotice GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static SystemNoticeTable Parse(string json)
        {
            var t = new SystemNoticeTable
            {
                DataList = JsonConvert.DeserializeObject<List<SystemNotice>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class SystemNotice {
		//通知id
		public int id {get;set;}
		//是否生效(1是0否）
		public int is_valid {get;set;}
		//再次循环所需秒数
		public int show_cd {get;set;}
		//内容
		public string content {get;set;}

	
	}
}
