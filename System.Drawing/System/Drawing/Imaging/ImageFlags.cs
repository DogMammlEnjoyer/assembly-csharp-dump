using System;

namespace System.Drawing.Imaging
{
	/// <summary>Specifies the attributes of the pixel data contained in an <see cref="T:System.Drawing.Image" /> object. The <see cref="P:System.Drawing.Image.Flags" /> property returns a member of this enumeration.</summary>
	[Flags]
	public enum ImageFlags
	{
		/// <summary>There is no format information.</summary>
		None = 0,
		/// <summary>The pixel data is scalable.</summary>
		Scalable = 1,
		/// <summary>The pixel data contains alpha information.</summary>
		HasAlpha = 2,
		/// <summary>Specifies that the pixel data has alpha values other than 0 (transparent) and 255 (opaque).</summary>
		HasTranslucent = 4,
		/// <summary>The pixel data is partially scalable, but there are some limitations.</summary>
		PartiallyScalable = 8,
		/// <summary>The pixel data uses an RGB color space.</summary>
		ColorSpaceRgb = 16,
		/// <summary>The pixel data uses a CMYK color space.</summary>
		ColorSpaceCmyk = 32,
		/// <summary>The pixel data is grayscale.</summary>
		ColorSpaceGray = 64,
		/// <summary>Specifies that the image is stored using a YCBCR color space.</summary>
		ColorSpaceYcbcr = 128,
		/// <summary>Specifies that the image is stored using a YCCK color space.</summary>
		ColorSpaceYcck = 256,
		/// <summary>Specifies that dots per inch information is stored in the image.</summary>
		HasRealDpi = 4096,
		/// <summary>Specifies that the pixel size is stored in the image.</summary>
		HasRealPixelSize = 8192,
		/// <summary>The pixel data is read-only.</summary>
		ReadOnly = 65536,
		/// <summary>The pixel data can be cached for faster access.</summary>
		Caching = 131072
	}
}
