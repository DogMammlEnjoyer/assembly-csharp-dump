using System;
using UnityEngine;

public class CameraShakeDispatcher : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.shakeOnEnable)
		{
			if (this.maxDistance > 0f)
			{
				this.ShakeInProximity(this.maxDistance);
				return;
			}
			this.Shake();
		}
	}

	private void OnDisable()
	{
		if (this.haltOnDisable)
		{
			this.Halt();
		}
	}

	public void Shake()
	{
		CameraShaker.Shake(this.duration, this.magnitude, this.freqRange, this.rollOffOverDuration);
	}

	public void ShakeInProximity(float distance)
	{
		CameraShaker.ShakeInProximity(this.duration, this.magnitude, this.freqRange, this.rollOffOverDuration, base.transform, distance);
	}

	public void Halt()
	{
		CameraShaker.Halt();
	}

	[SerializeField]
	private float magnitude = 1f;

	[SerializeField]
	private float duration = 0.5f;

	[SerializeField]
	private bool rollOffOverDuration = true;

	[SerializeField]
	private bool shakeOnEnable;

	[SerializeField]
	private bool haltOnDisable;

	[SerializeField]
	private Vector2 freqRange = new Vector2(0.02f, 0.1f);

	[SerializeField]
	private float maxDistance;
}
