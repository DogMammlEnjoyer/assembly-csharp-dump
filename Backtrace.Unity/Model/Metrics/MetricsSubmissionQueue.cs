using System;
using System.Collections.Generic;
using System.Linq;
using Backtrace.Unity.Common;
using Backtrace.Unity.Json;
using UnityEngine;

namespace Backtrace.Unity.Model.Metrics
{
	internal abstract class MetricsSubmissionQueue<T> where T : EventAggregationBase
	{
		public int Count
		{
			get
			{
				return this.Events.Count;
			}
		}

		public uint MaximumEvents { get; set; }

		internal string SubmissionUrl { get; set; }

		internal MetricsSubmissionQueue(string name, string submissionUrl)
		{
			this._name = name;
			this.SubmissionUrl = submissionUrl;
			this.MaximumEvents = 50U;
		}

		public bool ReachedLimit()
		{
			return (ulong)this.MaximumEvents == (ulong)((long)this.Events.Count) && this.MaximumEvents > 0U;
		}

		public bool ShouldProcessEvent(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				Debug.LogWarning("Skipping report: attribute name is null or empty");
				return false;
			}
			if (this.ReachedLimit())
			{
				Debug.LogWarning("Skipping report: Reached store limit.");
				return false;
			}
			return true;
		}

		public abstract void StartWithEvent(string eventName);

		internal void Send()
		{
			this.SendPayload(new LinkedList<T>(this.Events), 0U);
		}

		internal void SendPayload(ICollection<T> events, uint attempts = 0U)
		{
			if (events.Count == 0)
			{
				return;
			}
			BacktraceJObject jObject = this.CreateJsonPayload(events);
			this.RequestHandler.Post(this.SubmissionUrl, jObject, delegate(long statusCode, bool httpError, string response)
			{
				if (statusCode == 200L)
				{
					this.OnRequestCompleted();
					return;
				}
				if (httpError || (statusCode > 501L && statusCode != 505L))
				{
					this._numberOfDroppedRequests++;
					if (attempts + 1U == 3U)
					{
						this.OnMaximumAttemptsReached(events);
						return;
					}
					this._submissionJobs.Add(new MetricsSubmissionJob<T>
					{
						Events = events,
						NextInvokeTime = this.CalculateNextRetryTime(attempts + 1U) + (double)Time.unscaledTime,
						NumberOfAttempts = attempts + 1U
					});
				}
			});
		}

		public void SendPendingEvents(float time)
		{
			int i = 0;
			while (i < this._submissionJobs.Count)
			{
				MetricsSubmissionJob<T> metricsSubmissionJob = this._submissionJobs.ElementAt(i);
				if (metricsSubmissionJob.NextInvokeTime < (double)time)
				{
					this.SendPayload(metricsSubmissionJob.Events, metricsSubmissionJob.NumberOfAttempts);
					this._submissionJobs.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		internal virtual void OnMaximumAttemptsReached(ICollection<T> events)
		{
		}

		internal abstract IEnumerable<BacktraceJObject> GetEventsPayload(ICollection<T> events);

		internal virtual BacktraceJObject CreateJsonPayload(ICollection<T> events)
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			backtraceJObject.Add("application", this._applicationName);
			backtraceJObject.Add("appversion", this._applicationVersion);
			backtraceJObject.Add("metadata", this.CreatePayloadMetadata());
			backtraceJObject.Add(this._name, this.GetEventsPayload(events));
			return backtraceJObject;
		}

		private double CalculateNextRetryTime(uint attemps)
		{
			double num = MathHelper.Clamp(10.0 * Math.Pow(10.0, attemps), 0.0, 300.0);
			double maximum = num + num * 1.0;
			return MathHelper.Uniform(num, maximum);
		}

		private BacktraceJObject CreatePayloadMetadata()
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			backtraceJObject.Add("dropped_events", (long)this._numberOfDroppedRequests);
			return backtraceJObject;
		}

		private void OnRequestCompleted()
		{
			this._numberOfDroppedRequests = 0;
		}

		public const int DefaultTimeInSecBetweenRequests = 10;

		private readonly string _name;

		private readonly List<MetricsSubmissionJob<T>> _submissionJobs = new List<MetricsSubmissionJob<T>>();

		internal LinkedList<T> Events = new LinkedList<T>();

		private int _numberOfDroppedRequests;

		internal IBacktraceHttpClient RequestHandler = new BacktraceHttpClient();

		private readonly string _applicationName = Application.productName;

		private readonly string _applicationVersion = Application.version;
	}
}
