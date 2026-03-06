using System;
using System.Collections.Generic;
using Backtrace.Unity.Common;
using Backtrace.Unity.Json;
using Backtrace.Unity.Model.JsonData;

namespace Backtrace.Unity.Model.Metrics
{
	internal sealed class UniqueEventsSubmissionQueue : MetricsSubmissionQueue<UniqueEvent>
	{
		public UniqueEventsSubmissionQueue(string submissionUrl, AttributeProvider attributeProvider) : base("unique_events", submissionUrl)
		{
			this._attributeProvider = attributeProvider;
		}

		public override void StartWithEvent(string eventName)
		{
			IDictionary<string, string> uniqueEventAttributes = this.GetUniqueEventAttributes();
			string value;
			if (uniqueEventAttributes.TryGetValue(eventName, out value) && !string.IsNullOrEmpty(value))
			{
				this.Events.AddLast(new UniqueEvent(eventName, (long)DateTimeHelper.Timestamp(), uniqueEventAttributes));
			}
			base.Send();
		}

		internal override IEnumerable<BacktraceJObject> GetEventsPayload(ICollection<UniqueEvent> events)
		{
			List<BacktraceJObject> list = new List<BacktraceJObject>();
			foreach (UniqueEvent uniqueEvent in events)
			{
				list.Add(uniqueEvent.ToJson());
				uniqueEvent.UpdateTimestamp((long)DateTimeHelper.Timestamp(), this.GetUniqueEventAttributes());
			}
			return list;
		}

		private IDictionary<string, string> GetUniqueEventAttributes()
		{
			return this._attributeProvider.GenerateAttributes(false);
		}

		private const string Name = "unique_events";

		private readonly AttributeProvider _attributeProvider;
	}
}
