using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public struct OVRResult<TValue, TStatus> : IEquatable<OVRResult<TValue, TStatus>> where TStatus : struct, Enum, IConvertible
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

	public bool HasValue
	{
		get
		{
			return this.Success;
		}
	}

	public TValue Value
	{
		get
		{
			if (!this._initialized)
			{
				throw new InvalidOperationException("The OVRResult object is not valid.");
			}
			if (this._statusCode < 0)
			{
				throw new InvalidOperationException("The OVRResult does not have a value because the " + string.Format("operation failed with {0}.", this._status));
			}
			return this._value;
		}
	}

	public bool TryGetValue(out TValue value)
	{
		if (this.HasValue)
		{
			value = this._value;
			return true;
		}
		value = default(TValue);
		return false;
	}

	private OVRResult(TValue value, TStatus status)
	{
		if (UnsafeUtility.SizeOf<TStatus>() != 4)
		{
			throw new InvalidOperationException("TStatus must have a 4 byte underlying storage type.");
		}
		this._initialized = true;
		this._value = value;
		this._status = status;
		this._statusCode = UnsafeUtility.EnumToInt<TStatus>(this._status);
	}

	public static OVRResult<TValue, TStatus> From(TValue value, TStatus status)
	{
		return new OVRResult<TValue, TStatus>(value, status);
	}

	public unsafe static OVRResult<TValue, TStatus> FromSuccess(TValue value, TStatus status)
	{
		if (!(*UnsafeUtility.As<TStatus, OVRPlugin.Result>(ref status)).IsSuccess())
		{
			throw new ArgumentException("Not of a valid success status. Success values must have an integral value >= 0.", "status");
		}
		return new OVRResult<TValue, TStatus>(value, status);
	}

	public unsafe static OVRResult<TValue, TStatus> FromFailure(TStatus status)
	{
		if ((*UnsafeUtility.As<TStatus, OVRPlugin.Result>(ref status)).IsSuccess())
		{
			throw new ArgumentException("Not of a valid failure status. Failure values must have an integral value < 0.", "status");
		}
		return new OVRResult<TValue, TStatus>(default(TValue), status);
	}

	public bool Equals(OVRResult<TValue, TStatus> other)
	{
		return this._initialized == other._initialized && EqualityComparer<TValue>.Default.Equals(this._value, other._value) && this._statusCode == other._statusCode;
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRResult<TValue, TStatus>)
		{
			OVRResult<TValue, TStatus> other = (OVRResult<TValue, TStatus>)obj;
			return this.Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = ((17 * 31 + this._initialized.GetHashCode()) * 31 + this._statusCode.GetHashCode()) * 31;
		TValue value = this._value;
		return num + ((value != null) ? value.GetHashCode() : 0);
	}

	public override string ToString()
	{
		if (!this._initialized)
		{
			return "(invalid result)";
		}
		if (!this.HasValue)
		{
			TStatus status = this._status;
			return status.ToString();
		}
		return string.Format("(Value={0}, Status={1})", this._value, this._status);
	}

	public static implicit operator bool(OVRResult<TValue, TStatus> value)
	{
		return value.Success;
	}

	public unsafe static implicit operator OVRResult<TValue, TStatus>(OVRPlugin.Result result)
	{
		return OVRResult<TValue, TStatus>.FromFailure(*UnsafeUtility.As<OVRPlugin.Result, TStatus>(ref result));
	}

	private readonly bool _initialized;

	private readonly TValue _value;

	private readonly int _statusCode;

	private readonly TStatus _status;
}
