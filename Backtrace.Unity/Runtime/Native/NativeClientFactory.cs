using System;
using System.Collections.Generic;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Breadcrumbs;
using Backtrace.Unity.Runtime.Native.Windows;
using UnityEngine;

namespace Backtrace.Unity.Runtime.Native
{
	internal static class NativeClientFactory
	{
		internal static INativeClient CreateNativeClient(BacktraceConfiguration configuration, string gameObjectName, BacktraceBreadcrumbs breadcrumbs, IDictionary<string, string> attributes, ICollection<string> attachments)
		{
			INativeClient result;
			try
			{
				result = new NativeClient(configuration, breadcrumbs, attributes, attachments);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(string.Format("Cannot startup the native client. Reason: {0}", ex.Message));
				result = null;
			}
			return result;
		}
	}
}
