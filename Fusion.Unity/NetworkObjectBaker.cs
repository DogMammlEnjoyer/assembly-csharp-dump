using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Fusion
{
	public class NetworkObjectBaker
	{
		protected virtual void SetDirty(MonoBehaviour obj)
		{
		}

		protected virtual bool TryGetExecutionOrder(MonoBehaviour obj, out int order)
		{
			order = 0;
			return false;
		}

		protected virtual uint GetSortKey(NetworkObject obj)
		{
			return 0U;
		}

		protected virtual bool PostprocessBehaviour(SimulationBehaviour behaviour)
		{
			return false;
		}

		[Conditional("FUSION_EDITOR_TRACE")]
		protected static void Trace(string msg)
		{
			Debug.Log("[Fusion/NetworkObjectBaker] " + msg);
		}

		protected static void Warn(string msg, Object context = null)
		{
			Debug.LogWarning("[Fusion/NetworkObjectBaker] " + msg, context);
		}

		public NetworkObjectBaker.Result Bake(GameObject root)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			root.GetComponentsInChildren<NetworkObject>(true, this._allNetworkObjects);
			this._allNetworkObjects.RemoveAll((NetworkObject x) => x == null);
			if (this._allNetworkObjects.Count == 0)
			{
				return new NetworkObjectBaker.Result(false, 0, 0);
			}
			NetworkObjectBaker.Result result;
			try
			{
				foreach (NetworkObject networkObject in this._allNetworkObjects)
				{
					this._networkObjectsPaths.Add(this._pathCache.Create(networkObject.transform));
				}
				bool dirty = false;
				this._allNetworkObjects.Reverse();
				this._networkObjectsPaths.Reverse();
				root.GetComponentsInChildren<SimulationBehaviour>(true, this._allSimulationBehaviours);
				this._allSimulationBehaviours.RemoveAll((SimulationBehaviour x) => x == null);
				int count = this._allNetworkObjects.Count;
				int count2 = this._allSimulationBehaviours.Count;
				for (int i = 0; i < this._allNetworkObjects.Count; i++)
				{
					NetworkObject networkObject2 = this._allNetworkObjects[i];
					bool flag = false;
					bool activeInHierarchy = networkObject2.gameObject.activeInHierarchy;
					int? num = null;
					if (!activeInHierarchy)
					{
						int value;
						if (this.TryGetExecutionOrder(networkObject2, out value))
						{
							num = new int?(value);
						}
						else
						{
							NetworkObjectBaker.Warn(string.Format("Unable to get execution order for {0}. ", networkObject2) + "Because the object is initially inactive, Fusion is unable to guarantee the script's Awake will be invoked before Spawned. Please implement TryGetExecutionOrder.", null);
						}
					}
					this._arrayBufferNB.Clear();
					NetworkObjectBaker.TransformPath x2 = this._networkObjectsPaths[i];
					x2.ToString();
					for (int j = this._allSimulationBehaviours.Count - 1; j >= 0; j--)
					{
						SimulationBehaviour simulationBehaviour = this._allSimulationBehaviours[j];
						NetworkObjectBaker.TransformPath y = this._pathCache.Create(simulationBehaviour.transform);
						if (this._pathCache.IsEqualOrAncestorOf(x2, y))
						{
							NetworkBehaviour networkBehaviour = simulationBehaviour as NetworkBehaviour;
							if (networkBehaviour != null)
							{
								this._arrayBufferNB.Add(networkBehaviour);
							}
							flag |= this.PostprocessBehaviour(simulationBehaviour);
							this._allSimulationBehaviours.RemoveAt(j);
							if (num != null)
							{
								int num2;
								if (this.TryGetExecutionOrder(simulationBehaviour, out num2))
								{
									int? num3 = num;
									int num4 = num2;
									if (num3.GetValueOrDefault() <= num4 & num3 != null)
									{
										NetworkObjectBaker.Warn(string.Format("{0} execution order is less or equal than of the script {1}. ", networkObject2, simulationBehaviour) + "Because the object is initially inactive, Spawned callback will be invoked before the script's Awake on activation.", simulationBehaviour);
									}
								}
								else
								{
									NetworkObjectBaker.Warn(string.Format("Unable to get execution order for {0}. ", simulationBehaviour) + "Because the object is initially inactive, Fusion is unable to guarantee the script's Awake will be invoked before Spawned. Please implement TryGetExecutionOrder.", null);
								}
							}
						}
						else if (this._pathCache.Compare(x2, y) >= 0)
						{
							break;
						}
					}
					this._arrayBufferNB.Reverse();
					flag |= this.Set<NetworkBehaviour>(networkObject2, ref networkObject2.NetworkedBehaviours, this._arrayBufferNB);
					NetworkObjectFlags networkObjectFlags = networkObject2.Flags;
					if (!networkObjectFlags.IsVersionCurrent())
					{
						networkObjectFlags = networkObjectFlags.SetCurrentVersion();
					}
					flag |= this.Set<NetworkObjectFlags>(networkObject2, ref networkObject2.Flags, networkObjectFlags);
					this._arrayBufferNO.Clear();
					for (int k = i - 1; k >= 0; k--)
					{
						NetworkObjectBaker.TransformPathCache pathCache = this._pathCache;
						NetworkObjectBaker.TransformPath transformPath = this._networkObjectsPaths[k];
						if (!pathCache.IsAncestorOf(x2, transformPath))
						{
							break;
						}
						this._arrayBufferNO.Add(this._allNetworkObjects[k]);
					}
					flag |= this.Set<NetworkObject>(networkObject2, ref networkObject2.NestedObjects, this._arrayBufferNO);
					flag |= this.Set<uint>(networkObject2, ref networkObject2.SortKey, this.GetSortKey(networkObject2));
					if (flag)
					{
						this.SetDirty(networkObject2);
						dirty = true;
					}
				}
				result = new NetworkObjectBaker.Result(dirty, count, count2);
			}
			finally
			{
				this._pathCache.Clear();
				this._allNetworkObjects.Clear();
				this._allSimulationBehaviours.Clear();
				this._networkObjectsPaths.Clear();
				this._arrayBufferNB.Clear();
				this._arrayBufferNO.Clear();
			}
			return result;
		}

		private bool Set<T>(MonoBehaviour host, ref T field, T value)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				return true;
			}
			return false;
		}

		private bool Set<T>(MonoBehaviour host, ref T[] field, List<T> value)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (field == null || field.Length != value.Count || !field.SequenceEqual(value, @default))
			{
				field = value.ToArray();
				return true;
			}
			return false;
		}

		private List<NetworkObject> _allNetworkObjects = new List<NetworkObject>();

		private List<NetworkObjectBaker.TransformPath> _networkObjectsPaths = new List<NetworkObjectBaker.TransformPath>();

		private List<SimulationBehaviour> _allSimulationBehaviours = new List<SimulationBehaviour>();

		private NetworkObjectBaker.TransformPathCache _pathCache = new NetworkObjectBaker.TransformPathCache();

		private List<NetworkBehaviour> _arrayBufferNB = new List<NetworkBehaviour>();

		private List<NetworkObject> _arrayBufferNO = new List<NetworkObject>();

		public struct Result
		{
			public readonly bool HadChanges { get; }

			public readonly int ObjectCount { get; }

			public readonly int BehaviourCount { get; }

			public Result(bool dirty, int objectCount, int behaviourCount)
			{
				this.HadChanges = dirty;
				this.ObjectCount = objectCount;
				this.BehaviourCount = behaviourCount;
			}
		}

		public readonly struct TransformPath
		{
			internal unsafe TransformPath(ushort depth, ushort next, List<ushort> indices, int offset, int count)
			{
				this.Depth = depth;
				this.Next = next;
				for (int i = 0; i < count; i++)
				{
					*(ref this.Indices.Value.FixedElementField + (IntPtr)i * 2) = indices[i + offset];
				}
			}

			public unsafe override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				while (num < (int)this.Depth && num < 10)
				{
					if (num > 0)
					{
						stringBuilder.Append("/");
					}
					stringBuilder.Append(*(ref this.Indices.Value.FixedElementField + (IntPtr)num * 2));
					num++;
				}
				if (this.Depth > 10)
				{
					stringBuilder.Append(string.Format("/...[{0}]", (int)(this.Depth - 10)));
				}
				return stringBuilder.ToString();
			}

			public const int MaxDepth = 10;

			public readonly NetworkObjectBaker.TransformPath._Indices Indices;

			public readonly ushort Depth;

			public readonly ushort Next;

			public struct _Indices
			{
				[FixedBuffer(typeof(ushort), 10)]
				public NetworkObjectBaker.TransformPath._Indices.<Value>e__FixedBuffer Value;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 20)]
				public struct <Value>e__FixedBuffer
				{
					public ushort FixedElementField;
				}
			}
		}

		public sealed class TransformPathCache : IComparer<NetworkObjectBaker.TransformPath>, IEqualityComparer<NetworkObjectBaker.TransformPath>
		{
			public NetworkObjectBaker.TransformPath Create(Transform transform)
			{
				NetworkObjectBaker.TransformPath result;
				if (this._cache.TryGetValue(transform, out result))
				{
					return result;
				}
				this._siblingIndexStack.Clear();
				Transform transform2 = transform;
				ushort num;
				ushort next;
				checked
				{
					while (transform2 != null)
					{
						this._siblingIndexStack.Add((ushort)transform2.GetSiblingIndex());
						transform2 = transform2.parent;
					}
					this._siblingIndexStack.Reverse();
					num = (ushort)this._siblingIndexStack.Count;
					next = 0;
				}
				if (num > 10)
				{
					int i;
					if (num % 10 != 0)
					{
						i = (int)(num - num % 10);
					}
					else
					{
						i = (int)(num - 10);
					}
					while (i > 0)
					{
						checked
						{
							NetworkObjectBaker.TransformPath item = new NetworkObjectBaker.TransformPath((ushort)((int)num - i), next, this._siblingIndexStack, i, Mathf.Min(10, (int)num - i));
							this._nexts.Add(item);
							next = (ushort)this._nexts.Count;
						}
						i -= 10;
					}
				}
				NetworkObjectBaker.TransformPath transformPath = new NetworkObjectBaker.TransformPath(num, next, this._siblingIndexStack, 0, Mathf.Min(10, (int)num));
				this._cache.Add(transform, transformPath);
				return transformPath;
			}

			public void Clear()
			{
				this._nexts.Clear();
				this._cache.Clear();
				this._siblingIndexStack.Clear();
			}

			public bool Equals(NetworkObjectBaker.TransformPath x, NetworkObjectBaker.TransformPath y)
			{
				return x.Depth == y.Depth && this.CompareToDepthUnchecked(x, y, (int)x.Depth) == 0;
			}

			public int GetHashCode(NetworkObjectBaker.TransformPath obj)
			{
				int depth = (int)obj.Depth;
				return this.GetHashCode(obj, depth);
			}

			public int Compare(NetworkObjectBaker.TransformPath x, NetworkObjectBaker.TransformPath y)
			{
				int num = this.CompareToDepthUnchecked(x, y, Mathf.Min((int)x.Depth, (int)y.Depth));
				if (num != 0)
				{
					return num;
				}
				return (int)(x.Depth - y.Depth);
			}

			private unsafe int CompareToDepthUnchecked(in NetworkObjectBaker.TransformPath x, in NetworkObjectBaker.TransformPath y, int depth)
			{
				int num = 0;
				while (num < depth && num < 10)
				{
					int num2 = (int)(*(ref x.Indices.Value.FixedElementField + (IntPtr)num * 2) - *(ref y.Indices.Value.FixedElementField + (IntPtr)num * 2));
					if (num2 != 0)
					{
						return num2;
					}
					num++;
				}
				if (depth > 10)
				{
					NetworkObjectBaker.TransformPath transformPath = this._nexts[(int)(x.Next - 1)];
					NetworkObjectBaker.TransformPath transformPath2 = this._nexts[(int)(y.Next - 1)];
					return this.CompareToDepthUnchecked(transformPath, transformPath2, depth - 10);
				}
				return 0;
			}

			private unsafe int GetHashCode(in NetworkObjectBaker.TransformPath path, int hash)
			{
				int num = 0;
				while (num < (int)path.Depth && num < 10)
				{
					hash = hash * 31 + (int)(*(ref path.Indices.Value.FixedElementField + (IntPtr)num * 2));
					num++;
				}
				if (path.Depth > 10)
				{
					NetworkObjectBaker.TransformPath transformPath = this._nexts[(int)(path.Next - 1)];
					hash = this.GetHashCode(transformPath, hash);
				}
				return hash;
			}

			public bool IsAncestorOf(in NetworkObjectBaker.TransformPath x, in NetworkObjectBaker.TransformPath y)
			{
				return x.Depth < y.Depth && this.CompareToDepthUnchecked(x, y, (int)x.Depth) == 0;
			}

			public bool IsEqualOrAncestorOf(in NetworkObjectBaker.TransformPath x, in NetworkObjectBaker.TransformPath y)
			{
				return x.Depth <= y.Depth && this.CompareToDepthUnchecked(x, y, (int)x.Depth) == 0;
			}

			public string Dump(in NetworkObjectBaker.TransformPath x)
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.Dump(x, stringBuilder);
				return stringBuilder.ToString();
			}

			private unsafe void Dump(in NetworkObjectBaker.TransformPath x, StringBuilder builder)
			{
				int num = 0;
				while (num < (int)x.Depth && num < 10)
				{
					if (num > 0)
					{
						builder.Append("/");
					}
					builder.Append(*(ref x.Indices.Value.FixedElementField + (IntPtr)num * 2));
					num++;
				}
				if (x.Depth > 10)
				{
					builder.Append("/");
					NetworkObjectBaker.TransformPath transformPath = this._nexts[(int)(x.Next - 1)];
					this.Dump(transformPath, builder);
				}
			}

			private Dictionary<Transform, NetworkObjectBaker.TransformPath> _cache = new Dictionary<Transform, NetworkObjectBaker.TransformPath>();

			private List<ushort> _siblingIndexStack = new List<ushort>();

			private List<NetworkObjectBaker.TransformPath> _nexts = new List<NetworkObjectBaker.TransformPath>();
		}
	}
}
