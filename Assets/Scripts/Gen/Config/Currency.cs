
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class CurrencyTable
    {
        public List<Currency> DataList { get; set; }
        public Dictionary<int, Currency> DataDict { get; set; } = new Dictionary<int, Currency>();

		public bool TryGetById(int id, out Currency value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Currency GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static CurrencyTable Parse(string json)
        {
            var t = new CurrencyTable
            {
                DataList = JsonConvert.DeserializeObject<List<Currency>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Currency {
		//资源ID
		public int id {get;set;}
		//品质
		public int color {get;set;}
		//资源名称
		public string currency_name {get;set;}
		//资源描述
		public string currency_desc {get;set;}
		//图片
		public string currency_icon {get;set;}

	
	}
}
