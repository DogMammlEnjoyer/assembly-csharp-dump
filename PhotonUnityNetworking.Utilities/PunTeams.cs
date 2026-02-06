using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
	[Obsolete("do not use this or add it to the scene. use PhotonTeamsManager instead")]
	public class PunTeams : MonoBehaviourPunCallbacks
	{
		public void Start()
		{
			PunTeams.PlayersPerTeam = new Dictionary<PunTeams.Team, List<Player>>();
			foreach (object obj in Enum.GetValues(typeof(PunTeams.Team)))
			{
				PunTeams.PlayersPerTeam[(PunTeams.Team)obj] = new List<Player>();
			}
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.Start();
		}

		public override void OnJoinedRoom()
		{
			this.UpdateTeams();
		}

		public override void OnLeftRoom()
		{
			this.Start();
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			this.UpdateTeams();
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			this.UpdateTeams();
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			this.UpdateTeams();
		}

		[Obsolete("do not call this.")]
		public void UpdateTeams()
		{
			foreach (object obj in Enum.GetValues(typeof(PunTeams.Team)))
			{
				PunTeams.PlayersPerTeam[(PunTeams.Team)obj].Clear();
			}
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				Player player = PhotonNetwork.PlayerList[i];
				PunTeams.Team team = player.GetTeam();
				PunTeams.PlayersPerTeam[team].Add(player);
			}
		}

		[Obsolete("use PhotonTeamsManager.Instance.TryGetTeamMembers instead")]
		public static Dictionary<PunTeams.Team, List<Player>> PlayersPerTeam;

		[Obsolete("do not use this. PhotonTeamsManager.TeamPlayerProp is used internally instead.")]
		public const string TeamPlayerProp = "team";

		[Obsolete("use custom PhotonTeam instead")]
		public enum Team : byte
		{
			none,
			red,
			blue
		}
	}
}
