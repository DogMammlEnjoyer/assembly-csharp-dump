using System;

namespace System.Data
{
	/// <summary>Describes the version of a <see cref="T:System.Data.DataRow" />.</summary>
	public enum DataRowVersion
	{
		/// <summary>The row contains its original values.</summary>
		Original = 256,
		/// <summary>The row contains current values.</summary>
		Current = 512,
		/// <summary>The row contains a proposed value.</summary>
		Proposed = 1024,
		/// <summary>The default version of <see cref="T:System.Data.DataRowState" />. For a <see langword="DataRowState" /> value of <see langword="Added" />, <see langword="Modified" /> or <see langword="Deleted" />, the default version is <see langword="Current" />. For a <see cref="T:System.Data.DataRowState" /> value of <see langword="Detached" />, the version is <see langword="Proposed" />.</summary>
		Default = 1536
	}
}
