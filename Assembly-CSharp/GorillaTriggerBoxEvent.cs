using System;
using UnityEngine.Events;

public class GorillaTriggerBoxEvent : GorillaTriggerBox
{
	public override void OnBoxTriggered()
	{
		UnityEvent unityEvent = this.onBoxTriggered;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	public override void OnBoxExited()
	{
		UnityEvent unityEvent = this.onBoxExited;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	public UnityEvent onBoxTriggered;

	public UnityEvent onBoxExited;
}
