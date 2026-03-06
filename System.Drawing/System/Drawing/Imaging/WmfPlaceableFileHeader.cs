using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	/// <summary>Defines a placeable metafile. Not inheritable.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WmfPlaceableFileHeader
	{
		/// <summary>Gets or sets a value indicating the presence of a placeable metafile header.</summary>
		/// <returns>A value indicating presence of a placeable metafile header.</returns>
		public int Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
			}
		}

		/// <summary>Gets or sets the handle of the metafile in memory.</summary>
		/// <returns>The handle of the metafile in memory.</returns>
		public short Hmf
		{
			get
			{
				return this._hmf;
			}
			set
			{
				this._hmf = value;
			}
		}

		/// <summary>Gets or sets the x-coordinate of the upper-left corner of the bounding rectangle of the metafile image on the output device.</summary>
		/// <returns>The x-coordinate of the upper-left corner of the bounding rectangle of the metafile image on the output device.</returns>
		public short BboxLeft
		{
			get
			{
				return this._bboxLeft;
			}
			set
			{
				this._bboxLeft = value;
			}
		}

		/// <summary>Gets or sets the y-coordinate of the upper-left corner of the bounding rectangle of the metafile image on the output device.</summary>
		/// <returns>The y-coordinate of the upper-left corner of the bounding rectangle of the metafile image on the output device.</returns>
		public short BboxTop
		{
			get
			{
				return this._bboxTop;
			}
			set
			{
				this._bboxTop = value;
			}
		}

		/// <summary>Gets or sets the x-coordinate of the lower-right corner of the bounding rectangle of the metafile image on the output device.</summary>
		/// <returns>The x-coordinate of the lower-right corner of the bounding rectangle of the metafile image on the output device.</returns>
		public short BboxRight
		{
			get
			{
				return this._bboxRight;
			}
			set
			{
				this._bboxRight = value;
			}
		}

		/// <summary>Gets or sets the y-coordinate of the lower-right corner of the bounding rectangle of the metafile image on the output device.</summary>
		/// <returns>The y-coordinate of the lower-right corner of the bounding rectangle of the metafile image on the output device.</returns>
		public short BboxBottom
		{
			get
			{
				return this._bboxBottom;
			}
			set
			{
				this._bboxBottom = value;
			}
		}

		/// <summary>Gets or sets the number of twips per inch.</summary>
		/// <returns>The number of twips per inch.</returns>
		public short Inch
		{
			get
			{
				return this._inch;
			}
			set
			{
				this._inch = value;
			}
		}

		/// <summary>Reserved. Do not use.</summary>
		/// <returns>Reserved. Do not use.</returns>
		public int Reserved
		{
			get
			{
				return this._reserved;
			}
			set
			{
				this._reserved = value;
			}
		}

		/// <summary>Gets or sets the checksum value for the previous ten <see langword="WORD" /> s in the header.</summary>
		/// <returns>The checksum value for the previous ten <see langword="WORD" /> s in the header.</returns>
		public short Checksum
		{
			get
			{
				return this._checksum;
			}
			set
			{
				this._checksum = value;
			}
		}

		private int _key = -1698247209;

		private short _hmf;

		private short _bboxLeft;

		private short _bboxTop;

		private short _bboxRight;

		private short _bboxBottom;

		private short _inch;

		private int _reserved;

		private short _checksum;
	}
}
