
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class GiftTabsTable
    {
        public List<GiftTabs> DataList { get; set; }
        public Dictionary<int, GiftTabs> DataDict { get; set; } = new Dictionary<int, GiftTabs>();

		public bool TryGetById(int id, out GiftTabs value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public GiftTabs GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static GiftTabsTable Parse(string json)
        {
            var t = new GiftTabsTable
            {
                DataList = JsonConvert.DeserializeObject<List<GiftTabs>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class GiftTabs {
		//页签类型id
		public int id {get;set;}
		//主页签类型
		public int tab_type {get;set;}
		//子页签名称多语言
		public string tab_name {get;set;}
		//Banner
		public string tab_banner {get;set;}
		//图标
		public string tab_icon {get;set;}
		//排序
		public int order {get;set;}

	
	}
}
