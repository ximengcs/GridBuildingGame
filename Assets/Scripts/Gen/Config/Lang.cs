
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class LangTable
    {
        public List<Lang> DataList { get; set; }
        public Dictionary<string, Lang> DataDict { get; set; } = new Dictionary<string, Lang>();

		public bool TryGetById(string id, out Lang value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Lang GetById(string id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static LangTable Parse(string json)
        {
            var t = new LangTable
            {
                DataList = JsonConvert.DeserializeObject<List<Lang>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Lang {
		//多语言id【系统名_(ui,tips,text)(_子项名,可缺省)_序号】
		public string id {get;set;}
		//简体中文
		public string cn {get;set;}

	
	}
}
