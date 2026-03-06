using System;

public readonly struct OVRStorable : IOVRAnchorComponent<OVRStorable>, IEquatable<OVRStorable>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRStorable>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRStorable>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRStorable IOVRAnchorComponent<OVRStorable>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRStorable(anchor);
	}

	public bool IsNull
	{
		get
		{
			return this.Handle == 0UL;
		}
	}

	public bool IsEnabled
	{
		get
		{
			bool flag;
			bool flag2;
			return !this.IsNull && OVRPlugin.GetSpaceComponentStatus(this.Handle, this.Type, out flag, out flag2) && flag && !flag2;
		}
	}

	public OVRTask<bool> SetEnabledAsync(bool enabled, double timeout = 0.0)
	{
		bool flag;
		bool flag2;
		if (!OVRPlugin.GetSpaceComponentStatus(this.Handle, this.Type, out flag, out flag2))
		{
			return OVRTask.FromResult<bool>(false);
		}
		if (flag2)
		{
			return OVRAnchor.CreateDeferredSpaceComponentStatusTask(this.Handle, this.Type, enabled, timeout);
		}
		if (flag != enabled)
		{
			ulong requestId;
			return OVRTask.Build(OVRPlugin.SetSpaceComponentStatus(this.Handle, this.Type, enabled, timeout, out requestId), requestId).ToTask<bool>(false);
		}
		return OVRTask.FromResult<bool>(true);
	}

	[Obsolete("Use SetEnabledAsync instead.")]
	public OVRTask<bool> SetEnabledSafeAsync(bool enabled, double timeout = 0.0)
	{
		return this.SetEnabledAsync(enabled, timeout);
	}

	public bool Equals(OVRStorable other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRStorable lhs, OVRStorable rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRStorable lhs, OVRStorable rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRStorable)
		{
			OVRStorable other = (OVRStorable)obj;
			return this.Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return this.Handle.GetHashCode() * 486187739 + ((int)this.Type).GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}.Storable", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.Storable;
		}
	}

	internal ulong Handle { get; }

	private OVRStorable(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public static readonly OVRStorable Null;
}
