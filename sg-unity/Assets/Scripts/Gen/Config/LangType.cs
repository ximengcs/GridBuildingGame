
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class LangTypeTable
    {
        public List<LangType> DataList { get; set; }
        public Dictionary<int, LangType> DataDict { get; set; } = new Dictionary<int, LangType>();

		public bool TryGetById(int id, out LangType value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public LangType GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static LangTypeTable Parse(string json)
        {
            var t = new LangTypeTable
            {
                DataList = JsonConvert.DeserializeObject<List<LangType>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class LangType {
		//多语言枚举
		public int id {get;set;}
		//多语言类型名称
		public string lang_type_name {get;set;}
		//多语言类型英文名
		public string lang_type {get;set;}

	
	}
}
