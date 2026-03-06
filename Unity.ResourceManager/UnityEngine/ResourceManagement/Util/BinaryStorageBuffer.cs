using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.ResourceManagement.Util
{
	internal class BinaryStorageBuffer
	{
		private unsafe static void ComputeHash(void* pData, ulong size, Hash128* hash)
		{
			if (pData == null || size == 0UL)
			{
				*hash = default(Hash128);
				return;
			}
			HashUnsafeUtilities.ComputeHash128(pData, size, hash);
		}

		private static void AddSerializationAdapter(Dictionary<Type, BinaryStorageBuffer.ISerializationAdapter> serializationAdapters, BinaryStorageBuffer.ISerializationAdapter adapter, bool forceOverride = false)
		{
			bool flag = false;
			foreach (Type type in adapter.GetType().GetInterfaces())
			{
				if (type.IsGenericType && typeof(BinaryStorageBuffer.ISerializationAdapter).IsAssignableFrom(type))
				{
					Type type2 = type.GenericTypeArguments[0];
					if (serializationAdapters.ContainsKey(type2))
					{
						if (forceOverride)
						{
							BinaryStorageBuffer.ISerializationAdapter arg = serializationAdapters[type2];
							serializationAdapters.Remove(type2);
							serializationAdapters[type2] = adapter;
							flag = true;
							Debug.Log(string.Format("Replacing adapter for type {0}: {1} -> {2}", type2, arg, adapter));
						}
						else
						{
							Debug.Log(string.Format("Failed to register adapter for type {0}: {1}, {2} is already registered.", type2, adapter, serializationAdapters[type2]));
						}
					}
					else
					{
						serializationAdapters[type2] = adapter;
						flag = true;
					}
				}
			}
			if (flag)
			{
				IEnumerable<BinaryStorageBuffer.ISerializationAdapter> dependencies = adapter.Dependencies;
				if (dependencies != null)
				{
					foreach (BinaryStorageBuffer.ISerializationAdapter adapter2 in dependencies)
					{
						BinaryStorageBuffer.AddSerializationAdapter(serializationAdapters, adapter2, false);
					}
				}
			}
		}

		private static bool GetSerializationAdapter(Dictionary<Type, BinaryStorageBuffer.ISerializationAdapter> serializationAdapters, Type t, out BinaryStorageBuffer.ISerializationAdapter adapter)
		{
			if (!serializationAdapters.TryGetValue(t, out adapter))
			{
				foreach (KeyValuePair<Type, BinaryStorageBuffer.ISerializationAdapter> keyValuePair in serializationAdapters)
				{
					if (keyValuePair.Key.IsAssignableFrom(t))
					{
						BinaryStorageBuffer.ISerializationAdapter value;
						adapter = (value = keyValuePair.Value);
						return value != null;
					}
				}
				Debug.LogError(string.Format("Unable to find serialization adapter for type {0}.", t));
			}
			return adapter != null;
		}

		private const uint kUnicodeStringFlag = 2147483648U;

		private const uint kDynamicStringFlag = 1073741824U;

		private const uint kClearFlagsMask = 1073741823U;

		private class BuiltinTypesSerializer : BinaryStorageBuffer.ISerializationAdapter<int>, BinaryStorageBuffer.ISerializationAdapter, BinaryStorageBuffer.ISerializationAdapter<bool>, BinaryStorageBuffer.ISerializationAdapter<long>, BinaryStorageBuffer.ISerializationAdapter<string>, BinaryStorageBuffer.ISerializationAdapter<Hash128>
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
				size = 0U;
				if (offset == 4294967295U)
				{
					return null;
				}
				if (t == typeof(int))
				{
					return reader.ReadValue<int>(offset, out size);
				}
				if (t == typeof(bool))
				{
					return reader.ReadValue<bool>(offset, out size);
				}
				if (t == typeof(long))
				{
					return reader.ReadValue<long>(offset, out size);
				}
				if (t == typeof(Hash128))
				{
					return reader.ReadValue<Hash128>(offset, out size);
				}
				if (t == typeof(string))
				{
					BinaryStorageBuffer.BuiltinTypesSerializer.ObjectToStringRemap objectToStringRemap = reader.ReadValue<BinaryStorageBuffer.BuiltinTypesSerializer.ObjectToStringRemap>(offset, out size);
					uint num;
					object result = reader.ReadString(objectToStringRemap.stringId, out num, objectToStringRemap.separator, false);
					size += num;
					return result;
				}
				return null;
			}

			private char FindBestSeparator(string str, params char[] seps)
			{
				int num = 0;
				char c2 = '\0';
				for (int i = 0; i < seps.Length; i++)
				{
					char s = seps[i];
					int num2 = str.Count((char c) => c == s);
					if (num2 > num)
					{
						num = num2;
						c2 = s;
					}
				}
				if (num == 0)
				{
					return '\0';
				}
				string[] array = str.Split(c2, StringSplitOptions.None);
				int num3 = 0;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i].Length > 4)
					{
						num3++;
					}
				}
				if (num3 <= 1)
				{
					return '\0';
				}
				return c2;
			}

			public uint Serialize(BinaryStorageBuffer.Writer writer, object val)
			{
				if (val == null)
				{
					return uint.MaxValue;
				}
				Type type = val.GetType();
				if (type == typeof(int))
				{
					return writer.Write<int>((int)val);
				}
				if (type == typeof(bool))
				{
					return writer.Write<bool>((bool)val);
				}
				if (type == typeof(long))
				{
					return writer.Write<long>((long)val);
				}
				if (type == typeof(Hash128))
				{
					return writer.Write<Hash128>((Hash128)val);
				}
				if (!(type == typeof(string)))
				{
					return uint.MaxValue;
				}
				string text = val as string;
				if (string.IsNullOrEmpty(text))
				{
					return uint.MaxValue;
				}
				char c = this.FindBestSeparator(text, new char[]
				{
					'/',
					'\\',
					'.',
					'-',
					'_',
					','
				});
				return writer.Write<BinaryStorageBuffer.BuiltinTypesSerializer.ObjectToStringRemap>(new BinaryStorageBuffer.BuiltinTypesSerializer.ObjectToStringRemap
				{
					stringId = writer.WriteString((string)val, c),
					separator = c
				});
			}

			private struct ObjectToStringRemap
			{
				public uint stringId;

				public char separator;
			}
		}

		private class TypeSerializer : BinaryStorageBuffer.ISerializationAdapter<Type>, BinaryStorageBuffer.ISerializationAdapter
		{
			public IEnumerable<BinaryStorageBuffer.ISerializationAdapter> Dependencies
			{
				get
				{
					return null;
				}
			}

			public object Deserialize(BinaryStorageBuffer.Reader reader, Type type, uint offset, out uint size)
			{
				object result;
				try
				{
					uint num;
					BinaryStorageBuffer.TypeSerializer.Data data = reader.ReadValue<BinaryStorageBuffer.TypeSerializer.Data>(offset, out num);
					uint num2;
					string assemblyString = reader.ReadString(data.assemblyId, out num2, '.', true);
					uint num3;
					string name = reader.ReadString(data.classId, out num3, '.', true);
					size = num + num2 + num3;
					Assembly assembly = Assembly.Load(assemblyString);
					result = ((assembly == null) ? null : assembly.GetType(name));
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					size = 0U;
					result = null;
				}
				return result;
			}

			public uint Serialize(BinaryStorageBuffer.Writer writer, object val)
			{
				if (val == null)
				{
					return uint.MaxValue;
				}
				Type type = val as Type;
				return writer.Write<BinaryStorageBuffer.TypeSerializer.Data>(new BinaryStorageBuffer.TypeSerializer.Data
				{
					assemblyId = writer.WriteString(type.Assembly.FullName, '.'),
					classId = writer.WriteString(type.FullName, '.')
				});
			}

			private struct Data
			{
				public uint assemblyId;

				public uint classId;
			}
		}

		private struct DynamicString
		{
			public uint stringId;

			public uint nextId;
		}

		private struct ObjectTypeData
		{
			public uint typeId;

			public uint objectId;
		}

		public interface ISerializationAdapter
		{
			IEnumerable<BinaryStorageBuffer.ISerializationAdapter> Dependencies { get; }

			uint Serialize(BinaryStorageBuffer.Writer writer, object val);

			object Deserialize(BinaryStorageBuffer.Reader reader, Type t, uint offset, out uint size);
		}

		public interface ISerializationAdapter<T> : BinaryStorageBuffer.ISerializationAdapter
		{
		}

		public class Reader
		{
			public void GetCacheStats(out int reqCount, out int reqHits)
			{
				reqCount = this.m_Cache.requestCount;
				reqHits = this.m_Cache.requestHits;
			}

			public void ResetCache(int maxCachedObjects, uint minCachedObjSize)
			{
				this.m_MinCachedObjectSize = minCachedObjSize;
				this.m_Cache = new LRUCache<uint, object>(maxCachedObjects);
			}

			private void Init(byte[] data, int maxCachedObjects, uint minCachedObjSize, params BinaryStorageBuffer.ISerializationAdapter[] adapters)
			{
				this.m_Buffer = data;
				this.m_MinCachedObjectSize = minCachedObjSize;
				this.stringBuilder = new StringBuilder(1024);
				this.m_Cache = new LRUCache<uint, object>(maxCachedObjects);
				this.m_Adapters = new Dictionary<Type, BinaryStorageBuffer.ISerializationAdapter>();
				foreach (BinaryStorageBuffer.ISerializationAdapter adapter in adapters)
				{
					BinaryStorageBuffer.AddSerializationAdapter(this.m_Adapters, adapter, false);
				}
				BinaryStorageBuffer.AddSerializationAdapter(this.m_Adapters, new BinaryStorageBuffer.TypeSerializer(), false);
				BinaryStorageBuffer.AddSerializationAdapter(this.m_Adapters, new BinaryStorageBuffer.BuiltinTypesSerializer(), false);
			}

			public void AddSerializationAdapter(BinaryStorageBuffer.ISerializationAdapter a)
			{
				BinaryStorageBuffer.AddSerializationAdapter(this.m_Adapters, a, false);
			}

			public Reader(byte[] data, int maxCachedObjects = 1024, uint minCachedObjSize = 64U, params BinaryStorageBuffer.ISerializationAdapter[] adapters)
			{
				this.Init(data, maxCachedObjects, minCachedObjSize, adapters);
			}

			internal byte[] GetBuffer()
			{
				return this.m_Buffer;
			}

			public Reader(Stream inputStream, uint bufferSize, int maxCachedObjects, uint minCachedObjSize, params BinaryStorageBuffer.ISerializationAdapter[] adapters)
			{
				byte[] array = new byte[(bufferSize == 0U) ? inputStream.Length : ((long)((ulong)bufferSize))];
				inputStream.Read(array, 0, array.Length);
				this.Init(array, maxCachedObjects, minCachedObjSize, adapters);
			}

			private bool TryGetCachedValue(Type t, uint offset, out object val)
			{
				object obj;
				if (this.m_Cache.TryGet(t, offset, out obj))
				{
					val = obj;
					return true;
				}
				val = null;
				return false;
			}

			private bool TryGetCachedValue<T>(uint offset, out T val)
			{
				object obj;
				if (this.m_Cache.TryGet(typeof(T), offset, out obj))
				{
					val = (T)((object)obj);
					return true;
				}
				val = default(T);
				return false;
			}

			public unsafe T[] ReadValueArray<[IsUnmanaged] T>(uint id, out uint readSize, bool cacheValue = true) where T : struct, ValueType
			{
				readSize = 0U;
				if (id == 4294967295U)
				{
					return null;
				}
				if ((ulong)(id - 4U) >= (ulong)((long)this.m_Buffer.Length))
				{
					throw new Exception(string.Format("Data offset {0} is out of bounds of buffer with length of {1}.", id, this.m_Buffer.Length));
				}
				fixed (byte* ptr = &this.m_Buffer[(int)(id - 4U)])
				{
					byte* ptr2 = ptr;
					T[] result;
					if (this.TryGetCachedValue<T[]>(id, out result))
					{
						return result;
					}
					uint num = 0U;
					UnsafeUtility.MemCpy((void*)(&num), (void*)ptr2, 4L);
					if ((ulong)(id + num) > (ulong)((long)this.m_Buffer.Length))
					{
						throw new Exception(string.Format("Data size {0} is out of bounds of buffer with length of {1}.", num, this.m_Buffer.Length));
					}
					T[] array = new T[(ulong)num / (ulong)((long)sizeof(T))];
					T[] array2;
					T* destination;
					if ((array2 = array) == null || array2.Length == 0)
					{
						destination = null;
					}
					else
					{
						destination = &array2[0];
					}
					UnsafeUtility.MemCpy((void*)destination, (void*)(ptr2 + 4), (long)((ulong)num));
					array2 = null;
					if (cacheValue && num >= this.m_MinCachedObjectSize)
					{
						this.m_Cache.TryAdd(id, array);
					}
					readSize = num;
					return array;
				}
			}

			public unsafe uint ProcessObjectArray<T, C>(uint id, out uint size, C context, Action<T, C, int, int> procFunc, bool cacheValues = true)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return 0U;
				}
				fixed (byte* ptr = &this.m_Buffer[(int)(id - 4U)])
				{
					byte* ptr2 = ptr;
					uint num = 0U;
					uint num2 = *(uint*)ptr2 / 4U;
					int num3 = 0;
					while ((long)num3 < (long)((ulong)num2))
					{
						uint num4;
						procFunc(this.ReadObject<T>(*(uint*)(ptr2 + 4 * (1 + num3)), out num4, cacheValues), context, num3, (int)num2);
						num += num4;
						num3++;
					}
					size = num;
					return num2;
				}
			}

			public unsafe uint ReadObjectArray<T>(ref List<T> results, uint id, out uint size, bool cacheValues = true)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return 0U;
				}
				fixed (byte* ptr = &this.m_Buffer[(int)(id - 4U)])
				{
					byte* ptr2 = ptr;
					uint num = 0U;
					uint num2 = *(uint*)ptr2 / 4U;
					int num3 = 0;
					while ((long)num3 < (long)((ulong)num2))
					{
						uint num4;
						results.Add(this.ReadObject<T>(*(uint*)(ptr2 + 4 * (1 + num3)), out num4, cacheValues));
						num += num4;
						num3++;
					}
					size = num;
					return num2;
				}
			}

			public unsafe object[] ReadObjectArray(uint id, out uint size, bool cacheValues = true, bool cacheFullArray = false)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return null;
				}
				object[] result;
				if (this.TryGetCachedValue<object[]>(id, out result))
				{
					size = 0U;
					return result;
				}
				fixed (byte* ptr = &this.m_Buffer[(int)(id - 4U)])
				{
					byte* ptr2 = ptr;
					uint num = 0U;
					uint num2 = *(uint*)ptr2 / 4U;
					object[] array = new object[num2];
					int num3 = 0;
					while ((long)num3 < (long)((ulong)num2))
					{
						uint num4;
						array[num3] = this.ReadObject(*(uint*)(ptr2 + 4 * (1 + num3)), out num4, cacheValues);
						num += num4;
						num3++;
					}
					size = num;
					if (cacheFullArray && size >= this.m_MinCachedObjectSize)
					{
						this.m_Cache.TryAdd(id, array);
					}
					return array;
				}
			}

			public unsafe object[] ReadObjectArray(Type t, uint id, out uint size, bool cacheValues = true, bool cacheFullArray = false)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return null;
				}
				object[] result;
				if (this.TryGetCachedValue<object[]>(id, out result))
				{
					size = 0U;
					return result;
				}
				fixed (byte* ptr = &this.m_Buffer[(int)(id - 4U)])
				{
					byte* ptr2 = ptr;
					uint num = 0U;
					uint num2 = *(uint*)ptr2 / 4U;
					object[] array = new object[num2];
					int num3 = 0;
					while ((long)num3 < (long)((ulong)num2))
					{
						uint num4;
						array[num3] = this.ReadObject(t, *(uint*)(ptr2 + 4 * (1 + num3)), out num4, cacheValues);
						num += num4;
						num3++;
					}
					size = num;
					if (cacheFullArray && size >= this.m_MinCachedObjectSize)
					{
						this.m_Cache.TryAdd(id, array);
					}
					return array;
				}
			}

			public unsafe T[] ReadObjectArray<T>(uint id, out uint size, bool cacheValues = true, bool cacheFullArray = false)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return null;
				}
				T[] result;
				if (this.TryGetCachedValue<T[]>(id, out result))
				{
					size = 0U;
					return result;
				}
				fixed (byte* ptr = &this.m_Buffer[(int)(id - 4U)])
				{
					byte* ptr2 = ptr;
					uint num = 0U;
					uint num2 = *(uint*)ptr2 / 4U;
					T[] array = new T[num2];
					int num3 = 0;
					while ((long)num3 < (long)((ulong)num2))
					{
						uint num4;
						array[num3] = this.ReadObject<T>(*(uint*)(ptr2 + 4 * (1 + num3)), out num4, cacheValues);
						num += num4;
						num3++;
					}
					size = num;
					if (cacheFullArray && size >= this.m_MinCachedObjectSize)
					{
						this.m_Cache.TryAdd(id, array);
					}
					return array;
				}
			}

			public object ReadObject(uint id, out uint size, bool cacheValue = true)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return null;
				}
				uint num;
				BinaryStorageBuffer.ObjectTypeData objectTypeData = this.ReadValue<BinaryStorageBuffer.ObjectTypeData>(id, out num);
				uint num2;
				Type t = this.ReadObject<Type>(objectTypeData.typeId, out num2, true);
				uint num3;
				object result = this.ReadObject(t, objectTypeData.objectId, out num3, cacheValue);
				size = num + num2 + num3;
				return result;
			}

			public T ReadObject<T>(uint id, out uint size, bool cacheValue = true)
			{
				size = 0U;
				if (id == 4294967295U)
				{
					T result = default(T);
					return result;
				}
				T result2;
				if (this.TryGetCachedValue<T>(id, out result2))
				{
					return result2;
				}
				BinaryStorageBuffer.ISerializationAdapter serializationAdapter;
				if (!BinaryStorageBuffer.GetSerializationAdapter(this.m_Adapters, typeof(T), out serializationAdapter))
				{
					T result = default(T);
					return result;
				}
				T t = default(T);
				try
				{
					t = (T)((object)serializationAdapter.Deserialize(this, typeof(T), id, out size));
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					T result = default(T);
					return result;
				}
				if (cacheValue && t != null && size >= this.m_MinCachedObjectSize)
				{
					this.m_Cache.TryAdd(id, t);
				}
				return t;
			}

			public object ReadObject(Type t, uint id, out uint size, bool cacheValue = true)
			{
				size = 0U;
				if (id == 4294967295U)
				{
					return null;
				}
				object result;
				if (this.TryGetCachedValue(t, id, out result))
				{
					return result;
				}
				BinaryStorageBuffer.ISerializationAdapter serializationAdapter;
				if (!BinaryStorageBuffer.GetSerializationAdapter(this.m_Adapters, t, out serializationAdapter))
				{
					return null;
				}
				object obj = null;
				try
				{
					obj = serializationAdapter.Deserialize(this, t, id, out size);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return null;
				}
				if (cacheValue && obj != null && size >= this.m_MinCachedObjectSize)
				{
					this.m_Cache.TryAdd(id, obj);
				}
				return obj;
			}

			public unsafe T ReadValue<[IsUnmanaged] T>(uint id, out uint size) where T : struct, ValueType
			{
				size = (uint)sizeof(T);
				if (id == 4294967295U)
				{
					return default(T);
				}
				if ((ulong)id >= (ulong)((long)this.m_Buffer.Length))
				{
					throw new Exception(string.Format("Data offset {0} is out of bounds of buffer with length of {1}.", id, this.m_Buffer.Length));
				}
				byte[] buffer;
				byte* ptr;
				if ((buffer = this.m_Buffer) == null || buffer.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &buffer[0];
				}
				T result;
				UnsafeUtility.MemCpy((void*)(&result), (void*)(ptr + id), (long)((ulong)size));
				return result;
			}

			public string ReadString(uint id, out uint size, char sep = '\0', bool cacheValue = true)
			{
				if (id == 4294967295U)
				{
					size = 0U;
					return null;
				}
				string result;
				if (this.TryGetCachedValue<string>(id, out result))
				{
					size = 0U;
					return result;
				}
				if (sep == '\0')
				{
					return this.ReadAutoEncodedString(id, out size, cacheValue);
				}
				return this.ReadDynamicString(id, out size, sep, cacheValue);
			}

			private unsafe string ReadStringInternal(uint offset, out uint size, Encoding enc, bool cacheValue = true)
			{
				if ((ulong)(offset - 4U) >= (ulong)((long)this.m_Buffer.Length))
				{
					throw new Exception(string.Format("Data offset {0} is out of bounds of buffer with length of {1}.", offset, this.m_Buffer.Length));
				}
				string result;
				if (this.TryGetCachedValue<string>(offset, out result))
				{
					size = 0U;
					return result;
				}
				byte[] buffer;
				byte* ptr;
				if ((buffer = this.m_Buffer) == null || buffer.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &buffer[0];
				}
				uint num = *(uint*)(ptr + (offset - 4U));
				if ((ulong)(offset + num) > (ulong)((long)this.m_Buffer.Length))
				{
					throw new Exception(string.Format("Data offset {0}, len {1} is out of bounds of buffer with length of {2}.", offset, num, this.m_Buffer.Length));
				}
				string @string = enc.GetString(ptr + offset, (int)num);
				size = num;
				if (cacheValue && size >= this.m_MinCachedObjectSize)
				{
					this.m_Cache.TryAdd(offset, @string);
				}
				return @string;
			}

			private string ReadAutoEncodedString(uint id, out uint size, bool cacheValue)
			{
				if ((id & 2147483648U) == 2147483648U)
				{
					return this.ReadStringInternal(id & 1073741823U, out size, Encoding.Unicode, cacheValue);
				}
				return this.ReadStringInternal(id, out size, Encoding.ASCII, cacheValue);
			}

			public int ComputeStringLength(uint id, char sep = '\0')
			{
				if (id == 4294967295U)
				{
					return 0;
				}
				if (sep == '\0')
				{
					return this.GetAutoEncodedStringLength(id);
				}
				return this.GetDynamicStringLength(id, sep);
			}

			private int GetDynamicStringLength(uint id, char sep)
			{
				if ((id & 1073741824U) == 1073741824U)
				{
					int num = 0;
					uint num2 = id;
					while (num2 != 4294967295U)
					{
						uint num3;
						BinaryStorageBuffer.DynamicString dynamicString = this.ReadValue<BinaryStorageBuffer.DynamicString>(num2 & 1073741823U, out num3);
						num += this.GetAutoEncodedStringLength(dynamicString.stringId);
						num2 = dynamicString.nextId;
						if (num2 != 4294967295U)
						{
							num++;
						}
					}
					return num;
				}
				return this.GetAutoEncodedStringLength(id);
			}

			private int GetAutoEncodedStringLength(uint id)
			{
				if ((id & 2147483648U) == 2147483648U)
				{
					return this.GetStringLengthInternal(id & 1073741823U, Encoding.Unicode);
				}
				return this.GetStringLengthInternal(id, Encoding.ASCII);
			}

			private unsafe int GetStringLengthInternal(uint offset, Encoding enc)
			{
				if ((ulong)(offset - 4U) >= (ulong)((long)this.m_Buffer.Length))
				{
					throw new Exception(string.Format("Data offset {0} is out of bounds of buffer with length of {1}.", offset, this.m_Buffer.Length));
				}
				byte[] buffer;
				byte* ptr;
				if ((buffer = this.m_Buffer) == null || buffer.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &buffer[0];
				}
				uint num = *(uint*)(ptr + (offset - 4U));
				if ((ulong)(offset + num) > (ulong)((long)this.m_Buffer.Length))
				{
					throw new Exception(string.Format("Data offset {0}, len {1} is out of bounds of buffer with length of {2}.", offset, num, this.m_Buffer.Length));
				}
				return enc.GetCharCount(ptr + offset, (int)num);
			}

			private unsafe string ReadDynamicString(uint id, out uint size, char sep, bool cacheValue)
			{
				if ((id & 1073741824U) != 1073741824U)
				{
					return this.ReadAutoEncodedString(id, out size, cacheValue);
				}
				size = 0U;
				string text;
				if (this.TryGetCachedValue<string>(id, out text))
				{
					return text;
				}
				int dynamicStringLength = this.GetDynamicStringLength(id, sep);
				BinaryStorageBuffer.Reader.stringCreationState.id = id;
				BinaryStorageBuffer.Reader.stringCreationState.sep = sep;
				BinaryStorageBuffer.Reader.stringCreationState.length = dynamicStringLength;
				BinaryStorageBuffer.Reader.stringCreationState.size = 0U;
				text = string.Create<BinaryStorageBuffer.Reader.StringCreationState>(dynamicStringLength, BinaryStorageBuffer.Reader.stringCreationState, delegate(Span<char> chars, BinaryStorageBuffer.Reader.StringCreationState state)
				{
					int num = state.length;
					uint num2 = state.id;
					while (num2 != 4294967295U)
					{
						uint num3;
						BinaryStorageBuffer.DynamicString dynamicString = this.ReadValue<BinaryStorageBuffer.DynamicString>(num2 & 1073741823U, out num3);
						uint num4;
						string text2 = this.ReadAutoEncodedString(dynamicString.stringId, out num4, true);
						text2.AsSpan().CopyTo(chars.Slice(num - text2.Length, text2.Length));
						state.size += num3 + num4;
						num -= text2.Length;
						num2 = dynamicString.nextId;
						if (num2 != 4294967295U)
						{
							*chars[--num] = state.sep;
						}
					}
				});
				size = BinaryStorageBuffer.Reader.stringCreationState.size;
				if (cacheValue && size >= this.m_MinCachedObjectSize)
				{
					this.m_Cache.TryAdd(id, text);
				}
				return text;
			}

			private byte[] m_Buffer;

			private Dictionary<Type, BinaryStorageBuffer.ISerializationAdapter> m_Adapters;

			private LRUCache<uint, object> m_Cache;

			private uint m_MinCachedObjectSize;

			private StringBuilder stringBuilder;

			private static BinaryStorageBuffer.Reader.StringCreationState stringCreationState = new BinaryStorageBuffer.Reader.StringCreationState();

			private class StringCreationState
			{
				public uint id;

				public char sep;

				public int length;

				public uint size;
			}
		}

		public class Writer
		{
			public uint Length
			{
				get
				{
					return this.totalBytes;
				}
			}

			public Writer(int chunkSize = 1048576, params BinaryStorageBuffer.ISerializationAdapter[] adapters)
			{
				this.defaulChunkSize = (uint)((chunkSize > 0) ? chunkSize : 1048576);
				this.existingValues = new Dictionary<Hash128, uint>();
				this.chunks = new List<BinaryStorageBuffer.Writer.Chunk>(10);
				this.chunks.Add(new BinaryStorageBuffer.Writer.Chunk
				{
					position = 0U
				});
				this.serializationAdapters = new Dictionary<Type, BinaryStorageBuffer.ISerializationAdapter>();
				BinaryStorageBuffer.AddSerializationAdapter(this.serializationAdapters, new BinaryStorageBuffer.TypeSerializer(), false);
				BinaryStorageBuffer.AddSerializationAdapter(this.serializationAdapters, new BinaryStorageBuffer.BuiltinTypesSerializer(), false);
				foreach (BinaryStorageBuffer.ISerializationAdapter adapter in adapters)
				{
					BinaryStorageBuffer.AddSerializationAdapter(this.serializationAdapters, adapter, true);
				}
			}

			private BinaryStorageBuffer.Writer.Chunk FindChunkWithSpace(uint length)
			{
				BinaryStorageBuffer.Writer.Chunk chunk = this.chunks[this.chunks.Count - 1];
				if (chunk.data == null)
				{
					chunk.data = new byte[(length > this.defaulChunkSize) ? length : this.defaulChunkSize];
				}
				if ((ulong)length > (ulong)((long)chunk.data.Length - (long)((ulong)chunk.position)))
				{
					chunk = new BinaryStorageBuffer.Writer.Chunk
					{
						position = 0U,
						data = new byte[(length > this.defaulChunkSize) ? length : this.defaulChunkSize]
					};
					this.chunks.Add(chunk);
				}
				return chunk;
			}

			private unsafe uint WriteInternal(void* pData, uint dataSize, bool prefixSize)
			{
				Hash128 key;
				BinaryStorageBuffer.ComputeHash(pData, (ulong)dataSize, &key);
				uint result;
				if (this.existingValues.TryGetValue(key, out result))
				{
					return result;
				}
				uint num = prefixSize ? (dataSize + 4U) : dataSize;
				BinaryStorageBuffer.Writer.Chunk chunk = this.FindChunkWithSpace(num);
				fixed (byte* ptr = &chunk.data[(int)chunk.position])
				{
					byte* ptr2 = ptr;
					uint num2 = this.totalBytes;
					if (prefixSize)
					{
						UnsafeUtility.MemCpy((void*)ptr2, (void*)(&dataSize), 4L);
						if (dataSize > 0U)
						{
							UnsafeUtility.MemCpy((void*)(ptr2 + 4), pData, (long)((ulong)dataSize));
						}
						num2 += 4U;
					}
					else
					{
						if (dataSize == 0U)
						{
							return uint.MaxValue;
						}
						UnsafeUtility.MemCpy((void*)ptr2, pData, (long)((ulong)dataSize));
					}
					this.totalBytes += num;
					chunk.position += num;
					this.existingValues[key] = num2;
					return num2;
				}
			}

			private uint ReserveInternal(uint dataSize, bool prefixSize)
			{
				uint num = prefixSize ? (dataSize + 4U) : dataSize;
				BinaryStorageBuffer.Writer.Chunk chunk = this.FindChunkWithSpace(num);
				this.totalBytes += num;
				chunk.position += num;
				return this.totalBytes - dataSize;
			}

			private unsafe void WriteInternal(uint id, void* pData, uint dataSize, bool prefixSize)
			{
				Hash128 key;
				BinaryStorageBuffer.ComputeHash(pData, (ulong)dataSize, &key);
				this.existingValues[key] = id;
				uint num = id;
				foreach (BinaryStorageBuffer.Writer.Chunk chunk in this.chunks)
				{
					if (num < chunk.position)
					{
						try
						{
							byte[] array;
							byte* ptr;
							if ((array = chunk.data) == null || array.Length == 0)
							{
								ptr = null;
							}
							else
							{
								ptr = &array[0];
							}
							if (prefixSize)
							{
								UnsafeUtility.MemCpy((void*)(ptr + (num - 4U)), (void*)(&dataSize), 4L);
							}
							UnsafeUtility.MemCpy((void*)(ptr + num), pData, (long)((ulong)dataSize));
							break;
						}
						finally
						{
							byte[] array = null;
						}
					}
					num -= chunk.position;
				}
			}

			public uint Reserve<[IsUnmanaged] T>() where T : struct, ValueType
			{
				return this.ReserveInternal((uint)sizeof(T), false);
			}

			public unsafe uint Write<[IsUnmanaged] T>(in T val) where T : struct, ValueType
			{
				fixed (T* ptr = &val)
				{
					T* pData = ptr;
					return this.WriteInternal((void*)pData, (uint)sizeof(T), false);
				}
			}

			public unsafe uint Write<[IsUnmanaged] T>(T val) where T : struct, ValueType
			{
				return this.WriteInternal((void*)(&val), (uint)sizeof(T), false);
			}

			public unsafe uint Write<[IsUnmanaged] T>(uint offset, in T val) where T : struct, ValueType
			{
				fixed (T* ptr = &val)
				{
					T* pData = ptr;
					this.WriteInternal(offset, (void*)pData, (uint)sizeof(T), false);
				}
				return offset;
			}

			public unsafe uint Write<[IsUnmanaged] T>(uint offset, T val) where T : struct, ValueType
			{
				this.WriteInternal(offset, (void*)(&val), (uint)sizeof(T), false);
				return offset;
			}

			public uint Reserve<[IsUnmanaged] T>(uint count) where T : struct, ValueType
			{
				return this.ReserveInternal((uint)(sizeof(T) * (int)count), true);
			}

			public unsafe uint Write<[IsUnmanaged] T>(T[] values, bool hashElements = true) where T : struct, ValueType
			{
				T* ptr;
				if (values == null || values.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &values[0];
				}
				uint num = (uint)(values.Length * sizeof(T));
				Hash128 key;
				BinaryStorageBuffer.ComputeHash((void*)ptr, (ulong)num, &key);
				uint result;
				if (this.existingValues.TryGetValue(key, out result))
				{
					return result;
				}
				BinaryStorageBuffer.Writer.Chunk chunk = this.FindChunkWithSpace(num + 4U);
				fixed (byte* ptr2 = &chunk.data[(int)chunk.position])
				{
					byte* ptr3 = ptr2;
					uint num2 = this.totalBytes + 4U;
					UnsafeUtility.MemCpy((void*)ptr3, (void*)(&num), 4L);
					UnsafeUtility.MemCpy((void*)(ptr3 + 4), (void*)ptr, (long)((ulong)num));
					uint num3 = num + 4U;
					this.totalBytes += num3;
					chunk.position += num3;
					this.existingValues[key] = num2;
					if (hashElements && sizeof(T) > 4)
					{
						for (int i = 0; i < values.Length; i++)
						{
							key = default(Hash128);
							BinaryStorageBuffer.ComputeHash((void*)(ptr + (IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (ulong)((long)sizeof(T)), &key);
							this.existingValues[key] = num2 + (uint)(i * sizeof(T));
						}
					}
					return num2;
				}
			}

			public unsafe uint Write<[IsUnmanaged] T>(uint offset, T[] values, bool hashElements = true) where T : struct, ValueType
			{
				uint num = (uint)(values.Length * sizeof(T));
				uint num2 = offset;
				fixed (T[] array = values)
				{
					T* source;
					if (values == null || array.Length == 0)
					{
						source = null;
					}
					else
					{
						source = &array[0];
					}
					foreach (BinaryStorageBuffer.Writer.Chunk chunk in this.chunks)
					{
						if (num2 < chunk.position)
						{
							try
							{
								byte[] array2;
								byte* ptr;
								if ((array2 = chunk.data) == null || array2.Length == 0)
								{
									ptr = null;
								}
								else
								{
									ptr = &array2[0];
								}
								UnsafeUtility.MemCpy((void*)(ptr + (num2 - 4U)), (void*)(&num), 4L);
								UnsafeUtility.MemCpy((void*)(ptr + num2), (void*)source, (long)((ulong)num));
								if (hashElements && sizeof(T) > 4)
								{
									for (int i = 0; i < values.Length; i++)
									{
										T t = values[i];
										Hash128 key;
										BinaryStorageBuffer.ComputeHash((void*)(&t), (ulong)((long)sizeof(T)), &key);
										this.existingValues[key] = offset + (uint)(i * sizeof(T));
									}
								}
								return offset;
							}
							finally
							{
								byte[] array2 = null;
							}
						}
						num2 -= chunk.position;
					}
				}
				return uint.MaxValue;
			}

			public uint WriteObjects<T>(IEnumerable<T> objs, bool serizalizeTypeData)
			{
				if (objs == null)
				{
					return uint.MaxValue;
				}
				uint[] array = new uint[objs.Count<T>()];
				int num = 0;
				foreach (T t in objs)
				{
					array[num++] = this.WriteObject(t, serizalizeTypeData);
				}
				return this.Write<uint>(array, true);
			}

			public uint WriteObject(object obj, bool serializeTypeData)
			{
				if (obj == null)
				{
					return uint.MaxValue;
				}
				Type type = obj.GetType();
				BinaryStorageBuffer.ISerializationAdapter serializationAdapter;
				if (!BinaryStorageBuffer.GetSerializationAdapter(this.serializationAdapters, type, out serializationAdapter))
				{
					return uint.MaxValue;
				}
				uint num = serializationAdapter.Serialize(this, obj);
				if (serializeTypeData)
				{
					num = this.Write<BinaryStorageBuffer.ObjectTypeData>(new BinaryStorageBuffer.ObjectTypeData
					{
						typeId = this.WriteObject(type, false),
						objectId = num
					});
				}
				return num;
			}

			public uint WriteString(string str, char sep = '\0')
			{
				if (str == null)
				{
					return uint.MaxValue;
				}
				if (sep != '\0')
				{
					return this.WriteDynamicString(str, sep);
				}
				return this.WriteAutoEncodedString(str);
			}

			private unsafe uint WriteStringInternal(string val, Encoding enc)
			{
				if (val == null)
				{
					return uint.MaxValue;
				}
				byte[] bytes = enc.GetBytes(val);
				byte[] array;
				byte* pData;
				if ((array = bytes) == null || array.Length == 0)
				{
					pData = null;
				}
				else
				{
					pData = &array[0];
				}
				return this.WriteInternal((void*)pData, (uint)bytes.Length, true);
			}

			public unsafe byte[] SerializeToByteArray()
			{
				byte[] array = new byte[this.totalBytes];
				byte[] array2;
				byte* ptr;
				if ((array2 = array) == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				uint num = 0U;
				foreach (BinaryStorageBuffer.Writer.Chunk chunk in this.chunks)
				{
					try
					{
						byte[] array3;
						byte* source;
						if ((array3 = chunk.data) == null || array3.Length == 0)
						{
							source = null;
						}
						else
						{
							source = &array3[0];
						}
						UnsafeUtility.MemCpy((void*)(ptr + num), (void*)source, (long)((ulong)chunk.position));
					}
					finally
					{
						byte[] array3 = null;
					}
					num += chunk.position;
				}
				array2 = null;
				return array;
			}

			public uint SerializeToStream(Stream str)
			{
				foreach (BinaryStorageBuffer.Writer.Chunk chunk in this.chunks)
				{
					str.Write(chunk.data, 0, (int)chunk.position);
				}
				return this.totalBytes;
			}

			private static bool IsUnicode(string str)
			{
				for (int i = 0; i < str.Length; i++)
				{
					if (str[i] > 'ÿ')
					{
						return true;
					}
				}
				return false;
			}

			private uint WriteAutoEncodedString(string str)
			{
				if (str == null)
				{
					return uint.MaxValue;
				}
				if (BinaryStorageBuffer.Writer.IsUnicode(str))
				{
					return this.WriteUnicodeString(str);
				}
				return this.WriteStringInternal(str, Encoding.ASCII);
			}

			private uint WriteUnicodeString(string str)
			{
				uint num = this.WriteStringInternal(str, Encoding.Unicode);
				return 2147483648U | num;
			}

			private static uint ComputeStringSize(string str, out bool isUnicode)
			{
				if (isUnicode = BinaryStorageBuffer.Writer.IsUnicode(str))
				{
					return (uint)Encoding.Unicode.GetByteCount(str);
				}
				return (uint)Encoding.ASCII.GetByteCount(str);
			}

			private uint WriteDynamicString(string str, char sep)
			{
				if (str == null)
				{
					return uint.MaxValue;
				}
				string[] array = str.Split(sep, StringSplitOptions.None);
				uint num = (uint)sizeof(BinaryStorageBuffer.DynamicString);
				BinaryStorageBuffer.Writer.StringParts[] array2 = new BinaryStorageBuffer.Writer.StringParts[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					bool isUnicode;
					uint dataSize = BinaryStorageBuffer.Writer.ComputeStringSize(array[i], out isUnicode);
					array2[i] = new BinaryStorageBuffer.Writer.StringParts
					{
						str = array[i],
						dataSize = dataSize,
						isUnicode = isUnicode
					};
				}
				if (array2.Length < 2 || (array2.Length == 2 && array2[0].dataSize + array2[1].dataSize < num))
				{
					return this.WriteAutoEncodedString(str);
				}
				return 1073741824U | this.RecurseDynamicStringParts(array2, array2.Length - 1, sep, num);
			}

			private uint RecurseDynamicStringParts(BinaryStorageBuffer.Writer.StringParts[] parts, int index, char sep, uint minSize)
			{
				while (index > 0)
				{
					uint dataSize = parts[index].dataSize;
					if (dataSize >= minSize)
					{
						break;
					}
					parts[index - 1].str = string.Format("{0}{1}{2}", parts[index - 1].str, sep, parts[index].str);
					int num = index - 1;
					parts[num].dataSize = parts[num].dataSize + (dataSize + 1U);
					int num2 = index - 1;
					parts[num2].isUnicode = (parts[num2].isUnicode | parts[index].isUnicode);
					index--;
				}
				uint stringId = parts[index].isUnicode ? this.WriteUnicodeString(parts[index].str) : this.WriteStringInternal(parts[index].str, Encoding.ASCII);
				uint nextId = (index > 0) ? this.RecurseDynamicStringParts(parts, index - 1, sep, minSize) : uint.MaxValue;
				return this.Write<BinaryStorageBuffer.DynamicString>(new BinaryStorageBuffer.DynamicString
				{
					stringId = stringId,
					nextId = nextId
				});
			}

			private uint totalBytes;

			private uint defaulChunkSize;

			private List<BinaryStorageBuffer.Writer.Chunk> chunks;

			private Dictionary<Hash128, uint> existingValues;

			private Dictionary<Type, BinaryStorageBuffer.ISerializationAdapter> serializationAdapters;

			private class Chunk
			{
				public uint position;

				public byte[] data;
			}

			private struct StringParts
			{
				public string str;

				public uint dataSize;

				public bool isUnicode;
			}
		}
	}
}
