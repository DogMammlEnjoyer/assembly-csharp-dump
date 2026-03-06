using System;
using System.Collections.Generic;

public readonly struct OVRAnchorContainer : IOVRAnchorComponent<OVRAnchorContainer>, IEquatable<OVRAnchorContainer>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRAnchorContainer>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRAnchorContainer>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRAnchorContainer IOVRAnchorComponent<OVRAnchorContainer>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRAnchorContainer(anchor);
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

	OVRTask<bool> IOVRAnchorComponent<OVRAnchorContainer>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The AnchorContainer component cannot be enabled or disabled.");
	}

	public bool Equals(OVRAnchorContainer other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRAnchorContainer lhs, OVRAnchorContainer rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRAnchorContainer lhs, OVRAnchorContainer rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRAnchorContainer)
		{
			OVRAnchorContainer other = (OVRAnchorContainer)obj;
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
		return string.Format("{0}.AnchorContainer", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.SpaceContainer;
		}
	}

	internal ulong Handle { get; }

	private OVRAnchorContainer(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public Guid[] Uuids
	{
		get
		{
			Guid[] result;
			if (!OVRPlugin.GetSpaceContainer(this.Handle, out result))
			{
				throw new InvalidOperationException("Could not get Uuids");
			}
			return result;
		}
	}

	[Obsolete("Use FetchAnchorsAsync instead")]
	public OVRTask<bool> FetchChildrenAsync(List<OVRAnchor> anchors)
	{
		return OVRAnchor.FetchAnchorsAsync(this.Uuids, anchors, OVRSpace.StorageLocation.Local, 0.0);
	}

	public OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchAnchorsAsync(List<OVRAnchor> anchors)
	{
		return OVRAnchor.FetchAnchorsAsync(anchors, new OVRAnchor.FetchOptions
		{
			Uuids = this.Uuids
		}, null);
	}

	public static readonly OVRAnchorContainer Null;
}
