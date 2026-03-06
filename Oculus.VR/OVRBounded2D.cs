using System;
using Unity.Collections;
using UnityEngine;

public readonly struct OVRBounded2D : IOVRAnchorComponent<OVRBounded2D>, IEquatable<OVRBounded2D>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRBounded2D>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRBounded2D>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRBounded2D IOVRAnchorComponent<OVRBounded2D>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRBounded2D(anchor);
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

	OVRTask<bool> IOVRAnchorComponent<OVRBounded2D>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The Bounded2D component cannot be enabled or disabled.");
	}

	public bool Equals(OVRBounded2D other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRBounded2D lhs, OVRBounded2D rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRBounded2D lhs, OVRBounded2D rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRBounded2D)
		{
			OVRBounded2D other = (OVRBounded2D)obj;
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
		return string.Format("{0}.Bounded2D", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.Bounded2D;
		}
	}

	internal ulong Handle { get; }

	private OVRBounded2D(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public Rect BoundingBox
	{
		get
		{
			OVRPlugin.Rectf openXrRect;
			if (!OVRPlugin.GetSpaceBoundingBox2D(this.Handle, out openXrRect))
			{
				throw new InvalidOperationException("Could not get BoundingBox");
			}
			return this.ConvertRect(openXrRect);
		}
	}

	private Rect ConvertRect(OVRPlugin.Rectf openXrRect)
	{
		Vector2 vector = openXrRect.Size.FromSizef();
		Vector2 position = openXrRect.Pos.FromFlippedXVector2f();
		position.x -= vector.x;
		return new Rect(position, vector);
	}

	public bool TryGetBoundaryPointsCount(out int count)
	{
		return OVRPlugin.GetSpaceBoundary2DCount(this.Handle, out count);
	}

	public bool TryGetBoundaryPoints(NativeArray<Vector2> positions)
	{
		if (!positions.IsCreated)
		{
			throw new ArgumentException("NativeArray is not created", "positions");
		}
		int num;
		if (!OVRPlugin.GetSpaceBoundary2D(this.Handle, positions, out num))
		{
			return false;
		}
		int i = 0;
		int num2 = num - 1;
		while (i <= num2)
		{
			Vector2 vector = positions[num2];
			Vector2 vector2 = positions[i];
			positions[i] = new Vector2(-vector.x, vector.y);
			positions[num2] = new Vector2(-vector2.x, vector2.y);
			i++;
			num2--;
		}
		return true;
	}

	public static readonly OVRBounded2D Null;
}
