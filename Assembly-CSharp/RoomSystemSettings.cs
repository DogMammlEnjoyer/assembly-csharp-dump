using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTag;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	public ExpectedUsersDecayTimer ExpectedUsersTimer
	{
		get
		{
			return this.expectedUsersTimer;
		}
	}

	public TickSystemTimer ResyncNetworkTimeTimer
	{
		get
		{
			return this.resyncNetworkTimeTimer;
		}
	}

	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	public CallLimiterWithCooldown PlayerEffectLimiter
	{
		get
		{
			return this.playerEffectLimiter;
		}
	}

	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	public List<RoomSystem.PlayerEffectConfig> PlayerEffects
	{
		get
		{
			return this.playerEffects;
		}
	}

	public int PausedDCTimer
	{
		get
		{
			return this.pausedDCTimer;
		}
	}

	public int GetRoomCount(bool privateRoom, bool sub)
	{
		if (privateRoom)
		{
			if (!sub)
			{
				return this.privateRoomCountZoneModeMapping.GetRoomCount();
			}
			return this.subsPrivateRoomCountZoneModeMapping.GetRoomCount();
		}
		else
		{
			if (!sub)
			{
				return this.publicRoomCountZoneModeMapping.GetRoomCount();
			}
			return this.subsPublicRoomCountZoneModeMapping.GetRoomCount();
		}
	}

	public int GetRoomCount(GTZone zone, GameModeType mode, bool privateRoom, bool sub)
	{
		if (privateRoom)
		{
			if (!sub)
			{
				return this.privateRoomCountZoneModeMapping.GetRoomCount(zone, mode);
			}
			return this.subsPrivateRoomCountZoneModeMapping.GetRoomCount(zone, mode);
		}
		else
		{
			if (!sub)
			{
				return this.publicRoomCountZoneModeMapping.GetRoomCount(zone, mode);
			}
			return this.subsPublicRoomCountZoneModeMapping.GetRoomCount(zone, mode);
		}
	}

	[SerializeField]
	private ExpectedUsersDecayTimer expectedUsersTimer;

	[SerializeField]
	private TickSystemTimer resyncNetworkTimeTimer;

	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	[SerializeField]
	private CallLimiterWithCooldown playerEffectLimiter;

	[SerializeField]
	private GameObject playerImpactEffect;

	[SerializeField]
	private List<RoomSystem.PlayerEffectConfig> playerEffects = new List<RoomSystem.PlayerEffectConfig>();

	[SerializeField]
	private int pausedDCTimer;

	[SerializeField]
	private RoomCount publicRoomCountZoneModeMapping;

	[SerializeField]
	private PrivateRoomCount privateRoomCountZoneModeMapping;

	[SerializeField]
	private RoomCount subsPublicRoomCountZoneModeMapping;

	[SerializeField]
	private PrivateRoomCount subsPrivateRoomCountZoneModeMapping;
}
