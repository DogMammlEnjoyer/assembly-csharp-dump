using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

[DefaultExecutionOrder(0)]
public class SuperInfectionManager : MonoBehaviour, IGameEntityZoneComponent, IFactoryItemProvider
{
	private void Awake()
	{
		GameEntityManager gameEntityManager = this.gameEntityManager;
		gameEntityManager.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(gameEntityManager.OnEntityRemoved, new Action<GameEntity>(this.OnEntityRemoved));
	}

	public void OnEnableZoneSuperInfection(SuperInfection zone)
	{
		this.zoneSuperInfection = zone;
		if (this.PendingZoneInit)
		{
			this.PendingZoneInit = false;
			this.OnZoneInit();
		}
		if (this.gameEntityManager.PendingTableData)
		{
			this.gameEntityManager.ResolveTableData();
		}
	}

	private void OnEnable()
	{
		if (!SuperInfectionManager.siManagerByZone.TryAdd(this.gameEntityManager.zone, this))
		{
			Debug.LogError("[GT/SuperInfectionManager]  ERROR!!!  " + string.Format("Tried to add a duplicate Manager for zone `{0}`. Did you forget to change the ", this.gameEntityManager.zone) + "zone on the GameEntityManager on GameObject at path: " + base.transform.GetPathQ(), this);
			return;
		}
		GameMode.OnStartGameMode += this._OnStartGameMode;
	}

	private void OnDisable()
	{
		SuperInfectionManager.siManagerByZone.Remove(this.gameEntityManager.zone);
		GameMode.OnStartGameMode -= this._OnStartGameMode;
	}

	private void _OnStartGameMode(GameModeType newGameModeType)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		List<GameEntityId> list;
		using (CollectionPool<List<GameEntityId>, GameEntityId>.Get(out list))
		{
			ESuperGameModes esuperGameModes = (ESuperGameModes)(1 << (int)newGameModeType);
			foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
			{
				SIGadget sigadget;
				SITechTreePage sitechTreePage;
				if (!(gameEntity == null) && gameEntity.TryGetComponent<SIGadget>(out sigadget) && this.techTreeSO.TryGetTreePage(sigadget.PageId, out sitechTreePage) && (sitechTreePage.excludedGameModes & esuperGameModes) != (ESuperGameModes)0)
				{
					list.Add(gameEntity.id);
				}
			}
			if (list.Count > 0)
			{
				this.gameEntityManager.RequestDestroyItems(list);
				Debug.Log("[GT/SuperInfectionManager]  _OnStartGameMode: " + string.Format("Removed {0} blaster gadgets because they are only allowed in SuperInfection.", list.Count), this);
			}
		}
	}

	public static SuperInfectionManager GetSIManagerForZone(GTZone targetZone)
	{
		SuperInfectionManager result;
		if (SuperInfectionManager.siManagerByZone.TryGetValue(targetZone, out result))
		{
			return result;
		}
		return null;
	}

	public void OnZoneCreate()
	{
	}

	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].WriteDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].WriteDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.WriteDataPUN(stream, info);
	}

	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthorityPlayer(info.Sender))
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].ReadDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].ReadDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.ReadDataPUN(stream, info);
	}

	void IGameEntityZoneComponent.SerializeZoneData(BinaryWriter writer)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].SerializeZoneData(writer);
		}
	}

	void IGameEntityZoneComponent.DeserializeZoneData(BinaryReader reader)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].DeserializeZoneData(reader);
		}
	}

	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	void IGameEntityZoneComponent.SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		SIPlayer siplayer = SIPlayer.Get(actorNumber);
		siplayer.SerializeNetworkState(writer, siplayer.gamePlayer.rig.OwningNetPlayer);
	}

	void IGameEntityZoneComponent.DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		SIPlayer player = SIPlayer.Get(actorNumber);
		SIPlayer.DeserializeNetworkStateAndBurn(reader, player, this);
	}

	public bool IsZoneReady()
	{
		return NetworkSystem.Instance.InRoom && SuperInfectionManager.IsSuperGameMode() && this.zoneSuperInfection.IsNotNull() && VRRig.LocalRig.zoneEntity.currentZone == this.gameEntityManager.zone;
	}

	public bool ShouldClearZone()
	{
		if (GameMode.ActiveGameMode != null)
		{
			GameModeType gameModeType = GameMode.ActiveGameMode.GameType();
			return gameModeType != GameModeType.SuperInfect && gameModeType != GameModeType.SuperCasual;
		}
		return false;
	}

	public static bool IsSuperGameMode()
	{
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (activeGameMode != null)
		{
			GameModeType gameModeType = activeGameMode.GameType();
			return gameModeType == GameModeType.SuperInfect || gameModeType == GameModeType.SuperCasual;
		}
		return false;
	}

	public void OnCreateGameEntity(GameEntity entity)
	{
		SIGadget component = entity.GetComponent<SIGadget>();
		if (component != null)
		{
			SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)-1)));
			if (siplayer != null)
			{
				int num = 0;
				for (int i = siplayer.activePlayerGadgets.Count - 1; i >= 0; i--)
				{
					GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
					if (gameEntityFromNetId == null)
					{
						siplayer.activePlayerGadgets.RemoveAt(i);
					}
					else
					{
						num++;
						if (num >= siplayer.totalGadgetLimit)
						{
							this.gameEntityManager.DestroyItemLocal(gameEntityFromNetId.id);
							break;
						}
					}
				}
				if (!siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
				{
					siplayer.activePlayerGadgets.Add(entity.GetNetId());
				}
			}
			SIUpgradeSet siupgradeSet = new SIUpgradeSet((int)(entity.createData >> 32));
			siupgradeSet = component.FilterUpgradeNodes(siupgradeSet);
			component.ApplyUpgradeNodes(siupgradeSet);
			component.RefreshUpgradeVisuals(siupgradeSet);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.AddGadget(component);
			}
		}
		List<SuperInfectionSnapPoint> list;
		using (CollectionPool<List<SuperInfectionSnapPoint>, SuperInfectionSnapPoint>.Get(out list))
		{
			entity.GetComponentsInChildren<SuperInfectionSnapPoint>(true, list);
			foreach (SuperInfectionSnapPoint snapPoint in list)
			{
				this.RegisterSnapPoint(snapPoint);
			}
		}
	}

	public void OnZoneClear(ZoneClearReason reason)
	{
		SuperInfection superInfection = this.zoneSuperInfection;
		if (superInfection != null)
		{
			superInfection.OnZoneClear(reason);
		}
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null)
		{
			localPlayer.Reset();
		}
		SIPlayer.ClearPlayerCache();
		this.allSnapPoints.Clear();
	}

	public void OnZoneInit()
	{
		if (this.zoneSuperInfection == null)
		{
			this.PendingZoneInit = true;
			return;
		}
		string[] array = new string[5];
		array[0] = "poop - switching super infection zone from \"";
		int num = 1;
		SuperInfectionManager superInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		array[num] = ((superInfectionManager != null) ? superInfectionManager.name : null);
		array[2] = "\" to \"";
		array[3] = base.name;
		array[4] = "\"";
		Debug.Log(string.Concat(array), this);
		SuperInfectionManager.activeSuperInfectionManager = this;
		if (this.gameEntityManager.IsAuthority())
		{
			this.TestSpawnGadget();
		}
		this.zoneSuperInfection.OnZoneInit();
		if (SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.progression.Init();
			if (this.progression.ClientReady)
			{
				SIPlayer.SetAndBroadcastProgression();
			}
			else
			{
				this.progression.OnClientReady += this.<OnZoneInit>g__WhenReady|39_0;
			}
		}
		this.allSnapPoints.Clear();
		foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
		{
			if (!(gameEntity == null))
			{
				List<SuperInfectionSnapPoint> list;
				using (CollectionPool<List<SuperInfectionSnapPoint>, SuperInfectionSnapPoint>.Get(out list))
				{
					gameEntity.GetComponentsInChildren<SuperInfectionSnapPoint>(true, list);
					foreach (SuperInfectionSnapPoint snapPoint in list)
					{
						this.RegisterSnapPoint(snapPoint);
					}
				}
			}
		}
	}

	public void RegisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		List<SuperInfectionSnapPoint> list;
		if (!this.allSnapPoints.TryGetValue(snapPoint.jointType, out list))
		{
			list = (this.allSnapPoints[snapPoint.jointType] = new List<SuperInfectionSnapPoint>());
		}
		list.Add(snapPoint);
	}

	public void UnregisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		if (this.allSnapPoints.ContainsKey(snapPoint.jointType))
		{
			this.allSnapPoints[snapPoint.jointType].Remove(snapPoint);
			if (this.allSnapPoints[snapPoint.jointType].Count == 0)
			{
				this.allSnapPoints.Remove(snapPoint.jointType);
			}
		}
	}

	public IEnumerable<SuperInfectionSnapPoint> GetPoints(SnapJointType jointType)
	{
		foreach (KeyValuePair<SnapJointType, List<SuperInfectionSnapPoint>> keyValuePair in this.allSnapPoints)
		{
			if ((keyValuePair.Key & jointType) != SnapJointType.None)
			{
				foreach (SuperInfectionSnapPoint superInfectionSnapPoint in keyValuePair.Value)
				{
					yield return superInfectionSnapPoint;
				}
				List<SuperInfectionSnapPoint>.Enumerator enumerator2 = default(List<SuperInfectionSnapPoint>.Enumerator);
			}
		}
		Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator enumerator = default(Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator);
		yield break;
		yield break;
	}

	public SuperInfectionSnapPoint FindNearestSnapPoint(SnapJointType jointType, Vector3 origin, float maxDist, bool includeOccupied = false)
	{
		SuperInfectionSnapPoint result = null;
		float num = maxDist * maxDist;
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.GetPoints(jointType))
		{
			if (!(superInfectionSnapPoint == null) && superInfectionSnapPoint.isActiveAndEnabled && (includeOccupied || !superInfectionSnapPoint.HasSnapped()))
			{
				float sqrMagnitude = (superInfectionSnapPoint.transform.position - origin).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = superInfectionSnapPoint;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public void CallRPC(SuperInfectionManager.ClientToAuthorityRPC clientToAuthorityRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToAuthorityRPC", this.gameEntityManager.GetAuthorityPlayer(), new object[]
			{
				(int)clientToAuthorityRPC,
				data
			});
		}
	}

	public void CallRPC(SuperInfectionManager.ClientToClientRPC clientToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToClientRPC", RpcTarget.Others, new object[]
			{
				(int)clientToClientRPC,
				data
			});
		}
	}

	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", RpcTarget.Others, new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, int actorNr, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", NetworkSystem.Instance.GetNetPlayerByID(actorNr).GetPlayerRef(), new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	[PunRPC]
	public void SIClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToAuthorityRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToAuthorityRPC(clientToAuthorityRPCEnum, data, info);
	}

	public void ProcessClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToAuthorityRPCEnum)
		{
		case 0:
		{
			if (data.Length != 4)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			int data2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out data2))
			{
				return;
			}
			int num2;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num2))
			{
				return;
			}
			int num3;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out num3))
			{
				return;
			}
			if (num3 < 0 || num3 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SITouchscreenButton.SITouchscreenButtonType), (SITouchscreenButton.SITouchscreenButtonType)num))
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SICombinedTerminal.TerminalSubFunction), (SICombinedTerminal.TerminalSubFunction)num2))
			{
				return;
			}
			this.zoneSuperInfection.siTerminals[num3].TouchscreenButtonPressed((SITouchscreenButton.SITouchscreenButtonType)num, data2, info.Sender.ActorNumber, (SICombinedTerminal.TerminalSubFunction)num2);
			return;
		}
		case 1:
		{
			if (data.Length != 1)
			{
				return;
			}
			int num4;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num4))
			{
				return;
			}
			if (num4 < 0 || num4 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			SICombinedTerminal sicombinedTerminal = this.zoneSuperInfection.siTerminals[num4];
			if (!siplayer.gamePlayer.rig.IsPositionInRange(sicombinedTerminal.transform.position, 3f))
			{
				return;
			}
			sicombinedTerminal.PlayerHandScanned(info.Sender.ActorNumber);
			return;
		}
		case 2:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			int num5;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num5))
			{
				return;
			}
			if (num5 < 0 || num5 >= this.zoneSuperInfection.siDeposits.Length)
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (gameEntityFromNetId == null)
			{
				return;
			}
			SIResourceDeposit siresourceDeposit = this.zoneSuperInfection.siDeposits[num5];
			if ((gameEntityFromNetId.transform.position - siresourceDeposit.transform.position).IsLongerThan(3f))
			{
				return;
			}
			SIResource component = gameEntityFromNetId.GetComponent<SIResource>();
			if (component != null)
			{
				siresourceDeposit.ResourceDeposited(component);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToAuthorityRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId3))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data3;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data3))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(netId3);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToAuthorityRPC(info, rpcID2, data3);
			}
			break;
		}
		default:
			return;
		}
	}

	[PunRPC]
	public void SIAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.authorityToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessAuthorityToClientRPC(authorityToClientRPCEnum, data, info);
	}

	public void ProcessAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (authorityToClientRPCEnum)
		{
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (!gameEntityFromNetId)
			{
				return;
			}
			SIGadget component = gameEntityFromNetId.GetComponent<SIGadget>();
			if (component)
			{
				component.ProcessAuthorityToClientRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data2;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data2))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessAuthorityToClientRPC(info, rpcID2, data2);
				return;
			}
			break;
		}
		case 5:
		{
			if (data.Length != 1)
			{
				return;
			}
			Vector3 position;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out position))
			{
				return;
			}
			if (SIPlayer.LocalPlayer)
			{
				SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(position);
				return;
			}
			break;
		}
		case 6:
			break;
		default:
			return;
		}
	}

	[PunRPC]
	public void SIClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToClientRPC(clientToClientRPCEnum, data, info);
	}

	public void ProcessClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToClientRPCEnum)
		{
		case 0:
		{
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			if (data.Length != 8)
			{
				return;
			}
			int[] resourceArray;
			if (!GameEntityManager.ValidateDataType<int[]>(data[0], out resourceArray))
			{
				return;
			}
			int[] limitedDepositTimeArray;
			if (!GameEntityManager.ValidateDataType<int[]>(data[1], out limitedDepositTimeArray))
			{
				return;
			}
			bool[][] techTreeData;
			if (!GameEntityManager.ValidateDataType<bool[][]>(data[2], out techTreeData))
			{
				return;
			}
			int stashedQuests;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out stashedQuests))
			{
				return;
			}
			int stashedBonusPoints;
			if (!GameEntityManager.ValidateDataType<int>(data[4], out stashedBonusPoints))
			{
				return;
			}
			int bonusProgress;
			if (!GameEntityManager.ValidateDataType<int>(data[5], out bonusProgress))
			{
				return;
			}
			int[] currentQuestIds;
			if (!GameEntityManager.ValidateDataType<int[]>(data[6], out currentQuestIds))
			{
				return;
			}
			int[] currentQuestProgresses;
			if (!GameEntityManager.ValidateDataType<int[]>(data[7], out currentQuestProgresses))
			{
				return;
			}
			siplayer.UpdateProgression(resourceArray, limitedDepositTimeArray, techTreeData, stashedQuests, stashedBonusPoints, bonusProgress, currentQuestIds, currentQuestProgresses);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.RefreshStations(info.Sender.ActorNumber);
				return;
			}
			break;
		}
		case 1:
		{
			if (data.Length != 5)
			{
				return;
			}
			if (SIPlayer.Get(info.Sender.ActorNumber) == null)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			Vector3 velocity;
			if (GameEntityManager.ValidateDataType<Vector3>(data[1], out velocity))
			{
				float num = 10000f;
				if (velocity.IsValid(num))
				{
					Vector3 angVelocity;
					if (GameEntityManager.ValidateDataType<Vector3>(data[2], out angVelocity))
					{
						num = 10000f;
						if (angVelocity.IsValid(num))
						{
							Vector3 targetPosition;
							if (GameEntityManager.ValidateDataType<Vector3>(data[3], out targetPosition))
							{
								num = 10000f;
								if (targetPosition.IsValid(num))
								{
									Quaternion targetRotation;
									if (!GameEntityManager.ValidateDataType<Quaternion>(data[4], out targetRotation) || !targetRotation.IsValid())
									{
										return;
									}
									GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
									if (gameEntityFromNetId == null)
									{
										return;
									}
									if (gameEntityFromNetId.heldByActorNumber != info.Sender.ActorNumber && gameEntityFromNetId.snappedByActorNumber != info.Sender.ActorNumber)
									{
										return;
									}
									SIGadgetDashYoyo component = gameEntityFromNetId.GetComponent<SIGadgetDashYoyo>();
									if (component == null)
									{
										return;
									}
									component.RemoteThrowYoYoTarget(velocity, angVelocity, targetPosition, targetRotation);
									return;
								}
							}
							return;
						}
					}
					return;
				}
			}
			return;
		}
		case 2:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToClientRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId3))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data2;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data2))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(netId3);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToClientRPC(info, rpcID2, data2);
			}
			break;
		}
		default:
			return;
		}
	}

	[ContextMenu("Spawn Debug Object")]
	private void TestSpawnGadget()
	{
		this.testSpawner.Spawn(this.gameEntityManager);
	}

	public IEnumerable<GameEntity> GetFactoryItems()
	{
		return this.techTreeSO.SpawnableEntities;
	}

	private void OnEntityRemoved(GameEntity entity)
	{
		SIGadget sigadget;
		entity.TryGetComponent<SIGadget>(out sigadget);
		if (this.zoneSuperInfection != null && sigadget != null)
		{
			this.zoneSuperInfection.RemoveGadget(sigadget);
		}
		if (sigadget == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)-1)));
		if (siplayer != null && siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
		{
			Debug.Log(string.Format("GadgetDebug: removing gadget grom list {0} {1}", siplayer.gameObject.name, entity.GetNetId()));
			siplayer.activePlayerGadgets.Remove(entity.GetNetId());
		}
	}

	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		if (entity.GetComponent<SIGadget>() == null)
		{
			return createData;
		}
		return createData >> 32 << 32 | (long)SIPlayer.LocalPlayer.ActorNr;
	}

	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		GameObject gameObject = this.gameEntityManager.FactoryPrefabById(entityTypeId);
		if (gameObject == null)
		{
			return false;
		}
		if (gameObject.GetComponent<SIGadget>() == null)
		{
			return false;
		}
		SIPlayer siplayer = SIPlayer.Get(actorNr);
		if (siplayer == null)
		{
			return false;
		}
		SIPlayer y = SIPlayer.Get((int)(createData & (long)((ulong)-1)));
		if (siplayer != y)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < siplayer.activePlayerGadgets.Count; i++)
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
			if (((gameEntityFromNetId != null) ? gameEntityFromNetId.GetComponent<SIGadget>() : null) != null)
			{
				num++;
			}
		}
		if (num > siplayer.totalGadgetLimit)
		{
			return false;
		}
		bool flag = false;
		int num2 = 0;
		while (num2 < siplayer.CurrentProgression.techTreeData.Length && !flag)
		{
			for (int j = 0; j < siplayer.CurrentProgression.techTreeData[num2].Length; j++)
			{
				if (siplayer.CurrentProgression.techTreeData[num2][j] && siplayer.progressionSORef.IsValidNode(num2, j))
				{
					SITechTreeNode treeNode = siplayer.progressionSORef.GetTreeNode(num2, j);
					if (treeNode != null && treeNode.IsDispensableGadget && treeNode.unlockedGadgetPrefab.gameObject.name.GetStaticHash() == entityTypeId)
					{
						flag = true;
						break;
					}
				}
			}
			num2++;
		}
		return flag;
	}

	public void ClearPlayerGadgets(SIPlayer siPlayer)
	{
		for (int i = siPlayer.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			if (i < siPlayer.activePlayerGadgets.Count && siPlayer.activePlayerGadgets[i] >= 0)
			{
				GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siPlayer.activePlayerGadgets[i]);
				if (!(gameEntityFromNetId == null) && !(gameEntityFromNetId.id == GameEntityId.Invalid))
				{
					this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
				}
			}
		}
		siPlayer.activePlayerGadgets.Clear();
	}

	[CompilerGenerated]
	private void <OnZoneInit>g__WhenReady|39_0()
	{
		this.progression.OnClientReady -= this.<OnZoneInit>g__WhenReady|39_0;
		SIPlayer.SetAndBroadcastProgression();
	}

	private const string preLog = "[GT/SuperInfectionManager]  ";

	private const string preErr = "[GT/SuperInfectionManager]  ERROR!!!  ";

	public GameEntityManager gameEntityManager;

	public TestSpawnGadget testSpawner;

	public PhotonView photonView;

	public XSceneRef zoneSuperInfectionRef;

	[NonSerialized]
	public SuperInfection zoneSuperInfection;

	[SerializeField]
	private SITechTreeSO techTreeSO;

	[SerializeField]
	private SIProgression progression;

	[DebugReadout]
	public static SuperInfectionManager activeSuperInfectionManager;

	public static Dictionary<GTZone, SuperInfectionManager> siManagerByZone = new Dictionary<GTZone, SuperInfectionManager>();

	private static List<VRRig> tempRigs = new List<VRRig>(10);

	private static List<VRRig> tempRigs2 = new List<VRRig>(10);

	private readonly Dictionary<SnapJointType, List<SuperInfectionSnapPoint>> allSnapPoints = new Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>();

	private const float rpcProximityCheckRange = 3f;

	private bool PendingZoneInit;

	public enum ClientToAuthorityRPC
	{
		CombinedTerminalButtonPress,
		CombinedTerminalHandScan,
		ResourceDepositDeposited,
		CallEntityRPC,
		CallEntityRPCData
	}

	public enum AuthorityToClientRPC
	{
		TechPointGranted,
		ResourceDepositTechPointGranted,
		ResourceDepositTechPointRejected,
		CallEntityRPC,
		CallEntityRPCData,
		TriggerMonkeIdolDepositCelebration,
		StartUnderwaterFX
	}

	public enum ClientToClientRPC
	{
		BroadcastProgression,
		LaunchDashYoyo,
		CallEntityRPC,
		CallEntityRPCData
	}
}
