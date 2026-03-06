using System;
using System.Text;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Core.Output
{
	public class StringOutput : IOutput
	{
		public StringOutput()
		{
			this.output = new StringBuilder();
		}

		public StringOutput(int capacity)
		{
			this.output = new StringBuilder(capacity);
		}

		public StringOutput(StringBuilder output)
		{
			this.output = output;
		}

		public void SetCapacity(int capacity)
		{
			if (this.output.Capacity < capacity)
			{
				this.output.Capacity = capacity;
			}
		}

		public void Write(string text, IFormattingInfo formattingInfo)
		{
			this.output.Append(text);
		}

		public void Write(string text, int startIndex, int length, IFormattingInfo formattingInfo)
		{
			this.output.Append(text, startIndex, length);
		}

		public void Clear()
		{
			this.output.Clear();
		}

		public override string ToString()
		{
			return this.output.ToString();
		}

		private readonly StringBuilder output;
	}
}
