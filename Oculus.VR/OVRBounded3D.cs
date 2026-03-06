using System;
using UnityEngine;

public readonly struct OVRBounded3D : IOVRAnchorComponent<OVRBounded3D>, IEquatable<OVRBounded3D>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRBounded3D>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRBounded3D>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRBounded3D IOVRAnchorComponent<OVRBounded3D>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRBounded3D(anchor);
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

	OVRTask<bool> IOVRAnchorComponent<OVRBounded3D>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The Bounded3D component cannot be enabled or disabled.");
	}

	public bool Equals(OVRBounded3D other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRBounded3D lhs, OVRBounded3D rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRBounded3D lhs, OVRBounded3D rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRBounded3D)
		{
			OVRBounded3D other = (OVRBounded3D)obj;
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
		return string.Format("{0}.Bounded3D", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.Bounded3D;
		}
	}

	internal ulong Handle { get; }

	private OVRBounded3D(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public Bounds BoundingBox
	{
		get
		{
			OVRPlugin.Boundsf openXrBounds;
			if (!OVRPlugin.GetSpaceBoundingBox3D(this.Handle, out openXrBounds))
			{
				throw new InvalidOperationException("Could not get BoundingBox");
			}
			return this.ConvertBounds(openXrBounds);
		}
	}

	private Bounds ConvertBounds(OVRPlugin.Boundsf openXrBounds)
	{
		Vector3 vector = openXrBounds.Size.FromSize3f();
		Vector3 a = openXrBounds.Pos.FromFlippedXVector3f();
		a.x -= vector.x;
		Vector3 b = vector * 0.5f;
		return new Bounds(a + b, vector);
	}

	public static readonly OVRBounded3D Null;
}
