using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Fusion
{
	public abstract class FusionUnityLoggerBase : IDisposable
	{
		public FusionUnityLoggerBase(Thread mainThread = null, bool isDarkMode = false)
		{
			this._mainThread = (mainThread ?? Thread.CurrentThread);
			this.MinRandomColor = (isDarkMode ? new Color32(158, 158, 158, byte.MaxValue) : new Color32(30, 30, 30, byte.MaxValue));
			this.MaxRandomColor = (isDarkMode ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(90, 90, 90, byte.MaxValue));
			this.UseColorTags = true;
			this.UseGlobalPrefix = true;
			this.GlobalPrefixColor = FusionUnityLoggerBase.Color32ToRGBString(isDarkMode ? FusionUnityLoggerBase.DefaultDarkPrefixColor : FusionUnityLoggerBase.DefaultLightPrefixColor);
		}

		private bool IsInMainThread
		{
			get
			{
				return this._mainThread == Thread.CurrentThread;
			}
		}

		public void Dispose()
		{
			this._threadedStringBuilder.Dispose();
		}

		public LogStream CreateLogStream(LogLevel logLevel, LogFlags flags, TraceChannels channel)
		{
			return new UnityLogStream(this, logLevel, channel, flags);
		}

		protected internal virtual ValueTuple<string, Object> CreateMessage(in FusionUnityLoggerBase.LogContext context)
		{
			bool flag;
			StringBuilder threadSafeStringBuilder = this.GetThreadSafeStringBuilder(out flag);
			ILogSource source = context.Source;
			Object @object = (source != null) ? source.GetUnityObject() : null;
			ValueTuple<string, Object> result;
			try
			{
				this.AppendPrefix(threadSafeStringBuilder, context.Flags, context.Prefix);
				if (@object != null)
				{
					int length = threadSafeStringBuilder.Length;
					this.AppendNameThreadSafe(threadSafeStringBuilder, @object);
					if (threadSafeStringBuilder.Length > length)
					{
						threadSafeStringBuilder.Append(": ");
					}
				}
				threadSafeStringBuilder.Append(context.Message);
				result = new ValueTuple<string, Object>(threadSafeStringBuilder.ToString(), flag ? @object : null);
			}
			finally
			{
				threadSafeStringBuilder.Clear();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected StringBuilder GetThreadSafeStringBuilder(out bool isMainThread)
		{
			isMainThread = this.IsInMainThread;
			if (!this.IsInMainThread)
			{
				return this._threadedStringBuilder.Value;
			}
			return this._mainThreadBuilder;
		}

		protected void AppendPrefix(StringBuilder sb, LogFlags flags, string prefix)
		{
			if (this.UseGlobalPrefix)
			{
				if (this.UseColorTags)
				{
					sb.Append("<color=");
					sb.Append(this.GlobalPrefixColor);
					sb.Append(">");
				}
				sb.Append("[");
				sb.Append(this.GlobalPrefix);
				if (!string.IsNullOrEmpty(prefix))
				{
					sb.Append("/");
					sb.Append(prefix);
				}
				sb.Append("]");
				if (this.UseColorTags)
				{
					sb.Append("</color>");
				}
				sb.Append(" ");
			}
			else if (!string.IsNullOrEmpty(prefix))
			{
				sb.Append(prefix);
				sb.Append(": ");
			}
			if ((flags & LogFlags.Debug) == LogFlags.Debug)
			{
				sb.Append(this.DebugPrefix);
			}
			if ((flags & LogFlags.Trace) == LogFlags.Trace)
			{
				sb.Append(this.TracePrefix);
			}
		}

		public void AppendNameThreadSafe(StringBuilder builder, Object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text;
			if (obj == null)
			{
				text = this.NameUnavailableObjectDestroyedLabel;
			}
			else if (!this.IsInMainThread)
			{
				text = this.NameUnavailableInWorkerThreadLabel;
			}
			else
			{
				text = obj.name;
			}
			if (this.UseColorTags)
			{
				builder.AppendFormat("<color=#{0:X6}>", this.GetColorFromHash(text));
			}
			if (this.AddHashCodePrefix)
			{
				builder.AppendFormat("{0:X8}", obj.GetHashCode());
			}
			if (text != null && text.Length > 0)
			{
				if (this.AddHashCodePrefix)
				{
					builder.Append(" ");
				}
				builder.Append(text);
			}
			if (this.UseColorTags)
			{
				builder.Append("</color>");
			}
		}

		private int GetColorFromHash(string name)
		{
			int num = 0;
			for (int i = 0; i < name.Length; i++)
			{
				num = num * 31 + (int)name[i];
			}
			return FusionUnityLoggerBase.GetRandomColor(num, this.MinRandomColor, this.MaxRandomColor);
		}

		private static int GetRandomColor(int seed, Color32 min, Color32 max)
		{
			ulong num = (ulong)((long)seed);
			ulong num2 = FusionUnityLoggerBase.<GetRandomColor>g__NextSplitMix64|22_0(ref num);
			uint num3 = (uint)(max.r - min.r + 1);
			uint num4 = (uint)(max.g - min.g + 1);
			uint num5 = (uint)(max.b - min.b + 1);
			uint num6 = (uint)(num2 % (ulong)num3);
			ulong num7 = num2 / (ulong)num3;
			uint num8 = (uint)(num7 % (ulong)num4);
			uint num9 = (uint)(num7 / (ulong)num4 % (ulong)num5);
			return (int)(num6 << 16 | num8 << 8 | num9);
		}

		private static int Color32ToRGB24(Color32 c)
		{
			return (int)c.r << 16 | (int)c.g << 8 | (int)c.b;
		}

		private static string Color32ToRGBString(Color32 c)
		{
			return string.Format("#{0:X6}", FusionUnityLoggerBase.Color32ToRGB24(c));
		}

		public static Color DefaultLightPrefixColor
		{
			get
			{
				return new Color32(20, 64, 120, byte.MaxValue);
			}
		}

		public static Color DefaultDarkPrefixColor
		{
			get
			{
				return new Color32(115, 172, 229, byte.MaxValue);
			}
		}

		[CompilerGenerated]
		internal static ulong <GetRandomColor>g__NextSplitMix64|22_0(ref ulong x)
		{
			ulong num = x += 11400714819323198485UL;
			ulong num2 = (num ^ num >> 30) * 13787848793156543929UL;
			ulong num3 = (num2 ^ num2 >> 27) * 10723151780598845931UL;
			return num3 ^ num3 >> 31;
		}

		private readonly Thread _mainThread;

		private readonly StringBuilder _mainThreadBuilder = new StringBuilder();

		private readonly ThreadLocal<StringBuilder> _threadedStringBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());

		public bool AddHashCodePrefix;

		public string GlobalPrefix = "Fusion";

		public string GlobalPrefixColor;

		public Color32 MaxRandomColor;

		public Color32 MinRandomColor;

		public string NameUnavailableInWorkerThreadLabel = "";

		public string NameUnavailableObjectDestroyedLabel = "(destroyed)";

		public bool UseColorTags;

		public bool UseGlobalPrefix;

		public string DebugPrefix = "[DEBUG] ";

		public string TracePrefix = "[TRACE] ";

		protected internal readonly struct LogContext
		{
			public LogContext(string message, string prefix, ILogSource source, LogFlags flags)
			{
				this.Message = message;
				this.Source = source;
				this.Prefix = prefix;
				this.Flags = flags;
			}

			public readonly string Message;

			public readonly ILogSource Source;

			public readonly string Prefix;

			public readonly LogFlags Flags;
		}
	}
}
