using System;
using System.Reflection;
using System.Collections;
using DOL.Events;
using DOL.GS;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.Database;
using log4net;

namespace DOL.GS
{
	public enum eRelicType : int
	{
		Invalid = -1,
		Strength = 0,
		Magic = 1
	}

	public class GameRelic : GameStaticItem
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public const string PLAYER_CARRY_RELIC_WEAK = "IAmCarryingARelic";
		protected const int RelicEffectInterval = 4000;

		#region declarations
		InventoryItem m_item;
		GamePlayer m_currentCarrier = null;
		GameRelicPad m_currentRelicPad = null;	
		GameRelicPad m_returnRelicPad = null;
		RegionTimer m_currentCarrierTimer;
		DBRelic m_dbRelic;
		eRelicType m_relicType;
		RegionTimer m_returnRelicTimer;
		long m_timeRelicOnGround = 0;

		protected int ReturnRelicInterval
		{
			get { return ServerProperties.Properties.RELIC_RETURN_TIME * 1000; }
		}

		/// <summary>
		/// The place were the relic should go if it is lost by players
		/// after the expiration timer
		/// </summary>
		public virtual GameRelicPad ReturnRelicPad
		{
			get { return m_returnRelicPad; }
			set { m_returnRelicPad = value; }
		}
		
		/// <summary>
		/// Get the RelicType (melee or magic)
		/// </summary>
		public eRelicType RelicType
		{
			get
			{
				return m_relicType;
			}
		}

		private eRealm m_originalRealm;

		/// <summary>
		/// Get the original Realm of the relict (can only be 1(alb),2(mid) or 3(hibernia))
		/// </summary>
		public eRealm OriginalRealm
		{
			get
			{
				return m_originalRealm;
			}
		}

		/// <summary>
		/// Returns the carriing player if there is one.
		/// </summary>
		public GameRelicPad CurrentRelicPad
		{
			get
			{
				return m_currentRelicPad;
			}
		}

		/// <summary>
		/// Returns the carriing player if there is one.
		/// </summary>
		public GamePlayer CurrentCarrier
		{
			get
			{
				return m_currentCarrier;
			}
		}

		public bool IsMounted
		{
			get
			{
				return (m_currentRelicPad != null);
			}
		}

		public static bool IsPlayerCarryingRelic(GamePlayer player)
		{
			return player.TempProperties.getProperty<object>(PLAYER_CARRY_RELIC_WEAK, null) != null;
		}

		#endregion

		#region constructor
		public GameRelic() : base() { m_saveInDB = true; }


		public GameRelic(DBRelic obj)
			: this()
		{
			LoadFromDatabase(obj);
		}
		#endregion

		#region behavior
		/// <summary>
		/// This method is called whenever a player tries to interact with this object
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;

			if (!player.IsAlive)
			{
				player.Out.SendMessage("You cannot pickup " + GetName(0, false) + ". You are dead!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			if (IsMounted && player.Realm == Realm)
			{
				player.Out.SendMessage("You cannot pickup " + GetName(0, false) + ". It is owned by your realm.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			if (IsMounted && !RelicMgr.CanPickupRelicFromShrine(player, this))
			{
				player.Out.SendMessage("You cannot pickup " + GetName(0, false) + ". You need to capture your realms " + (Enum.GetName(typeof(eRelicType), RelicType)) + " relic first.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}

			PlayerTakesRelic(player);
			return true;
		}

		public virtual void RelicPadTakesOver(GameRelicPad pad, bool returning)
		{
			m_currentRelicPad = pad;
			Realm = pad.Realm;
			pad.MountRelic(this, returning);
			CurrentRegionID = pad.CurrentRegionID;
			PlayerLoosesRelic(true);
			X = pad.X;
			Y = pad.Y;
			Z = pad.Z;
			Heading = pad.Heading;
			SaveIntoDatabase();
			AddToWorld();
		}


		#region protected stuff

		protected virtual void Update()
		{
			if (m_item == null || m_currentCarrier == null)
				return;
			CurrentRegionID = m_currentCarrier.CurrentRegionID;
			X = m_currentCarrier.X;
			Y = m_currentCarrier.Y;
			Z = m_currentCarrier.Z;
			Heading = m_currentCarrier.Heading;
		}


		/// <summary>
		/// This method is called from the Interaction with the GameStaticItem
		/// </summary>
		/// <param name="player"></param>
		protected virtual void PlayerTakesRelic(GamePlayer player)
		{
			if (player.TempProperties.getProperty<object>(PLAYER_CARRY_RELIC_WEAK, null) != null)
			{
				player.Out.SendMessage("You are already carrying a relic.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			if (player.IsStealthed)
			{
				player.Out.SendMessage("You cannot carry a relic while stealthed.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (!player.IsAlive)
			{
				player.Out.SendMessage("You are dead!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (IsMounted)
			{
				AbstractGameKeep keep = KeepMgr.getKeepCloseToSpot(m_currentRelicPad.CurrentRegionID, m_currentRelicPad, WorldMgr.VISIBILITY_DISTANCE);

				log.DebugFormat("keep {0}", keep);

				if (keep != null && keep.Realm != player.Realm)
				{
					player.Out.SendMessage("You must capture this keep before taking a relic.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return;
				}
			}

			if (player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, m_item))
			{
				if (m_item == null)
					log.Warn("GameRelic: Could not retrive " + Name + " as InventoryItem on player " + player.Name);


				m_currentCarrier = player;
				player.TempProperties.setProperty(PLAYER_CARRY_RELIC_WEAK, this);
				player.Out.SendUpdateMaxSpeed();

				if (IsMounted)
				{
					m_currentRelicPad.RemoveRelic(this);
					ReturnRelicPad = m_currentRelicPad;
					m_currentRelicPad = null;
				}

				RemoveFromWorld();
				SaveIntoDatabase();
				Realm = 0;
				SetHandlers(player, true);
				StartPlayerTimer(player);
				if (m_returnRelicTimer != null)
				{
					m_returnRelicTimer.Stop();
					m_returnRelicTimer = null;
				}

			}
			else
			{
				player.Out.SendMessage("You dont have enough space in your backpack to carry this.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
		}


		/// <summary>
		/// Is called whenever the CurrentCarrier is supposed to loose the relic.
		/// </summary>
		/// <param name="removeFromInventory">Defines wheater the Item in the Inventory should be removed.</param>
		protected virtual void PlayerLoosesRelic(bool removeFromInventory)
		{
			if (m_currentCarrier == null)
			{
				return;
			}

			GamePlayer player = m_currentCarrier;

			if (player.TempProperties.getProperty<object>(PLAYER_CARRY_RELIC_WEAK, null) == null)
			{
				log.Warn("GameRelic: " + player.Name + " has already lost" + Name);
				return;
			}
			if (removeFromInventory)
			{
				lock (player.Inventory.AllItems.SyncRoot)
				{
					bool success = player.Inventory.RemoveItem(m_item);
					log.Debug("Remove " + m_item.Name + " from " + player.Name + "'s Inventory " + ((success) ? "successfully." : "with errors."));
				}

			}

			// remove the handlers from the player
			SetHandlers(player, false);
			//kill the pulsingEffectTimer on the player
			StartPlayerTimer(null);

			player.TempProperties.removeProperty(PLAYER_CARRY_RELIC_WEAK);
			m_currentCarrier = null;
			player.Out.SendUpdateMaxSpeed();

			if (IsMounted == false)
			{
				// launch the reset timer if this relic is not dropped on a pad
				m_timeRelicOnGround = CurrentRegion.Time;
				m_returnRelicTimer = new RegionTimer(this, new RegionTimerCallback(ReturnRelicTick), RelicEffectInterval);
				log.DebugFormat("{0} dropped, return timer for relic set to {1} seconds.", Name, ReturnRelicInterval / 1000);

				// update the position of the worldObject Relic
				Update();
				SaveIntoDatabase();
				AddToWorld();
			}
		}

		/// <summary>
		/// when the relic is lost and ReturnRelicInterval is elapsed
		/// </summary>
		protected virtual int ReturnRelicTick(RegionTimer timer)
		{
			if (CurrentRegion.Time - m_timeRelicOnGround < ReturnRelicInterval)
			{
				// Note: This does not show up, possible issue with SendSpellEffect
				ushort effectID = (ushort)Util.Random(5811, 5815);
				foreach (GamePlayer ppl in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				{
					ppl.Out.SendSpellEffectAnimation(this, this, effectID, 0, false, 0x01);
				}
				return RelicEffectInterval;
			}

			if (ReturnRelicPad != null)
			{
				log.Debug("Relic " + this.Name + " is lost and returns to " + ReturnRelicPad.ToString());
				RemoveFromWorld();
				RelicPadTakesOver(ReturnRelicPad, true);
				SaveIntoDatabase();
				AddToWorld();
			}
			else
			{
				log.Error("Relic " + this.Name + " is lost and ReturnRelicPad is null!");
			}
			m_returnRelicTimer.Stop();
			m_returnRelicTimer = null;
			return 0;
		}
		
		/// <summary>
		/// Starts the "signalising effect" sequence on the carrier.
		/// </summary>
		/// <param name="player">Player to set the timer on. Timer stops if param is null</param>
		protected virtual void StartPlayerTimer(GamePlayer player)
		{
			if (player != null)
			{
				if (m_currentCarrierTimer != null)
				{
					log.Warn("GameRelic: PlayerTimer already set on a player, stopping timer!");
					m_currentCarrierTimer.Stop();
					m_currentCarrierTimer = null;
				}
				m_currentCarrierTimer = new RegionTimer(player, new RegionTimerCallback(CarrierTimerTick));
				m_currentCarrierTimer.Start(RelicEffectInterval);

			}
			else
			{
				if (m_currentCarrierTimer != null)
				{
					m_currentCarrierTimer.Stop();
					m_currentCarrierTimer = null;
				}
			}


		}

		/// <summary>
		/// The callback for the pulsing spelleffect
		/// </summary>
		/// <param name="timer">The ObjectTimerCallback object</param>
		private int CarrierTimerTick(RegionTimer timer)
		{
			//update the relic position
			Update();

			// check to make sure relic is in a legal region and still in the players backpack

			if (CurrentRegionID != DOL.GS.Keeps.KeepMgr.NEW_FRONTIERS)
			{
				log.DebugFormat("{0} taken out of New Frontiers, relic returned to previous pad.", Name);
				RelicPadTakesOver(ReturnRelicPad, true);
				SaveIntoDatabase();
				AddToWorld();
				return 0;
			}

			if (CurrentCarrier != null && CurrentCarrier.Inventory.GetFirstItemByID(m_item.Id_nb, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack) == null)
			{
				log.DebugFormat("{0} not found in carriers backpack, relic returned to previous pad.", Name);
				RelicPadTakesOver(ReturnRelicPad, true);
				SaveIntoDatabase();
				AddToWorld();
				return 0;
			}

			//fireworks spells temp
			ushort effectID = (ushort)Util.Random(5811, 5815);
			foreach (GamePlayer ppl in m_currentCarrier.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				ppl.Out.SendSpellEffectAnimation(m_currentCarrier, m_currentCarrier, effectID, 0, false, 0x01);

			return RelicEffectInterval;
		}


		/// <summary>
		/// Enables or Deactivate the handlers for the carrying player behavior
		/// </summary>
		/// <param name="player"></param>
		/// <param name="activate"></param>
		protected virtual void SetHandlers(GamePlayer player, bool activate)
		{
			if (activate)
			{
				GameEventMgr.AddHandler(player, GamePlayerEvent.Quit, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.AddHandler(player, GamePlayerEvent.Dying, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.AddHandler(player, GamePlayerEvent.StealthStateChanged, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.AddHandler(player, GamePlayerEvent.Linkdeath, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.AddHandler(player, PlayerInventoryEvent.ItemDropped, new DOLEventHandler(PlayerAbsence));

			}
			else
			{
				GameEventMgr.RemoveHandler(player, GamePlayerEvent.Quit, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.RemoveHandler(player, GamePlayerEvent.Dying, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.RemoveHandler(player, GamePlayerEvent.StealthStateChanged, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.RemoveHandler(player, GamePlayerEvent.Linkdeath, new DOLEventHandler(PlayerAbsence));
				GameEventMgr.RemoveHandler(player, PlayerInventoryEvent.ItemDropped, new DOLEventHandler(PlayerAbsence));
			}


		}
		protected void PlayerAbsence(DOLEvent e, object sender, EventArgs args)
		{
			Realm=0;
			if (e == PlayerInventoryEvent.ItemDropped)
			{
				ItemDroppedEventArgs idArgs = args as ItemDroppedEventArgs;
				if (idArgs.SourceItem.Name != m_item.Name) return;
				idArgs.GroundItem.RemoveFromWorld();
				PlayerLoosesRelic(false);
				return;
			}
			PlayerLoosesRelic(true);
		}


		#endregion


		#endregion

		public override IList GetExamineMessages(GamePlayer player)
		{

			IList messages = base.GetExamineMessages(player);
			messages.Add((IsMounted) ? ("It is owned by " + ((player.Realm == Realm) ? "your realm" : GlobalConstants.RealmToName((eRealm)Realm)) + ".") : "It is without owner, take it!");
			return messages;
		}

		#region database load/save
		/// <summary>
		/// Loads the GameRelic from Database
		/// </summary>
		/// <param name="obj">The DBRelic-object for this relic</param>
		public override void LoadFromDatabase(DataObject obj)
		{
			InternalID = obj.ObjectId;
			m_dbRelic = obj as DBRelic;
			CurrentRegionID = (ushort)m_dbRelic.Region;
			X = m_dbRelic.X;
			Y = m_dbRelic.Y;
			Z = m_dbRelic.Z;
			Heading = (ushort)m_dbRelic.Heading;
			m_relicType = (eRelicType)m_dbRelic.relicType;
			Realm = (eRealm)m_dbRelic.Realm;
			m_originalRealm = (eRealm)m_dbRelic.OriginalRealm;


			//get constant values
			MiniTemp template = GetRelicTemplate(m_originalRealm, m_relicType);
			m_Name = template.Name;
			m_Model = template.Model;
			template = null;

			//set still empty fields
			Emblem = 0;
			Level = 99;



			ItemTemplate m_itemTemp;
			//generate itemtemplate for inventoryitem
			m_itemTemp = new ItemTemplate();
			m_itemTemp.Name = Name;
			m_itemTemp.Object_Type = (int)eObjectType.Magical;
			m_itemTemp.Model = Model;
			m_itemTemp.IsDropable = true;
			m_itemTemp.IsPickable = false;
			m_itemTemp.Level = 99;
			m_itemTemp.Quality = 100;
			m_itemTemp.Copper = 0;
			m_itemTemp.Silver = 0;
			m_itemTemp.Gold = 0;
			m_itemTemp.Platinum = 0;
			m_itemTemp.PackSize = 1;
			m_itemTemp.AutoSave = false;
			m_itemTemp.Weight = 1000;
			m_itemTemp.Id_nb = "ARelic";
			m_itemTemp.IsTradable = false;
			m_item = new InventoryItem(m_itemTemp);
		}
		/// <summary>
		/// Saves the current GameRelic to the database
		/// </summary>
		public override void SaveIntoDatabase()
		{
			m_dbRelic.Realm = (int)Realm;
			m_dbRelic.OriginalRealm = (int)OriginalRealm;
			m_dbRelic.Heading = (int)Heading;
			m_dbRelic.Region = (int)CurrentRegionID;
			m_dbRelic.relicType = (int)RelicType;
			m_dbRelic.X = X;
			m_dbRelic.Y = Y;
			m_dbRelic.Z = Z;

			if (InternalID == null)
			{
				GameServer.Database.AddObject(m_dbRelic);
				InternalID = m_dbRelic.ObjectId;
			}
			else
				GameServer.Database.SaveObject(m_dbRelic);
		}
		#endregion

		#region utils

		/// <summary>
		/// Returns a Template for Name and Model for the relic
		/// </summary>
		/// <returns>this object has only set Realm and Name</returns>
		public class MiniTemp
		{
			public MiniTemp() { }
			public string Name;
			public ushort Model;
		}

		public static MiniTemp GetRelicTemplate(eRealm Realm, eRelicType RelicType)
		{


			MiniTemp m_template = new MiniTemp();
			switch (Realm)
			{
				case eRealm.Albion:
					if (RelicType == eRelicType.Magic)
					{
						m_template.Name = "Merlins Staff";
						m_template.Model = 630;
					}
					else
					{
						m_template.Name = "Scabbard of Excalibur";
						m_template.Model = 631;
					}
					break;
				case eRealm.Midgard:
					if (RelicType == eRelicType.Magic)
					{
						m_template.Name = "Horn of Valhalla";
						m_template.Model = 635;
					}
					else
					{
						m_template.Name = "Thors Hammer";
						m_template.Model = 634;
					}
					break;
				case eRealm.Hibernia:
					if (RelicType == eRelicType.Magic)
					{
						m_template.Name = "Cauldron of Dagda";
						m_template.Model = 632;
					}
					else
					{
						m_template.Name = " Lughs Spear of Lightning";
						m_template.Model = 633;
					}
					break;
				default:
					m_template.Name = "Unkown Relic";
					m_template.Model = 633;
					break;

			}
			return m_template;
		}
		#endregion

	}

}
