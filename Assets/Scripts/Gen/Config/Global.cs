
/**
 * ！！自动导出，请不要修改
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Config{
	public class Global {
		//【体力】单次体力恢复时间（秒）
		public int StaminaCooltime {get;set;}
		//【体力】单次体力恢复数量
		public int StaminaChargeCount {get;set;}
		//【体力】体力值上限
		public int StaminaMaxCount {get;set;}
		//【体力】单局体力消耗
		public int StaminaGameCost {get;set;}
		//【消耗】修改昵称消耗宝石
		public int ChangeNickNameCost {get;set;}
		//昵称长度--最小值
		public int NicknameMinLength {get;set;}
		//昵称长度--最大值
		public int NicknameMaxLength {get;set;}
		//【奖励类型】Currency资源货币奖励
		public int RewardTypeCurrency {get;set;}
		//【奖励类型】Item道具奖励
		public int RewardTypeItem {get;set;}
		//【奖励类型】Crops作物奖励
		public int RewardTypeCrops {get;set;}
		//问题反馈字数限制
		public int FeedbackWordLimit {get;set;}
		//广告id
		public string AdsId {get;set;}
		//首充弹窗最低间隔时间（小时）
		public int FirstPayInterval {get;set;}
		//世界频道界面展示消息条数上限
		public int DisplayQuantity {get;set;}
		//世界频道表情，快捷语，发言公共CD
		public int SpeechCD {get;set;}
		//私聊消息条数上限
		public int PrivateChatLimit {get;set;}
		//世界频道开启所需玩家等级
		public int ChatActivationLevel {get;set;}
		//聊天输入框字符上限
		public int CharacterLimit {get;set;}
		//玩家打开世界频道时的停留最大显示条数（超过最大条数就只显示最大条数）
		public int ChatMaxCount {get;set;}
		//玩家每天第一次进入游戏世界频道显示消息条数
		public int ChatInitCount {get;set;}
		//世界频道发言需要等级
		public int SpeechLevel {get;set;}
		//添加好友需要等级
		public int AddLevel {get;set;}
		//推荐好友人数
		public int FriendReferNumLimit {get;set;}
		//好友申请列表人数限制
		public int ApplicationRestrictions {get;set;}
		//好友人数限制
		public int FriendLimit {get;set;}
		//黑名单人数限制
		public int BlacklistUpperLimit {get;set;}
		//推荐好友刷新时间（秒）
		public int FriendReferCD {get;set;}
		//加公会所需玩家等级
		public int AllianceGetInNeedLv {get;set;}
		//公会创建消耗内容
		public List<Reward> CreateAllianceSpend {get;set;}
		//公会捐献普通月卡增加灵玉捐献类型(3)次数
		public int AllianceDonateNumAdd {get;set;}
		//公会名称长度最大限制
		public int AllianceNameMaxLength {get;set;}
		//公会公告字数最大限制
		public int AllianceNoticeMaxLength {get;set;}
		//重复加公会冷却时间叠加秒数基数
		public int AllianceRepeatEnterCdBase {get;set;}
		//重新加公会冷却时间叠加秒数上限
		public int AllianceRepeatEnterCdMax {get;set;}
		//公会改名消耗灵玉数量
		public List<Reward> AllianceRename {get;set;}
		//公会挑战--每完成一个每日挑战获得档位积分
		public int AllianceChallengeScore {get;set;}
		//公会挑战--当日未领取奖励补发邮件id
		public int AllianceChallengeMail {get;set;}
		//公会申请列表数量限制
		public int AllianceListNum {get;set;}
		//公会--加公会时推荐公会条数
		public int AllianceJoinShowNum {get;set;}
		//公会动态事件--保存条数
		public int AllianceEventNum {get;set;}
		//公会权限--发布招募冷却时间（秒）
		public int AllianceRecruitCD {get;set;}
		//公会--会长自动转让所需时长（天）
		public int AllianceMakeOverTime {get;set;}
		//【体力】看广告奖励体力--每日次数
		public int StaminaAdsCount {get;set;}
		//【体力】看广告奖励体力--单次数量
		public int AdsStaminaGiveCount {get;set;}
		//【体力】宝石购买体力--单次数量
		public int PaidStaminaGiveCount {get;set;}
		//【消耗】购买体力单次消耗宝石
		public int BuyStaminaCost {get;set;}
		//【跑马灯】字幕默认滚动速度（X帧每秒）
		public int MarqueeSpeed {get;set;}
		//【跑马灯】关闭不再展示的小时数
		public int MarqueeCloseHours {get;set;}
		//【跑马灯】播放条目间隔秒数
		public float MarqueeIntervalSeconds {get;set;}

	
	}
}
