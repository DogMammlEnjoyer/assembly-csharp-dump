using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{
	/// <summary>Defines a particular format for text, including font face, size, and style attributes. This class cannot be inherited.</summary>
	[TypeConverter(typeof(FontConverter))]
	[Editor("System.Drawing.Design.FontEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[ComVisible(true)]
	[Serializable]
	public sealed class Font : MarshalByRefObject, ISerializable, ICloneable, IDisposable
	{
		private void CreateFont(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte charSet, bool isVertical)
		{
			this.originalFontName = familyName;
			FontFamily fontFamily;
			try
			{
				fontFamily = new FontFamily(familyName);
			}
			catch (Exception)
			{
				fontFamily = FontFamily.GenericSansSerif;
			}
			this.setProperties(fontFamily, emSize, style, unit, charSet, isVertical);
			Status status = GDIPlus.GdipCreateFont(fontFamily.NativeFamily, emSize, style, unit, out this.fontObject);
			if (status == Status.FontStyleNotFound)
			{
				throw new ArgumentException(Locale.GetText("Style {0} isn't supported by font {1}.", new object[]
				{
					style.ToString(),
					familyName
				}));
			}
			GDIPlus.CheckStatus(status);
		}

		private Font(SerializationInfo info, StreamingContext context)
		{
			string familyName = (string)info.GetValue("Name", typeof(string));
			float emSize = (float)info.GetValue("Size", typeof(float));
			FontStyle style = (FontStyle)info.GetValue("Style", typeof(FontStyle));
			GraphicsUnit unit = (GraphicsUnit)info.GetValue("Unit", typeof(GraphicsUnit));
			this.CreateFont(familyName, emSize, style, unit, 1, false);
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
		/// <param name="si">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("Name", this.Name);
			si.AddValue("Size", this.Size);
			si.AddValue("Style", this.Style);
			si.AddValue("Unit", this.Unit);
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		~Font()
		{
			this.Dispose();
		}

		/// <summary>Releases all resources used by this <see cref="T:System.Drawing.Font" />.</summary>
		public void Dispose()
		{
			if (this.fontObject != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteFont(this.fontObject);
				this.fontObject = IntPtr.Zero;
				GC.SuppressFinalize(this);
				GDIPlus.CheckStatus(status);
			}
		}

		internal void SetSystemFontName(string newSystemFontName)
		{
			this.systemFontName = newSystemFontName;
		}

		internal void unitConversion(GraphicsUnit fromUnit, GraphicsUnit toUnit, float nSrc, out float nTrg)
		{
			nTrg = 0f;
			float num;
			switch (fromUnit)
			{
			case GraphicsUnit.World:
			case GraphicsUnit.Pixel:
				num = nSrc / Graphics.systemDpiX;
				break;
			case GraphicsUnit.Display:
				num = nSrc / 75f;
				break;
			case GraphicsUnit.Point:
				num = nSrc / 72f;
				break;
			case GraphicsUnit.Inch:
				num = nSrc;
				break;
			case GraphicsUnit.Document:
				num = nSrc / 300f;
				break;
			case GraphicsUnit.Millimeter:
				num = nSrc / 25.4f;
				break;
			default:
				throw new ArgumentException("Invalid GraphicsUnit");
			}
			switch (toUnit)
			{
			case GraphicsUnit.World:
			case GraphicsUnit.Pixel:
				nTrg = num * Graphics.systemDpiX;
				return;
			case GraphicsUnit.Display:
				nTrg = num * 75f;
				return;
			case GraphicsUnit.Point:
				nTrg = num * 72f;
				return;
			case GraphicsUnit.Inch:
				nTrg = num;
				return;
			case GraphicsUnit.Document:
				nTrg = num * 300f;
				return;
			case GraphicsUnit.Millimeter:
				nTrg = num * 25.4f;
				return;
			default:
				throw new ArgumentException("Invalid GraphicsUnit");
			}
		}

		private void setProperties(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte charSet, bool isVertical)
		{
			this._name = family.Name;
			this._fontFamily = family;
			this._size = emSize;
			this._unit = unit;
			this._style = style;
			this._gdiCharSet = charSet;
			this._gdiVerticalFont = isVertical;
			this.unitConversion(unit, GraphicsUnit.Point, emSize, out this._sizeInPoints);
			this._bold = (this._italic = (this._strikeout = (this._underline = false)));
			if ((style & FontStyle.Bold) == FontStyle.Bold)
			{
				this._bold = true;
			}
			if ((style & FontStyle.Italic) == FontStyle.Italic)
			{
				this._italic = true;
			}
			if ((style & FontStyle.Strikeout) == FontStyle.Strikeout)
			{
				this._strikeout = true;
			}
			if ((style & FontStyle.Underline) == FontStyle.Underline)
			{
				this._underline = true;
			}
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Font" /> from the specified Windows handle.</summary>
		/// <param name="hfont">A Windows handle to a GDI font.</param>
		/// <returns>The <see cref="T:System.Drawing.Font" /> this method creates.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hfont" /> points to an object that is not a TrueType font.</exception>
		public static Font FromHfont(IntPtr hfont)
		{
			FontStyle fontStyle = FontStyle.Regular;
			LOGFONT logfont = default(LOGFONT);
			if (hfont == IntPtr.Zero)
			{
				return new Font("Arial", 10f, FontStyle.Regular);
			}
			IntPtr newFontObject;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFromHfont(hfont, out newFontObject, ref logfont));
			}
			else
			{
				IntPtr dc = GDIPlus.GetDC(IntPtr.Zero);
				try
				{
					return Font.FromLogFont(logfont, dc);
				}
				finally
				{
					GDIPlus.ReleaseDC(IntPtr.Zero, dc);
				}
			}
			if (logfont.lfItalic != 0)
			{
				fontStyle |= FontStyle.Italic;
			}
			if (logfont.lfUnderline != 0)
			{
				fontStyle |= FontStyle.Underline;
			}
			if (logfont.lfStrikeOut != 0)
			{
				fontStyle |= FontStyle.Strikeout;
			}
			if (logfont.lfWeight > 400U)
			{
				fontStyle |= FontStyle.Bold;
			}
			float size;
			if (logfont.lfHeight < 0)
			{
				size = (float)(logfont.lfHeight * -1);
			}
			else
			{
				size = (float)logfont.lfHeight;
			}
			return new Font(newFontObject, logfont.lfFaceName, fontStyle, size);
		}

		/// <summary>Returns a handle to this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>A Windows handle to this <see cref="T:System.Drawing.Font" />.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The operation was unsuccessful.</exception>
		public IntPtr ToHfont()
		{
			if (this.fontObject == IntPtr.Zero)
			{
				throw new ArgumentException(Locale.GetText("Object has been disposed."));
			}
			if (GDIPlus.RunningOnUnix())
			{
				return this.fontObject;
			}
			if (this.olf == null)
			{
				this.olf = default(LOGFONT);
				this.ToLogFont(this.olf);
			}
			LOGFONT logfont = (LOGFONT)this.olf;
			return GDIPlus.CreateFontIndirect(ref logfont);
		}

		internal Font(IntPtr newFontObject, string familyName, FontStyle style, float size)
		{
			FontFamily family;
			try
			{
				family = new FontFamily(familyName);
			}
			catch (Exception)
			{
				family = FontFamily.GenericSansSerif;
			}
			this.setProperties(family, size, style, GraphicsUnit.Pixel, 0, false);
			this.fontObject = newFontObject;
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> that uses the specified existing <see cref="T:System.Drawing.Font" /> and <see cref="T:System.Drawing.FontStyle" /> enumeration.</summary>
		/// <param name="prototype">The existing <see cref="T:System.Drawing.Font" /> from which to create the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="newStyle">The <see cref="T:System.Drawing.FontStyle" /> to apply to the new <see cref="T:System.Drawing.Font" />. Multiple values of the <see cref="T:System.Drawing.FontStyle" /> enumeration can be combined with the <see langword="OR" /> operator.</param>
		public Font(Font prototype, FontStyle newStyle)
		{
			this.setProperties(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, prototype.GdiCharSet, prototype.GdiVerticalFont);
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFont(this._fontFamily.NativeFamily, this.Size, this.Style, this.Unit, out this.fontObject));
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and unit. Sets the style to <see cref="F:System.Drawing.FontStyle.Regular" />.</summary>
		/// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="family" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		public Font(FontFamily family, float emSize, GraphicsUnit unit) : this(family, emSize, FontStyle.Regular, unit, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and unit. The style is set to <see cref="F:System.Drawing.FontStyle.Regular" />.</summary>
		/// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		public Font(string familyName, float emSize, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, FontStyle.Regular, unit, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size.</summary>
		/// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size, in points, of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		public Font(FontFamily family, float emSize) : this(family, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and style.</summary>
		/// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size, in points, of the new font.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="family" /> is <see langword="null" />.</exception>
		public Font(FontFamily family, float emSize, FontStyle style) : this(family, emSize, style, GraphicsUnit.Point, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, and unit.</summary>
		/// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="family" /> is <see langword="null" />.</exception>
		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit) : this(family, emSize, style, unit, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, unit, and character set.</summary>
		/// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a  
		///  GDI character set to use for the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="family" /> is <see langword="null" />.</exception>
		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(family, emSize, style, unit, gdiCharSet, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, unit, and character set.</summary>
		/// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a  
		///  GDI character set to use for this font.</param>
		/// <param name="gdiVerticalFont">A Boolean value indicating whether the new font is derived from a GDI vertical font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="family" /> is <see langword="null" /></exception>
		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
		{
			if (family == null)
			{
				throw new ArgumentNullException("family");
			}
			this.setProperties(family, emSize, style, unit, gdiCharSet, gdiVerticalFont);
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFont(family.NativeFamily, emSize, style, unit, out this.fontObject));
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size.</summary>
		/// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size, in points, of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity or is not a valid number.</exception>
		public Font(string familyName, float emSize) : this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and style.</summary>
		/// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size, in points, of the new font.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		public Font(string familyName, float emSize, FontStyle style) : this(familyName, emSize, style, GraphicsUnit.Point, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, and unit.</summary>
		/// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity or is not a valid number.</exception>
		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit) : this(familyName, emSize, style, unit, 1, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, unit, and character set.</summary>
		/// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a GDI character set to use for this font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(familyName, emSize, style, unit, gdiCharSet, false)
		{
		}

		/// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using the specified size, style, unit, and character set.</summary>
		/// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
		/// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
		/// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
		/// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a GDI character set to use for this font.</param>
		/// <param name="gdiVerticalFont">A Boolean value indicating whether the new <see cref="T:System.Drawing.Font" /> is derived from a GDI vertical font.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
		{
			this.CreateFont(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
		}

		internal Font(string familyName, float emSize, string systemName) : this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
		{
			this.systemFontName = systemName;
		}

		/// <summary>Creates an exact copy of this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>The <see cref="T:System.Drawing.Font" /> this method creates, cast as an <see cref="T:System.Object" />.</returns>
		public object Clone()
		{
			return new Font(this, this.Style);
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.fontObject;
			}
		}

		/// <summary>Gets a value that indicates whether this <see cref="T:System.Drawing.Font" /> is bold.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Drawing.Font" /> is bold; otherwise, <see langword="false" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Bold
		{
			get
			{
				return this._bold;
			}
		}

		/// <summary>Gets the <see cref="T:System.Drawing.FontFamily" /> associated with this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>The <see cref="T:System.Drawing.FontFamily" /> associated with this <see cref="T:System.Drawing.Font" />.</returns>
		[Browsable(false)]
		public FontFamily FontFamily
		{
			get
			{
				return this._fontFamily;
			}
		}

		/// <summary>Gets a byte value that specifies the GDI character set that this <see cref="T:System.Drawing.Font" /> uses.</summary>
		/// <returns>A byte value that specifies the GDI character set that this <see cref="T:System.Drawing.Font" /> uses. The default is 1.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public byte GdiCharSet
		{
			get
			{
				return this._gdiCharSet;
			}
		}

		/// <summary>Gets a Boolean value that indicates whether this <see cref="T:System.Drawing.Font" /> is derived from a GDI vertical font.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Drawing.Font" /> is derived from a GDI vertical font; otherwise, <see langword="false" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool GdiVerticalFont
		{
			get
			{
				return this._gdiVerticalFont;
			}
		}

		/// <summary>Gets the line spacing of this font.</summary>
		/// <returns>The line spacing, in pixels, of this font.</returns>
		[Browsable(false)]
		public int Height
		{
			get
			{
				return (int)Math.Ceiling((double)this.GetHeight());
			}
		}

		/// <summary>Gets a value indicating whether the font is a member of <see cref="T:System.Drawing.SystemFonts" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the font is a member of <see cref="T:System.Drawing.SystemFonts" />; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[Browsable(false)]
		public bool IsSystemFont
		{
			get
			{
				return !string.IsNullOrEmpty(this.systemFontName);
			}
		}

		/// <summary>Gets a value that indicates whether this font has the italic style applied.</summary>
		/// <returns>
		///   <see langword="true" /> to indicate this font has the italic style applied; otherwise, <see langword="false" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Italic
		{
			get
			{
				return this._italic;
			}
		}

		/// <summary>Gets the face name of this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>A string representation of the face name of this <see cref="T:System.Drawing.Font" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[TypeConverter(typeof(FontConverter.FontNameConverter))]
		[Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		/// <summary>Gets the em-size of this <see cref="T:System.Drawing.Font" /> measured in the units specified by the <see cref="P:System.Drawing.Font.Unit" /> property.</summary>
		/// <returns>The em-size of this <see cref="T:System.Drawing.Font" />.</returns>
		public float Size
		{
			get
			{
				return this._size;
			}
		}

		/// <summary>Gets the em-size, in points, of this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>The em-size, in points, of this <see cref="T:System.Drawing.Font" />.</returns>
		[Browsable(false)]
		public float SizeInPoints
		{
			get
			{
				return this._sizeInPoints;
			}
		}

		/// <summary>Gets a value that indicates whether this <see cref="T:System.Drawing.Font" /> specifies a horizontal line through the font.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Drawing.Font" /> has a horizontal line through it; otherwise, <see langword="false" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Strikeout
		{
			get
			{
				return this._strikeout;
			}
		}

		/// <summary>Gets style information for this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.FontStyle" /> enumeration that contains style information for this <see cref="T:System.Drawing.Font" />.</returns>
		[Browsable(false)]
		public FontStyle Style
		{
			get
			{
				return this._style;
			}
		}

		/// <summary>Gets the name of the system font if the <see cref="P:System.Drawing.Font.IsSystemFont" /> property returns <see langword="true" />.</summary>
		/// <returns>The name of the system font, if <see cref="P:System.Drawing.Font.IsSystemFont" /> returns <see langword="true" />; otherwise, an empty string ("").</returns>
		[Browsable(false)]
		public string SystemFontName
		{
			get
			{
				return this.systemFontName;
			}
		}

		/// <summary>Gets the name of the font originally specified.</summary>
		/// <returns>The string representing the name of the font originally specified.</returns>
		[Browsable(false)]
		public string OriginalFontName
		{
			get
			{
				return this.originalFontName;
			}
		}

		/// <summary>Gets a value that indicates whether this <see cref="T:System.Drawing.Font" /> is underlined.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Drawing.Font" /> is underlined; otherwise, <see langword="false" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Underline
		{
			get
			{
				return this._underline;
			}
		}

		/// <summary>Gets the unit of measure for this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.GraphicsUnit" /> that represents the unit of measure for this <see cref="T:System.Drawing.Font" />.</returns>
		[TypeConverter(typeof(FontConverter.FontUnitConverter))]
		public GraphicsUnit Unit
		{
			get
			{
				return this._unit;
			}
		}

		/// <summary>Indicates whether the specified object is a <see cref="T:System.Drawing.Font" /> and has the same <see cref="P:System.Drawing.Font.FontFamily" />, <see cref="P:System.Drawing.Font.GdiVerticalFont" />, <see cref="P:System.Drawing.Font.GdiCharSet" />, <see cref="P:System.Drawing.Font.Style" />, <see cref="P:System.Drawing.Font.Size" />, and <see cref="P:System.Drawing.Font.Unit" /> property values as this <see cref="T:System.Drawing.Font" />.</summary>
		/// <param name="obj">The object to test.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="obj" /> parameter is a <see cref="T:System.Drawing.Font" /> and has the same <see cref="P:System.Drawing.Font.FontFamily" />, <see cref="P:System.Drawing.Font.GdiVerticalFont" />, <see cref="P:System.Drawing.Font.GdiCharSet" />, <see cref="P:System.Drawing.Font.Style" />, <see cref="P:System.Drawing.Font.Size" />, and <see cref="P:System.Drawing.Font.Unit" /> property values as this <see cref="T:System.Drawing.Font" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			Font font = obj as Font;
			return font != null && (font.FontFamily.Equals(this.FontFamily) && font.Size == this.Size && font.Style == this.Style && font.Unit == this.Unit && font.GdiCharSet == this.GdiCharSet && font.GdiVerticalFont == this.GdiVerticalFont);
		}

		/// <summary>Gets the hash code for this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>The hash code for this <see cref="T:System.Drawing.Font" />.</returns>
		public override int GetHashCode()
		{
			if (this._hashCode == 0)
			{
				this._hashCode = 17;
				this._hashCode = this._hashCode * 23 + this._name.GetHashCode();
				this._hashCode = this._hashCode * 23 + this.FontFamily.GetHashCode();
				this._hashCode = this._hashCode * 23 + this._size.GetHashCode();
				this._hashCode = this._hashCode * 23 + this._unit.GetHashCode();
				this._hashCode = this._hashCode * 23 + this._style.GetHashCode();
				this._hashCode = this._hashCode * 23 + (int)this._gdiCharSet;
				this._hashCode = this._hashCode * 23 + this._gdiVerticalFont.GetHashCode();
			}
			return this._hashCode;
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Font" /> from the specified Windows handle to a device context.</summary>
		/// <param name="hdc">A handle to a device context.</param>
		/// <returns>The <see cref="T:System.Drawing.Font" /> this method creates.</returns>
		/// <exception cref="T:System.ArgumentException">The font for the specified device context is not a TrueType font.</exception>
		[MonoTODO("The hdc parameter has no direct equivalent in libgdiplus.")]
		public static Font FromHdc(IntPtr hdc)
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Font" /> from the specified GDI logical font (LOGFONT) structure.</summary>
		/// <param name="lf">An <see cref="T:System.Object" /> that represents the GDI <see langword="LOGFONT" /> structure from which to create the <see cref="T:System.Drawing.Font" />.</param>
		/// <param name="hdc">A handle to a device context that contains additional information about the <paramref name="lf" /> structure.</param>
		/// <returns>The <see cref="T:System.Drawing.Font" /> that this method creates.</returns>
		/// <exception cref="T:System.ArgumentException">The font is not a TrueType font.</exception>
		[MonoTODO("The returned font may not have all it's properties initialized correctly.")]
		public static Font FromLogFont(object lf, IntPtr hdc)
		{
			LOGFONT logfont = (LOGFONT)lf;
			IntPtr newFontObject;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFromLogfont(hdc, ref logfont, out newFontObject));
			return new Font(newFontObject, "Microsoft Sans Serif", FontStyle.Regular, 10f);
		}

		/// <summary>Returns the line spacing, in pixels, of this font.</summary>
		/// <returns>The line spacing, in pixels, of this font.</returns>
		public float GetHeight()
		{
			return this.GetHeight(Graphics.systemDpiY);
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Font" /> from the specified GDI logical font (LOGFONT) structure.</summary>
		/// <param name="lf">An <see cref="T:System.Object" /> that represents the GDI <see langword="LOGFONT" /> structure from which to create the <see cref="T:System.Drawing.Font" />.</param>
		/// <returns>The <see cref="T:System.Drawing.Font" /> that this method creates.</returns>
		public static Font FromLogFont(object lf)
		{
			if (GDIPlus.RunningOnUnix())
			{
				return Font.FromLogFont(lf, IntPtr.Zero);
			}
			IntPtr intPtr = IntPtr.Zero;
			Font result;
			try
			{
				intPtr = GDIPlus.GetDC(IntPtr.Zero);
				result = Font.FromLogFont(lf, intPtr);
			}
			finally
			{
				GDIPlus.ReleaseDC(IntPtr.Zero, intPtr);
			}
			return result;
		}

		/// <summary>Creates a GDI logical font (LOGFONT) structure from this <see cref="T:System.Drawing.Font" />.</summary>
		/// <param name="logFont">An <see cref="T:System.Object" /> to represent the <see langword="LOGFONT" /> structure that this method creates.</param>
		public void ToLogFont(object logFont)
		{
			if (GDIPlus.RunningOnUnix())
			{
				using (Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						this.ToLogFont(logFont, graphics);
						return;
					}
				}
			}
			IntPtr dc = GDIPlus.GetDC(IntPtr.Zero);
			try
			{
				using (Graphics graphics2 = Graphics.FromHdc(dc))
				{
					this.ToLogFont(logFont, graphics2);
				}
			}
			finally
			{
				GDIPlus.ReleaseDC(IntPtr.Zero, dc);
			}
		}

		/// <summary>Creates a GDI logical font (LOGFONT) structure from this <see cref="T:System.Drawing.Font" />.</summary>
		/// <param name="logFont">An <see cref="T:System.Object" /> to represent the <see langword="LOGFONT" /> structure that this method creates.</param>
		/// <param name="graphics">A <see cref="T:System.Drawing.Graphics" /> that provides additional information for the <see langword="LOGFONT" /> structure.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="graphics" /> is <see langword="null" />.</exception>
		public void ToLogFont(object logFont, Graphics graphics)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			if (logFont == null)
			{
				throw new AccessViolationException("logFont");
			}
			if (!logFont.GetType().GetTypeInfo().IsLayoutSequential)
			{
				throw new ArgumentException("logFont", Locale.GetText("Layout must be sequential."));
			}
			Type typeFromHandle = typeof(LOGFONT);
			int num = Marshal.SizeOf(logFont);
			if (num >= Marshal.SizeOf(typeFromHandle))
			{
				IntPtr intPtr = Marshal.AllocHGlobal(num);
				Status status;
				try
				{
					Marshal.StructureToPtr(logFont, intPtr, false);
					status = GDIPlus.GdipGetLogFont(this.NativeObject, graphics.NativeObject, logFont);
					if (status != Status.Ok)
					{
						Marshal.PtrToStructure(intPtr, logFont);
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (Font.CharSetOffset == -1)
				{
					Font.CharSetOffset = (int)Marshal.OffsetOf(typeFromHandle, "lfCharSet");
				}
				GCHandle gchandle = GCHandle.Alloc(logFont, GCHandleType.Pinned);
				try
				{
					IntPtr ptr = gchandle.AddrOfPinnedObject();
					if (Marshal.ReadByte(ptr, Font.CharSetOffset) == 0)
					{
						Marshal.WriteByte(ptr, Font.CharSetOffset, 1);
					}
				}
				finally
				{
					gchandle.Free();
				}
				GDIPlus.CheckStatus(status);
			}
		}

		/// <summary>Returns the line spacing, in the current unit of a specified <see cref="T:System.Drawing.Graphics" />, of this font.</summary>
		/// <param name="graphics">A <see cref="T:System.Drawing.Graphics" /> that holds the vertical resolution, in dots per inch, of the display device as well as settings for page unit and page scale.</param>
		/// <returns>The line spacing, in pixels, of this font.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="graphics" /> is <see langword="null" />.</exception>
		public float GetHeight(Graphics graphics)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			float result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetFontHeight(this.fontObject, graphics.NativeObject, out result));
			return result;
		}

		/// <summary>Returns the height, in pixels, of this <see cref="T:System.Drawing.Font" /> when drawn to a device with the specified vertical resolution.</summary>
		/// <param name="dpi">The vertical resolution, in dots per inch, used to calculate the height of the font.</param>
		/// <returns>The height, in pixels, of this <see cref="T:System.Drawing.Font" />.</returns>
		public float GetHeight(float dpi)
		{
			float result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetFontHeightGivenDPI(this.fontObject, dpi, out result));
			return result;
		}

		/// <summary>Returns a human-readable string representation of this <see cref="T:System.Drawing.Font" />.</summary>
		/// <returns>A string that represents this <see cref="T:System.Drawing.Font" />.</returns>
		public override string ToString()
		{
			return string.Format("[Font: Name={0}, Size={1}, Units={2}, GdiCharSet={3}, GdiVerticalFont={4}]", new object[]
			{
				this._name,
				this.Size,
				(int)this._unit,
				this._gdiCharSet,
				this._gdiVerticalFont
			});
		}

		private IntPtr fontObject = IntPtr.Zero;

		private string systemFontName;

		private string originalFontName;

		private float _size;

		private object olf;

		private const byte DefaultCharSet = 1;

		private static int CharSetOffset = -1;

		private bool _bold;

		private FontFamily _fontFamily;

		private byte _gdiCharSet;

		private bool _gdiVerticalFont;

		private bool _italic;

		private string _name;

		private float _sizeInPoints;

		private bool _strikeout;

		private FontStyle _style;

		private bool _underline;

		private GraphicsUnit _unit;

		private int _hashCode;
	}
}
