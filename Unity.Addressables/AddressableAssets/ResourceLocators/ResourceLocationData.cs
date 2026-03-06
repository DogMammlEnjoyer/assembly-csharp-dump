using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.Utility;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Serialization;

namespace UnityEngine.AddressableAssets.ResourceLocators
{
	[Serializable]
	public class ResourceLocationData
	{
		public string[] Keys
		{
			get
			{
				return this.m_Keys;
			}
		}

		public string InternalId
		{
			get
			{
				return this.m_InternalId;
			}
		}

		public string Provider
		{
			get
			{
				return this.m_Provider;
			}
		}

		public string[] Dependencies
		{
			get
			{
				return this.m_Dependencies;
			}
		}

		public Type ResourceType
		{
			get
			{
				return this.m_ResourceType.Value;
			}
		}

		public object Data
		{
			get
			{
				if (this._Data == null)
				{
					if (this.SerializedData == null || this.SerializedData.Length == 0)
					{
						return null;
					}
					this._Data = SerializationUtilities.ReadObjectFromByteArray(this.SerializedData, 0);
				}
				return this._Data;
			}
			set
			{
				List<byte> list = new List<byte>();
				SerializationUtilities.WriteObjectToByteList(value, list);
				this.SerializedData = list.ToArray();
			}
		}

		public ResourceLocationData(string[] keys, string id, Type provider, Type t, string[] dependencies = null)
		{
			this.m_Keys = keys;
			this.m_InternalId = id;
			this.m_Provider = ((provider == null) ? "" : provider.FullName);
			this.m_Dependencies = ((dependencies == null) ? new string[0] : dependencies);
			this.m_ResourceType = new SerializedType
			{
				Value = t
			};
		}

		[FormerlySerializedAs("m_keys")]
		[SerializeField]
		private string[] m_Keys;

		[FormerlySerializedAs("m_internalId")]
		[SerializeField]
		private string m_InternalId;

		[FormerlySerializedAs("m_provider")]
		[SerializeField]
		private string m_Provider;

		[FormerlySerializedAs("m_dependencies")]
		[SerializeField]
		private string[] m_Dependencies;

		[SerializeField]
		private SerializedType m_ResourceType;

		[SerializeField]
		private byte[] SerializedData;

		private object _Data;
	}
}
