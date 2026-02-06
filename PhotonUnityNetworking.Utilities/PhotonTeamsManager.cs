using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	[DisallowMultipleComponent]
	public class PhotonTeamsManager : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks
	{
		public static event Action<Player, PhotonTeam> PlayerJoinedTeam;

		public static event Action<Player, PhotonTeam> PlayerLeftTeam;

		public static PhotonTeamsManager Instance
		{
			get
			{
				if (PhotonTeamsManager.instance == null)
				{
					PhotonTeamsManager.instance = Object.FindObjectOfType<PhotonTeamsManager>();
					if (PhotonTeamsManager.instance == null)
					{
						PhotonTeamsManager.instance = new GameObject
						{
							name = "PhotonTeamsManager"
						}.AddComponent<PhotonTeamsManager>();
					}
					PhotonTeamsManager.instance.Init();
				}
				return PhotonTeamsManager.instance;
			}
		}

		private void Awake()
		{
			if (PhotonTeamsManager.instance == null || this == PhotonTeamsManager.instance)
			{
				this.Init();
				PhotonTeamsManager.instance = this;
				return;
			}
			Object.Destroy(this);
		}

		private void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		private void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			this.ClearTeams();
		}

		private void Init()
		{
			this.teamsByCode = new Dictionary<byte, PhotonTeam>(this.teamsList.Count);
			this.teamsByName = new Dictionary<string, PhotonTeam>(this.teamsList.Count);
			this.playersPerTeam = new Dictionary<byte, HashSet<Player>>(this.teamsList.Count);
			for (int i = 0; i < this.teamsList.Count; i++)
			{
				this.teamsByCode[this.teamsList[i].Code] = this.teamsList[i];
				this.teamsByName[this.teamsList[i].Name] = this.teamsList[i];
				this.playersPerTeam[this.teamsList[i].Code] = new HashSet<Player>();
			}
		}

		void IMatchmakingCallbacks.OnJoinedRoom()
		{
			this.UpdateTeams();
		}

		void IMatchmakingCallbacks.OnLeftRoom()
		{
			this.ClearTeams();
		}

		void IMatchmakingCallbacks.OnPreLeavingRoom()
		{
		}

		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			object obj;
			if (changedProps.TryGetValue("_pt", out obj))
			{
				if (obj == null)
				{
					using (Dictionary<byte, HashSet<Player>>.KeyCollection.Enumerator enumerator = this.playersPerTeam.Keys.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							byte key = enumerator.Current;
							if (this.playersPerTeam[key].Remove(targetPlayer))
							{
								if (PhotonTeamsManager.PlayerLeftTeam != null)
								{
									PhotonTeamsManager.PlayerLeftTeam(targetPlayer, this.teamsByCode[key]);
									break;
								}
								break;
							}
						}
						return;
					}
				}
				if (obj is byte)
				{
					byte b = (byte)obj;
					foreach (byte b2 in this.playersPerTeam.Keys)
					{
						if (b2 != b && this.playersPerTeam[b2].Remove(targetPlayer))
						{
							if (PhotonTeamsManager.PlayerLeftTeam != null)
							{
								PhotonTeamsManager.PlayerLeftTeam(targetPlayer, this.teamsByCode[b2]);
								break;
							}
							break;
						}
					}
					PhotonTeam photonTeam = this.teamsByCode[b];
					if (!this.playersPerTeam[b].Add(targetPlayer))
					{
						Debug.LogWarningFormat("Unexpected situation while setting team {0} for player {1}, updating teams for all", new object[]
						{
							photonTeam,
							targetPlayer
						});
						this.UpdateTeams();
					}
					if (PhotonTeamsManager.PlayerJoinedTeam != null)
					{
						PhotonTeamsManager.PlayerJoinedTeam(targetPlayer, photonTeam);
						return;
					}
				}
				else
				{
					Debug.LogErrorFormat("Unexpected: custom property key {0} should have of type byte, instead we got {1} of type {2}. Player: {3}", new object[]
					{
						"_pt",
						obj,
						obj.GetType(),
						targetPlayer
					});
				}
			}
		}

		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
			if (otherPlayer.IsInactive)
			{
				return;
			}
			PhotonTeam photonTeam = otherPlayer.GetPhotonTeam();
			if (photonTeam != null && !this.playersPerTeam[photonTeam.Code].Remove(otherPlayer))
			{
				Debug.LogWarningFormat("Unexpected situation while removing player {0} who left from team {1}, updating teams for all", new object[]
				{
					otherPlayer,
					photonTeam
				});
				this.UpdateTeams();
			}
		}

		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
			PhotonTeam photonTeam = newPlayer.GetPhotonTeam();
			if (photonTeam == null)
			{
				return;
			}
			if (this.playersPerTeam[photonTeam.Code].Contains(newPlayer))
			{
				return;
			}
			foreach (byte key in this.teamsByCode.Keys)
			{
				if (this.playersPerTeam[key].Remove(newPlayer))
				{
					break;
				}
			}
			if (!this.playersPerTeam[photonTeam.Code].Add(newPlayer))
			{
				Debug.LogWarningFormat("Unexpected situation while adding player {0} who joined to team {1}, updating teams for all", new object[]
				{
					newPlayer,
					photonTeam
				});
				this.UpdateTeams();
			}
		}

		private void UpdateTeams()
		{
			this.ClearTeams();
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				Player player = PhotonNetwork.PlayerList[i];
				PhotonTeam photonTeam = player.GetPhotonTeam();
				if (photonTeam != null)
				{
					this.playersPerTeam[photonTeam.Code].Add(player);
				}
			}
		}

		private void ClearTeams()
		{
			foreach (byte key in this.playersPerTeam.Keys)
			{
				this.playersPerTeam[key].Clear();
			}
		}

		public bool TryGetTeamByCode(byte code, out PhotonTeam team)
		{
			return this.teamsByCode.TryGetValue(code, out team);
		}

		public bool TryGetTeamByName(string teamName, out PhotonTeam team)
		{
			return this.teamsByName.TryGetValue(teamName, out team);
		}

		public PhotonTeam[] GetAvailableTeams()
		{
			if (this.teamsList != null)
			{
				return this.teamsList.ToArray();
			}
			return null;
		}

		public bool TryGetTeamMembers(byte code, out Player[] members)
		{
			members = null;
			HashSet<Player> hashSet;
			if (this.playersPerTeam.TryGetValue(code, out hashSet))
			{
				members = new Player[hashSet.Count];
				int num = 0;
				foreach (Player player in hashSet)
				{
					members[num] = player;
					num++;
				}
				return true;
			}
			return false;
		}

		public bool TryGetTeamMembers(string teamName, out Player[] members)
		{
			members = null;
			PhotonTeam photonTeam;
			return this.TryGetTeamByName(teamName, out photonTeam) && this.TryGetTeamMembers(photonTeam.Code, out members);
		}

		public bool TryGetTeamMembers(PhotonTeam team, out Player[] members)
		{
			members = null;
			return team != null && this.TryGetTeamMembers(team.Code, out members);
		}

		public bool TryGetTeamMatesOfPlayer(Player player, out Player[] teamMates)
		{
			teamMates = null;
			if (player == null)
			{
				return false;
			}
			PhotonTeam photonTeam = player.GetPhotonTeam();
			if (photonTeam == null)
			{
				return false;
			}
			HashSet<Player> hashSet;
			if (this.playersPerTeam.TryGetValue(photonTeam.Code, out hashSet))
			{
				if (!hashSet.Contains(player))
				{
					Debug.LogWarningFormat("Unexpected situation while getting team mates of player {0} who is joined to team {1}, updating teams for all", new object[]
					{
						player,
						photonTeam
					});
					this.UpdateTeams();
				}
				teamMates = new Player[hashSet.Count - 1];
				int num = 0;
				foreach (Player player2 in hashSet)
				{
					if (!player2.Equals(player))
					{
						teamMates[num] = player2;
						num++;
					}
				}
				return true;
			}
			return false;
		}

		public int GetTeamMembersCount(byte code)
		{
			PhotonTeam team;
			if (this.TryGetTeamByCode(code, out team))
			{
				return this.GetTeamMembersCount(team);
			}
			return 0;
		}

		public int GetTeamMembersCount(string name)
		{
			PhotonTeam team;
			if (this.TryGetTeamByName(name, out team))
			{
				return this.GetTeamMembersCount(team);
			}
			return 0;
		}

		public int GetTeamMembersCount(PhotonTeam team)
		{
			HashSet<Player> hashSet;
			if (team != null && this.playersPerTeam.TryGetValue(team.Code, out hashSet) && hashSet != null)
			{
				return hashSet.Count;
			}
			return 0;
		}

		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
		{
		}

		[SerializeField]
		private List<PhotonTeam> teamsList = new List<PhotonTeam>
		{
			new PhotonTeam
			{
				Name = "Blue",
				Code = 1
			},
			new PhotonTeam
			{
				Name = "Red",
				Code = 2
			}
		};

		private Dictionary<byte, PhotonTeam> teamsByCode;

		private Dictionary<string, PhotonTeam> teamsByName;

		private Dictionary<byte, HashSet<Player>> playersPerTeam;

		public const string TeamPlayerProp = "_pt";

		private static PhotonTeamsManager instance;
	}
}
