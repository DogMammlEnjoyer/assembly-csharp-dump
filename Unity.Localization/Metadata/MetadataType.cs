using System;

namespace UnityEngine.Localization.Metadata
{
	[Flags]
	public enum MetadataType
	{
		Locale = 1,
		SharedTableData = 2,
		StringTable = 4,
		AssetTable = 8,
		StringTableEntry = 16,
		AssetTableEntry = 32,
		SharedStringTableEntry = 64,
		SharedAssetTableEntry = 128,
		LocalizationSettings = 256,
		AllTables = 12,
		AllTableEntries = 48,
		AllSharedTableEntries = 192,
		All = 511
	}
}
