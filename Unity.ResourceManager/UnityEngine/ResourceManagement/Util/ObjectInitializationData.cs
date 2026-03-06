using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace UnityEngine.ResourceManagement.Util
{
	[Serializable]
	public struct ObjectInitializationData
	{
		public string Id
		{
			get
			{
				return this.m_Id;
			}
		}

		public SerializedType ObjectType
		{
			get
			{
				return this.m_ObjectType;
			}
		}

		public string Data
		{
			get
			{
				return this.m_Data;
			}
		}

		public override string ToString()
		{
			return string.Format("ObjectInitializationData: id={0}, type={1}", this.m_Id, this.m_ObjectType);
		}

		public TObject CreateInstance<TObject>(string idOverride = null)
		{
			TObject tobject;
			try
			{
				Type value = this.m_ObjectType.Value;
				if (value == null)
				{
					tobject = default(TObject);
					tobject = tobject;
				}
				else
				{
					object obj = Activator.CreateInstance(value, true);
					IInitializableObject initializableObject = obj as IInitializableObject;
					if (initializableObject != null && !initializableObject.Initialize((idOverride == null) ? this.m_Id : idOverride, this.m_Data))
					{
						tobject = default(TObject);
					}
					else
					{
						tobject = (TObject)((object)obj);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				tobject = default(TObject);
			}
			return tobject;
		}

		public AsyncOperationHandle GetAsyncInitHandle(ResourceManager rm, string idOverride = null)
		{
			AsyncOperationHandle asyncOperationHandle;
			try
			{
				Type value = this.m_ObjectType.Value;
				if (value == null)
				{
					asyncOperationHandle = default(AsyncOperationHandle);
					asyncOperationHandle = asyncOperationHandle;
				}
				else
				{
					IInitializableObject initializableObject = Activator.CreateInstance(value, true) as IInitializableObject;
					if (initializableObject != null)
					{
						asyncOperationHandle = initializableObject.InitializeAsync(rm, (idOverride == null) ? this.m_Id : idOverride, this.m_Data);
					}
					else
					{
						asyncOperationHandle = default(AsyncOperationHandle);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				asyncOperationHandle = default(AsyncOperationHandle);
			}
			return asyncOperationHandle;
		}

		[FormerlySerializedAs("m_id")]
		[SerializeField]
		private string m_Id;

		[FormerlySerializedAs("m_objectType")]
		[SerializeField]
		private SerializedType m_ObjectType;

		[FormerlySerializedAs("m_data")]
		[SerializeField]
		private string m_Data;

		internal class Serializer : BinaryStorageBuffer.ISerializationAdapter<ObjectInitializationData>, BinaryStorageBuffer.ISerializationAdapter
		{
			public IEnumerable<BinaryStorageBuffer.ISerializationAdapter> Dependencies
			{
				get
				{
					return null;
				}
			}

			public object Deserialize(BinaryStorageBuffer.Reader reader, Type t, uint offset, out uint size)
			{
				uint num;
				ObjectInitializationData.Serializer.Data data = reader.ReadValue<ObjectInitializationData.Serializer.Data>(offset, out num);
				uint num2;
				string id = reader.ReadString(data.id, out num2, '\0', true);
				uint num3;
				SerializedType objectType = new SerializedType
				{
					Value = reader.ReadObject<Type>(data.type, out num3, true)
				};
				uint num4;
				ObjectInitializationData objectInitializationData = new ObjectInitializationData
				{
					m_Id = id,
					m_ObjectType = objectType,
					m_Data = reader.ReadString(data.data, out num4, '\0', true)
				};
				size = num + num2 + num3 + num4;
				return objectInitializationData;
			}

			public uint Serialize(BinaryStorageBuffer.Writer writer, object val)
			{
				ObjectInitializationData objectInitializationData = (ObjectInitializationData)val;
				ObjectInitializationData.Serializer.Data val2 = new ObjectInitializationData.Serializer.Data
				{
					id = writer.WriteString(objectInitializationData.m_Id, '\0'),
					type = writer.WriteObject(objectInitializationData.ObjectType.Value, false),
					data = writer.WriteString(objectInitializationData.m_Data, '\0')
				};
				return writer.Write<ObjectInitializationData.Serializer.Data>(val2);
			}

			private struct Data
			{
				public uint id;

				public uint type;

				public uint data;
			}
		}
	}
}
