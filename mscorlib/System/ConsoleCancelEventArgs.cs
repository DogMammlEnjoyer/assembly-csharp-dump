using System;
using Unity;

namespace System
{
	/// <summary>Provides data for the <see cref="E:System.Console.CancelKeyPress" /> event. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class ConsoleCancelEventArgs : EventArgs
	{
		internal ConsoleCancelEventArgs(ConsoleSpecialKey type)
		{
			this._type = type;
		}

		/// <summary>Gets or sets a value that indicates whether simultaneously pressing the <see cref="F:System.ConsoleModifiers.Control" /> modifier key and the <see cref="F:System.ConsoleKey.C" /> console key (Ctrl+C) or the Ctrl+Break keys terminates the current process. The default is <see langword="false" />, which terminates the current process.</summary>
		/// <returns>
		///   <see langword="true" /> if the current process should resume when the event handler concludes; <see langword="false" /> if the current process should terminate. The default value is <see langword="false" />; the current process terminates when the event handler returns. If <see langword="true" />, the current process continues.</returns>
		public bool Cancel { get; set; }

		/// <summary>Gets the combination of modifier and console keys that interrupted the current process.</summary>
		/// <returns>One of the enumeration values that specifies the key combination that interrupted the current process. There is no default value.</returns>
		public ConsoleSpecialKey SpecialKey
		{
			get
			{
				return this._type;
			}
		}

		internal ConsoleCancelEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly ConsoleSpecialKey _type;
	}
}
