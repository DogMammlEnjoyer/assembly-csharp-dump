using System;
using GorillaGameModes;
using UnityEngine;

[Serializable]
internal class RoomCount : PrivateRoomCount
{
	public int GetRoomCount(GTZone zone)
	{
		for (int i = 0; i < this.zoneCountOverrides.Length; i++)
		{
			if (this.zoneCountOverrides[i].Zone == zone)
			{
				return this.zoneCountOverrides[i].Count;
			}
		}
		return this.count;
	}

	public override int GetRoomCount(GTZone zone, GameModeType mode)
	{
		return Mathf.Min(this.GetRoomCount(zone), base.GetRoomCount(mode));
	}

	[SerializeField]
	private RoomCountForZone[] zoneCountOverrides;
}
