using System;
using System.IO;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Core.Output
{
	public class TextWriterOutput : IOutput
	{
		public TextWriterOutput(TextWriter output)
		{
			this.Output = output;
		}

		public TextWriter Output { get; }

		public void Write(string text, IFormattingInfo formattingInfo)
		{
			this.Output.Write(text);
		}

		public void Write(string text, int startIndex, int length, IFormattingInfo formattingInfo)
		{
			this.Output.Write(text.Substring(startIndex, length));
		}
	}
}
