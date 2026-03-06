using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Unity;

namespace System.Globalization
{
	/// <summary>Enumerates the text elements of a string.</summary>
	[ComVisible(true)]
	[Serializable]
	public class TextElementEnumerator : IEnumerator
	{
		internal TextElementEnumerator(string str, int startIndex, int strLen)
		{
			this.str = str;
			this.startIndex = startIndex;
			this.strLen = strLen;
			this.Reset();
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext ctx)
		{
			this.charLen = -1;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			this.strLen = this.endIndex + 1;
			this.currTextElementLen = this.nextTextElementLen;
			if (this.charLen == -1)
			{
				this.uc = CharUnicodeInfo.InternalGetUnicodeCategory(this.str, this.index, out this.charLen);
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			this.endIndex = this.strLen - 1;
			this.nextTextElementLen = this.currTextElementLen;
		}

		/// <summary>Advances the enumerator to the next text element of the string.</summary>
		/// <returns>
		///   <see langword="true" /> if the enumerator was successfully advanced to the next text element; <see langword="false" /> if the enumerator has passed the end of the string.</returns>
		public bool MoveNext()
		{
			if (this.index >= this.strLen)
			{
				this.index = this.strLen + 1;
				return false;
			}
			this.currTextElementLen = StringInfo.GetCurrentTextElementLen(this.str, this.index, this.strLen, ref this.uc, ref this.charLen);
			this.index += this.currTextElementLen;
			return true;
		}

		/// <summary>Gets the current text element in the string.</summary>
		/// <returns>An object containing the current text element in the string.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first text element of the string or after the last text element.</exception>
		public object Current
		{
			get
			{
				return this.GetTextElement();
			}
		}

		/// <summary>Gets the current text element in the string.</summary>
		/// <returns>A new string containing the current text element in the string being read.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first text element of the string or after the last text element.</exception>
		public string GetTextElement()
		{
			if (this.index == this.startIndex)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Enumeration has not started. Call MoveNext."));
			}
			if (this.index > this.strLen)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Enumeration already finished."));
			}
			return this.str.Substring(this.index - this.currTextElementLen, this.currTextElementLen);
		}

		/// <summary>Gets the index of the text element that the enumerator is currently positioned over.</summary>
		/// <returns>The index of the text element that the enumerator is currently positioned over.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first text element of the string or after the last text element.</exception>
		public int ElementIndex
		{
			get
			{
				if (this.index == this.startIndex)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Enumeration has not started. Call MoveNext."));
				}
				return this.index - this.currTextElementLen;
			}
		}

		/// <summary>Sets the enumerator to its initial position, which is before the first text element in the string.</summary>
		public void Reset()
		{
			this.index = this.startIndex;
			if (this.index < this.strLen)
			{
				this.uc = CharUnicodeInfo.InternalGetUnicodeCategory(this.str, this.index, out this.charLen);
			}
		}

		internal TextElementEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string str;

		private int index;

		private int startIndex;

		[NonSerialized]
		private int strLen;

		[NonSerialized]
		private int currTextElementLen;

		[OptionalField(VersionAdded = 2)]
		private UnicodeCategory uc;

		[OptionalField(VersionAdded = 2)]
		private int charLen;

		private int endIndex;

		private int nextTextElementLen;
	}
}
