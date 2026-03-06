using System;
using System.Collections.Generic;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model
{
	public class BacktraceSourceCode
	{
		public string Text { get; set; }

		internal BacktraceJObject ToJson()
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			BacktraceJObject backtraceJObject2 = new BacktraceJObject(new Dictionary<string, string>
			{
				{
					"id",
					BacktraceSourceCode.SOURCE_CODE_PROPERTY
				},
				{
					"type",
					this.Type
				},
				{
					"title",
					this.Title
				},
				{
					"text",
					this.Text
				}
			});
			backtraceJObject2.Add("highlightLine", false);
			backtraceJObject.Add(BacktraceSourceCode.SOURCE_CODE_PROPERTY, backtraceJObject2);
			return backtraceJObject;
		}

		internal static string SOURCE_CODE_PROPERTY = "main";

		public readonly string Type = "Text";

		public readonly string Title = "Log File";
	}
}
