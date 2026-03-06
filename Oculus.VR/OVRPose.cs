using System;
using UnityEngine;

[Serializable]
public struct OVRPose
{
	public static OVRPose identity
	{
		get
		{
			return new OVRPose
			{
				position = Vector3.zero,
				orientation = Quaternion.identity
			};
		}
	}

	public override bool Equals(object obj)
	{
		return obj is OVRPose && this == (OVRPose)obj;
	}

	public override int GetHashCode()
	{
		return this.position.GetHashCode() ^ this.orientation.GetHashCode();
	}

	public static bool operator ==(OVRPose x, OVRPose y)
	{
		return x.position == y.position && x.orientation == y.orientation;
	}

	public static bool operator !=(OVRPose x, OVRPose y)
	{
		return !(x == y);
	}

	public static OVRPose operator *(OVRPose lhs, OVRPose rhs)
	{
		return new OVRPose
		{
			position = lhs.position + lhs.orientation * rhs.position,
			orientation = lhs.orientation * rhs.orientation
		};
	}

	public OVRPose Inverse()
	{
		OVRPose ovrpose;
		ovrpose.orientation = Quaternion.Inverse(this.orientation);
		ovrpose.position = ovrpose.orientation * -this.position;
		return ovrpose;
	}

	public OVRPose flipZ()
	{
		OVRPose ovrpose = this;
		ovrpose.position.z = -ovrpose.position.z;
		ovrpose.orientation.z = -ovrpose.orientation.z;
		ovrpose.orientation.w = -ovrpose.orientation.w;
		return ovrpose;
	}

	public OVRPlugin.Posef ToPosef_Legacy()
	{
		return new OVRPlugin.Posef
		{
			Position = this.position.ToVector3f(),
			Orientation = this.orientation.ToQuatf()
		};
	}

	public OVRPlugin.Posef ToPosef()
	{
		OVRPlugin.Posef result = default(OVRPlugin.Posef);
		result.Position.x = this.position.x;
		result.Position.y = this.position.y;
		result.Position.z = -this.position.z;
		result.Orientation.x = -this.orientation.x;
		result.Orientation.y = -this.orientation.y;
		result.Orientation.z = this.orientation.z;
		result.Orientation.w = this.orientation.w;
		return result;
	}

	public OVRPose Rotate180AlongX()
	{
		OVRPose result = this;
		result.orientation *= Quaternion.Euler(180f, 0f, 0f);
		return result;
	}

	public Vector3 position;

	public Quaternion orientation;
}
