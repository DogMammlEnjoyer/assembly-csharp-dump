using System;

namespace Liv.Lck
{
	public class LckResult<T> : ILckResult
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

		public T Result
		{
			get
			{
				return this._result;
			}
		}

		private LckResult(bool success, string message, LckError? error, T result)
		{
			this._success = success;
			this._message = message;
			this._error = error;
			this._result = result;
		}

		internal static LckResult<T> NewSuccess(T result)
		{
			return new LckResult<T>(true, null, null, result);
		}

		internal static LckResult<T> NewError(LckError error, string message)
		{
			return new LckResult<T>(false, message, new LckError?(error), default(T));
		}

		private readonly bool _success;

		private readonly string _message;

		private readonly LckError? _error;

		private readonly T _result;
	}
}
