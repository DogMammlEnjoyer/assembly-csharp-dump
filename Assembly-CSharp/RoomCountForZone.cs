using System;
using UnityEngine;

[Serializable]
internal class RoomCountForZone
{
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	[SerializeField]
	private GTZone zone;

	[SerializeField]
	private int count;
}
