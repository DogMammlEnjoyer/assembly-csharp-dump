using System;

namespace Liv.Lck
{
	public class LckResult : ILckResult
	{
		public bool Success
		{
			get
			{
				return this._success;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public LckError? Error
		{
			get
			{
				return this._error;
			}
		}

		private LckResult(bool success, string message, LckError? error)
		{
			this._success = success;
			this._message = message;
			this._error = error;
		}

		internal static LckResult NewSuccess()
		{
			return new LckResult(true, null, null);
		}

		internal static LckResult NewError(LckError error, string message)
		{
			return new LckResult(false, message, new LckError?(error));
		}

		private readonly bool _success;

		private readonly string _message;

		private readonly LckError? _error;
	}
}
