using System;
using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	private static event Action<float, float, Vector2, bool> ShakeRequested;

	private static event Action HaltRequested;

	public static void Shake(float duration, float magnitude)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, new Vector2(0.02f, 0.1f), true);
		}
	}

	public static void Shake(float duration, float magnitude, Vector2 freqRange)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, true);
		}
	}

	public static void Shake(float duration, float magnitude, Vector2 freqRange, bool rollOffOverDuration)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, rollOffOverDuration);
		}
	}

	public static void Halt()
	{
		if (CameraShaker.HaltRequested != null)
		{
			CameraShaker.HaltRequested();
		}
	}

	private void OnEnable()
	{
		CameraShaker.ShakeRequested += this._ShakeRequested;
		CameraShaker.HaltRequested += this._HaltRequested;
	}

	private void _ShakeRequested(float _duration, float _magnitude, Vector2 _freqRange, bool _rollOff)
	{
		this.stopTime = Time.time + _duration;
		this.duration = _duration;
		this.magnitude = _magnitude;
		this.freqRange = _freqRange;
		this.rollOff = _rollOff;
		if (!this.rumbling)
		{
			base.StartCoroutine(this.crRumble());
		}
	}

	private void _HaltRequested()
	{
		this.stopTime = Time.time;
	}

	private void OnDisable()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
		CameraShaker.HaltRequested -= this._HaltRequested;
	}

	private void OnDestroy()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
		CameraShaker.HaltRequested -= this._HaltRequested;
	}

	private IEnumerator crRumble()
	{
		this.rumbling = true;
		while (this.stopTime > Time.time)
		{
			Vector3 vector = Random.insideUnitSphere * this.magnitude;
			if (this.rollOff)
			{
				vector *= (this.stopTime - Time.time) / this.duration;
			}
			base.transform.localPosition += vector;
			yield return new WaitForSeconds(Random.Range(this.freqRange.x, this.freqRange.y));
		}
		this.rumbling = false;
		yield break;
	}

	private bool rumbling;

	private float stopTime;

	private bool rollOff;

	private float magnitude;

	private float duration;

	private Vector2 freqRange;
}
