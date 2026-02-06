using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
	public static class ScoreExtensions
	{
		public static void SetScore(this Player player, int newScore)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["score"] = newScore;
			player.SetCustomProperties(hashtable, null, null);
		}

		public static void AddScore(this Player player, int scoreToAddToCurrent)
		{
			int num = player.GetScore();
			num += scoreToAddToCurrent;
			Hashtable hashtable = new Hashtable();
			hashtable["score"] = num;
			player.SetCustomProperties(hashtable, null, null);
		}

		public static int GetScore(this Player player)
		{
			object obj;
			if (player.CustomProperties.TryGetValue("score", out obj))
			{
				return (int)obj;
			}
			return 0;
		}
	}
}
