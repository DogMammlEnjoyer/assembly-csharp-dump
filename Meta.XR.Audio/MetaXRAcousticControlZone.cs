using System;
using System.Collections.Generic;
using Meta.XR.Acoustics;
using UnityEngine;

internal sealed class MetaXRAcousticControlZone : MonoBehaviour
{
	internal MetaXRAcousticControlZone.State state
	{
		get
		{
			return this._state;
		}
	}

	internal Color ZoneColor
	{
		get
		{
			return this._state.color;
		}
		set
		{
			this._state.color = value;
		}
	}

	internal Spectrum Rt60
	{
		get
		{
			return this._state.rt60;
		}
		set
		{
			this._state.rt60 = value;
		}
	}

	internal Spectrum ReverbLevel
	{
		get
		{
			return this._state.reverbLevel;
		}
		set
		{
			this._state.reverbLevel = value;
		}
	}

	internal float FadeDistance
	{
		get
		{
			return this._state.fadeDistance;
		}
		set
		{
			this._state.fadeDistance = value;
			this.ApplyTransform();
		}
	}

	private Vector3 NativeFadeDistance
	{
		get
		{
			return new Vector3(this._state.fadeDistance / base.transform.localScale.x, this._state.fadeDistance / base.transform.localScale.y, this._state.fadeDistance / base.transform.localScale.z);
		}
	}

	private Vector3 NativeBoxSize
	{
		get
		{
			return new Vector3(2f + this.NativeFadeDistance.x, 2f + this.NativeFadeDistance.y, 2f + this.NativeFadeDistance.z);
		}
	}

	internal void Clone(MetaXRAcousticControlZone.State other)
	{
		this._state.Clone(other);
	}

	internal MetaXRAcousticControlZone()
	{
		this.Rt60.points = new List<Spectrum.Point>
		{
			new Spectrum.Point(1000f, 0f)
		};
		this.ReverbLevel.points = new List<Spectrum.Point>
		{
			new Spectrum.Point(1000f, 0f)
		};
	}

	private void Start()
	{
		this.StartInternal();
	}

	internal void StartInternal()
	{
		if (this._controlHandle != IntPtr.Zero)
		{
			return;
		}
		if (MetaXRAcousticNativeInterface.Interface.CreateControlZone(out this._controlHandle) != 0)
		{
			Debug.LogError("Unable to create internal Control Zone", base.gameObject);
			return;
		}
		this.ApplyProperties();
	}

	private void OnDestroy()
	{
		this.DestroyInternal();
	}

	internal void DestroyInternal()
	{
		if (this._controlHandle != IntPtr.Zero)
		{
			MetaXRAcousticNativeInterface.Interface.DestroyControlZone(this._controlHandle);
			this._controlHandle = IntPtr.Zero;
		}
	}

	private void OnEnable()
	{
		if (this._controlHandle == IntPtr.Zero)
		{
			return;
		}
		MetaXRAcousticNativeInterface.Interface.ControlZoneSetEnabled(this._controlHandle, true);
	}

	private void OnDisable()
	{
		if (this._controlHandle == IntPtr.Zero)
		{
			return;
		}
		MetaXRAcousticNativeInterface.Interface.ControlZoneSetEnabled(this._controlHandle, false);
	}

	private void LateUpdate()
	{
		if (this._controlHandle == IntPtr.Zero)
		{
			return;
		}
		if (base.transform.hasChanged)
		{
			this.ApplyTransform();
			base.transform.hasChanged = false;
		}
	}

	private void ApplyTransform()
	{
		if (this._controlHandle == IntPtr.Zero)
		{
			return;
		}
		MetaXRAcousticNativeInterface.Interface.ControlZoneSetBox(this._controlHandle, this.NativeBoxSize.x, this.NativeBoxSize.y, this.NativeBoxSize.z);
		MetaXRAcousticNativeInterface.Interface.ControlZoneSetFadeDistance(this._controlHandle, this.NativeFadeDistance.x, this.NativeFadeDistance.y, this.NativeFadeDistance.z);
		MetaXRAcousticNativeInterface.INativeInterface @interface = MetaXRAcousticNativeInterface.Interface;
		IntPtr controlHandle = this._controlHandle;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		@interface.ControlZoneSetTransform(controlHandle, localToWorldMatrix);
	}

	internal void ApplyProperties()
	{
		if (this._controlHandle == IntPtr.Zero)
		{
			return;
		}
		this.ApplyTransform();
		MetaXRAcousticNativeInterface.Interface.ControlZoneReset(this._controlHandle, ControlZoneProperty.RT60);
		foreach (Spectrum.Point point in this.Rt60.points)
		{
			MetaXRAcousticNativeInterface.Interface.ControlZoneSetFrequency(this._controlHandle, ControlZoneProperty.RT60, point.frequency, point.data);
		}
		MetaXRAcousticNativeInterface.Interface.ControlZoneReset(this._controlHandle, ControlZoneProperty.REVERB_LEVEL);
		foreach (Spectrum.Point point2 in this.ReverbLevel.points)
		{
			MetaXRAcousticNativeInterface.Interface.ControlZoneSetFrequency(this._controlHandle, ControlZoneProperty.REVERB_LEVEL, point2.frequency, point2.data);
		}
	}

	[SerializeField]
	private MetaXRAcousticControlZone.State _state = new MetaXRAcousticControlZone.State();

	private IntPtr _controlHandle = IntPtr.Zero;

	[Serializable]
	internal class State
	{
		internal void Clone(MetaXRAcousticControlZone.State other)
		{
			this.color = other.color;
			this.reverbLevel.Clone(other.reverbLevel);
			this.rt60.Clone(other.rt60);
			this.fadeDistance = other.fadeDistance;
		}

		[SerializeField]
		internal Color color = Color.blue;

		[SerializeField]
		internal Spectrum rt60 = new Spectrum(null);

		[SerializeField]
		internal Spectrum reverbLevel = new Spectrum(null);

		[SerializeField]
		internal float fadeDistance = 1f;
	}
}
