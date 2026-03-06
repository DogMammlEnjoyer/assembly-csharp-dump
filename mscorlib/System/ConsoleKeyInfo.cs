using System;

namespace System
{
	/// <summary>Describes the console key that was pressed, including the character represented by the console key and the state of the SHIFT, ALT, and CTRL modifier keys.</summary>
	[Serializable]
	public readonly struct ConsoleKeyInfo
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ConsoleKeyInfo" /> structure using the specified character, console key, and modifier keys.</summary>
		/// <param name="keyChar">The Unicode character that corresponds to the <paramref name="key" /> parameter.</param>
		/// <param name="key">The console key that corresponds to the <paramref name="keyChar" /> parameter.</param>
		/// <param name="shift">
		///   <see langword="true" /> to indicate that a SHIFT key was pressed; otherwise, <see langword="false" />.</param>
		/// <param name="alt">
		///   <see langword="true" /> to indicate that an ALT key was pressed; otherwise, <see langword="false" />.</param>
		/// <param name="control">
		///   <see langword="true" /> to indicate that a CTRL key was pressed; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The numeric value of the <paramref name="key" /> parameter is less than 0 or greater than 255.</exception>
		public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
		{
			if (key < (ConsoleKey)0 || key > (ConsoleKey)255)
			{
				throw new ArgumentOutOfRangeException("key", "Console key values must be between 0 and 255 inclusive.");
			}
			this._keyChar = keyChar;
			this._key = key;
			this._mods = (ConsoleModifiers)0;
			if (shift)
			{
				this._mods |= ConsoleModifiers.Shift;
			}
			if (alt)
			{
				this._mods |= ConsoleModifiers.Alt;
			}
			if (control)
			{
				this._mods |= ConsoleModifiers.Control;
			}
		}

		/// <summary>Gets the Unicode character represented by the current <see cref="T:System.ConsoleKeyInfo" /> object.</summary>
		/// <returns>An object that corresponds to the console key represented by the current <see cref="T:System.ConsoleKeyInfo" /> object.</returns>
		public char KeyChar
		{
			get
			{
				return this._keyChar;
			}
		}

		/// <summary>Gets the console key represented by the current <see cref="T:System.ConsoleKeyInfo" /> object.</summary>
		/// <returns>A value that identifies the console key that was pressed.</returns>
		public ConsoleKey Key
		{
			get
			{
				return this._key;
			}
		}

		/// <summary>Gets a bitwise combination of <see cref="T:System.ConsoleModifiers" /> values that specifies one or more modifier keys pressed simultaneously with the console key.</summary>
		/// <returns>A bitwise combination of the enumeration values. There is no default value.</returns>
		public ConsoleModifiers Modifiers
		{
			get
			{
				return this._mods;
			}
		}

		/// <summary>Gets a value indicating whether the specified object is equal to the current <see cref="T:System.ConsoleKeyInfo" /> object.</summary>
		/// <param name="value">An object to compare to the current <see cref="T:System.ConsoleKeyInfo" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is a <see cref="T:System.ConsoleKeyInfo" /> object and is equal to the current <see cref="T:System.ConsoleKeyInfo" /> object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			return value is ConsoleKeyInfo && this.Equals((ConsoleKeyInfo)value);
		}

		/// <summary>Gets a value indicating whether the specified <see cref="T:System.ConsoleKeyInfo" /> object is equal to the current <see cref="T:System.ConsoleKeyInfo" /> object.</summary>
		/// <param name="obj">An object to compare to the current <see cref="T:System.ConsoleKeyInfo" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is equal to the current <see cref="T:System.ConsoleKeyInfo" /> object; otherwise, <see langword="false" />.</returns>
		public bool Equals(ConsoleKeyInfo obj)
		{
			return obj._keyChar == this._keyChar && obj._key == this._key && obj._mods == this._mods;
		}

		/// <summary>Indicates whether the specified <see cref="T:System.ConsoleKeyInfo" /> objects are equal.</summary>
		/// <param name="a">The first object to compare.</param>
		/// <param name="b">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(ConsoleKeyInfo a, ConsoleKeyInfo b)
		{
			return a.Equals(b);
		}

		/// <summary>Indicates whether the specified <see cref="T:System.ConsoleKeyInfo" /> objects are not equal.</summary>
		/// <param name="a">The first object to compare.</param>
		/// <param name="b">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(ConsoleKeyInfo a, ConsoleKeyInfo b)
		{
			return !(a == b);
		}

		/// <summary>Returns the hash code for the current <see cref="T:System.ConsoleKeyInfo" /> object.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (int)((ConsoleKey)this._keyChar | (int)this._key << 16 | (ConsoleKey)((int)this._mods << 24));
		}

		private readonly char _keyChar;

		private readonly ConsoleKey _key;

		private readonly ConsoleModifiers _mods;
	}
}
