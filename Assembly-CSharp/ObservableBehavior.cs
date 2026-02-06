using System;
using UnityEngine;

public abstract class ObservableBehavior : MonoBehaviour, IGorillaSliceableSimple
{
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.UnityOnEnable();
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (this.observable)
		{
			this.observable = false;
			this.OnLostObservable();
		}
		this.UnityOnDisable();
	}

	private void OnDestroy()
	{
		if (this.observable)
		{
			this.observable = false;
			this.OnLostObservable();
		}
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		Transform transform = Camera.main.transform;
		Vector3.Distance(transform.position, base.transform.position);
		Vector3.Dot((transform.position - base.transform.position).normalized, transform.transform.forward);
		bool flag = true;
		if (this.firstFrame || this.observable != flag)
		{
			if (flag)
			{
				this.OnBecameObservable();
			}
			else
			{
				this.OnLostObservable();
			}
		}
		this.observable = flag;
		this.firstFrame = false;
		if (flag)
		{
			this.ObservableSliceUpdate();
		}
	}

	protected virtual void UnityOnEnable()
	{
	}

	protected virtual void UnityOnDisable()
	{
	}

	protected abstract void OnLostObservable();

	protected abstract void OnBecameObservable();

	protected abstract void ObservableSliceUpdate();

	private bool firstFrame = true;

	private bool observable = true;
}
