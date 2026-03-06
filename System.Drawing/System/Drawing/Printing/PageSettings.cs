using System;

namespace System.Drawing.Printing
{
	/// <summary>Specifies settings that apply to a single, printed page.</summary>
	[Serializable]
	public class PageSettings : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PageSettings" /> class using the default printer.</summary>
		public PageSettings() : this(new PrinterSettings())
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PageSettings" /> class using a specified printer.</summary>
		/// <param name="printerSettings">The <see cref="T:System.Drawing.Printing.PrinterSettings" /> that describes the printer to use.</param>
		public PageSettings(PrinterSettings printerSettings)
		{
			this.margins = new Margins();
			base..ctor();
			this.PrinterSettings = printerSettings;
			this.color = printerSettings.DefaultPageSettings.color;
			this.landscape = printerSettings.DefaultPageSettings.landscape;
			this.paperSize = printerSettings.DefaultPageSettings.paperSize;
			this.paperSource = printerSettings.DefaultPageSettings.paperSource;
			this.printerResolution = printerSettings.DefaultPageSettings.printerResolution;
		}

		internal PageSettings(PrinterSettings printerSettings, bool color, bool landscape, PaperSize paperSize, PaperSource paperSource, PrinterResolution printerResolution)
		{
			this.margins = new Margins();
			base..ctor();
			this.PrinterSettings = printerSettings;
			this.color = color;
			this.landscape = landscape;
			this.paperSize = paperSize;
			this.paperSource = paperSource;
			this.printerResolution = printerResolution;
		}

		/// <summary>Gets the size of the page, taking into account the page orientation specified by the <see cref="P:System.Drawing.Printing.PageSettings.Landscape" /> property.</summary>
		/// <returns>A <see cref="T:System.Drawing.Rectangle" /> that represents the length and width, in hundredths of an inch, of the page.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		public Rectangle Bounds
		{
			get
			{
				int num = this.paperSize.Width;
				int num2 = this.paperSize.Height;
				num -= this.margins.Left + this.margins.Right;
				num2 -= this.margins.Top + this.margins.Bottom;
				if (this.landscape)
				{
					int num3 = num;
					num = num2;
					num2 = num3;
				}
				return new Rectangle(this.margins.Left, this.margins.Top, num, num2);
			}
		}

		/// <summary>Gets or sets a value indicating whether the page should be printed in color.</summary>
		/// <returns>
		///   <see langword="true" /> if the page should be printed in color; otherwise, <see langword="false" />. The default is determined by the printer.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		public bool Color
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the page is printed in landscape or portrait orientation.</summary>
		/// <returns>
		///   <see langword="true" /> if the page should be printed in landscape orientation; otherwise, <see langword="false" />. The default is determined by the printer.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		public bool Landscape
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.landscape;
			}
			set
			{
				this.landscape = value;
			}
		}

		/// <summary>Gets or sets the margins for this page.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.Margins" /> that represents the margins, in hundredths of an inch, for the page. The default is 1-inch margins on all sides.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		public Margins Margins
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.margins;
			}
			set
			{
				this.margins = value;
			}
		}

		/// <summary>Gets or sets the paper size for the page.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PaperSize" /> that represents the size of the paper. The default is the printer's default paper size.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist or there is no default printer installed.</exception>
		public PaperSize PaperSize
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.paperSize;
			}
			set
			{
				if (value != null)
				{
					this.paperSize = value;
				}
			}
		}

		/// <summary>Gets or sets the page's paper source; for example, the printer's upper tray.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PaperSource" /> that specifies the source of the paper. The default is the printer's default paper source.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist or there is no default printer installed.</exception>
		public PaperSource PaperSource
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.paperSource;
			}
			set
			{
				if (value != null)
				{
					this.paperSource = value;
				}
			}
		}

		/// <summary>Gets or sets the printer resolution for the page.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PrinterResolution" /> that specifies the printer resolution for the page. The default is the printer's default resolution.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist or there is no default printer installed.</exception>
		public PrinterResolution PrinterResolution
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.printerResolution;
			}
			set
			{
				if (value != null)
				{
					this.printerResolution = value;
				}
			}
		}

		/// <summary>Gets or sets the printer settings associated with the page.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PrinterSettings" /> that represents the printer settings associated with the page.</returns>
		public PrinterSettings PrinterSettings
		{
			get
			{
				return this.printerSettings;
			}
			set
			{
				this.printerSettings = value;
			}
		}

		/// <summary>Gets the x-coordinate, in hundredths of an inch, of the hard margin at the left of the page.</summary>
		/// <returns>The x-coordinate, in hundredths of an inch, of the left-hand hard margin.</returns>
		public float HardMarginX
		{
			get
			{
				return this.hardMarginX;
			}
		}

		/// <summary>Gets the y-coordinate, in hundredths of an inch, of the hard margin at the top of the page.</summary>
		/// <returns>The y-coordinate, in hundredths of an inch, of the hard margin at the top of the page.</returns>
		public float HardMarginY
		{
			get
			{
				return this.hardMarginY;
			}
		}

		/// <summary>Gets the bounds of the printable area of the page for the printer.</summary>
		/// <returns>A <see cref="T:System.Drawing.RectangleF" /> representing the length and width, in hundredths of an inch, of the area the printer is capable of printing in.</returns>
		public RectangleF PrintableArea
		{
			get
			{
				return this.printableArea;
			}
		}

		/// <summary>Creates a copy of this <see cref="T:System.Drawing.Printing.PageSettings" />.</summary>
		/// <returns>A copy of this object.</returns>
		public object Clone()
		{
			PrinterResolution printerResolution = new PrinterResolution(this.printerResolution.Kind, this.printerResolution.X, this.printerResolution.Y);
			PaperSource paperSource = new PaperSource(this.paperSource.Kind, this.paperSource.SourceName);
			PaperSize paperSize = new PaperSize(this.paperSize.PaperName, this.paperSize.Width, this.paperSize.Height);
			paperSize.RawKind = (int)this.paperSize.Kind;
			return new PageSettings(this.printerSettings, this.color, this.landscape, paperSize, paperSource, printerResolution)
			{
				Margins = (Margins)this.margins.Clone()
			};
		}

		/// <summary>Copies the relevant information from the <see cref="T:System.Drawing.Printing.PageSettings" /> to the specified <see langword="DEVMODE" /> structure.</summary>
		/// <param name="hdevmode">The handle to a Win32 <see langword="DEVMODE" /> structure.</param>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist or there is no default printer installed.</exception>
		[MonoTODO("PageSettings.CopyToHdevmode")]
		public void CopyToHdevmode(IntPtr hdevmode)
		{
			throw new NotImplementedException();
		}

		/// <summary>Copies relevant information to the <see cref="T:System.Drawing.Printing.PageSettings" /> from the specified <see langword="DEVMODE" /> structure.</summary>
		/// <param name="hdevmode">The handle to a Win32 <see langword="DEVMODE" /> structure.</param>
		/// <exception cref="T:System.ArgumentException">The printer handle is not valid.</exception>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist or there is no default printer installed.</exception>
		[MonoTODO("PageSettings.SetHdevmode")]
		public void SetHdevmode(IntPtr hdevmode)
		{
			throw new NotImplementedException();
		}

		/// <summary>Converts the <see cref="T:System.Drawing.Printing.PageSettings" /> to string form.</summary>
		/// <returns>A string showing the various property settings for the <see cref="T:System.Drawing.Printing.PageSettings" />.</returns>
		public override string ToString()
		{
			return string.Format("[PageSettings: Color={0}" + ", Landscape={1}" + ", Margins={2}" + ", PaperSize={3}" + ", PaperSource={4}" + ", PrinterResolution={5}" + "]", new object[]
			{
				this.color,
				this.landscape,
				this.margins,
				this.paperSize,
				this.paperSource,
				this.printerResolution
			});
		}

		internal bool color;

		internal bool landscape;

		internal PaperSize paperSize;

		internal PaperSource paperSource;

		internal PrinterResolution printerResolution;

		private Margins margins;

		private float hardMarginX;

		private float hardMarginY;

		private RectangleF printableArea;

		private PrinterSettings printerSettings;
	}
}
