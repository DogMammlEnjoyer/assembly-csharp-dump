using System;
using System.Collections.Generic;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.JsonData
{
	public class BacktraceAttributes
	{
		public BacktraceAttributes(BacktraceReport report, Dictionary<string, string> clientAttributes)
		{
			this.Attributes = clientAttributes;
			if (report != null && report.Attributes != null)
			{
				if (this.Attributes == null)
				{
					this.Attributes = report.Attributes;
				}
				else
				{
					foreach (KeyValuePair<string, string> keyValuePair in report.Attributes)
					{
						this.Attributes[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			if (this.Attributes == null)
			{
				this.Attributes = new Dictionary<string, string>();
			}
		}

		public BacktraceJObject ToJson()
		{
			return new BacktraceJObject(this.Attributes);
		}

		public readonly Dictionary<string, string> Attributes;
	}
}
