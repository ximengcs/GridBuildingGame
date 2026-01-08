
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceFlagTable
    {
        public List<AllianceFlag> DataList { get; set; }
        public Dictionary<int, AllianceFlag> DataDict { get; set; } = new Dictionary<int, AllianceFlag>();

		public bool TryGetById(int id, out AllianceFlag value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceFlag GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceFlagTable Parse(string json)
        {
            var t = new AllianceFlagTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceFlag>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceFlag {
		//公会旗帜id
		public int id {get;set;}
		//旗帜图片ID
		public string pic {get;set;}

	
	}
}
