using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	/// <summary>Each property of the <see cref="T:System.Drawing.TextureBrush" /> class is a <see cref="T:System.Drawing.Brush" /> object that uses an image to fill the interior of a shape. This class cannot be inherited.</summary>
	public sealed class TextureBrush : Brush
	{
		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image.</summary>
		/// <param name="bitmap">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		public TextureBrush(Image bitmap) : this(bitmap, WrapMode.Tile)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image and wrap mode.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="wrapMode">A <see cref="T:System.Drawing.Drawing2D.WrapMode" /> enumeration that specifies how this <see cref="T:System.Drawing.TextureBrush" /> object is tiled.</param>
		public TextureBrush(Image image, WrapMode wrapMode)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("wrapMode", (int)wrapMode, typeof(WrapMode));
			}
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTexture(new HandleRef(image, image.nativeImage), (int)wrapMode, out zero));
			base.SetNativeBrushInternal(zero);
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image, wrap mode, and bounding rectangle.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="wrapMode">A <see cref="T:System.Drawing.Drawing2D.WrapMode" /> enumeration that specifies how this <see cref="T:System.Drawing.TextureBrush" /> object is tiled.</param>
		/// <param name="dstRect">A <see cref="T:System.Drawing.RectangleF" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		public TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("wrapMode", (int)wrapMode, typeof(WrapMode));
			}
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTexture2(new HandleRef(image, image.nativeImage), (int)wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out zero));
			base.SetNativeBrushInternal(zero);
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image, wrap mode, and bounding rectangle.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="wrapMode">A <see cref="T:System.Drawing.Drawing2D.WrapMode" /> enumeration that specifies how this <see cref="T:System.Drawing.TextureBrush" /> object is tiled.</param>
		/// <param name="dstRect">A <see cref="T:System.Drawing.Rectangle" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		public TextureBrush(Image image, WrapMode wrapMode, Rectangle dstRect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("wrapMode", (int)wrapMode, typeof(WrapMode));
			}
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTexture2I(new HandleRef(image, image.nativeImage), (int)wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out zero));
			base.SetNativeBrushInternal(zero);
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image and bounding rectangle.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="dstRect">A <see cref="T:System.Drawing.RectangleF" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		public TextureBrush(Image image, RectangleF dstRect) : this(image, dstRect, null)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image, bounding rectangle, and image attributes.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="dstRect">A <see cref="T:System.Drawing.RectangleF" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		/// <param name="imageAttr">An <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object that contains additional information about the image used by this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		public TextureBrush(Image image, RectangleF dstRect, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTextureIA(new HandleRef(image, image.nativeImage), new HandleRef(imageAttr, (imageAttr == null) ? IntPtr.Zero : imageAttr.nativeImageAttributes), dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out zero));
			base.SetNativeBrushInternal(zero);
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image and bounding rectangle.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="dstRect">A <see cref="T:System.Drawing.Rectangle" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		public TextureBrush(Image image, Rectangle dstRect) : this(image, dstRect, null)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.TextureBrush" /> object that uses the specified image, bounding rectangle, and image attributes.</summary>
		/// <param name="image">The <see cref="T:System.Drawing.Image" /> object with which this <see cref="T:System.Drawing.TextureBrush" /> object fills interiors.</param>
		/// <param name="dstRect">A <see cref="T:System.Drawing.Rectangle" /> structure that represents the bounding rectangle for this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		/// <param name="imageAttr">An <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object that contains additional information about the image used by this <see cref="T:System.Drawing.TextureBrush" /> object.</param>
		public TextureBrush(Image image, Rectangle dstRect, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTextureIAI(new HandleRef(image, image.nativeImage), new HandleRef(imageAttr, (imageAttr == null) ? IntPtr.Zero : imageAttr.nativeImageAttributes), dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out zero));
			base.SetNativeBrushInternal(zero);
		}

		internal TextureBrush(IntPtr nativeBrush)
		{
			base.SetNativeBrushInternal(nativeBrush);
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.TextureBrush" /> object.</summary>
		/// <returns>The <see cref="T:System.Drawing.TextureBrush" /> object this method creates, cast as an <see cref="T:System.Object" /> object.</returns>
		public override object Clone()
		{
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out zero));
			return new TextureBrush(zero);
		}

		/// <summary>Gets or sets a copy of the <see cref="T:System.Drawing.Drawing2D.Matrix" /> object that defines a local geometric transformation for the image associated with this <see cref="T:System.Drawing.TextureBrush" /> object.</summary>
		/// <returns>A copy of the <see cref="T:System.Drawing.Drawing2D.Matrix" /> object that defines a geometric transformation that applies only to fills drawn by using this <see cref="T:System.Drawing.TextureBrush" /> object.</returns>
		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetTextureTransform(new HandleRef(this, base.NativeBrush), new HandleRef(matrix, matrix.nativeMatrix)));
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetTextureTransform(new HandleRef(this, base.NativeBrush), new HandleRef(value, value.nativeMatrix)));
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Drawing.Drawing2D.WrapMode" /> enumeration that indicates the wrap mode for this <see cref="T:System.Drawing.TextureBrush" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.Drawing2D.WrapMode" /> enumeration that specifies how fills drawn by using this <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> object are tiled.</returns>
		public WrapMode WrapMode
		{
			get
			{
				int result = 0;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetTextureWrapMode(new HandleRef(this, base.NativeBrush), out result));
				return (WrapMode)result;
			}
			set
			{
				if (value < WrapMode.Tile || value > WrapMode.Clamp)
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(WrapMode));
				}
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetTextureWrapMode(new HandleRef(this, base.NativeBrush), (int)value));
			}
		}

		/// <summary>Gets the <see cref="T:System.Drawing.Image" /> object associated with this <see cref="T:System.Drawing.TextureBrush" /> object.</summary>
		/// <returns>An <see cref="T:System.Drawing.Image" /> object that represents the image with which this <see cref="T:System.Drawing.TextureBrush" /> object fills shapes.</returns>
		public Image Image
		{
			get
			{
				IntPtr nativeImage;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetTextureImage(new HandleRef(this, base.NativeBrush), out nativeImage));
				return Image.CreateImageObject(nativeImage);
			}
		}

		/// <summary>Resets the <see langword="Transform" /> property of this <see cref="T:System.Drawing.TextureBrush" /> object to identity.</summary>
		public void ResetTransform()
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipResetTextureTransform(new HandleRef(this, base.NativeBrush)));
		}

		/// <summary>Multiplies the <see cref="T:System.Drawing.Drawing2D.Matrix" /> object that represents the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified <see cref="T:System.Drawing.Drawing2D.Matrix" /> object by prepending the specified <see cref="T:System.Drawing.Drawing2D.Matrix" /> object.</summary>
		/// <param name="matrix">The <see cref="T:System.Drawing.Drawing2D.Matrix" /> object by which to multiply the geometric transformation.</param>
		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		/// <summary>Multiplies the <see cref="T:System.Drawing.Drawing2D.Matrix" /> object that represents the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified <see cref="T:System.Drawing.Drawing2D.Matrix" /> object in the specified order.</summary>
		/// <param name="matrix">The <see cref="T:System.Drawing.Drawing2D.Matrix" /> object by which to multiply the geometric transformation.</param>
		/// <param name="order">A <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> enumeration that specifies the order in which to multiply the two matrices.</param>
		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			if (matrix.nativeMatrix == IntPtr.Zero)
			{
				return;
			}
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipMultiplyTextureTransform(new HandleRef(this, base.NativeBrush), new HandleRef(matrix, matrix.nativeMatrix), order));
		}

		/// <summary>Translates the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified dimensions. This method prepends the translation to the transformation.</summary>
		/// <param name="dx">The dimension by which to translate the transformation in the x direction.</param>
		/// <param name="dy">The dimension by which to translate the transformation in the y direction.</param>
		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		/// <summary>Translates the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified dimensions in the specified order.</summary>
		/// <param name="dx">The dimension by which to translate the transformation in the x direction.</param>
		/// <param name="dy">The dimension by which to translate the transformation in the y direction.</param>
		/// <param name="order">The order (prepend or append) in which to apply the translation.</param>
		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipTranslateTextureTransform(new HandleRef(this, base.NativeBrush), dx, dy, order));
		}

		/// <summary>Scales the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified amounts. This method prepends the scaling matrix to the transformation.</summary>
		/// <param name="sx">The amount by which to scale the transformation in the x direction.</param>
		/// <param name="sy">The amount by which to scale the transformation in the y direction.</param>
		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		/// <summary>Scales the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified amounts in the specified order.</summary>
		/// <param name="sx">The amount by which to scale the transformation in the x direction.</param>
		/// <param name="sy">The amount by which to scale the transformation in the y direction.</param>
		/// <param name="order">A <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> enumeration that specifies whether to append or prepend the scaling matrix.</param>
		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipScaleTextureTransform(new HandleRef(this, base.NativeBrush), sx, sy, order));
		}

		/// <summary>Rotates the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified amount. This method prepends the rotation to the transformation.</summary>
		/// <param name="angle">The angle of rotation.</param>
		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		/// <summary>Rotates the local geometric transformation of this <see cref="T:System.Drawing.TextureBrush" /> object by the specified amount in the specified order.</summary>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="order">A <see cref="T:System.Drawing.Drawing2D.MatrixOrder" /> enumeration that specifies whether to append or prepend the rotation matrix.</param>
		public void RotateTransform(float angle, MatrixOrder order)
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipRotateTextureTransform(new HandleRef(this, base.NativeBrush), angle, order));
		}
	}
}
