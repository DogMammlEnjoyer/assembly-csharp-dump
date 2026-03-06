using System;

namespace System.Data
{
	/// <summary>Describes the version of data in a <see cref="T:System.Data.DataRow" />.</summary>
	[Flags]
	public enum DataViewRowState
	{
		/// <summary>None.</summary>
		None = 0,
		/// <summary>An unchanged row.</summary>
		Unchanged = 2,
		/// <summary>A new row.</summary>
		Added = 4,
		/// <summary>A deleted row.</summary>
		Deleted = 8,
		/// <summary>A current version of original data that has been modified (see <see langword="ModifiedOriginal" />).</summary>
		ModifiedCurrent = 16,
		/// <summary>The original version of the data that was modified. (Although the data has since been modified, it is available as <see langword="ModifiedCurrent" />).</summary>
		ModifiedOriginal = 32,
		/// <summary>Original rows including unchanged and deleted rows.</summary>
		OriginalRows = 42,
		/// <summary>Current rows including unchanged, new, and modified rows. By default, <see cref="T:System.Data.DataViewRowState" /> is set to CurrentRows.</summary>
		CurrentRows = 22
	}
}
