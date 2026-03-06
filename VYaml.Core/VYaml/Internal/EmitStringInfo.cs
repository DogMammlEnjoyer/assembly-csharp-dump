using System;
using VYaml.Emitter;

namespace VYaml.Internal
{
	internal readonly struct EmitStringInfo
	{
		public EmitStringInfo(int lines, bool needsQuotes, bool isReservedWord)
		{
			this.Lines = lines;
			this.NeedsQuotes = needsQuotes;
			this.IsReservedWord = isReservedWord;
		}

		public ScalarStyle SuggestScalarStyle()
		{
			if (this.Lines > 1)
			{
				return ScalarStyle.Literal;
			}
			if (!this.NeedsQuotes)
			{
				return ScalarStyle.Plain;
			}
			return ScalarStyle.DoubleQuoted;
		}

		public readonly int Lines;

		public readonly bool NeedsQuotes;

		public readonly bool IsReservedWord;
	}
}
