using System;
using UnityEngine;
using UnityEngine.Events;

public class GenericCounter : MonoBehaviour
{
	public void CountUp()
	{
		this.currentCount++;
		this.DoCallbacks();
	}

	public void CountDown()
	{
		this.currentCount--;
		this.DoCallbacks();
	}

	private void DoCallbacks()
	{
		if (this.currentCount < this.Threshold)
		{
			this.whenLessThan.Invoke();
			return;
		}
		if (this.currentCount == this.Threshold)
		{
			this.whenEqual.Invoke();
			return;
		}
		this.whenGreaterThan.Invoke();
	}

	public void ResetCounter()
	{
		this.currentCount = 0;
	}

	[SerializeField]
	private int Threshold;

	[SerializeField]
	private UnityEvent whenLessThan;

	[SerializeField]
	private UnityEvent whenEqual;

	[SerializeField]
	private UnityEvent whenGreaterThan;

	private int currentCount;
}
