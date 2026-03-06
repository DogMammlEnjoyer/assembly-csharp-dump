using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace UnityEngine.Localization.Tables
{
	[Serializable]
	public struct TableReference : ISerializationCallbackReceiver, IEquatable<TableReference>
	{
		public TableReference.Type ReferenceType { readonly get; private set; }

		public Guid TableCollectionNameGuid { readonly get; private set; }

		public string TableCollectionName
		{
			get
			{
				if (this.ReferenceType == TableReference.Type.Name)
				{
					return this.m_TableCollectionName;
				}
				SharedTableData sharedTableData = this.SharedTableData;
				if (sharedTableData == null)
				{
					return null;
				}
				return sharedTableData.TableCollectionName;
			}
			private set
			{
				this.m_TableCollectionName = value;
			}
		}

		internal SharedTableData SharedTableData
		{
			get
			{
				if (this.ReferenceType == TableReference.Type.Empty || !LocalizationSettings.HasSettings)
				{
					return null;
				}
				if (this.ReferenceType == TableReference.Type.Guid)
				{
					AsyncOperationHandle<SharedTableData> asyncOperationHandle;
					if (LocalizationSettings.StringDatabase != null && LocalizationSettings.StringDatabase.SharedTableDataOperations.TryGetValue(this.TableCollectionNameGuid, out asyncOperationHandle))
					{
						return asyncOperationHandle.Result;
					}
					if (LocalizationSettings.AssetDatabase != null && LocalizationSettings.AssetDatabase.SharedTableDataOperations.TryGetValue(this.TableCollectionNameGuid, out asyncOperationHandle))
					{
						return asyncOperationHandle.Result;
					}
				}
				else if (this.ReferenceType == TableReference.Type.Name)
				{
					LocalizedStringDatabase stringDatabase = LocalizationSettings.StringDatabase;
					foreach (KeyValuePair<Guid, AsyncOperationHandle<SharedTableData>> keyValuePair in ((stringDatabase != null) ? stringDatabase.SharedTableDataOperations : null))
					{
						SharedTableData result = keyValuePair.Value.Result;
						if (((result != null) ? result.TableCollectionName : null) == this.m_TableCollectionName)
						{
							return keyValuePair.Value.Result;
						}
					}
					LocalizedAssetDatabase assetDatabase = LocalizationSettings.AssetDatabase;
					foreach (KeyValuePair<Guid, AsyncOperationHandle<SharedTableData>> keyValuePair2 in ((assetDatabase != null) ? assetDatabase.SharedTableDataOperations : null))
					{
						SharedTableData result2 = keyValuePair2.Value.Result;
						if (((result2 != null) ? result2.TableCollectionName : null) == this.m_TableCollectionName)
						{
							return keyValuePair2.Value.Result;
						}
					}
				}
				return null;
			}
		}

		public static implicit operator TableReference(string tableCollectionName)
		{
			return new TableReference
			{
				TableCollectionName = tableCollectionName,
				ReferenceType = (string.IsNullOrWhiteSpace(tableCollectionName) ? TableReference.Type.Empty : TableReference.Type.Name)
			};
		}

		public static implicit operator TableReference(Guid tableCollectionNameGuid)
		{
			return new TableReference
			{
				TableCollectionNameGuid = tableCollectionNameGuid,
				ReferenceType = ((tableCollectionNameGuid == Guid.Empty) ? TableReference.Type.Empty : TableReference.Type.Guid)
			};
		}

		public static implicit operator string(TableReference tableReference)
		{
			return tableReference.TableCollectionName;
		}

		public static implicit operator Guid(TableReference tableReference)
		{
			return tableReference.TableCollectionNameGuid;
		}

		internal void Validate()
		{
			if (this.m_Valid)
			{
				return;
			}
			switch (this.ReferenceType)
			{
			case TableReference.Type.Empty:
				throw new ArgumentException("Empty Table Reference. Must contain a Guid or Table Collection Name");
			case TableReference.Type.Guid:
				if (this.TableCollectionNameGuid == Guid.Empty)
				{
					throw new ArgumentException("Must use a valid Table Collection Name Guid, can not be Empty.");
				}
				break;
			case TableReference.Type.Name:
				if (string.IsNullOrWhiteSpace(this.TableCollectionName))
				{
					throw new ArgumentException("Table Collection Name can not be null or empty.");
				}
				break;
			}
			this.m_Valid = true;
		}

		internal string GetSerializedString()
		{
			TableReference.Type referenceType = this.ReferenceType;
			if (referenceType == TableReference.Type.Guid)
			{
				return "GUID:" + TableReference.StringFromGuid(this.TableCollectionNameGuid);
			}
			if (referenceType != TableReference.Type.Name)
			{
				return string.Empty;
			}
			return this.TableCollectionName;
		}

		public override string ToString()
		{
			if (this.ReferenceType == TableReference.Type.Guid)
			{
				return string.Format("{0}({1} - {2})", "TableReference", this.TableCollectionNameGuid, this.TableCollectionName);
			}
			if (this.ReferenceType == TableReference.Type.Name)
			{
				return "TableReference(" + this.TableCollectionName + ")";
			}
			return "TableReference(Empty)";
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is TableReference)
			{
				TableReference other = (TableReference)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this.ReferenceType == TableReference.Type.Guid)
			{
				return this.TableCollectionNameGuid.GetHashCode();
			}
			if (this.ReferenceType == TableReference.Type.Name)
			{
				return this.TableCollectionName.GetHashCode();
			}
			return base.GetHashCode();
		}

		public bool Equals(TableReference other)
		{
			if (this.ReferenceType != other.ReferenceType)
			{
				return false;
			}
			if (this.ReferenceType == TableReference.Type.Guid)
			{
				return this.TableCollectionNameGuid == other.TableCollectionNameGuid;
			}
			return this.ReferenceType != TableReference.Type.Name || this.TableCollectionName == other.TableCollectionName;
		}

		internal static Guid GuidFromString(string value)
		{
			Guid result;
			if (TableReference.s_StringToGuidCache.TryGetValue(value, out result))
			{
				return result;
			}
			Guid guid;
			if (Guid.TryParse(value.Substring("GUID:".Length, value.Length - "GUID:".Length), out guid))
			{
				TableReference.s_StringToGuidCache[value] = guid;
				return guid;
			}
			return Guid.Empty;
		}

		internal static string StringFromGuid(Guid value)
		{
			string text;
			if (TableReference.s_GuidToStringCache.TryGetValue(value, out text))
			{
				return text.ToString();
			}
			string text2 = value.ToString("N");
			TableReference.s_GuidToStringCache[value] = text2;
			return text2;
		}

		internal static TableReference TableReferenceFromString(string value)
		{
			if (TableReference.IsGuid(value))
			{
				return TableReference.GuidFromString(value);
			}
			return value;
		}

		internal static bool IsGuid(string value)
		{
			return !string.IsNullOrEmpty(value) && value.StartsWith("GUID:", StringComparison.OrdinalIgnoreCase);
		}

		public void OnBeforeSerialize()
		{
			this.m_TableCollectionName = this.GetSerializedString();
		}

		public void OnAfterDeserialize()
		{
			if (string.IsNullOrEmpty(this.m_TableCollectionName))
			{
				this.ReferenceType = TableReference.Type.Empty;
				return;
			}
			if (TableReference.IsGuid(this.m_TableCollectionName))
			{
				this.TableCollectionNameGuid = TableReference.GuidFromString(this.m_TableCollectionName);
				this.ReferenceType = TableReference.Type.Guid;
				return;
			}
			this.ReferenceType = TableReference.Type.Name;
		}

		private static readonly Dictionary<Guid, string> s_GuidToStringCache = new Dictionary<Guid, string>();

		private static readonly Dictionary<string, Guid> s_StringToGuidCache = new Dictionary<string, Guid>();

		[SerializeField]
		[FormerlySerializedAs("m_TableName")]
		private string m_TableCollectionName;

		private bool m_Valid;

		private const string k_GuidTag = "GUID:";

		public enum Type
		{
			Empty,
			Guid,
			Name
		}
	}
}
