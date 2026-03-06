using System;

namespace System.Drawing.Printing
{
	/// <summary>Provides data for the <see cref="E:System.Drawing.Printing.PrintDocument.QueryPageSettings" /> event.</summary>
	public class QueryPageSettingsEventArgs : PrintEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.QueryPageSettingsEventArgs" /> class.</summary>
		/// <param name="pageSettings">The page settings for the page to be printed.</param>
		public QueryPageSettingsEventArgs(PageSettings pageSettings)
		{
			this._pageSettings = pageSettings;
		}

		/// <summary>Gets or sets the page settings for the page to be printed.</summary>
		/// <returns>The page settings for the page to be printed.</returns>
		public PageSettings PageSettings
		{
			get
			{
				this.PageSettingsChanged = true;
				return this._pageSettings;
			}
			set
			{
				if (value == null)
				{
					value = new PageSettings();
				}
				this._pageSettings = value;
				this.PageSettingsChanged = true;
			}
		}

		private PageSettings _pageSettings;

		internal bool PageSettingsChanged;
	}
}
