using System;
using System.Collections.Generic;
using System.Linq;
using Liv.Lck.Core;

namespace Liv.Lck.Telemetry
{
	public class LckTelemetryEvent
	{
		public LckTelemetryEventType EventType { get; set; }

		public Dictionary<string, object> Context { get; set; }

		public LckTelemetryEvent(LckTelemetryEventType eventType)
		{
			this.EventType = eventType;
		}

		public LckTelemetryEvent(LckTelemetryEventType eventType, Dictionary<string, object> context)
		{
			this.EventType = eventType;
			this.Context = context;
		}

		public override string ToString()
		{
			string text = string.Join(", ", from kvp in this.Context
			select string.Format("{0}: {1}", kvp.Key, kvp.Value));
			return string.Format("{0}={1} | {2}={{{3}}}", new object[]
			{
				"EventType",
				this.EventType,
				"Context",
				text
			});
		}
	}
}
