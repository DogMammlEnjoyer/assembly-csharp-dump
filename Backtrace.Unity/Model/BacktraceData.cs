using System;
using System.Collections.Generic;
using Backtrace.Unity.Json;
using Backtrace.Unity.Model.JsonData;

namespace Backtrace.Unity.Model
{
	public class BacktraceData
	{
		public Guid Uuid { get; private set; }

		internal string UuidString
		{
			get
			{
				if (string.IsNullOrEmpty(this._uuidString))
				{
					this._uuidString = this.Uuid.ToString();
				}
				return this._uuidString;
			}
		}

		public long Timestamp { get; private set; }

		public BacktraceReport Report { get; set; }

		public BacktraceData(BacktraceReport report, Dictionary<string, string> clientAttributes = null, int gameObjectDepth = -1)
		{
			if (report == null)
			{
				return;
			}
			this.Report = report;
			this.Uuid = this.Report.Uuid;
			this.Timestamp = this.Report.Timestamp;
			string[] classifier;
			if (!this.Report.ExceptionTypeReport)
			{
				classifier = new string[0];
			}
			else
			{
				(classifier = new string[1])[0] = this.Report.Classifier;
			}
			this.Classifier = classifier;
			this.Symbolication = report.Symbolication;
			this.SetAttributes(clientAttributes, gameObjectDepth);
			this.SetThreadInformations();
			this.Attachments = new HashSet<string>(this.Report.AttachmentPaths);
		}

		public string ToJson()
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject(new Dictionary<string, string>
			{
				{
					"uuid",
					this.UuidString
				},
				{
					"lang",
					"csharp"
				},
				{
					"langVersion",
					this.LangVersion
				},
				{
					"agent",
					"backtrace-unity"
				},
				{
					"agentVersion",
					"3.9.1"
				},
				{
					"mainThread",
					this.MainThread
				}
			});
			backtraceJObject.Add("timestamp", this.Timestamp);
			backtraceJObject.Add("classifiers", this.Classifier);
			backtraceJObject.Add("attributes", this.Attributes.ToJson());
			backtraceJObject.Add("annotations", this.Annotation.ToJson());
			backtraceJObject.Add("threads", this.ThreadData.ToJson());
			if (!string.IsNullOrEmpty(this.Symbolication))
			{
				backtraceJObject.Add("symbolication", this.Symbolication);
			}
			if (this.SourceCode != null)
			{
				backtraceJObject.Add("sourceCode", this.SourceCode.ToJson());
			}
			return backtraceJObject.ToJson();
		}

		private void SetThreadInformations()
		{
			bool faultingThread = !(this.Report.Exception is BacktraceUnhandledException) || !string.IsNullOrEmpty(this.Report.Exception.StackTrace);
			this.ThreadData = new ThreadData(this.Report.DiagnosticStack, faultingThread);
			this.ThreadInformations = this.ThreadData.ThreadInformations;
			this.MainThread = this.ThreadData.MainThread;
			this.SourceCode = this.Report.SourceCode;
		}

		private void SetAttributes(Dictionary<string, string> clientAttributes, int gameObjectDepth)
		{
			this.Attributes = new BacktraceAttributes(this.Report, clientAttributes);
			this.Annotation = new Annotations(this.Report.ExceptionTypeReport ? this.Report.Exception : null, gameObjectDepth);
		}

		private string _uuidString;

		public const string Lang = "csharp";

		public readonly string LangVersion = "Mono";

		public const string Agent = "backtrace-unity";

		public const string AgentVersion = "3.9.1";

		public Dictionary<string, ThreadInformation> ThreadInformations;

		public string MainThread;

		public string[] Classifier;

		public string Symbolication;

		public BacktraceSourceCode SourceCode;

		public ICollection<string> Attachments;

		public BacktraceAttributes Attributes;

		public Annotations Annotation;

		public ThreadData ThreadData;

		public int Deduplication;
	}
}
