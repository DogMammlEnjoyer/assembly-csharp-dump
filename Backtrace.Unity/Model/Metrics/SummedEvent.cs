using System;
using System.Collections.Generic;
using Backtrace.Unity.Common;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.Metrics
{
	internal sealed class SummedEvent : EventAggregationBase
	{
		internal SummedEvent(string name) : this(name, (long)DateTimeHelper.Timestamp(), new Dictionary<string, string>())
		{
		}

		internal SummedEvent(string name, long timestamp, IDictionary<string, string> attributes) : base(name, timestamp)
		{
			this.Attributes = (attributes ?? new Dictionary<string, string>());
		}

		internal BacktraceJObject ToJson(IDictionary<string, string> scopedAttributes)
		{
			if (scopedAttributes != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in scopedAttributes)
				{
					this.Attributes[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			BacktraceJObject backtraceJObject = base.ToBaseObject(this.Attributes);
			backtraceJObject.Add("metric_group", base.Name);
			return backtraceJObject;
		}

		internal const string MetricGroupName = "metric_group";

		internal readonly IDictionary<string, string> Attributes;
	}
}
