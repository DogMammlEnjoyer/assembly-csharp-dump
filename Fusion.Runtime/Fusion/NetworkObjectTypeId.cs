using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion.Sockets;

namespace Fusion
{
	[InlineHelp]
	[NetworkStructWeaved(2)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkObjectTypeId : INetworkStruct, IEquatable<NetworkObjectTypeId>
	{
		public static NetworkObjectTypeId.EqualityComparer Comparer { get; } = new NetworkObjectTypeId.EqualityComparer();

		public static NetworkObjectTypeId PlayerData
		{
			get
			{
				return NetworkObjectTypeId.FromStruct(1);
			}
		}

		public NetworkTypeIdKind Kind
		{
			get
			{
				bool flag = this._value0 == 0U && this._value1 == 0U;
				NetworkTypeIdKind result;
				if (flag)
				{
					result = NetworkTypeIdKind.Invalid;
				}
				else
				{
					result = (NetworkTypeIdKind)(this._value1 & 3U);
				}
				return result;
			}
		}

		public static NetworkObjectTypeId FromSceneRefAndObjectIndex(SceneRef sceneRef, int objIndex, NetworkSceneLoadId loadId = default(NetworkSceneLoadId))
		{
			return NetworkObjectTypeId.FromSceneObjectId(new NetworkSceneObjectId
			{
				Scene = sceneRef,
				ObjectId = objIndex,
				LoadId = loadId
			});
		}

		public static NetworkObjectTypeId FromSceneObjectId(NetworkSceneObjectId sceneObjectId)
		{
			bool flag = !sceneObjectId.Scene.IsValid;
			if (flag)
			{
				throw new ArgumentException("SceneRef is not valid", "sceneObjectId");
			}
			bool flag2 = sceneObjectId.ObjectId < 0 || sceneObjectId.ObjectId > 4194303;
			if (flag2)
			{
				throw new ArgumentException("ObjectId is out of range", "sceneObjectId");
			}
			NetworkObjectTypeId result;
			result._value0 = sceneObjectId.Scene.RawValue;
			result._value1 = (uint)(3 | sceneObjectId.ObjectId << 2 | (int)sceneObjectId.LoadId.Value << 24);
			return result;
		}

		public NetworkSceneObjectId AsSceneObjectId
		{
			get
			{
				bool flag = !this.IsSceneObject;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Invalid kind, got {0}, expected {1}", this.Kind, NetworkTypeIdKind.SceneObject));
				}
				SceneRef scene = SceneRef.FromRaw(this._value0);
				int objectId = (int)(this._value1 >> 2 & 4194303U);
				byte value = (byte)(this._value1 >> 24);
				return new NetworkSceneObjectId
				{
					ObjectId = objectId,
					Scene = scene,
					LoadId = value
				};
			}
		}

		public static NetworkObjectTypeId FromPrefabId(NetworkPrefabId prefabId)
		{
			bool flag = !prefabId.IsValid;
			if (flag)
			{
				throw new ArgumentException("PrefabId is not valid", "prefabId");
			}
			NetworkObjectTypeId result;
			result._value0 = prefabId.RawValue;
			result._value1 = 0U;
			return result;
		}

		public NetworkPrefabId AsPrefabId
		{
			get
			{
				bool flag = !this.IsPrefab;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Invalid kind, got {0}, expected {1}", this.Kind, NetworkTypeIdKind.Prefab));
				}
				return NetworkPrefabId.FromRaw(this._value0);
			}
		}

		public static NetworkObjectTypeId FromCustom(uint raw)
		{
			NetworkObjectTypeId result;
			result._value0 = raw;
			result._value1 = 1U;
			return result;
		}

		public uint AsCustom
		{
			get
			{
				bool flag = !this.IsCustom;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Invalid kind, got {0}, expected {1}", this.Kind, NetworkTypeIdKind.Custom));
				}
				return this._value0;
			}
		}

		public static NetworkObjectTypeId FromStruct(ushort structId)
		{
			NetworkObjectTypeId result;
			result._value0 = (uint)structId;
			result._value1 = 2U;
			return result;
		}

		public ushort AsInternalStructId
		{
			get
			{
				bool flag = !this.IsStruct;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Invalid kind, got {0}, expected {1}", this.Kind, NetworkTypeIdKind.InternalStruct));
				}
				return (ushort)this._value0;
			}
		}

		public bool IsNone
		{
			get
			{
				return this.Kind == NetworkTypeIdKind.Invalid;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Kind != NetworkTypeIdKind.Invalid;
			}
		}

		public bool IsSceneObject
		{
			get
			{
				return this.Kind == NetworkTypeIdKind.SceneObject;
			}
		}

		public bool IsPrefab
		{
			get
			{
				return this.Kind == NetworkTypeIdKind.Prefab;
			}
		}

		public bool IsStruct
		{
			get
			{
				return this.Kind == NetworkTypeIdKind.InternalStruct;
			}
		}

		public bool IsCustom
		{
			get
			{
				return this.Kind == NetworkTypeIdKind.Custom;
			}
		}

		public bool Equals(NetworkObjectTypeId other)
		{
			return this._value0 == other._value0 && this._value1 == other._value1;
		}

		public override int GetHashCode()
		{
			int value = (int)this._value0;
			return value * 397 ^ (int)this._value1;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkObjectTypeId)
			{
				NetworkObjectTypeId other = (NetworkObjectTypeId)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override string ToString()
		{
			bool flag = !this.IsValid;
			string result;
			if (flag)
			{
				result = "[None]";
			}
			else
			{
				switch (this.Kind)
				{
				case NetworkTypeIdKind.Prefab:
					result = string.Format("[Prefab {0}]", this.AsPrefabId.AsIndex);
					break;
				case NetworkTypeIdKind.Custom:
					result = string.Format("[Custom 0x{0:X8}]", this.AsCustom);
					break;
				case NetworkTypeIdKind.InternalStruct:
					result = string.Format("[Struct 0x{0:X4}]", this.AsInternalStructId);
					break;
				case NetworkTypeIdKind.SceneObject:
					result = this.AsSceneObjectId.ToString();
					break;
				default:
					result = "[Invalid]";
					break;
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(NetworkObjectTypeId a, NetworkObjectTypeId b)
		{
			return a._value0 == b._value0 && a._value1 == b._value1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(NetworkObjectTypeId a, NetworkObjectTypeId b)
		{
			return a._value0 != b._value0 || a._value1 != b._value1;
		}

		public static implicit operator NetworkObjectTypeId(NetworkPrefabId prefabId)
		{
			return NetworkObjectTypeId.FromPrefabId(prefabId);
		}

		internal unsafe static void WriteInternal(NetworkObjectTypeId typeId, NetBitBuffer* buffer, int blockSize)
		{
			buffer->WriteUInt32VarLength(typeId._value0, blockSize);
			buffer->WriteUInt32VarLength(typeId._value1, blockSize);
		}

		internal unsafe static NetworkObjectTypeId ReadInternal(NetBitBuffer* buffer, int blockSize)
		{
			return new NetworkObjectTypeId
			{
				_value0 = buffer->ReadUInt32VarLength(blockSize),
				_value1 = buffer->ReadUInt32VarLength(blockSize)
			};
		}

		public const int SIZE = 8;

		public const int ALIGNMENT = 4;

		private const int KIND_MASK = 3;

		private const int KIND_BITS = 2;

		private const int SCENE_OBJECT_INDEX_SHIFT = 2;

		private const int SCENE_OBJECT_INDEX_BITS = 22;

		private const int SCENE_OBJECT_INDEX_MASK = 4194303;

		private const int SCENE_OBJECT_LOAD_ID_SHIFT = 24;

		private const int SCENE_OBJECT_LOAD_ID_BITS = 8;

		public const int MAX_SCENE_OBJECT_INDEX = 4194303;

		private const ushort STRUCT_TYPE_PLAYERDATA = 1;

		[FieldOffset(0)]
		private uint _value0;

		[FieldOffset(4)]
		private uint _value1;

		public sealed class EqualityComparer : IEqualityComparer<NetworkObjectTypeId>
		{
			public bool Equals(NetworkObjectTypeId x, NetworkObjectTypeId y)
			{
				return x.Equals(y);
			}

			public int GetHashCode(NetworkObjectTypeId obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}
