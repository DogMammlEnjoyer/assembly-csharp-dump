using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Lib.Wit.Runtime.Utilities.Logging;
using UnityEngine;

namespace Meta.Voice.Logging
{
	[LogCategory(LogCategory.Logging, LogCategory.ErrorMitigator)]
	public class ErrorMitigator : IErrorMitigator, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.ErrorMitigator, null);

		public ErrorMitigator()
		{
			try
			{
				foreach (object obj in Enum.GetValues(typeof(KnownErrorCode)))
				{
					KnownErrorCode knownErrorCode = (KnownErrorCode)obj;
					DescriptionAttribute customAttribute = typeof(KnownErrorCode).GetField(knownErrorCode.ToString()).GetCustomAttribute<DescriptionAttribute>();
					if (customAttribute != null)
					{
						this._mitigations[knownErrorCode] = customAttribute.Description;
					}
					else
					{
						this.Logger.Error(KnownErrorCode.KnownErrorMissingDescription, "Missing error description for {0}", new object[]
						{
							knownErrorCode
						});
						this._mitigations[knownErrorCode] = "Please file a bug report.";
					}
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to get known error mitigations. Exception: {0}", arg));
			}
		}

		public string GetMitigation(ErrorCode errorCode)
		{
			if (this._mitigations.ContainsKey(errorCode))
			{
				return this._mitigations[errorCode];
			}
			return "There are no known mitigations. Please report to the Voice SDK team.";
		}

		public void SetMitigation(ErrorCode errorCode, string mitigation)
		{
			this._mitigations[errorCode] = mitigation;
		}

		private readonly Dictionary<ErrorCode, string> _mitigations = new Dictionary<ErrorCode, string>();
	}
}
