
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class GameChannelTable
    {
        public List<GameChannel> DataList { get; set; }
        public Dictionary<int, GameChannel> DataDict { get; set; } = new Dictionary<int, GameChannel>();

		public bool TryGetById(int id, out GameChannel value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public GameChannel GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static GameChannelTable Parse(string json)
        {
            var t = new GameChannelTable
            {
                DataList = JsonConvert.DeserializeObject<List<GameChannel>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class GameChannel {
		//渠道id
		public int id {get;set;}
		//渠道名称
		public string name {get;set;}
		//下载地址
		public string download_address {get;set;}

	
	}
}
