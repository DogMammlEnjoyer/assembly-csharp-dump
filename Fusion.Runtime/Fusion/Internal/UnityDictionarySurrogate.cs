using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion.Internal
{
	[Serializable]
	public abstract class UnityDictionarySurrogate<[IsUnmanaged] TKeyType, [IsUnmanaged] TKeyReaderWriter, [IsUnmanaged] TValueType, [IsUnmanaged] TValueReaderWriter> : UnitySurrogateBase where TKeyType : struct, ValueType where TKeyReaderWriter : struct, ValueType, IElementReaderWriter<TKeyType> where TValueType : struct, ValueType where TValueReaderWriter : struct, ValueType, IElementReaderWriter<TValueType>
	{
		public abstract SerializableDictionary<TKeyType, TValueType> DataProperty { get; set; }

		public unsafe override void Read(int* data, int capacity)
		{
			bool flag = false;
			SerializableDictionary<TKeyType, TValueType> dataProperty = this.DataProperty;
			NetworkDictionary<TKeyType, TValueType> networkDictionary = new NetworkDictionary<TKeyType, TValueType>(data, capacity, UnityDictionarySurrogate<TKeyType, TKeyReaderWriter, TValueType, TValueReaderWriter>._keyReaderWriter, UnityDictionarySurrogate<TKeyType, TKeyReaderWriter, TValueType, TValueReaderWriter>._valReaderWriter);
			bool flag2 = networkDictionary.Count != dataProperty.Count;
			if (flag2)
			{
				flag = true;
			}
			else
			{
				foreach (KeyValuePair<TKeyType, TValueType> keyValuePair in networkDictionary)
				{
					bool flag3 = !dataProperty.ContainsKey(keyValuePair.Key);
					if (flag3)
					{
						flag = true;
						break;
					}
					dataProperty[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				dataProperty.Clear();
				foreach (KeyValuePair<TKeyType, TValueType> keyValuePair2 in networkDictionary)
				{
					dataProperty.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
			}
			dataProperty.Store();
		}

		public unsafe override void Write(int* data, int capacity)
		{
			NetworkDictionary<TKeyType, TValueType> networkDictionary = new NetworkDictionary<TKeyType, TValueType>(data, capacity, UnityDictionarySurrogate<TKeyType, TKeyReaderWriter, TValueType, TValueReaderWriter>._keyReaderWriter, UnityDictionarySurrogate<TKeyType, TKeyReaderWriter, TValueType, TValueReaderWriter>._valReaderWriter);
			networkDictionary.Clear();
			foreach (KeyValuePair<TKeyType, TValueType> keyValuePair in this.DataProperty)
			{
				networkDictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public override void Init(int capacity)
		{
			this.DataProperty = new SerializableDictionary<TKeyType, TValueType>();
		}

		private static IElementReaderWriter<TKeyType> _keyReaderWriter = Activator.CreateInstance<TKeyReaderWriter>();

		private static IElementReaderWriter<TValueType> _valReaderWriter = Activator.CreateInstance<TValueReaderWriter>();
	}
}
