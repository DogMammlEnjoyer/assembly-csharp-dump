using System;

namespace System.Drawing.Printing
{
	/// <summary>Specifies a print controller that sends information to a printer.</summary>
	public class StandardPrintController : PrintController
	{
		/// <summary>Completes the control sequence that determines when and how to print a page of a document.</summary>
		/// <param name="document">A <see cref="T:System.Drawing.Printing.PrintDocument" /> that represents the document being printed.</param>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintPageEventArgs" /> that contains data about how to print a page in the document.</param>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The native Win32 Application Programming Interface (API) could not finish writing to a page.</exception>
		public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
		{
			SysPrn.GlobalService.EndPage(e.GraphicsContext);
		}

		/// <summary>Begins the control sequence that determines when and how to print a document.</summary>
		/// <param name="document">A <see cref="T:System.Drawing.Printing.PrintDocument" /> that represents the document being printed.</param>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs" /> that contains data about how to print the document.</param>
		/// <exception cref="T:System.Drawing.Printing.InvalidPrinterException">The printer settings are not valid.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The native Win32 Application Programming Interface (API) could not start a print job.</exception>
		public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
		{
			IntPtr dc = SysPrn.GlobalService.CreateGraphicsContext(document.PrinterSettings, document.DefaultPageSettings);
			e.GraphicsContext = new GraphicsPrinter(null, dc);
			SysPrn.GlobalService.StartDoc(e.GraphicsContext, document.DocumentName, string.Empty);
		}

		/// <summary>Completes the control sequence that determines when and how to print a document.</summary>
		/// <param name="document">A <see cref="T:System.Drawing.Printing.PrintDocument" /> that represents the document being printed.</param>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs" /> that contains data about how to print the document.</param>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The native Win32 Application Programming Interface (API) could not complete the print job.  
		///  -or-  
		///  The native Win32 API could not delete the specified device context (DC).</exception>
		public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
		{
			SysPrn.GlobalService.EndDoc(e.GraphicsContext);
		}

		/// <summary>Begins the control sequence that determines when and how to print a page in a document.</summary>
		/// <param name="document">A <see cref="T:System.Drawing.Printing.PrintDocument" /> that represents the document being printed.</param>
		/// <param name="e">A <see cref="T:System.Drawing.Printing.PrintPageEventArgs" /> that contains data about how to print a page in the document. Initially, the <see cref="P:System.Drawing.Printing.PrintPageEventArgs.Graphics" /> property of this parameter will be <see langword="null" />. The value returned from the <see cref="M:System.Drawing.Printing.StandardPrintController.OnStartPage(System.Drawing.Printing.PrintDocument,System.Drawing.Printing.PrintPageEventArgs)" /> method will be used to set this property.</param>
		/// <returns>A <see cref="T:System.Drawing.Graphics" /> object that represents a page from a <see cref="T:System.Drawing.Printing.PrintDocument" />.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The native Win32 Application Programming Interface (API) could not prepare the printer driver to accept data.  
		///  -or-  
		///  The native Win32 API could not update the specified printer or plotter device context (DC) using the specified information.</exception>
		public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
		{
			SysPrn.GlobalService.StartPage(e.GraphicsContext);
			return e.Graphics;
		}
	}
}
