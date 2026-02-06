using System;
using GorillaGameModes;
using UnityEngine;

[Serializable]
internal class RoomCountForMode
{
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public GameModeType Mode
	{
		get
		{
			return this.mode;
		}
	}

	[SerializeField]
	private GameModeType mode;

	[SerializeField]
	private int count;
}
