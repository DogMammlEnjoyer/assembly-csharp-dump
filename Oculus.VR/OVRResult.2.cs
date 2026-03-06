using System;
using Unity.Collections.LowLevel.Unsafe;

public struct OVRResult<TStatus> : IEquatable<OVRResult<TStatus>> where TStatus : struct, Enum, IConvertible
{
	public bool Success
	{
		get
		{
			return this._initialized && ((OVRPlugin.Result)this._statusCode).IsSuccess();
		}
	}

	public unsafe TStatus Status
	{
		get
		{
			if (this._initialized)
			{
				return this._status;
			}
			OVRPlugin.Result result = OVRPlugin.Result.Failure_DataIsInvalid;
			return *UnsafeUtility.As<OVRPlugin.Result, TStatus>(ref result);
		}
	}

	private OVRResult(TStatus status)
	{
		if (UnsafeUtility.SizeOf<TStatus>() != 4)
		{
			throw new InvalidOperationException("TStatus must have a 4 byte underlying storage type.");
		}
		this._initialized = true;
		this._status = status;
		this._statusCode = UnsafeUtility.EnumToInt<TStatus>(this._status);
	}

	public static OVRResult<TStatus> From(TStatus status)
	{
		return new OVRResult<TStatus>(status);
	}

	public unsafe static OVRResult<TStatus> FromSuccess(TStatus status)
	{
		if (!(*UnsafeUtility.As<TStatus, OVRPlugin.Result>(ref status)).IsSuccess())
		{
			throw new ArgumentException("Not of a valid success status", "status");
		}
		return new OVRResult<TStatus>(status);
	}

	public unsafe static OVRResult<TStatus> FromFailure(TStatus status)
	{
		if ((*UnsafeUtility.As<TStatus, OVRPlugin.Result>(ref status)).IsSuccess())
		{
			throw new ArgumentException("Not of a valid failure status", "status");
		}
		return new OVRResult<TStatus>(status);
	}

	public bool Equals(OVRResult<TStatus> other)
	{
		return this._initialized == other._initialized && this._statusCode == other._statusCode;
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRResult<TStatus>)
		{
			OVRResult<TStatus> other = (OVRResult<TStatus>)obj;
			return this.Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 31 + this._initialized.GetHashCode()) * 31 + this._statusCode.GetHashCode();
	}

	public override string ToString()
	{
		if (!this._initialized)
		{
			return "(invalid result)";
		}
		TStatus status = this._status;
		return status.ToString();
	}

	public static implicit operator bool(OVRResult<TStatus> value)
	{
		return value.Success;
	}

	public unsafe static implicit operator OVRResult<TStatus>(OVRPlugin.Result result)
	{
		return OVRResult<TStatus>.From(*UnsafeUtility.As<OVRPlugin.Result, TStatus>(ref result));
	}

	private readonly bool _initialized;

	private readonly int _statusCode;

	private readonly TStatus _status;
}
