using System;

public readonly struct OVRSharable : IOVRAnchorComponent<OVRSharable>, IEquatable<OVRSharable>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRSharable>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRSharable>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRSharable IOVRAnchorComponent<OVRSharable>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRSharable(anchor);
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

	public bool Equals(OVRSharable other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRSharable lhs, OVRSharable rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRSharable lhs, OVRSharable rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRSharable)
		{
			OVRSharable other = (OVRSharable)obj;
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
		return string.Format("{0}.Sharable", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.Sharable;
		}
	}

	internal ulong Handle { get; }

	private OVRSharable(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public static readonly OVRSharable Null;
}
