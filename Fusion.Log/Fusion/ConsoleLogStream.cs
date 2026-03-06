using System;

namespace Fusion
{
	public class ConsoleLogStream : TextWriterLogStream
	{
		public ConsoleLogStream(ConsoleColor color, string prefix = null) : base(Console.Out, false, prefix)
		{
			this._color = color;
		}

		public override void Log(ILogSource source, string message)
		{
			if (Console.ForegroundColor == this._color)
			{
				base.Log(source, message);
				return;
			}
			Console.ForegroundColor = this._color;
			try
			{
				base.Log(source, message);
			}
			finally
			{
				Console.ForegroundColor = ConsoleColor.Gray;
			}
		}

		public override void Log(ILogSource source, string message, Exception error)
		{
			if (Console.ForegroundColor == ConsoleColor.Red)
			{
				base.Log(source, message, error);
				return;
			}
			Console.ForegroundColor = this._color;
			try
			{
				base.Log(source, message, error);
			}
			finally
			{
				Console.ForegroundColor = ConsoleColor.Gray;
			}
		}

		private readonly ConsoleColor _color;
	}
}
