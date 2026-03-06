using System;

namespace Meta.Voice.Logging
{
	public readonly struct ErrorCode
	{
		private string Value { get; }

		private ErrorCode(string value)
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return this.Value;
		}

		public static implicit operator string(ErrorCode errorCode)
		{
			return errorCode.Value;
		}

		public static explicit operator ErrorCode(string value)
		{
			return new ErrorCode(value);
		}

		public static implicit operator ErrorCode(KnownErrorCode value)
		{
			return new ErrorCode(value.ToString());
		}

		public override bool Equals(object obj)
		{
			if (obj is ErrorCode)
			{
				ErrorCode errorCode = (ErrorCode)obj;
				return this.Value == errorCode.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}
	}
}
