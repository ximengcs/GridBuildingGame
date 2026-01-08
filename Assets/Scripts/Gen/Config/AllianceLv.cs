
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class AllianceLvTable
    {
        public List<AllianceLv> DataList { get; set; }
        public Dictionary<int, AllianceLv> DataDict { get; set; } = new Dictionary<int, AllianceLv>();

		public bool TryGetById(int id, out AllianceLv value)
        {
            return DataDict.TryGetValue(id, out value);
        }
			
    	public AllianceLv GetById(int id)
        {
	        return DataDict.TryGetValue(id, out var value) ? value : default;
        }

        public static AllianceLvTable Parse(string json)
        {
            var t = new AllianceLvTable
            {
                DataList = JsonConvert.DeserializeObject<List<AllianceLv>>(json)
            };

            foreach (var item in t.DataList)
            {
                t.DataDict[item.id] = item;
            }
            return t;
        }
    }
	

	public class AllianceLv {
		//公会等级id
		public int id {get;set;}
		//等级所需累积公会经验值(113)
		public int lv_alliance_exp_total {get;set;}
		//公会可容纳人数
		public int player_num_max {get;set;}
		//公会科技等级上限
		public int science_lv_max {get;set;}
		//该等级是否解锁商店新商品
		public int is_new_good {get;set;}
		//副会长人数上限
		public int vicepresident_num_max {get;set;}
		//可砍价人数每日上限
		public int specialoffer_num_max {get;set;}
		//等级解锁功能预览-多语言
		public string lv_preview_text {get;set;}

	
	}
}
