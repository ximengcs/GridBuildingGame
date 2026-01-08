
/**
 * ！！自动导出，请不要修改
 */
using System;
using Newtonsoft.Json;

namespace Config{
 
	public static partial class Table
	{
		public const int TotalCount = 48;
	
		public static ActivityTable ActivityTable  { get; private set; }

		public static ActivityEntranceTable ActivityEntranceTable  { get; private set; }

		public static AdsTable AdsTable  { get; private set; }

		public static AllianceFlagTable AllianceFlagTable  { get; private set; }

		public static AllianceLvTable AllianceLvTable  { get; private set; }

		public static AllianceScienceTable AllianceScienceTable  { get; private set; }

		public static AllianceRightTable AllianceRightTable  { get; private set; }

		public static AllianceRightTypeTable AllianceRightTypeTable  { get; private set; }

		public static AllianceDonateTable AllianceDonateTable  { get; private set; }

		public static AllianceShopTable AllianceShopTable  { get; private set; }

		public static AllianceSpecialOfferTable AllianceSpecialOfferTable  { get; private set; }

		public static AllianceChallengeRewardTable AllianceChallengeRewardTable  { get; private set; }

		public static AvatarTable AvatarTable  { get; private set; }

		public static AvatarFrameTable AvatarFrameTable  { get; private set; }

		public static CurrencyTable CurrencyTable  { get; private set; }

		public static DailyBlessingTable DailyBlessingTable  { get; private set; }

		public static FiveDayGoalTable FiveDayGoalTable  { get; private set; }

		public static FiveDayGoalStageTable FiveDayGoalStageTable  { get; private set; }

		public static GameChannelTable GameChannelTable  { get; private set; }

		public static GiftTabsTable GiftTabsTable  { get; private set; }

		public static GiftPackTable GiftPackTable  { get; private set; }

		public static Global Global  { get; private set; }

		public static InviteNewPlayerTable InviteNewPlayerTable  { get; private set; }

		public static ItemTable ItemTable  { get; private set; }

		public static WarehouseUpgradeTable WarehouseUpgradeTable  { get; private set; }

		public static WarehouseTypeTable WarehouseTypeTable  { get; private set; }

		public static LangTable LangTable  { get; private set; }

		public static LangTypeTable LangTypeTable  { get; private set; }

		public static MailTable MailTable  { get; private set; }

		public static SystemNoticeTable SystemNoticeTable  { get; private set; }

		public static MarqueeTable MarqueeTable  { get; private set; }

		public static PassCardTable PassCardTable  { get; private set; }

		public static PayTable PayTable  { get; private set; }

		public static PlayerLevelTable PlayerLevelTable  { get; private set; }

		public static ResourceBattleTable ResourceBattleTable  { get; private set; }

		public static SettingTable SettingTable  { get; private set; }

		public static ShopBaseTable ShopBaseTable  { get; private set; }

		public static ShopDailyTable ShopDailyTable  { get; private set; }

		public static ShopGoldTable ShopGoldTable  { get; private set; }

		public static ShopMythicTable ShopMythicTable  { get; private set; }

		public static ShopDiamondsTable ShopDiamondsTable  { get; private set; }

		public static SignInRewardTable SignInRewardTable  { get; private set; }

		public static SoundsTable SoundsTable  { get; private set; }

		public static TaskAchievementTable TaskAchievementTable  { get; private set; }

		public static TaskDailyTable TaskDailyTable  { get; private set; }

		public static TaskDailyRewardTable TaskDailyRewardTable  { get; private set; }

		public static TaskMainTable TaskMainTable  { get; private set; }

		public static JumpModuleTable JumpModuleTable  { get; private set; }


		public static void Initialize(Func<string, string> jsonLoader, Action<int, int> onProgress)
		{
		ActivityTable = ActivityTable.Parse(jsonLoader("Activity"));

		onProgress?.Invoke(1, TotalCount);
		ActivityEntranceTable = ActivityEntranceTable.Parse(jsonLoader("ActivityEntrance"));

		onProgress?.Invoke(2, TotalCount);
		AdsTable = AdsTable.Parse(jsonLoader("Ads"));

		onProgress?.Invoke(3, TotalCount);
		AllianceFlagTable = AllianceFlagTable.Parse(jsonLoader("AllianceFlag"));

		onProgress?.Invoke(4, TotalCount);
		AllianceLvTable = AllianceLvTable.Parse(jsonLoader("AllianceLv"));

		onProgress?.Invoke(5, TotalCount);
		AllianceScienceTable = AllianceScienceTable.Parse(jsonLoader("AllianceScience"));

		onProgress?.Invoke(6, TotalCount);
		AllianceRightTable = AllianceRightTable.Parse(jsonLoader("AllianceRight"));

		onProgress?.Invoke(7, TotalCount);
		AllianceRightTypeTable = AllianceRightTypeTable.Parse(jsonLoader("AllianceRightType"));

		onProgress?.Invoke(8, TotalCount);
		AllianceDonateTable = AllianceDonateTable.Parse(jsonLoader("AllianceDonate"));

		onProgress?.Invoke(9, TotalCount);
		AllianceShopTable = AllianceShopTable.Parse(jsonLoader("AllianceShop"));

		onProgress?.Invoke(10, TotalCount);
		AllianceSpecialOfferTable = AllianceSpecialOfferTable.Parse(jsonLoader("AllianceSpecialOffer"));

		onProgress?.Invoke(11, TotalCount);
		AllianceChallengeRewardTable = AllianceChallengeRewardTable.Parse(jsonLoader("AllianceChallengeReward"));

		onProgress?.Invoke(12, TotalCount);
		AvatarTable = AvatarTable.Parse(jsonLoader("Avatar"));

		onProgress?.Invoke(13, TotalCount);
		AvatarFrameTable = AvatarFrameTable.Parse(jsonLoader("AvatarFrame"));

		onProgress?.Invoke(14, TotalCount);
		CurrencyTable = CurrencyTable.Parse(jsonLoader("Currency"));

		onProgress?.Invoke(15, TotalCount);
		DailyBlessingTable = DailyBlessingTable.Parse(jsonLoader("DailyBlessing"));

		onProgress?.Invoke(16, TotalCount);
		FiveDayGoalTable = FiveDayGoalTable.Parse(jsonLoader("FiveDayGoal"));

		onProgress?.Invoke(17, TotalCount);
		FiveDayGoalStageTable = FiveDayGoalStageTable.Parse(jsonLoader("FiveDayGoalStage"));

		onProgress?.Invoke(18, TotalCount);
		GameChannelTable = GameChannelTable.Parse(jsonLoader("GameChannel"));

		onProgress?.Invoke(19, TotalCount);
		GiftTabsTable = GiftTabsTable.Parse(jsonLoader("GiftTabs"));

		onProgress?.Invoke(20, TotalCount);
		GiftPackTable = GiftPackTable.Parse(jsonLoader("GiftPack"));

		onProgress?.Invoke(21, TotalCount);
		Global = JsonConvert.DeserializeObject<Global>(jsonLoader("Global"));

		onProgress?.Invoke(22, TotalCount);
		InviteNewPlayerTable = InviteNewPlayerTable.Parse(jsonLoader("InviteNewPlayer"));

		onProgress?.Invoke(23, TotalCount);
		ItemTable = ItemTable.Parse(jsonLoader("Item"));

		onProgress?.Invoke(24, TotalCount);
		WarehouseUpgradeTable = WarehouseUpgradeTable.Parse(jsonLoader("WarehouseUpgrade"));

		onProgress?.Invoke(25, TotalCount);
		WarehouseTypeTable = WarehouseTypeTable.Parse(jsonLoader("WarehouseType"));

		onProgress?.Invoke(26, TotalCount);
		LangTable = LangTable.Parse(jsonLoader("Lang"));

		onProgress?.Invoke(27, TotalCount);
		LangTypeTable = LangTypeTable.Parse(jsonLoader("LangType"));

		onProgress?.Invoke(28, TotalCount);
		MailTable = MailTable.Parse(jsonLoader("Mail"));

		onProgress?.Invoke(29, TotalCount);
		SystemNoticeTable = SystemNoticeTable.Parse(jsonLoader("SystemNotice"));

		onProgress?.Invoke(30, TotalCount);
		MarqueeTable = MarqueeTable.Parse(jsonLoader("Marquee"));

		onProgress?.Invoke(31, TotalCount);
		PassCardTable = PassCardTable.Parse(jsonLoader("PassCard"));

		onProgress?.Invoke(32, TotalCount);
		PayTable = PayTable.Parse(jsonLoader("Pay"));

		onProgress?.Invoke(33, TotalCount);
		PlayerLevelTable = PlayerLevelTable.Parse(jsonLoader("PlayerLevel"));

		onProgress?.Invoke(34, TotalCount);
		ResourceBattleTable = ResourceBattleTable.Parse(jsonLoader("ResourceBattle"));

		onProgress?.Invoke(35, TotalCount);
		SettingTable = SettingTable.Parse(jsonLoader("Setting"));

		onProgress?.Invoke(36, TotalCount);
		ShopBaseTable = ShopBaseTable.Parse(jsonLoader("ShopBase"));

		onProgress?.Invoke(37, TotalCount);
		ShopDailyTable = ShopDailyTable.Parse(jsonLoader("ShopDaily"));

		onProgress?.Invoke(38, TotalCount);
		ShopGoldTable = ShopGoldTable.Parse(jsonLoader("ShopGold"));

		onProgress?.Invoke(39, TotalCount);
		ShopMythicTable = ShopMythicTable.Parse(jsonLoader("ShopMythic"));

		onProgress?.Invoke(40, TotalCount);
		ShopDiamondsTable = ShopDiamondsTable.Parse(jsonLoader("ShopDiamonds"));

		onProgress?.Invoke(41, TotalCount);
		SignInRewardTable = SignInRewardTable.Parse(jsonLoader("SignInReward"));

		onProgress?.Invoke(42, TotalCount);
		SoundsTable = SoundsTable.Parse(jsonLoader("Sounds"));

		onProgress?.Invoke(43, TotalCount);
		TaskAchievementTable = TaskAchievementTable.Parse(jsonLoader("TaskAchievement"));

		onProgress?.Invoke(44, TotalCount);
		TaskDailyTable = TaskDailyTable.Parse(jsonLoader("TaskDaily"));

		onProgress?.Invoke(45, TotalCount);
		TaskDailyRewardTable = TaskDailyRewardTable.Parse(jsonLoader("TaskDailyReward"));

		onProgress?.Invoke(46, TotalCount);
		TaskMainTable = TaskMainTable.Parse(jsonLoader("TaskMain"));

		onProgress?.Invoke(47, TotalCount);
		JumpModuleTable = JumpModuleTable.Parse(jsonLoader("JumpModule"));

		onProgress?.Invoke(48, TotalCount);
		}
	}
}
