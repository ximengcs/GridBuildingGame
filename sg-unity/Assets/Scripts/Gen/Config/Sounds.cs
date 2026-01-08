
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class SoundsTable
    {
        public List<Sounds> DataList { get; set; }
        public Dictionary<int, Sounds> DataDict { get; set; } = new Dictionary<int, Sounds>();

		public bool TryGetById(int id, out Sounds value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Sounds GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static SoundsTable Parse(string json)
        {
            var t = new SoundsTable
            {
                DataList = JsonConvert.DeserializeObject<List<Sounds>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Sounds {
		//音频ID
		public int id {get;set;}
		//包名
		public string bundle {get;set;}
		//资源路径
		public string path {get;set;}
		//次数限制
		public int frequency {get;set;}

	
	}
}
