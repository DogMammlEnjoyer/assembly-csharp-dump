using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fusion
{
	public abstract class LogStream : IDisposable
	{
		public virtual void Log(ILogSource source, string message)
		{
			this.Log(message);
		}

		public abstract void Log(string message);

		public virtual void Log(ILogSource source, string message, Exception error)
		{
			this.Log(error);
		}

		public virtual void Log(ILogSource source, Exception error)
		{
			this.Log(error);
		}

		public virtual void Log(string message, Exception error)
		{
			this.Log(error);
		}

		public abstract void Log(Exception error);

		public virtual void Dispose()
		{
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Log(object message)
		{
			this.Log(string.Format("{0}", message));
		}

		[CanBeNull]
		public LogStream Once(ref bool flag)
		{
			if (!flag)
			{
				flag = true;
				return this;
			}
			return null;
		}
	}
}
