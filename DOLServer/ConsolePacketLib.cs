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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.Housing;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.GS.Quests;

using log4net;
using DOL.Database;

namespace DOLGameServerConsole
{
	/// <summary>
	/// The packetlib for dummy console clients for /commands
	/// </summary>
	public class ConsolePacketLib : IPacketLib
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void SendMessage(string msg, eChatType type, eChatLoc loc)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("({0}, {1}): {2}", type, loc, msg));
			}
		}

		public void SendCustomDialog(string msg, CustomDialogResponse callback)
		{
			if (msg == null)
				msg = "(null)";
			if (callback == null)
			{
				if (log.IsDebugEnabled)
					log.Debug(string.Format("(info dialog): {0}", msg));
			}
			else
			{
				if (log.IsDebugEnabled)
					log.Debug(string.Format("Accepting dialog: {0} {1}\n\"{2}\"", callback.Target, callback.Method, msg));
				callback(null, 1);
			}
		}

		public byte GetPacketCode(eServerPackets packetCode) { return 0; }
		public void SendTCP(GSTCPPacketOut packet) { }
		public void SendWarlockChamberEffect(GamePlayer player) { }
		public void SendTCP(byte[] buf) { }
		public void SendTCPRaw(GSTCPPacketOut packet) { }
		public void SendUDP(GSUDPPacketOut packet) { }
		public void SendUDP(byte[] buf) { }
		public void SendUDPRaw(GSUDPPacketOut packet) { }
		public void SendVersionAndCryptKey() { }
		public void SendLoginDenied(eLoginError et) { }
		public void SendLoginGranted() { }
		public void SendLoginGranted(byte color) { } // help for rvr AND pvp servers
		public void SendSessionID() { }
		public void SendPingReply(ulong timestamp, ushort sequence) { }
		public void SendRealm(eRealm realm) { }
		public void SendCharacterOverview(eRealm realm) { }
		public void SendDupNameCheckReply(string name, bool nameExists) { }
		public void SendBadNameCheckReply(string name, bool bad) { }
		public void SendAttackMode(bool attackState) { }
		public void SendCharCreateReply(string name) { }
		public void SendCharStatsUpdate() { }
		public void SendCharResistsUpdate() { }
		public void SendRegions() { }
		public void SendGameOpenReply() { }
		public void SendPlayerPositionAndObjectID() { }
		public void SendPlayerJump(bool headingOnly) { }
		public void SendPlayerInitFinished(byte mobs) { }
		public void SendUDPInitReply() { }
		public void SendTime() { }
		public void SendPlayerCreate(GamePlayer playerToCreate) { }
		public void SendObjectGuildID(GameObject obj, Guild guild) { }
		public void SendPlayerQuit(bool totalOut) { }
		public void SendDebugMode(bool on) { }
		public void SendModelChange(GameObject obj, ushort newModel) { }
		public void SendModelAndSizeChange(GameObject obj, ushort newModel, byte newSize) { }
		public void SendModelAndSizeChange(ushort objectId, ushort newModel, byte newSize) { }
		public void SendEmoteAnimation(GameObject obj, eEmote emote) { }
		public void SendNPCCreate(GameNPC npc) { }
		public void SendLivingEquipmentUpdate(GameLiving living) { }
		public void SendRegionChanged() { }
		public void SendUpdatePoints() { }
		public void SendUpdateMoney() { }
		public void SendUpdateMaxSpeed() { }
		public void SendCombatAnimation(GameObject attacker, GameObject defender, ushort weaponID, ushort shieldID, int style, byte stance, byte result, byte targetHealthPercent) { }
		public void SendStatusUpdate() { }
		public void SendStatusUpdate(byte sittingFlag) { }
		public void SendSpellCastAnimation(GameLiving spellCaster, ushort spellID, ushort castingTime) { }
		public void SendSpellEffectAnimation(GameObject spellCaster, GameObject spellTarget, ushort spellid, ushort boltTime, bool noSound, byte success) { }
		public void SendRiding(GameObject rider, GameObject steed, bool dismount) { }
		public void SendFindGroupWindowUpdate(GamePlayer[] list) { }
		public void SendDialogBox(eDialogCode code, ushort data1, ushort data2, ushort data3, ushort data4, eDialogType type, bool autoWarpText, string message) { }
		public void SendGroupInviteCommand(GamePlayer invitingPlayer, string inviteMessage) { }
		public void SendGuildLeaveCommand(GamePlayer invitingPlayer, string inviteMessage) { }
		public void SendGuildInviteCommand(GamePlayer invitingPlayer, string inviteMessage) { }
		public void SendQuestSubscribeCommand(GameNPC invitingNPC, ushort questid, string inviteMessage) { }
		public void SendQuestOfferWindow(GameNPC questNPC, GamePlayer player, RewardQuest quest) { }
		public void SendQuestRewardWindow(GameNPC questNPC, GamePlayer player, RewardQuest quest) { }
		public void SendQuestOfferWindow(GameNPC questNPC, GamePlayer player, DataQuest quest) { }
		public void SendQuestRewardWindow(GameNPC questNPC, GamePlayer player, DataQuest quest) { }
		public void SendQuestAbortCommand(GameNPC abortingNPC, ushort questid, string abortMessage) { }
		public void SendGroupWindowUpdate() { }
		public void SendGroupMemberUpdate(bool updateIcons, GameLiving living) { }
		public void SendGroupMembersUpdate(bool updateIcons) { }
		public void SendInventoryItemsUpdate(ICollection<InventoryItem> itemsToUpdate) { }
		public void SendInventorySlotsUpdate(ICollection<int> slots) { }
		public void SendInventoryItemsUpdate(byte preAction, ICollection<InventoryItem> itemsToUpdate) { }
		public void SendInventoryItemsUpdate(IDictionary<int, InventoryItem> updateItems, byte windowType) { }
		public void SendInventoryItemsPartialUpdate(IDictionary<int, InventoryItem> items, byte windowType) { }
		public void SendDoorState(Region region, IDoor door) { }
		public void SendMerchantWindow(MerchantTradeItems itemlist, eMerchantWindowType windowType) { }
		public void SendTradeWindow() { }
		public void SendCloseTradeWindow() { }
		public void SendPlayerDied(GamePlayer killedPlayer, GameObject killer) { }
		public void SendPlayerRevive(GamePlayer revivedPlayer) { }
		public void SendUpdatePlayer() { }
		public void SendUpdatePlayerSkills() { }
		public void SendUpdateWeaponAndArmorStats() { }
		public void SendCustomTextWindow(string caption, IList<string> text) { }
		public void SendEncumberance() { }
		public void SendAddFriends(string[] friendNames) { }
		public void SendRemoveFriends(string[] friendNames) { }
		public void SendTimerWindow(string title, int seconds) { }
		public void SendCloseTimerWindow() { }
		public void SendTrainerWindow() { }
        public void SendChampionTrainerWindow(int type) { }
		public void SendInterruptAnimation(GameLiving living) { }
		public void SendDisableSkill(Skill skill, int duration) { }
		public void SendUpdateIcons(IList changedEffects, ref int lastUpdateEffectsCount) { }
		public void SendLevelUpSound() { }
		public void SendRegionEnterSound(byte soundId) { }
		public void SendSoundEffect(ushort soundId, ushort zoneId, ushort x, ushort y, ushort z, ushort radius) { }
		public void SendDebugMessage(string format, params object[] parameters) { }
		public void SendDebugPopupMessage(string format, params object[] parameters) { }
		public void SendEmblemDialogue() { }
		public void SendWeather(uint x, uint width, ushort speed, ushort fogdiffusion, ushort intensity) { }
		public void SendPlayerModelTypeChange(GamePlayer player, byte modelType) { }
		public void SendObjectDelete(GameObject obj) { }
		public void SendObjectUpdate(GameObject obj) { }
		public void SendObjectRemove(GameObject obj) { }
		public void SendObjectCreate(GameObject obj) { }
		public void SendQuestListUpdate() { }
		public void SendQuestUpdate(AbstractQuest quest) { }
		public void SendConcentrationList() { }
		public void SendUpdateCraftingSkills() { }
		public void SendChangeTarget(GameObject newTarget) { }
		public void SendChangeGroundTarget(Point3D newTarget) { }
		public void SendPetWindow(GameLiving pet, ePetWindowAction windowAction, eAggressionState aggroState, eWalkState walkState) { }
		public void SendKeepInfo(AbstractGameKeep keep) { }
		public void SendKeepRealmUpdate(AbstractGameKeep keep) { }
		public void SendKeepRemove(AbstractGameKeep keep) { }
		public void SendKeepComponentInfo(GameKeepComponent keepComponent) { }
		public void SendKeepComponentDetailUpdate(GameKeepComponent keepComponent) { }
		public void SendKeepComponentUpdate(AbstractGameKeep keep, bool levelup) { }
		public void SendKeepClaim(AbstractGameKeep keep, byte flag) { }
		public void SendKeepComponentInteract(GameKeepComponent component) { }
		public void SendKeepComponentHookPoint(GameKeepComponent component, int selectedHookPointIndex) { }
		public void SendKeepDoorUpdate(GameKeepDoor door) { }
		public void SendClearKeepComponentHookPoint(GameKeepComponent component, int selectedHookPointIndex) { }
		public void SendHookPointStore(GameKeepHookPoint hookPoint) { }
		public void SendPlaySound(eSoundType soundType, ushort soundID) { }
        public void SendNPCsQuestEffect(GameNPC npc, eQuestIndicator indicator) { }
		public void SendMasterLevelWindow(byte ml) { }
		public void SendHexEffect(GamePlayer player, byte effect1, byte effect2, byte effect3, byte effect4, byte effect5) { }

		public void SendSiegeWeaponAnimation(GameSiegeWeapon siegeWeapon) { }
		public void SendSiegeWeaponFireAnimation(GameSiegeWeapon siegeWeapon, int timer) { }
		public void SendSiegeWeaponCloseInterface() { }
		public void SendSiegeWeaponInterface(GameSiegeWeapon siegeWeapon, int time) { }
		public void SendHouse(House house) { }
		public void SendHouseOccupied(House house, bool flagHouseOccuped) { }
		public void SendHousePermissions(House house) { }
		public void SendRemoveHouse(House house) { }
		public void SendGarden(House house, int i) { }
		public void SendHousePayRentDialog(string title) { }
		public void SendGarden(House house) { }
		public void SendEnterHouse(House house) { }
		public void SendExitHouse(House house, ushort unknown = 0) { }
		public void SendHouseUsersPermissions(House house) { }
		public void SendFurniture(House house) { }
		public void SendFurniture(House house, int i) { }
		public void SendToggleHousePoints(House house) { }
		public void SendRentReminder(House house) { }
		public void SendMovingObjectCreate(GameMovingObject obj) { }
		public void SendWarmapUpdate(ICollection<AbstractGameKeep> list) { }
		public void SendWarmapDetailUpdate(List<List<byte>> fights, List<List<byte>> groups) { }
		public void SendWarmapBonuses() { }
		public void SendCheckLOS(GameObject Checker, GameObject Target, DOL.GS.PacketHandler.CheckLOSResponse callback) { }
		public void SendLivingDataUpdate(GameLiving living, bool updateStrings) { }
		public void SendPlayerTitles() { }
		public void SendPlayerTitleUpdate(GamePlayer player) { }
		public void SendSetControlledHorse(GamePlayer player) { }
		public void SendControlledHorse(GamePlayer player, bool flag) { }
		public void CheckLengthHybridSkillsPacket(ref GSTCPPacketOut pak, ref int maxSkills, ref int first) { }
		public void SendNonHybridSpellLines() { }
		public void SendCrash(string str) { }
		public void SendRvRGuildBanner(GamePlayer player, bool show) { }
		public void SendPlayerFreeLevelUpdate() { }
		public void SendRegionColorSheme() { }
		public void SendRegionColorSheme(byte color) { }
		public void SendStarterHelp() { }
		public void SendVampireEffect(GameLiving living, bool show) { }
		public void SendXFireInfo(byte flag) { }
		public void SendMarketExplorerWindow() { }
		public void SendMarketExplorerWindow(IList<InventoryItem> items, byte page, byte maxpage) { }
		public void SendConsignmentMerchantMoney(long copper) { }
        public void SendMinotaurRelicMapRemove(byte id) { }
        public void SendMinotaurRelicMapUpdate(byte id, ushort region, int x, int y, int z) { }
        public virtual void SendMinotaurRelicWindow(GamePlayer player, int spell, bool flag) { }
        public virtual void SendMinotaurRelicBarUpdate(GamePlayer player, int xp) { }
        public virtual void SendBlinkPanel(byte flag) { }
		/// <summary>
		/// The bow prepare animation
		/// </summary>
		public int BowPrepare { get { return 0; } }
		/// <summary>
		/// The bow shoot animation
		/// </summary>
		public int BowShoot { get { return 0; } }
		/// <summary>
		/// one dual weapon hit animation
		/// </summary>
		public int OneDualWeaponHit { get { return 0; } }
		/// <summary>
		/// both dual weapons hit animation
		/// </summary>
		public int BothDualWeaponHit { get { return 0; } }
	}
}
