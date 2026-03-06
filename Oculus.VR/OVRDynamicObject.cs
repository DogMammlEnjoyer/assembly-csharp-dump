using System;

public readonly struct OVRDynamicObject : IOVRAnchorComponent<OVRDynamicObject>, IEquatable<OVRDynamicObject>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRDynamicObject>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRDynamicObject>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRDynamicObject IOVRAnchorComponent<OVRDynamicObject>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRDynamicObject(anchor);
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

	OVRTask<bool> IOVRAnchorComponent<OVRDynamicObject>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The DynamicObject component cannot be enabled or disabled.");
	}

	public bool Equals(OVRDynamicObject other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRDynamicObject lhs, OVRDynamicObject rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRDynamicObject lhs, OVRDynamicObject rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRDynamicObject)
		{
			OVRDynamicObject other = (OVRDynamicObject)obj;
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
		return string.Format("{0}.DynamicObject", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.DynamicObject;
		}
	}

	internal ulong Handle { get; }

	private OVRDynamicObject(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public OVRAnchor.TrackableType TrackableType
	{
		get
		{
			OVRPlugin.DynamicObjectData dynamicObjectData;
			OVRAnchor.TrackableType result;
			if (OVRPlugin.GetSpaceDynamicObjectData(this.Handle, out dynamicObjectData).IsSuccess())
			{
				OVRAnchor.TrackableType trackableType;
				if (dynamicObjectData.ClassType == OVRPlugin.DynamicObjectClass.Keyboard)
				{
					trackableType = OVRAnchor.TrackableType.Keyboard;
				}
				else
				{
					trackableType = OVRAnchor.TrackableType.None;
				}
				result = trackableType;
			}
			else
			{
				result = OVRAnchor.TrackableType.None;
			}
			return result;
		}
	}

	public static readonly OVRDynamicObject Null;
}
