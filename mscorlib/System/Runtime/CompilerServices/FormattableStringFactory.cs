using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>Provides a static method to create a <see cref="T:System.FormattableString" /> object from a composite format string and its arguments.</summary>
	public static class FormattableStringFactory
	{
		/// <summary>Creates a <see cref="T:System.FormattableString" /> instance from a composite format string and its arguments.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arguments">The arguments whose string representations are to be inserted in the result string.</param>
		/// <returns>The object that represents the composite format string and its arguments.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="arguments" /> is <see langword="null" />.</exception>
		public static FormattableString Create(string format, params object[] arguments)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			return new FormattableStringFactory.ConcreteFormattableString(format, arguments);
		}

		private sealed class ConcreteFormattableString : FormattableString
		{
			internal ConcreteFormattableString(string format, object[] arguments)
			{
				this._format = format;
				this._arguments = arguments;
			}

			public override string Format
			{
				get
				{
					return this._format;
				}
			}

			public override object[] GetArguments()
			{
				return this._arguments;
			}

			public override int ArgumentCount
			{
				get
				{
					return this._arguments.Length;
				}
			}

			public override object GetArgument(int index)
			{
				return this._arguments[index];
			}

			public override string ToString(IFormatProvider formatProvider)
			{
				return string.Format(formatProvider, this._format, this._arguments);
			}

			private readonly string _format;

			private readonly object[] _arguments;
		}
	}
}
