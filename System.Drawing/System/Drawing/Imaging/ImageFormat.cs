using System;
using System.ComponentModel;

namespace System.Drawing.Imaging
{
	/// <summary>Specifies the file format of the image. Not inheritable.</summary>
	[TypeConverter(typeof(ImageFormatConverter))]
	public sealed class ImageFormat
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Imaging.ImageFormat" /> class by using the specified <see cref="T:System.Guid" /> structure.</summary>
		/// <param name="guid">The <see cref="T:System.Guid" /> structure that specifies a particular image format.</param>
		public ImageFormat(Guid guid)
		{
			this.guid = guid;
		}

		private ImageFormat(string name, string guid)
		{
			this.name = name;
			this.guid = new Guid(guid);
		}

		/// <summary>Returns a value that indicates whether the specified object is an <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that is equivalent to this <see cref="T:System.Drawing.Imaging.ImageFormat" /> object.</summary>
		/// <param name="o">The object to test.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="o" /> is an <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that is equivalent to this <see cref="T:System.Drawing.Imaging.ImageFormat" /> object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			ImageFormat imageFormat = o as ImageFormat;
			return imageFormat != null && imageFormat.Guid.Equals(this.guid);
		}

		/// <summary>Returns a hash code value that represents this object.</summary>
		/// <returns>A hash code that represents this object.</returns>
		public override int GetHashCode()
		{
			return this.guid.GetHashCode();
		}

		/// <summary>Converts this <see cref="T:System.Drawing.Imaging.ImageFormat" /> object to a human-readable string.</summary>
		/// <returns>A string that represents this <see cref="T:System.Drawing.Imaging.ImageFormat" /> object.</returns>
		public override string ToString()
		{
			if (this.name != null)
			{
				return this.name;
			}
			return "[ImageFormat: " + this.guid.ToString() + "]";
		}

		/// <summary>Gets a <see cref="T:System.Guid" /> structure that represents this <see cref="T:System.Drawing.Imaging.ImageFormat" /> object.</summary>
		/// <returns>A <see cref="T:System.Guid" /> structure that represents this <see cref="T:System.Drawing.Imaging.ImageFormat" /> object.</returns>
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		/// <summary>Gets the bitmap (BMP) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the bitmap image format.</returns>
		public static ImageFormat Bmp
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat bmpImageFormat;
				lock (obj)
				{
					if (ImageFormat.BmpImageFormat == null)
					{
						ImageFormat.BmpImageFormat = new ImageFormat("Bmp", "b96b3cab-0728-11d3-9d7b-0000f81ef32e");
					}
					bmpImageFormat = ImageFormat.BmpImageFormat;
				}
				return bmpImageFormat;
			}
		}

		/// <summary>Gets the enhanced metafile (EMF) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the enhanced metafile image format.</returns>
		public static ImageFormat Emf
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat emfImageFormat;
				lock (obj)
				{
					if (ImageFormat.EmfImageFormat == null)
					{
						ImageFormat.EmfImageFormat = new ImageFormat("Emf", "b96b3cac-0728-11d3-9d7b-0000f81ef32e");
					}
					emfImageFormat = ImageFormat.EmfImageFormat;
				}
				return emfImageFormat;
			}
		}

		/// <summary>Gets the Exchangeable Image File (Exif) format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the Exif format.</returns>
		public static ImageFormat Exif
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat exifImageFormat;
				lock (obj)
				{
					if (ImageFormat.ExifImageFormat == null)
					{
						ImageFormat.ExifImageFormat = new ImageFormat("Exif", "b96b3cb2-0728-11d3-9d7b-0000f81ef32e");
					}
					exifImageFormat = ImageFormat.ExifImageFormat;
				}
				return exifImageFormat;
			}
		}

		/// <summary>Gets the Graphics Interchange Format (GIF) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the GIF image format.</returns>
		public static ImageFormat Gif
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat gifImageFormat;
				lock (obj)
				{
					if (ImageFormat.GifImageFormat == null)
					{
						ImageFormat.GifImageFormat = new ImageFormat("Gif", "b96b3cb0-0728-11d3-9d7b-0000f81ef32e");
					}
					gifImageFormat = ImageFormat.GifImageFormat;
				}
				return gifImageFormat;
			}
		}

		/// <summary>Gets the Windows icon image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the Windows icon image format.</returns>
		public static ImageFormat Icon
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat iconImageFormat;
				lock (obj)
				{
					if (ImageFormat.IconImageFormat == null)
					{
						ImageFormat.IconImageFormat = new ImageFormat("Icon", "b96b3cb5-0728-11d3-9d7b-0000f81ef32e");
					}
					iconImageFormat = ImageFormat.IconImageFormat;
				}
				return iconImageFormat;
			}
		}

		/// <summary>Gets the Joint Photographic Experts Group (JPEG) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the JPEG image format.</returns>
		public static ImageFormat Jpeg
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat jpegImageFormat;
				lock (obj)
				{
					if (ImageFormat.JpegImageFormat == null)
					{
						ImageFormat.JpegImageFormat = new ImageFormat("Jpeg", "b96b3cae-0728-11d3-9d7b-0000f81ef32e");
					}
					jpegImageFormat = ImageFormat.JpegImageFormat;
				}
				return jpegImageFormat;
			}
		}

		/// <summary>Gets the format of a bitmap in memory.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the format of a bitmap in memory.</returns>
		public static ImageFormat MemoryBmp
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat memoryBmpImageFormat;
				lock (obj)
				{
					if (ImageFormat.MemoryBmpImageFormat == null)
					{
						ImageFormat.MemoryBmpImageFormat = new ImageFormat("MemoryBMP", "b96b3caa-0728-11d3-9d7b-0000f81ef32e");
					}
					memoryBmpImageFormat = ImageFormat.MemoryBmpImageFormat;
				}
				return memoryBmpImageFormat;
			}
		}

		/// <summary>Gets the W3C Portable Network Graphics (PNG) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the PNG image format.</returns>
		public static ImageFormat Png
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat pngImageFormat;
				lock (obj)
				{
					if (ImageFormat.PngImageFormat == null)
					{
						ImageFormat.PngImageFormat = new ImageFormat("Png", "b96b3caf-0728-11d3-9d7b-0000f81ef32e");
					}
					pngImageFormat = ImageFormat.PngImageFormat;
				}
				return pngImageFormat;
			}
		}

		/// <summary>Gets the Tagged Image File Format (TIFF) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the TIFF image format.</returns>
		public static ImageFormat Tiff
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat tiffImageFormat;
				lock (obj)
				{
					if (ImageFormat.TiffImageFormat == null)
					{
						ImageFormat.TiffImageFormat = new ImageFormat("Tiff", "b96b3cb1-0728-11d3-9d7b-0000f81ef32e");
					}
					tiffImageFormat = ImageFormat.TiffImageFormat;
				}
				return tiffImageFormat;
			}
		}

		/// <summary>Gets the Windows metafile (WMF) image format.</summary>
		/// <returns>An <see cref="T:System.Drawing.Imaging.ImageFormat" /> object that indicates the Windows metafile image format.</returns>
		public static ImageFormat Wmf
		{
			get
			{
				object obj = ImageFormat.locker;
				ImageFormat wmfImageFormat;
				lock (obj)
				{
					if (ImageFormat.WmfImageFormat == null)
					{
						ImageFormat.WmfImageFormat = new ImageFormat("Wmf", "b96b3cad-0728-11d3-9d7b-0000f81ef32e");
					}
					wmfImageFormat = ImageFormat.WmfImageFormat;
				}
				return wmfImageFormat;
			}
		}

		private Guid guid;

		private string name;

		private const string BmpGuid = "b96b3cab-0728-11d3-9d7b-0000f81ef32e";

		private const string EmfGuid = "b96b3cac-0728-11d3-9d7b-0000f81ef32e";

		private const string ExifGuid = "b96b3cb2-0728-11d3-9d7b-0000f81ef32e";

		private const string GifGuid = "b96b3cb0-0728-11d3-9d7b-0000f81ef32e";

		private const string TiffGuid = "b96b3cb1-0728-11d3-9d7b-0000f81ef32e";

		private const string PngGuid = "b96b3caf-0728-11d3-9d7b-0000f81ef32e";

		private const string MemoryBmpGuid = "b96b3caa-0728-11d3-9d7b-0000f81ef32e";

		private const string IconGuid = "b96b3cb5-0728-11d3-9d7b-0000f81ef32e";

		private const string JpegGuid = "b96b3cae-0728-11d3-9d7b-0000f81ef32e";

		private const string WmfGuid = "b96b3cad-0728-11d3-9d7b-0000f81ef32e";

		private static object locker = new object();

		private static ImageFormat BmpImageFormat;

		private static ImageFormat EmfImageFormat;

		private static ImageFormat ExifImageFormat;

		private static ImageFormat GifImageFormat;

		private static ImageFormat TiffImageFormat;

		private static ImageFormat PngImageFormat;

		private static ImageFormat MemoryBmpImageFormat;

		private static ImageFormat IconImageFormat;

		private static ImageFormat JpegImageFormat;

		private static ImageFormat WmfImageFormat;
	}
}
