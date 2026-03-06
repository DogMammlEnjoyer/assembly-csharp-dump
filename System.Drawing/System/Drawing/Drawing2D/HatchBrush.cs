using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	/// <summary>Defines a rectangular brush with a hatch style, a foreground color, and a background color. This class cannot be inherited.</summary>
	public sealed class HatchBrush : Brush
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> class with the specified <see cref="T:System.Drawing.Drawing2D.HatchStyle" /> enumeration and foreground color.</summary>
		/// <param name="hatchstyle">One of the <see cref="T:System.Drawing.Drawing2D.HatchStyle" /> values that represents the pattern drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</param>
		/// <param name="foreColor">The <see cref="T:System.Drawing.Color" /> structure that represents the color of lines drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</param>
		public HatchBrush(HatchStyle hatchstyle, Color foreColor) : this(hatchstyle, foreColor, Color.FromArgb(-16777216))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> class with the specified <see cref="T:System.Drawing.Drawing2D.HatchStyle" /> enumeration, foreground color, and background color.</summary>
		/// <param name="hatchstyle">One of the <see cref="T:System.Drawing.Drawing2D.HatchStyle" /> values that represents the pattern drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</param>
		/// <param name="foreColor">The <see cref="T:System.Drawing.Color" /> structure that represents the color of lines drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</param>
		/// <param name="backColor">The <see cref="T:System.Drawing.Color" /> structure that represents the color of spaces between the lines drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</param>
		public HatchBrush(HatchStyle hatchstyle, Color foreColor, Color backColor)
		{
			if (hatchstyle < HatchStyle.Horizontal || hatchstyle > HatchStyle.SolidDiamond)
			{
				throw new ArgumentException(SR.Format("The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.", new object[]
				{
					"hatchstyle",
					hatchstyle,
					"HatchStyle"
				}), "hatchstyle");
			}
			IntPtr nativeBrushInternal;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateHatchBrush((int)hatchstyle, foreColor.ToArgb(), backColor.ToArgb(), out nativeBrushInternal));
			base.SetNativeBrushInternal(nativeBrushInternal);
		}

		internal HatchBrush(IntPtr nativeBrush)
		{
			base.SetNativeBrushInternal(nativeBrush);
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> object.</summary>
		/// <returns>The <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> this method creates, cast as an object.</returns>
		public override object Clone()
		{
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out zero));
			return new HatchBrush(zero);
		}

		/// <summary>Gets the hatch style of this <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> object.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.Drawing2D.HatchStyle" /> values that represents the pattern of this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</returns>
		public HatchStyle HatchStyle
		{
			get
			{
				int result;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchStyle(new HandleRef(this, base.NativeBrush), out result));
				return (HatchStyle)result;
			}
		}

		/// <summary>Gets the color of hatch lines drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> structure that represents the foreground color for this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</returns>
		public Color ForegroundColor
		{
			get
			{
				int argb;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchForegroundColor(new HandleRef(this, base.NativeBrush), out argb));
				return Color.FromArgb(argb);
			}
		}

		/// <summary>Gets the color of spaces between the hatch lines drawn by this <see cref="T:System.Drawing.Drawing2D.HatchBrush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> structure that represents the background color for this <see cref="T:System.Drawing.Drawing2D.HatchBrush" />.</returns>
		public Color BackgroundColor
		{
			get
			{
				int argb;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchBackgroundColor(new HandleRef(this, base.NativeBrush), out argb));
				return Color.FromArgb(argb);
			}
		}
	}
}
