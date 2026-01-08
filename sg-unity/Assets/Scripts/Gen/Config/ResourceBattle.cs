
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ResourceBattleTable
    {
        public List<ResourceBattle> DataList { get; set; }
        public Dictionary<int, ResourceBattle> DataDict { get; set; } = new Dictionary<int, ResourceBattle>();

		public bool TryGetById(int id, out ResourceBattle value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public ResourceBattle GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ResourceBattleTable Parse(string json)
        {
            var t = new ResourceBattleTable
            {
                DataList = JsonConvert.DeserializeObject<List<ResourceBattle>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class ResourceBattle {
		//战斗特效id
		public int id {get;set;}
		//文件名
		public string prefabName {get;set;}
		//资源路径（拉公式自动生成）
		public string path {get;set;}
		//包名
		public string bundle {get;set;}
		//使用通用帧动画
		public string sprFrameName {get;set;}
		//图集名
		public string atlasName {get;set;}
		//特效弱化标记
		public int effectWeak {get;set;}

	
	}
}
