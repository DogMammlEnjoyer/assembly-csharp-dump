using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	/// <summary>Defines a group of type faces having a similar basic design and certain variations in styles. This class cannot be inherited.</summary>
	public sealed class FontFamily : MarshalByRefObject, IDisposable
	{
		internal FontFamily(IntPtr fntfamily)
		{
			this.nativeFontFamily = fntfamily;
		}

		internal unsafe void refreshName()
		{
			if (this.nativeFontFamily == IntPtr.Zero)
			{
				return;
			}
			char* value = stackalloc char[(UIntPtr)64];
			GDIPlus.CheckStatus(GDIPlus.GdipGetFamilyName(this.nativeFontFamily, (IntPtr)((void*)value), 0));
			this.name = Marshal.PtrToStringUni((IntPtr)((void*)value));
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~FontFamily()
		{
			this.Dispose();
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeFontFamily;
			}
		}

		internal IntPtr NativeFamily
		{
			get
			{
				return this.nativeFontFamily;
			}
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.FontFamily" /> from the specified generic font family.</summary>
		/// <param name="genericFamily">The <see cref="T:System.Drawing.Text.GenericFontFamilies" /> from which to create the new <see cref="T:System.Drawing.FontFamily" />.</param>
		public FontFamily(GenericFontFamilies genericFamily)
		{
			Status status;
			switch (genericFamily)
			{
			case GenericFontFamilies.Serif:
				status = GDIPlus.GdipGetGenericFontFamilySerif(out this.nativeFontFamily);
				goto IL_4D;
			case GenericFontFamilies.SansSerif:
				status = GDIPlus.GdipGetGenericFontFamilySansSerif(out this.nativeFontFamily);
				goto IL_4D;
			}
			status = GDIPlus.GdipGetGenericFontFamilyMonospace(out this.nativeFontFamily);
			IL_4D:
			GDIPlus.CheckStatus(status);
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.FontFamily" /> with the specified name.</summary>
		/// <param name="name">The name of the new <see cref="T:System.Drawing.FontFamily" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string ("").  
		/// -or-  
		/// <paramref name="name" /> specifies a font that is not installed on the computer running the application.  
		/// -or-  
		/// <paramref name="name" /> specifies a font that is not a TrueType font.</exception>
		public FontFamily(string name) : this(name, null)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.FontFamily" /> in the specified <see cref="T:System.Drawing.Text.FontCollection" /> with the specified name.</summary>
		/// <param name="name">A <see cref="T:System.String" /> that represents the name of the new <see cref="T:System.Drawing.FontFamily" />.</param>
		/// <param name="fontCollection">The <see cref="T:System.Drawing.Text.FontCollection" /> that contains this <see cref="T:System.Drawing.FontFamily" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string ("").  
		/// -or-  
		/// <paramref name="name" /> specifies a font that is not installed on the computer running the application.  
		/// -or-  
		/// <paramref name="name" /> specifies a font that is not a TrueType font.</exception>
		public FontFamily(string name, FontCollection fontCollection)
		{
			IntPtr collection = (fontCollection == null) ? IntPtr.Zero : fontCollection._nativeFontCollection;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFamilyFromName(name, collection, out this.nativeFontFamily));
		}

		/// <summary>Gets the name of this <see cref="T:System.Drawing.FontFamily" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that represents the name of this <see cref="T:System.Drawing.FontFamily" />.</returns>
		public string Name
		{
			get
			{
				if (this.nativeFontFamily == IntPtr.Zero)
				{
					throw new ArgumentException("Name", Locale.GetText("Object was disposed."));
				}
				if (this.name == null)
				{
					this.refreshName();
				}
				return this.name;
			}
		}

		/// <summary>Gets a generic monospace <see cref="T:System.Drawing.FontFamily" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.FontFamily" /> that represents a generic monospace font.</returns>
		public static FontFamily GenericMonospace
		{
			get
			{
				return new FontFamily(GenericFontFamilies.Monospace);
			}
		}

		/// <summary>Gets a generic sans serif <see cref="T:System.Drawing.FontFamily" /> object.</summary>
		/// <returns>A <see cref="T:System.Drawing.FontFamily" /> object that represents a generic sans serif font.</returns>
		public static FontFamily GenericSansSerif
		{
			get
			{
				return new FontFamily(GenericFontFamilies.SansSerif);
			}
		}

		/// <summary>Gets a generic serif <see cref="T:System.Drawing.FontFamily" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.FontFamily" /> that represents a generic serif font.</returns>
		public static FontFamily GenericSerif
		{
			get
			{
				return new FontFamily(GenericFontFamilies.Serif);
			}
		}

		/// <summary>Returns the cell ascent, in design units, of the <see cref="T:System.Drawing.FontFamily" /> of the specified style.</summary>
		/// <param name="style">A <see cref="T:System.Drawing.FontStyle" /> that contains style information for the font.</param>
		/// <returns>The cell ascent for this <see cref="T:System.Drawing.FontFamily" /> that uses the specified <see cref="T:System.Drawing.FontStyle" />.</returns>
		public int GetCellAscent(FontStyle style)
		{
			short result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetCellAscent(this.nativeFontFamily, (int)style, out result));
			return (int)result;
		}

		/// <summary>Returns the cell descent, in design units, of the <see cref="T:System.Drawing.FontFamily" /> of the specified style.</summary>
		/// <param name="style">A <see cref="T:System.Drawing.FontStyle" /> that contains style information for the font.</param>
		/// <returns>The cell descent metric for this <see cref="T:System.Drawing.FontFamily" /> that uses the specified <see cref="T:System.Drawing.FontStyle" />.</returns>
		public int GetCellDescent(FontStyle style)
		{
			short result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetCellDescent(this.nativeFontFamily, (int)style, out result));
			return (int)result;
		}

		/// <summary>Gets the height, in font design units, of the em square for the specified style.</summary>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> for which to get the em height.</param>
		/// <returns>The height of the em square.</returns>
		public int GetEmHeight(FontStyle style)
		{
			short result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetEmHeight(this.nativeFontFamily, (int)style, out result));
			return (int)result;
		}

		/// <summary>Returns the line spacing, in design units, of the <see cref="T:System.Drawing.FontFamily" /> of the specified style. The line spacing is the vertical distance between the base lines of two consecutive lines of text.</summary>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> to apply.</param>
		/// <returns>The distance between two consecutive lines of text.</returns>
		public int GetLineSpacing(FontStyle style)
		{
			short result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetLineSpacing(this.nativeFontFamily, (int)style, out result));
			return (int)result;
		}

		/// <summary>Indicates whether the specified <see cref="T:System.Drawing.FontStyle" /> enumeration is available.</summary>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Drawing.FontStyle" /> is available; otherwise, <see langword="false" />.</returns>
		[MonoDocumentationNote("When used with libgdiplus this method always return true (styles are created on demand).")]
		public bool IsStyleAvailable(FontStyle style)
		{
			bool result;
			GDIPlus.CheckStatus(GDIPlus.GdipIsStyleAvailable(this.nativeFontFamily, (int)style, out result));
			return result;
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.FontFamily" />.</summary>
		public void Dispose()
		{
			if (this.nativeFontFamily != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteFontFamily(this.nativeFontFamily);
				this.nativeFontFamily = IntPtr.Zero;
				GC.SuppressFinalize(this);
				GDIPlus.CheckStatus(status);
			}
		}

		/// <summary>Indicates whether the specified object is a <see cref="T:System.Drawing.FontFamily" /> and is identical to this <see cref="T:System.Drawing.FontFamily" />.</summary>
		/// <param name="obj">The object to test.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Drawing.FontFamily" /> and is identical to this <see cref="T:System.Drawing.FontFamily" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			FontFamily fontFamily = obj as FontFamily;
			return fontFamily != null && this.Name == fontFamily.Name;
		}

		/// <summary>Gets a hash code for this <see cref="T:System.Drawing.FontFamily" />.</summary>
		/// <returns>The hash code for this <see cref="T:System.Drawing.FontFamily" />.</returns>
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		/// <summary>Returns an array that contains all the <see cref="T:System.Drawing.FontFamily" /> objects associated with the current graphics context.</summary>
		/// <returns>An array of <see cref="T:System.Drawing.FontFamily" /> objects associated with the current graphics context.</returns>
		public static FontFamily[] Families
		{
			get
			{
				return new InstalledFontCollection().Families;
			}
		}

		/// <summary>Returns an array that contains all the <see cref="T:System.Drawing.FontFamily" /> objects available for the specified graphics context.</summary>
		/// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> object from which to return <see cref="T:System.Drawing.FontFamily" /> objects.</param>
		/// <returns>An array of <see cref="T:System.Drawing.FontFamily" /> objects available for the specified <see cref="T:System.Drawing.Graphics" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="graphics" /> is <see langword="null" />.</exception>
		public static FontFamily[] GetFamilies(Graphics graphics)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			return new InstalledFontCollection().Families;
		}

		/// <summary>Returns the name, in the specified language, of this <see cref="T:System.Drawing.FontFamily" />.</summary>
		/// <param name="language">The language in which the name is returned.</param>
		/// <returns>A <see cref="T:System.String" /> that represents the name, in the specified language, of this <see cref="T:System.Drawing.FontFamily" />.</returns>
		[MonoLimitation("The language parameter is ignored. We always return the name using the default system language.")]
		public string GetName(int language)
		{
			return this.Name;
		}

		/// <summary>Converts this <see cref="T:System.Drawing.FontFamily" /> to a human-readable string representation.</summary>
		/// <returns>The string that represents this <see cref="T:System.Drawing.FontFamily" />.</returns>
		public override string ToString()
		{
			return "[FontFamily: Name=" + this.Name + "]";
		}

		private string name;

		private IntPtr nativeFontFamily = IntPtr.Zero;
	}
}
