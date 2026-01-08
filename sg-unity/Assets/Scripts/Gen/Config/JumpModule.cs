
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class JumpModuleTable
    {
        public List<JumpModule> DataList { get; set; }
        public Dictionary<int, JumpModule> DataDict { get; set; } = new Dictionary<int, JumpModule>();

		public bool TryGetById(int id, out JumpModule value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public JumpModule GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static JumpModuleTable Parse(string json)
        {
            var t = new JumpModuleTable
            {
                DataList = JsonConvert.DeserializeObject<List<JumpModule>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class JumpModule {
		//跳转id
		public int id {get;set;}
		//跳转功能名称
		public string jump_name {get;set;}
		//跳转路径
		public string jump_line {get;set;}

	
	}
}
