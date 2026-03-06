using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.Drawing.Imaging
{
	/// <summary>Contains attributes of an associated <see cref="T:System.Drawing.Imaging.Metafile" />. Not inheritable.</summary>
	[MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class MetafileHeader
	{
		internal MetafileHeader(IntPtr henhmetafile)
		{
			Marshal.PtrToStructure<MetafileHeader>(henhmetafile, this);
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is device dependent.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is device dependent; otherwise, <see langword="false" />.</returns>
		[MonoTODO("always returns false")]
		public bool IsDisplay()
		{
			return false;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format; otherwise, <see langword="false" />.</returns>
		public bool IsEmf()
		{
			return this.Type == MetafileType.Emf;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format or the Windows enhanced metafile plus format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format or the Windows enhanced metafile plus format; otherwise, <see langword="false" />.</returns>
		public bool IsEmfOrEmfPlus()
		{
			return this.Type >= MetafileType.Emf;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile plus format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile plus format; otherwise, <see langword="false" />.</returns>
		public bool IsEmfPlus()
		{
			return this.Type >= MetafileType.EmfPlusOnly;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Dual enhanced metafile format. This format supports both the enhanced and the enhanced plus format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Dual enhanced metafile format; otherwise, <see langword="false" />.</returns>
		public bool IsEmfPlusDual()
		{
			return this.Type == MetafileType.EmfPlusDual;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> supports only the Windows enhanced metafile plus format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> supports only the Windows enhanced metafile plus format; otherwise, <see langword="false" />.</returns>
		public bool IsEmfPlusOnly()
		{
			return this.Type == MetafileType.EmfPlusOnly;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows metafile format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows metafile format; otherwise, <see langword="false" />.</returns>
		public bool IsWmf()
		{
			return this.Type <= MetafileType.WmfPlaceable;
		}

		/// <summary>Returns a value that indicates whether the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows placeable metafile format.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows placeable metafile format; otherwise, <see langword="false" />.</returns>
		public bool IsWmfPlaceable()
		{
			return this.Type == MetafileType.WmfPlaceable;
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Rectangle" /> that bounds the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.Rectangle" /> that bounds the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.header.x, this.header.y, this.header.width, this.header.height);
			}
		}

		/// <summary>Gets the horizontal resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>The horizontal resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public float DpiX
		{
			get
			{
				return this.header.dpi_x;
			}
		}

		/// <summary>Gets the vertical resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>The vertical resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public float DpiY
		{
			get
			{
				return this.header.dpi_y;
			}
		}

		/// <summary>Gets the size, in bytes, of the enhanced metafile plus header file.</summary>
		/// <returns>The size, in bytes, of the enhanced metafile plus header file.</returns>
		public int EmfPlusHeaderSize
		{
			get
			{
				return this.header.emfplus_header_size;
			}
		}

		/// <summary>Gets the logical horizontal resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>The logical horizontal resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public int LogicalDpiX
		{
			get
			{
				return this.header.logical_dpi_x;
			}
		}

		/// <summary>Gets the logical vertical resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>The logical vertical resolution, in dots per inch, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public int LogicalDpiY
		{
			get
			{
				return this.header.logical_dpi_y;
			}
		}

		/// <summary>Gets the size, in bytes, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>The size, in bytes, of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public int MetafileSize
		{
			get
			{
				return this.header.size;
			}
		}

		/// <summary>Gets the type of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.Imaging.MetafileType" /> enumeration that represents the type of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public MetafileType Type
		{
			get
			{
				return this.header.type;
			}
		}

		/// <summary>Gets the version number of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>The version number of the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public int Version
		{
			get
			{
				return this.header.version;
			}
		}

		/// <summary>Gets the Windows metafile (WMF) header file for the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.Imaging.MetaHeader" /> that contains the WMF header file for the associated <see cref="T:System.Drawing.Imaging.Metafile" />.</returns>
		public MetaHeader WmfHeader
		{
			get
			{
				if (this.IsWmf())
				{
					return new MetaHeader(this.header.wmf_header);
				}
				throw new ArgumentException("WmfHeader only available on WMF files.");
			}
		}

		internal MetafileHeader()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private MonoMetafileHeader header;
	}
}
