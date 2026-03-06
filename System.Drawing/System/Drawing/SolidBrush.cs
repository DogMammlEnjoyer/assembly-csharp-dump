using System;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	/// <summary>Defines a brush of a single color. Brushes are used to fill graphics shapes, such as rectangles, ellipses, pies, polygons, and paths. This class cannot be inherited.</summary>
	public sealed class SolidBrush : Brush
	{
		/// <summary>Initializes a new <see cref="T:System.Drawing.SolidBrush" /> object of the specified color.</summary>
		/// <param name="color">A <see cref="T:System.Drawing.Color" /> structure that represents the color of this brush.</param>
		public SolidBrush(Color color)
		{
			this._color = color;
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateSolidFill(this._color.ToArgb(), out zero));
			base.SetNativeBrushInternal(zero);
		}

		internal SolidBrush(Color color, bool immutable) : this(color)
		{
			this._immutable = immutable;
		}

		internal SolidBrush(IntPtr nativeBrush)
		{
			base.SetNativeBrushInternal(nativeBrush);
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.SolidBrush" /> object.</summary>
		/// <returns>The <see cref="T:System.Drawing.SolidBrush" /> object that this method creates.</returns>
		public override object Clone()
		{
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out zero));
			return new SolidBrush(zero);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing)
			{
				this._immutable = false;
			}
			else if (this._immutable)
			{
				throw new ArgumentException(SR.Format("Changes cannot be made to {0} because permissions are not valid.", new object[]
				{
					"Brush"
				}));
			}
			base.Dispose(disposing);
		}

		/// <summary>Gets or sets the color of this <see cref="T:System.Drawing.SolidBrush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> structure that represents the color of this brush.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.SolidBrush.Color" /> property is set on an immutable <see cref="T:System.Drawing.SolidBrush" />.</exception>
		public Color Color
		{
			get
			{
				if (this._color == Color.Empty)
				{
					int argb;
					SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetSolidFillColor(new HandleRef(this, base.NativeBrush), out argb));
					this._color = Color.FromArgb(argb);
				}
				return this._color;
			}
			set
			{
				if (this._immutable)
				{
					throw new ArgumentException(SR.Format("Changes cannot be made to {0} because permissions are not valid.", new object[]
					{
						"Brush"
					}));
				}
				if (this._color != value)
				{
					Color color = this._color;
					this.InternalSetColor(value);
				}
			}
		}

		private void InternalSetColor(Color value)
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetSolidFillColor(new HandleRef(this, base.NativeBrush), value.ToArgb()));
			this._color = value;
		}

		private Color _color = Color.Empty;

		private bool _immutable;
	}
}
