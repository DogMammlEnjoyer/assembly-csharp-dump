using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Fusion.Statistics;

namespace Fusion
{
	internal class SimulationBehaviourUpdater
	{
		private static Type[] CallbackInterfacesDefualts
		{
			get
			{
				return new Type[]
				{
					typeof(IAfterRender),
					typeof(IBeforeTick),
					typeof(IAfterTick),
					typeof(IBeforeAllTicks),
					typeof(IAfterAllTicks),
					typeof(IBeforeSimulation),
					typeof(IBeforeHitboxRegistration),
					typeof(IPlayerJoined),
					typeof(IPlayerLeft),
					typeof(IBeforeUpdate),
					typeof(IAfterUpdate),
					typeof(ISceneLoadDone),
					typeof(ISceneLoadStart),
					typeof(IAfterClientPredictionReset),
					typeof(IBeforeClientPredictionReset),
					typeof(IBeforeCopyPreviousState),
					typeof(IBeforeUpdateRemotePrefabs),
					typeof(IAfterUpdateRemotePrefabs),
					typeof(IAfterHostMigration)
				};
			}
		}

		public SimulationBehaviourUpdater(NetworkProjectConfig config)
		{
			this._byTypeLookup = new Dictionary<Type, SimulationBehaviourUpdater.BehaviourList>();
			this._byTypeHierarchy = new Dictionary<Type, ValueTuple<SimulationBehaviour[], Type[]>>();
			this._inOrderList = new List<SimulationBehaviourUpdater.BehaviourList>();
			this._inOrderByInterfaceList = new Dictionary<Type, List<SimulationBehaviourUpdater.BehaviourList>>();
			this._behavioursChecked = new HashSet<Type>();
			this._config = config;
		}

		private static List<Type> Scanlibrary()
		{
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				bool flag = assembly.GetCustomAttribute<NetworkAssemblyIgnoreAttribute>() != null;
				if (!flag)
				{
					try
					{
						Type[] types = assembly.GetTypes();
						foreach (Type type in types)
						{
							bool flag2 = typeof(SimulationBehaviour).IsAssignableFrom(type);
							if (flag2)
							{
								bool flag3 = !list.Contains(type);
								if (flag3)
								{
									list.Add(type);
								}
							}
						}
					}
					catch (Exception)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn("Error while loading types from Assembly: " + assembly.FullName + ". Ignore.");
						}
					}
				}
			}
			return list;
		}

		private static ValueTuple<SimulationModes, SimulationStages, Topologies> GetSimulationFlags(Type type)
		{
			while (typeof(SimulationBehaviour).IsAssignableFrom(type))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(SimulationBehaviourAttribute), true);
				bool flag = customAttributes.Length != 0;
				if (flag)
				{
					SimulationBehaviourAttribute simulationBehaviourAttribute = (SimulationBehaviourAttribute)customAttributes[0];
					return new ValueTuple<SimulationModes, SimulationStages, Topologies>((simulationBehaviourAttribute.Modes == (SimulationModes)0) ? (SimulationModes.Server | SimulationModes.Host | SimulationModes.Client) : simulationBehaviourAttribute.Modes, (simulationBehaviourAttribute.Stages == (SimulationStages)0) ? (SimulationStages.Forward | SimulationStages.Resimulate) : simulationBehaviourAttribute.Stages, (simulationBehaviourAttribute.Topologies == (Topologies)0) ? (Topologies.ClientServer | Topologies.Shared) : simulationBehaviourAttribute.Topologies);
				}
				type = type.BaseType;
			}
			return new ValueTuple<SimulationModes, SimulationStages, Topologies>(SimulationModes.Server | SimulationModes.Host | SimulationModes.Client, SimulationStages.Forward | SimulationStages.Resimulate, Topologies.ClientServer | Topologies.Shared);
		}

		private int GetExecutionOrder(Type type)
		{
			while (type != typeof(object))
			{
				int? executionOrder = this._config.GetExecutionOrder(type);
				bool flag = executionOrder != null;
				if (flag)
				{
					return executionOrder.Value;
				}
				type = type.BaseType;
			}
			return 0;
		}

		public void BuildTypeOrder(Type[] customCallbackInterfaces)
		{
			bool flag = customCallbackInterfaces != null;
			if (flag)
			{
				Assert.Always(customCallbackInterfaces.All((Type x) => x.IsInterface), "All types provided as custom callback interfaces must be interfaces.");
			}
			else
			{
				customCallbackInterfaces = new Type[0];
			}
			this._inOrderList.Clear();
			this._byTypeLookup.Clear();
			foreach (Type type in SimulationBehaviourUpdater.Scanlibrary())
			{
				this.AddType(type, SimulationBehaviourUpdater.GetSimulationFlags(type));
			}
			this._inOrderList.Sort((SimulationBehaviourUpdater.BehaviourList a, SimulationBehaviourUpdater.BehaviourList b) => a.ExecutionOrder.CompareTo(b.ExecutionOrder));
			foreach (Type type2 in SimulationBehaviourUpdater.CallbackInterfacesDefualts.Concat(customCallbackInterfaces))
			{
				List<SimulationBehaviourUpdater.BehaviourList> list = new List<SimulationBehaviourUpdater.BehaviourList>();
				for (int i = 0; i < this._inOrderList.Count; i++)
				{
					SimulationBehaviourUpdater.BehaviourList behaviourList = this._inOrderList[i];
					bool flag2 = type2.IsAssignableFrom(behaviourList.Type);
					if (flag2)
					{
						list.Add(behaviourList);
					}
				}
				this._inOrderByInterfaceList.Add(type2, list);
			}
		}

		public void InvokeRender()
		{
			int count = this._inOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					SimulationBehaviourUpdater.BehaviourList behaviourList = this._inOrderList[i];
					SimulationBehaviour simulationBehaviour = behaviourList.Head;
					int num = 0;
					Timer timer = Timer.StartNew();
					while (BehaviourUtils.IsNotNull(simulationBehaviour))
					{
						bool canReceiveRenderCallback = simulationBehaviour.CanReceiveRenderCallback;
						if (canReceiveRenderCallback)
						{
							simulationBehaviour.PreRender();
							simulationBehaviour.Render();
							num++;
						}
						simulationBehaviour = simulationBehaviour.Next;
					}
					timer.Stop();
					behaviourList.BehaviourStats.PendingSnapshot.AccumulateRenderExecutionCount(num);
					behaviourList.BehaviourStats.PendingSnapshot.AccumulateRenderExecutionTime(timer.ElapsedInMilliseconds);
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

		public int GetCallbackCount(Type type)
		{
			return this._inOrderByInterfaceList[type].Count;
		}

		public SimulationBehaviourListScope GetCallbackHead(Type type, int index, out SimulationBehaviour head)
		{
			SimulationBehaviourUpdater.BehaviourList behaviourList = this._inOrderByInterfaceList[type][index];
			head = behaviourList.Head;
			return new SimulationBehaviourListScope(behaviourList);
		}

		public void GetAllSimulationBehaviours(List<SimulationBehaviour> allSb)
		{
			Type typeFromHandle = typeof(NetworkBehaviour);
			for (int i = 0; i < this._inOrderList.Count; i++)
			{
				SimulationBehaviourUpdater.BehaviourList behaviourList = this._inOrderList[i];
				SimulationBehaviour simulationBehaviour = behaviourList.Head;
				while (BehaviourUtils.IsNotNull(simulationBehaviour))
				{
					bool flag = !typeFromHandle.IsInstanceOfType(simulationBehaviour);
					if (flag)
					{
						allSb.Add(simulationBehaviour);
					}
					simulationBehaviour = simulationBehaviour.Next;
				}
			}
		}

		public void InvokeFixedUpdateNetwork(SimulationStages stage, SimulationModes mode, Topologies topology)
		{
			EngineProfiler.Begin("SimulationBehaviourUpdater.InvokeFixedUpdateNetwork");
			int count = this._inOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					SimulationBehaviourUpdater.BehaviourList behaviourList = this._inOrderList[i];
					Timer timer = Timer.StartNew();
					int num = 0;
					bool flag = (behaviourList.Modes & mode) == mode && (behaviourList.Stages & stage) == stage && (behaviourList.Topologies & topology) == topology;
					if (flag)
					{
						SimulationBehaviour simulationBehaviour = behaviourList.Head;
						Assert.Check(behaviourList.LockCount == 0);
						behaviourList.LockCount++;
						try
						{
							while (BehaviourUtils.IsNotNull(simulationBehaviour))
							{
								SimulationBehaviour next = simulationBehaviour.Next;
								bool flag2 = (simulationBehaviour.Flags & SimulationBehaviourRuntimeFlags.SkipNextUpdate) > (SimulationBehaviourRuntimeFlags)0;
								if (flag2)
								{
									simulationBehaviour.Flags &= ~SimulationBehaviourRuntimeFlags.SkipNextUpdate;
								}
								else
								{
									bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
									if (canReceiveSimulationCallback)
									{
										simulationBehaviour.FixedUpdateNetwork();
										num++;
									}
								}
								simulationBehaviour = next;
							}
						}
						finally
						{
							SimulationBehaviourUpdater.BehaviourList behaviourList2 = behaviourList;
							int num2 = behaviourList2.LockCount - 1;
							behaviourList2.LockCount = num2;
							bool flag3 = num2 == 0;
							if (flag3)
							{
								behaviourList.RemoveAllPending();
							}
							timer.Stop();
							behaviourList.BehaviourStats.PendingSnapshot.AccumulateFixedUpdateNetworkExecutionCount(num);
							behaviourList.BehaviourStats.PendingSnapshot.AccumulateFixedUpdateNetworkExecutionTime(timer.ElapsedInMilliseconds);
						}
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
			EngineProfiler.End();
		}

		public void AddObject(NetworkRunner runner, NetworkObject obj, bool skipFirstCall, bool isInSimulation)
		{
			Assert.Check(BehaviourUtils.IsSameNotNull(obj.Runner, runner));
			Assert.Check((obj.RuntimeFlags & NetworkObjectRuntimeFlags.InSimulation) == NetworkObjectRuntimeFlags.None);
			if (isInSimulation)
			{
				obj.RuntimeFlags |= NetworkObjectRuntimeFlags.InSimulation;
			}
			for (int i = 0; i < obj.NetworkedBehaviours.Length; i++)
			{
				Assert.Check(BehaviourUtils.IsSame(obj.NetworkedBehaviours[i].Object, obj));
				Assert.Check(BehaviourUtils.IsSame(obj.NetworkedBehaviours[i].Runner, runner));
				this.AddBehaviour(obj.NetworkedBehaviours[i], skipFirstCall);
				if (isInSimulation)
				{
					obj.NetworkedBehaviours[i].Flags |= SimulationBehaviourRuntimeFlags.InSimulation;
					ISimulationEnter simulationEnter = obj.NetworkedBehaviours[i] as ISimulationEnter;
					bool flag = simulationEnter != null;
					if (flag)
					{
						try
						{
							simulationEnter.SimulationEnter();
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
		}

		public void AddBehaviour(SimulationBehaviour behaviour, bool skipFirstCall)
		{
			this.CheckSimulationBehaviourForNetworkedAttribute(behaviour.GetType());
			if (skipFirstCall)
			{
				behaviour.Flags |= SimulationBehaviourRuntimeFlags.SkipNextUpdate;
			}
			else
			{
				behaviour.Flags &= ~SimulationBehaviourRuntimeFlags.SkipNextUpdate;
			}
			SimulationBehaviourUpdater.BehaviourList behaviourList = this.FindList(behaviour.GetType());
			bool flag = behaviourList.IsInList(behaviour);
			if (flag)
			{
				TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
				if (logTraceObject != null)
				{
					logTraceObject.Warn(behaviour, string.Format("Not added {0}: already on the list {1}", BehaviourUtils.GetName(behaviour), LogUtils.GetDump<SimulationBehaviourUpdater.BehaviourList>(behaviourList)));
				}
			}
			else
			{
				SimulationBehaviour simulationBehaviour = behaviourList.Head;
				bool flag2 = BehaviourUtils.IsNotNull(behaviour.Object);
				if (flag2)
				{
					while (BehaviourUtils.IsNotNull(simulationBehaviour) && BehaviourUtils.IsNull(simulationBehaviour.Object))
					{
						simulationBehaviour = simulationBehaviour.Next;
					}
					while (BehaviourUtils.IsNotNull(simulationBehaviour) && BehaviourUtils.IsNotNull(simulationBehaviour.Next))
					{
						bool flag3 = simulationBehaviour.Object.Id == behaviour.Object.Id;
						if (flag3)
						{
							behaviourList.AddAfter(behaviour, simulationBehaviour);
							return;
						}
						bool flag4 = behaviourList.PendingRemovals != null && behaviourList.PendingRemovals.Contains(simulationBehaviour.Next);
						if (flag4)
						{
							simulationBehaviour = simulationBehaviour.Next.Next;
						}
						else
						{
							bool flag5 = simulationBehaviour.Object.Id.Raw < behaviour.Object.Id.Raw && behaviour.Object.Id.Raw <= simulationBehaviour.Next.Object.Id.Raw;
							if (flag5)
							{
								behaviourList.AddAfter(behaviour, simulationBehaviour);
								return;
							}
							simulationBehaviour = simulationBehaviour.Next;
						}
					}
					bool flag6 = BehaviourUtils.IsNull(simulationBehaviour) || BehaviourUtils.IsNull(simulationBehaviour.Next);
					if (flag6)
					{
						behaviourList.AddLast(behaviour);
					}
				}
				else
				{
					string name = behaviour.GetType().Name;
					while (BehaviourUtils.IsNotNull(simulationBehaviour) && BehaviourUtils.IsNotNull(simulationBehaviour.Next))
					{
						bool flag7 = BehaviourUtils.IsNotNull(simulationBehaviour.Object);
						if (flag7)
						{
							break;
						}
						bool flag8 = BehaviourUtils.IsNotNull(simulationBehaviour.Next.Object);
						if (flag8)
						{
							behaviourList.AddAfter(behaviour, simulationBehaviour);
							return;
						}
						int num = string.CompareOrdinal(simulationBehaviour.GetType().Name, name);
						int num2 = string.CompareOrdinal(name, simulationBehaviour.Next.GetType().Name);
						bool flag9 = num < 0 && num2 <= 0;
						if (flag9)
						{
							behaviourList.AddAfter(behaviour, simulationBehaviour);
							return;
						}
						simulationBehaviour = simulationBehaviour.Next;
					}
					bool flag10 = BehaviourUtils.IsNull(simulationBehaviour) || BehaviourUtils.IsNull(simulationBehaviour.Next);
					if (flag10)
					{
						behaviourList.AddFirst(behaviour);
					}
				}
			}
		}

		private void CheckSimulationBehaviourForNetworkedAttribute(Type type)
		{
			bool flag = this._behavioursChecked.Contains(type);
			if (!flag)
			{
				this._behavioursChecked.Add(type);
				bool flag2 = !typeof(NetworkBehaviour).IsAssignableFrom(type);
				if (flag2)
				{
					foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
					{
						Attribute customAttribute = propertyInfo.GetCustomAttribute(typeof(NetworkedAttribute));
						bool flag3 = customAttribute != null;
						if (flag3)
						{
							LogStream logError = InternalLogStreams.LogError;
							if (logError != null)
							{
								logError.Log(string.Concat(new string[]
								{
									"[Networked] attribute found on property ",
									propertyInfo.Name,
									" on ",
									type.FullName,
									". [Networked] properties are only supported on types inheriting from NetworkBehaviour."
								}));
							}
						}
					}
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
					{
						Attribute customAttribute2 = methodInfo.GetCustomAttribute(typeof(RpcAttribute));
						bool flag4 = customAttribute2 != null;
						if (flag4)
						{
							LogStream logError2 = InternalLogStreams.LogError;
							if (logError2 != null)
							{
								logError2.Log(string.Concat(new string[]
								{
									"[Rpc] attribute found on method ",
									methodInfo.Name,
									" on ",
									type.FullName,
									". [Rpc] methods are only supported on types inheriting from NetworkBehaviour."
								}));
							}
						}
					}
				}
			}
		}

		public void RemoveBehaviour(SimulationBehaviour behaviour)
		{
			SimulationBehaviourUpdater.BehaviourList behaviourList = this.FindList(behaviour.GetType());
			Assert.Check((behaviour.Flags & SimulationBehaviourRuntimeFlags.PendingRemoval) == (SimulationBehaviourRuntimeFlags)0);
			bool flag = behaviourList.LockCount > 0;
			if (flag)
			{
				TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
				if (logTraceObject != null)
				{
					logTraceObject.Log(behaviour, string.Format("Pending removal of {0} {1}", BehaviourUtils.GetName(behaviour), LogUtils.GetDump<SimulationBehaviourUpdater.BehaviourList>(behaviourList)));
				}
				behaviour.Flags |= SimulationBehaviourRuntimeFlags.PendingRemoval;
				behaviourList.PendingRemove(behaviour);
			}
			else
			{
				TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
				if (logTraceObject2 != null)
				{
					logTraceObject2.Log(behaviour, string.Format("Removing {0} {1}", BehaviourUtils.GetName(behaviour), LogUtils.GetDump<SimulationBehaviourUpdater.BehaviourList>(behaviourList)));
				}
				behaviourList.Remove(behaviour);
			}
		}

		public SimulationBehaviour[] GetTypeHeads(Type type)
		{
			ValueTuple<SimulationBehaviour[], Type[]> valueTuple;
			bool flag = !this._byTypeHierarchy.TryGetValue(type, out valueTuple);
			if (flag)
			{
				List<Type> list = new List<Type>();
				for (int i = 0; i < this._inOrderList.Count; i++)
				{
					bool flag2 = type.IsAssignableFrom(this._inOrderList[i].Type);
					if (flag2)
					{
						list.Add(this._inOrderList[i].Type);
					}
				}
				Dictionary<Type, ValueTuple<SimulationBehaviour[], Type[]>> byTypeHierarchy = this._byTypeHierarchy;
				valueTuple = new ValueTuple<SimulationBehaviour[], Type[]>(new SimulationBehaviour[list.Count], list.ToArray());
				byTypeHierarchy.Add(type, valueTuple);
			}
			ValueTuple<SimulationBehaviour[], Type[]> valueTuple2 = valueTuple;
			SimulationBehaviour[] item = valueTuple2.Item1;
			Type[] item2 = valueTuple2.Item2;
			Assert.Check(item.Length == item2.Length);
			for (int j = 0; j < item2.Length; j++)
			{
				item[j] = this.FindList(item2[j]).Head;
			}
			return item;
		}

		private void AddType(Type type, ValueTuple<SimulationModes, SimulationStages, Topologies> attr)
		{
			bool flag = !type.IsAbstract;
			if (flag)
			{
				bool flag2 = typeof(NetworkBehaviour).IsAssignableFrom(type);
				if (flag2)
				{
					NetworkBehaviourUtils.RegisterRpcInvokeDelegates(type);
					NetworkBehaviourUtils.RegisterMetaData(type);
				}
				else
				{
					bool flag3 = typeof(SimulationBehaviour).IsAssignableFrom(type);
					if (flag3)
					{
						NetworkBehaviourUtils.RegisterRpcInvokeDelegates(type);
					}
				}
			}
			SimulationModes item = attr.Item1;
			SimulationStages item2 = attr.Item2;
			Topologies item3 = attr.Item3;
			SimulationBehaviourUpdater.BehaviourList behaviourList = new SimulationBehaviourUpdater.BehaviourList();
			behaviourList.Type = type;
			behaviourList.Modes = item;
			behaviourList.Stages = item2;
			behaviourList.Topologies = item3;
			behaviourList.ExecutionOrder = this.GetExecutionOrder(type);
			this._byTypeLookup.Add(type, behaviourList);
			this._inOrderList.Add(behaviourList);
		}

		private SimulationBehaviourUpdater.BehaviourList FindList(Type type)
		{
			SimulationBehaviourUpdater.BehaviourList behaviourList;
			bool flag = this._byTypeLookup.TryGetValue(type, out behaviourList);
			if (!flag)
			{
				Type key = type;
				while (typeof(SimulationBehaviour).IsAssignableFrom(type))
				{
					bool flag2 = this._byTypeLookup.TryGetValue(type, out behaviourList);
					if (flag2)
					{
						this._byTypeLookup.Add(key, behaviourList);
						return behaviourList;
					}
					type = type.BaseType;
				}
				string format = "{0} or any of its base-classes found in _byTypeLookup: {1}";
				object arg = type;
				string separator = ", ";
				string[] array = new string[1];
				array[0] = (from x in this._byTypeLookup
				select x.Key.ToString()).ToString();
				throw new InvalidOperationException(string.Format(format, arg, string.Join(separator, array)));
			}
			return behaviourList;
		}

		[Conditional("DEBUG")]
		public void FinishBehaviourStatisticsPendingSnapshot()
		{
			foreach (SimulationBehaviourUpdater.BehaviourList behaviourList in this._inOrderList)
			{
				behaviourList.BehaviourStats.FinishPendingSnapshot();
			}
		}

		public bool TryGetBehaviourStatisticsSnapshot(Type behaviourType, out BehaviourStatisticsSnapshot behaviourStatisticsSnapshot)
		{
			SimulationBehaviourUpdater.BehaviourList behaviourList;
			bool flag = this._byTypeLookup.TryGetValue(behaviourType, out behaviourList);
			bool result;
			if (flag)
			{
				behaviourStatisticsSnapshot = behaviourList.BehaviourStats.CompletedSnapshot;
				result = true;
			}
			else
			{
				behaviourStatisticsSnapshot = null;
				result = false;
			}
			return result;
		}

		private readonly Dictionary<Type, SimulationBehaviourUpdater.BehaviourList> _byTypeLookup;

		private readonly Dictionary<Type, ValueTuple<SimulationBehaviour[], Type[]>> _byTypeHierarchy;

		private readonly List<SimulationBehaviourUpdater.BehaviourList> _inOrderList;

		private readonly Dictionary<Type, List<SimulationBehaviourUpdater.BehaviourList>> _inOrderByInterfaceList;

		private readonly HashSet<Type> _behavioursChecked;

		private readonly NetworkProjectConfig _config;

		internal class BehaviourList : ILogDumpable
		{
			public void AddAfter(SimulationBehaviour item, SimulationBehaviour after)
			{
				Assert.Check(this.IsInList(after));
				Assert.Check(!this.IsInList(item));
				Assert.Check((item.Flags & SimulationBehaviourRuntimeFlags.PendingRemoval) == (SimulationBehaviourRuntimeFlags)0);
				bool flag = BehaviourUtils.IsSame(after, this.Tail);
				if (flag)
				{
					this.AddLast(item);
				}
				else
				{
					Assert.Check(BehaviourUtils.IsNotNull(after.Next));
					item.Next = after.Next;
					item.Prev = after;
					after.Next.Prev = item;
					after.Next = item;
				}
				Assert.Check(this.IsInList(after));
				Assert.Check(this.IsInList(item));
			}

			public void AddFirst(SimulationBehaviour item)
			{
				Assert.Check(!this.IsInList(item));
				item.Next = this.Head;
				item.Prev = null;
				bool flag = BehaviourUtils.IsNotNull(this.Head);
				if (flag)
				{
					this.Head.Prev = item;
					this.Head = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
			}

			public void AddLast(SimulationBehaviour item)
			{
				Assert.Check(BehaviourUtils.IsNull(item.Prev));
				Assert.Check(BehaviourUtils.IsNull(item.Next));
				Assert.Check(!this.IsInList(item));
				Assert.Check((item.Flags & SimulationBehaviourRuntimeFlags.PendingRemoval) == (SimulationBehaviourRuntimeFlags)0);
				item.Next = null;
				item.Prev = this.Tail;
				bool flag = BehaviourUtils.IsNotNull(this.Tail);
				if (flag)
				{
					this.Tail.Next = item;
					this.Tail = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
			}

			public void RemoveAllPending()
			{
				Assert.Check(this.LockCount == 0);
				bool flag = this.PendingRemovals == null || this.PendingRemovals.Count == 0;
				if (!flag)
				{
					foreach (SimulationBehaviour item in this.PendingRemovals)
					{
						this.Remove(item);
					}
					this.PendingRemovals.Clear();
				}
			}

			public void PendingRemove(SimulationBehaviour item)
			{
				Assert.Check(this.IsInList(item));
				Assert.Check(this.LockCount > 0);
				bool flag = this.PendingRemovals == null;
				if (flag)
				{
					this.PendingRemovals = new List<SimulationBehaviour>();
				}
				this.PendingRemovals.Add(item);
			}

			public void Remove(SimulationBehaviour item)
			{
				bool flag = !this.IsInList(item);
				if (flag)
				{
					TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
					if (logTraceObject != null)
					{
						logTraceObject.Warn(item, string.Format("Not in list {0}", BehaviourUtils.GetName(item)));
					}
				}
				else
				{
					bool flag2 = BehaviourUtils.IsNotNull(item.Prev);
					if (flag2)
					{
						item.Prev.Next = item.Next;
					}
					bool flag3 = BehaviourUtils.IsNotNull(item.Next);
					if (flag3)
					{
						item.Next.Prev = item.Prev;
					}
					bool flag4 = BehaviourUtils.IsSame(item, this.Tail);
					if (flag4)
					{
						this.Tail = item.Prev;
					}
					bool flag5 = BehaviourUtils.IsSame(item, this.Head);
					if (flag5)
					{
						this.Head = item.Next;
					}
					item.Prev = null;
					item.Next = null;
					item.Flags &= ~SimulationBehaviourRuntimeFlags.PendingRemoval;
				}
			}

			public bool IsInList(SimulationBehaviour item)
			{
				SimulationBehaviour simulationBehaviour = this.Head;
				while (BehaviourUtils.IsNotNull(simulationBehaviour))
				{
					bool flag = BehaviourUtils.IsSame(simulationBehaviour, item);
					if (flag)
					{
						return true;
					}
					simulationBehaviour = simulationBehaviour.Next;
				}
				return false;
			}

			void ILogDumpable.Dump(StringBuilder builder)
			{
				builder.Append("[Type: ").Append(this.Type.Name).Append(", List: ");
				SimulationBehaviour simulationBehaviour = this.Head;
				while (!BehaviourUtils.IsNull(simulationBehaviour))
				{
					bool flag = !BehaviourUtils.IsSame(simulationBehaviour, this.Head);
					if (flag)
					{
						builder.Append("->");
					}
					bool flag2 = !simulationBehaviour.CanReceiveRenderCallback;
					if (flag2)
					{
						builder.Append("[x]");
					}
					builder.Append(BehaviourUtils.GetName(simulationBehaviour));
					simulationBehaviour = simulationBehaviour.Next;
				}
				builder.Append("]");
			}

			public Type Type;

			public int ExecutionOrder;

			public SimulationModes Modes;

			public SimulationStages Stages;

			public Topologies Topologies;

			public SimulationBehaviour Head;

			public SimulationBehaviour Tail;

			public int LockCount;

			public List<SimulationBehaviour> PendingRemovals;

			public BehaviourStatisticsManager BehaviourStats = new BehaviourStatisticsManager();
		}
	}
}
