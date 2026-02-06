using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class NetworkBehaviourUtils
	{
		internal static void ResetStatics()
		{
			NetworkBehaviourUtils.InvokeRpc = false;
			NetworkBehaviourUtils._metaData.Clear();
			NetworkBehaviourUtils._wordCounts.Clear();
			NetworkBehaviourUtils._invokerDelegates.Clear();
			NetworkBehaviourUtils._staticInvokers.Clear();
		}

		public static NetworkBehaviourUtils.MetaData GetMetaData(Type type)
		{
			NetworkBehaviourUtils.MetaData metaData;
			return NetworkBehaviourUtils._metaData.TryGetValue(type, out metaData) ? metaData : default(NetworkBehaviourUtils.MetaData);
		}

		public static void RegisterMetaData(Type type)
		{
			bool flag = NetworkBehaviourUtils._metaData.ContainsKey(type);
			if (!flag)
			{
				NetworkBehaviourUtils.MetaData value;
				NetworkBehaviourUtils._metaData.Add(type, value);
			}
		}

		public static int GetWordCount(NetworkBehaviour behaviour)
		{
			int? dynamicWordCount = behaviour.DynamicWordCount;
			bool flag = dynamicWordCount != null;
			int result;
			if (flag)
			{
				Assert.Check<int, LogUtils.DumpDeferredClass>(dynamicWordCount.Value >= 0, "DynamicWordCount returned a negative value {0} {1}", dynamicWordCount.Value, LogUtils.GetDump<NetworkBehaviour>(behaviour));
				result = dynamicWordCount.Value;
			}
			else
			{
				int staticWordCount = NetworkBehaviourUtils.GetStaticWordCount(behaviour.GetType());
				Assert.Check<int, LogUtils.DumpDeferredClass>(staticWordCount >= 0, "GetStaticWordCount returned a negative value {0} {1}", staticWordCount, LogUtils.GetDump<NetworkBehaviour>(behaviour));
				result = staticWordCount;
			}
			return result;
		}

		public static bool HasStaticWordCount(Type type)
		{
			Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(type));
			return ReflectionUtils.GetWeavedAttributeOrThrow(type).WordCount >= 0;
		}

		public static int GetStaticWordCount(Type type)
		{
			Assert.Check(typeof(NetworkBehaviour).IsAssignableFrom(type));
			int wordCount;
			bool flag = !NetworkBehaviourUtils._wordCounts.TryGetValue(type, out wordCount);
			if (flag)
			{
				NetworkBehaviourWeavedAttribute weavedAttributeOrThrow = ReflectionUtils.GetWeavedAttributeOrThrow(type);
				Assert.Check(weavedAttributeOrThrow.WordCount >= 0);
				NetworkBehaviourUtils._wordCounts.Add(type, wordCount = weavedAttributeOrThrow.WordCount);
			}
			return wordCount;
		}

		public static bool ShouldRegisterRpcInvokeDelegates(Type type)
		{
			return !NetworkBehaviourUtils._invokerDelegates.ContainsKey(type);
		}

		public static void RegisterRpcInvokeDelegates(Type type)
		{
			bool flag = NetworkBehaviourUtils.ShouldRegisterRpcInvokeDelegates(type);
			if (flag)
			{
				List<RpcInvokeData> list = new List<RpcInvokeData>();
				list.Add(default(RpcInvokeData));
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				foreach (MethodInfo methodInfo in methods)
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(NetworkRpcWeavedInvokerAttribute), false);
					NetworkRpcWeavedInvokerAttribute networkRpcWeavedInvokerAttribute;
					bool flag2;
					if (customAttributes.Length != 0)
					{
						networkRpcWeavedInvokerAttribute = (customAttributes[0] as NetworkRpcWeavedInvokerAttribute);
						flag2 = (networkRpcWeavedInvokerAttribute != null);
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						list.Add(new RpcInvokeData
						{
							Key = networkRpcWeavedInvokerAttribute.Key,
							Sources = networkRpcWeavedInvokerAttribute.Sources,
							Targets = networkRpcWeavedInvokerAttribute.Targets,
							Delegate = (RpcInvokeDelegate)Delegate.CreateDelegate(typeof(RpcInvokeDelegate), methodInfo)
						});
					}
					bool flag4 = methodInfo.DeclaringType == type;
					if (flag4)
					{
						object[] customAttributes2 = methodInfo.GetCustomAttributes(typeof(NetworkRpcStaticWeavedInvokerAttribute), false);
						NetworkRpcStaticWeavedInvokerAttribute networkRpcStaticWeavedInvokerAttribute;
						bool flag5;
						if (customAttributes2.Length != 0)
						{
							networkRpcStaticWeavedInvokerAttribute = (customAttributes2[0] as NetworkRpcStaticWeavedInvokerAttribute);
							flag5 = (networkRpcStaticWeavedInvokerAttribute != null);
						}
						else
						{
							flag5 = false;
						}
						bool flag6 = flag5;
						if (flag6)
						{
							NetworkBehaviourUtils._staticInvokers.Add(networkRpcStaticWeavedInvokerAttribute.Key, (RpcStaticInvokeDelegate)Delegate.CreateDelegate(typeof(RpcStaticInvokeDelegate), methodInfo));
						}
					}
				}
				list.Sort((RpcInvokeData a, RpcInvokeData b) => a.Key.CompareTo(b.Key));
				NetworkBehaviourUtils._invokerDelegates.Add(type, list.ToArray());
			}
		}

		public static bool TryGetRpcInvokeDelegateArray(Type type, out RpcInvokeData[] delegates)
		{
			return NetworkBehaviourUtils._invokerDelegates.TryGetValue(type, out delegates);
		}

		public static int GetRpcStaticIndexOrThrow(string key)
		{
			int num = NetworkBehaviourUtils._staticInvokers.IndexOfKey(key);
			bool flag = num < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("Static RPC not found: " + key);
			}
			return num;
		}

		public static bool TryGetRpcStaticInvokeDelegate(int index, out RpcStaticInvokeDelegate del)
		{
			bool flag = index >= 0 && index < NetworkBehaviourUtils._staticInvokers.Count;
			bool result;
			if (flag)
			{
				del = NetworkBehaviourUtils._staticInvokers.Values[index];
				result = true;
			}
			else
			{
				del = null;
				result = false;
			}
			return result;
		}

		public static void NotifyRpcPayloadSizeExceeded(string rpc, int size)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError != null)
			{
				logError.Log(string.Format("{0}: payload is too large ({1} bytes). Max allowed: {2} bytes)", rpc, size, 512));
			}
		}

		public static void NotifyRpcTargetUnreachable(PlayerRef player, string rpc)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError != null)
			{
				logError.Log(string.Format("{0}: target {1} not reachable.", rpc, player));
			}
		}

		public static void NotifyLocalSimulationNotAllowedToSendRpc(string rpc, NetworkObject obj, int sources)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError != null)
			{
				logError.Log(rpc + ": Local simulation is not allowed to send this RPC on " + obj.Name + ".");
			}
		}

		public static void NotifyLocalTargetedRpcCulled(PlayerRef player, string methodName)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn != null)
			{
				logWarn.Log(string.Format("{0} culled for target {1}: player is local and InvokeLocal is set to false", methodName, player));
			}
		}

		public static void ThrowIfBehaviourNotInitialized(NetworkBehaviour behaviour)
		{
			bool flag = BehaviourUtils.IsNotAlive(behaviour.Object);
			if (flag)
			{
				throw new InvalidOperationException("Behaviour not initialized: Object not set.");
			}
			bool flag2 = BehaviourUtils.IsNotAlive(behaviour.Runner);
			if (flag2)
			{
				throw new InvalidOperationException("Behaviour not initialized: Runner not set.");
			}
		}

		public static void NotifyNetworkWrapFailed<T>(T value)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn != null)
			{
				logWarn.Log(string.Format("Failed to wrap {0}", value));
			}
		}

		public static void NotifyNetworkWrapFailed<T>(T value, Type wrapperType)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn != null)
			{
				logWarn.Log(string.Format("Failed to wrap {0} as {1}", value, wrapperType));
			}
		}

		public static void NotifyNetworkUnwrapFailed<T>(T wrapper, Type valueType)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn != null)
			{
				logWarn.Log(string.Format("Failed to unwrap {0} to {1}", wrapper, valueType));
			}
		}

		public static void InitializeNetworkArray<[IsUnmanaged] T>(NetworkArray<T> networkArray, T[] sourceArray, string name) where T : struct, ValueType
		{
			int num = (sourceArray != null) ? sourceArray.Length : 0;
			bool flag = num == 0;
			if (!flag)
			{
				bool flag2 = networkArray.Length < num;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("Source array is too long for {0} with capacity of {1}: {2}. Ignoring extra elements.", name, networkArray.Length, num));
					}
					num = networkArray.Length;
				}
				networkArray.CopyFrom(sourceArray, 0, num);
			}
		}

		public static void CopyFromNetworkArray<[IsUnmanaged] T>(NetworkArray<T> networkArray, ref T[] dstArray) where T : struct, ValueType
		{
			T[] array = dstArray;
			int? num = (array != null) ? new int?(array.Length) : null;
			int length = networkArray.Length;
			bool flag = !(num.GetValueOrDefault() == length & num != null);
			if (flag)
			{
				dstArray = new T[networkArray.Length];
			}
			networkArray.CopyTo(dstArray, true);
		}

		public static T[] CloneArray<T>(T[] array)
		{
			bool flag = array == null;
			T[] result;
			if (flag)
			{
				result = Array.Empty<T>();
			}
			else
			{
				T[] array2 = new T[array.Length];
				Array.Copy(array, array2, array.Length);
				result = array2;
			}
			return result;
		}

		public static void InitializeNetworkList<[IsUnmanaged] T>(NetworkLinkedList<T> networkList, T[] sourceArray, string name) where T : struct, ValueType
		{
			int num = (sourceArray != null) ? sourceArray.Length : 0;
			bool flag = num == 0;
			if (!flag)
			{
				bool flag2 = networkList.Capacity < num;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("Source array is too long for {0} with capacity of {1}: {2}. Ignoring extra elements.", name, networkList.Capacity, num));
					}
					num = networkList.Capacity;
				}
				networkList.Clear();
				for (int i = 0; i < num; i++)
				{
					networkList.Add(sourceArray[i]);
				}
			}
		}

		public static void CopyFromNetworkList<[IsUnmanaged] T>(NetworkLinkedList<T> networkList, ref T[] dstArray) where T : struct, ValueType
		{
			T[] array = dstArray;
			int? num = (array != null) ? new int?(array.Length) : null;
			int count = networkList.Count;
			bool flag = !(num.GetValueOrDefault() == count & num != null);
			if (flag)
			{
				dstArray = new T[networkList.Count];
			}
			int num2 = 0;
			foreach (T t in networkList)
			{
				dstArray[num2++] = t;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InternalOnDestroy(SimulationBehaviour obj)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(obj, "OnDestroy");
			}
			obj.Flags |= SimulationBehaviourRuntimeFlags.IsUnityDestroyed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InternalOnEnable(SimulationBehaviour obj)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(obj, "OnEnable");
			}
			obj.Flags &= ~SimulationBehaviourRuntimeFlags.IsUnityDisabled;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InternalOnDisable(SimulationBehaviour obj)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(obj, "OnDisable");
			}
			obj.Flags |= SimulationBehaviourRuntimeFlags.IsUnityDisabled;
		}

		public static void InitializeNetworkDictionary<D, [IsUnmanaged] K, [IsUnmanaged] V>(NetworkDictionary<K, V> networkDictionary, D dictionary, string name) where D : IDictionary<K, V> where K : struct, ValueType where V : struct, ValueType
		{
			int num = (dictionary != null) ? dictionary.Count : 0;
			bool flag = num == 0;
			if (!flag)
			{
				bool flag2 = num > networkDictionary.Capacity;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("Source dictionary is too long for {0} with capacity of {1}: {2}. Ignoring extra elements.", name, networkDictionary.Capacity, num));
					}
					num = networkDictionary.Capacity;
				}
				networkDictionary.Clear();
				foreach (KeyValuePair<K, V> keyValuePair in dictionary)
				{
					bool flag3 = --num < 0;
					if (flag3)
					{
						break;
					}
					networkDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public static void CopyFromNetworkDictionary<D, [IsUnmanaged] K, [IsUnmanaged] V>(NetworkDictionary<K, V> networkDictionary, ref D dictionary) where D : IDictionary<K, V>, new() where K : struct, ValueType where V : struct, ValueType
		{
			bool flag = dictionary == null;
			if (flag)
			{
				dictionary = Activator.CreateInstance<D>();
			}
			else
			{
				dictionary.Clear();
			}
			foreach (KeyValuePair<K, V> keyValuePair in networkDictionary)
			{
				ref D ptr = ref dictionary;
				if (default(D) == null)
				{
					D d = dictionary;
					ptr = ref d;
				}
				ptr.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public static SerializableDictionary<K, V> MakeSerializableDictionary<[IsUnmanaged] K, [IsUnmanaged] V>(Dictionary<K, V> dictionary) where K : struct, ValueType where V : struct, ValueType
		{
			return SerializableDictionary<K, V>.Wrap(dictionary);
		}

		private static Dictionary<Type, int> _wordCounts = new Dictionary<Type, int>();

		private static Dictionary<Type, RpcInvokeData[]> _invokerDelegates = new Dictionary<Type, RpcInvokeData[]>();

		private static SortedList<string, RpcStaticInvokeDelegate> _staticInvokers = new SortedList<string, RpcStaticInvokeDelegate>();

		private static Dictionary<Type, NetworkBehaviourUtils.MetaData> _metaData = new Dictionary<Type, NetworkBehaviourUtils.MetaData>();

		public static bool InvokeRpc = false;

		public struct MetaData
		{
		}

		public struct ArrayInitializer<T>
		{
			public static implicit operator NetworkArray<T>(NetworkBehaviourUtils.ArrayInitializer<T> arr)
			{
				throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
			}

			public static implicit operator NetworkLinkedList<T>(NetworkBehaviourUtils.ArrayInitializer<T> arr)
			{
				throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
			}
		}

		public struct DictionaryInitializer<K, V>
		{
			public static implicit operator NetworkDictionary<K, V>(NetworkBehaviourUtils.DictionaryInitializer<K, V> arr)
			{
				throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
			}
		}
	}
}
