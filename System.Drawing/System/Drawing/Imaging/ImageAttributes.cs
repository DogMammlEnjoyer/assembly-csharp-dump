using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	/// <summary>Contains information about how bitmap and metafile colors are manipulated during rendering.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ImageAttributes : ICloneable, IDisposable
	{
		internal void SetNativeImageAttributes(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			this.nativeImageAttributes = handle;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Imaging.ImageAttributes" /> class.</summary>
		public ImageAttributes()
		{
			IntPtr zero = IntPtr.Zero;
			int num = GDIPlus.GdipCreateImageAttributes(out zero);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			this.SetNativeImageAttributes(zero);
		}

		internal ImageAttributes(IntPtr newNativeImageAttributes)
		{
			this.SetNativeImageAttributes(newNativeImageAttributes);
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.nativeImageAttributes != IntPtr.Zero)
			{
				try
				{
					GDIPlus.GdipDisposeImageAttributes(new HandleRef(this, this.nativeImageAttributes));
				}
				catch (Exception ex)
				{
					if (ClientUtils.IsSecurityOrCriticalException(ex))
					{
						throw;
					}
				}
				finally
				{
					this.nativeImageAttributes = IntPtr.Zero;
				}
			}
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~ImageAttributes()
		{
			this.Dispose(false);
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object.</summary>
		/// <returns>The <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object this class creates, cast as an object.</returns>
		public object Clone()
		{
			IntPtr zero = IntPtr.Zero;
			int num = GDIPlus.GdipCloneImageAttributes(new HandleRef(this, this.nativeImageAttributes), out zero);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return new ImageAttributes(zero);
		}

		/// <summary>Sets the color-adjustment matrix for the default category.</summary>
		/// <param name="newColorMatrix">The color-adjustment matrix.</param>
		public void SetColorMatrix(ColorMatrix newColorMatrix)
		{
			this.SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
		}

		/// <summary>Sets the color-adjustment matrix for the default category.</summary>
		/// <param name="newColorMatrix">The color-adjustment matrix.</param>
		/// <param name="flags">An element of <see cref="T:System.Drawing.Imaging.ColorMatrixFlag" /> that specifies the type of image and color that will be affected by the color-adjustment matrix.</param>
		public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag flags)
		{
			this.SetColorMatrix(newColorMatrix, flags, ColorAdjustType.Default);
		}

		/// <summary>Sets the color-adjustment matrix for a specified category.</summary>
		/// <param name="newColorMatrix">The color-adjustment matrix.</param>
		/// <param name="mode">An element of <see cref="T:System.Drawing.Imaging.ColorMatrixFlag" /> that specifies the type of image and color that will be affected by the color-adjustment matrix.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color-adjustment matrix is set.</param>
		public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag mode, ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesColorMatrix(new HandleRef(this, this.nativeImageAttributes), type, true, newColorMatrix, null, mode);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Clears the color-adjustment matrix for the default category.</summary>
		public void ClearColorMatrix()
		{
			this.ClearColorMatrix(ColorAdjustType.Default);
		}

		/// <summary>Clears the color-adjustment matrix for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color-adjustment matrix is cleared.</param>
		public void ClearColorMatrix(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesColorMatrix(new HandleRef(this, this.nativeImageAttributes), type, false, null, null, ColorMatrixFlag.Default);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the color-adjustment matrix and the grayscale-adjustment matrix for the default category.</summary>
		/// <param name="newColorMatrix">The color-adjustment matrix.</param>
		/// <param name="grayMatrix">The grayscale-adjustment matrix.</param>
		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix)
		{
			this.SetColorMatrices(newColorMatrix, grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
		}

		/// <summary>Sets the color-adjustment matrix and the grayscale-adjustment matrix for the default category.</summary>
		/// <param name="newColorMatrix">The color-adjustment matrix.</param>
		/// <param name="grayMatrix">The grayscale-adjustment matrix.</param>
		/// <param name="flags">An element of <see cref="T:System.Drawing.Imaging.ColorMatrixFlag" /> that specifies the type of image and color that will be affected by the color-adjustment and grayscale-adjustment matrices.</param>
		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags)
		{
			this.SetColorMatrices(newColorMatrix, grayMatrix, flags, ColorAdjustType.Default);
		}

		/// <summary>Sets the color-adjustment matrix and the grayscale-adjustment matrix for a specified category.</summary>
		/// <param name="newColorMatrix">The color-adjustment matrix.</param>
		/// <param name="grayMatrix">The grayscale-adjustment matrix.</param>
		/// <param name="mode">An element of <see cref="T:System.Drawing.Imaging.ColorMatrixFlag" /> that specifies the type of image and color that will be affected by the color-adjustment and grayscale-adjustment matrices.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color-adjustment and grayscale-adjustment matrices are set.</param>
		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag mode, ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesColorMatrix(new HandleRef(this, this.nativeImageAttributes), type, true, newColorMatrix, grayMatrix, mode);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the threshold (transparency range) for the default category.</summary>
		/// <param name="threshold">A real number that specifies the threshold value.</param>
		public void SetThreshold(float threshold)
		{
			this.SetThreshold(threshold, ColorAdjustType.Default);
		}

		/// <summary>Sets the threshold (transparency range) for a specified category.</summary>
		/// <param name="threshold">A threshold value from 0.0 to 1.0 that is used as a breakpoint to sort colors that will be mapped to either a maximum or a minimum value.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color threshold is set.</param>
		public void SetThreshold(float threshold, ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesThreshold(new HandleRef(this, this.nativeImageAttributes), type, true, threshold);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Clears the threshold value for the default category.</summary>
		public void ClearThreshold()
		{
			this.ClearThreshold(ColorAdjustType.Default);
		}

		/// <summary>Clears the threshold value for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the threshold is cleared.</param>
		public void ClearThreshold(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesThreshold(new HandleRef(this, this.nativeImageAttributes), type, false, 0f);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the gamma value for the default category.</summary>
		/// <param name="gamma">The gamma correction value.</param>
		public void SetGamma(float gamma)
		{
			this.SetGamma(gamma, ColorAdjustType.Default);
		}

		/// <summary>Sets the gamma value for a specified category.</summary>
		/// <param name="gamma">The gamma correction value.</param>
		/// <param name="type">An element of the <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> enumeration that specifies the category for which the gamma value is set.</param>
		public void SetGamma(float gamma, ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesGamma(new HandleRef(this, this.nativeImageAttributes), type, true, gamma);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Disables gamma correction for the default category.</summary>
		public void ClearGamma()
		{
			this.ClearGamma(ColorAdjustType.Default);
		}

		/// <summary>Disables gamma correction for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which gamma correction is disabled.</param>
		public void ClearGamma(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesGamma(new HandleRef(this, this.nativeImageAttributes), type, false, 0f);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Turns off color adjustment for the default category. You can call the <see cref="Overload:System.Drawing.Imaging.ImageAttributes.ClearNoOp" /> method to reinstate the color-adjustment settings that were in place before the call to the <see cref="Overload:System.Drawing.Imaging.ImageAttributes.SetNoOp" /> method.</summary>
		public void SetNoOp()
		{
			this.SetNoOp(ColorAdjustType.Default);
		}

		/// <summary>Turns off color adjustment for a specified category. You can call the <see cref="Overload:System.Drawing.Imaging.ImageAttributes.ClearNoOp" /> method to reinstate the color-adjustment settings that were in place before the call to the <see cref="Overload:System.Drawing.Imaging.ImageAttributes.SetNoOp" /> method.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which color correction is turned off.</param>
		public void SetNoOp(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesNoOp(new HandleRef(this, this.nativeImageAttributes), type, true);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Clears the <see langword="NoOp" /> setting for the default category.</summary>
		public void ClearNoOp()
		{
			this.ClearNoOp(ColorAdjustType.Default);
		}

		/// <summary>Clears the <see langword="NoOp" /> setting for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the <see langword="NoOp" /> setting is cleared.</param>
		public void ClearNoOp(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesNoOp(new HandleRef(this, this.nativeImageAttributes), type, false);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the color key for the default category.</summary>
		/// <param name="colorLow">The low color-key value.</param>
		/// <param name="colorHigh">The high color-key value.</param>
		public void SetColorKey(Color colorLow, Color colorHigh)
		{
			this.SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);
		}

		/// <summary>Sets the color key (transparency range) for a specified category.</summary>
		/// <param name="colorLow">The low color-key value.</param>
		/// <param name="colorHigh">The high color-key value.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color key is set.</param>
		public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type)
		{
			int colorLow2 = colorLow.ToArgb();
			int colorHigh2 = colorHigh.ToArgb();
			int num = GDIPlus.GdipSetImageAttributesColorKeys(new HandleRef(this, this.nativeImageAttributes), type, true, colorLow2, colorHigh2);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Clears the color key (transparency range) for the default category.</summary>
		public void ClearColorKey()
		{
			this.ClearColorKey(ColorAdjustType.Default);
		}

		/// <summary>Clears the color key (transparency range) for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color key is cleared.</param>
		public void ClearColorKey(ColorAdjustType type)
		{
			int num = 0;
			int num2 = GDIPlus.GdipSetImageAttributesColorKeys(new HandleRef(this, this.nativeImageAttributes), type, false, num, num);
			if (num2 != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num2);
			}
		}

		/// <summary>Sets the CMYK (cyan-magenta-yellow-black) output channel for the default category.</summary>
		/// <param name="flags">An element of <see cref="T:System.Drawing.Imaging.ColorChannelFlag" /> that specifies the output channel.</param>
		public void SetOutputChannel(ColorChannelFlag flags)
		{
			this.SetOutputChannel(flags, ColorAdjustType.Default);
		}

		/// <summary>Sets the CMYK (cyan-magenta-yellow-black) output channel for a specified category.</summary>
		/// <param name="flags">An element of <see cref="T:System.Drawing.Imaging.ColorChannelFlag" /> that specifies the output channel.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the output channel is set.</param>
		public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesOutputChannel(new HandleRef(this, this.nativeImageAttributes), type, true, flags);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Clears the CMYK (cyan-magenta-yellow-black) output channel setting for the default category.</summary>
		public void ClearOutputChannel()
		{
			this.ClearOutputChannel(ColorAdjustType.Default);
		}

		/// <summary>Clears the (cyan-magenta-yellow-black) output channel setting for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the output channel setting is cleared.</param>
		public void ClearOutputChannel(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesOutputChannel(new HandleRef(this, this.nativeImageAttributes), type, false, ColorChannelFlag.ColorChannelLast);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the output channel color-profile file for the default category.</summary>
		/// <param name="colorProfileFilename">The path name of a color-profile file. If the color-profile file is in the %SystemRoot%\System32\Spool\Drivers\Color directory, this parameter can be the file name. Otherwise, this parameter must be the fully qualified path name.</param>
		public void SetOutputChannelColorProfile(string colorProfileFilename)
		{
			this.SetOutputChannelColorProfile(colorProfileFilename, ColorAdjustType.Default);
		}

		/// <summary>Sets the output channel color-profile file for a specified category.</summary>
		/// <param name="colorProfileFilename">The path name of a color-profile file. If the color-profile file is in the %SystemRoot%\System32\Spool\Drivers\Color directory, this parameter can be the file name. Otherwise, this parameter must be the fully qualified path name.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the output channel color-profile file is set.</param>
		public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
		{
			Path.GetFullPath(colorProfileFilename);
			int num = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(new HandleRef(this, this.nativeImageAttributes), type, true, colorProfileFilename);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Clears the output channel color profile setting for the default category.</summary>
		public void ClearOutputChannelColorProfile()
		{
			this.ClearOutputChannel(ColorAdjustType.Default);
		}

		/// <summary>Clears the output channel color profile setting for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the output channel profile setting is cleared.</param>
		public void ClearOutputChannelColorProfile(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesOutputChannel(new HandleRef(this, this.nativeImageAttributes), type, false, ColorChannelFlag.ColorChannelLast);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the color-remap table for the default category.</summary>
		/// <param name="map">An array of color pairs of type <see cref="T:System.Drawing.Imaging.ColorMap" />. Each color pair contains an existing color (the first value) and the color that it will be mapped to (the second value).</param>
		public void SetRemapTable(ColorMap[] map)
		{
			this.SetRemapTable(map, ColorAdjustType.Default);
		}

		/// <summary>Sets the color-remap table for a specified category.</summary>
		/// <param name="map">An array of color pairs of type <see cref="T:System.Drawing.Imaging.ColorMap" />. Each color pair contains an existing color (the first value) and the color that it will be mapped to (the second value).</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the color-remap table is set.</param>
		public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
		{
			int num = map.Length;
			int num2 = 4;
			IntPtr intPtr = Marshal.AllocHGlobal(checked(num * num2 * 2));
			try
			{
				for (int i = 0; i < num; i++)
				{
					Marshal.StructureToPtr<int>(map[i].OldColor.ToArgb(), (IntPtr)((long)intPtr + (long)(i * num2 * 2)), false);
					Marshal.StructureToPtr<int>(map[i].NewColor.ToArgb(), (IntPtr)((long)intPtr + (long)(i * num2 * 2) + (long)num2), false);
				}
				int num3 = GDIPlus.GdipSetImageAttributesRemapTable(new HandleRef(this, this.nativeImageAttributes), type, true, num, new HandleRef(null, intPtr));
				if (num3 != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num3);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		/// <summary>Clears the color-remap table for the default category.</summary>
		public void ClearRemapTable()
		{
			this.ClearRemapTable(ColorAdjustType.Default);
		}

		/// <summary>Clears the color-remap table for a specified category.</summary>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category for which the remap table is cleared.</param>
		public void ClearRemapTable(ColorAdjustType type)
		{
			int num = GDIPlus.GdipSetImageAttributesRemapTable(new HandleRef(this, this.nativeImageAttributes), type, false, 0, NativeMethods.NullHandleRef);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Sets the color-remap table for the brush category.</summary>
		/// <param name="map">An array of <see cref="T:System.Drawing.Imaging.ColorMap" /> objects.</param>
		public void SetBrushRemapTable(ColorMap[] map)
		{
			this.SetRemapTable(map, ColorAdjustType.Brush);
		}

		/// <summary>Clears the brush color-remap table of this <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object.</summary>
		public void ClearBrushRemapTable()
		{
			this.ClearRemapTable(ColorAdjustType.Brush);
		}

		/// <summary>Sets the wrap mode that is used to decide how to tile a texture across a shape, or at shape boundaries. A texture is tiled across a shape to fill it in when the texture is smaller than the shape it is filling.</summary>
		/// <param name="mode">An element of <see cref="T:System.Drawing.Drawing2D.WrapMode" /> that specifies how repeated copies of an image are used to tile an area.</param>
		public void SetWrapMode(WrapMode mode)
		{
			this.SetWrapMode(mode, default(Color), false);
		}

		/// <summary>Sets the wrap mode and color used to decide how to tile a texture across a shape, or at shape boundaries. A texture is tiled across a shape to fill it in when the texture is smaller than the shape it is filling.</summary>
		/// <param name="mode">An element of <see cref="T:System.Drawing.Drawing2D.WrapMode" /> that specifies how repeated copies of an image are used to tile an area.</param>
		/// <param name="color">An <see cref="T:System.Drawing.Imaging.ImageAttributes" /> object that specifies the color of pixels outside of a rendered image. This color is visible if the mode parameter is set to <see cref="F:System.Drawing.Drawing2D.WrapMode.Clamp" /> and the source rectangle passed to <see cref="Overload:System.Drawing.Graphics.DrawImage" /> is larger than the image itself.</param>
		public void SetWrapMode(WrapMode mode, Color color)
		{
			this.SetWrapMode(mode, color, false);
		}

		/// <summary>Sets the wrap mode and color used to decide how to tile a texture across a shape, or at shape boundaries. A texture is tiled across a shape to fill it in when the texture is smaller than the shape it is filling.</summary>
		/// <param name="mode">An element of <see cref="T:System.Drawing.Drawing2D.WrapMode" /> that specifies how repeated copies of an image are used to tile an area.</param>
		/// <param name="color">A color object that specifies the color of pixels outside of a rendered image. This color is visible if the mode parameter is set to <see cref="F:System.Drawing.Drawing2D.WrapMode.Clamp" /> and the source rectangle passed to <see cref="Overload:System.Drawing.Graphics.DrawImage" /> is larger than the image itself.</param>
		/// <param name="clamp">This parameter has no effect. Set it to <see langword="false" />.</param>
		public void SetWrapMode(WrapMode mode, Color color, bool clamp)
		{
			int num = GDIPlus.GdipSetImageAttributesWrapMode(new HandleRef(this, this.nativeImageAttributes), (int)mode, color.ToArgb(), clamp);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		/// <summary>Adjusts the colors in a palette according to the adjustment settings of a specified category.</summary>
		/// <param name="palette">A <see cref="T:System.Drawing.Imaging.ColorPalette" /> that on input contains the palette to be adjusted, and on output contains the adjusted palette.</param>
		/// <param name="type">An element of <see cref="T:System.Drawing.Imaging.ColorAdjustType" /> that specifies the category whose adjustment settings will be applied to the palette.</param>
		public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
		{
			IntPtr intPtr = palette.ConvertToMemory();
			try
			{
				int num = GDIPlus.GdipGetImageAttributesAdjustedPalette(new HandleRef(this, this.nativeImageAttributes), new HandleRef(null, intPtr), type);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				palette.ConvertFromMemory(intPtr);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		internal IntPtr nativeImageAttributes;
	}
}
