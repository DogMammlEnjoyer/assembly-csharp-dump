using System;
using System.ComponentModel;
using System.Drawing.Text;

namespace System.Drawing
{
	/// <summary>Encapsulates text layout information (such as alignment, orientation and tab stops) display manipulations (such as ellipsis insertion and national digit substitution) and OpenType features. This class cannot be inherited.</summary>
	public sealed class StringFormat : MarshalByRefObject, IDisposable, ICloneable
	{
		/// <summary>Initializes a new <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		public StringFormat() : this((StringFormatFlags)0, 0)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.StringFormat" /> object with the specified <see cref="T:System.Drawing.StringFormatFlags" /> enumeration and language.</summary>
		/// <param name="options">The <see cref="T:System.Drawing.StringFormatFlags" /> enumeration for the new <see cref="T:System.Drawing.StringFormat" /> object.</param>
		/// <param name="language">A value that indicates the language of the text.</param>
		public StringFormat(StringFormatFlags options, int language)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			GDIPlus.CheckStatus(GDIPlus.GdipCreateStringFormat(options, language, out this.nativeStrFmt));
		}

		internal StringFormat(IntPtr native)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			this.nativeStrFmt = native;
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~StringFormat()
		{
			this.Dispose(false);
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.nativeStrFmt != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteStringFormat(this.nativeStrFmt);
				this.nativeStrFmt = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
			}
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.StringFormat" /> object from the specified existing <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <param name="format">The <see cref="T:System.Drawing.StringFormat" /> object from which to initialize the new <see cref="T:System.Drawing.StringFormat" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		public StringFormat(StringFormat format)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCloneStringFormat(format.NativeObject, out this.nativeStrFmt));
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.StringFormat" /> object with the specified <see cref="T:System.Drawing.StringFormatFlags" /> enumeration.</summary>
		/// <param name="options">The <see cref="T:System.Drawing.StringFormatFlags" /> enumeration for the new <see cref="T:System.Drawing.StringFormat" /> object.</param>
		public StringFormat(StringFormatFlags options)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			GDIPlus.CheckStatus(GDIPlus.GdipCreateStringFormat(options, 0, out this.nativeStrFmt));
		}

		/// <summary>Gets or sets horizontal alignment of the string.</summary>
		/// <returns>A <see cref="T:System.Drawing.StringAlignment" /> enumeration that specifies the horizontal  alignment of the string.</returns>
		public StringAlignment Alignment
		{
			get
			{
				StringAlignment result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatAlign(this.nativeStrFmt, out result));
				return result;
			}
			set
			{
				if (value < StringAlignment.Near || value > StringAlignment.Far)
				{
					throw new InvalidEnumArgumentException("Alignment");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatAlign(this.nativeStrFmt, value));
			}
		}

		/// <summary>Gets or sets the vertical alignment of the string.</summary>
		/// <returns>A <see cref="T:System.Drawing.StringAlignment" /> enumeration that represents the vertical line alignment.</returns>
		public StringAlignment LineAlignment
		{
			get
			{
				StringAlignment result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatLineAlign(this.nativeStrFmt, out result));
				return result;
			}
			set
			{
				if (value < StringAlignment.Near || value > StringAlignment.Far)
				{
					throw new InvalidEnumArgumentException("Alignment");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatLineAlign(this.nativeStrFmt, value));
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Drawing.StringFormatFlags" /> enumeration that contains formatting information.</summary>
		/// <returns>A <see cref="T:System.Drawing.StringFormatFlags" /> enumeration that contains formatting information.</returns>
		public StringFormatFlags FormatFlags
		{
			get
			{
				StringFormatFlags result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatFlags(this.nativeStrFmt, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatFlags(this.nativeStrFmt, value));
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Drawing.Text.HotkeyPrefix" /> object for this <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <returns>The <see cref="T:System.Drawing.Text.HotkeyPrefix" /> object for this <see cref="T:System.Drawing.StringFormat" /> object, the default is <see cref="F:System.Drawing.Text.HotkeyPrefix.None" />.</returns>
		public HotkeyPrefix HotkeyPrefix
		{
			get
			{
				HotkeyPrefix result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatHotkeyPrefix(this.nativeStrFmt, out result));
				return result;
			}
			set
			{
				if (value < HotkeyPrefix.None || value > HotkeyPrefix.Hide)
				{
					throw new InvalidEnumArgumentException("HotkeyPrefix");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatHotkeyPrefix(this.nativeStrFmt, value));
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Drawing.StringTrimming" /> enumeration for this <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.StringTrimming" /> enumeration that indicates how text drawn with this <see cref="T:System.Drawing.StringFormat" /> object is trimmed when it exceeds the edges of the layout rectangle.</returns>
		public StringTrimming Trimming
		{
			get
			{
				StringTrimming result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatTrimming(this.nativeStrFmt, out result));
				return result;
			}
			set
			{
				if (value < StringTrimming.None || value > StringTrimming.EllipsisPath)
				{
					throw new InvalidEnumArgumentException("Trimming");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatTrimming(this.nativeStrFmt, value));
			}
		}

		/// <summary>Gets a generic default <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <returns>The generic default <see cref="T:System.Drawing.StringFormat" /> object.</returns>
		public static StringFormat GenericDefault
		{
			get
			{
				IntPtr native;
				GDIPlus.CheckStatus(GDIPlus.GdipStringFormatGetGenericDefault(out native));
				return new StringFormat(native);
			}
		}

		/// <summary>Gets the language that is used when local digits are substituted for western digits.</summary>
		/// <returns>A National Language Support (NLS) language identifier that identifies the language that will be used when local digits are substituted for western digits. You can pass the <see cref="P:System.Globalization.CultureInfo.LCID" /> property of a <see cref="T:System.Globalization.CultureInfo" /> object as the NLS language identifier. For example, suppose you create a <see cref="T:System.Globalization.CultureInfo" /> object by passing the string "ar-EG" to a <see cref="T:System.Globalization.CultureInfo" /> constructor. If you pass the <see cref="P:System.Globalization.CultureInfo.LCID" /> property of that <see cref="T:System.Globalization.CultureInfo" /> object along with <see cref="F:System.Drawing.StringDigitSubstitute.Traditional" /> to the <see cref="M:System.Drawing.StringFormat.SetDigitSubstitution(System.Int32,System.Drawing.StringDigitSubstitute)" /> method, then Arabic-Indic digits will be substituted for western digits at display time.</returns>
		public int DigitSubstitutionLanguage
		{
			get
			{
				return this.language;
			}
		}

		/// <summary>Gets a generic typographic <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <returns>A generic typographic <see cref="T:System.Drawing.StringFormat" /> object.</returns>
		public static StringFormat GenericTypographic
		{
			get
			{
				IntPtr native;
				GDIPlus.CheckStatus(GDIPlus.GdipStringFormatGetGenericTypographic(out native));
				return new StringFormat(native);
			}
		}

		/// <summary>Gets the method to be used for digit substitution.</summary>
		/// <returns>A <see cref="T:System.Drawing.StringDigitSubstitute" /> enumeration value that specifies how to substitute characters in a string that cannot be displayed because they are not supported by the current font.</returns>
		public StringDigitSubstitute DigitSubstitutionMethod
		{
			get
			{
				StringDigitSubstitute result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatDigitSubstitution(this.nativeStrFmt, this.language, out result));
				return result;
			}
		}

		/// <summary>Specifies an array of <see cref="T:System.Drawing.CharacterRange" /> structures that represent the ranges of characters measured by a call to the <see cref="M:System.Drawing.Graphics.MeasureCharacterRanges(System.String,System.Drawing.Font,System.Drawing.RectangleF,System.Drawing.StringFormat)" /> method.</summary>
		/// <param name="ranges">An array of <see cref="T:System.Drawing.CharacterRange" /> structures that specifies the ranges of characters measured by a call to the <see cref="M:System.Drawing.Graphics.MeasureCharacterRanges(System.String,System.Drawing.Font,System.Drawing.RectangleF,System.Drawing.StringFormat)" /> method.</param>
		/// <exception cref="T:System.OverflowException">More than 32 character ranges are set.</exception>
		public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatMeasurableCharacterRanges(this.nativeStrFmt, ranges.Length, ranges));
		}

		internal int GetMeasurableCharacterRangeCount()
		{
			int result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatMeasurableCharacterRangeCount(this.nativeStrFmt, out result));
			return result;
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <returns>The <see cref="T:System.Drawing.StringFormat" /> object this method creates.</returns>
		public object Clone()
		{
			IntPtr native;
			GDIPlus.CheckStatus(GDIPlus.GdipCloneStringFormat(this.nativeStrFmt, out native));
			return new StringFormat(native);
		}

		/// <summary>Converts this <see cref="T:System.Drawing.StringFormat" /> object to a human-readable string.</summary>
		/// <returns>A string representation of this <see cref="T:System.Drawing.StringFormat" /> object.</returns>
		public override string ToString()
		{
			return "[StringFormat, FormatFlags=" + this.FormatFlags.ToString() + "]";
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeStrFmt;
			}
			set
			{
				this.nativeStrFmt = value;
			}
		}

		internal IntPtr nativeFormat
		{
			get
			{
				return this.nativeStrFmt;
			}
		}

		/// <summary>Sets tab stops for this <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <param name="firstTabOffset">The number of spaces between the beginning of a line of text and the first tab stop.</param>
		/// <param name="tabStops">An array of distances between tab stops in the units specified by the <see cref="P:System.Drawing.Graphics.PageUnit" /> property.</param>
		public void SetTabStops(float firstTabOffset, float[] tabStops)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatTabStops(this.nativeStrFmt, firstTabOffset, tabStops.Length, tabStops));
		}

		/// <summary>Specifies the language and method to be used when local digits are substituted for western digits.</summary>
		/// <param name="language">A National Language Support (NLS) language identifier that identifies the language that will be used when local digits are substituted for western digits. You can pass the <see cref="P:System.Globalization.CultureInfo.LCID" /> property of a <see cref="T:System.Globalization.CultureInfo" /> object as the NLS language identifier. For example, suppose you create a <see cref="T:System.Globalization.CultureInfo" /> object by passing the string "ar-EG" to a <see cref="T:System.Globalization.CultureInfo" /> constructor. If you pass the <see cref="P:System.Globalization.CultureInfo.LCID" /> property of that <see cref="T:System.Globalization.CultureInfo" /> object along with <see cref="F:System.Drawing.StringDigitSubstitute.Traditional" /> to the <see cref="M:System.Drawing.StringFormat.SetDigitSubstitution(System.Int32,System.Drawing.StringDigitSubstitute)" /> method, then Arabic-Indic digits will be substituted for western digits at display time.</param>
		/// <param name="substitute">An element of the <see cref="T:System.Drawing.StringDigitSubstitute" /> enumeration that specifies how digits are displayed.</param>
		public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatDigitSubstitution(this.nativeStrFmt, this.language, substitute));
		}

		/// <summary>Gets the tab stops for this <see cref="T:System.Drawing.StringFormat" /> object.</summary>
		/// <param name="firstTabOffset">The number of spaces between the beginning of a text line and the first tab stop.</param>
		/// <returns>An array of distances (in number of spaces) between tab stops.</returns>
		public float[] GetTabStops(out float firstTabOffset)
		{
			int num = 0;
			firstTabOffset = 0f;
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatTabStopCount(this.nativeStrFmt, out num));
			float[] array = new float[num];
			if (num != 0)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatTabStops(this.nativeStrFmt, num, out firstTabOffset, array));
			}
			return array;
		}

		private IntPtr nativeStrFmt;

		private int language;
	}
}
