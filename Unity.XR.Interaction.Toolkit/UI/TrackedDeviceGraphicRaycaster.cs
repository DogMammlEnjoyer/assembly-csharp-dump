using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	[AddComponentMenu("Event/Tracked Device Graphic Raycaster", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster.html")]
	public class TrackedDeviceGraphicRaycaster : BaseRaycaster, IPokeStateDataProvider, IMultiPokeStateDataProvider
	{
		public bool ignoreReversedGraphics
		{
			get
			{
				return this.m_IgnoreReversedGraphics;
			}
			set
			{
				this.m_IgnoreReversedGraphics = value;
			}
		}

		public bool checkFor2DOcclusion
		{
			get
			{
				return this.m_CheckFor2DOcclusion;
			}
			set
			{
				this.m_CheckFor2DOcclusion = value;
			}
		}

		public bool checkFor3DOcclusion
		{
			get
			{
				return this.m_CheckFor3DOcclusion;
			}
			set
			{
				this.m_CheckFor3DOcclusion = value;
			}
		}

		public LayerMask blockingMask
		{
			get
			{
				return this.m_BlockingMask;
			}
			set
			{
				this.m_BlockingMask = value;
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

		public override Camera eventCamera
		{
			get
			{
				if (!(this.canvas != null) || !(this.canvas.worldCamera != null))
				{
					return Camera.main;
				}
				return this.canvas.worldCamera;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			TrackedDeviceEventData trackedDeviceEventData = eventData as TrackedDeviceEventData;
			if (trackedDeviceEventData != null)
			{
				this.PerformRaycasts(trackedDeviceEventData, resultAppendList);
			}
		}

		private Canvas canvas
		{
			get
			{
				if (this.m_Canvas != null)
				{
					return this.m_Canvas;
				}
				base.TryGetComponent<Canvas>(out this.m_Canvas);
				return this.m_Canvas;
			}
		}

		public static bool IsPokeInteractingWithUI(IUIInteractor interactor)
		{
			using (Dictionary<TrackedDeviceGraphicRaycaster, HashSet<IUIInteractor>>.ValueCollection.Enumerator enumerator = TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contains(interactor))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void EndPokeInteraction(IUIInteractor interactor)
		{
			if (interactor == null)
			{
				return;
			}
			this.m_PokeLogic.OnHoverExited(interactor);
			TrackedDeviceGraphicRaycaster x;
			if (TrackedDeviceGraphicRaycaster.s_InteractorRaycasters.TryGetValue(interactor, out x) && x != null && x == this)
			{
				TrackedDeviceGraphicRaycaster.s_InteractorRaycasters.Remove(interactor);
			}
			TrackedDeviceGraphicRaycaster.s_InteractorHitData.Remove(interactor);
			TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters[this].Remove(interactor);
		}

		public static bool TryGetPokeStateDataForInteractor(IUIInteractor interactor, out PokeStateData data)
		{
			foreach (KeyValuePair<TrackedDeviceGraphicRaycaster, HashSet<IUIInteractor>> keyValuePair in TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters)
			{
				if (keyValuePair.Value.Contains(interactor))
				{
					TrackedDeviceGraphicRaycaster key = keyValuePair.Key;
					data = key.pokeStateData.Value;
					return true;
				}
			}
			data = default(PokeStateData);
			return false;
		}

		public IReadOnlyBindableVariable<PokeStateData> pokeStateData
		{
			get
			{
				XRPokeLogic pokeLogic = this.m_PokeLogic;
				if (pokeLogic == null)
				{
					return null;
				}
				return pokeLogic.pokeStateData;
			}
		}

		private Dictionary<Transform, BindableVariable<PokeStateData>> pokeStateDataDictionary { get; } = new Dictionary<Transform, BindableVariable<PokeStateData>>();

		public IReadOnlyBindableVariable<PokeStateData> GetPokeStateDataForTarget(Transform target)
		{
			if (!this.pokeStateDataDictionary.ContainsKey(target))
			{
				this.pokeStateDataDictionary[target] = new BindableVariable<PokeStateData>(default(PokeStateData), true, null, false);
			}
			return this.pokeStateDataDictionary[target];
		}

		public static bool IsPokeSelectingWithUI(IUIInteractor interactor)
		{
			TrackedDeviceGraphicRaycaster x;
			return interactor != null && TrackedDeviceGraphicRaycaster.s_InteractorRaycasters.TryGetValue(interactor, out x) && x != null;
		}

		private static RaycastHit FindClosestHit(RaycastHit[] hits, int count)
		{
			int num = 0;
			float num2 = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				if (hits[i].distance < num2)
				{
					num2 = hits[i].distance;
					num = i;
				}
			}
			return hits[num];
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			this.m_LocalPhysicsScene2D = base.gameObject.scene.GetPhysicsScene2D();
			TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters.Add(this, new HashSet<IUIInteractor>());
			this.SetupPoke();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			HashSet<IUIInteractor> hashSet;
			using (CollectionPool<HashSet<IUIInteractor>, IUIInteractor>.Get(out hashSet))
			{
				foreach (KeyValuePair<IUIInteractor, TrackedDeviceGraphicRaycaster> keyValuePair in TrackedDeviceGraphicRaycaster.s_InteractorRaycasters)
				{
					if (keyValuePair.Value == this)
					{
						hashSet.Add(keyValuePair.Key);
					}
				}
				foreach (IUIInteractor item in TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters[this])
				{
					hashSet.Add(item);
				}
				foreach (IUIInteractor interactor in hashSet)
				{
					this.EndPokeInteraction(interactor);
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters.Remove(this);
			this.pokeStateDataDictionary.Clear();
			this.m_BindingsGroup.Clear();
		}

		private void SetupPoke()
		{
			this.m_BindingsGroup.Clear();
			if (this.m_PokeLogic == null)
			{
				this.m_PokeLogic = new XRPokeLogic();
			}
			PokeThresholdData pokeThresholdData = new PokeThresholdData
			{
				pokeDirection = PokeAxis.Z,
				interactionDepthOffset = 0f,
				enablePokeAngleThreshold = true,
				pokeAngleThreshold = 89.9f
			};
			this.m_PokeLogic.Initialize(base.transform, pokeThresholdData, null);
			this.m_PokeLogic.SetPokeDepth(0.1f);
			this.m_BindingsGroup.AddBinding(this.m_PokeLogic.pokeStateData.SubscribeAndUpdate(delegate(PokeStateData data)
			{
				if (data.target != null)
				{
					if (!this.pokeStateDataDictionary.ContainsKey(data.target))
					{
						this.pokeStateDataDictionary[data.target] = new BindableVariable<PokeStateData>(default(PokeStateData), true, null, false);
					}
					this.pokeStateDataDictionary[data.target].Value = data;
					return;
				}
				foreach (BindableVariable<PokeStateData> bindableVariable in this.pokeStateDataDictionary.Values)
				{
					bindableVariable.Value = data;
				}
			}));
		}

		private void PerformRaycasts(TrackedDeviceEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (this.canvas == null)
			{
				return;
			}
			Camera eventCamera = this.eventCamera;
			if (eventCamera == null)
			{
				if (!this.m_HasWarnedEventCameraNull)
				{
					Debug.LogWarning("Event Camera must be set on World Space Canvas to perform ray casts with tracked device. UI events will not function correctly until it is set.", this);
					this.m_HasWarnedEventCameraNull = true;
				}
				return;
			}
			LayerMask layerMask = eventData.layerMask;
			IUIInteractor interactor = eventData.interactor;
			TrackedDeviceModel trackedDeviceModel;
			if (interactor != null && interactor.TryGetUIModel(out trackedDeviceModel) && trackedDeviceModel.interactionType == UIInteractionType.Poke)
			{
				if (!this.PerformSpherecast(trackedDeviceModel.position, trackedDeviceModel.pokeDepth, layerMask, eventCamera, resultAppendList) || resultAppendList.Count <= 0)
				{
					this.EndPokeInteraction(interactor);
					return;
				}
				eventData.rayHitIndex = 1;
				Transform transform = resultAppendList[0].gameObject.transform;
				this.m_PokeLogic.SetPokeDepth(trackedDeviceModel.pokeDepth);
				if (!TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters[this].Contains(interactor))
				{
					TrackedDeviceGraphicRaycaster.s_PokeHoverRaycasters[this].Add(interactor);
					this.m_PokeLogic.OnHoverEntered(interactor, new Pose(trackedDeviceModel.position, trackedDeviceModel.orientation), transform);
				}
				if (!this.m_PokeLogic.MeetsRequirementsForSelectAction(interactor, transform.position, trackedDeviceModel.position, 0f, transform))
				{
					TrackedDeviceGraphicRaycaster.s_InteractorRaycasters.Remove(interactor);
					return;
				}
				if (this.m_RaycastResultsCache.Count > 0)
				{
					TrackedDeviceGraphicRaycaster.RaycastHitData raycastHitData = this.m_RaycastResultsCache[0];
					TrackedDeviceGraphicRaycaster.RaycastHitData a;
					if (!TrackedDeviceGraphicRaycaster.s_InteractorHitData.TryGetValue(interactor, out a) || TrackedDeviceGraphicRaycaster.s_RaycastHitComparer.Compare(a, raycastHitData) < 0)
					{
						TrackedDeviceGraphicRaycaster.s_InteractorHitData[interactor] = raycastHitData;
						TrackedDeviceGraphicRaycaster.s_InteractorRaycasters[interactor] = this;
						return;
					}
				}
			}
			else
			{
				List<Vector3> rayPoints = eventData.rayPoints;
				float num = 0f;
				for (int i = 1; i < rayPoints.Count; i++)
				{
					Vector3 from = rayPoints[i - 1];
					Vector3 to = rayPoints[i];
					if (this.PerformRaycast(from, to, layerMask, eventCamera, resultAppendList, ref num))
					{
						eventData.rayHitIndex = i;
						return;
					}
				}
			}
		}

		private bool PerformSpherecast(Vector3 origin, float radius, LayerMask layerMask, Camera currentEventCamera, List<RaycastResult> resultAppendList)
		{
			this.m_RaycastResultsCache.Clear();
			TrackedDeviceGraphicRaycaster.SortedSpherecastGraphics(this.canvas, origin, radius, layerMask, currentEventCamera, this.m_RaycastResultsCache);
			if (this.m_RaycastResultsCache.Count <= 0)
			{
				return false;
			}
			TrackedDeviceGraphicRaycaster.RaycastHitData item = this.m_RaycastResultsCache[0];
			Ray ray = new Ray(origin, item.worldHitPosition - origin);
			this.m_RaycastResultsCache.Clear();
			this.m_RaycastResultsCache.Add(item);
			return this.ProcessSortedHitsResults(ray, float.PositiveInfinity, false, this.m_RaycastResultsCache, resultAppendList);
		}

		private bool PerformRaycast(Vector3 from, Vector3 to, LayerMask layerMask, Camera currentEventCamera, List<RaycastResult> resultAppendList, ref float existingHitLength)
		{
			bool hitSomething = false;
			float num = Vector3.Distance(to, from);
			Ray ray = new Ray(from, to - from);
			float num2 = num;
			if (this.m_CheckFor3DOcclusion)
			{
				int num3 = this.m_LocalPhysicsScene.Raycast(ray.origin, ray.direction, this.m_OcclusionHits3D, num2, this.m_BlockingMask, this.m_RaycastTriggerInteraction);
				if (num3 > 0)
				{
					RaycastHit raycastHit = TrackedDeviceGraphicRaycaster.FindClosestHit(this.m_OcclusionHits3D, num3);
					num2 = existingHitLength + raycastHit.distance;
					hitSomething = true;
				}
			}
			if (this.m_CheckFor2DOcclusion && this.m_LocalPhysicsScene2D.GetRayIntersection(ray, num2, this.m_OcclusionHits2D, this.m_BlockingMask) > 0)
			{
				num2 = this.m_OcclusionHits2D[0].distance;
				hitSomething = true;
			}
			this.m_RaycastResultsCache.Clear();
			TrackedDeviceGraphicRaycaster.SortedRaycastGraphics(this.canvas, ray, num2, layerMask, currentEventCamera, this.m_RaycastResultsCache);
			return this.ProcessSortedHitsResults(ray, num2, hitSomething, this.m_RaycastResultsCache, resultAppendList);
		}

		private bool ProcessSortedHitsResults(Ray ray, float hitDistance, bool hitSomething, List<TrackedDeviceGraphicRaycaster.RaycastHitData> raycastHitDatums, List<RaycastResult> resultAppendList)
		{
			foreach (TrackedDeviceGraphicRaycaster.RaycastHitData raycastHitData in raycastHitDatums)
			{
				bool flag = true;
				GameObject gameObject = raycastHitData.graphic.gameObject;
				if (this.m_IgnoreReversedGraphics)
				{
					Vector3 direction = ray.direction;
					Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
					flag = (Vector3.Dot(direction, rhs) > 0f);
				}
				flag &= (raycastHitData.distance <= hitDistance);
				if (flag)
				{
					Vector3 forward = gameObject.transform.forward;
					RaycastResult item = new RaycastResult
					{
						gameObject = gameObject,
						module = this,
						distance = raycastHitData.distance,
						index = (float)resultAppendList.Count,
						depth = raycastHitData.graphic.depth,
						sortingLayer = this.canvas.sortingLayerID,
						sortingOrder = this.canvas.sortingOrder,
						worldPosition = raycastHitData.worldHitPosition,
						worldNormal = -forward,
						screenPosition = raycastHitData.screenPosition,
						displayIndex = raycastHitData.displayIndex
					};
					resultAppendList.Add(item);
					hitSomething = true;
				}
			}
			return hitSomething;
		}

		private static void SortedSpherecastGraphics(Canvas canvas, Vector3 origin, float radius, LayerMask layerMask, Camera eventCamera, List<TrackedDeviceGraphicRaycaster.RaycastHitData> results)
		{
			IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
			TrackedDeviceGraphicRaycaster.s_SortedGraphics.Clear();
			for (int i = 0; i < graphicsForCanvas.Count; i++)
			{
				Graphic graphic = graphicsForCanvas[i];
				if (TrackedDeviceGraphicRaycaster.ShouldTestGraphic(graphic, layerMask))
				{
					Vector4 raycastPadding = graphic.raycastPadding;
					Vector3 vector;
					float num;
					if (TrackedDeviceGraphicRaycaster.SphereIntersectsRectTransform(graphic.rectTransform, raycastPadding, origin, out vector, out num) && num <= radius)
					{
						Vector2 vector2 = eventCamera.WorldToScreenPoint(vector);
						if (graphic.Raycast(vector2, eventCamera))
						{
							TrackedDeviceGraphicRaycaster.s_SortedGraphics.Add(new TrackedDeviceGraphicRaycaster.RaycastHitData(graphic, vector, vector2, num, eventCamera.targetDisplay));
						}
					}
				}
			}
			SortingHelpers.Sort<TrackedDeviceGraphicRaycaster.RaycastHitData>(TrackedDeviceGraphicRaycaster.s_SortedGraphics, TrackedDeviceGraphicRaycaster.s_RaycastHitComparer);
			results.AddRange(TrackedDeviceGraphicRaycaster.s_SortedGraphics);
		}

		private static void SortedRaycastGraphics(Canvas canvas, Ray ray, float maxDistance, LayerMask layerMask, Camera eventCamera, List<TrackedDeviceGraphicRaycaster.RaycastHitData> results)
		{
			IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
			TrackedDeviceGraphicRaycaster.s_SortedGraphics.Clear();
			for (int i = 0; i < graphicsForCanvas.Count; i++)
			{
				Graphic graphic = graphicsForCanvas[i];
				if (TrackedDeviceGraphicRaycaster.ShouldTestGraphic(graphic, layerMask))
				{
					Vector4 raycastPadding = graphic.raycastPadding;
					Vector3 vector;
					float num;
					if (TrackedDeviceGraphicRaycaster.RayIntersectsRectTransform(graphic.rectTransform, raycastPadding, ray, out vector, out num) && num <= maxDistance)
					{
						Vector2 vector2 = eventCamera.WorldToScreenPoint(vector);
						if (graphic.Raycast(vector2, eventCamera))
						{
							TrackedDeviceGraphicRaycaster.s_SortedGraphics.Add(new TrackedDeviceGraphicRaycaster.RaycastHitData(graphic, vector, vector2, num, eventCamera.targetDisplay));
						}
					}
				}
			}
			SortingHelpers.Sort<TrackedDeviceGraphicRaycaster.RaycastHitData>(TrackedDeviceGraphicRaycaster.s_SortedGraphics, TrackedDeviceGraphicRaycaster.s_RaycastHitComparer);
			results.AddRange(TrackedDeviceGraphicRaycaster.s_SortedGraphics);
		}

		private static bool ShouldTestGraphic(Graphic graphic, LayerMask layerMask)
		{
			return graphic.depth != -1 && graphic.raycastTarget && !graphic.canvasRenderer.cull && (1 << graphic.gameObject.layer & layerMask) != 0;
		}

		private static bool SphereIntersectsRectTransform(RectTransform transform, Vector4 raycastPadding, Vector3 from, out Vector3 worldPosition, out float distance)
		{
			Plane rectTransformPlane = TrackedDeviceGraphicRaycaster.GetRectTransformPlane(transform, raycastPadding, TrackedDeviceGraphicRaycaster.s_Corners);
			Vector3 a = rectTransformPlane.ClosestPointOnPlane(from);
			return TrackedDeviceGraphicRaycaster.RayIntersectsRectTransform(new Ray(from, a - from), rectTransformPlane, out worldPosition, out distance);
		}

		private static bool RayIntersectsRectTransform(RectTransform transform, Vector4 raycastPadding, Ray ray, out Vector3 worldPosition, out float distance)
		{
			Plane rectTransformPlane = TrackedDeviceGraphicRaycaster.GetRectTransformPlane(transform, raycastPadding, TrackedDeviceGraphicRaycaster.s_Corners);
			return TrackedDeviceGraphicRaycaster.RayIntersectsRectTransform(ray, rectTransformPlane, out worldPosition, out distance);
		}

		private static bool RayIntersectsRectTransform(Ray ray, Plane plane, out Vector3 worldPosition, out float distance)
		{
			float num;
			if (plane.Raycast(ray, out num))
			{
				Vector3 point = ray.GetPoint(num);
				Vector3 rhs = TrackedDeviceGraphicRaycaster.s_Corners[3] - TrackedDeviceGraphicRaycaster.s_Corners[0];
				Vector3 rhs2 = TrackedDeviceGraphicRaycaster.s_Corners[1] - TrackedDeviceGraphicRaycaster.s_Corners[0];
				float num2 = Vector3.Dot(point - TrackedDeviceGraphicRaycaster.s_Corners[0], rhs);
				if (Vector3.Dot(point - TrackedDeviceGraphicRaycaster.s_Corners[0], rhs2) >= 0f && num2 >= 0f)
				{
					Vector3 rhs3 = TrackedDeviceGraphicRaycaster.s_Corners[1] - TrackedDeviceGraphicRaycaster.s_Corners[2];
					Vector3 rhs4 = TrackedDeviceGraphicRaycaster.s_Corners[3] - TrackedDeviceGraphicRaycaster.s_Corners[2];
					float num3 = Vector3.Dot(point - TrackedDeviceGraphicRaycaster.s_Corners[2], rhs3);
					float num4 = Vector3.Dot(point - TrackedDeviceGraphicRaycaster.s_Corners[2], rhs4);
					if (num3 >= 0f && num4 >= 0f)
					{
						worldPosition = point;
						distance = num;
						return true;
					}
				}
			}
			worldPosition = Vector3.zero;
			distance = 0f;
			return false;
		}

		private static Plane GetRectTransformPlane(RectTransform transform, Vector4 raycastPadding, Vector3[] fourCornersArray)
		{
			TrackedDeviceGraphicRaycaster.GetRectTransformWorldCorners(transform, raycastPadding, fourCornersArray);
			return new Plane(fourCornersArray[0], fourCornersArray[1], fourCornersArray[2]);
		}

		private static void GetRectTransformWorldCorners(RectTransform transform, Vector4 offset, Vector3[] fourCornersArray)
		{
			if (fourCornersArray == null || fourCornersArray.Length < 4)
			{
				Debug.LogError("Calling GetRectTransformWorldCorners with an array that is null or has less than 4 elements.");
				return;
			}
			Rect rect = transform.rect;
			float x = rect.x + offset.x;
			float y = rect.y + offset.y;
			float x2 = rect.xMax - offset.z;
			float y2 = rect.yMax - offset.w;
			fourCornersArray[0] = new Vector3(x, y, 0f);
			fourCornersArray[1] = new Vector3(x, y2, 0f);
			fourCornersArray[2] = new Vector3(x2, y2, 0f);
			fourCornersArray[3] = new Vector3(x2, y, 0f);
			Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
			for (int i = 0; i < 4; i++)
			{
				fourCornersArray[i] = localToWorldMatrix.MultiplyPoint(fourCornersArray[i]);
			}
		}

		[Conditional("UNITY_EDITOR")]
		protected void OnDrawGizmosSelected()
		{
		}

		private const int k_MaxRaycastHits = 10;

		[SerializeField]
		[Tooltip("Whether Graphics facing away from the ray caster are checked for ray casts. Enable this to ignore backfacing Graphics.")]
		private bool m_IgnoreReversedGraphics;

		[SerializeField]
		[Tooltip("Whether or not 2D occlusion is checked when performing ray casts. Enable to make Graphics be blocked by 2D objects that exist in front of it.")]
		private bool m_CheckFor2DOcclusion;

		[SerializeField]
		[Tooltip("Whether or not 3D occlusion is checked when performing ray casts. Enable to make Graphics be blocked by 3D objects that exist in front of it.")]
		private bool m_CheckFor3DOcclusion;

		[SerializeField]
		[Tooltip("The layers of objects that are checked to determine if they block Graphic ray casts when checking for 2D or 3D occlusion.")]
		private LayerMask m_BlockingMask = -1;

		[SerializeField]
		[Tooltip("Specifies whether the ray cast should hit Triggers when checking for 3D occlusion. Use Global refers to the Queries Hit Triggers setting in Physics Project Settings.")]
		private QueryTriggerInteraction m_RaycastTriggerInteraction = QueryTriggerInteraction.Ignore;

		private Canvas m_Canvas;

		private bool m_HasWarnedEventCameraNull;

		private readonly RaycastHit[] m_OcclusionHits3D = new RaycastHit[10];

		private readonly RaycastHit2D[] m_OcclusionHits2D = new RaycastHit2D[1];

		private static readonly TrackedDeviceGraphicRaycaster.RaycastHitComparer s_RaycastHitComparer = new TrackedDeviceGraphicRaycaster.RaycastHitComparer();

		private static readonly Vector3[] s_Corners = new Vector3[4];

		private readonly List<TrackedDeviceGraphicRaycaster.RaycastHitData> m_RaycastResultsCache = new List<TrackedDeviceGraphicRaycaster.RaycastHitData>();

		[NonSerialized]
		private static readonly List<TrackedDeviceGraphicRaycaster.RaycastHitData> s_SortedGraphics = new List<TrackedDeviceGraphicRaycaster.RaycastHitData>();

		[NonSerialized]
		private static readonly Dictionary<IUIInteractor, TrackedDeviceGraphicRaycaster.RaycastHitData> s_InteractorHitData = new Dictionary<IUIInteractor, TrackedDeviceGraphicRaycaster.RaycastHitData>();

		private XRPokeLogic m_PokeLogic;

		[NonSerialized]
		private static readonly Dictionary<IUIInteractor, TrackedDeviceGraphicRaycaster> s_InteractorRaycasters = new Dictionary<IUIInteractor, TrackedDeviceGraphicRaycaster>();

		[NonSerialized]
		private static readonly Dictionary<TrackedDeviceGraphicRaycaster, HashSet<IUIInteractor>> s_PokeHoverRaycasters = new Dictionary<TrackedDeviceGraphicRaycaster, HashSet<IUIInteractor>>();

		private BindingsGroup m_BindingsGroup = new BindingsGroup();

		private PhysicsScene m_LocalPhysicsScene;

		private PhysicsScene2D m_LocalPhysicsScene2D;

		private readonly struct RaycastHitData
		{
			public RaycastHitData(Graphic graphic, Vector3 worldHitPosition, Vector2 screenPosition, float distance, int displayIndex)
			{
				this.graphic = graphic;
				this.worldHitPosition = worldHitPosition;
				this.screenPosition = screenPosition;
				this.distance = distance;
				this.displayIndex = displayIndex;
			}

			public Graphic graphic { get; }

			public Vector3 worldHitPosition { get; }

			public Vector2 screenPosition { get; }

			public float distance { get; }

			public int displayIndex { get; }
		}

		private sealed class RaycastHitComparer : IComparer<TrackedDeviceGraphicRaycaster.RaycastHitData>
		{
			public int Compare(TrackedDeviceGraphicRaycaster.RaycastHitData a, TrackedDeviceGraphicRaycaster.RaycastHitData b)
			{
				int num = b.graphic.canvas.sortingOrder.CompareTo(a.graphic.canvas.sortingOrder);
				if (num != 0)
				{
					return num;
				}
				return b.graphic.depth.CompareTo(a.graphic.depth);
			}
		}
	}
}
