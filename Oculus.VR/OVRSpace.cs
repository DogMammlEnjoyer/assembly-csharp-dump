using System;

public readonly struct OVRSpace : IEquatable<OVRSpace>
{
	public ulong Handle { get; }

	public bool TryGetUuid(out Guid uuid)
	{
		return OVRPlugin.GetSpaceUuid(this.Handle, out uuid);
	}

	public bool Valid
	{
		get
		{
			return this.Handle > 0UL;
		}
	}

	public OVRSpace(ulong handle)
	{
		this.Handle = handle;
	}

	public override string ToString()
	{
		return string.Format("0x{0:x16}", this.Handle);
	}

	public bool Equals(OVRSpace other)
	{
		return this.Handle == other.Handle;
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRSpace)
		{
			OVRSpace other = (OVRSpace)obj;
			return this.Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return this.Handle.GetHashCode();
	}

	public static bool operator ==(OVRSpace lhs, OVRSpace rhs)
	{
		return lhs.Handle == rhs.Handle;
	}

	public static bool operator !=(OVRSpace lhs, OVRSpace rhs)
	{
		return lhs.Handle != rhs.Handle;
	}

	public static implicit operator OVRSpace(ulong handle)
	{
		return new OVRSpace(handle);
	}

	public static implicit operator ulong(OVRSpace space)
	{
		return space.Handle;
	}

	[Obsolete("Anchor APIs no longer require a storage location.")]
	public enum StorageLocation
	{
		Local,
		Cloud
	}
}
