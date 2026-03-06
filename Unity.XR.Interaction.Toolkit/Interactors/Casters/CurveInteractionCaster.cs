using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/Curve Interaction Caster", 22)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.Casters.CurveInteractionCaster.html")]
	public class CurveInteractionCaster : InteractionCasterBase, ICurveInteractionCaster, IInteractionCaster, IUIModelUpdater
	{
		public NativeArray<Vector3> samplePoints
		{
			get
			{
				return this.m_SamplePoints;
			}
			protected set
			{
				this.m_SamplePoints = value;
			}
		}

		public Vector3 lastSamplePoint
		{
			get
			{
				if (base.isInitialized && this.m_SamplePoints.Length == this.targetNumCurveSegments + 1)
				{
					return this.m_SamplePoints[this.targetNumCurveSegments];
				}
				return base.castOrigin.position;
			}
		}

		public LayerMask raycastMask
		{
			get
			{
				return this.m_RaycastMask;
			}
			set
			{
				this.m_RaycastMask = value;
			}
		}

		public QueryTriggerInteraction raycastTriggerInteraction
		{
			get
			{
				return this.m_RaycastTriggerInteraction;
			}
			set
			{
				this.m_RaycastTriggerInteraction = value;
			}
		}

		public CurveInteractionCaster.QuerySnapVolumeInteraction raycastSnapVolumeInteraction
		{
			get
			{
				return this.m_RaycastSnapVolumeInteraction;
			}
			set
			{
				this.m_RaycastSnapVolumeInteraction = value;
			}
		}

		public QueryUIDocumentInteraction raycastUIDocumentTriggerInteraction
		{
			get
			{
				return this.m_RaycastUIDocumentTriggerInteraction;
			}
			set
			{
				this.m_RaycastUIDocumentTriggerInteraction = value;
			}
		}

		public int targetNumCurveSegments
		{
			get
			{
				return this.m_TargetNumCurveSegments;
			}
			set
			{
				this.m_TargetNumCurveSegments = Mathf.Clamp(value, 1, 100);
				base.isInitialized = false;
			}
		}

		public CurveInteractionCaster.HitDetectionType hitDetectionType
		{
			get
			{
				return this.m_HitDetectionType;
			}
			set
			{
				this.m_HitDetectionType = value;
			}
		}

		public float castDistance
		{
			get
			{
				return this.m_CastDistance;
			}
			set
			{
				this.m_CastDistance = value;
			}
		}

		public float sphereCastRadius
		{
			get
			{
				return this.m_SphereCastRadius;
			}
			set
			{
				this.m_SphereCastRadius = Mathf.Clamp(value, 0.01f, 0.25f);
			}
		}

		public float coneCastAngle
		{
			get
			{
				return this.m_ConeCastAngle;
			}
			set
			{
				this.m_ConeCastAngle = value;
			}
		}

		private float coneCastAngleRadius
		{
			get
			{
				if (!Mathf.Approximately(this.m_CachedConeCastAngle, this.m_ConeCastAngle))
				{
					this.m_CachedConeCastAngle = this.m_ConeCastAngle;
					this.m_CachedConeCastRadius = math.tan(math.radians(this.m_CachedConeCastAngle) * 0.5f);
				}
				return this.m_CachedConeCastRadius;
			}
		}

		public bool liveConeCastDebugVisuals
		{
			get
			{
				return this.m_LiveConeCastDebugVisuals;
			}
			set
			{
				this.m_LiveConeCastDebugVisuals = value;
			}
		}

		private protected bool isDestroyed { protected get; private set; }

		protected override void Awake()
		{
			base.Awake();
			if (!Application.isEditor)
			{
				this.m_LiveConeCastDebugVisuals = false;
			}
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.samplePoints.IsCreated)
			{
				this.samplePoints.Dispose();
			}
			this.isDestroyed = true;
		}

		protected override bool InitializeCaster()
		{
			if (!this.isDestroyed && !base.isInitialized)
			{
				if (this.samplePoints.IsCreated)
				{
					this.samplePoints.Dispose();
				}
				this.m_SamplePoints = new NativeArray<Vector3>(this.targetNumCurveSegments + 1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
				base.isInitialized = true;
			}
			return base.isInitialized;
		}

		public override bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets)
		{
			if (!base.TryGetColliderTargets(interactionManager, targets))
			{
				return false;
			}
			if (this.UpdatePhysicscastHits(interactionManager))
			{
				for (int i = 0; i < this.m_RaycastHitsCount; i++)
				{
					targets.Add(this.m_RaycastHits[i].collider);
				}
				return true;
			}
			return false;
		}

		public bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets, List<RaycastHit> raycastHits)
		{
			raycastHits.Clear();
			if (!base.TryGetColliderTargets(interactionManager, targets))
			{
				return false;
			}
			if (this.UpdatePhysicscastHits(interactionManager))
			{
				for (int i = 0; i < this.m_RaycastHitsCount; i++)
				{
					raycastHits.Add(this.m_RaycastHits[i]);
					targets.Add(this.m_RaycastHits[i].collider);
				}
				return true;
			}
			return false;
		}

		protected override void UpdateInternalData()
		{
			base.UpdateInternalData();
			this.UpdateSamplePoints();
		}

		protected virtual void UpdateSamplePoints()
		{
			if (!base.isInitialized)
			{
				return;
			}
			Transform effectiveCastOrigin = base.effectiveCastOrigin;
			Vector3 position = effectiveCastOrigin.position;
			Vector3 forward = effectiveCastOrigin.forward;
			this.UpdateSamplePoints(position, forward, this.castDistance, this.m_SamplePoints);
		}

		protected virtual void UpdateSamplePoints(in Vector3 origin, in Vector3 direction, float totalDistance, NativeArray<Vector3> points)
		{
			int length = points.Length;
			if (length < 2)
			{
				return;
			}
			if (length == 2)
			{
				points[0] = origin;
				points[1] = origin + direction * totalDistance;
				return;
			}
			float d = totalDistance / (float)(length - 1);
			Vector3 b = direction * d;
			points[0] = origin;
			for (int i = 1; i < length; i++)
			{
				points[i] = points[i - 1] + b;
			}
		}

		protected virtual bool UpdatePhysicscastHits(in XRInteractionManager interactionManager)
		{
			this.m_RaycastHitsCount = 0;
			this.m_ConeCastDebugInfo.Clear();
			float num = 0f;
			for (int i = 1; i < this.samplePoints.Length; i++)
			{
				float3 v = this.samplePoints[0];
				float3 @float = this.samplePoints[i - 1];
				float3 float2 = this.samplePoints[i];
				this.m_RaycastHitsCount = this.CheckCollidersBetweenPoints(interactionManager, @float, float2, v, this.m_RaycastHits);
				if (this.m_RaycastHitsCount > 0)
				{
					for (int j = 0; j < this.m_RaycastHitsCount; j++)
					{
						RaycastHit[] raycastHits = this.m_RaycastHits;
						int num2 = j;
						raycastHits[num2].distance = raycastHits[num2].distance + num;
					}
					break;
				}
				float num3 = math.length(float2 - @float);
				num += num3;
			}
			return this.m_RaycastHitsCount > 0;
		}

		protected virtual int CheckCollidersBetweenPoints(in XRInteractionManager interactionManager, Vector3 from, Vector3 to, Vector3 origin, RaycastHit[] raycastHits)
		{
			int num = 0;
			float3 x = to - from;
			float maxDistance = math.length(x);
			float3 v = math.normalize(x);
			QueryTriggerInteraction queryTriggerInteraction = (this.m_RaycastSnapVolumeInteraction == CurveInteractionCaster.QuerySnapVolumeInteraction.Collide) ? QueryTriggerInteraction.Collide : this.m_RaycastTriggerInteraction;
			switch (this.m_HitDetectionType)
			{
			case CurveInteractionCaster.HitDetectionType.Raycast:
				num = this.m_LocalPhysicsScene.Raycast(from, v, raycastHits, maxDistance, this.m_RaycastMask, queryTriggerInteraction);
				break;
			case CurveInteractionCaster.HitDetectionType.SphereCast:
				num = this.m_LocalPhysicsScene.SphereCast(from, this.m_SphereCastRadius, v, raycastHits, maxDistance, this.m_RaycastMask, queryTriggerInteraction);
				break;
			case CurveInteractionCaster.HitDetectionType.ConeCast:
			{
				XRInteractionManager interactionManager2 = interactionManager;
				Vector3 vector = v;
				num = this.FilteredConecast(interactionManager2, from, vector, origin, raycastHits, maxDistance, this.m_RaycastMask, queryTriggerInteraction);
				break;
			}
			}
			if (num > 0)
			{
				if (this.m_HitDetectionType != CurveInteractionCaster.HitDetectionType.ConeCast)
				{
					num = this.FilterOutTriggerColliders(interactionManager, raycastHits, num);
				}
				SortingHelpers.Sort<RaycastHit>(raycastHits, this.m_RaycastHitComparer, num);
			}
			return num;
		}

		private int FilteredConecast(XRInteractionManager interactionManager, in Vector3 from, in Vector3 direction, in Vector3 origin, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			CurveInteractionCaster.s_OptimalHits.Clear();
			float num = math.min(maxDistance, 1000f);
			int num2 = 0;
			int num3 = this.m_LocalPhysicsScene.Raycast(from, direction, CurveInteractionCaster.s_SpherecastScratch, maxDistance, layerMask, queryTriggerInteraction);
			if (num3 > 0)
			{
				num3 = this.FilterOutTriggerColliders(interactionManager, CurveInteractionCaster.s_SpherecastScratch, num3);
				for (int i = 0; i < num3; i++)
				{
					RaycastHit raycastHit = CurveInteractionCaster.s_SpherecastScratch[i];
					if (raycastHit.distance <= num)
					{
						Collider collider = raycastHit.collider;
						if (!interactionManager.IsColliderRegisteredToInteractable(collider))
						{
							num = math.min(raycastHit.distance, num);
							raycastHit.distance += 1.5f;
						}
						results[num2] = raycastHit;
						CurveInteractionCaster.s_OptimalHits.Add(raycastHit.collider);
						num2++;
					}
				}
			}
			Vector3 vector = origin - from;
			float magnitude = vector.magnitude;
			float maxOffset = num;
			float num4 = 0f;
			while (num4 < num && !Mathf.Approximately(num4, num))
			{
				float offsetFromOrigin = magnitude + num4;
				Vector3 b;
				float num5;
				float num6;
				BurstPhysicsUtils.GetMultiSegmentConecastParameters(this.coneCastAngleRadius, num4, offsetFromOrigin, maxOffset, direction, out b, out num5, out num6);
				if (this.m_LiveConeCastDebugVisuals)
				{
					this.m_ConeCastDebugInfo.Add(new Tuple<Vector3, float>(from + b, num5));
					this.m_ConeCastDebugInfo.Add(new Tuple<Vector3, float>(from + b + num6 * direction, num5));
				}
				int num7 = this.m_LocalPhysicsScene.SphereCast(from + b, num5, direction, CurveInteractionCaster.s_SpherecastScratch, num6, layerMask, queryTriggerInteraction);
				if (num7 > 0)
				{
					num7 = this.FilterOutTriggerColliders(interactionManager, CurveInteractionCaster.s_SpherecastScratch, num7);
					int num8 = 0;
					while (num8 < num7 && num2 < results.Length)
					{
						RaycastHit raycastHit2 = CurveInteractionCaster.s_SpherecastScratch[num8];
						if (num4 + raycastHit2.distance <= num && !CurveInteractionCaster.s_OptimalHits.Contains(raycastHit2.collider))
						{
							Collider collider = raycastHit2.collider;
							if (interactionManager.IsColliderRegisteredToInteractable(collider))
							{
								if (Mathf.Approximately(raycastHit2.distance, 0f))
								{
									vector = raycastHit2.point;
									Vector3 zero = Vector3.zero;
									if (BurstMathUtility.FastVectorEquals(vector, zero, 0.0001f))
									{
										goto IL_294;
									}
								}
								float3 @float = from;
								float3 float2 = raycastHit2.point;
								float3 float3 = direction;
								float num9;
								BurstPhysicsUtils.GetConecastOffset(@float, float2, float3, out num9);
								raycastHit2.distance += num4 + 1f + num9;
								results[num2] = raycastHit2;
								num2++;
							}
						}
						IL_294:
						num8++;
					}
				}
				num4 += num6;
			}
			CurveInteractionCaster.s_OptimalHits.Clear();
			Array.Clear(CurveInteractionCaster.s_SpherecastScratch, 0, 10);
			return num2;
		}

		private int FilterOutTriggerColliders(XRInteractionManager interactionManager, RaycastHit[] raycastHits, int raycastHitCount)
		{
			bool flag = this.m_RaycastTriggerInteraction == QueryTriggerInteraction.Collide || (this.m_RaycastTriggerInteraction == QueryTriggerInteraction.UseGlobal && Physics.queriesHitTriggers);
			if (this.m_RaycastSnapVolumeInteraction == CurveInteractionCaster.QuerySnapVolumeInteraction.Ignore && flag)
			{
				raycastHitCount = CurveInteractionCaster.FilterOutSnapTriggerColliders(interactionManager, raycastHits, raycastHitCount);
			}
			else if (this.m_RaycastSnapVolumeInteraction == CurveInteractionCaster.QuerySnapVolumeInteraction.Collide && !flag)
			{
				raycastHitCount = CurveInteractionCaster.FilterOutNonSnapTriggerColliders(interactionManager, raycastHits, raycastHitCount);
			}
			return raycastHitCount;
		}

		private static int FilterOutSnapTriggerColliders(in XRInteractionManager interactionManager, RaycastHit[] raycastHits, int count)
		{
			int num = count;
			for (int i = 0; i < num; i++)
			{
				Collider collider = raycastHits[i].collider;
				if (collider == null || (collider.isTrigger && interactionManager.IsColliderRegisteredSnapVolume(collider)))
				{
					raycastHits[i--] = raycastHits[--num];
				}
			}
			return num;
		}

		private static int FilterOutNonSnapTriggerColliders(in XRInteractionManager interactionManager, RaycastHit[] raycastHits, int count)
		{
			int num = count;
			for (int i = 0; i < num; i++)
			{
				Collider collider = raycastHits[i].collider;
				if (collider == null || (collider.isTrigger && !interactionManager.IsColliderRegisteredSnapVolume(collider) && !XRUIToolkitHandler.HasUIDocument(collider)))
				{
					raycastHits[i--] = raycastHits[--num];
				}
			}
			return num;
		}

		public bool UpdateUIModel(ref TrackedDeviceModel uiModel, bool isSelectActive, in Vector2 scrollDelta)
		{
			if (!base.isInitialized)
			{
				return false;
			}
			Transform effectiveCastOrigin = base.effectiveCastOrigin;
			uiModel.position = effectiveCastOrigin.position;
			uiModel.orientation = effectiveCastOrigin.rotation;
			uiModel.select = isSelectActive;
			uiModel.scrollDelta = scrollDelta;
			uiModel.raycastLayerMask = this.m_RaycastMask;
			uiModel.interactionType = UIInteractionType.Ray;
			List<Vector3> raycastPoints = uiModel.raycastPoints;
			raycastPoints.Clear();
			this.UpdateInternalData();
			int length = this.m_SamplePoints.Length;
			if (length <= 0)
			{
				return false;
			}
			if (raycastPoints.Capacity < length)
			{
				raycastPoints.Capacity = length;
			}
			for (int i = 0; i < length; i++)
			{
				raycastPoints.Add(this.m_SamplePoints[i]);
			}
			return true;
		}

		protected virtual void OnDrawGizmosSelected()
		{
			Transform transform = (base.castOrigin != null) ? base.castOrigin : base.transform;
			Vector3 position = transform.position;
			Vector3 vector = position + transform.forward * this.castDistance;
			Gizmos.color = new Color(0.22745098f, 0.47843137f, 0.972549f, 0.92941177f);
			switch (this.m_HitDetectionType)
			{
			case CurveInteractionCaster.HitDetectionType.Raycast:
				Gizmos.DrawLine(position, vector);
				break;
			case CurveInteractionCaster.HitDetectionType.SphereCast:
			{
				Vector3 b = transform.up * this.m_SphereCastRadius;
				Vector3 b2 = transform.right * this.m_SphereCastRadius;
				Gizmos.DrawWireSphere(position, this.m_SphereCastRadius);
				Gizmos.DrawLine(position + b2, vector + b2);
				Gizmos.DrawLine(position - b2, vector - b2);
				Gizmos.DrawLine(position + b, vector + b);
				Gizmos.DrawLine(position - b, vector - b);
				Gizmos.DrawWireSphere(vector, this.m_SphereCastRadius);
				break;
			}
			case CurveInteractionCaster.HitDetectionType.ConeCast:
			{
				float num = Mathf.Tan(this.m_ConeCastAngle * 0.017453292f * 0.5f) * this.castDistance;
				vector = position + transform.forward * (this.castDistance - num);
				Vector3 b3 = transform.up * num;
				Vector3 b4 = transform.right * num;
				Gizmos.DrawLine(position, vector);
				Gizmos.DrawLine(position, vector + b4);
				Gizmos.DrawLine(position, vector - b4);
				Gizmos.DrawLine(position, vector + b3);
				Gizmos.DrawLine(position, vector - b3);
				Gizmos.DrawWireSphere(vector, num);
				break;
			}
			}
			for (int i = 0; i < this.m_SamplePoints.Length; i++)
			{
				Vector3 vector2 = this.m_SamplePoints[i];
				float radius = (this.m_HitDetectionType == CurveInteractionCaster.HitDetectionType.SphereCast) ? this.m_SphereCastRadius : 0.025f;
				Gizmos.color = new Color(0.6392157f, 0.28627452f, 0.6431373f, 0.75f);
				Gizmos.DrawSphere(vector2, radius);
				if (i < this.m_SamplePoints.Length - 1)
				{
					Vector3 to = this.m_SamplePoints[i + 1];
					Gizmos.DrawLine(vector2, to);
				}
			}
			if (this.m_LiveConeCastDebugVisuals)
			{
				for (int j = 0; j < this.m_ConeCastDebugInfo.Count; j += 2)
				{
					Gizmos.color = Color.yellow;
					for (float num2 = 0f; num2 <= 4f; num2 += 1f)
					{
						float d = num2 / 4f;
						Gizmos.DrawWireSphere(this.m_ConeCastDebugInfo[j].Item1 + d * (this.m_ConeCastDebugInfo[j + 1].Item1 - this.m_ConeCastDebugInfo[j].Item1), this.m_ConeCastDebugInfo[j].Item2);
					}
				}
			}
		}

		bool IUIModelUpdater.UpdateUIModel(ref TrackedDeviceModel uiModel, bool isSelectActive, in Vector2 scrollDelta)
		{
			return this.UpdateUIModel(ref uiModel, isSelectActive, scrollDelta);
		}

		private const int k_MaxRaycastHits = 10;

		private const int k_MinNumCurveSegments = 1;

		private const int k_MaxNumCurveSegments = 100;

		private NativeArray<Vector3> m_SamplePoints;

		[SerializeField]
		private LayerMask m_RaycastMask = -1;

		[SerializeField]
		private QueryTriggerInteraction m_RaycastTriggerInteraction = QueryTriggerInteraction.Ignore;

		[SerializeField]
		private CurveInteractionCaster.QuerySnapVolumeInteraction m_RaycastSnapVolumeInteraction = CurveInteractionCaster.QuerySnapVolumeInteraction.Collide;

		[SerializeField]
		private QueryUIDocumentInteraction m_RaycastUIDocumentTriggerInteraction = QueryUIDocumentInteraction.Collide;

		[SerializeField]
		[Range(1f, 100f)]
		private int m_TargetNumCurveSegments = 1;

		[SerializeField]
		private CurveInteractionCaster.HitDetectionType m_HitDetectionType = CurveInteractionCaster.HitDetectionType.ConeCast;

		[SerializeField]
		private float m_CastDistance = 10f;

		[SerializeField]
		[Range(0.01f, 0.25f)]
		private float m_SphereCastRadius = 0.1f;

		[SerializeField]
		private float m_ConeCastAngle = 3f;

		private float m_CachedConeCastAngle;

		private float m_CachedConeCastRadius;

		[SerializeField]
		private bool m_LiveConeCastDebugVisuals;

		private PhysicsScene m_LocalPhysicsScene;

		private int m_RaycastHitsCount;

		private readonly RaycastHit[] m_RaycastHits = new RaycastHit[10];

		private readonly CurveInteractionCaster.RaycastHitComparer m_RaycastHitComparer = new CurveInteractionCaster.RaycastHitComparer();

		private static readonly RaycastHit[] s_SpherecastScratch = new RaycastHit[10];

		private static readonly HashSet<Collider> s_OptimalHits = new HashSet<Collider>();

		private readonly List<Tuple<Vector3, float>> m_ConeCastDebugInfo = new List<Tuple<Vector3, float>>();

		public enum HitDetectionType
		{
			Raycast,
			SphereCast,
			ConeCast
		}

		public enum QuerySnapVolumeInteraction
		{
			Ignore,
			Collide
		}

		protected sealed class RaycastHitComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit a, RaycastHit b)
			{
				return a.distance.CompareTo(b.distance);
			}
		}
	}
}
