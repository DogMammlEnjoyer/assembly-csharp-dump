using System;
using UnityEngine.Events;

public class GenericObservable : ObservableBehavior
{
	protected override void ObservableSliceUpdate()
	{
	}

	protected override void OnBecameObservable()
	{
		UnityEvent onObservable = this.OnObservable;
		if (onObservable == null)
		{
			return;
		}
		onObservable.Invoke();
	}

	protected override void OnLostObservable()
	{
		UnityEvent onUnobservable = this.OnUnobservable;
		if (onUnobservable == null)
		{
			return;
		}
		onUnobservable.Invoke();
	}

	public UnityEvent OnObservable;

	public UnityEvent OnUnobservable;
}
