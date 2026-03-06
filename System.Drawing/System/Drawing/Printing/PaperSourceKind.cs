using System;

namespace System.Drawing.Printing
{
	/// <summary>Standard paper sources.</summary>
	public enum PaperSourceKind
	{
		/// <summary>The upper bin of a printer (or the default bin, if the printer only has one bin).</summary>
		Upper = 1,
		/// <summary>The lower bin of a printer.</summary>
		Lower,
		/// <summary>The middle bin of a printer.</summary>
		Middle,
		/// <summary>Manually fed paper.</summary>
		Manual,
		/// <summary>An envelope.</summary>
		Envelope,
		/// <summary>Manually fed envelope.</summary>
		ManualFeed,
		/// <summary>Automatically fed paper.</summary>
		AutomaticFeed,
		/// <summary>A tractor feed.</summary>
		TractorFeed,
		/// <summary>Small-format paper.</summary>
		SmallFormat,
		/// <summary>Large-format paper.</summary>
		LargeFormat,
		/// <summary>The printer's large-capacity bin.</summary>
		LargeCapacity,
		/// <summary>A paper cassette.</summary>
		Cassette = 14,
		/// <summary>The printer's default input bin.</summary>
		FormSource,
		/// <summary>A printer-specific paper source.</summary>
		Custom = 257
	}
}
