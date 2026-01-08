
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceRightTable
    {
        public List<AllianceRight> DataList { get; set; }
        public Dictionary<int, AllianceRight> DataDict { get; set; } = new Dictionary<int, AllianceRight>();

		public bool TryGetById(int id, out AllianceRight value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceRight GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceRightTable Parse(string json)
        {
            var t = new AllianceRightTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceRight>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceRight {
		//成员类型
		public int id {get;set;}
		//成员类型多语言
		public string member_type_name {get;set;}
		//拥有权限
		public List<int> right_list {get;set;}

	
	}
}
