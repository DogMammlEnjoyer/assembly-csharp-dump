using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerTimeSyncRule", menuName = "Scriptable Objects/ServerTimeSyncRule")]
public class ServerTimeSyncRule : ScriptableObject
{
	public DateTime GetPrevious(DateTime dt)
	{
		DateTime result = DateTime.MinValue;
		switch (this.unit)
		{
		case ServerTimeSyncRule.Unit.Hours:
			result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
			result = result.AddHours((double)(-(double)(dt.Hour % this.value)));
			break;
		case ServerTimeSyncRule.Unit.Minutes:
			result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
			result = result.AddMinutes((double)(-(double)(dt.Minute % this.value)));
			break;
		case ServerTimeSyncRule.Unit.Seconds:
			result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			result = result.AddSeconds((double)(-(double)(dt.Second % this.value)));
			break;
		}
		return result;
	}

	public DateTime GetNext(DateTime dt)
	{
		DateTime result = DateTime.MaxValue;
		switch (this.unit)
		{
		case ServerTimeSyncRule.Unit.Hours:
			result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
			result = result.AddHours((double)(this.value - dt.Hour % this.value));
			break;
		case ServerTimeSyncRule.Unit.Minutes:
			result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
			result = result.AddMinutes((double)(this.value - dt.Minute % this.value));
			break;
		case ServerTimeSyncRule.Unit.Seconds:
			result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			result = result.AddSeconds((double)(this.value - dt.Second % this.value));
			break;
		}
		return result;
	}

	[SerializeField]
	private ServerTimeSyncRule.Unit unit;

	[SerializeField]
	private int value;

	private enum Unit
	{
		Hours,
		Minutes,
		Seconds
	}
}
