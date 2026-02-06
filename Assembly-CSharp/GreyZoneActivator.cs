using System;
using UnityEngine;

public class GreyZoneActivator : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.Activate();
		}
	}

	private void OnDisable()
	{
		if (this.deactivateOnDisable)
		{
			this.Deactivate();
		}
	}

	public void Activate()
	{
		GreyZoneManager.Instance.LocalSimpleActivation(true, this.gMultiplier);
	}

	public void ActivateWithG(float g)
	{
		GreyZoneManager.Instance.LocalSimpleActivation(true, g);
	}

	public void Deactivate()
	{
		GreyZoneManager.Instance.LocalSimpleActivation(false, 1f);
	}

	[SerializeField]
	private bool activateOnEnable;

	[SerializeField]
	private bool deactivateOnDisable;

	[Range(-5f, 5f)]
	[SerializeField]
	private float gMultiplier = 1f;
}
