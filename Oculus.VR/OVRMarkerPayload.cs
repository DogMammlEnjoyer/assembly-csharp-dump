using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public readonly struct OVRMarkerPayload : IOVRAnchorComponent<OVRMarkerPayload>, IEquatable<OVRMarkerPayload>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRMarkerPayload>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRMarkerPayload>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRMarkerPayload IOVRAnchorComponent<OVRMarkerPayload>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRMarkerPayload(anchor);
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

	OVRTask<bool> IOVRAnchorComponent<OVRMarkerPayload>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The MarkerPayload component cannot be enabled or disabled.");
	}

	public bool Equals(OVRMarkerPayload other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRMarkerPayload lhs, OVRMarkerPayload rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRMarkerPayload lhs, OVRMarkerPayload rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRMarkerPayload)
		{
			OVRMarkerPayload other = (OVRMarkerPayload)obj;
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
		return string.Format("{0}.MarkerPayload", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.MarkerPayload;
		}
	}

	internal ulong Handle { get; }

	private OVRMarkerPayload(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public OVRMarkerPayloadType PayloadType
	{
		get
		{
			OVRPlugin.SpaceMarkerPayload spaceMarkerPayload = default(OVRPlugin.SpaceMarkerPayload);
			if (!OVRPlugin.GetSpaceMarkerPayload(this.Handle, ref spaceMarkerPayload).IsSuccess())
			{
				return OVRMarkerPayloadType.InvalidQRCode;
			}
			return (OVRMarkerPayloadType)spaceMarkerPayload.PayloadType;
		}
	}

	public unsafe string AsString()
	{
		if (this.PayloadType != OVRMarkerPayloadType.StringQRCode)
		{
			throw new InvalidOperationException(string.Format("{0} must be {1}.", "PayloadType", OVRMarkerPayloadType.StringQRCode));
		}
		string result;
		using (NativeArray<byte> nativeArray = new NativeArray<byte>(this.ByteCount, Allocator.Temp, NativeArrayOptions.ClearMemory))
		{
			void* unsafeReadOnlyPtr = nativeArray.GetUnsafeReadOnlyPtr<byte>();
			result = Marshal.PtrToStringUTF8(new IntPtr(unsafeReadOnlyPtr), this.GetBytes(new Span<byte>(unsafeReadOnlyPtr, nativeArray.Length)));
		}
		return result;
	}

	public ArraySegment<byte> Bytes
	{
		get
		{
			int byteCount = this.ByteCount;
			if (byteCount == 0)
			{
				return Array.Empty<byte>();
			}
			byte[] array = new byte[byteCount];
			return new ArraySegment<byte>(array, 0, this.GetBytes(array));
		}
	}

	public int ByteCount
	{
		get
		{
			OVRPlugin.SpaceMarkerPayload spaceMarkerPayload = default(OVRPlugin.SpaceMarkerPayload);
			if (!OVRPlugin.GetSpaceMarkerPayload(this.Handle, ref spaceMarkerPayload).IsSuccess())
			{
				return 0;
			}
			return (int)spaceMarkerPayload.BufferCountOutput;
		}
	}

	public unsafe int GetBytes(Span<byte> buffer)
	{
		fixed (byte* pinnableReference = buffer.GetPinnableReference())
		{
			byte* buffer2 = pinnableReference;
			OVRPlugin.SpaceMarkerPayload spaceMarkerPayload = new OVRPlugin.SpaceMarkerPayload
			{
				BufferCapacityInput = (uint)buffer.Length,
				Buffer = buffer2
			};
			OVRPlugin.Result spaceMarkerPayload2 = OVRPlugin.GetSpaceMarkerPayload(this.Handle, ref spaceMarkerPayload);
			if (spaceMarkerPayload2 == OVRPlugin.Result.Failure_InsufficientSize)
			{
				throw new ArgumentException("buffer is not large enough to hold the payload data. It " + string.Format("must be at least {0} but was {1}.", spaceMarkerPayload.BufferCountOutput, buffer.Length), "buffer");
			}
			if (!spaceMarkerPayload2.IsSuccess())
			{
				return 0;
			}
			return (int)spaceMarkerPayload.BufferCountOutput;
		}
	}

	public static readonly OVRMarkerPayload Null;
}
