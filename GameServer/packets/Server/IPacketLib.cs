/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System.Collections;
using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Database;
using DOL.GS.Housing;
using DOL.GS.Keeps;
using DOL.GS.Quests;

namespace DOL.GS.PacketHandler
{
	public enum eServerPackets : byte
	{
		InventoryUpdate = 0x02,
		HouseUserPermissions = 0x03,
		CharacterJump = 0x04,
		HousingPermissions = 0x05,
		HouseEnter = 0x08,
		HousingItem = 0x09,
		HouseExit = 0x0A,
		HouseTogglePoints = 0x0F,
		MovingObjectCreate = 0x12,
		EquipmentUpdate = 0x15,
		VariousUpdate = 0x16,
		MerchantWindow = 0x17,
		HouseDecorationRotate = 0x18,
		SpellEffectAnimation = 0x1B,
		ConsignmentMerchantMoney = 0x1E,
		MarketExplorerWindow = 0x1F,
		PositionAndObjectID = 0x20,
		DebugMode = 0x21,
		CryptKey = 0x22,
		SessionID = 0x28,
		PingReply = 0x29,
		LoginGranted = 0x2A,
		CharacterInitFinished = 0x2B,
		LoginDenied = 0x2C,
		GameOpenReply = 0x2D,
		UDPInitReply = 0x2F,
		MinotaurRelicMapRemove = 0x45,
		MinotaurRelicMapUpdate = 0x46,
		WarMapClaimedKeeps = 0x49,
		WarMapDetailUpdate = 0x4A,
		PlayerCreate172 = 0x4B,
		VisualEffect = 0x4C,
		ControlledHorse = 0x4E,
		MinotaurRelicRealm = 0x59,
		XFire = 0x5C,
		KeepComponentInteractResponse = 0x61,
		KeepClaim = 0x62,
		KeepComponentHookpointStore = 0x63,
		KeepComponentHookpointUpdate = 0x65,
		WarmapBonuses = 0x66,
		KeepComponentUpdate = 0x67,
		KeepInfo = 0x69,
		KeepRealmUpdate = 0x6A,
		KeepRemove = 0x6B,
		KeepComponentInfo = 0x6C,
		KeepComponentDetailUpdate = 0x6D,
		GroupMemberUpdate = 0x70,
		SpellCastAnimation = 0x72,
		InterruptSpellCast = 0x73,
		AttackMode = 0x74,
		ConcentrationList = 0x75,
		TrainerWindow = 0x7B,
		Time = 0x7E,
		UpdateIcons = 0x7F,
		Dialog = 0x81,
		QuestEntry = 0x83,
		FindGroupUpdate = 0x86,
		PetWindow = 0x88,
		PlayerRevive = 0x89,
		PlayerModelTypeChange = 0x8D,
		CharacterPointsUpdate = 0x91,
		Weather = 0x92,
		DoorState = 0x99,
		ClientRegions = 0x9E,
		ObjectUpdate = 0xA1,
		RemoveObject = 0xA2,
		Quit = 0xA4,
		PlayerPosition = 0xA9,
		CharacterStatusUpdate = 0xAD,
		PlayerDeath = 0xAE,
		Message = 0xAF,
		MaxSpeed = 0xB6,
		RegionChanged = 0xB7,
		PlayerHeading = 0xBA,
		CombatAnimation = 0xBC,
		Encumberance = 0xBD,
		BadNameCheckReply = 0xC3,
		DetailWindow = 0xC4,
		AddFriend = 0xC5,
		RemoveFriend = 0xC6,
		Riding = 0xC8,
		SoundEffect = 0xC9,
		DupNameCheckReply = 0xCC,
		HouseCreate = 0xD1,
		HouseChangeGarden = 0xD2,
		PlaySound = 0xD3,
		PlayerCreate = 0xD4,
		DisableSkills = 0xD6,
		ObjectCreate = 0xD9,
		NPCCreate = 0xDA,
		ModelChange = 0xDB,
		ObjectGuildID = 0xDE,
		ChangeGroundTarget = 0xDF,
		ObjectDelete = 0xE1,
		EmblemDialogue = 0xE2,
		SiegeWeaponAnimation = 0xE3,
		TradeWindow = 0xEA,
		ObjectDataUpdate = 0xEE,
		RegionSound = 0xEF,
		CharacterCreateReply = 0xF0,
		TimerWindow = 0xF3,
		SiegeWeaponInterface = 0xF5,
		ChangeTarget = 0xF6,
		HelpWindow = 0xF7,
		EmoteAnimation = 0xF9,
		MoneyUpdate = 0xFA,
		StatsUpdate = 0xFB,
		CharacterOverview = 0xFD,
		Realm = 0xFE,
		MasterLevelWindow = 0x13,
	}

	public enum eClientPackets : byte
	{
		PlayerCancelsEffect = 0xF8,			// 0x50 ^ 168
		PlayerAttackRequest = 0x74,			// 0xDC ^ 168
		PlayerAppraiseItemRequest = 0xE0,	// 0x48 ^ 168
		PetWindow = 0x8A,
		ObjectInteractRequest = 0x7A,		// 0xD2 ^ 168
		InviteToGroup = 0x87,				// 0x2F ^ 168
		HouseEnterLeave = 0x0B,
		DoorRequest = 0x99,					// 0x31 ^ 168
		DisbandFromGroup = 0xA8,			// 0x37 ^ 168
		DialogResponse = 0x82,				// 0x2A ^ 168
		CheckLOSRequest = 0xD0,
		WorldInit = 0xD4,					// 0x7C ^ 168
		UseSpell = 0x7D,					// 0xD5 ^ 168
		UseSlot = 0x71,						// 0xD9 ^ 168
		UseSkill = 0xBB,					// 0x13 ^ 168
		RemoveConcentrationEffect = 0x76,	// 0xDE ^ 168
		PlayerRegionChangeRequest = 0x90,	// 0x38 ^ 168
		QuestRewardChosen = 0x40,
		PlayerTarget = 0xB0,				// 0x18 ^ 168
		PlayerSitRequest = 0xC7,			// 0x6F ^ 168
		PlayerInitRequest = 0xE8,			// 0x40 ^ 168
		PlayerGroundTarget = 0xEC,			// 0x44 ^ 168
		PlayerDismountRequest = 0xC8,		// 0x60 ^ 168
		PlayerHeadingUpdate = 0xBA,			// 0x12 ^ 168  also known as Short State
		PlayerPickupHouseItem = 0x0D,
	}

	/// <summary>
	/// Enum for LoginDeny reasons
	/// </summary>
	public enum eLoginError : byte
	{
		// From testing with client version 1.98 US:
		// All of these values send no message (client just displays "Service not available"):
		// 0x04, 0x0e, 0x0f, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f
		WrongPassword = 0x01,
		AccountInvalid = 0x02,
		AuthorizationServerUnavailable = 0x03,
		ClientVersionTooLow = 0x05,
		CannotAccessUserAccount = 0x06,
		AccountNotFound = 0x07,
		AccountNoAccessAnyGame = 0x08,
		AccountNoAccessThisGame = 0x09,
		AccountClosed = 0x0a,
		AccountAlreadyLoggedIn = 0x0b,
		TooManyPlayersLoggedIn = 0x0c,
		GameCurrentlyClosed = 0x0d,
		AccountAlreadyLoggedIntoOtherServer = 0x10,
		AccountIsInLogoutProcedure = 0x11,
		ExpansionPacketNotAllowed = 0x12, // "You have not been invited to join this server type." (1.98 US)
		AccountIsBannedFromThisServerType = 0x13,
		CafeIsOutOfPlayingTime = 0x14,
		PersonalAccountIsOutOfTime = 0x15,
		CafesAccountIsSuspended = 0x16,
		NotAuthorizedToUseExpansionVersion = 0x17, // "You are not authorized to use the expansion version!" (1.98 US)
		ServiceNotAvailable = 0xaa
	} ;

	/// <summary>
	/// Chat locations on the client window
	/// </summary>
	public enum eChatLoc : byte
	{
		CL_ChatWindow = 0x0,
		CL_PopupWindow = 0x1,
		CL_SystemWindow = 0x2
	} ;

	/// <summary>
	/// Types of chat messages
	/// </summary>
	public enum eChatType : byte
	{
		CT_System = 0x00,
		CT_Say = 0x01,
		CT_Send = 0x02,
		CT_Group = 0x03,
		CT_Guild = 0x04,
		CT_Broadcast = 0x05,
		CT_Emote = 0x06,
		CT_Help = 0x07,
		CT_Chat = 0x08,
		CT_Advise = 0x09,
		CT_Officer = 0x0a,
		CT_Alliance = 0x0b,
		CT_BattleGroup = 0x0c,
		CT_BattleGroupLeader = 0x0d,
		// 0x0e sends nothing (tested with client v1.99)
		CT_Staff = 0xf,

		CT_Spell = 0x10,
		CT_YouHit = 0x11,
		CT_YouWereHit = 0x12,
		CT_Skill = 0x13,
		CT_Merchant = 0x14,
		CT_YouDied = 0x15,
		CT_PlayerDied = 0x16,
		CT_OthersCombat = 0x17,
		CT_DamageAdd = 0x18,
		CT_SpellExpires = 0x19,
		CT_Loot = 0x1a,
		CT_SpellResisted = 0x1b,
		CT_Important = 0x1c,
		CT_Damaged = 0x1d,
		CT_Missed = 0x1e,
		CT_SpellPulse = 0x1f,
		CT_KilledByAlb = 0x20,
		CT_KilledByMid = 0x21,
		CT_KilledByHib = 0x22,
		CT_LFG = 0x23,
		CT_Trade = 0x24,

		CT_SocialInterface = 0x64,
		CT_ScreenCenter = 0xC8,
		CT_ScreenCenterSmaller = 0xC9,
		CT_ScreenCenter_And_CT_System = 0xCA,
		CT_ScreenCenterSmaller_And_CT_System = 0xCB,
	} ;

	public enum eEmote : byte
	{
		Beckon = 0x1,
		Blush = 0x2,
		Bow = 0x3,
		Cheer = 0x4,
		Clap = 0x5,
		Cry = 0x6,
		Curtsey = 0x7,
		Flex = 0x8,
		BlowKiss = 0x9,
		Dance = 0xa,
		Laugh = 0xb,
		Point = 0xc,
		Salute = 0xd,
		BangOnShield = 0xe,
		Victory = 0xf,
		Wave = 0x10,
		Distract = 0x11,
		MidgardFrenzy = 0x12,
		ThrowDirt = 0x13,
		StagFrenzy = 0x14,
		Roar = 0x15,
		Drink = 0x16,
		Ponder = 0x17,
		Military = 0x18,
		Present = 0x19,
		/*Taunt = 0x1a,*/
		Rude = 0x1b,
		Taunt = 0x1c,
		Hug = 0x1d,
		LetsGo = 0x1e,
		Meditate = 0x1f,
		No = 0x20,
		Raise = 0x21,
		Shrug = 0x22,
		Slap = 0x23,
		Slit = 0x24,
		Surrender = 0x25,
		Yes = 0x26,
		Beg = 0x27,
		Induct = 0x28,
		Dismiss = 0x29,
		LvlUp = 0x2a,
		Pray = 0x2b,
		Bind = 0x2c,
		SpellGoBoom = 0x2d,
		Knock = 0x2e,
		Smile = 0x2f,
		Angry = 0x30,
		Rider_LookFar = 0x31,
		Rider_Stench = 0x32,
		Rider_Halt = 0x33,
		Rider_pet = 0x34,
		Horse_Courbette = 0x35,
		Horse_Startle = 0x36,
		Horse_Nod = 0x37,
		Horse_Graze = 0x38,
		Horse_rear = 0x39,
		Sweat = 0x3a,
		Stagger = 0x3b,
		Rider_Trick = 0x3c,
		Yawn = 0x3d,
		Doh = 0x3e,
		Confused = 0x3f,
		Shiver = 0x40,
		Rofl = 0x41,
		Mememe = 0x42,
		Horse_whistle = 0x43,
		Worship = 0x44,
		PlayerPrepare = 0x45,
		PlayerPickup = 0x46,
		PlayerListen = 0x47,
		BindAlb = 0x49,
		BindMid = 0x4a,
		BindHib = 0x4b,
	};

	public enum ePreActionType : byte
	{
		UpdateLastOpened = 0,
		InitPaperdoll = 1,
		InitBackpack = 2,
		InitVaultKeeper = 3,
		InitHouseVault = 4,
		InitOwnConsigmentMerchant = 5, // have SetPrice,Withdraw
		InitConsigmentMerchant = 6,// have Buy
		HorseBags = 7,
		ContinueBackpack = 12,
		ContinueVaultKeeper = 13,
		ContinueHouseVault = 14,
		ContinueConsigmentMerchant = 15,
		ContinueOtherConsigmentMerchant = 16,
	}


	public enum eMerchantWindowType : byte
	{
		Normal = 0x00,
		Bp = 0x01,
		Count = 0x02,
		HousingOutsideMenu = 0x04,
		HousingNPCHookpoint = 0x05,
		HousingInsideShop = 0x06,
		HousingOutsideShop = 0x07,
		HousingVaultHookpoint = 0x08,
		HousingCraftingHookpoint = 0x09,
		HousingBindstoneHookpoint = 0x0A,
		HousingInsideMenu = 0x0B,
		HousingTicket = 0x0C,
		HousingGuildTicket = 0x0D,
	}

	/// <summary>
	/// The RegionEntry structure
	/// </summary>
	public struct RegionEntry
	{
		/// <summary>
		/// Region expansion
		/// </summary>
		public int expansion;

		/// <summary>
		/// Port client receives on
		/// </summary>
		public string fromPort;

		/// <summary>
		/// Region id
		/// </summary>
		public ushort id;

		/// <summary>
		/// Region IP address
		/// </summary>
		public string ip;

		/// <summary>
		/// Name of the region
		/// </summary>
		public string name;

		/// <summary>
		/// Port the region receives on
		/// </summary>
		public string toPort;
	} ;

	public delegate void CustomDialogResponse(GamePlayer player, byte response);

	public delegate void CheckLOSResponse(GamePlayer player, ushort response, ushort targetOID);

	public enum eSoundType : ushort
	{
		Craft = 0x01,
		Divers = 0x02,
	}

	public enum ePetWindowAction : byte
	{
		Open,
		Update,
		Close,
	}

	public enum eDialogType : byte
	{
		Ok = 0x00,
		YesNo = 0x01,
		Warmap = 0x01,
	}

	public enum eDialogCode : byte
	{
		SimpleWarning = 0x00,
		GuildInvite = 0x03,
		GroupInvite = 0x05,
		CustomDialog = 0x06,
		GuildLeave = 0x08,
		HousePayRent = 0x14,
		MasterLevelWindow = 0x19,
		KeepClaim = 0x1A,
		BuyRespec = 0x20,
		WarmapWindowHibernia = 0x30,
		WarmapWindowAlbion = 0x31,
		WarmapWindowMidgard = 0x32,
		QuestSubscribe = 0x64,
	}

	public enum eRealmWarmapKeepFlags : byte
	{
		Claimed = 0x04,
		UnderSiege = 0x08,
		Teleportable = 0x10,
	}

	public enum ePanel : byte
	{
		Command_Window = 0,
		Journal_Button = 1,
		Map_Button = 2,
		Sit_Button = 3,
		Stats_Index_Window = 4,
		Attributes_Button = 5,
		Inventory_Button = 6,
		Specializations_Button = 7,
		CombatStyles_Button = 8,
		MagicSpells_Button = 9,
		Group_Button = 0x0A,
		MiniInfo_Window = 0x0B,
		CommandEnter_Window = 0x0C,
		QuickBar1_Window = 0x0D,
		QBar1_Bank1Button = 0x0E,
		QBar1_Bank2Button = 0x0F,
		QBar1_Bank3Button = 0x10,
		QBar1_Bank4Button = 0x11,
		QBar1_Bank5Button = 0x12,
		QBar1_Bank6Button = 0x13,
		QBar1_Bank7Button = 0x14,
		QBar1_Bank8Button = 0x15,
		QBar1_Bank9Button = 0x16,
		QBar1_Bank10Button = 0x17,
	}

	public interface IPacketLib
	{
		/// <summary>
		/// The bow prepare animation
		/// </summary>
		int BowPrepare { get; }

		/// <summary>
		/// The bow shoot animation
		/// </summary>
		int BowShoot { get; }

		/// <summary>
		/// one dual weapon hit animation
		/// </summary>
		int OneDualWeaponHit { get; }

		/// <summary>
		/// both dual weapons hit animation
		/// </summary>
		int BothDualWeaponHit { get; }

		byte GetPacketCode(eServerPackets packetCode);
		void SendTCP(GSTCPPacketOut packet);
		void SendTCP(byte[] buf);
		void SendTCPRaw(GSTCPPacketOut packet);
		void SendUDP(GSUDPPacketOut packet);
		void SendUDP(byte[] buf);
		void SendUDPRaw(GSUDPPacketOut packet);
		// warlock
		void SendWarlockChamberEffect(GamePlayer player);
		void SendVersionAndCryptKey();
		void SendLoginDenied(eLoginError et);
		void SendLoginGranted();
		void SendLoginGranted(byte color);
		void SendSessionID();
		void SendPingReply(ulong timestamp, ushort sequence);
		void SendRealm(eRealm realm);
		void SendCharacterOverview(eRealm realm);
		void SendDupNameCheckReply(string name, bool nameExists);
		void SendBadNameCheckReply(string name, bool bad);
		void SendAttackMode(bool attackState);
		void SendCharCreateReply(string name);
		void SendCharStatsUpdate();
		void SendCharResistsUpdate();
		void SendRegions();
		void SendGameOpenReply();
		void SendPlayerPositionAndObjectID();
		void SendPlayerJump(bool headingOnly);
		void SendPlayerInitFinished(byte mobs);
		void SendUDPInitReply();
		void SendTime();
		void SendMessage(string msg, eChatType type, eChatLoc loc);
		void SendPlayerCreate(GamePlayer playerToCreate);
		void SendObjectGuildID(GameObject obj, Guild guild);
		void SendPlayerQuit(bool totalOut);
		void SendObjectRemove(GameObject obj);
		void SendObjectCreate(GameObject obj);
		void SendDebugMode(bool on);
		void SendModelChange(GameObject obj, ushort newModel);
		void SendModelAndSizeChange(GameObject obj, ushort newModel, byte newSize);
		void SendModelAndSizeChange(ushort objectId, ushort newModel, byte newSize);
		void SendEmoteAnimation(GameObject obj, eEmote emote);
		void SendNPCCreate(GameNPC npc);
		void SendLivingEquipmentUpdate(GameLiving living);
		void SendRegionChanged();
		void SendUpdatePoints();
		void SendUpdateMoney();
		void SendUpdateMaxSpeed();

		void SendCombatAnimation(GameObject attacker, GameObject defender, ushort weaponID, ushort shieldID, int style,
		                         byte stance, byte result, byte targetHealthPercent);

		void SendStatusUpdate();
		void SendStatusUpdate(byte sittingFlag);
		void SendSpellCastAnimation(GameLiving spellCaster, ushort spellID, ushort castingTime);

		void SendSpellEffectAnimation(GameObject spellCaster, GameObject spellTarget, ushort spellid, ushort boltTime,
		                              bool noSound, byte success);

		void SendRiding(GameObject rider, GameObject steed, bool dismount);
		void SendFindGroupWindowUpdate(GamePlayer[] list);
		void SendGroupInviteCommand(GamePlayer invitingPlayer, string inviteMessage);

		void SendDialogBox(eDialogCode code, ushort data1, ushort data2, ushort data3, ushort data4, eDialogType type,
		                   bool autoWrapText, string message);

		void SendCustomDialog(string msg, CustomDialogResponse callback);
		void SendCheckLOS(GameObject Checker, GameObject Target, CheckLOSResponse callback);
		void SendGuildLeaveCommand(GamePlayer invitingPlayer, string inviteMessage);
		void SendGuildInviteCommand(GamePlayer invitingPlayer, string inviteMessage);
		void SendQuestOfferWindow(GameNPC questNPC, GamePlayer player, RewardQuest quest);
		void SendQuestRewardWindow(GameNPC questNPC, GamePlayer player, RewardQuest quest);
		void SendQuestOfferWindow(GameNPC questNPC, GamePlayer player, DataQuest quest);
		void SendQuestRewardWindow(GameNPC questNPC, GamePlayer player, DataQuest quest);
		void SendQuestSubscribeCommand(GameNPC invitingNPC, ushort questid, string inviteMessage);
		void SendQuestAbortCommand(GameNPC abortingNPC, ushort questid, string abortMessage);
		void SendGroupWindowUpdate();
		void SendGroupMemberUpdate(bool updateIcons, GameLiving living);
		void SendGroupMembersUpdate(bool updateIcons);
		void SendInventoryItemsUpdate(ICollection<InventoryItem> itemsToUpdate);
		void SendInventorySlotsUpdate(ICollection<int> slots);
		void SendInventoryItemsUpdate(byte preAction, ICollection<InventoryItem> itemsToUpdate);
		void SendInventoryItemsUpdate(IDictionary<int, InventoryItem> updateItems, byte windowType);
		void SendDoorState(Region region, IDoor door);
		void SendMerchantWindow(MerchantTradeItems itemlist, eMerchantWindowType windowType);
		void SendTradeWindow();
		void SendCloseTradeWindow();
		void SendPlayerDied(GamePlayer killedPlayer, GameObject killer);
		void SendPlayerRevive(GamePlayer revivedPlayer);
		void SendUpdatePlayer();
		void SendUpdatePlayerSkills();
		void SendUpdateWeaponAndArmorStats();
		void SendCustomTextWindow(string caption, IList<string> text);
		void SendPlayerTitles();
		void SendPlayerTitleUpdate(GamePlayer player);
		void SendEncumberance();
		void SendAddFriends(string[] friendNames);
		void SendRemoveFriends(string[] friendNames);
		void SendTimerWindow(string title, int seconds);
		void SendCloseTimerWindow();
		void SendChampionTrainerWindow(int type);
		void SendTrainerWindow();
		void SendInterruptAnimation(GameLiving living);
		void SendDisableSkill(Skill skill, int duration);
		void SendUpdateIcons(IList changedEffects, ref int lastUpdateEffectsCount);
		void SendLevelUpSound();
		void SendRegionEnterSound(byte soundId);
		void SendDebugMessage(string format, params object[] parameters);
		void SendDebugPopupMessage(string format, params object[] parameters);
		void SendEmblemDialogue();
		void SendWeather(uint x, uint width, ushort speed, ushort fogdiffusion, ushort intensity);
		void SendPlayerModelTypeChange(GamePlayer player, byte modelType);
		void SendObjectDelete(GameObject obj);
		void SendObjectUpdate(GameObject obj);
		void SendQuestListUpdate();
		void SendQuestUpdate(AbstractQuest quest);
		void SendConcentrationList();
		void SendUpdateCraftingSkills();
		void SendChangeTarget(GameObject newTarget);
		void SendChangeGroundTarget(Point3D newTarget);
		void SendPetWindow(GameLiving pet, ePetWindowAction windowAction, eAggressionState aggroState, eWalkState walkState);
		void SendPlaySound(eSoundType soundType, ushort soundID);
		void SendNPCsQuestEffect(GameNPC npc, eQuestIndicator indicator);
		void SendMasterLevelWindow(byte ml);
		void SendHexEffect(GamePlayer player, byte effect1, byte effect2, byte effect3, byte effect4, byte effect5);
		void SendRvRGuildBanner(GamePlayer player, bool show);
		void SendSiegeWeaponAnimation(GameSiegeWeapon siegeWeapon);
		void SendSiegeWeaponFireAnimation(GameSiegeWeapon siegeWeapon, int timer);
		void SendSiegeWeaponCloseInterface();
		void SendSiegeWeaponInterface(GameSiegeWeapon siegeWeapon, int time);
		void SendLivingDataUpdate(GameLiving living, bool updateStrings);
		void SendSoundEffect(ushort soundId, ushort zoneId, ushort x, ushort y, ushort z, ushort radius);
		//keep
		void SendKeepInfo(AbstractGameKeep keep);
		void SendKeepRealmUpdate(AbstractGameKeep keep);
		void SendKeepRemove(AbstractGameKeep keep);
		void SendKeepComponentInfo(GameKeepComponent keepComponent);
		void SendKeepComponentDetailUpdate(GameKeepComponent keepComponent);
		void SendKeepClaim(AbstractGameKeep keep, byte flag);
		void SendKeepComponentUpdate(AbstractGameKeep keep, bool LevelUp);
		void SendKeepComponentInteract(GameKeepComponent component);
		void SendKeepComponentHookPoint(GameKeepComponent component, int selectedHookPointIndex);
		void SendClearKeepComponentHookPoint(GameKeepComponent component, int selectedHookPointIndex);
		void SendHookPointStore(GameKeepHookPoint hookPoint);
		void SendWarmapUpdate(ICollection<AbstractGameKeep> list);
		void SendWarmapDetailUpdate(List<List<byte>> fights, List<List<byte>> groups);
		void SendWarmapBonuses();

		//housing
		void SendHouse(House house);
		void SendHouseOccupied(House house, bool flagHouseOccuped);
		void SendRemoveHouse(House house);
		void SendGarden(House house);
		void SendGarden(House house, int i);
		void SendEnterHouse(House house);
		void SendExitHouse(House house, ushort unknown = 0);
		void SendFurniture(House house);
		void SendFurniture(House house, int i);
		void SendHousePayRentDialog(string title);
		void SendToggleHousePoints(House house);
		void SendRentReminder(House house);
		void SendMarketExplorerWindow(IList<InventoryItem> items, byte page, byte maxpage);
		void SendMarketExplorerWindow();
		void SendConsignmentMerchantMoney(long copper);
		void SendHouseUsersPermissions(House house);

		void SendStarterHelp();
		void SendPlayerFreeLevelUpdate();

		void SendMovingObjectCreate(GameMovingObject obj);
		void SendSetControlledHorse(GamePlayer player);
		void SendControlledHorse(GamePlayer player, bool flag);
		void CheckLengthHybridSkillsPacket(ref GSTCPPacketOut pak, ref int maxSkills, ref int first);
		void SendNonHybridSpellLines();
		void SendCrash(string str);
		void SendRegionColorSheme();
		void SendRegionColorSheme(byte color);
		void SendVampireEffect(GameLiving living, bool show);
		void SendXFireInfo(byte flag);
		void SendMinotaurRelicMapRemove(byte id);
		void SendMinotaurRelicMapUpdate(byte id, ushort region, int x, int y, int z);
		void SendMinotaurRelicWindow(GamePlayer player, int spell, bool flag);
		void SendMinotaurRelicBarUpdate(GamePlayer player, int xp);

		/// <summary>
		/// Makes a specific UI Part "blink"
		/// </summary>
		/// <param name="flag">The UI part as byte (See ePanel enum for details)</param>
		void SendBlinkPanel(byte flag);
	}
}