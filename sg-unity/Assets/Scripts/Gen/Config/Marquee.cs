
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class MarqueeTable
    {
        public List<Marquee> DataList { get; set; }
        public Dictionary<int, Marquee> DataDict { get; set; } = new Dictionary<int, Marquee>();

		public bool TryGetById(int id, out Marquee value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Marquee GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static MarqueeTable Parse(string json)
        {
            var t = new MarqueeTable
            {
                DataList = JsonConvert.DeserializeObject<List<Marquee>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Marquee {
		//跑马灯id
		public int id {get;set;}
		//展示次数
		public int ShowTimes {get;set;}
		//内容
		public string content {get;set;}

	
	}
}
