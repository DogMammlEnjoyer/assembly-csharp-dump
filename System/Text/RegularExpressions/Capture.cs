using System;
using Unity;

namespace System.Text.RegularExpressions
{
	/// <summary>Represents the results from a single successful subexpression capture.</summary>
	public class Capture
	{
		internal Capture(string text, int index, int length)
		{
			this.Text = text;
			this.Index = index;
			this.Length = length;
		}

		/// <summary>The position in the original string where the first character of the captured substring is found.</summary>
		/// <returns>The zero-based starting position in the original string where the captured substring is found.</returns>
		public int Index { get; private protected set; }

		/// <summary>Gets the length of the captured substring.</summary>
		/// <returns>The length of the captured substring.</returns>
		public int Length { get; private protected set; }

		protected internal string Text { internal get; private protected set; }

		/// <summary>Gets the captured substring from the input string.</summary>
		/// <returns>The substring that is captured by the match.</returns>
		public string Value
		{
			get
			{
				return this.Text.Substring(this.Index, this.Length);
			}
		}

		/// <summary>Retrieves the captured substring from the input string by calling the <see cref="P:System.Text.RegularExpressions.Capture.Value" /> property.</summary>
		/// <returns>The substring that was captured by the match.</returns>
		public override string ToString()
		{
			return this.Value;
		}

		internal ReadOnlySpan<char> GetLeftSubstring()
		{
			return this.Text.AsSpan(0, this.Index);
		}

		internal ReadOnlySpan<char> GetRightSubstring()
		{
			return this.Text.AsSpan(this.Index + this.Length, this.Text.Length - this.Index - this.Length);
		}

		internal Capture()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
