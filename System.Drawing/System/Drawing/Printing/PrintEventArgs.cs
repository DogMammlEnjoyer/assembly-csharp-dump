using System;
using System.ComponentModel;

namespace System.Drawing.Printing
{
	/// <summary>Provides data for the <see cref="E:System.Drawing.Printing.PrintDocument.BeginPrint" /> and <see cref="E:System.Drawing.Printing.PrintDocument.EndPrint" /> events.</summary>
	public class PrintEventArgs : CancelEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrintEventArgs" /> class.</summary>
		public PrintEventArgs()
		{
		}

		internal PrintEventArgs(PrintAction action)
		{
			this.action = action;
		}

		/// <summary>Returns <see cref="F:System.Drawing.Printing.PrintAction.PrintToFile" /> in all cases.</summary>
		/// <returns>
		///   <see cref="F:System.Drawing.Printing.PrintAction.PrintToFile" /> in all cases.</returns>
		public PrintAction PrintAction
		{
			get
			{
				return this.action;
			}
		}

		internal GraphicsPrinter GraphicsContext
		{
			get
			{
				return this.graphics_context;
			}
			set
			{
				this.graphics_context = value;
			}
		}

		private GraphicsPrinter graphics_context;

		private PrintAction action;
	}
}
