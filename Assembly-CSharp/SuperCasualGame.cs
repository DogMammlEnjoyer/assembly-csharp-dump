using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

public sealed class SuperCasualGame : GorillaGameManager
{
	public override int MyMatIndex(NetPlayer player)
	{
		return 0;
	}

	public override void OnSerializeRead(object newData)
	{
	}

	public override object OnSerializeWrite()
	{
		return null;
	}

	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	public override GameModeType GameType()
	{
		return GameModeType.SuperCasual;
	}

	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	public override string GameModeName()
	{
		return "SUPER CASUAL";
	}

	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_SUPER_CASUAL_ROOM_LABEL", out result, "(SUPER CASUAL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_SUPER_CASUAL_ROOM_LABEL]");
		}
		return result;
	}

	public override void StartPlaying()
	{
		base.StartPlaying();
		VRRig.LocalRig.EnableSuperInfectionHands(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableSuperInfectionHands(true);
			}
		}
	}

	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableSuperInfectionHands(false);
	}

	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableSuperInfectionHands(true);
		}
	}
}
