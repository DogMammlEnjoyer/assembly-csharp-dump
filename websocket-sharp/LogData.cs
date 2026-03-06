using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace WebSocketSharp
{
	public class LogData
	{
		internal LogData(LogLevel level, StackFrame caller, string message)
		{
			this._level = level;
			this._caller = caller;
			this._message = (message ?? string.Empty);
			this._date = DateTime.Now;
		}

		public StackFrame Caller
		{
			get
			{
				return this._caller;
			}
		}

		public DateTime Date
		{
			get
			{
				return this._date;
			}
		}

		public LogLevel Level
		{
			get
			{
				return this._level;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public override string ToString()
		{
			string text = string.Format("{0}|{1,-5}|", this._date, this._level);
			MethodBase method = this._caller.GetMethod();
			Type declaringType = method.DeclaringType;
			int fileLineNumber = this._caller.GetFileLineNumber();
			string arg = string.Format("{0}{1}.{2}:{3}|", new object[]
			{
				text,
				declaringType.Name,
				method.Name,
				fileLineNumber
			});
			string[] array = this._message.Replace("\r\n", "\n").TrimEnd(new char[]
			{
				'\n'
			}).Split(new char[]
			{
				'\n'
			});
			bool flag = array.Length <= 1;
			string result;
			if (flag)
			{
				result = string.Format("{0}{1}", arg, this._message);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1}\n", arg, array[0]), 64);
				string format = string.Format("{{0,{0}}}{{1}}\n", text.Length);
				for (int i = 1; i < array.Length; i++)
				{
					stringBuilder.AppendFormat(format, "", array[i]);
				}
				StringBuilder stringBuilder2 = stringBuilder;
				int length = stringBuilder2.Length;
				stringBuilder2.Length = length - 1;
				result = stringBuilder.ToString();
			}
			return result;
		}

		private StackFrame _caller;

		private DateTime _date;

		private LogLevel _level;

		private string _message;
	}
}
