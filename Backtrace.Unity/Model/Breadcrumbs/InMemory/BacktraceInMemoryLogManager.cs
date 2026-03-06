using System;
using System.Collections.Generic;
using Backtrace.Unity.Common;

namespace Backtrace.Unity.Model.Breadcrumbs.InMemory
{
	internal sealed class BacktraceInMemoryLogManager : IBacktraceLogManager
	{
		public int MaximumNumberOfBreadcrumbs { get; set; }

		public BacktraceInMemoryLogManager()
		{
			this._breadcrumbId = DateTimeHelper.TimestampMs();
			this.MaximumNumberOfBreadcrumbs = 100;
			this.Breadcrumbs = new Queue<InMemoryBreadcrumb>(100);
		}

		public string BreadcrumbsFilePath
		{
			get
			{
				return string.Empty;
			}
		}

		public bool Add(string message, BreadcrumbLevel type, UnityEngineLogLevel level, IDictionary<string, string> attributes)
		{
			object lockObject = this._lockObject;
			lock (lockObject)
			{
				if (this.Breadcrumbs.Count + 1 > this.MaximumNumberOfBreadcrumbs)
				{
					while (this.Breadcrumbs.Count + 1 > this.MaximumNumberOfBreadcrumbs)
					{
						this.Breadcrumbs.Dequeue();
					}
				}
			}
			this.Breadcrumbs.Enqueue(new InMemoryBreadcrumb
			{
				Message = message,
				Timestamp = DateTimeHelper.TimestampMs(),
				Level = level,
				Type = type,
				Attributes = attributes
			});
			this._breadcrumbId += 1.0;
			return true;
		}

		public bool Clear()
		{
			this.Breadcrumbs.Clear();
			return true;
		}

		public bool Enable()
		{
			return true;
		}

		public int Length()
		{
			return this.Breadcrumbs.Count;
		}

		public double BreadcrumbId()
		{
			return this._breadcrumbId;
		}

		public const int DefaultMaximumNumberOfInMemoryBreadcrumbs = 100;

		private object _lockObject = new object();

		internal readonly Queue<InMemoryBreadcrumb> Breadcrumbs;

		private double _breadcrumbId;
	}
}
