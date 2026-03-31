using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class DebugAutoNamePlayer : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
	}

	private void OnRoomJoined()
	{
		this.m_lastZone = DebugAutoNamePlayer.GetPrimaryZone();
		this.m_lastIsZoneAuthority = DebugAutoNamePlayer.GetIsZoneAuthority(this.m_lastZone);
		this.m_joinDelayTimer = 2f;
		this.ApplyAutoName();
	}

	private void OnPlayersChanged()
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		this.ApplyAutoName();
	}

	private void OnZoneChange(ZoneData[] zones)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		this.m_lastZone = DebugAutoNamePlayer.GetPrimaryZone();
		this.m_lastIsZoneAuthority = DebugAutoNamePlayer.GetIsZoneAuthority(this.m_lastZone);
		this.ApplyAutoName();
	}

	private void ApplyAutoName()
	{
		string platformCode = DebugAutoNamePlayer.GetPlatformCode();
		int localPlayerID = NetworkSystem.Instance.LocalPlayerID;
		string text = NetworkSystem.Instance.IsMasterClient ? "MC" : "C";
		GTZone primaryZone = DebugAutoNamePlayer.GetPrimaryZone();
		string text2 = DebugAutoNamePlayer.GetIsZoneAuthority(primaryZone) ? "ZA" : "Z";
		string text3 = primaryZone.ToString().ToUpper();
		string text4 = string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
		{
			platformCode,
			localPlayerID,
			text,
			text2,
			text3
		});
		if (text4.Length > 20)
		{
			text4 = text4.Substring(0, 20);
		}
		NetworkSystem.Instance.SetMyNickName(text4);
		if (GorillaComputer.instance != null)
		{
			GorillaComputer.instance.currentName = text4;
			GorillaComputer.instance.savedName = text4;
			GorillaComputer.instance.SetLocalNameTagText(text4);
		}
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				PlayerPrefs.GetFloat("redValue", 0f),
				PlayerPrefs.GetFloat("greenValue", 0f),
				PlayerPrefs.GetFloat("blueValue", 0f)
			});
		}
	}

	private static GTZone GetPrimaryZone()
	{
		ZoneManagement instance = ZoneManagement.instance;
		if (instance != null && instance.activeZones.Count > 0)
		{
			return instance.activeZones[0];
		}
		return GTZone.forest;
	}

	private static bool GetIsZoneAuthority(GTZone zone)
	{
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(zone);
		if (managerForZone == null)
		{
			return false;
		}
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		return localPlayer != null && managerForZone.IsAuthorityPlayer(localPlayer);
	}

	private static string GetPlatformCode()
	{
		return "ST";
	}

	private float m_authorityPollTimer;

	private float m_joinDelayTimer;

	private bool m_lastIsZoneAuthority;

	private GTZone m_lastZone;
}
