using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public sealed class DebugLogStream : IDisposable
	{
		public DebugLogStream(LogStream innerStream, LogStream warnStream, LogStream errorStream)
		{
			if (innerStream == null)
			{
				throw new ArgumentNullException("innerStream");
			}
			this.InfoStream = innerStream;
			if (warnStream == null)
			{
				throw new ArgumentNullException("warnStream");
			}
			this.WarnStream = warnStream;
			if (errorStream == null)
			{
				throw new ArgumentNullException("errorStream");
			}
			this.ErrorStream = errorStream;
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Log(ILogSource source, string message)
		{
			this.InfoStream.Log(source, message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Log(string message)
		{
			this.InfoStream.Log(message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Info(ILogSource source, string message)
		{
			this.InfoStream.Log(source, message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Info(string message)
		{
			this.InfoStream.Log(message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Error(ILogSource source, string message)
		{
			this.ErrorStream.Log(source, message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Error(string message)
		{
			this.ErrorStream.Log(message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Error(Exception message)
		{
			this.ErrorStream.Log(message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Exception(Exception message)
		{
			this.ErrorStream.Log(message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Warn(ILogSource source, string message)
		{
			this.WarnStream.Log(source, message);
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Warn(string message)
		{
			this.WarnStream.Log(message);
		}

		public void Dispose()
		{
			this.InfoStream.Dispose();
			this.WarnStream.Dispose();
			this.ErrorStream.Dispose();
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Log(object message)
		{
		}

		public readonly LogStream InfoStream;

		public readonly LogStream WarnStream;

		public readonly LogStream ErrorStream;
	}
}
