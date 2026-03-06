using System;

namespace UnityEngine.Localization.Tables
{
	[Serializable]
	public struct TableEntryReference : ISerializationCallbackReceiver, IEquatable<TableEntryReference>
	{
		public TableEntryReference.Type ReferenceType { readonly get; private set; }

		public long KeyId
		{
			get
			{
				return this.m_KeyId;
			}
			private set
			{
				this.m_KeyId = value;
			}
		}

		public string Key
		{
			get
			{
				return this.m_Key;
			}
			private set
			{
				this.m_Key = value;
			}
		}

		public static implicit operator TableEntryReference(string key)
		{
			if (!string.IsNullOrWhiteSpace(key))
			{
				return new TableEntryReference
				{
					Key = key,
					ReferenceType = TableEntryReference.Type.Name
				};
			}
			return default(TableEntryReference);
		}

		public static implicit operator TableEntryReference(long keyId)
		{
			if (keyId != 0L)
			{
				return new TableEntryReference
				{
					KeyId = keyId,
					ReferenceType = TableEntryReference.Type.Id
				};
			}
			return default(TableEntryReference);
		}

		public static implicit operator string(TableEntryReference tableEntryReference)
		{
			return tableEntryReference.Key;
		}

		public static implicit operator long(TableEntryReference tableEntryReference)
		{
			return tableEntryReference.KeyId;
		}

		internal void Validate()
		{
			if (this.m_Valid)
			{
				return;
			}
			switch (this.ReferenceType)
			{
			case TableEntryReference.Type.Empty:
				throw new ArgumentException("Empty Table Entry Reference. Must contain a Name or Key Id");
			case TableEntryReference.Type.Name:
				if (string.IsNullOrWhiteSpace(this.Key))
				{
					throw new ArgumentException("Must use a valid Key, can not be null or Empty.");
				}
				break;
			case TableEntryReference.Type.Id:
				if (this.KeyId == 0L)
				{
					throw new ArgumentException("Key Id can not be empty.");
				}
				break;
			}
			this.m_Valid = true;
		}

		public string ResolveKeyName(SharedTableData sharedData)
		{
			if (this.ReferenceType == TableEntryReference.Type.Name)
			{
				return this.Key;
			}
			if (this.ReferenceType != TableEntryReference.Type.Id)
			{
				return null;
			}
			if (!(sharedData != null))
			{
				return string.Format("Key Id {0}", this.KeyId);
			}
			return sharedData.GetKey(this.KeyId);
		}

		public override string ToString()
		{
			TableEntryReference.Type referenceType = this.ReferenceType;
			if (referenceType == TableEntryReference.Type.Name)
			{
				return "TableEntryReference(" + this.Key + ")";
			}
			if (referenceType != TableEntryReference.Type.Id)
			{
				return "TableEntryReference(Empty)";
			}
			return string.Format("{0}({1})", "TableEntryReference", this.KeyId);
		}

		public string ToString(TableReference tableReference)
		{
			SharedTableData sharedTableData = tableReference.SharedTableData;
			if (sharedTableData != null)
			{
				string key;
				long num;
				if (this.ReferenceType == TableEntryReference.Type.Name)
				{
					key = this.Key;
					num = sharedTableData.GetId(key);
				}
				else
				{
					if (this.ReferenceType != TableEntryReference.Type.Id)
					{
						return this.ToString();
					}
					num = this.KeyId;
					key = sharedTableData.GetKey(num);
				}
				return string.Format("{0}({1} - {2})", "TableEntryReference", num, key);
			}
			return this.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is TableEntryReference)
			{
				TableEntryReference other = (TableEntryReference)obj;
				return this.Equals(other);
			}
			return false;
		}

		public bool Equals(TableEntryReference other)
		{
			if (this.ReferenceType != other.ReferenceType)
			{
				return false;
			}
			if (this.ReferenceType == TableEntryReference.Type.Name)
			{
				return this.Key == other.Key;
			}
			return this.ReferenceType != TableEntryReference.Type.Id || this.KeyId == other.KeyId;
		}

		public override int GetHashCode()
		{
			if (this.ReferenceType == TableEntryReference.Type.Name)
			{
				return this.Key.GetHashCode();
			}
			if (this.ReferenceType == TableEntryReference.Type.Id)
			{
				return this.KeyId.GetHashCode();
			}
			return base.GetHashCode();
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.KeyId != 0L)
			{
				this.ReferenceType = TableEntryReference.Type.Id;
				return;
			}
			if (string.IsNullOrEmpty(this.m_Key))
			{
				this.ReferenceType = TableEntryReference.Type.Empty;
				return;
			}
			this.ReferenceType = TableEntryReference.Type.Name;
		}

		[SerializeField]
		private long m_KeyId;

		[SerializeField]
		private string m_Key;

		private bool m_Valid;

		public enum Type
		{
			Empty,
			Name,
			Id
		}
	}
}
