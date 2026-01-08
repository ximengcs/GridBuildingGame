
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class WarehouseTypeTable
    {
        public List<WarehouseType> DataList { get; set; }
        public Dictionary<int, WarehouseType> DataDict { get; set; } = new Dictionary<int, WarehouseType>();

		public bool TryGetById(int id, out WarehouseType value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public WarehouseType GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static WarehouseTypeTable Parse(string json)
        {
            var t = new WarehouseTypeTable
            {
                DataList = JsonConvert.DeserializeObject<List<WarehouseType>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class WarehouseType {
		//序号ID
		public int id {get;set;}
		//分类名称
		public string name {get;set;}
		//多语言名称ID
		public string warehouse_type {get;set;}

	
	}
}
