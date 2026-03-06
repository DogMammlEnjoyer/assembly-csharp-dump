using System;

namespace Liv.Lck.Core
{
	public class Result<T>
	{
		public bool IsOk
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

		public CoreError? Err
		{
			get
			{
				return this._error;
			}
		}

		public T Ok
		{
			get
			{
				return this._result;
			}
		}

		private Result(bool success, string message, CoreError? error, T result)
		{
			this._success = success;
			this._message = message;
			this._error = error;
			this._result = result;
		}

		public static Result<T> NewSuccess(T result)
		{
			return new Result<T>(true, null, null, result);
		}

		public static Result<T> NewError(CoreError error, string message)
		{
			return new Result<T>(false, message, new CoreError?(error), default(T));
		}

		private readonly bool _success;

		private readonly string _message;

		private readonly CoreError? _error;

		private readonly T _result;
	}
}
