using System;
using System.Collections.Generic;
using System.Globalization;

namespace Backtrace.Unity.Model.Breadcrumbs.InMemory
{
	[Serializable]
	public class InMemoryBreadcrumb
	{
		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public double Timestamp
		{
			get
			{
				return Convert.ToDouble(this.timestamp);
			}
			set
			{
				this.timestamp = value.ToString("F0", CultureInfo.InvariantCulture);
			}
		}

		public BreadcrumbLevel Type
		{
			get
			{
				return (BreadcrumbLevel)Enum.Parse(typeof(BreadcrumbLevel), this.type, true);
			}
			set
			{
				this.type = Enum.GetName(typeof(BreadcrumbLevel), value).ToLower();
			}
		}

		public UnityEngineLogLevel Level
		{
			get
			{
				return (UnityEngineLogLevel)Enum.Parse(typeof(UnityEngineLogLevel), this.level, true);
			}
			set
			{
				this.level = Enum.GetName(typeof(UnityEngineLogLevel), value).ToLower();
			}
		}

		public string message;

		public string timestamp;

		public string type;

		public string level;

		[NonSerialized]
		public IDictionary<string, string> Attributes;
	}
}
