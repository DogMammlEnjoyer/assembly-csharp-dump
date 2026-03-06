using System;
using System.IO;

namespace System.Drawing.Text
{
	/// <summary>Provides a collection of font families built from font files that are provided by the client application.</summary>
	public sealed class PrivateFontCollection : FontCollection
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Text.PrivateFontCollection" /> class.</summary>
		public PrivateFontCollection()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipNewPrivateFontCollection(out this._nativeFontCollection));
		}

		/// <summary>Adds a font from the specified file to this <see cref="T:System.Drawing.Text.PrivateFontCollection" />.</summary>
		/// <param name="filename">A <see cref="T:System.String" /> that contains the file name of the font to add.</param>
		/// <exception cref="T:System.IO.FileNotFoundException">The specified font is not supported or the font file cannot be found.</exception>
		public void AddFontFile(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			string fullPath = Path.GetFullPath(filename);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException();
			}
			GDIPlus.CheckStatus(GDIPlus.GdipPrivateAddFontFile(this._nativeFontCollection, fullPath));
		}

		/// <summary>Adds a font contained in system memory to this <see cref="T:System.Drawing.Text.PrivateFontCollection" />.</summary>
		/// <param name="memory">The memory address of the font to add.</param>
		/// <param name="length">The memory length of the font to add.</param>
		public void AddMemoryFont(IntPtr memory, int length)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipPrivateAddMemoryFont(this._nativeFontCollection, memory, length));
		}

		protected override void Dispose(bool disposing)
		{
			if (this._nativeFontCollection != IntPtr.Zero)
			{
				GDIPlus.GdipDeletePrivateFontCollection(ref this._nativeFontCollection);
				this._nativeFontCollection = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
	}
}
