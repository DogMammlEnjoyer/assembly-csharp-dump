using System;
using GorillaGameModes;
using UnityEngine;

[Serializable]
internal class PrivateRoomCount
{
	public int GetRoomCount()
	{
		return this.count;
	}

	public int GetRoomCount(GameModeType mode)
	{
		for (int i = 0; i < this.modeCountOverrides.Length; i++)
		{
			if (this.modeCountOverrides[i].Mode == mode)
			{
				return this.modeCountOverrides[i].Count;
			}
		}
		return this.count;
	}

	public virtual int GetRoomCount(GTZone zone, GameModeType mode)
	{
		return this.GetRoomCount(mode);
	}

	[SerializeField]
	protected int count;

	[SerializeField]
	protected RoomCountForMode[] modeCountOverrides;
}
