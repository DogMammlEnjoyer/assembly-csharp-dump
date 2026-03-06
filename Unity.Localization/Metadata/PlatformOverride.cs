using System;
using System.Collections.Generic;
using UnityEngine.Localization.Tables;

namespace UnityEngine.Localization.Metadata
{
	[Metadata(AllowedTypes = (MetadataType.StringTableEntry | MetadataType.AssetTableEntry | MetadataType.SharedStringTableEntry | MetadataType.SharedAssetTableEntry), AllowMultiple = false)]
	[Serializable]
	public class PlatformOverride : IEntryOverride, IMetadata, ISerializationCallbackReceiver
	{
		public void AddPlatformTableOverride(RuntimePlatform platform, TableReference table)
		{
			this.AddPlatformOverride(platform, table, default(TableEntryReference), EntryOverrideType.Table);
		}

		public void AddPlatformEntryOverride(RuntimePlatform platform, TableEntryReference entry)
		{
			this.AddPlatformOverride(platform, default(TableReference), entry, EntryOverrideType.Entry);
		}

		public void AddPlatformOverride(RuntimePlatform platform, TableReference table, TableEntryReference entry, EntryOverrideType entryOverrideType = EntryOverrideType.TableAndEntry)
		{
			PlatformOverride.PlatformOverrideData platformOverrideData = null;
			for (int i = 0; i < this.m_PlatformOverrides.Count; i++)
			{
				if (this.m_PlatformOverrides[i].platform == platform)
				{
					platformOverrideData = this.m_PlatformOverrides[i];
					break;
				}
			}
			if (platformOverrideData == null)
			{
				platformOverrideData = new PlatformOverride.PlatformOverrideData
				{
					platform = platform
				};
				this.m_PlatformOverrides.Add(platformOverrideData);
			}
			platformOverrideData.entryOverrideType = entryOverrideType;
			platformOverrideData.tableReference = table;
			platformOverrideData.tableEntryReference = entry;
		}

		public bool RemovePlatformOverride(RuntimePlatform platform)
		{
			for (int i = 0; i < this.m_PlatformOverrides.Count; i++)
			{
				if (this.m_PlatformOverrides[i].platform == platform)
				{
					this.m_PlatformOverrides.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public EntryOverrideType GetOverride(out TableReference tableReference, out TableEntryReference tableEntryReference)
		{
			if (this.m_PlayerPlatformOverride == null)
			{
				tableReference = default(TableReference);
				tableEntryReference = default(TableEntryReference);
				return EntryOverrideType.None;
			}
			tableReference = this.m_PlayerPlatformOverride.tableReference;
			tableEntryReference = this.m_PlayerPlatformOverride.tableEntryReference;
			return this.m_PlayerPlatformOverride.entryOverrideType;
		}

		public EntryOverrideType GetOverride(out TableReference tableReference, out TableEntryReference tableEntryReference, RuntimePlatform platform)
		{
			for (int i = 0; i < this.m_PlatformOverrides.Count; i++)
			{
				if (this.m_PlatformOverrides[i].platform == platform)
				{
					PlatformOverride.PlatformOverrideData platformOverrideData = this.m_PlatformOverrides[i];
					tableReference = platformOverrideData.tableReference;
					tableEntryReference = platformOverrideData.tableEntryReference;
					return platformOverrideData.entryOverrideType;
				}
			}
			tableReference = default(TableReference);
			tableEntryReference = default(TableEntryReference);
			return EntryOverrideType.None;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			for (int i = 0; i < this.m_PlatformOverrides.Count; i++)
			{
				if (this.m_PlatformOverrides[i].platform == Application.platform)
				{
					this.m_PlayerPlatformOverride = this.m_PlatformOverrides[i];
					return;
				}
			}
		}

		[SerializeField]
		private List<PlatformOverride.PlatformOverrideData> m_PlatformOverrides = new List<PlatformOverride.PlatformOverrideData>();

		private PlatformOverride.PlatformOverrideData m_PlayerPlatformOverride;

		[Serializable]
		private class PlatformOverrideData
		{
			public override string ToString()
			{
				switch (this.entryOverrideType)
				{
				case EntryOverrideType.Table:
					return string.Format("{0}: {1}", this.platform, this.tableReference);
				case EntryOverrideType.Entry:
					return string.Format("{0}: {1}", this.platform, this.tableEntryReference);
				case EntryOverrideType.TableAndEntry:
					return string.Format("{0}: {1}/{2}", this.platform, this.tableReference, this.tableEntryReference);
				default:
					return string.Format("{0}: None", this.platform);
				}
			}

			public RuntimePlatform platform;

			public EntryOverrideType entryOverrideType;

			public TableReference tableReference;

			public TableEntryReference tableEntryReference;
		}
	}
}
