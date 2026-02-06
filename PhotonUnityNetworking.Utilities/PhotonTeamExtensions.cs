using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public static class PhotonTeamExtensions
	{
		public static PhotonTeam GetPhotonTeam(this Player player)
		{
			object obj;
			PhotonTeam result;
			if (player.CustomProperties.TryGetValue("_pt", out obj) && PhotonTeamsManager.Instance.TryGetTeamByCode((byte)obj, out result))
			{
				return result;
			}
			return null;
		}

		public static bool JoinTeam(this Player player, PhotonTeam team)
		{
			if (team == null)
			{
				Debug.LogWarning("JoinTeam failed: PhotonTeam provided is null");
				return false;
			}
			if (player.GetPhotonTeam() != null)
			{
				Debug.LogWarningFormat("JoinTeam failed: player ({0}) is already joined to a team ({1}), call SwitchTeam instead", new object[]
				{
					player,
					team
				});
				return false;
			}
			return player.SetCustomProperties(new Hashtable
			{
				{
					"_pt",
					team.Code
				}
			}, null, null);
		}

		public static bool JoinTeam(this Player player, byte teamCode)
		{
			PhotonTeam team;
			return PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out team) && player.JoinTeam(team);
		}

		public static bool JoinTeam(this Player player, string teamName)
		{
			PhotonTeam team;
			return PhotonTeamsManager.Instance.TryGetTeamByName(teamName, out team) && player.JoinTeam(team);
		}

		public static bool SwitchTeam(this Player player, PhotonTeam team)
		{
			if (team == null)
			{
				Debug.LogWarning("SwitchTeam failed: PhotonTeam provided is null");
				return false;
			}
			PhotonTeam photonTeam = player.GetPhotonTeam();
			if (photonTeam == null)
			{
				Debug.LogWarningFormat("SwitchTeam failed: player ({0}) was not joined to any team, call JoinTeam instead", new object[]
				{
					player
				});
				return false;
			}
			if (photonTeam.Code == team.Code)
			{
				Debug.LogWarningFormat("SwitchTeam failed: player ({0}) is already joined to the same team {1}", new object[]
				{
					player,
					team
				});
				return false;
			}
			return player.SetCustomProperties(new Hashtable
			{
				{
					"_pt",
					team.Code
				}
			}, new Hashtable
			{
				{
					"_pt",
					photonTeam.Code
				}
			}, null);
		}

		public static bool SwitchTeam(this Player player, byte teamCode)
		{
			PhotonTeam team;
			return PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out team) && player.SwitchTeam(team);
		}

		public static bool SwitchTeam(this Player player, string teamName)
		{
			PhotonTeam team;
			return PhotonTeamsManager.Instance.TryGetTeamByName(teamName, out team) && player.SwitchTeam(team);
		}

		public static bool LeaveCurrentTeam(this Player player)
		{
			PhotonTeam photonTeam = player.GetPhotonTeam();
			if (photonTeam == null)
			{
				Debug.LogWarningFormat("LeaveCurrentTeam failed: player ({0}) was not joined to any team", new object[]
				{
					player
				});
				return false;
			}
			return player.SetCustomProperties(new Hashtable
			{
				{
					"_pt",
					null
				}
			}, new Hashtable
			{
				{
					"_pt",
					photonTeam.Code
				}
			}, null);
		}

		public static bool TryGetTeamMates(this Player player, out Player[] teamMates)
		{
			return PhotonTeamsManager.Instance.TryGetTeamMatesOfPlayer(player, out teamMates);
		}
	}
}
