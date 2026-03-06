using System;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	public class BacktraceResult
	{
		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public string Object
		{
			get
			{
				return this.@object;
			}
			set
			{
				this.@object = value;
				this.Status = BacktraceResultStatus.Ok;
			}
		}

		public string RxId
		{
			get
			{
				return this._rxId;
			}
			set
			{
				this._rxId = value;
				this.Status = BacktraceResultStatus.Ok;
			}
		}

		internal static BacktraceResult OnLimitReached()
		{
			return new BacktraceResult
			{
				Status = BacktraceResultStatus.LimitReached,
				Message = "Client report limit reached"
			};
		}

		internal static BacktraceResult OnNetworkError(Exception exception)
		{
			return new BacktraceResult
			{
				Message = exception.Message,
				Status = BacktraceResultStatus.NetworkError
			};
		}

		internal void AddInnerResult(BacktraceResult innerResult)
		{
			if (this.InnerExceptionResult == null)
			{
				this.InnerExceptionResult = innerResult;
				return;
			}
			this.InnerExceptionResult.AddInnerResult(innerResult);
		}

		public static BacktraceResult FromJson(string json)
		{
			BacktraceResult backtraceResult = new BacktraceResult
			{
				Status = (string.IsNullOrEmpty(json) ? BacktraceResultStatus.Empty : BacktraceResultStatus.Ok)
			};
			if (backtraceResult.Status == BacktraceResultStatus.Empty)
			{
				return backtraceResult;
			}
			try
			{
				BacktraceResult.BacktraceRawResult backtraceRawResult = JsonUtility.FromJson<BacktraceResult.BacktraceRawResult>(json);
				backtraceResult.response = backtraceRawResult.response;
				backtraceResult._rxId = backtraceRawResult._rxid;
			}
			catch (Exception ex)
			{
				Debug.LogWarning(string.Format("Cannot parse Backtrace JSON response. Error: {0}. Content: {1}", json, ex.Message));
			}
			return backtraceResult;
		}

		public BacktraceResult InnerExceptionResult;

		public string message;

		public string response;

		public BacktraceResultStatus Status = BacktraceResultStatus.Ok;

		private string @object;

		public string _rxId;

		[Serializable]
		private class BacktraceRawResult
		{
			public string response;

			public string _rxid;
		}
	}
}
