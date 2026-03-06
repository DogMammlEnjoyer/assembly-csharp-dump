using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Drawing.Printing
{
	/// <summary>Specifies the dimensions of the margins of a printed page.</summary>
	[TypeConverter(typeof(MarginsConverter))]
	[Serializable]
	public class Margins : ICloneable
	{
		[OnDeserialized]
		private void OnDeserializedMethod(StreamingContext context)
		{
			if (this._doubleLeft == 0.0 && this._left != 0)
			{
				this._doubleLeft = (double)this._left;
			}
			if (this._doubleRight == 0.0 && this._right != 0)
			{
				this._doubleRight = (double)this._right;
			}
			if (this._doubleTop == 0.0 && this._top != 0)
			{
				this._doubleTop = (double)this._top;
			}
			if (this._doubleBottom == 0.0 && this._bottom != 0)
			{
				this._doubleBottom = (double)this._bottom;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.Margins" /> class with 1-inch wide margins.</summary>
		public Margins() : this(100, 100, 100, 100)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.Margins" /> class with the specified left, right, top, and bottom margins.</summary>
		/// <param name="left">The left margin, in hundredths of an inch.</param>
		/// <param name="right">The right margin, in hundredths of an inch.</param>
		/// <param name="top">The top margin, in hundredths of an inch.</param>
		/// <param name="bottom">The bottom margin, in hundredths of an inch.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="left" /> parameter value is less than 0.  
		///  -or-  
		///  The <paramref name="right" /> parameter value is less than 0.  
		///  -or-  
		///  The <paramref name="top" /> parameter value is less than 0.  
		///  -or-  
		///  The <paramref name="bottom" /> parameter value is less than 0.</exception>
		public Margins(int left, int right, int top, int bottom)
		{
			this.CheckMargin(left, "left");
			this.CheckMargin(right, "right");
			this.CheckMargin(top, "top");
			this.CheckMargin(bottom, "bottom");
			this._left = left;
			this._right = right;
			this._top = top;
			this._bottom = bottom;
			this._doubleLeft = (double)left;
			this._doubleRight = (double)right;
			this._doubleTop = (double)top;
			this._doubleBottom = (double)bottom;
		}

		/// <summary>Gets or sets the left margin width, in hundredths of an inch.</summary>
		/// <returns>The left margin width, in hundredths of an inch.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.Margins.Left" /> property is set to a value that is less than 0.</exception>
		public int Left
		{
			get
			{
				return this._left;
			}
			set
			{
				this.CheckMargin(value, "Left");
				this._left = value;
				this._doubleLeft = (double)value;
			}
		}

		/// <summary>Gets or sets the right margin width, in hundredths of an inch.</summary>
		/// <returns>The right margin width, in hundredths of an inch.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.Margins.Right" /> property is set to a value that is less than 0.</exception>
		public int Right
		{
			get
			{
				return this._right;
			}
			set
			{
				this.CheckMargin(value, "Right");
				this._right = value;
				this._doubleRight = (double)value;
			}
		}

		/// <summary>Gets or sets the top margin width, in hundredths of an inch.</summary>
		/// <returns>The top margin width, in hundredths of an inch.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.Margins.Top" /> property is set to a value that is less than 0.</exception>
		public int Top
		{
			get
			{
				return this._top;
			}
			set
			{
				this.CheckMargin(value, "Top");
				this._top = value;
				this._doubleTop = (double)value;
			}
		}

		/// <summary>Gets or sets the bottom margin, in hundredths of an inch.</summary>
		/// <returns>The bottom margin, in hundredths of an inch.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.Margins.Bottom" /> property is set to a value that is less than 0.</exception>
		public int Bottom
		{
			get
			{
				return this._bottom;
			}
			set
			{
				this.CheckMargin(value, "Bottom");
				this._bottom = value;
				this._doubleBottom = (double)value;
			}
		}

		internal double DoubleLeft
		{
			get
			{
				return this._doubleLeft;
			}
			set
			{
				this.Left = (int)Math.Round(value);
				this._doubleLeft = value;
			}
		}

		internal double DoubleRight
		{
			get
			{
				return this._doubleRight;
			}
			set
			{
				this.Right = (int)Math.Round(value);
				this._doubleRight = value;
			}
		}

		internal double DoubleTop
		{
			get
			{
				return this._doubleTop;
			}
			set
			{
				this.Top = (int)Math.Round(value);
				this._doubleTop = value;
			}
		}

		internal double DoubleBottom
		{
			get
			{
				return this._doubleBottom;
			}
			set
			{
				this.Bottom = (int)Math.Round(value);
				this._doubleBottom = value;
			}
		}

		private void CheckMargin(int margin, string name)
		{
			if (margin < 0)
			{
				throw new ArgumentException(SR.Format("Value of '{1}' is not valid for '{0}'. '{0}' must be greater than or equal to {2}.", new object[]
				{
					name,
					margin,
					"0"
				}));
			}
		}

		/// <summary>Retrieves a duplicate of this object, member by member.</summary>
		/// <returns>A duplicate of this object.</returns>
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Compares this <see cref="T:System.Drawing.Printing.Margins" /> to the specified <see cref="T:System.Object" /> to determine whether they have the same dimensions.</summary>
		/// <param name="obj">The object to which to compare this <see cref="T:System.Drawing.Printing.Margins" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is a <see cref="T:System.Drawing.Printing.Margins" /> and has the same <see cref="P:System.Drawing.Printing.Margins.Top" />, <see cref="P:System.Drawing.Printing.Margins.Bottom" />, <see cref="P:System.Drawing.Printing.Margins.Right" /> and <see cref="P:System.Drawing.Printing.Margins.Left" /> values as this <see cref="T:System.Drawing.Printing.Margins" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			Margins margins = obj as Margins;
			return margins == this || (!(margins == null) && (margins.Left == this.Left && margins.Right == this.Right && margins.Top == this.Top) && margins.Bottom == this.Bottom);
		}

		/// <summary>Calculates and retrieves a hash code based on the width of the left, right, top, and bottom margins.</summary>
		/// <returns>A hash code based on the left, right, top, and bottom margins.</returns>
		public override int GetHashCode()
		{
			int left = this.Left;
			uint right = (uint)this.Right;
			uint top = (uint)this.Top;
			uint bottom = (uint)this.Bottom;
			return left ^ (int)(right << 13 | right >> 19) ^ (int)(top << 26 | top >> 6) ^ (int)(bottom << 7 | bottom >> 25);
		}

		/// <summary>Compares two <see cref="T:System.Drawing.Printing.Margins" /> to determine if they have the same dimensions.</summary>
		/// <param name="m1">The first <see cref="T:System.Drawing.Printing.Margins" /> to compare for equality.</param>
		/// <param name="m2">The second <see cref="T:System.Drawing.Printing.Margins" /> to compare for equality.</param>
		/// <returns>
		///   <see langword="true" /> to indicate the <see cref="P:System.Drawing.Printing.Margins.Left" />, <see cref="P:System.Drawing.Printing.Margins.Right" />, <see cref="P:System.Drawing.Printing.Margins.Top" />, and <see cref="P:System.Drawing.Printing.Margins.Bottom" /> properties of both margins have the same value; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(Margins m1, Margins m2)
		{
			return m1 == null == (m2 == null) && (m1 == null || (m1.Left == m2.Left && m1.Top == m2.Top && m1.Right == m2.Right && m1.Bottom == m2.Bottom));
		}

		/// <summary>Compares two <see cref="T:System.Drawing.Printing.Margins" /> to determine whether they are of unequal width.</summary>
		/// <param name="m1">The first <see cref="T:System.Drawing.Printing.Margins" /> to compare for inequality.</param>
		/// <param name="m2">The second <see cref="T:System.Drawing.Printing.Margins" /> to compare for inequality.</param>
		/// <returns>
		///   <see langword="true" /> to indicate if the <see cref="P:System.Drawing.Printing.Margins.Left" />, <see cref="P:System.Drawing.Printing.Margins.Right" />, <see cref="P:System.Drawing.Printing.Margins.Top" />, or <see cref="P:System.Drawing.Printing.Margins.Bottom" /> properties of both margins are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(Margins m1, Margins m2)
		{
			return !(m1 == m2);
		}

		/// <summary>Converts the <see cref="T:System.Drawing.Printing.Margins" /> to a string.</summary>
		/// <returns>A <see cref="T:System.String" /> representation of the <see cref="T:System.Drawing.Printing.Margins" />.</returns>
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[Margins Left=",
				this.Left.ToString(CultureInfo.InvariantCulture),
				" Right=",
				this.Right.ToString(CultureInfo.InvariantCulture),
				" Top=",
				this.Top.ToString(CultureInfo.InvariantCulture),
				" Bottom=",
				this.Bottom.ToString(CultureInfo.InvariantCulture),
				"]"
			});
		}

		private int _left;

		private int _right;

		private int _bottom;

		private int _top;

		[OptionalField]
		private double _doubleLeft;

		[OptionalField]
		private double _doubleRight;

		[OptionalField]
		private double _doubleTop;

		[OptionalField]
		private double _doubleBottom;
	}
}
