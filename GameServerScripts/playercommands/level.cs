/*
*level command v1.2 by Etaew! - Fallen Realms
*
*Changes:
*You can only /level at your trainer now (helps reduce server lag)
*
*ToDo:
*Make it easier to change what people can /level to?
*/

using DOL.Database;
using DOL.GS.PacketHandler;
using NHibernate.Expression;

namespace DOL.GS.Scripts
{
	[CmdAttribute("&level", //command to handle
	(int)ePrivLevel.Player, //minimum privelege level
	"Allows you level 20s instantly if you have a level 50", "/level")] //usage
	public class LevelCommandHandler : ICommandHandler
	{

		public int OnCommand(GameClient client, string[] args)
		{
			if (client.Player.TargetObject is GameTrainer)
			{
				//find a char level 50 in this account
				if (GameServer.Database.SelectObject(typeof(GamePlayer), Expression.And(Expression.Eq("AccountId" , client.Account.AccountId), Expression.Ge("Level", 50))) != null)
				{
					//if there is a level 50.. calculate the xp needed to get to
					//level 20 from the current level and give it to the player

					// only do this if the players level is  < 20
					if (client.Player.Experience < GamePlayer.XPLevel[19])
					{
						long newXP;
						//calculate xp to level 20 from current level..
						newXP = GamePlayer.XPLevel[19] - client.Player.Experience;
						if (newXP < 0) newXP = 0;
						client.Player.GainExperience(newXP);
						client.Player.UsedLevelCommand = true;
						client.Player.Out.SendMessage("You have been rewarded enough Experience to reach level 20, right click on your trainer to gain levels!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						return 1;
					}
					client.Player.Out.SendMessage("/level only allows you to level to 20", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return 0;
				}
					
				client.Player.Out.SendMessage("You don't have a level 50 on your account!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return 0;
			}
			client.Player.Out.SendMessage("You need to be at your trainer to use this command.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			return 0;
		}
	}
}
