using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fusion
{
	public static class NetworkInputUtils
	{
		private static void LoadTypes()
		{
			bool initialized = NetworkInputUtils._initialized;
			if (!initialized)
			{
				NetworkInputUtils._initialized = true;
				NetworkInputUtils._wordCount = new Dictionary<Type, int>();
				NetworkInputUtils._typeKey = new Dictionary<Type, int>();
				List<ValueTuple<Type, NetworkInputWeavedAttribute>> list = new List<ValueTuple<Type, NetworkInputWeavedAttribute>>();
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					bool flag = assembly.GetCustomAttribute<NetworkAssemblyIgnoreAttribute>() != null;
					if (!flag)
					{
						try
						{
							foreach (Type type in assembly.GetTypes())
							{
								bool flag2 = type.IsValueType && typeof(INetworkInput).IsAssignableFrom(type);
								if (flag2)
								{
									try
									{
										object[] customAttributes = type.GetCustomAttributes(typeof(NetworkInputWeavedAttribute), false);
										NetworkInputWeavedAttribute networkInputWeavedAttribute;
										bool flag3;
										if (customAttributes.Length == 1)
										{
											networkInputWeavedAttribute = (customAttributes[0] as NetworkInputWeavedAttribute);
											flag3 = (networkInputWeavedAttribute != null);
										}
										else
										{
											flag3 = false;
										}
										bool flag4 = flag3;
										if (flag4)
										{
											list.Add(new ValueTuple<Type, NetworkInputWeavedAttribute>(type, networkInputWeavedAttribute));
										}
									}
									catch (Exception error)
									{
										LogStream logException = InternalLogStreams.LogException;
										if (logException != null)
										{
											logException.Log(error);
										}
									}
								}
							}
						}
						catch
						{
						}
					}
				}
				list.Sort(delegate(ValueTuple<Type, NetworkInputWeavedAttribute> a, ValueTuple<Type, NetworkInputWeavedAttribute> b)
				{
					int num = StringComparer.Ordinal.Compare(a.Item1.AssemblyQualifiedName, b.Item1.AssemblyQualifiedName);
					bool flag5 = num == 0;
					if (flag5)
					{
						Assert.AlwaysFail("order == 0");
					}
					return num;
				});
				for (int k = 0; k < list.Count; k++)
				{
					ValueTuple<Type, NetworkInputWeavedAttribute> valueTuple = list[k];
					Type item = valueTuple.Item1;
					NetworkInputWeavedAttribute item2 = valueTuple.Item2;
					NetworkInputUtils._typeKey.Add(item, k + 1);
					NetworkInputUtils._wordCount.Add(item, item2.WordCount);
				}
			}
		}

		public static int GetMaxWordCount()
		{
			NetworkInputUtils.LoadTypes();
			bool flag = NetworkInputUtils._wordCount.Count == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int num = NetworkInputUtils._wordCount.Values.Max();
				foreach (KeyValuePair<Type, int> keyValuePair in NetworkInputUtils._wordCount)
				{
					Assert.Check<Type>(Native.SizeOf(keyValuePair.Key) <= num * 4, keyValuePair.Key);
				}
				result = num + 1;
			}
			return result;
		}

		public static int GetWordCount(Type type)
		{
			Assert.Check(typeof(INetworkInput).IsAssignableFrom(type));
			NetworkInputUtils.LoadTypes();
			int num;
			bool flag = NetworkInputUtils._wordCount.TryGetValue(type, out num);
			int result;
			if (flag)
			{
				result = num;
			}
			else
			{
				Assert.AlwaysFail(string.Format("GetWordCount for {0}", type));
				result = -1;
			}
			return result;
		}

		public static int GetTypeKey(Type type)
		{
			Assert.Check(typeof(INetworkInput).IsAssignableFrom(type));
			NetworkInputUtils.LoadTypes();
			int num;
			bool flag = NetworkInputUtils._typeKey.TryGetValue(type, out num);
			int result;
			if (flag)
			{
				result = num;
			}
			else
			{
				Assert.AlwaysFail(string.Format("GetTypeKey for {0}", type));
				result = -1;
			}
			return result;
		}

		public static Type GetType(int typeKey)
		{
			NetworkInputUtils.LoadTypes();
			foreach (KeyValuePair<Type, int> keyValuePair in NetworkInputUtils._typeKey)
			{
				bool flag = keyValuePair.Value == typeKey;
				if (flag)
				{
					return keyValuePair.Key;
				}
			}
			return null;
		}

		internal static void ResetStatics()
		{
			NetworkInputUtils._initialized = false;
			NetworkInputUtils._wordCount = null;
			NetworkInputUtils._typeKey = null;
		}

		private static bool _initialized;

		private static Dictionary<Type, int> _wordCount;

		private static Dictionary<Type, int> _typeKey;
	}
}
