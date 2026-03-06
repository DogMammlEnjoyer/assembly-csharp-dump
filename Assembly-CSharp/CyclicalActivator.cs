using System;
using GorillaNetworking;
using UnityEngine;

public class CyclicalActivator : MonoBehaviour, IGorillaSliceableSimple
{
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		float nowSeconds = (float)(serverTime.Minute * 60) + ((float)serverTime.Second + (float)serverTime.Millisecond * 0.001f);
		for (int i = 0; i < this.objects.Length; i++)
		{
			this.objects[i].gameObject.SetActive(this.objects[i].schedule.CheckTime(nowSeconds));
		}
	}

	[SerializeField]
	private CyclicalActivator.CyclicalActivatorObject[] objects;

	[Serializable]
	private class CyclicalActivatorObjectScheduleNode
	{
		public Vector2 secondsActiveRange;
	}

	[Serializable]
	private class CyclicalActivatorObjectSchedule
	{
		public bool CheckTime(float nowSeconds)
		{
			nowSeconds %= (float)this.totalSeconds;
			for (int i = 0; i < this.schedule.Length; i++)
			{
				if (this.schedule[i].secondsActiveRange.x <= nowSeconds && this.schedule[i].secondsActiveRange.y > nowSeconds)
				{
					return true;
				}
			}
			return false;
		}

		[Range(10f, 3599f)]
		public int totalSeconds = 60;

		public CyclicalActivator.CyclicalActivatorObjectScheduleNode[] schedule;
	}

	[Serializable]
	private class CyclicalActivatorObject
	{
		public GameObject gameObject;

		public CyclicalActivator.CyclicalActivatorObjectSchedule schedule;
	}
}
