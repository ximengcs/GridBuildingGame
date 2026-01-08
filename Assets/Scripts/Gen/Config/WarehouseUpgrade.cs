
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class WarehouseUpgradeTable
    {
        public List<WarehouseUpgrade> DataList { get; set; }
        public Dictionary<int, WarehouseUpgrade> DataDict { get; set; } = new Dictionary<int, WarehouseUpgrade>();

		public bool TryGetById(int id, out WarehouseUpgrade value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public WarehouseUpgrade GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static WarehouseUpgradeTable Parse(string json)
        {
            var t = new WarehouseUpgradeTable
            {
                DataList = JsonConvert.DeserializeObject<List<WarehouseUpgrade>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class WarehouseUpgrade {
		//序号ID
		public int id {get;set;}
		//仓库类型
		public int warehouse_type {get;set;}
		//此级存储量
		public int storage_num {get;set;}
		//提升至下级消耗
		public List<Reward> upgrade_use {get;set;}

	
	}
}
