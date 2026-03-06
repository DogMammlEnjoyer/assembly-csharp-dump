using System;
using System.Collections.Generic;
using Backtrace.Unity.Json;
using Backtrace.Unity.Model.JsonData;

namespace Backtrace.Unity.Model.Metrics
{
	internal sealed class SummedEventsSubmissionQueue : MetricsSubmissionQueue<SummedEvent>
	{
		public SummedEventsSubmissionQueue(string submissionUrl, AttributeProvider attributeProvider) : base("summed_events", submissionUrl)
		{
			this._attributeProvider = attributeProvider;
		}

		public override void StartWithEvent(string eventName)
		{
			this.Events.AddLast(new SummedEvent(eventName));
			base.Send();
		}

		internal override IEnumerable<BacktraceJObject> GetEventsPayload(ICollection<SummedEvent> events)
		{
			List<BacktraceJObject> list = new List<BacktraceJObject>();
			IDictionary<string, string> scopedAttributes = this._attributeProvider.GenerateAttributes(false);
			foreach (SummedEvent summedEvent in events)
			{
				list.Add(summedEvent.ToJson(scopedAttributes));
			}
			this.Events.Clear();
			return list;
		}

		internal override void OnMaximumAttemptsReached(ICollection<SummedEvent> events)
		{
			if ((long)(base.Count + events.Count) < (long)((ulong)base.MaximumEvents))
			{
				foreach (SummedEvent value in events)
				{
					this.Events.AddFirst(value);
				}
			}
		}

		private const string Name = "summed_events";

		private readonly AttributeProvider _attributeProvider;
	}
}
