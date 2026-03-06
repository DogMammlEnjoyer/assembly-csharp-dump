using System;
using System.Collections.Generic;

public readonly struct OVRRoomLayout : IOVRAnchorComponent<OVRRoomLayout>, IEquatable<OVRRoomLayout>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRRoomLayout>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRRoomLayout>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRRoomLayout IOVRAnchorComponent<OVRRoomLayout>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRRoomLayout(anchor);
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

	OVRTask<bool> IOVRAnchorComponent<OVRRoomLayout>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The RoomLayout component cannot be enabled or disabled.");
	}

	public bool Equals(OVRRoomLayout other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRRoomLayout lhs, OVRRoomLayout rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRRoomLayout lhs, OVRRoomLayout rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRRoomLayout)
		{
			OVRRoomLayout other = (OVRRoomLayout)obj;
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
		return string.Format("{0}.RoomLayout", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.RoomLayout;
		}
	}

	internal ulong Handle { get; }

	private OVRRoomLayout(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	[Obsolete("Use FetchAnchorsAsync instead.")]
	public OVRTask<bool> FetchLayoutAnchorsAsync(List<OVRAnchor> anchors)
	{
		OVRPlugin.RoomLayout roomLayout;
		if (!OVRPlugin.GetSpaceRoomLayout(this.Handle, out roomLayout))
		{
			throw new InvalidOperationException("Could not get Room Layout");
		}
		List<Guid> list;
		OVRTask<bool> result;
		using (new OVRObjectPool.ListScope<Guid>(ref list))
		{
			list.Add(roomLayout.floorUuid);
			list.Add(roomLayout.ceilingUuid);
			list.AddRange(roomLayout.wallUuids);
			result = OVRAnchor.FetchAnchorsAsync(list, anchors, OVRSpace.StorageLocation.Local, 0.0);
		}
		return result;
	}

	public OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchAnchorsAsync(List<OVRAnchor> anchors)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		OVRPlugin.RoomLayout roomLayout;
		if (!OVRPlugin.GetSpaceRoomLayout(this.Handle, out roomLayout))
		{
			throw new InvalidOperationException("Could not get Room Layout");
		}
		List<Guid> list;
		OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> result;
		using (new OVRObjectPool.ListScope<Guid>(ref list))
		{
			list.Add(roomLayout.floorUuid);
			list.Add(roomLayout.ceilingUuid);
			list.AddRange(roomLayout.wallUuids);
			result = OVRAnchor.FetchAnchorsAsync(anchors, new OVRAnchor.FetchOptions
			{
				Uuids = list
			}, null);
		}
		return result;
	}

	public bool TryGetRoomLayout(out Guid ceiling, out Guid floor, out Guid[] walls)
	{
		ceiling = Guid.Empty;
		floor = Guid.Empty;
		walls = null;
		OVRPlugin.RoomLayout roomLayout;
		if (!OVRPlugin.GetSpaceRoomLayout(this.Handle, out roomLayout))
		{
			return false;
		}
		ceiling = roomLayout.ceilingUuid;
		floor = roomLayout.floorUuid;
		walls = roomLayout.wallUuids;
		return true;
	}

	public static readonly OVRRoomLayout Null;
}
