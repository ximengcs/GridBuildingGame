
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class MailTable
    {
        public List<Mail> DataList { get; set; }
        public Dictionary<int, Mail> DataDict { get; set; } = new Dictionary<int, Mail>();

		public bool TryGetById(int id, out Mail value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Mail GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static MailTable Parse(string json)
        {
            var t = new MailTable
            {
                DataList = JsonConvert.DeserializeObject<List<Mail>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Mail {
		//邮件id
		public int id {get;set;}
		//未领取过期时间（天）
		public int overdue {get;set;}
		//标题
		public string title {get;set;}
		//邮件文本
		public string content {get;set;}

	
	}
}
