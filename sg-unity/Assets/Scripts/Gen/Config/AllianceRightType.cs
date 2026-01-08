
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceRightTypeTable
    {
        public List<AllianceRightType> DataList { get; set; }
        public Dictionary<int, AllianceRightType> DataDict { get; set; } = new Dictionary<int, AllianceRightType>();

		public bool TryGetById(int id, out AllianceRightType value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceRightType GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceRightTypeTable Parse(string json)
        {
            var t = new AllianceRightTypeTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceRightType>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceRightType {
		//权限枚举ID
		public int id {get;set;}
		//成员类型描述
		public string permission {get;set;}
		//枚举枚举多语言
		public string right_name {get;set;}

	
	}
}
