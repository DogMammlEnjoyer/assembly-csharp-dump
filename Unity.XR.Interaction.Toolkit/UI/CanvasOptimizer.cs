using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	[AddComponentMenu("Event/Canvas Optimizer", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.UI.CanvasOptimizer.html")]
	public class CanvasOptimizer : MonoBehaviour
	{
		public float rayPositionIgnoreAngle
		{
			get
			{
				return this.m_RayPositionIgnoreAngle;
			}
			set
			{
				this.m_RayPositionIgnoreAngle = value;
			}
		}

		public float rayFacingIgnoreAngle
		{
			get
			{
				return this.m_RayFacingIgnoreAngle;
			}
			set
			{
				this.m_RayFacingIgnoreAngle = value;
			}
		}

		public float rayPositionIgnoreDistance
		{
			get
			{
				return this.m_RayPositionIgnoreDistance;
			}
			set
			{
				this.m_RayPositionIgnoreDistance = value;
			}
		}

		protected void Awake()
		{
			if (ComponentLocatorUtility<CanvasOptimizer>.FindComponent() != this)
			{
				Debug.LogWarning("Duplicate Canvas Optimizer " + base.gameObject.name + " found. Only one Canvas Optimizer is allowed in the scene at a time.", this);
				Object.Destroy(this);
				base.enabled = false;
				return;
			}
			this.FindCullingCamera();
			foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				this.RegisterCanvas(canvas);
			}
		}

		protected void Update()
		{
			this.CheckForNestedCanvasChanges();
			this.CheckForOutOfViewCanvases();
		}

		public void RegisterCanvas(Canvas canvas)
		{
			CanvasTracker canvasTracker = CanvasOptimizer.InitializeCanvasTracking(canvas);
			if (this.m_CanvasTrackers.ContainsKey(canvasTracker))
			{
				return;
			}
			CanvasOptimizer.CanvasState canvasState = new CanvasOptimizer.CanvasState();
			canvasState.Initialize(canvasTracker);
			this.m_CanvasTrackers.Add(canvasTracker, canvasState);
		}

		public void UnregisterCanvas(Canvas canvas)
		{
			CanvasTracker key;
			if (canvas.TryGetComponent<CanvasTracker>(out key))
			{
				this.m_CanvasTrackers.Remove(key);
			}
		}

		private static CanvasTracker InitializeCanvasTracking(Canvas target)
		{
			CanvasTracker canvasTracker;
			if (!target.gameObject.TryGetComponent<CanvasTracker>(out canvasTracker))
			{
				canvasTracker = target.gameObject.AddComponent<CanvasTracker>();
				canvasTracker.hideFlags = HideFlags.HideAndDontSave;
			}
			return canvasTracker;
		}

		private void CheckForNestedCanvasChanges()
		{
			foreach (CanvasOptimizer.CanvasState canvasState in this.m_CanvasTrackers.Values)
			{
				canvasState.CheckForNestedChanges(false);
			}
		}

		private void CheckForOutOfViewCanvases()
		{
			if (this.m_CullingCamera == null || !this.m_CullingCamera.enabled)
			{
				this.FindCullingCamera();
				if (this.m_CullingCameraTransform == null)
				{
					return;
				}
			}
			foreach (CanvasOptimizer.CanvasState canvasState in this.m_CanvasTrackers.Values)
			{
				canvasState.CheckForOutOfView(this.m_CullingCameraTransform, this.m_RayPositionIgnoreAngle, this.m_RayFacingIgnoreAngle, this.m_RayPositionIgnoreDistance);
			}
		}

		private void FindCullingCamera()
		{
			this.m_CullingCamera = Camera.main;
			this.m_CullingCameraTransform = ((this.m_CullingCamera != null) ? this.m_CullingCamera.transform : null);
		}

		[SerializeField]
		[Tooltip("How wide of an field-of-view to use when determining if a canvas is in view.")]
		private float m_RayPositionIgnoreAngle = 45f;

		[SerializeField]
		[Tooltip("How much the camera and canvas rotate away from one another and still be considered facing.")]
		private float m_RayFacingIgnoreAngle = 75f;

		[SerializeField]
		[Tooltip("How far away a canvas can be from this camera and still receive input.")]
		private float m_RayPositionIgnoreDistance = 25f;

		private Camera m_CullingCamera;

		private Transform m_CullingCameraTransform;

		private readonly Dictionary<CanvasTracker, CanvasOptimizer.CanvasState> m_CanvasTrackers = new Dictionary<CanvasTracker, CanvasOptimizer.CanvasState>();

		private class CanvasState
		{
			internal void Initialize(CanvasTracker tracker)
			{
				this.m_Tracker = tracker;
				GameObject gameObject = this.m_Tracker.gameObject;
				gameObject.TryGetComponent<Canvas>(out this.m_Canvas);
				gameObject.TryGetComponent<GraphicRaycaster>(out this.m_Raycaster);
				this.CheckForNestedChanges(true);
			}

			internal void CheckForNestedChanges(bool force = false)
			{
				if (!this.m_Tracker.transformDirty && !force)
				{
					return;
				}
				this.m_Tracker.transformDirty = false;
				Transform transform = this.m_Tracker.transform;
				Transform parent = transform.parent;
				Canvas canvas = (parent != null) ? parent.GetComponentInParent<Canvas>() : null;
				this.m_Nested = (canvas != null);
				if (this.m_Nested && (!this.m_WasNested || force))
				{
					CanvasScaler canvasScaler;
					if (transform.TryGetComponent<CanvasScaler>(out canvasScaler))
					{
						this.m_CanvasScalerSettings.present = true;
						this.m_CanvasScalerSettings.CopyFrom(canvasScaler);
						Object.Destroy(canvasScaler);
					}
					else
					{
						this.m_CanvasScalerSettings.present = false;
					}
					GraphicRaycaster graphicRaycaster;
					if (transform.TryGetComponent<GraphicRaycaster>(out graphicRaycaster))
					{
						this.m_GraphicRaycasterSettings.present = true;
						this.m_GraphicRaycasterSettings.CopyFrom(graphicRaycaster);
						Object.Destroy(graphicRaycaster);
					}
					else
					{
						this.m_GraphicRaycasterSettings.present = false;
					}
					Canvas canvas2;
					if (transform.TryGetComponent<Canvas>(out canvas2))
					{
						this.m_CanvasSettings.present = true;
						this.m_CanvasSettings.CopyFrom(canvas2);
						Object.Destroy(canvas2);
					}
					else
					{
						this.m_CanvasSettings.present = false;
					}
					if (transform.TryGetComponent<TrackedDeviceGraphicRaycaster>(out this.m_TrackedDeviceGraphicRaycaster))
					{
						TrackedDeviceGraphicRaycaster trackedDeviceGraphicRaycaster;
						if (!canvas.TryGetComponent<TrackedDeviceGraphicRaycaster>(out trackedDeviceGraphicRaycaster))
						{
							Debug.LogWarning("Tracked device raycaster not present on parent canvas: " + parent.name + ". Tracked device input will likely not work on: " + transform.name, transform);
						}
						this.m_TrackedDeviceGraphicRaycaster.enabled = false;
					}
				}
				if (!this.m_Nested && (this.m_WasNested || force) && this.m_CanvasSettings.present)
				{
					GameObject gameObject = transform.gameObject;
					this.m_Canvas = gameObject.AddComponent<Canvas>();
					this.m_CanvasSettings.CopyTo(this.m_Canvas);
					if (this.m_CanvasScalerSettings.present)
					{
						CanvasScaler dest = gameObject.AddComponent<CanvasScaler>();
						this.m_CanvasScalerSettings.CopyTo(dest);
					}
					if (this.m_GraphicRaycasterSettings.present)
					{
						this.m_Raycaster = gameObject.AddComponent<GraphicRaycaster>();
						this.m_GraphicRaycasterSettings.CopyTo(this.m_Raycaster);
					}
					if (this.m_TrackedDeviceGraphicRaycaster != null)
					{
						this.m_TrackedDeviceGraphicRaycaster.enabled = true;
					}
				}
				this.m_WasNested = this.m_Nested;
			}

			internal void CheckForOutOfView(Transform gazeSource, float fovAngle, float facingAngle, float maxDistance)
			{
				if (this.m_Nested)
				{
					return;
				}
				if (this.m_Canvas.renderMode != RenderMode.WorldSpace)
				{
					return;
				}
				this.m_CheckTimer += Time.deltaTime;
				if (this.m_CheckTimer < 0.5f)
				{
					return;
				}
				this.m_CheckTimer = 0f;
				Transform transform = this.m_Canvas.transform;
				Vector3 position = gazeSource.position;
				Vector3 forward = gazeSource.forward;
				Vector3 position2 = transform.position;
				Vector3 forward2 = transform.forward;
				float3 @float = position;
				float3 float2 = forward;
				float3 float3 = position2;
				bool flag;
				if (!BurstGazeUtility.IsOutsideGaze(@float, float2, float3, fovAngle))
				{
					float3 float4 = forward;
					float3 float5 = forward2;
					if (!BurstGazeUtility.IsAlignedToGazeForward(float4, float5, facingAngle))
					{
						float3 float6 = position;
						float3 float7 = position2;
						flag = BurstGazeUtility.IsOutsideDistanceRange(float6, float7, maxDistance);
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				if (this.m_RaysDisabled != flag2)
				{
					this.m_RaysDisabled = flag2;
					if (this.m_Raycaster != null)
					{
						this.m_Raycaster.enabled = !this.m_RaysDisabled;
					}
					if (this.m_TrackedDeviceGraphicRaycaster != null)
					{
						this.m_TrackedDeviceGraphicRaycaster.enabled = !this.m_RaysDisabled;
					}
				}
			}

			private const float k_CanvasCheckInterval = 0.5f;

			private CanvasTracker m_Tracker;

			private readonly CanvasOptimizer.CanvasState.CanvasSettings m_CanvasSettings = new CanvasOptimizer.CanvasState.CanvasSettings();

			private readonly CanvasOptimizer.CanvasState.CanvasScalerSettings m_CanvasScalerSettings = new CanvasOptimizer.CanvasState.CanvasScalerSettings();

			private readonly CanvasOptimizer.CanvasState.GraphicRaycasterSettings m_GraphicRaycasterSettings = new CanvasOptimizer.CanvasState.GraphicRaycasterSettings();

			private bool m_WasNested;

			private bool m_Nested;

			private bool m_RaysDisabled;

			private Canvas m_Canvas;

			private GraphicRaycaster m_Raycaster;

			private TrackedDeviceGraphicRaycaster m_TrackedDeviceGraphicRaycaster;

			private float m_CheckTimer;

			private class CanvasSettings
			{
				public bool present { get; set; }

				public void CopyFrom(Canvas source)
				{
					this.m_AdditionalShaderChannels = source.additionalShaderChannels;
					this.m_NormalizedSortingGridSize = source.normalizedSortingGridSize;
					this.m_OverridePixelPerfect = source.overridePixelPerfect;
					this.m_OverrideSorting = source.overrideSorting;
					this.m_PlaneDistance = source.planeDistance;
					this.m_ReferencePixelsPerUnit = source.referencePixelsPerUnit;
					this.m_RenderMode = source.renderMode;
					this.m_ScaleFactor = source.scaleFactor;
					this.m_SortingLayerID = source.sortingLayerID;
					this.m_SortingLayerName = source.sortingLayerName;
					this.m_SortingOrder = source.sortingOrder;
					this.m_TargetDisplay = source.targetDisplay;
				}

				public void CopyTo(Canvas dest)
				{
					dest.additionalShaderChannels = this.m_AdditionalShaderChannels;
					dest.normalizedSortingGridSize = this.m_NormalizedSortingGridSize;
					dest.overridePixelPerfect = this.m_OverridePixelPerfect;
					dest.overrideSorting = this.m_OverrideSorting;
					dest.planeDistance = this.m_PlaneDistance;
					dest.referencePixelsPerUnit = this.m_ReferencePixelsPerUnit;
					dest.renderMode = this.m_RenderMode;
					dest.scaleFactor = this.m_ScaleFactor;
					dest.sortingLayerID = this.m_SortingLayerID;
					dest.sortingLayerName = this.m_SortingLayerName;
					dest.sortingOrder = this.m_SortingOrder;
					dest.targetDisplay = this.m_TargetDisplay;
				}

				private AdditionalCanvasShaderChannels m_AdditionalShaderChannels;

				private float m_NormalizedSortingGridSize;

				private bool m_OverridePixelPerfect;

				private bool m_OverrideSorting;

				private float m_PlaneDistance;

				private float m_ReferencePixelsPerUnit;

				private RenderMode m_RenderMode;

				private float m_ScaleFactor;

				private int m_SortingLayerID;

				private string m_SortingLayerName;

				private int m_SortingOrder;

				private int m_TargetDisplay;
			}

			private class CanvasScalerSettings
			{
				public bool present { get; set; }

				public void CopyFrom(CanvasScaler source)
				{
					this.m_DefaultSpriteDPI = source.defaultSpriteDPI;
					this.m_DynamicPixelsPerUnit = source.dynamicPixelsPerUnit;
					this.m_FallbackScreenDPI = source.fallbackScreenDPI;
					this.m_MatchWidthOrHeight = source.matchWidthOrHeight;
					this.m_PhysicalUnit = source.physicalUnit;
					this.m_ReferencePixelsPerUnit = source.referencePixelsPerUnit;
					this.m_ReferenceResolution = source.referenceResolution;
					this.m_ScaleFactor = source.scaleFactor;
					this.m_ScreenMatchMode = source.screenMatchMode;
					this.m_UiScaleMode = source.uiScaleMode;
				}

				public void CopyTo(CanvasScaler dest)
				{
					dest.defaultSpriteDPI = this.m_DefaultSpriteDPI;
					dest.dynamicPixelsPerUnit = this.m_DynamicPixelsPerUnit;
					dest.fallbackScreenDPI = this.m_FallbackScreenDPI;
					dest.matchWidthOrHeight = this.m_MatchWidthOrHeight;
					dest.physicalUnit = this.m_PhysicalUnit;
					dest.referencePixelsPerUnit = this.m_ReferencePixelsPerUnit;
					dest.referenceResolution = this.m_ReferenceResolution;
					dest.scaleFactor = this.m_ScaleFactor;
					dest.screenMatchMode = this.m_ScreenMatchMode;
					dest.uiScaleMode = this.m_UiScaleMode;
				}

				private float m_DefaultSpriteDPI;

				private float m_DynamicPixelsPerUnit;

				private float m_FallbackScreenDPI;

				private float m_MatchWidthOrHeight;

				private CanvasScaler.Unit m_PhysicalUnit;

				private float m_ReferencePixelsPerUnit;

				private Vector2 m_ReferenceResolution;

				private float m_ScaleFactor;

				private CanvasScaler.ScreenMatchMode m_ScreenMatchMode;

				private CanvasScaler.ScaleMode m_UiScaleMode;
			}

			private class GraphicRaycasterSettings
			{
				public bool present { get; set; }

				public void CopyFrom(GraphicRaycaster source)
				{
					this.m_BlockingMask = source.blockingMask;
					this.m_BlockingObjects = source.blockingObjects;
					this.m_IgnoreReversedGraphics = source.ignoreReversedGraphics;
				}

				public void CopyTo(GraphicRaycaster dest)
				{
					dest.blockingMask = this.m_BlockingMask;
					dest.blockingObjects = this.m_BlockingObjects;
					dest.ignoreReversedGraphics = this.m_IgnoreReversedGraphics;
				}

				private LayerMask m_BlockingMask;

				private GraphicRaycaster.BlockingObjects m_BlockingObjects;

				private bool m_IgnoreReversedGraphics;
			}
		}
	}
}
