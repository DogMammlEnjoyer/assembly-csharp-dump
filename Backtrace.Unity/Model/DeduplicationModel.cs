using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Backtrace.Unity.Extensions;
using Backtrace.Unity.Types;

namespace Backtrace.Unity.Model
{
	public class DeduplicationModel
	{
		public DeduplicationModel(BacktraceData backtraceData, DeduplicationStrategy strategy)
		{
			this._backtraceData = backtraceData;
			this._strategy = strategy;
		}

		public string StackTrace
		{
			get
			{
				if (this._strategy == DeduplicationStrategy.None)
				{
					return "";
				}
				if (this._backtraceData.Report == null || this._backtraceData.Report.DiagnosticStack == null)
				{
					return "";
				}
				string[] value = new HashSet<string>(from n in this._backtraceData.Report.DiagnosticStack
				select n.FunctionName into n
				orderby n descending
				select n).ToArray<string>();
				return string.Join(",", value);
			}
		}

		public string Classifier
		{
			get
			{
				if ((this._strategy & DeduplicationStrategy.Classifier) == DeduplicationStrategy.None)
				{
					return "";
				}
				string[] value = (this._backtraceData.Classifier != null) ? this._backtraceData.Classifier : new string[0];
				return string.Join(",", value);
			}
		}

		public string ExceptionMessage
		{
			get
			{
				if ((this._strategy & DeduplicationStrategy.Message) == DeduplicationStrategy.None)
				{
					return string.Empty;
				}
				if (this._backtraceData.Report == null || string.IsNullOrEmpty(this._backtraceData.Report.Message))
				{
					return string.Empty;
				}
				return this._backtraceData.Report.Message.OnlyLetters();
			}
		}

		public string Factor
		{
			get
			{
				if (this._backtraceData.Report == null)
				{
					return string.Empty;
				}
				return this._backtraceData.Report.Factor;
			}
		}

		public string GetSha()
		{
			if (!string.IsNullOrEmpty(this._backtraceData.Report.Fingerprint))
			{
				return this._backtraceData.Report.Fingerprint;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.ExceptionMessage);
			stringBuilder.Append(this.Classifier);
			stringBuilder.Append(this.StackTrace);
			return stringBuilder.GetSha();
		}

		private readonly BacktraceData _backtraceData;

		private readonly DeduplicationStrategy _strategy;
	}
}
