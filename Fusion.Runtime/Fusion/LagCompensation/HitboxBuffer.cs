using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion.Statistics;
using UnityEngine;

namespace Fusion.LagCompensation
{
	internal class HitboxBuffer
	{
		internal int Length
		{
			get
			{
				return this._buffer.Length;
			}
		}

		internal BVH BVH
		{
			get
			{
				return this._buffer[this._head]._broadphase as BVH;
			}
		}

		internal HitboxBuffer.HitboxSnapshot Current
		{
			get
			{
				return this._buffer[this._head];
			}
		}

		internal HitboxBuffer(List<HitboxRoot> initialObjects, int bufferSize, int hitboxCapacity, float expansionFactor)
		{
			bool flag = bufferSize <= 0;
			if (flag)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Warn(string.Format("Trying to initialize {0} with {1} length. Initiatizing with 1 instead.", "HitboxBuffer", bufferSize));
				}
				bufferSize = 1;
			}
			this._buffer = new HitboxBuffer.HitboxSnapshot[bufferSize];
			this._mapper = new Mapper();
			this._head = 0;
			this._advanced = 0;
			Assert.Check(this.Length > 0);
			hitboxCapacity = ((initialObjects != null) ? Math.Max(hitboxCapacity, initialObjects.Count) : hitboxCapacity);
			this._buffer[0] = new HitboxBuffer.HitboxSnapshot(this._mapper, initialObjects, hitboxCapacity, expansionFactor);
			for (int i = 1; i < this.Length; i++)
			{
				this._buffer[i] = new HitboxBuffer.HitboxSnapshot(this._mapper, null, hitboxCapacity, expansionFactor);
			}
		}

		internal void Advance(int tick, int dataTick)
		{
			bool flag = tick == this.Tick;
			int num;
			if (flag)
			{
				num = (this._head + this._buffer.Length - 1) % this._buffer.Length;
			}
			else
			{
				num = this._head;
				this._advanced++;
			}
			this._head = (num + 1) % this._buffer.Length;
			this._buffer[this._head].CopyFrom(tick, dataTick, this._buffer[num]);
			this.Tick = tick;
		}

		internal void PosUpdateRefit()
		{
			this.BVH.PosUpdateRefit();
		}

		internal void Add(HitboxRoot root, LagCompensationStatisticsManager lagCompStatManager)
		{
			this._buffer[this._head].Add(root, lagCompStatManager);
		}

		internal bool Remove(HitboxRoot root)
		{
			return this._buffer[this._head].Remove(root);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Update(HitboxRoot root, LagCompensationStatisticsManager lagCompStatManager)
		{
			this._buffer[this._head].Update(root, lagCompStatManager);
		}

		internal bool PerformQuery(Query query, List<HitboxHit> hits)
		{
			this._colliderCandidates.Clear();
			IHitboxColliderContainer hitboxColliderContainer;
			this.QueryBroadphase(query, this._colliderCandidates, out hitboxColliderContainer);
			bool flag = this._colliderCandidates.Count <= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.InitColliderCandidatesForNarrowPhase(hitboxColliderContainer, this._colliderCandidates);
				bool flag2 = query.NarrowPhase(hitboxColliderContainer, this._colliderCandidates, hits);
				hitboxColliderContainer.ReleaseTempColliders();
				result = flag2;
			}
			return result;
		}

		internal unsafe void PositionQueryInternal(ref PositionRotationQueryParams param, out Vector3 position, out Quaternion rotation)
		{
			HitboxBuffer.HitboxSnapshot hitboxSnapshot;
			this.GetClosestSnapshotForTick(param.QueryParams.Tick, out hitboxSnapshot);
			int colliderIndex = param.Hitbox.ColliderIndex;
			HitboxCollider hitboxCollider = *hitboxSnapshot.GetCollider(colliderIndex);
			bool flag = (param.QueryParams.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy && param.QueryParams.TickTo != null && param.QueryParams.Alpha != null;
			if (flag)
			{
				HitboxBuffer.HitboxSnapshot hitboxSnapshot2;
				this.GetClosestSnapshotForTick(param.QueryParams.TickTo.Value, out hitboxSnapshot2);
				HitboxCollider.Lerp(ref hitboxCollider, hitboxSnapshot2.GetCollider(colliderIndex), param.QueryParams.Alpha.Value, ref hitboxCollider);
			}
			position = hitboxCollider.Position;
			rotation = HitboxBuffer.QuaternionFromMatrix(hitboxCollider.LocalToWorld);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitColliderCandidatesForNarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates)
		{
			foreach (int index in candidates)
			{
				ref HitboxCollider collider = ref container.GetCollider(index);
				collider.InitNarrowData();
			}
		}

		internal static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}

		private void GetClosestSnapshotForTick(int tick, out HitboxBuffer.HitboxSnapshot snapshot)
		{
			int num = tick - this.Tick;
			bool flag = num > 0;
			if (flag)
			{
				snapshot = this._buffer[this._head];
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Warn(string.Format("Tick {0} is not in the Hitbox history, using closest instead: {1}. Buffer length: {2}, Buffer current tick: {3}", new object[]
					{
						tick,
						snapshot.Tick,
						this.Length,
						this.Tick
					}));
				}
			}
			else
			{
				bool flag2 = num < 1 - this.Length;
				if (flag2)
				{
					int num2 = (this._advanced >= this.Length) ? ((this._head + 1) % this.Length) : 1;
					snapshot = this._buffer[num2];
					DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
					if (logDebug2 != null)
					{
						logDebug2.Warn(string.Format("Tick {0} is not in the Hitbox history, using closest instead: {1}. Buffer length: {2}, Buffer current tick: {3}", new object[]
						{
							tick,
							snapshot.Tick,
							this.Length,
							this.Tick
						}));
					}
				}
				else
				{
					snapshot = this._buffer[(this._head + num + this.Length) % this.Length];
					Assert.Check(snapshot.Tick == tick, "The hitbox buffer seems to be missing the correct snapshot, make sure lag compensation is enabled in the network project config.");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void QueryBroadphase(Query query, HashSet<int> processedColliderIndices, out IHitboxColliderContainer container)
		{
			query.Tick = new int?(this.GetClosestTick(query));
			this._broadphaseCandidates.Clear();
			HitboxBuffer.HitboxSnapshot hitboxSnapshot;
			this.GetClosestSnapshotForTick(query.Tick.Value, out hitboxSnapshot);
			hitboxSnapshot.QueryBroadphase(query, this._broadphaseCandidates);
			bool flag = (query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy && query.TickTo != null && query.Alpha != null;
			if (flag)
			{
				HitboxBuffer.HitboxSnapshot hitboxSnapshot2;
				this.GetClosestSnapshotForTick(query.TickTo.Value, out hitboxSnapshot2);
				hitboxSnapshot2.QueryBroadphase(query, this._broadphaseCandidates);
				PreProcessingDelegate preProcessingDelegate = query.PreProcessingDelegate;
				if (preProcessingDelegate != null)
				{
					preProcessingDelegate(query, this._broadphaseCandidates, processedColliderIndices);
				}
				HitboxBuffer.HitboxSnapshot.ProcessBroadphaseRootCandidates(query, hitboxSnapshot, this._broadphaseCandidates, processedColliderIndices, hitboxSnapshot2);
			}
			else
			{
				PreProcessingDelegate preProcessingDelegate2 = query.PreProcessingDelegate;
				if (preProcessingDelegate2 != null)
				{
					preProcessingDelegate2(query, this._broadphaseCandidates, processedColliderIndices);
				}
				HitboxBuffer.HitboxSnapshot.ProcessBroadphaseRootCandidates(query, hitboxSnapshot, this._broadphaseCandidates, processedColliderIndices, null);
			}
			container = hitboxSnapshot;
		}

		private int GetClosestTick(Query query)
		{
			bool flag = query.TickTo == null || (query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy;
			int result;
			if (flag)
			{
				result = query.Tick.Value;
			}
			else
			{
				result = Mathf.RoundToInt(Mathf.Lerp((float)query.Tick.Value, (float)query.TickTo.Value, query.Alpha.Value));
			}
			return result;
		}

		internal HitboxBuffer.HitboxSnapshot[] _buffer;

		private Mapper _mapper;

		private int _head = 0;

		private int _advanced = 0;

		internal int Tick;

		private readonly HashSet<HitboxRoot> _broadphaseCandidates = new HashSet<HitboxRoot>();

		private readonly HashSet<int> _colliderCandidates = new HashSet<int>();

		internal class HitboxSnapshot : IHitboxColliderContainer
		{
			internal int CollidersCapacity
			{
				get
				{
					return this._colliders.Length;
				}
			}

			internal int CollidersCount
			{
				get
				{
					return this._collidersCount - 1;
				}
			}

			internal HitboxSnapshot(Mapper mapper, List<HitboxRoot> initialObjects, int hitboxCapacity, float expansionFactor)
			{
				int num = Math.Max(16, hitboxCapacity);
				this._colliders = new HitboxCollider[num];
				bool flag = initialObjects != null;
				if (flag)
				{
					foreach (HitboxRoot hitboxRoot in initialObjects)
					{
						hitboxRoot.RegisterColliders(this, 0);
					}
				}
				this._broadphase = new BVH(mapper, num * 2, initialObjects, expansionFactor, 3);
			}

			internal void CopyFrom(int tick, int dataTick, HitboxBuffer.HitboxSnapshot from)
			{
				this.ReleaseTempColliders();
				this._broadphase.CopyFrom(from._broadphase);
				this.Tick = tick;
				this.DataTick = dataTick;
				bool flag = this.CollidersCapacity < from._collidersCount;
				if (flag)
				{
					this.ResizeCollidersArray(this._collidersCount - this.CollidersCapacity);
				}
				Array.Copy(from._colliders, 0, this._colliders, 0, from._collidersCount);
				Array.Clear(this._colliders, from._collidersCount, this._colliders.Length - from._collidersCount);
				this._collidersCount = from._collidersCount;
				this._collidersFreeHead = from._collidersFreeHead;
			}

			public ref HitboxCollider GetNextCollider(out int index)
			{
				Assert.Check<int, int, int>(this._collidersTempCount == 0, "Temp Colliders were not released. {0} {1} {2}", this._collidersTempCount, this._collidersCount, this.CollidersCapacity);
				bool flag = this._collidersFreeHead == 0;
				if (flag)
				{
					bool flag2 = this._collidersCount >= this.CollidersCapacity;
					if (flag2)
					{
						this.ResizeCollidersArray(this.CollidersCapacity);
					}
					int collidersCount = this._collidersCount;
					this._collidersCount = collidersCount + 1;
					index = collidersCount;
				}
				else
				{
					index = this._collidersFreeHead;
					this._collidersFreeHead = this._colliders[this._collidersFreeHead].Next;
				}
				Assert.Check<int>(!this._colliders[index].Used, index);
				this._colliders[index] = default(HitboxCollider);
				this._colliders[index].Used = true;
				return ref this._colliders[index];
			}

			private void ResizeCollidersArray(int minimumIncrease)
			{
				int num = this.CollidersCapacity * Math.Max(2, Mathf.FloorToInt((float)minimumIncrease / (float)this.CollidersCapacity + 1f));
				bool flag = num >= 1024;
				if (flag)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn(string.Format("Resizing Hitboxsnapshot colliders capacity from {0} to {1}, this value appears to be elevated and may not have been intended. It is recommended to check for any potential unnecessary Hitboxes.", this.CollidersCapacity, num));
					}
				}
				Array.Resize<HitboxCollider>(ref this._colliders, num);
			}

			public ref HitboxCollider GetNextTempCollider(out int tmpIndex)
			{
				bool flag = this._collidersCount + this._collidersTempCount >= this.CollidersCapacity;
				if (flag)
				{
					this.ResizeCollidersArray(this.CollidersCapacity);
				}
				int collidersCount = this._collidersCount;
				int collidersTempCount = this._collidersTempCount;
				this._collidersTempCount = collidersTempCount + 1;
				tmpIndex = collidersCount + collidersTempCount;
				Assert.Check<int>(!this._colliders[tmpIndex].Used, tmpIndex);
				this._colliders[tmpIndex] = default(HitboxCollider);
				return ref this._colliders[tmpIndex];
			}

			public void ReleaseTempColliders()
			{
				bool flag = this._collidersTempCount > 0;
				if (flag)
				{
					Array.Clear(this._colliders, this._collidersCount, this._collidersTempCount);
				}
				this._collidersTempCount = 0;
			}

			public void ReleaseCollider(int index)
			{
				bool flag = index <= 0 || index >= this.CollidersCapacity;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Index {0} is out of valid range: (0, {1})", index, this.CollidersCapacity));
				}
				Assert.Check<int>(this._colliders[index].Used, index);
				this._colliders[index].Used = false;
				this._colliders[index].Next = this._collidersFreeHead;
				this._collidersFreeHead = index;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ref HitboxCollider GetCollider(int index)
			{
				bool flag = index <= 0 || index >= this.CollidersCapacity;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Index {0} is out of valid range: (0, {1})", index, this.CollidersCapacity));
				}
				return ref this._colliders[index];
			}

			internal void Add(HitboxRoot h, LagCompensationStatisticsManager lagCompStatManager)
			{
				Timer timer = Timer.StartNew();
				h.RegisterColliders(this, this.DataTick);
				timer.Stop();
				lagCompStatManager.PendingSnapshot.SetAddOnBufferTime(timer.ElapsedInMilliseconds, false);
				Timer timer2 = Timer.StartNew();
				this._broadphase.Add(h);
				timer2.Stop();
				lagCompStatManager.PendingSnapshot.SetAddOnBVHTime(timer2.ElapsedInMilliseconds, false);
			}

			internal bool Remove(HitboxRoot hr)
			{
				hr.DeregisterColliders(this);
				return this._broadphase.Remove(hr);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Update(HitboxRoot h, LagCompensationStatisticsManager lagCompStatManager)
			{
				Timer timer = Timer.StartNew();
				bool hitboxRootActive = h.HitboxRootActive;
				foreach (Hitbox hitbox in h.Hitboxes)
				{
					ref HitboxCollider collider = ref this.GetCollider(hitbox.ColliderIndex);
					bool flag = hitboxRootActive;
					if (flag)
					{
						hitbox.SetColliderData(ref collider, this.DataTick);
					}
					else
					{
						collider.Active = false;
					}
				}
				timer.Stop();
				lagCompStatManager.PendingSnapshot.SetUpdateBufferTime(timer.ElapsedInMilliseconds, false);
				Timer timer2 = Timer.StartNew();
				this._broadphase.Update(h, this.DataTick);
				timer2.Stop();
				lagCompStatManager.PendingSnapshot.SetUpdateBVHTime(timer2.ElapsedInMilliseconds, false);
			}

			public void QueryBroadphase(Query query, HashSet<HitboxRoot> broadphaseCandidates)
			{
				this._broadphase.Traverse(query, broadphaseCandidates, query.LayerMask);
			}

			public static void ProcessBroadphaseRootCandidates(Query query, IHitboxColliderContainer fromContainer, HashSet<HitboxRoot> rootCandidates, HashSet<int> processedColliderIndices, IHitboxColliderContainer toContainer = null)
			{
				bool flag = (query.Options & HitOptions.IgnoreInputAuthority) == HitOptions.IgnoreInputAuthority && query.Player.IsRealPlayer;
				bool flag2 = toContainer != null && (query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy;
				foreach (HitboxRoot hitboxRoot in rootCandidates)
				{
					bool flag3 = flag && hitboxRoot.Object.InputAuthority == query.Player;
					if (!flag3)
					{
						foreach (Hitbox hitbox in hitboxRoot.Hitboxes)
						{
							int colliderIndex = hitbox.ColliderIndex;
							ref HitboxCollider collider = ref fromContainer.GetCollider(colliderIndex);
							bool flag4 = collider.Active && (query.LayerMask & collider.layerMask) != 0 && collider.Used;
							bool flag5 = flag4;
							if (flag5)
							{
								processedColliderIndices.Add(colliderIndex);
							}
							bool flag6 = flag2;
							if (flag6)
							{
								ref HitboxCollider collider2 = ref toContainer.GetCollider(colliderIndex);
								bool flag7 = collider2.Active && collider2.Used && (collider2.layerMask & query.LayerMask) != 0;
								if (flag7)
								{
									bool flag8 = !flag4 || collider.Hitbox != collider2.Hitbox;
									if (flag8)
									{
										int item;
										ref HitboxCollider nextTempCollider = ref fromContainer.GetNextTempCollider(out item);
										nextTempCollider = collider2;
										processedColliderIndices.Add(item);
									}
									else
									{
										processedColliderIndices.Remove(colliderIndex);
										int item2;
										ref HitboxCollider nextTempCollider2 = ref fromContainer.GetNextTempCollider(out item2);
										HitboxCollider.Lerp(ref collider, ref collider2, query.Alpha.Value, ref nextTempCollider2);
										processedColliderIndices.Add(item2);
									}
								}
							}
						}
					}
				}
			}

			private HitboxCollider[] _colliders;

			private int _collidersCount = 1;

			private int _collidersTempCount = 0;

			private int _collidersFreeHead = 0;

			internal ILagCompensationBroadphase _broadphase;

			internal int Tick;

			internal int DataTick;

			private const int HIGH_COLLIDERS_CAPACITY = 1024;
		}
	}
}
