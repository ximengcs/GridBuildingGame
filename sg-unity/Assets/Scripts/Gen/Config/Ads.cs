
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AdsTable
    {
        public List<Ads> DataList { get; set; }
        public Dictionary<int, Ads> DataDict { get; set; } = new Dictionary<int, Ads>();

		public bool TryGetById(int id, out Ads value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Ads GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AdsTable Parse(string json)
        {
            var t = new AdsTable
            {
                DataList = JsonConvert.DeserializeObject<List<Ads>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Ads {
		//id不能修改
		public int id {get;set;}
		//每日广告次数（和广告cd互斥，两个只能配置其一，若两个都配置了数据程序以cd为准）
		public int ads_times {get;set;}

	
	}
}
