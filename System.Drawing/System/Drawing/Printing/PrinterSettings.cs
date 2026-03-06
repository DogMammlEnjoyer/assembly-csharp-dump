using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace System.Drawing.Printing
{
	/// <summary>Specifies information about how a document is printed, including the printer that prints it, when printing from a Windows Forms application.</summary>
	[Serializable]
	public class PrinterSettings : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrinterSettings" /> class.</summary>
		public PrinterSettings() : this(SysPrn.CreatePrintingService())
		{
		}

		internal PrinterSettings(PrintingServices printing_services)
		{
			this.printing_services = printing_services;
			this.printer_name = printing_services.DefaultPrinter;
			this.ResetToDefaults();
			printing_services.LoadPrinterSettings(this.printer_name, this);
		}

		private void ResetToDefaults()
		{
			this.printer_resolutions = null;
			this.paper_sizes = null;
			this.paper_sources = null;
			this.default_pagesettings = null;
			this.maximum_page = 9999;
			this.copies = 1;
			this.collate = true;
		}

		/// <summary>Gets a value indicating whether the printer supports double-sided printing.</summary>
		/// <returns>
		///   <see langword="true" /> if the printer supports double-sided printing; otherwise, <see langword="false" />.</returns>
		public bool CanDuplex
		{
			get
			{
				return this.can_duplex;
			}
		}

		/// <summary>Gets or sets a value indicating whether the printed document is collated.</summary>
		/// <returns>
		///   <see langword="true" /> if the printed document is collated; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool Collate
		{
			get
			{
				return this.collate;
			}
			set
			{
				this.collate = value;
			}
		}

		/// <summary>Gets or sets the number of copies of the document to print.</summary>
		/// <returns>The number of copies to print. The default is 1.</returns>
		/// <exception cref="T:System.ArgumentException">The value of the <see cref="P:System.Drawing.Printing.PrinterSettings.Copies" /> property is less than zero.</exception>
		public short Copies
		{
			get
			{
				return this.copies;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the Copies property is less than zero.");
				}
				this.copies = value;
			}
		}

		/// <summary>Gets the default page settings for this printer.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PageSettings" /> that represents the default page settings for this printer.</returns>
		public PageSettings DefaultPageSettings
		{
			get
			{
				if (this.default_pagesettings == null)
				{
					this.default_pagesettings = new PageSettings(this, this.SupportsColor, false, new PaperSize("A4", 827, 1169), new PaperSource(PaperSourceKind.FormSource, "Tray"), new PrinterResolution(PrinterResolutionKind.Medium, 200, 200));
				}
				return this.default_pagesettings;
			}
		}

		/// <summary>Gets or sets the printer setting for double-sided printing.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.Printing.Duplex" /> values. The default is determined by the printer.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value of the <see cref="P:System.Drawing.Printing.PrinterSettings.Duplex" /> property is not one of the <see cref="T:System.Drawing.Printing.Duplex" /> values.</exception>
		public Duplex Duplex
		{
			get
			{
				return this.duplex;
			}
			set
			{
				this.duplex = value;
			}
		}

		/// <summary>Gets or sets the page number of the first page to print.</summary>
		/// <returns>The page number of the first page to print.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Drawing.Printing.PrinterSettings.FromPage" /> property's value is less than zero.</exception>
		public int FromPage
		{
			get
			{
				return this.from_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the FromPage property is less than zero");
				}
				this.from_page = value;
			}
		}

		/// <summary>Gets the names of all printers installed on the computer.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PrinterSettings.StringCollection" /> that represents the names of all printers installed on the computer.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The available printers could not be enumerated.</exception>
		public static PrinterSettings.StringCollection InstalledPrinters
		{
			get
			{
				return SysPrn.GlobalService.InstalledPrinters;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property designates the default printer, except when the user explicitly sets <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" />.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> designates the default printer; otherwise, <see langword="false" />.</returns>
		public bool IsDefaultPrinter
		{
			get
			{
				return this.printer_name == this.printing_services.DefaultPrinter;
			}
		}

		/// <summary>Gets a value indicating whether the printer is a plotter.</summary>
		/// <returns>
		///   <see langword="true" /> if the printer is a plotter; <see langword="false" /> if the printer is a raster.</returns>
		public bool IsPlotter
		{
			get
			{
				return this.is_plotter;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property designates a valid printer.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property designates a valid printer; otherwise, <see langword="false" />.</returns>
		public bool IsValid
		{
			get
			{
				return this.printing_services.IsPrinterValid(this.printer_name);
			}
		}

		/// <summary>Gets the angle, in degrees, that the portrait orientation is rotated to produce the landscape orientation.</summary>
		/// <returns>The angle, in degrees, that the portrait orientation is rotated to produce the landscape orientation.</returns>
		public int LandscapeAngle
		{
			get
			{
				return this.landscape_angle;
			}
		}

		/// <summary>Gets the maximum number of copies that the printer enables the user to print at a time.</summary>
		/// <returns>The maximum number of copies that the printer enables the user to print at a time.</returns>
		public int MaximumCopies
		{
			get
			{
				return this.maximum_copies;
			}
		}

		/// <summary>Gets or sets the maximum <see cref="P:System.Drawing.Printing.PrinterSettings.FromPage" /> or <see cref="P:System.Drawing.Printing.PrinterSettings.ToPage" /> that can be selected in a <see cref="T:System.Windows.Forms.PrintDialog" />.</summary>
		/// <returns>The maximum <see cref="P:System.Drawing.Printing.PrinterSettings.FromPage" /> or <see cref="P:System.Drawing.Printing.PrinterSettings.ToPage" /> that can be selected in a <see cref="T:System.Windows.Forms.PrintDialog" />.</returns>
		/// <exception cref="T:System.ArgumentException">The value of the <see cref="P:System.Drawing.Printing.PrinterSettings.MaximumPage" /> property is less than zero.</exception>
		public int MaximumPage
		{
			get
			{
				return this.maximum_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the MaximumPage property is less than zero");
				}
				this.maximum_page = value;
			}
		}

		/// <summary>Gets or sets the minimum <see cref="P:System.Drawing.Printing.PrinterSettings.FromPage" /> or <see cref="P:System.Drawing.Printing.PrinterSettings.ToPage" /> that can be selected in a <see cref="T:System.Windows.Forms.PrintDialog" />.</summary>
		/// <returns>The minimum <see cref="P:System.Drawing.Printing.PrinterSettings.FromPage" /> or <see cref="P:System.Drawing.Printing.PrinterSettings.ToPage" /> that can be selected in a <see cref="T:System.Windows.Forms.PrintDialog" />.</returns>
		/// <exception cref="T:System.ArgumentException">The value of the <see cref="P:System.Drawing.Printing.PrinterSettings.MinimumPage" /> property is less than zero.</exception>
		public int MinimumPage
		{
			get
			{
				return this.minimum_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the MaximumPage property is less than zero");
				}
				this.minimum_page = value;
			}
		}

		/// <summary>Gets the paper sizes that are supported by this printer.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSizeCollection" /> that represents the paper sizes that are supported by this printer.</returns>
		public PrinterSettings.PaperSizeCollection PaperSizes
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidPrinterException(this);
				}
				return this.paper_sizes;
			}
		}

		/// <summary>Gets the paper source trays that are available on the printer.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSourceCollection" /> that represents the paper source trays that are available on this printer.</returns>
		public PrinterSettings.PaperSourceCollection PaperSources
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidPrinterException(this);
				}
				return this.paper_sources;
			}
		}

		/// <summary>Gets or sets the file name, when printing to a file.</summary>
		/// <returns>The file name, when printing to a file.</returns>
		public string PrintFileName
		{
			get
			{
				return this.print_filename;
			}
			set
			{
				this.print_filename = value;
			}
		}

		/// <summary>Gets or sets the name of the printer to use.</summary>
		/// <returns>The name of the printer to use.</returns>
		public string PrinterName
		{
			get
			{
				return this.printer_name;
			}
			set
			{
				if (this.printer_name == value)
				{
					return;
				}
				this.printer_name = value;
				this.printing_services.LoadPrinterSettings(this.printer_name, this);
			}
		}

		/// <summary>Gets all the resolutions that are supported by this printer.</summary>
		/// <returns>A <see cref="T:System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection" /> that represents the resolutions that are supported by this printer.</returns>
		public PrinterSettings.PrinterResolutionCollection PrinterResolutions
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidPrinterException(this);
				}
				if (this.printer_resolutions == null)
				{
					this.printer_resolutions = new PrinterSettings.PrinterResolutionCollection(new PrinterResolution[0]);
					this.printing_services.LoadPrinterResolutions(this.printer_name, this);
				}
				return this.printer_resolutions;
			}
		}

		/// <summary>Gets or sets the page numbers that the user has specified to be printed.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.Printing.PrintRange" /> values.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value of the <see cref="P:System.Drawing.Printing.PrinterSettings.PrintRange" /> property is not one of the <see cref="T:System.Drawing.Printing.PrintRange" /> values.</exception>
		public PrintRange PrintRange
		{
			get
			{
				return this.print_range;
			}
			set
			{
				if (value != PrintRange.AllPages && value != PrintRange.Selection && value != PrintRange.SomePages)
				{
					throw new InvalidEnumArgumentException("The value of the PrintRange property is not one of the PrintRange values");
				}
				this.print_range = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the printing output is sent to a file instead of a port.</summary>
		/// <returns>
		///   <see langword="true" /> if the printing output is sent to a file; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool PrintToFile
		{
			get
			{
				return this.print_tofile;
			}
			set
			{
				this.print_tofile = value;
			}
		}

		/// <summary>Gets a value indicating whether this printer supports color printing.</summary>
		/// <returns>
		///   <see langword="true" /> if this printer supports color; otherwise, <see langword="false" />.</returns>
		public bool SupportsColor
		{
			get
			{
				return this.supports_color;
			}
		}

		/// <summary>Gets or sets the number of the last page to print.</summary>
		/// <returns>The number of the last page to print.</returns>
		/// <exception cref="T:System.ArgumentException">The value of the <see cref="P:System.Drawing.Printing.PrinterSettings.ToPage" /> property is less than zero.</exception>
		public int ToPage
		{
			get
			{
				return this.to_page;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value of the ToPage property is less than zero");
				}
				this.to_page = value;
			}
		}

		internal NameValueCollection PrinterCapabilities
		{
			get
			{
				if (this.printer_capabilities == null)
				{
					this.printer_capabilities = new NameValueCollection();
				}
				return this.printer_capabilities;
			}
		}

		/// <summary>Creates a copy of this <see cref="T:System.Drawing.Printing.PrinterSettings" />.</summary>
		/// <returns>A copy of this object.</returns>
		public object Clone()
		{
			return new PrinterSettings(this.printing_services);
		}

		/// <summary>Returns a <see cref="T:System.Drawing.Graphics" /> that contains printer information that is useful when creating a <see cref="T:System.Drawing.Printing.PrintDocument" />.</summary>
		/// <returns>A <see cref="T:System.Drawing.Graphics" /> that contains information from a printer.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics()
		{
			throw new NotImplementedException();
		}

		/// <summary>Returns a <see cref="T:System.Drawing.Graphics" /> that contains printer information, optionally specifying the origin at the margins.</summary>
		/// <param name="honorOriginAtMargins">
		///   <see langword="true" /> to indicate the origin at the margins; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.Drawing.Graphics" /> that contains printer information from the <see cref="T:System.Drawing.Printing.PageSettings" />.</returns>
		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics(bool honorOriginAtMargins)
		{
			throw new NotImplementedException();
		}

		/// <summary>Returns a <see cref="T:System.Drawing.Graphics" /> that contains printer information associated with the specified <see cref="T:System.Drawing.Printing.PageSettings" />.</summary>
		/// <param name="pageSettings">The <see cref="T:System.Drawing.Printing.PageSettings" /> to retrieve a graphics object for.</param>
		/// <returns>A <see cref="T:System.Drawing.Graphics" /> that contains printer information from the <see cref="T:System.Drawing.Printing.PageSettings" />.</returns>
		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics(PageSettings pageSettings)
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Graphics" /> associated with the specified page settings and optionally specifying the origin at the margins.</summary>
		/// <param name="pageSettings">The <see cref="T:System.Drawing.Printing.PageSettings" /> to retrieve a <see cref="T:System.Drawing.Graphics" /> object for.</param>
		/// <param name="honorOriginAtMargins">
		///   <see langword="true" /> to specify the origin at the margins; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.Drawing.Graphics" /> that contains printer information from the <see cref="T:System.Drawing.Printing.PageSettings" />.</returns>
		[MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
		public Graphics CreateMeasurementGraphics(PageSettings pageSettings, bool honorOriginAtMargins)
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a handle to a <see langword="DEVMODE" /> structure that corresponds to the printer settings.</summary>
		/// <returns>A handle to a <see langword="DEVMODE" /> structure.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The printer's initialization information could not be retrieved.</exception>
		[MonoTODO("PrinterSettings.GetHdevmode")]
		public IntPtr GetHdevmode()
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a handle to a <see langword="DEVMODE" /> structure that corresponds to the printer and the page settings specified through the <paramref name="pageSettings" /> parameter.</summary>
		/// <param name="pageSettings">The <see cref="T:System.Drawing.Printing.PageSettings" /> object that the <see langword="DEVMODE" /> structure's handle corresponds to.</param>
		/// <returns>A handle to a <see langword="DEVMODE" /> structure.</returns>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer named in the <see cref="P:System.Drawing.Printing.PrinterSettings.PrinterName" /> property does not exist.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The printer's initialization information could not be retrieved.</exception>
		[MonoTODO("PrinterSettings.GetHdevmode")]
		public IntPtr GetHdevmode(PageSettings pageSettings)
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a handle to a <see langword="DEVNAMES" /> structure that corresponds to the printer settings.</summary>
		/// <returns>A handle to a <see langword="DEVNAMES" /> structure.</returns>
		[MonoTODO("PrinterSettings.GetHdevname")]
		public IntPtr GetHdevnames()
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets a value indicating whether the printer supports printing the specified image file.</summary>
		/// <param name="image">The image to print.</param>
		/// <returns>
		///   <see langword="true" /> if the printer supports printing the specified image; otherwise, <see langword="false" />.</returns>
		[MonoTODO("IsDirectPrintingSupported")]
		public bool IsDirectPrintingSupported(Image image)
		{
			throw new NotImplementedException();
		}

		/// <summary>Returns a value indicating whether the printer supports printing the specified image format.</summary>
		/// <param name="imageFormat">An <see cref="T:System.Drawing.Imaging.ImageFormat" /> to print.</param>
		/// <returns>
		///   <see langword="true" /> if the printer supports printing the specified image format; otherwise, <see langword="false" />.</returns>
		[MonoTODO("IsDirectPrintingSupported")]
		public bool IsDirectPrintingSupported(ImageFormat imageFormat)
		{
			throw new NotImplementedException();
		}

		/// <summary>Copies the relevant information out of the given handle and into the <see cref="T:System.Drawing.Printing.PrinterSettings" />.</summary>
		/// <param name="hdevmode">The handle to a Win32 <see langword="DEVMODE" /> structure.</param>
		/// <exception cref="T:System.ArgumentException">The printer handle is not valid.</exception>
		[MonoTODO("PrinterSettings.SetHdevmode")]
		public void SetHdevmode(IntPtr hdevmode)
		{
			throw new NotImplementedException();
		}

		/// <summary>Copies the relevant information out of the given handle and into the <see cref="T:System.Drawing.Printing.PrinterSettings" />.</summary>
		/// <param name="hdevnames">The handle to a Win32 <see langword="DEVNAMES" /> structure.</param>
		/// <exception cref="T:System.ArgumentException">The printer handle is invalid.</exception>
		[MonoTODO("PrinterSettings.SetHdevnames")]
		public void SetHdevnames(IntPtr hdevnames)
		{
			throw new NotImplementedException();
		}

		/// <summary>Provides information about the <see cref="T:System.Drawing.Printing.PrinterSettings" /> in string form.</summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Printer [PrinterSettings ",
				this.printer_name,
				" Copies=",
				this.copies.ToString(),
				" Collate=",
				this.collate.ToString(),
				" Duplex=",
				this.can_duplex.ToString(),
				" FromPage=",
				this.from_page.ToString(),
				" LandscapeAngle=",
				this.landscape_angle.ToString(),
				" MaximumCopies=",
				this.maximum_copies.ToString(),
				" OutputPort= ToPage=",
				this.to_page.ToString(),
				"]"
			});
		}

		private string printer_name;

		private string print_filename;

		private short copies;

		private int maximum_page;

		private int minimum_page;

		private int from_page;

		private int to_page;

		private bool collate;

		private PrintRange print_range;

		internal int maximum_copies;

		internal bool can_duplex;

		internal bool supports_color;

		internal int landscape_angle;

		private bool print_tofile;

		internal PrinterSettings.PrinterResolutionCollection printer_resolutions;

		internal PrinterSettings.PaperSizeCollection paper_sizes;

		internal PrinterSettings.PaperSourceCollection paper_sources;

		private PageSettings default_pagesettings;

		private Duplex duplex;

		internal bool is_plotter;

		private PrintingServices printing_services;

		internal NameValueCollection printer_capabilities;

		/// <summary>Contains a collection of <see cref="T:System.Drawing.Printing.PaperSource" /> objects.</summary>
		public class PaperSourceCollection : ICollection, IEnumerable
		{
			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSourceCollection" /> class.</summary>
			/// <param name="array">An array of type <see cref="T:System.Drawing.Printing.PaperSource" />.</param>
			public PaperSourceCollection(PaperSource[] array)
			{
				foreach (PaperSource value in array)
				{
					this._PaperSources.Add(value);
				}
			}

			/// <summary>Gets the number of different paper sources in the collection.</summary>
			/// <returns>The number of different paper sources in the collection.</returns>
			public int Count
			{
				get
				{
					return this._PaperSources.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.Count" />.</summary>
			int ICollection.Count
			{
				get
				{
					return this._PaperSources.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			/// <summary>Adds the specified <see cref="T:System.Drawing.Printing.PaperSource" /> to end of the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSourceCollection" />.</summary>
			/// <param name="paperSource">The <see cref="T:System.Drawing.Printing.PaperSource" /> to add to the collection.</param>
			/// <returns>The zero-based index where the <see cref="T:System.Drawing.Printing.PaperSource" /> was added.</returns>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(PaperSource paperSource)
			{
				return this._PaperSources.Add(paperSource);
			}

			/// <summary>Copies the contents of the current <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSourceCollection" /> to the specified array, starting at the specified index.</summary>
			/// <param name="paperSources">A zero-based array that receives the items copied from the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSourceCollection" />.</param>
			/// <param name="index">The index at which to start copying items.</param>
			public void CopyTo(PaperSource[] paperSources, int index)
			{
				throw new NotImplementedException();
			}

			/// <summary>Gets the <see cref="T:System.Drawing.Printing.PaperSource" /> at a specified index.</summary>
			/// <param name="index">The index of the <see cref="T:System.Drawing.Printing.PaperSource" /> to get.</param>
			/// <returns>The <see cref="T:System.Drawing.Printing.PaperSource" /> at the specified index.</returns>
			public virtual PaperSource this[int index]
			{
				get
				{
					return this._PaperSources[index] as PaperSource;
				}
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</summary>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._PaperSources.GetEnumerator();
			}

			/// <summary>Returns an enumerator that can iterate through the collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSourceCollection" />.</returns>
			public IEnumerator GetEnumerator()
			{
				return this._PaperSources.GetEnumerator();
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.ICollection.CopyTo(System.Array,System.Int32)" />.</summary>
			/// <param name="array">The destination array for the contents of the collection.</param>
			/// <param name="index">The index at which to start the copy operation.</param>
			void ICollection.CopyTo(Array array, int index)
			{
				this._PaperSources.CopyTo(array, index);
			}

			internal void Clear()
			{
				this._PaperSources.Clear();
			}

			private ArrayList _PaperSources = new ArrayList();
		}

		/// <summary>Contains a collection of <see cref="T:System.Drawing.Printing.PaperSize" /> objects.</summary>
		public class PaperSizeCollection : ICollection, IEnumerable
		{
			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSizeCollection" /> class.</summary>
			/// <param name="array">An array of type <see cref="T:System.Drawing.Printing.PaperSize" />.</param>
			public PaperSizeCollection(PaperSize[] array)
			{
				foreach (PaperSize value in array)
				{
					this._PaperSizes.Add(value);
				}
			}

			/// <summary>Gets the number of different paper sizes in the collection.</summary>
			/// <returns>The number of different paper sizes in the collection.</returns>
			public int Count
			{
				get
				{
					return this._PaperSizes.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.Count" />.</summary>
			int ICollection.Count
			{
				get
				{
					return this._PaperSizes.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			/// <summary>Adds a <see cref="T:System.Drawing.Printing.PrinterResolution" /> to the end of the collection.</summary>
			/// <param name="paperSize">The <see cref="T:System.Drawing.Printing.PaperSize" /> to add to the collection.</param>
			/// <returns>The zero-based index of the newly added item.</returns>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(PaperSize paperSize)
			{
				return this._PaperSizes.Add(paperSize);
			}

			/// <summary>Copies the contents of the current <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSizeCollection" /> to the specified array, starting at the specified index.</summary>
			/// <param name="paperSizes">A zero-based array that receives the items copied from the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSizeCollection" />.</param>
			/// <param name="index">The index at which to start copying items.</param>
			public void CopyTo(PaperSize[] paperSizes, int index)
			{
				throw new NotImplementedException();
			}

			/// <summary>Gets the <see cref="T:System.Drawing.Printing.PaperSize" /> at a specified index.</summary>
			/// <param name="index">The index of the <see cref="T:System.Drawing.Printing.PaperSize" /> to get.</param>
			/// <returns>The <see cref="T:System.Drawing.Printing.PaperSize" /> at the specified index.</returns>
			public virtual PaperSize this[int index]
			{
				get
				{
					return this._PaperSizes[index] as PaperSize;
				}
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</summary>
			/// <returns>An enumerator associated with the collection.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._PaperSizes.GetEnumerator();
			}

			/// <summary>Returns an enumerator that can iterate through the collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Drawing.Printing.PrinterSettings.PaperSizeCollection" />.</returns>
			public IEnumerator GetEnumerator()
			{
				return this._PaperSizes.GetEnumerator();
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.ICollection.CopyTo(System.Array,System.Int32)" />.</summary>
			/// <param name="array">A zero-based array that receives the items copied from the collection.</param>
			/// <param name="index">The index at which to start copying items.</param>
			void ICollection.CopyTo(Array array, int index)
			{
				this._PaperSizes.CopyTo(array, index);
			}

			internal void Clear()
			{
				this._PaperSizes.Clear();
			}

			private ArrayList _PaperSizes = new ArrayList();
		}

		/// <summary>Contains a collection of <see cref="T:System.Drawing.Printing.PrinterResolution" /> objects.</summary>
		public class PrinterResolutionCollection : ICollection, IEnumerable
		{
			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection" /> class.</summary>
			/// <param name="array">An array of type <see cref="T:System.Drawing.Printing.PrinterResolution" />.</param>
			public PrinterResolutionCollection(PrinterResolution[] array)
			{
				foreach (PrinterResolution value in array)
				{
					this._PrinterResolutions.Add(value);
				}
			}

			/// <summary>Gets the number of available printer resolutions in the collection.</summary>
			/// <returns>The number of available printer resolutions in the collection.</returns>
			public int Count
			{
				get
				{
					return this._PrinterResolutions.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.Count" />.</summary>
			int ICollection.Count
			{
				get
				{
					return this._PrinterResolutions.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			/// <summary>Adds a <see cref="T:System.Drawing.Printing.PrinterResolution" /> to the end of the collection.</summary>
			/// <param name="printerResolution">The <see cref="T:System.Drawing.Printing.PrinterResolution" /> to add to the collection.</param>
			/// <returns>The zero-based index of the newly added item.</returns>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(PrinterResolution printerResolution)
			{
				return this._PrinterResolutions.Add(printerResolution);
			}

			/// <summary>Copies the contents of the current <see cref="T:System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection" /> to the specified array, starting at the specified index.</summary>
			/// <param name="printerResolutions">A zero-based array that receives the items copied from the <see cref="T:System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection" />.</param>
			/// <param name="index">The index at which to start copying items.</param>
			public void CopyTo(PrinterResolution[] printerResolutions, int index)
			{
				throw new NotImplementedException();
			}

			/// <summary>Gets the <see cref="T:System.Drawing.Printing.PrinterResolution" /> at a specified index.</summary>
			/// <param name="index">The index of the <see cref="T:System.Drawing.Printing.PrinterResolution" /> to get.</param>
			/// <returns>The <see cref="T:System.Drawing.Printing.PrinterResolution" /> at the specified index.</returns>
			public virtual PrinterResolution this[int index]
			{
				get
				{
					return this._PrinterResolutions[index] as PrinterResolution;
				}
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</summary>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._PrinterResolutions.GetEnumerator();
			}

			/// <summary>Returns an enumerator that can iterate through the collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection" />.</returns>
			public IEnumerator GetEnumerator()
			{
				return this._PrinterResolutions.GetEnumerator();
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.ICollection.CopyTo(System.Array,System.Int32)" />.</summary>
			/// <param name="array">The destination array.</param>
			/// <param name="index">The index at which to start the copy operation.</param>
			void ICollection.CopyTo(Array array, int index)
			{
				this._PrinterResolutions.CopyTo(array, index);
			}

			internal void Clear()
			{
				this._PrinterResolutions.Clear();
			}

			private ArrayList _PrinterResolutions = new ArrayList();
		}

		/// <summary>Contains a collection of <see cref="T:System.String" /> objects.</summary>
		public class StringCollection : ICollection, IEnumerable
		{
			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrinterSettings.StringCollection" /> class.</summary>
			/// <param name="array">An array of type <see cref="T:System.String" />.</param>
			public StringCollection(string[] array)
			{
				foreach (string value in array)
				{
					this._Strings.Add(value);
				}
			}

			/// <summary>Gets the number of strings in the collection.</summary>
			/// <returns>The number of strings in the collection.</returns>
			public int Count
			{
				get
				{
					return this._Strings.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.Count" />.</summary>
			int ICollection.Count
			{
				get
				{
					return this._Strings.Count;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			/// <summary>Gets the <see cref="T:System.String" /> at a specified index.</summary>
			/// <param name="index">The index of the <see cref="T:System.String" /> to get.</param>
			/// <returns>The <see cref="T:System.String" /> at the specified index.</returns>
			public virtual string this[int index]
			{
				get
				{
					return this._Strings[index] as string;
				}
			}

			/// <summary>Adds a string to the end of the collection.</summary>
			/// <param name="value">The string to add to the collection.</param>
			/// <returns>The zero-based index of the newly added item.</returns>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public int Add(string value)
			{
				return this._Strings.Add(value);
			}

			/// <summary>Copies the contents of the current <see cref="T:System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection" /> to the specified array, starting at the specified index</summary>
			/// <param name="strings">A zero-based array that receives the items copied from the <see cref="T:System.Drawing.Printing.PrinterSettings.StringCollection" />.</param>
			/// <param name="index">The index at which to start copying items.</param>
			public void CopyTo(string[] strings, int index)
			{
				throw new NotImplementedException();
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</summary>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._Strings.GetEnumerator();
			}

			/// <summary>Returns an enumerator that can iterate through the collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Drawing.Printing.PrinterSettings.StringCollection" />.</returns>
			public IEnumerator GetEnumerator()
			{
				return this._Strings.GetEnumerator();
			}

			/// <summary>For a description of this member, see <see cref="M:System.Collections.ICollection.CopyTo(System.Array,System.Int32)" />.</summary>
			/// <param name="array">The array for items to be copied to.</param>
			/// <param name="index">The starting index.</param>
			void ICollection.CopyTo(Array array, int index)
			{
				this._Strings.CopyTo(array, index);
			}

			private ArrayList _Strings = new ArrayList();
		}
	}
}
