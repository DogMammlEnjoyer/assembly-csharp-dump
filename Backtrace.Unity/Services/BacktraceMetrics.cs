using System;
using System.Collections.Generic;
using System.Linq;
using Backtrace.Unity.Common;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Attributes;
using Backtrace.Unity.Model.JsonData;
using Backtrace.Unity.Model.Metrics;
using UnityEngine;

namespace Backtrace.Unity.Services
{
	internal sealed class BacktraceMetrics : IBacktraceMetrics, IScopeAttributeProvider
	{
		public string StartupUniqueAttributeName { get; set; }

		public uint MaximumUniqueEvents
		{
			get
			{
				return this._uniqueEventsSubmissionQueue.MaximumEvents;
			}
			set
			{
				this._uniqueEventsSubmissionQueue.MaximumEvents = value;
			}
		}

		public uint MaximumSummedEvents
		{
			get
			{
				return this._summedEventsSubmissionQueue.MaximumEvents;
			}
			set
			{
				this._summedEventsSubmissionQueue.MaximumEvents = value;
			}
		}

		public string UniqueEventsSubmissionUrl
		{
			get
			{
				return this._uniqueEventsSubmissionQueue.SubmissionUrl;
			}
			set
			{
				this._uniqueEventsSubmissionQueue.SubmissionUrl = value;
			}
		}

		public string SummedEventsSubmissionUrl
		{
			get
			{
				return this._summedEventsSubmissionQueue.SubmissionUrl;
			}
			set
			{
				this._summedEventsSubmissionQueue.SubmissionUrl = value;
			}
		}

		public bool IgnoreSslValidation
		{
			set
			{
				this._uniqueEventsSubmissionQueue.RequestHandler.IgnoreSslValidation = value;
				this._summedEventsSubmissionQueue.RequestHandler.IgnoreSslValidation = value;
			}
		}

		public LinkedList<UniqueEvent> UniqueEvents
		{
			get
			{
				return this._uniqueEventsSubmissionQueue.Events;
			}
		}

		internal LinkedList<SummedEvent> SummedEvents
		{
			get
			{
				return this._summedEventsSubmissionQueue.Events;
			}
		}

		public BacktraceMetrics(AttributeProvider attributeProvider, long timeIntervalInSec, string uniqueEventsSubmissionUrl, string summedEventsSubmissionUrl)
		{
			this._attributeProvider = attributeProvider;
			this._timeIntervalInSec = timeIntervalInSec;
			this._uniqueEventsSubmissionQueue = new UniqueEventsSubmissionQueue(uniqueEventsSubmissionUrl, this._attributeProvider);
			this._summedEventsSubmissionQueue = new SummedEventsSubmissionQueue(summedEventsSubmissionUrl, this._attributeProvider);
			this._sessionId = this.SessionId.ToString();
			this.StartupUniqueAttributeName = "guid";
		}

		internal void OverrideHttpClient(IBacktraceHttpClient client)
		{
			this._uniqueEventsSubmissionQueue.RequestHandler = client;
			this._summedEventsSubmissionQueue.RequestHandler = client;
		}

		public void SendStartupEvent()
		{
			this._uniqueEventsSubmissionQueue.StartWithEvent(this.StartupUniqueAttributeName);
			this._summedEventsSubmissionQueue.StartWithEvent("Application Launches");
		}

		public void Tick(float time)
		{
			object @object = this._object;
			lock (@object)
			{
				this.SendPendingSubmissionJobs(time);
			}
			if (this._timeIntervalInSec == 0L)
			{
				return;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			@object = this._object;
			lock (@object)
			{
				flag4 = (time - this._lastUpdateTime >= (float)this._timeIntervalInSec);
				flag2 = this._summedEventsSubmissionQueue.ReachedLimit();
				flag3 = this._summedEventsSubmissionQueue.ReachedLimit();
				if (flag4 == flag2 != flag3)
				{
					return;
				}
				this._lastUpdateTime = time;
			}
			if (flag4)
			{
				this.Send();
				return;
			}
			if (flag2)
			{
				this._summedEventsSubmissionQueue.Send();
			}
			if (flag3)
			{
				this._summedEventsSubmissionQueue.Send();
			}
		}

		public void Send()
		{
			this._uniqueEventsSubmissionQueue.Send();
			this._summedEventsSubmissionQueue.Send();
		}

		public bool AddUniqueEvent(string attributeName)
		{
			return this.AddUniqueEvent(attributeName, null);
		}

		public bool AddUniqueEvent(string attributeName, IDictionary<string, string> attributes = null)
		{
			if (!this._uniqueEventsSubmissionQueue.ShouldProcessEvent(attributeName))
			{
				return false;
			}
			if (attributes == null)
			{
				attributes = new Dictionary<string, string>();
			}
			this._attributeProvider.AddAttributes(attributes, true);
			string value;
			if (!attributes.TryGetValue(attributeName, out value) || string.IsNullOrEmpty(value))
			{
				Debug.LogWarning("Attribute name is not available in attribute scope. Please define attribute to set unique event.");
				return false;
			}
			if (this.UniqueEvents.Any((UniqueEvent n) => n.Name == attributeName))
			{
				return false;
			}
			UniqueEvent value2 = new UniqueEvent(attributeName, (long)DateTimeHelper.Timestamp(), attributes);
			this._uniqueEventsSubmissionQueue.Events.AddLast(value2);
			return true;
		}

		public int Count()
		{
			return this._uniqueEventsSubmissionQueue.Count + this._summedEventsSubmissionQueue.Count;
		}

		public bool AddSummedEvent(string metricsGroupName)
		{
			return this.AddSummedEvent(metricsGroupName, null);
		}

		public bool AddSummedEvent(string metricsGroupName, IDictionary<string, string> attributes = null)
		{
			if (!this._summedEventsSubmissionQueue.ShouldProcessEvent(metricsGroupName))
			{
				return false;
			}
			SummedEvent value = new SummedEvent(metricsGroupName, (long)DateTimeHelper.Timestamp(), attributes);
			this._summedEventsSubmissionQueue.Events.AddLast(value);
			return true;
		}

		private void SendPendingSubmissionJobs(float time)
		{
			this._uniqueEventsSubmissionQueue.SendPendingEvents(time);
			this._summedEventsSubmissionQueue.SendPendingEvents(time);
		}

		internal static string GetDefaultUniqueEventsUrl(string universeName, string token)
		{
			return BacktraceMetrics.GetDefaultSubmissionUrl("unique-events", universeName, token);
		}

		internal static string GetDefaultSummedEventsUrl(string universeName, string token)
		{
			return BacktraceMetrics.GetDefaultSubmissionUrl("summed-events", universeName, token);
		}

		private static string GetDefaultSubmissionUrl(string serviceName, string universeName, string token)
		{
			return string.Format("{0}{1}/submit?token={2}&universe={3}", new object[]
			{
				"https://events.backtrace.io/api/",
				serviceName,
				token,
				universeName
			});
		}

		public void GetAttributes(IDictionary<string, string> attributes)
		{
			attributes["application.session"] = this._sessionId;
		}

		public readonly Guid SessionId = Guid.NewGuid();

		public const string DefaultSubmissionUrl = "https://events.backtrace.io/api/";

		public const uint DefaultTimeIntervalInMin = 30U;

		public const uint DefaultTimeIntervalInSec = 1800U;

		public const string DefaultUniqueAttributeName = "guid";

		public const int MaxTimeBetweenRequests = 300;

		public const int MaxNumberOfAttempts = 3;

		internal const string ApplicationSessionKey = "application.session";

		internal readonly UniqueEventsSubmissionQueue _uniqueEventsSubmissionQueue;

		internal readonly SummedEventsSubmissionQueue _summedEventsSubmissionQueue;

		private const string StartupEventName = "Application Launches";

		private readonly long _timeIntervalInSec;

		private float _lastUpdateTime;

		private readonly AttributeProvider _attributeProvider;

		private object _object = new object();

		private readonly string _sessionId;
	}
}
