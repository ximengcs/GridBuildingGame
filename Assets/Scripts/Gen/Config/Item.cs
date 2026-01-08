
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class ItemTable
    {
        public List<Item> DataList { get; set; }
        public Dictionary<int, Item> DataDict { get; set; } = new Dictionary<int, Item>();

		public bool TryGetById(int id, out Item value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public Item GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static ItemTable Parse(string json)
        {
            var t = new ItemTable
            {
                DataList = JsonConvert.DeserializeObject<List<Item>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class Item {
		//物品ID
		public int id {get;set;}
		//名称
		public string name {get;set;}
		//道具类型
		public int type {get;set;}
		//道具子类型
		public int sub_type {get;set;}
		//所属仓库
		public int in_warehouse {get;set;}
		//品质
		public int color {get;set;}
		//显示等级
		public int show_level {get;set;}
		//数值
		public int value {get;set;}
		//扩展数值
		public List<int> option {get;set;}
		//奖励内容
		public List<Reward> reward {get;set;}
		//随机奖励内容
		public List<Randreward> rand_reward {get;set;}
		//物品名称
		public string item_name {get;set;}
		//物品描述
		public string item_desc {get;set;}
		//图集
		public string package {get;set;}
		//图片
		public string icon {get;set;}
		//动态图片
		public string dynamics_picture {get;set;}
		//道具标签图片(名次）
		public string picture_mark {get;set;}
		//是否有边框特效(1为有）
		public int show_effects {get;set;}
		//动态路径(动态要配
		public string dynamics_route {get;set;}
		//动态图片bundle（动态要配
		public string dynamics_bundle {get;set;}
		//购买价格
		public Reward buy_use {get;set;}
		//售卖价格
		public Reward sell_value {get;set;}
		//排序权重（小的优先级高）
		public int order_id {get;set;}

	
	}
}
