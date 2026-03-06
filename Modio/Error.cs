using System;
using Modio.Errors;

namespace Modio
{
	public class Error : IEquatable<Error>
	{
		public Error(ErrorCode code)
		{
			this.Code = code;
		}

		public Error(ErrorCode code, string customMessage)
		{
			this.Code = code;
			this.CustomMessage = customMessage;
		}

		public bool IsSilent
		{
			get
			{
				ErrorCode code = this.Code;
				return code == ErrorCode.SHUTTING_DOWN || code == ErrorCode.OPERATION_CANCELLED;
			}
		}

		public virtual string GetMessage()
		{
			string customMessage = this.CustomMessage;
			if (customMessage == null || customMessage.Length <= 0)
			{
				return this.Code.GetMessage(null);
			}
			return this.CustomMessage;
		}

		public static implicit operator bool(Error error)
		{
			return error.Code > ErrorCode.NONE;
		}

		public static explicit operator Error(ErrorCode errorCode)
		{
			if (errorCode != ErrorCode.NONE)
			{
				return new Error(errorCode);
			}
			return Error.None;
		}

		public override string ToString()
		{
			if (this.Code != ErrorCode.NONE)
			{
				return this.GetMessage();
			}
			return "Success";
		}

		public bool Equals(Error other)
		{
			return this.Code == other.Code;
		}

		public override bool Equals(object obj)
		{
			if (this != obj)
			{
				Error error = obj as Error;
				return error != null && this.Equals(error);
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.Code.GetHashCode();
		}

		public static readonly Error None = new Error(ErrorCode.NONE);

		public static readonly Error Unknown = new Error(ErrorCode.UNKNOWN);

		public readonly ErrorCode Code;

		public readonly string CustomMessage;
	}
}
