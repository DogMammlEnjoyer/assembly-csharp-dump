using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion.LagCompensation;
using Fusion.Statistics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Fusion
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Fusion/Lag Compensation/Hitbox Manager")]
	[DefaultExecutionOrder(2000)]
	public sealed class HitboxManager : SimulationBehaviour, IAfterTick, IPublicFacingInterface, IBeforeSimulation, ISpawned
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Raycast(Vector3 origin, Vector3 direction, float length, PlayerRef player, out LagCompensatedHit hit, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			int? tick;
			int? tickTo;
			float? alpha;
			this.GetPlayerTickAndAlpha(player, out tick, out tickTo, out alpha);
			this._raycastQuery.Player = player;
			this._raycastQuery.Origin = origin;
			this._raycastQuery.Direction = direction;
			this._raycastQuery.Length = length;
			this._raycastQuery.Tick = tick;
			this._raycastQuery.TickTo = tickTo;
			this._raycastQuery.Alpha = alpha;
			this._raycastQuery.LayerMask = layerMask;
			this._raycastQuery.Options = options;
			this._raycastQuery.TriggerInteraction = queryTriggerInteraction;
			this._raycastQuery.PreProcessingDelegate = preProcessRoots;
			this._raycastHits.Clear();
			bool flag = this.QueryInternal(this._raycastQuery, this._raycastHits, false) <= 0;
			bool result;
			if (flag)
			{
				hit = default(LagCompensatedHit);
				result = false;
			}
			else
			{
				hit = HitboxManager.GetClosestHit(this._raycastHits);
				result = true;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Raycast(Vector3 origin, Vector3 direction, float length, int tick, int? tickTo, float? alpha, out LagCompensatedHit hit, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			this._raycastQuery.Player = default(PlayerRef);
			this._raycastQuery.Origin = origin;
			this._raycastQuery.Direction = direction;
			this._raycastQuery.Length = length;
			this._raycastQuery.Tick = new int?(tick);
			this._raycastQuery.TickTo = tickTo;
			this._raycastQuery.Alpha = alpha;
			this._raycastQuery.LayerMask = layerMask;
			this._raycastQuery.Options = options;
			this._raycastQuery.TriggerInteraction = queryTriggerInteraction;
			this._raycastQuery.PreProcessingDelegate = preProcessRoots;
			this._raycastHits.Clear();
			bool flag = this.QueryInternal(this._raycastQuery, this._raycastHits, false) <= 0;
			bool result;
			if (flag)
			{
				hit = default(LagCompensatedHit);
				result = false;
			}
			else
			{
				hit = HitboxManager.GetClosestHit(this._raycastHits);
				result = true;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int RaycastAll(Vector3 origin, Vector3 direction, float length, PlayerRef player, List<LagCompensatedHit> hits, int layerMask = -1, bool clearHits = true, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			int? tick;
			int? tickTo;
			float? alpha;
			this.GetPlayerTickAndAlpha(player, out tick, out tickTo, out alpha);
			this._raycastAllQuery.Player = player;
			this._raycastAllQuery.Origin = origin;
			this._raycastAllQuery.Direction = direction;
			this._raycastAllQuery.Length = length;
			this._raycastAllQuery.Tick = tick;
			this._raycastAllQuery.TickTo = tickTo;
			this._raycastAllQuery.Alpha = alpha;
			this._raycastAllQuery.LayerMask = layerMask;
			this._raycastAllQuery.Options = options;
			this._raycastAllQuery.TriggerInteraction = queryTriggerInteraction;
			this._raycastAllQuery.PreProcessingDelegate = preProcessRoots;
			return this.QueryInternal(this._raycastAllQuery, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int RaycastAll(Vector3 origin, Vector3 direction, float length, int tick, int? tickTo, float? alpha, List<LagCompensatedHit> hits, int layerMask = -1, bool clearHits = true, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			this._raycastAllQuery.Player = default(PlayerRef);
			this._raycastAllQuery.Origin = origin;
			this._raycastAllQuery.Direction = direction;
			this._raycastAllQuery.Length = length;
			this._raycastAllQuery.Tick = new int?(tick);
			this._raycastAllQuery.TickTo = tickTo;
			this._raycastAllQuery.Alpha = alpha;
			this._raycastAllQuery.LayerMask = layerMask;
			this._raycastAllQuery.Options = options;
			this._raycastAllQuery.TriggerInteraction = queryTriggerInteraction;
			this._raycastAllQuery.PreProcessingDelegate = preProcessRoots;
			return this.QueryInternal(this._raycastAllQuery, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int OverlapSphere(Vector3 origin, float radius, PlayerRef player, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			int? tick;
			int? tickTo;
			float? alpha;
			this.GetPlayerTickAndAlpha(player, out tick, out tickTo, out alpha);
			this._sphereOverlapQuery.Player = player;
			this._sphereOverlapQuery.Center = origin;
			this._sphereOverlapQuery.Radius = radius;
			this._sphereOverlapQuery.Tick = tick;
			this._sphereOverlapQuery.TickTo = tickTo;
			this._sphereOverlapQuery.Alpha = alpha;
			this._sphereOverlapQuery.LayerMask = layerMask;
			this._sphereOverlapQuery.Options = options;
			this._sphereOverlapQuery.TriggerInteraction = queryTriggerInteraction;
			this._sphereOverlapQuery.PreProcessingDelegate = preProcessRoots;
			return this.QueryInternal(this._sphereOverlapQuery, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int OverlapSphere(Vector3 origin, float radius, int tick, int? tickTo, float? alpha, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			this._sphereOverlapQuery.Player = default(PlayerRef);
			this._sphereOverlapQuery.Radius = radius;
			this._sphereOverlapQuery.Tick = new int?(tick);
			this._sphereOverlapQuery.TickTo = tickTo;
			this._sphereOverlapQuery.Alpha = alpha;
			this._sphereOverlapQuery.LayerMask = layerMask;
			this._sphereOverlapQuery.Options = options;
			this._sphereOverlapQuery.TriggerInteraction = queryTriggerInteraction;
			this._sphereOverlapQuery.PreProcessingDelegate = preProcessRoots;
			return this.QueryInternal(this._sphereOverlapQuery, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int OverlapBox(Vector3 center, Vector3 extents, Quaternion orientation, PlayerRef player, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			int? tick;
			int? tickTo;
			float? alpha;
			this.GetPlayerTickAndAlpha(player, out tick, out tickTo, out alpha);
			this._boxOverlapQuery.Player = player;
			this._boxOverlapQuery.Center = center;
			this._boxOverlapQuery.Extents = extents;
			this._boxOverlapQuery.Rotation = orientation;
			this._boxOverlapQuery.Tick = tick;
			this._boxOverlapQuery.TickTo = tickTo;
			this._boxOverlapQuery.Alpha = alpha;
			this._boxOverlapQuery.LayerMask = layerMask;
			this._boxOverlapQuery.Options = options;
			this._boxOverlapQuery.TriggerInteraction = queryTriggerInteraction;
			this._boxOverlapQuery.PreProcessingDelegate = preProcessRoots;
			return this.QueryInternal(this._boxOverlapQuery, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int OverlapBox(Vector3 center, Vector3 extents, Quaternion orientation, int tick, int? tickTo, float? alpha, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
		{
			this._boxOverlapQuery.Player = default(PlayerRef);
			this._boxOverlapQuery.Center = center;
			this._boxOverlapQuery.Extents = extents;
			this._boxOverlapQuery.Rotation = orientation;
			this._boxOverlapQuery.Tick = new int?(tick);
			this._boxOverlapQuery.TickTo = tickTo;
			this._boxOverlapQuery.Alpha = alpha;
			this._boxOverlapQuery.LayerMask = layerMask;
			this._boxOverlapQuery.Options = options;
			this._boxOverlapQuery.TriggerInteraction = queryTriggerInteraction;
			this._boxOverlapQuery.PreProcessingDelegate = preProcessRoots;
			return this.QueryInternal(this._boxOverlapQuery, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PositionRotation(Hitbox hitbox, int tick, out Vector3 position, out Quaternion rotation, bool subtickAccuracy = false, int? tickTo = null, float? alpha = null)
		{
			HitOptions options = subtickAccuracy ? HitOptions.SubtickAccuracy : HitOptions.None;
			QueryParams queryParams = new QueryParams
			{
				Tick = tick,
				TickTo = tickTo,
				Alpha = alpha,
				Options = options
			};
			PositionRotationQueryParams positionRotationQueryParams = new PositionRotationQueryParams
			{
				Hitbox = hitbox,
				QueryParams = queryParams
			};
			this.PositionRotationInternal(ref positionRotationQueryParams, out position, out rotation);
		}

		public void PositionRotation(Hitbox hitbox, PlayerRef player, out Vector3 position, out Quaternion rotation, bool subTickAccuracy = false)
		{
			int? num;
			int? tickTo;
			float? alpha;
			this.GetPlayerTickAndAlpha(player, out num, out tickTo, out alpha);
			HitOptions options = subTickAccuracy ? HitOptions.SubtickAccuracy : HitOptions.None;
			QueryParams queryParams = new QueryParams
			{
				Player = player,
				Tick = num.Value,
				TickTo = tickTo,
				Alpha = alpha,
				Options = options
			};
			PositionRotationQueryParams positionRotationQueryParams = new PositionRotationQueryParams
			{
				Hitbox = hitbox,
				QueryParams = queryParams
			};
			this.PositionRotationInternal(ref positionRotationQueryParams, out position, out rotation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static LagCompensatedHit GetClosestHit(List<LagCompensatedHit> hits)
		{
			Assert.Check(hits != null);
			Assert.Check<int>(hits.Count > 0, hits.Count);
			int index = 0;
			float num = hits[0].Distance;
			for (int i = 1; i < hits.Count; i++)
			{
				float distance = hits[i].Distance;
				bool flag = distance < num;
				if (flag)
				{
					num = distance;
					index = i;
				}
			}
			return hits[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Raycast(RaycastQuery query, out LagCompensatedHit hit)
		{
			bool flag = query.Tick == null;
			if (flag)
			{
				this.GetPlayerTickAndAlpha(query.Player, out query.Tick, out query.TickTo, out query.Alpha);
			}
			this._raycastHits.Clear();
			bool flag2 = this.QueryInternal(query, this._raycastHits, false) <= 0;
			bool result;
			if (flag2)
			{
				hit = default(LagCompensatedHit);
				result = false;
			}
			else
			{
				hit = HitboxManager.GetClosestHit(this._raycastHits);
				result = true;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int RaycastAll(RaycastAllQuery query, List<LagCompensatedHit> hits, bool clearHits = true)
		{
			bool flag = query.Tick == null;
			if (flag)
			{
				this.GetPlayerTickAndAlpha(query.Player, out query.Tick, out query.TickTo, out query.Alpha);
			}
			return this.QueryInternal(query, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int OverlapSphere(SphereOverlapQuery query, List<LagCompensatedHit> hits, bool clearHits = true)
		{
			bool flag = query.Tick == null;
			if (flag)
			{
				this.GetPlayerTickAndAlpha(query.Player, out query.Tick, out query.TickTo, out query.Alpha);
			}
			return this.QueryInternal(query, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int OverlapBox(BoxOverlapQuery query, List<LagCompensatedHit> hits, bool clearHits = true)
		{
			bool flag = query.Tick == null;
			if (flag)
			{
				this.GetPlayerTickAndAlpha(query.Player, out query.Tick, out query.TickTo, out query.Alpha);
			}
			return this.QueryInternal(query, hits, clearHits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void GetPlayerTickAndAlpha(PlayerRef player, out int? tickFrom, out int? tickTo, out float? alpha)
		{
			SimulationInput inputForPlayer = base.Runner.Simulation.GetInputForPlayer(player);
			bool flag = inputForPlayer == null;
			if (flag)
			{
				HitboxBuffer hitboxBuffer = this._hitboxBuffer;
				tickFrom = new int?((hitboxBuffer != null) ? hitboxBuffer.Current.Tick : base.Runner.Simulation.Tick);
				tickTo = null;
				alpha = null;
			}
			else
			{
				bool isClient = base.Runner.IsClient;
				if (isClient)
				{
					tickFrom = new int?(inputForPlayer.Header->Tick);
					tickTo = null;
					alpha = null;
				}
				else
				{
					tickFrom = new int?(inputForPlayer.Header->InterpFrom);
					tickTo = new int?(inputForPlayer.Header->InterpTo);
					alpha = new float?(inputForPlayer.Header->InterpAlpha);
				}
			}
			Assert.Check(tickFrom != null);
		}

		public LagCompensationStatisticsSnapshot GetStatisticsSnapshot()
		{
			return this._lagCompStatManager.CompletedSnapshot;
		}

		private int QueryInternal(Query query, List<LagCompensatedHit> hits, bool clearHits)
		{
			bool flag = base.Runner.Topology == Topologies.Shared;
			int result;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log("Lag Compensation is not supported in Shared Mode.");
				}
				result = 0;
			}
			else
			{
				if (clearHits)
				{
					hits.Clear();
				}
				Assert.Check(query.Tick != null);
				this._lagCompensatedHits.Clear();
				this._hitboxBuffer.PerformQuery(query, this._lagCompensatedHits);
				int count = hits.Count;
				for (int i = 0; i < this._lagCompensatedHits.Count; i++)
				{
					HitboxHit hitboxHit = this._lagCompensatedHits[i];
					hits.Add(LagCompensatedHit.FromHitboxHit(ref hitboxHit));
				}
				bool flag2 = (query.Options & HitOptions.IncludePhysX) != HitOptions.None || (query.Options & HitOptions.IncludeBox2D) > HitOptions.None;
				if (flag2)
				{
					query.PerformStaticQuery(base.Runner, hits, query.Options);
				}
				result = hits.Count - count;
			}
			return result;
		}

		private void PositionRotationInternal(ref PositionRotationQueryParams param, out Vector3 position, out Quaternion rotation)
		{
			bool isClient = base.Runner.IsClient;
			if (isClient)
			{
				param.QueryParams.TickTo = null;
				param.QueryParams.Alpha = null;
			}
			else
			{
				bool flag = (param.QueryParams.Options & HitOptions.SubtickAccuracy) == HitOptions.None && param.QueryParams.Alpha != null && param.QueryParams.Alpha.Value > 0.5f;
				if (flag)
				{
					param.QueryParams.Tick = param.QueryParams.Tick + 1;
				}
			}
			this._hitboxBuffer.PositionQueryInternal(ref param, out position, out rotation);
		}

		private void Init()
		{
			this._settings = base.Runner.Config.LagCompensation;
			this.Init(this.GetObjects(base.Runner));
			this.InitQueries();
			this.DrawInfo = new LagCompensationDraw(this._hitboxBuffer);
		}

		private void InitQueries()
		{
			RaycastQueryParams raycastQueryParams = default(RaycastQueryParams);
			SphereOverlapQueryParams sphereOverlapQueryParams = default(SphereOverlapQueryParams);
			BoxOverlapQueryParams boxOverlapQueryParams = default(BoxOverlapQueryParams);
			int cachedStaticCollidersSize = this._settings.CachedStaticCollidersSize;
			this._raycastQuery = new RaycastQuery(ref raycastQueryParams);
			this._raycastAllQuery = new RaycastAllQuery(ref raycastQueryParams, new RaycastHit[cachedStaticCollidersSize], new RaycastHit2D[cachedStaticCollidersSize]);
			this._sphereOverlapQuery = new SphereOverlapQuery(ref sphereOverlapQueryParams, new Collider[cachedStaticCollidersSize], new Collider2D[cachedStaticCollidersSize]);
			this._boxOverlapQuery = new BoxOverlapQuery(ref boxOverlapQueryParams, new Collider[cachedStaticCollidersSize], new Collider2D[cachedStaticCollidersSize]);
		}

		private void Init(List<HitboxRoot> initialObjects)
		{
			int num = Mathf.Max(this._settings.HitboxBufferLengthInMs, 30);
			float f = (float)num * 0.001f * (float)base.Runner.Simulation.TickRate;
			int hitboxCapacity = (this._settings.HitboxDefaultCapacity < 16) ? 16 : this._settings.HitboxDefaultCapacity;
			this._hitboxBuffer = new HitboxBuffer(initialObjects, Mathf.CeilToInt(f), hitboxCapacity, this._settings.ExpansionFactor);
		}

		private List<HitboxRoot> GetObjects(NetworkRunner runner)
		{
			List<HitboxRoot> list = new List<HitboxRoot>();
			foreach (SimulationBehaviour simulationBehaviour in runner.GetAllBehaviours(typeof(HitboxRoot)))
			{
				while (BehaviourUtils.IsNotNull(simulationBehaviour))
				{
					bool canReceiveRenderCallback = simulationBehaviour.CanReceiveRenderCallback;
					if (canReceiveRenderCallback)
					{
						HitboxRoot hitboxRoot = (HitboxRoot)simulationBehaviour;
						hitboxRoot.Manager = this;
						list.Add(hitboxRoot);
					}
					simulationBehaviour = simulationBehaviour.Next;
				}
			}
			return list;
		}

		private void RegisterHitboxSnapshot(int tick, int dataTick)
		{
			bool isShutdown = base.Runner.IsShutdown;
			if (!isShutdown)
			{
				bool flag = this._hitboxBuffer == null;
				if (flag)
				{
					this.Init();
				}
				Assert.Check(this._hitboxBuffer != null);
				bool isServer = base.Runner.IsServer;
				if (isServer)
				{
					Profiler.BeginSample("Server Hitbox Manager");
					this.AdvanceAndRegister(tick, base.Runner.Simulation.Tick);
					Profiler.EndSample();
				}
				else
				{
					bool flag2 = base.Runner.Stage == SimulationStages.Resimulate;
					if (flag2)
					{
						return;
					}
					Profiler.BeginSample("Client Hitbox Manager");
					this.AdvanceAndRegister(tick, dataTick);
					Profiler.EndSample();
				}
				bool flag3 = this._hitboxBuffer.BVH != null;
				if (flag3)
				{
					this.BVHDepth = this._hitboxBuffer.BVH.maxDepth;
					this.BVHNodes = this._hitboxBuffer.BVH.UsedNodesCount;
				}
				this.TotalHitboxes = this._hitboxBuffer.Current.CollidersCount;
				this._lagCompStatManager.PendingSnapshot.SetBVHMaxDeep(this.BVHDepth, true);
				this._lagCompStatManager.PendingSnapshot.SetBVHNodeCount(this.BVHNodes, true);
				this._lagCompStatManager.PendingSnapshot.SetHitboxesCount(this.TotalHitboxes, true);
			}
		}

		private void AdvanceAndRegister(int tick, int dataTick)
		{
			base.Runner.InvokeOnBeforeHitboxRegistration();
			Timer timer = Timer.StartNew();
			this._hitboxBuffer.Advance(tick, dataTick);
			timer.Stop();
			this._lagCompStatManager.PendingSnapshot.SetAdvanceBufferTime(timer.ElapsedInMilliseconds, false);
			foreach (SimulationBehaviour simulationBehaviour in base.Runner.GetAllBehaviours(typeof(HitboxRoot)))
			{
				while (BehaviourUtils.IsNotNull(simulationBehaviour))
				{
					bool canReceiveRenderCallback = simulationBehaviour.CanReceiveRenderCallback;
					if (canReceiveRenderCallback)
					{
						HitboxRoot hitboxRoot = (HitboxRoot)simulationBehaviour;
						bool flag = !hitboxRoot.Registered;
						if (flag)
						{
							hitboxRoot.Manager = this;
							this._hitboxBuffer.Add(hitboxRoot, this._lagCompStatManager);
						}
						else
						{
							bool inInterest = hitboxRoot.InInterest;
							if (inInterest)
							{
								this._hitboxBuffer.Update(hitboxRoot, this._lagCompStatManager);
							}
						}
					}
					simulationBehaviour = simulationBehaviour.Next;
				}
			}
			Timer timer2 = Timer.StartNew();
			this._hitboxBuffer.PosUpdateRefit();
			timer2.Stop();
			this._lagCompStatManager.PendingSnapshot.SetRefitBVHTime(timer2.ElapsedInMilliseconds, false);
		}

		internal bool Remove(HitboxRoot root)
		{
			return this._hitboxBuffer.Remove(root);
		}

		void IAfterTick.AfterTick()
		{
			bool isServer = base.Runner.IsServer;
			if (isServer)
			{
				this.RegisterHitboxSnapshot(base.Runner.Simulation.Tick, base.Runner.Simulation.RemoteTickPrevious);
				this._lagCompStatManager.FinishPendingSnapshot();
			}
		}

		void IBeforeSimulation.BeforeSimulation(int forwardTickCount)
		{
			bool flag = !base.Runner.IsClient;
			if (!flag)
			{
				int tickStride = base.Runner.Simulation.TickStride;
				Tick b = base.Runner.Simulation.Tick.Next(forwardTickCount * tickStride);
				Tick tick = base.Runner.Simulation.Tick.Next(tickStride);
				while (tick <= b)
				{
					this.RegisterHitboxSnapshot(tick, base.Runner.Simulation.RemoteTickPrevious);
					tick = tick.Next(tickStride);
				}
				this._lagCompStatManager.FinishPendingSnapshot();
			}
		}

		void ISpawned.Spawned()
		{
			this.Init();
		}

		[ReadOnly]
		[InlineHelp]
		public int BVHDepth;

		[ReadOnly]
		[InlineHelp]
		public int BVHNodes;

		[ReadOnly]
		[InlineHelp]
		public int TotalHitboxes;

		[ReadOnly]
		[InlineHelp]
		public LagCompensationDraw DrawInfo;

		private readonly List<LagCompensatedHit> _raycastHits = new List<LagCompensatedHit>();

		private RaycastQuery _raycastQuery;

		private RaycastAllQuery _raycastAllQuery;

		private SphereOverlapQuery _sphereOverlapQuery;

		private BoxOverlapQuery _boxOverlapQuery;

		private LagCompensationSettings _settings;

		private HitboxBuffer _hitboxBuffer;

		private readonly List<HitboxHit> _lagCompensatedHits = new List<HitboxHit>();

		private LagCompensationStatisticsManager _lagCompStatManager = new LagCompensationStatisticsManager();
	}
}
