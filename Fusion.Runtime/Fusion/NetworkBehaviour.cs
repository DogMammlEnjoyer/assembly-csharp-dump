using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace Fusion
{
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Blue)]
	[HelpURL("https://doc.photonengine.com/fusion/current/manual/network-object#networkbehaviour")]
	public abstract class NetworkBehaviour : SimulationBehaviour, ISpawned, IPublicFacingInterface, IDespawned, IElementReaderWriter<NetworkObject>, IElementReaderWriter<NetworkBehaviour>
	{
		public bool StateBufferIsValid
		{
			get
			{
				return this.Ptr != null;
			}
		}

		public NetworkBehaviourBuffer StateBuffer
		{
			get
			{
				return new NetworkBehaviourBuffer(base.Runner.Simulation.Tick, this.Ptr, this.WordCount);
			}
		}

		[TupleElementNames(new string[]
		{
			"offset",
			"count"
		})]
		public ValueTuple<int, int> WordInfo
		{
			[return: TupleElementNames(new string[]
			{
				"offset",
				"count"
			})]
			get
			{
				return new ValueTuple<int, int>(this.WordOffset, this.WordCount);
			}
		}

		public unsafe Tick ChangedTick
		{
			get
			{
				return *base.Object.BehaviourChangedTickArray[this.ObjectIndex];
			}
		}

		public NetworkBehaviourId Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new NetworkBehaviourId
				{
					Object = (BehaviourUtils.IsNotNull(base.Object) ? base.Object.Id : default(NetworkId)),
					Behaviour = this.ObjectIndex
				};
			}
		}

		public bool HasInputAuthority
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return BehaviourUtils.IsAlive(base.Object) && base.Object.HasInputAuthority;
			}
		}

		public bool HasStateAuthority
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return BehaviourUtils.IsAlive(base.Object) && base.Object.HasStateAuthority;
			}
		}

		public bool IsProxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return BehaviourUtils.IsAlive(base.Object) && base.Object.IsProxy;
			}
		}

		public virtual int? DynamicWordCount
		{
			get
			{
				return null;
			}
		}

		internal bool IsEditorWritable
		{
			get
			{
				bool flag = BehaviourUtils.IsNotAlive(base.Object);
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool flag2 = !base.Object.IsValid;
					if (flag2)
					{
						result = true;
					}
					else
					{
						bool hasStateAuthority = base.Object.HasStateAuthority;
						result = hasStateAuthority;
					}
				}
				return result;
			}
		}

		public int GetLocalAuthorityMask()
		{
			bool flag = BehaviourUtils.IsNotAlive(base.Runner);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = AuthorityMasks.Create(this.HasStateAuthority, this.HasInputAuthority);
			}
			return result;
		}

		public void ReplicateTo(PlayerRef player, bool replicate)
		{
			base.Runner.SetBehaviourReplicateTo(this, player, replicate);
		}

		public void ReplicateToAll(bool replicate)
		{
			base.Runner.SetBehaviourReplicateToAll(this, replicate);
		}

		public unsafe void CopyStateFrom(NetworkBehaviour source)
		{
			Assert.Check(BehaviourUtils.IsAlive(base.Object));
			Assert.Check(source);
			bool flag = base.GetType() == source.GetType();
			if (flag)
			{
				Native.MemCpy((void*)this.Ptr, (void*)source.Ptr, this.WordCount * 4);
			}
		}

		public override void FixedUpdateNetwork()
		{
		}

		public unsafe void ResetState()
		{
			Assert.Check(BehaviourUtils.IsAlive(base.Object));
			Native.MemClear((void*)this.Ptr, this.WordCount * 4);
			this.CopyBackingFieldsToState(false);
		}

		public virtual void CopyBackingFieldsToState(bool firstTime)
		{
		}

		public virtual void CopyStateToBackingFields()
		{
		}

		internal override void PreRender()
		{
			NetworkBehaviour.ChangeDetector onRenderCallbacksDetector = this._onRenderCallbacksDetector;
			if (onRenderCallbacksDetector != null)
			{
				onRenderCallbacksDetector.DetectChanges(this, true);
			}
		}

		internal void PreSpawned()
		{
			bool flag = NetworkBehaviour.ChangeDetector.HasChangeCallbacks(base.GetType());
			if (flag)
			{
				this._onRenderCallbacksDetector = this.GetChangeDetector(NetworkBehaviour.ChangeDetector.Source.SnapshotFrom, true);
				this._onRenderCallbacksDetector.InvokeCallbacks = true;
			}
		}

		public virtual void Spawned()
		{
		}

		public virtual void Despawned(NetworkRunner runner, bool hasState)
		{
		}

		public unsafe ref T ReinterpretState<[IsUnmanaged] T>(int offset = 0) where T : struct, ValueType
		{
			Assert.Check(this.StateBufferIsValid);
			Assert.Check(offset < this.WordCount);
			Assert.Check(offset + Native.WordCount(sizeof(T), 4) <= this.WordCount);
			return ref *(T*)(this.Ptr + offset);
		}

		public NetworkBehaviour.BehaviourReader<T> GetBehaviourReader<T>(string property) where T : NetworkBehaviour
		{
			return NetworkBehaviour.GetBehaviourReader<T>(base.Runner, base.GetType(), property);
		}

		public NetworkBehaviour.ArrayReader<T> GetArrayReader<T>(string property)
		{
			return NetworkBehaviour.GetArrayReader<T>(base.GetType(), property, this as IElementReaderWriter<T>);
		}

		public NetworkBehaviour.LinkListReader<T> GetLinkListReader<T>(string property)
		{
			return NetworkBehaviour.GetLinkListReader<T>(base.GetType(), property, this as IElementReaderWriter<T>);
		}

		public NetworkBehaviour.DictionaryReader<K, V> GetDictionaryReader<K, V>(string property)
		{
			return NetworkBehaviour.GetDictionaryReader<K, V>(base.GetType(), property, this as IElementReaderWriter<K>, this as IElementReaderWriter<V>);
		}

		public static NetworkBehaviour.BehaviourReader<T> GetBehaviourReader<T>(NetworkRunner runner, Type behaviourType, string property) where T : NetworkBehaviour
		{
			return new NetworkBehaviour.BehaviourReader<T>
			{
				Runner = runner,
				Reader = NetworkBehaviour.GetPropertyReader<NetworkBehaviourId>(behaviourType, property)
			};
		}

		public static NetworkBehaviour.BehaviourReader<TProperty> GetBehaviourReader<TBehaviour, TProperty>(NetworkRunner runner, string property) where TBehaviour : NetworkBehaviour where TProperty : NetworkBehaviour
		{
			return new NetworkBehaviour.BehaviourReader<TProperty>
			{
				Runner = runner,
				Reader = NetworkBehaviour.GetPropertyReader<NetworkBehaviourId>(typeof(TBehaviour), property)
			};
		}

		public static NetworkBehaviour.PropertyReader<TProperty> GetPropertyReader<TBehaviour, [IsUnmanaged] TProperty>(string property) where TBehaviour : NetworkBehaviour where TProperty : struct, ValueType
		{
			return NetworkBehaviour.GetPropertyReader<TProperty>(typeof(TBehaviour), property);
		}

		public static NetworkBehaviour.PropertyReader<T> GetPropertyReader<[IsUnmanaged] T>(Type behaviourType, string property) where T : struct, ValueType
		{
			return NetworkBehaviour.GetPropertyReader<T>(NetworkBehaviour.GetReadersForType(behaviourType), property);
		}

		public static NetworkBehaviour.ArrayReader<T> GetArrayReader<T>(Type behaviourType, string property, IElementReaderWriter<T> readerWriter = null)
		{
			return new NetworkBehaviour.ArrayReader<T>
			{
				Data = NetworkBehaviour.GetPropertyReaderData(NetworkBehaviour.GetReadersForType(behaviourType), property, typeof(NetworkArray<T>)),
				ReaderWriter = readerWriter
			};
		}

		public static NetworkBehaviour.DictionaryReader<K, V> GetDictionaryReader<K, V>(Type behaviourType, string property, IElementReaderWriter<K> keyReaderWriter = null, IElementReaderWriter<V> valueReaderWriter = null)
		{
			return new NetworkBehaviour.DictionaryReader<K, V>
			{
				Data = NetworkBehaviour.GetPropertyReaderData(NetworkBehaviour.GetReadersForType(behaviourType), property, typeof(NetworkDictionary<K, V>)),
				KeyReaderWriter = keyReaderWriter,
				ValueReaderWriter = valueReaderWriter
			};
		}

		public static NetworkBehaviour.LinkListReader<T> GetLinkListReader<T>(Type behaviourType, string property, IElementReaderWriter<T> readerWriter = null)
		{
			return new NetworkBehaviour.LinkListReader<T>
			{
				Data = NetworkBehaviour.GetPropertyReaderData(NetworkBehaviour.GetReadersForType(behaviourType), property, typeof(NetworkLinkedList<T>)),
				ReaderWriter = readerWriter
			};
		}

		public NetworkBehaviour.PropertyReader<T> GetPropertyReader<[IsUnmanaged] T>(string property) where T : struct, ValueType
		{
			bool flag = this._readersForType == null;
			if (flag)
			{
				this._readersForType = NetworkBehaviour.GetReadersForType(base.GetType());
			}
			return NetworkBehaviour.GetPropertyReader<T>(this._readersForType, property);
		}

		private static NetworkBehaviour.ReadersForType GetReadersForType(Type type)
		{
			bool flag = NetworkBehaviour._readersByType == null;
			if (flag)
			{
				NetworkBehaviour._readersByType = new Dictionary<Type, NetworkBehaviour.ReadersForType>();
			}
			NetworkBehaviour.ReadersForType readersForType;
			bool flag2 = !NetworkBehaviour._readersByType.TryGetValue(type, out readersForType);
			if (flag2)
			{
				NetworkBehaviour._readersByType.Add(type, readersForType = new NetworkBehaviour.ReadersForType());
				readersForType.Properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			return readersForType;
		}

		private static bool IsArray(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NetworkArray<>);
		}

		private static bool IsList(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NetworkLinkedList<>);
		}

		private static bool IsDict(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NetworkDictionary<, >);
		}

		private static NetworkBehaviour.PropertyReader<T> GetPropertyReader<[IsUnmanaged] T>(NetworkBehaviour.ReadersForType readersForType, string property) where T : struct, ValueType
		{
			return new NetworkBehaviour.PropertyReader<T>(NetworkBehaviour.GetPropertyReaderData(readersForType, property, typeof(T)));
		}

		private static NetworkBehaviour.PropertyReaderData GetPropertyReaderData(NetworkBehaviour.ReadersForType readersForType, string property, Type typeExpected)
		{
			NetworkBehaviour.PropertyReaderData propertyReaderData;
			bool flag = !readersForType.PropertyReaders.TryGetValue(property, out propertyReaderData);
			if (flag)
			{
				PropertyInfo propertyInfo = null;
				for (int i = 0; i < readersForType.Properties.Length; i++)
				{
					bool flag2 = readersForType.Properties[i].Name == property;
					if (flag2)
					{
						propertyInfo = readersForType.Properties[i];
						break;
					}
				}
				bool flag3 = propertyInfo == null;
				if (flag3)
				{
					throw new KeyNotFoundException("Property with name " + property + " does not exist");
				}
				NetworkedWeavedAttribute customAttribute = propertyInfo.GetCustomAttribute<NetworkedWeavedAttribute>();
				bool flag4 = customAttribute == null;
				if (flag4)
				{
					throw new InvalidOperationException("Property with name " + property + " did not have the [NetworkWeaved] attribute");
				}
				Type type = propertyInfo.PropertyType;
				bool flag5 = type.IsPointer || type.IsByRef;
				if (flag5)
				{
					type = type.GetElementType();
				}
				bool flag6 = typeof(NetworkBehaviour).IsAssignableFrom(type);
				if (flag6)
				{
					type = typeof(NetworkBehaviourId);
				}
				else
				{
					bool flag7 = typeof(NetworkObject).IsAssignableFrom(type);
					if (flag7)
					{
						type = typeof(NetworkId);
					}
				}
				bool flag8 = type != typeExpected;
				if (flag8)
				{
					throw new InvalidOperationException(string.Format("Property with name {0} has a type of {1} but reader was request for type {2}", property, propertyInfo.PropertyType, typeExpected));
				}
				propertyReaderData = new NetworkBehaviour.PropertyReaderData();
				propertyReaderData.Offset = customAttribute.WordOffset;
				bool flag9 = NetworkBehaviour.IsArray(type);
				if (flag9)
				{
					NetworkedWeavedArrayAttribute customAttributeOrThrow = propertyInfo.GetCustomAttributeOrThrow(false);
					propertyReaderData.Capacity = customAttributeOrThrow.Capacity;
					propertyReaderData.ValueReaderWriterType = customAttributeOrThrow.ElementReaderWriterType;
				}
				else
				{
					bool flag10 = NetworkBehaviour.IsList(type);
					if (flag10)
					{
						NetworkedWeavedLinkedListAttribute customAttributeOrThrow2 = propertyInfo.GetCustomAttributeOrThrow(false);
						propertyReaderData.Capacity = customAttributeOrThrow2.Capacity;
						propertyReaderData.ValueReaderWriterType = customAttributeOrThrow2.ElementReaderWriterType;
					}
					else
					{
						bool flag11 = NetworkBehaviour.IsDict(type);
						if (flag11)
						{
							NetworkedWeavedDictionaryAttribute customAttributeOrThrow3 = propertyInfo.GetCustomAttributeOrThrow(false);
							propertyReaderData.Capacity = customAttributeOrThrow3.Capacity;
							propertyReaderData.KeyReaderWriterType = customAttributeOrThrow3.KeyReaderWriterType;
							propertyReaderData.ValueReaderWriterType = customAttributeOrThrow3.ValueReaderWriterType;
						}
					}
				}
				readersForType.PropertyReaders.Add(property, propertyReaderData);
			}
			return propertyReaderData;
		}

		public NetworkBehaviour.ChangeDetector GetChangeDetector(NetworkBehaviour.ChangeDetector.Source source, bool copyInitial = true)
		{
			NetworkBehaviour.ChangeDetector changeDetector = new NetworkBehaviour.ChangeDetector();
			changeDetector.Init(this, source, copyInitial);
			return changeDetector;
		}

		public bool TryGetSnapshotsBuffers(out NetworkBehaviourBuffer from, out NetworkBehaviourBuffer to, out float alpha)
		{
			RenderTimeline.GetRenderBuffers(this, out from, out to, out alpha);
			return from.Valid && to.Valid;
		}

		[Obsolete("Not called anymore, used ReplicateTo(PlayerRef, bool) instead")]
		protected virtual bool ReplicateTo(PlayerRef player)
		{
			return true;
		}

		public unsafe T? GetInput<[IsUnmanaged] T>() where T : struct, ValueType, INetworkInput
		{
			bool flag;
			if (BehaviourUtils.IsAlive(base.Object))
			{
				Simulation simulation = base.Object.Simulation;
				flag = (simulation != null && simulation.PlayerValid(*base.Object.Meta.InputAuthority));
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			T? result;
			if (flag2)
			{
				result = base.Object.Runner.GetInputForPlayer<T>(*base.Object.Meta.InputAuthority);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public unsafe bool GetInput<[IsUnmanaged] T>(out T input) where T : struct, ValueType, INetworkInput
		{
			bool flag;
			if (BehaviourUtils.IsAlive(base.Object))
			{
				Simulation simulation = base.Object.Simulation;
				flag = (simulation != null && simulation.PlayerValid(*base.Object.Meta.InputAuthority));
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = base.Object.Runner.TryGetInputForPlayer<T>(*base.Object.Meta.InputAuthority, out input);
			}
			else
			{
				input = default(T);
				result = false;
			}
			return result;
		}

		[Obsolete("Use NetworkWrap(NetworkBehaviour) instead", true)]
		public unsafe static int NetworkSerialize(NetworkRunner runner, NetworkBehaviour obj, byte* data)
		{
			throw new NotImplementedException();
		}

		[Obsolete("Use NetworkUnwrap(NetworkRunner, NetworkBehaviourId) instead", true)]
		public unsafe static int NetworkDeserialize(NetworkRunner runner, byte* data, ref NetworkBehaviour result)
		{
			throw new NotImplementedException();
		}

		[Obsolete("Use NetworkWrap(NetworkBehaviour) instead")]
		public static NetworkBehaviourId NetworkWrap(NetworkRunner runner, NetworkBehaviour obj)
		{
			return NetworkBehaviour.NetworkWrap(obj);
		}

		[NetworkSerializeMethod]
		public static NetworkBehaviourId NetworkWrap(NetworkBehaviour obj)
		{
			bool flag = BehaviourUtils.IsNotAlive(obj);
			NetworkBehaviourId result;
			if (flag)
			{
				result = default(NetworkBehaviourId);
			}
			else
			{
				bool flag2 = BehaviourUtils.IsNotAlive(obj.Object);
				if (flag2)
				{
					result = default(NetworkBehaviourId);
				}
				else
				{
					result = new NetworkBehaviourId
					{
						Object = obj.Object.Id,
						Behaviour = obj.ObjectIndex
					};
				}
			}
			return result;
		}

		[NetworkDeserializeMethod]
		public static NetworkBehaviour NetworkUnwrap(NetworkRunner runner, NetworkBehaviourId wrapper)
		{
			bool flag = !wrapper.IsValid;
			NetworkBehaviour result;
			if (flag)
			{
				result = null;
			}
			else
			{
				NetworkBehaviour networkBehaviour;
				bool flag2 = !runner.TryFindBehaviour(wrapper, out networkBehaviour);
				if (flag2)
				{
					NetworkBehaviourUtils.NotifyNetworkUnwrapFailed<NetworkBehaviourId>(wrapper, typeof(NetworkBehaviour));
				}
				result = networkBehaviour;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator NetworkBehaviourId(NetworkBehaviour behaviour)
		{
			return BehaviourUtils.IsAlive(behaviour.Runner) ? NetworkBehaviour.NetworkWrap(behaviour) : default(NetworkBehaviourId);
		}

		protected internal static void InvokeWeavedCode()
		{
		}

		public static ref T MakeRef<[IsUnmanaged] T>() where T : struct, ValueType
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		public static ref T MakeRef<[IsUnmanaged] T>(T defaultValue) where T : struct, ValueType
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		public unsafe static T* MakePtr<[IsUnmanaged] T>() where T : struct, ValueType
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		public unsafe static T* MakePtr<[IsUnmanaged] T>(T defaultValue) where T : struct, ValueType
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		public static NetworkBehaviourUtils.ArrayInitializer<T> MakeInitializer<T>(T[] array)
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		public static NetworkBehaviourUtils.DictionaryInitializer<K, V> MakeInitializer<K, V>(Dictionary<K, V> dictionary)
		{
			throw new NotImplementedException("This is a special method that is meant to be used only for [Networked] properties inline initialization.");
		}

		internal void MakeOwned(NetworkRunner runner, NetworkObject obj, int index)
		{
			base.MakeOwned(runner, obj);
			this.ObjectIndex = index;
		}

		internal new void MakeUnowned()
		{
			base.MakeUnowned();
			this.ObjectIndex = 0;
		}

		int IElementReaderWriter<NetworkBehaviour>.GetElementHashCode(NetworkBehaviour element)
		{
			return NetworkBehaviour.NetworkWrap(element).GetHashCode();
		}

		int IElementReaderWriter<NetworkBehaviour>.GetElementWordCount()
		{
			return 2;
		}

		unsafe NetworkBehaviour IElementReaderWriter<NetworkBehaviour>.Read(byte* data, int index)
		{
			return NetworkBehaviour.NetworkUnwrap(base.Runner, *(NetworkBehaviourId*)(data + index * 8));
		}

		unsafe ref NetworkBehaviour IElementReaderWriter<NetworkBehaviour>.ReadRef(byte* data, int index)
		{
			throw new NotSupportedException("Only supported for trivially copyable types. Fusion.NetworkBehaviour is not trivially copyable.");
		}

		unsafe void IElementReaderWriter<NetworkBehaviour>.Write(byte* data, int index, NetworkBehaviour element)
		{
			*(NetworkBehaviourId*)(data + index * 8) = NetworkBehaviour.NetworkWrap(element);
		}

		int IElementReaderWriter<NetworkObject>.GetElementWordCount()
		{
			return 1;
		}

		int IElementReaderWriter<NetworkObject>.GetElementHashCode(NetworkObject element)
		{
			return NetworkObject.NetworkWrap(element).GetHashCode();
		}

		unsafe NetworkObject IElementReaderWriter<NetworkObject>.Read(byte* data, int index)
		{
			NetworkObject result = null;
			NetworkObject.NetworkUnwrap(base.Runner, *(NetworkId*)(data + index * 4), ref result);
			return result;
		}

		unsafe ref NetworkObject IElementReaderWriter<NetworkObject>.ReadRef(byte* data, int index)
		{
			throw new NotSupportedException("Only supported for trivially copyable types. Fusion.NetworkObject is not trivially copyable.");
		}

		unsafe void IElementReaderWriter<NetworkObject>.Write(byte* data, int index, NetworkObject element)
		{
			*(NetworkId*)(data + index * 4) = NetworkObject.NetworkWrap(element);
		}

		private static Dictionary<Type, NetworkBehaviour.ReadersForType> _readersByType;

		private NetworkBehaviour.ReadersForType _readersForType;

		[Preserve]
		internal unsafe int* Ptr;

		internal bool InvokeRpc;

		internal RpcInvokeData[] RpcCache;

		internal int ObjectIndex;

		internal int WordOffset;

		internal int WordCount;

		internal bool DefaultReplicated = true;

		private NetworkBehaviour.ChangeDetector _onRenderCallbacksDetector;

		public struct ArrayReader<T>
		{
			public unsafe NetworkArrayReadOnly<T> Read(NetworkBehaviourBuffer first)
			{
				return new NetworkArrayReadOnly<T>((byte*)(first._ptr + this.Data.Offset), this.Data.Capacity, this.ReaderWriter ?? ReaderWriterCache.Get<T>(this.Data.ValueReaderWriterType));
			}

			internal NetworkBehaviour.PropertyReaderData Data;

			internal IElementReaderWriter<T> ReaderWriter;
		}

		public struct LinkListReader<T>
		{
			public unsafe NetworkLinkedListReadOnly<T> Read(NetworkBehaviourBuffer first)
			{
				return new NetworkLinkedListReadOnly<T>((byte*)(first._ptr + this.Data.Offset), this.Data.Capacity, this.ReaderWriter ?? ReaderWriterCache.Get<T>(this.Data.ValueReaderWriterType));
			}

			internal NetworkBehaviour.PropertyReaderData Data;

			internal IElementReaderWriter<T> ReaderWriter;
		}

		public struct DictionaryReader<K, V>
		{
			public NetworkDictionaryReadOnly<K, V> Read(NetworkBehaviourBuffer first)
			{
				return new NetworkDictionaryReadOnly<K, V>(first._ptr + this.Data.Offset, this.Data.Capacity, this.KeyReaderWriter ?? ReaderWriterCache.Get<K>(this.Data.KeyReaderWriterType), this.ValueReaderWriter ?? ReaderWriterCache.Get<V>(this.Data.ValueReaderWriterType));
			}

			internal NetworkBehaviour.PropertyReaderData Data;

			internal IElementReaderWriter<K> KeyReaderWriter;

			internal IElementReaderWriter<V> ValueReaderWriter;
		}

		public struct BehaviourReader<T> where T : NetworkBehaviour
		{
			public T Read(NetworkBehaviourBuffer first)
			{
				NetworkBehaviour networkBehaviour;
				return this.Runner.TryFindBehaviour(this.Reader.Read(first), out networkBehaviour) ? (networkBehaviour as T) : default(T);
			}

			public ValueTuple<T, T> Read(NetworkBehaviourBuffer first, NetworkBehaviourBuffer second)
			{
				return new ValueTuple<T, T>(this.Read(first), this.Read(second));
			}

			internal NetworkRunner Runner;

			internal NetworkBehaviour.PropertyReader<NetworkBehaviourId> Reader;
		}

		[NullableContext(1)]
		[Nullable(0)]
		internal class PropertyReaderData : IEquatable<NetworkBehaviour.PropertyReaderData>
		{
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(NetworkBehaviour.PropertyReaderData);
				}
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("PropertyReaderData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("Offset = ");
				builder.Append(this.Offset.ToString());
				builder.Append(", Capacity = ");
				builder.Append(this.Capacity.ToString());
				builder.Append(", KeyReaderWriterType = ");
				builder.Append(this.KeyReaderWriterType);
				builder.Append(", ValueReaderWriterType = ");
				builder.Append(this.ValueReaderWriterType);
				return true;
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(NetworkBehaviour.PropertyReaderData left, NetworkBehaviour.PropertyReaderData right)
			{
				return !(left == right);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(NetworkBehaviour.PropertyReaderData left, NetworkBehaviour.PropertyReaderData right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Offset)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Capacity)) * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(this.KeyReaderWriterType)) * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(this.ValueReaderWriterType);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as NetworkBehaviour.PropertyReaderData);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(NetworkBehaviour.PropertyReaderData other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(this.Offset, other.Offset) && EqualityComparer<int>.Default.Equals(this.Capacity, other.Capacity) && EqualityComparer<Type>.Default.Equals(this.KeyReaderWriterType, other.KeyReaderWriterType) && EqualityComparer<Type>.Default.Equals(this.ValueReaderWriterType, other.ValueReaderWriterType));
			}

			[CompilerGenerated]
			protected PropertyReaderData(NetworkBehaviour.PropertyReaderData original)
			{
				this.Offset = original.Offset;
				this.Capacity = original.Capacity;
				this.KeyReaderWriterType = original.KeyReaderWriterType;
				this.ValueReaderWriterType = original.ValueReaderWriterType;
			}

			public PropertyReaderData()
			{
			}

			public int Offset;

			public int Capacity;

			[Nullable(0)]
			public Type KeyReaderWriterType;

			[Nullable(0)]
			public Type ValueReaderWriterType;
		}

		public struct PropertyReader<[IsUnmanaged] T> where T : struct, ValueType
		{
			internal PropertyReader(NetworkBehaviour.PropertyReaderData data)
			{
				this.Data = data;
			}

			public PropertyReader(int offset)
			{
				this.Data = null;
				this.Data.Offset = offset;
			}

			public T Read(NetworkBehaviourBuffer first)
			{
				return first.Read<T>(this);
			}

			public ValueTuple<T, T> Read(NetworkBehaviourBuffer first, NetworkBehaviourBuffer second)
			{
				return new ValueTuple<T, T>(first.Read<T>(this), second.Read<T>(this));
			}

			internal NetworkBehaviour.PropertyReaderData Data;
		}

		private class ReadersForType
		{
			public ReadersForType()
			{
				this.PropertyReaders = new Dictionary<string, NetworkBehaviour.PropertyReaderData>();
			}

			public PropertyInfo[] Properties;

			public Dictionary<string, NetworkBehaviour.PropertyReaderData> PropertyReaders;
		}

		public class ChangeDetector
		{
			internal static bool HasChangeCallbacks(Type type)
			{
				bool flag = !NetworkBehaviour.ChangeDetector._propertyMappings.ContainsKey(type);
				if (flag)
				{
					NetworkBehaviour.ChangeDetector.GetPropertyMappping(type);
				}
				bool flag2;
				return NetworkBehaviour.ChangeDetector._hasChangeCallbacks.TryGetValue(type, out flag2) && flag2;
			}

			private static NetworkBehaviour.ChangeDetector.OnChangedPrevCallbackWrapper GetWrapperPrev<T>(MethodInfo methodInfo) where T : NetworkBehaviour
			{
				NetworkBehaviour.ChangeDetector.OnChangedPrevCallback<T> callback = (NetworkBehaviour.ChangeDetector.OnChangedPrevCallback<T>)Delegate.CreateDelegate(typeof(NetworkBehaviour.ChangeDetector.OnChangedPrevCallback<T>), null, methodInfo);
				return delegate(NetworkBehaviour behaviour, NetworkBehaviourBuffer prev)
				{
					callback((T)((object)behaviour), prev);
				};
			}

			private static NetworkBehaviour.ChangeDetector.OnChangedCallbackWrapper GetWrapper<T>(MethodInfo methodInfo) where T : NetworkBehaviour
			{
				NetworkBehaviour.ChangeDetector.OnChangedCallback<T> callback = (NetworkBehaviour.ChangeDetector.OnChangedCallback<T>)Delegate.CreateDelegate(typeof(NetworkBehaviour.ChangeDetector.OnChangedCallback<T>), null, methodInfo);
				return delegate(NetworkBehaviour behaviour)
				{
					callback((T)((object)behaviour));
				};
			}

			private static NetworkBehaviour.ChangeDetector.PropertyData[] GetPropertyMappping(Type type)
			{
				bool flag = NetworkBehaviour.ChangeDetector._propertyMappings == null;
				if (flag)
				{
					NetworkBehaviour.ChangeDetector._propertyMappings = new Dictionary<Type, NetworkBehaviour.ChangeDetector.PropertyData[]>();
				}
				NetworkBehaviour.ChangeDetector.PropertyData[] result;
				bool flag2 = !NetworkBehaviour.ChangeDetector._propertyMappings.TryGetValue(type, out result);
				if (flag2)
				{
					List<NetworkBehaviour.ChangeDetector.PropertyData> list = new List<NetworkBehaviour.ChangeDetector.PropertyData>();
					bool flag3;
					NetworkBehaviour.ChangeDetector.AddPropertiesToMappingForType(type, list, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, out flag3);
					Type baseType = type.BaseType;
					while (baseType != null && baseType != typeof(NetworkBehaviour))
					{
						bool flag4;
						NetworkBehaviour.ChangeDetector.AddPropertiesToMappingForType(baseType, list, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic, out flag4);
						flag3 = (flag4 || flag3);
						baseType = baseType.BaseType;
					}
					bool flag5 = flag3;
					if (flag5)
					{
						NetworkBehaviour.ChangeDetector._hasChangeCallbacks[type] = true;
					}
					NetworkBehaviour.ChangeDetector._propertyMappings.Add(type, result = list.ToArray());
				}
				return result;
			}

			private static void AddPropertiesToMappingForType(Type type, List<NetworkBehaviour.ChangeDetector.PropertyData> result, BindingFlags bindingFlags, out bool hasChangeCallbacks)
			{
				hasChangeCallbacks = false;
				PropertyInfo[] properties = type.GetProperties(bindingFlags);
				int i = 0;
				while (i < properties.Length)
				{
					PropertyInfo propertyInfo = properties[i];
					NetworkedWeavedAttribute customAttribute = propertyInfo.GetCustomAttribute<NetworkedWeavedAttribute>();
					bool flag = customAttribute != null;
					if (flag)
					{
						NetworkBehaviour.ChangeDetector.PropertyData item = new NetworkBehaviour.ChangeDetector.PropertyData
						{
							PropertyInfo = propertyInfo,
							WeavedAttribute = customAttribute
						};
						OnChangedRenderAttribute customAttribute2 = propertyInfo.GetCustomAttribute<OnChangedRenderAttribute>();
						bool flag2 = customAttribute2 != null;
						if (flag2)
						{
							Type type2 = type;
							MethodInfo method;
							do
							{
								method = type2.GetMethod(customAttribute2.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
								type2 = type2.BaseType;
							}
							while (method == null && type2 != null);
							bool flag3 = method == null;
							if (flag3)
							{
								LogStream logError = InternalLogStreams.LogError;
								if (logError != null)
								{
									logError.Log(string.Concat(new string[]
									{
										"Change Detector was not able to find any method named (",
										customAttribute2.MethodName,
										") on type (",
										type.Name,
										") or any base class."
									}));
								}
								goto IL_1A2;
							}
							bool flag4 = method.GetParameters().Length == 1;
							if (flag4)
							{
								item.OnChangedPrev = (NetworkBehaviour.ChangeDetector.OnChangedPrevCallbackWrapper)typeof(NetworkBehaviour.ChangeDetector).GetMethod("GetWrapperPrev", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
								{
									type
								}).Invoke(null, new object[]
								{
									method
								});
							}
							else
							{
								item.OnChanged = (NetworkBehaviour.ChangeDetector.OnChangedCallbackWrapper)typeof(NetworkBehaviour.ChangeDetector).GetMethod("GetWrapper", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
								{
									type
								}).Invoke(null, new object[]
								{
									method
								});
							}
							hasChangeCallbacks = true;
						}
						result.Add(item);
					}
					IL_1A2:
					i++;
					continue;
					goto IL_1A2;
				}
			}

			~ChangeDetector()
			{
				Native.Free<int>(ref this._wordsPrevious);
			}

			public unsafe void Init(NetworkBehaviour networkBehaviour, NetworkBehaviour.ChangeDetector.Source source, bool copyInitial = true)
			{
				bool flag = networkBehaviour.WordCount == 0;
				if (flag)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("Change detector cannot be bound to a behaviour with zero network properties.");
					}
				}
				else
				{
					int[] words = this._words;
					bool flag2 = ((words != null) ? new int?(words.Length) : null).GetValueOrDefault() < networkBehaviour.WordCount;
					if (flag2)
					{
						this._words = new int[networkBehaviour.WordCount];
					}
					else
					{
						Array.Clear(this._words, 0, this._words.Length);
					}
					int num = NetworkBehaviour.ChangeDetector.GetPropertyMappping(networkBehaviour.GetType()).Length;
					bool flag3 = this._changed == null || this._changed.Length < num;
					if (flag3)
					{
						this._changed = new string[num];
						this._changedProperty = new NetworkBehaviour.ChangeDetector.PropertyData[num];
					}
					this._instance = new int?(networkBehaviour.GetInstanceID());
					this._source = source;
					this._sourceTick = default(Tick);
					if (copyInitial)
					{
						int[] array;
						int* destination;
						if ((array = this._words) == null || array.Length == 0)
						{
							destination = null;
						}
						else
						{
							destination = &array[0];
						}
						Native.MemCpy((void*)destination, (void*)networkBehaviour.Ptr, this._words.Length * 4);
						array = null;
					}
				}
			}

			public NetworkBehaviour.ChangeDetector.Enumerable DetectChanges(NetworkBehaviour b, out NetworkBehaviourBuffer previous, out NetworkBehaviourBuffer current, bool copyChanges = true)
			{
				return this.DetectChangesInternal(b, out previous, out current, copyChanges);
			}

			public NetworkBehaviour.ChangeDetector.Enumerable DetectChanges(NetworkBehaviour b, bool copyChanges = true)
			{
				NetworkBehaviourBuffer networkBehaviourBuffer;
				NetworkBehaviourBuffer networkBehaviourBuffer2;
				return this.DetectChangesInternal(b, out networkBehaviourBuffer, out networkBehaviourBuffer2, copyChanges);
			}

			private unsafe NetworkBehaviour.ChangeDetector.Enumerable DetectChangesInternal(NetworkBehaviour b, out NetworkBehaviourBuffer previous, out NetworkBehaviourBuffer current, bool copyChanges = true)
			{
				bool flag = b.GetInstanceID() != this._instance.GetValueOrDefault();
				NetworkBehaviour.ChangeDetector.Enumerable result;
				if (flag)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("This behaviour is not bound to this change detector");
					}
					current = default(NetworkBehaviourBuffer);
					previous = default(NetworkBehaviourBuffer);
					result = default(NetworkBehaviour.ChangeDetector.Enumerable);
				}
				else
				{
					int num = 0;
					int* ptr = b.Ptr;
					NetworkBehaviour.ChangeDetector.PropertyData[] propertyMappping = NetworkBehaviour.ChangeDetector.GetPropertyMappping(b.GetType());
					Tick sourceTick = this._sourceTick;
					bool flag2 = this._source > NetworkBehaviour.ChangeDetector.Source.SimulationState;
					if (flag2)
					{
						NetworkBehaviourBuffer networkBehaviourBuffer;
						NetworkBehaviourBuffer networkBehaviourBuffer2;
						float num2;
						RenderTimeline.GetRenderBuffers(b, out networkBehaviourBuffer, out networkBehaviourBuffer2, out num2);
						NetworkBehaviourBuffer networkBehaviourBuffer3 = (this._source == NetworkBehaviour.ChangeDetector.Source.SnapshotFrom) ? networkBehaviourBuffer : networkBehaviourBuffer2;
						bool flag3 = networkBehaviourBuffer3._ptr == null;
						if (flag3)
						{
							current = default(NetworkBehaviourBuffer);
							previous = default(NetworkBehaviourBuffer);
							return default(NetworkBehaviour.ChangeDetector.Enumerable);
						}
						bool flag4 = this._sourceTick == networkBehaviourBuffer3.Tick;
						if (flag4)
						{
							current = default(NetworkBehaviourBuffer);
							previous = default(NetworkBehaviourBuffer);
							return default(NetworkBehaviour.ChangeDetector.Enumerable);
						}
						this._sourceTick = networkBehaviourBuffer3.Tick;
						ptr = networkBehaviourBuffer3._ptr;
					}
					else
					{
						this._sourceTick = b.Object.Runner.Tick;
					}
					int[] array;
					int* ptr2;
					if ((array = this._words) == null || array.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array[0];
					}
					bool flag5 = this._wordsPrevious == null;
					if (flag5)
					{
						this._wordsPrevious = Native.MallocAndClearArray<int>(this._words.Length);
					}
					Native.MemCpy((void*)this._wordsPrevious, (void*)ptr2, this._words.Length * 4);
					foreach (NetworkBehaviour.ChangeDetector.PropertyData propertyData in propertyMappping)
					{
						int wordOffset = propertyData.WeavedAttribute.WordOffset;
						int wordCount = propertyData.WeavedAttribute.WordCount;
						for (int j = wordOffset; j < wordOffset + wordCount; j++)
						{
							bool flag6 = ptr2[j] != ptr[j];
							if (flag6)
							{
								this._changedProperty[num] = propertyData;
								this._changed[num++] = propertyData.PropertyInfo.Name;
								if (copyChanges)
								{
									Native.MemCpy((void*)(ptr2 + wordOffset), (void*)(ptr + wordOffset), wordCount * 4);
								}
								break;
							}
						}
					}
					bool flag7 = num > 0;
					if (flag7)
					{
						previous = new NetworkBehaviourBuffer(sourceTick, this._wordsPrevious, this._words.Length);
					}
					else
					{
						previous = default(NetworkBehaviourBuffer);
					}
					array = null;
					bool flag8 = this.InvokeCallbacks && this._changedProperty != null;
					if (flag8)
					{
						int num3 = 0;
						while (num3 < num && num3 < this._changedProperty.Length)
						{
							NetworkBehaviour.ChangeDetector.PropertyData propertyData2 = this._changedProperty[num3];
							bool flag9 = propertyData2.OnChangedPrev != null;
							if (flag9)
							{
								try
								{
									propertyData2.OnChangedPrev(b, previous);
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
							bool flag10 = propertyData2.OnChanged != null;
							if (flag10)
							{
								try
								{
									propertyData2.OnChanged(b);
								}
								catch (Exception error2)
								{
									LogStream logException2 = InternalLogStreams.LogException;
									if (logException2 != null)
									{
										logException2.Log(error2);
									}
								}
							}
							num3++;
						}
					}
					current = new NetworkBehaviourBuffer(this._sourceTick, ptr, this._words.Length);
					result = new NetworkBehaviour.ChangeDetector.Enumerable(this._changed, num);
				}
				return result;
			}

			private static Dictionary<Type, NetworkBehaviour.ChangeDetector.PropertyData[]> _propertyMappings = new Dictionary<Type, NetworkBehaviour.ChangeDetector.PropertyData[]>();

			private static Dictionary<Type, bool> _hasChangeCallbacks = new Dictionary<Type, bool>();

			private int? _instance;

			private int[] _words;

			private unsafe int* _wordsPrevious;

			private NetworkBehaviour.ChangeDetector.Source _source;

			private Tick _sourceTick;

			private string[] _changed;

			private NetworkBehaviour.ChangeDetector.PropertyData[] _changedProperty;

			internal bool InvokeCallbacks;

			public enum Source
			{
				SimulationState,
				SnapshotFrom,
				SnapshotTo
			}

			private struct PropertyData
			{
				public MemberInfo PropertyInfo;

				public NetworkedWeavedAttribute WeavedAttribute;

				public NetworkBehaviour.ChangeDetector.OnChangedCallbackWrapper OnChanged;

				public NetworkBehaviour.ChangeDetector.OnChangedPrevCallbackWrapper OnChangedPrev;
			}

			internal delegate void OnChangedPrevCallback<T>(T b, NetworkBehaviourBuffer prev) where T : NetworkBehaviour;

			internal delegate void OnChangedPrevCallbackWrapper(NetworkBehaviour b, NetworkBehaviourBuffer prev);

			internal delegate void OnChangedCallback<T>(T b) where T : NetworkBehaviour;

			internal delegate void OnChangedCallbackWrapper(NetworkBehaviour b);

			public struct Enumerable
			{
				internal Enumerable(string[] changed, int count)
				{
					this._changed = changed;
					this._count = count;
				}

				public NetworkBehaviour.ChangeDetector.Enumerator GetEnumerator()
				{
					return new NetworkBehaviour.ChangeDetector.Enumerator(this._changed, this._count);
				}

				public bool Changed(string name)
				{
					return this._count > 0 && Array.IndexOf<string>(this._changed, name, 0) >= 0;
				}

				private string[] _changed;

				private int _count;
			}

			public struct Enumerator
			{
				public string Current
				{
					get
					{
						return this._changed[this._current];
					}
				}

				internal Enumerator(string[] changed, int count)
				{
					this._changed = changed;
					this._count = count;
					this._current = -1;
				}

				public void Reset()
				{
					this._current = -1;
				}

				public bool MoveNext()
				{
					int num = this._current + 1;
					this._current = num;
					return num < this._count;
				}

				private string[] _changed;

				private int _count;

				private int _current;
			}
		}
	}
}
