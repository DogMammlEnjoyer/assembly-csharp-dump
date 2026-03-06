using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Curves;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/XR Ray Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor.html")]
	public class XRRayInteractor : XRBaseInputInteractor, IAdvancedLineRenderable, ILineRenderable, IUIHoverInteractor, IUIInteractor, IXRRayProvider, IXRScaleValueProvider
	{
		public XRRayInteractor.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				this.m_LineType = value;
			}
		}

		public bool blendVisualLinePoints
		{
			get
			{
				return this.m_BlendVisualLinePoints;
			}
			set
			{
				this.m_BlendVisualLinePoints = value;
			}
		}

		public float maxRaycastDistance
		{
			get
			{
				return this.m_MaxRaycastDistance;
			}
			set
			{
				this.m_MaxRaycastDistance = value;
			}
		}

		public Transform rayOriginTransform
		{
			get
			{
				return this.m_RayOriginTransform;
			}
			set
			{
				this.m_RayOriginTransform = value;
				this.m_HasRayOriginTransform = (this.m_RayOriginTransform != null);
			}
		}

		public Transform referenceFrame
		{
			get
			{
				return this.m_ReferenceFrame;
			}
			set
			{
				this.m_ReferenceFrame = value;
				this.m_HasReferenceFrame = (this.m_ReferenceFrame != null);
			}
		}

		public float velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		public float acceleration
		{
			get
			{
				return this.m_Acceleration;
			}
			set
			{
				this.m_Acceleration = value;
			}
		}

		public float additionalGroundHeight
		{
			get
			{
				return this.m_AdditionalGroundHeight;
			}
			set
			{
				this.m_AdditionalGroundHeight = value;
			}
		}

		public float additionalFlightTime
		{
			get
			{
				return this.m_AdditionalFlightTime;
			}
			set
			{
				this.m_AdditionalFlightTime = value;
			}
		}

		public float endPointDistance
		{
			get
			{
				return this.m_EndPointDistance;
			}
			set
			{
				this.m_EndPointDistance = value;
			}
		}

		public float endPointHeight
		{
			get
			{
				return this.m_EndPointHeight;
			}
			set
			{
				this.m_EndPointHeight = value;
			}
		}

		public float controlPointDistance
		{
			get
			{
				return this.m_ControlPointDistance;
			}
			set
			{
				this.m_ControlPointDistance = value;
			}
		}

		public float controlPointHeight
		{
			get
			{
				return this.m_ControlPointHeight;
			}
			set
			{
				this.m_ControlPointHeight = value;
			}
		}

		public int sampleFrequency
		{
			get
			{
				return this.m_SampleFrequency;
			}
			set
			{
				this.m_SampleFrequency = XRRayInteractor.SanitizeSampleFrequency(value);
			}
		}

		public XRRayInteractor.HitDetectionType hitDetectionType
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

		public float sphereCastRadius
		{
			get
			{
				return this.m_SphereCastRadius;
			}
			set
			{
				this.m_SphereCastRadius = value;
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

		public XRRayInteractor.QuerySnapVolumeInteraction raycastSnapVolumeInteraction
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

		public bool hitClosestOnly
		{
			get
			{
				return this.m_HitClosestOnly;
			}
			set
			{
				this.m_HitClosestOnly = value;
			}
		}

		public bool hoverToSelect
		{
			get
			{
				return this.m_HoverToSelect;
			}
			set
			{
				this.m_HoverToSelect = value;
			}
		}

		public float hoverTimeToSelect
		{
			get
			{
				return this.m_HoverTimeToSelect;
			}
			set
			{
				this.m_HoverTimeToSelect = value;
			}
		}

		public bool autoDeselect
		{
			get
			{
				return this.m_AutoDeselect;
			}
			set
			{
				this.m_AutoDeselect = value;
			}
		}

		public float timeToAutoDeselect
		{
			get
			{
				return this.m_TimeToAutoDeselect;
			}
			set
			{
				this.m_TimeToAutoDeselect = value;
			}
		}

		public bool enableUIInteraction
		{
			get
			{
				return this.m_EnableUIInteraction;
			}
			set
			{
				if (this.m_EnableUIInteraction != value)
				{
					this.m_EnableUIInteraction = value;
					RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
					if (registeredUIInteractorCache == null)
					{
						return;
					}
					registeredUIInteractorCache.RegisterOrUnregisterXRUIInputModule(this.m_EnableUIInteraction);
				}
			}
		}

		public bool blockInteractionsWithScreenSpaceUI
		{
			get
			{
				return this.m_BlockInteractionsWithScreenSpaceUI;
			}
			set
			{
				this.m_BlockInteractionsWithScreenSpaceUI = value;
			}
		}

		public bool blockUIOnInteractableSelection
		{
			get
			{
				return this.m_BlockUIOnInteractableSelection;
			}
			set
			{
				this.m_BlockUIOnInteractableSelection = value;
			}
		}

		public bool manipulateAttachTransform
		{
			get
			{
				return this.m_ManipulateAttachTransform;
			}
			set
			{
				this.m_ManipulateAttachTransform = value;
			}
		}

		public bool useForceGrab
		{
			get
			{
				return this.m_UseForceGrab;
			}
			set
			{
				this.m_UseForceGrab = value;
			}
		}

		public float rotateSpeed
		{
			get
			{
				return this.m_RotateSpeed;
			}
			set
			{
				this.m_RotateSpeed = value;
			}
		}

		public float translateSpeed
		{
			get
			{
				return this.m_TranslateSpeed;
			}
			set
			{
				this.m_TranslateSpeed = value;
			}
		}

		public Transform rotateReferenceFrame
		{
			get
			{
				return this.m_RotateReferenceFrame;
			}
			set
			{
				this.m_RotateReferenceFrame = value;
			}
		}

		public XRRayInteractor.RotateMode rotateMode
		{
			get
			{
				return this.m_RotateMode;
			}
			set
			{
				this.m_RotateMode = value;
			}
		}

		public UIHoverEnterEvent uiHoverEntered
		{
			get
			{
				return this.m_UIHoverEntered;
			}
			set
			{
				this.m_UIHoverEntered = value;
			}
		}

		public UIHoverExitEvent uiHoverExited
		{
			get
			{
				return this.m_UIHoverExited;
			}
			set
			{
				this.m_UIHoverExited = value;
			}
		}

		public bool enableARRaycasting
		{
			get
			{
				return this.m_EnableARRaycasting;
			}
			set
			{
				this.m_EnableARRaycasting = value;
			}
		}

		public bool occludeARHitsWith3DObjects
		{
			get
			{
				return this.m_OccludeARHitsWith3DObjects;
			}
			set
			{
				this.m_OccludeARHitsWith3DObjects = value;
			}
		}

		public bool occludeARHitsWith2DObjects
		{
			get
			{
				return this.m_OccludeARHitsWith2DObjects;
			}
			set
			{
				this.m_OccludeARHitsWith2DObjects = value;
			}
		}

		public ScaleMode scaleMode
		{
			get
			{
				return this.m_ScaleMode;
			}
			set
			{
				this.m_ScaleMode = value;
			}
		}

		public XRInputButtonReader uiPressInput
		{
			get
			{
				return this.m_UIPressInput;
			}
			set
			{
				base.SetInputProperty(ref this.m_UIPressInput, value);
			}
		}

		public XRInputValueReader<Vector2> uiScrollInput
		{
			get
			{
				return this.m_UIScrollInput;
			}
			set
			{
				base.SetInputProperty<Vector2>(ref this.m_UIScrollInput, value);
			}
		}

		public XRInputValueReader<Vector2> translateManipulationInput
		{
			get
			{
				return this.m_TranslateManipulationInput;
			}
			set
			{
				base.SetInputProperty<Vector2>(ref this.m_TranslateManipulationInput, value);
			}
		}

		public XRInputValueReader<Vector2> rotateManipulationInput
		{
			get
			{
				return this.m_RotateManipulationInput;
			}
			set
			{
				base.SetInputProperty<Vector2>(ref this.m_RotateManipulationInput, value);
			}
		}

		public XRInputValueReader<Vector2> directionalManipulationInput
		{
			get
			{
				return this.m_DirectionalManipulationInput;
			}
			set
			{
				base.SetInputProperty<Vector2>(ref this.m_DirectionalManipulationInput, value);
			}
		}

		public XRInputButtonReader scaleToggleInput
		{
			get
			{
				return this.m_ScaleToggleInput;
			}
			set
			{
				base.SetInputProperty(ref this.m_ScaleToggleInput, value);
			}
		}

		public XRInputValueReader<Vector2> scaleOverTimeInput
		{
			get
			{
				return this.m_ScaleOverTimeInput;
			}
			set
			{
				base.SetInputProperty<Vector2>(ref this.m_ScaleOverTimeInput, value);
			}
		}

		public XRInputValueReader<float> scaleDistanceDeltaInput
		{
			get
			{
				return this.m_ScaleDistanceDeltaInput;
			}
			set
			{
				base.SetInputProperty<float>(ref this.m_ScaleDistanceDeltaInput, value);
			}
		}

		public float angle
		{
			get
			{
				Vector3 vector;
				Vector3 lineDirection;
				this.GetLineOriginAndDirection(out vector, out lineDirection);
				return this.GetProjectileAngle(lineDirection);
			}
		}

		private protected IXRInteractable currentNearestValidTarget { protected get; private set; }

		public Vector3 rayEndPoint { get; private set; }

		public Transform rayEndTransform { get; private set; }

		public float scaleValue { get; protected set; }

		private Transform effectiveRayOrigin
		{
			get
			{
				if (!this.m_HasRayOriginTransform)
				{
					return base.transform;
				}
				return this.m_RayOriginTransform;
			}
		}

		private Vector3 referenceUp
		{
			get
			{
				if (!this.m_HasReferenceFrame)
				{
					return Vector3.up;
				}
				return this.m_ReferenceFrame.up;
			}
		}

		private Vector3 referencePosition
		{
			get
			{
				if (!this.m_HasReferenceFrame)
				{
					return Vector3.zero;
				}
				return this.m_ReferenceFrame.position;
			}
		}

		private int closestAnyHitIndex
		{
			get
			{
				if (this.m_RaycastHitEndpointIndex > 0 && this.m_UIRaycastHitEndpointIndex > 0)
				{
					return Mathf.Min(this.m_RaycastHitEndpointIndex, this.m_UIRaycastHitEndpointIndex);
				}
				if (this.m_RaycastHitEndpointIndex <= 0)
				{
					return this.m_UIRaycastHitEndpointIndex;
				}
				return this.m_RaycastHitEndpointIndex;
			}
		}

		protected void OnValidate()
		{
			this.m_HasRayOriginTransform = (this.m_RayOriginTransform != null);
			this.m_HasReferenceFrame = (this.m_ReferenceFrame != null);
			this.m_SampleFrequency = XRRayInteractor.SanitizeSampleFrequency(this.m_SampleFrequency);
			RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
			if (registeredUIInteractorCache == null)
			{
				return;
			}
			registeredUIInteractorCache.RegisterOrUnregisterXRUIInputModule(this.m_EnableUIInteraction);
		}

		protected override void Awake()
		{
			base.Awake();
			base.buttonReaders.Add(this.m_UIPressInput);
			base.valueReaders.Add(this.m_UIScrollInput);
			base.valueReaders.Add(this.m_TranslateManipulationInput);
			base.valueReaders.Add(this.m_RotateManipulationInput);
			base.valueReaders.Add(this.m_DirectionalManipulationInput);
			base.buttonReaders.Add(this.m_ScaleToggleInput);
			base.valueReaders.Add(this.m_ScaleOverTimeInput);
			base.valueReaders.Add(this.m_ScaleDistanceDeltaInput);
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			this.m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);
			this.CreateSamplePointsListsIfNecessary();
			this.FindReferenceFrame();
			this.CreateRayOrigin();
			if (!Application.isEditor)
			{
				this.m_LiveConeCastDebugVisuals = false;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_EnableUIInteraction)
			{
				RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
				if (registeredUIInteractorCache == null)
				{
					return;
				}
				registeredUIInteractorCache.RegisterWithXRUIInputModule();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			List<XRRayInteractor.SamplePoint> samplePoints = this.m_SamplePoints;
			if (samplePoints != null)
			{
				samplePoints.Clear();
			}
			RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
			if (registeredUIInteractorCache == null)
			{
				return;
			}
			registeredUIInteractorCache.UnregisterFromXRUIInputModule();
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (this.m_LineType == XRRayInteractor.LineType.StraightLine)
			{
				Transform transform = (this.m_RayOriginTransform != null) ? this.m_RayOriginTransform : base.transform;
				Vector3 position = transform.position;
				Vector3 vector = position + transform.forward * this.m_MaxRaycastDistance;
				Gizmos.color = new Color(0.22745098f, 0.47843137f, 0.972549f, 0.92941177f);
				switch (this.m_HitDetectionType)
				{
				case XRRayInteractor.HitDetectionType.Raycast:
					Gizmos.DrawLine(position, vector);
					break;
				case XRRayInteractor.HitDetectionType.SphereCast:
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
				case XRRayInteractor.HitDetectionType.ConeCast:
				{
					float num = Mathf.Tan(this.m_ConeCastAngle * 0.017453292f * 0.5f) * this.m_MaxRaycastDistance;
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
			}
			if (!Application.isPlaying || this.m_SamplePoints == null || this.m_SamplePoints.Count < 2)
			{
				return;
			}
			RaycastHit raycastHit;
			if (this.TryGetCurrent3DRaycastHit(out raycastHit))
			{
				Gizmos.color = new Color(0.22745098f, 0.47843137f, 0.972549f, 0.92941177f);
				Gizmos.DrawLine(raycastHit.point, raycastHit.point + raycastHit.normal.normalized * 0.075f);
			}
			RaycastResult raycastResult;
			if (this.TryGetCurrentUIRaycastResult(out raycastResult))
			{
				Gizmos.color = new Color(0.22745098f, 0.47843137f, 0.972549f, 0.92941177f);
				Gizmos.DrawLine(raycastResult.worldPosition, raycastResult.worldPosition + raycastResult.worldNormal.normalized * 0.075f);
			}
			int closestAnyHitIndex = this.closestAnyHitIndex;
			for (int i = 0; i < this.m_SamplePoints.Count; i++)
			{
				XRRayInteractor.SamplePoint samplePoint = this.m_SamplePoints[i];
				float radius = (this.m_HitDetectionType == XRRayInteractor.HitDetectionType.SphereCast) ? this.m_SphereCastRadius : 0.025f;
				Gizmos.color = ((closestAnyHitIndex == 0 || i < closestAnyHitIndex) ? new Color(0.6392157f, 0.28627452f, 0.6431373f, 0.75f) : new Color(0.8039216f, 0.56078434f, 0.8039216f, 0.5f));
				Gizmos.DrawSphere(samplePoint.position, radius);
				if (i < this.m_SamplePoints.Count - 1)
				{
					XRRayInteractor.SamplePoint samplePoint2 = this.m_SamplePoints[i + 1];
					Gizmos.DrawLine(samplePoint.position, samplePoint2.position);
				}
			}
			XRRayInteractor.LineType lineType = this.m_LineType;
			if (lineType != XRRayInteractor.LineType.ProjectileCurve)
			{
				if (lineType == XRRayInteractor.LineType.BezierCurve)
				{
					XRRayInteractor.DrawQuadraticBezierGizmo(this.m_ControlPoints[0], this.m_ControlPoints[1], this.m_ControlPoints[2]);
				}
			}
			else
			{
				XRRayInteractor.DrawQuadraticBezierGizmo(this.m_HitChordControlPoints[0], this.m_HitChordControlPoints[1], this.m_HitChordControlPoints[2]);
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

		private static void DrawQuadraticBezierGizmo(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
			Gizmos.DrawSphere(p0, 0.025f);
			Gizmos.DrawSphere(p1, 0.025f);
			Gizmos.DrawSphere(p2, 0.025f);
			Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
			Gizmos.DrawLine(p0, p1);
			Gizmos.DrawLine(p1, p2);
			Gizmos.color = new Color(0f, 0f, 8.2f, 0.75f);
			for (float num = 0.1f; num <= 0.9f; num += 0.1f)
			{
				Vector3 from = Vector3.Lerp(p0, p1, num);
				Vector3 to = Vector3.Lerp(p1, p2, num);
				Gizmos.DrawLine(from, to);
			}
		}

		private void FindReferenceFrame()
		{
			this.m_HasReferenceFrame = (this.m_ReferenceFrame != null);
			if (this.m_HasReferenceFrame)
			{
				return;
			}
			XROrigin xrorigin;
			if (!ComponentLocatorUtility<XROrigin>.TryFindComponent(out xrorigin))
			{
				Debug.Log("Reference frame of the curve not set and XROrigin is not found, using global up as default.", this);
				return;
			}
			GameObject origin = xrorigin.Origin;
			if (origin != null)
			{
				this.m_ReferenceFrame = origin.transform;
				this.m_HasReferenceFrame = true;
				return;
			}
			Debug.Log("Reference frame of the curve not set and XROrigin.Origin is not set, using global up as default.", this);
		}

		private void CreateRayOrigin()
		{
			this.m_HasRayOriginTransform = (this.m_RayOriginTransform != null);
			if (this.m_HasRayOriginTransform)
			{
				return;
			}
			this.m_RayOriginTransform = new GameObject("[" + base.gameObject.name + "] Ray Origin").transform;
			this.m_HasRayOriginTransform = true;
			this.m_RayOriginTransform.SetParent(base.transform, false);
			if (base.attachTransform == null)
			{
				base.CreateAttachTransform();
			}
			if (base.attachTransform == null)
			{
				this.m_RayOriginTransform.SetLocalPose(Pose.identity);
				return;
			}
			if (base.attachTransform.parent == base.transform)
			{
				this.m_RayOriginTransform.SetLocalPose(base.attachTransform.GetLocalPose());
				return;
			}
			this.m_RayOriginTransform.SetWorldPose(base.attachTransform.GetWorldPose());
		}

		Transform IXRRayProvider.GetOrCreateRayOrigin()
		{
			this.CreateRayOrigin();
			return this.m_RayOriginTransform;
		}

		Transform IXRRayProvider.GetOrCreateAttachTransform()
		{
			base.CreateAttachTransform();
			return base.attachTransform;
		}

		void IXRRayProvider.SetRayOrigin(Transform newOrigin)
		{
			this.rayOriginTransform = newOrigin;
		}

		void IXRRayProvider.SetAttachTransform(Transform newAttach)
		{
			base.attachTransform = newAttach;
		}

		public bool IsOverUIGameObject()
		{
			return this.m_EnableUIInteraction && this.m_RegisteredUIInteractorCache != null && this.m_RegisteredUIInteractorCache.IsOverUIGameObject();
		}

		private bool IsOverScreenSpaceCanvas()
		{
			GameObject gameObject;
			if (!this.m_EnableUIInteraction || this.m_RegisteredUIInteractorCache == null || !this.m_RegisteredUIInteractorCache.TryGetCurrentUIGameObject(true, out gameObject))
			{
				return false;
			}
			Canvas componentInParent = gameObject.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				RenderMode renderMode = componentInParent.renderMode;
				return renderMode == RenderMode.ScreenSpaceOverlay || renderMode == RenderMode.ScreenSpaceCamera;
			}
			PanelRaycaster panelRaycaster;
			return gameObject.TryGetComponent<PanelRaycaster>(out panelRaycaster);
		}

		public bool GetLinePoints(ref NativeArray<Vector3> linePoints, out int numPoints, Ray? rayOriginOverride = null)
		{
			if (this.m_SamplePoints == null || this.m_SamplePoints.Count < 2)
			{
				numPoints = 0;
				return false;
			}
			if (!this.m_BlendVisualLinePoints)
			{
				numPoints = this.m_SamplePoints.Count;
				XRRayInteractor.EnsureCapacity(ref linePoints, numPoints);
				NativeArray<float3> nativeArray = linePoints.Reinterpret<float3>();
				for (int i = 0; i < numPoints; i++)
				{
					nativeArray[i] = this.m_SamplePoints[i].position;
				}
				return true;
			}
			this.CreateSamplePointsListsIfNecessary();
			this.UpdateSamplePoints(this.m_SamplePoints.Count, XRRayInteractor.s_ScratchSamplePoints, rayOriginOverride);
			if (this.m_LineType == XRRayInteractor.LineType.StraightLine)
			{
				numPoints = 2;
				XRRayInteractor.EnsureCapacity(ref linePoints, numPoints);
				NativeArray<float3> nativeArray = linePoints.Reinterpret<float3>();
				nativeArray[0] = XRRayInteractor.s_ScratchSamplePoints[0].position;
				nativeArray[1] = this.m_SamplePoints[this.m_SamplePoints.Count - 1].position;
				return true;
			}
			int closestAnyHitIndex = this.closestAnyHitIndex;
			this.CreateBezierCurve(XRRayInteractor.s_ScratchSamplePoints, closestAnyHitIndex, XRRayInteractor.s_ScratchControlPoints, rayOriginOverride);
			float3 value;
			float3 @float;
			float3 float2;
			float3 float3;
			CurveUtility.ElevateQuadraticToCubicBezier(XRRayInteractor.s_ScratchControlPoints[0], XRRayInteractor.s_ScratchControlPoints[1], XRRayInteractor.s_ScratchControlPoints[2], out value, out @float, out float2, out float3);
			float3 float4;
			float3 float5;
			CurveUtility.ElevateQuadraticToCubicBezier(this.m_HitChordControlPoints[0], this.m_HitChordControlPoints[1], this.m_HitChordControlPoints[2], out float3, out float2, out float4, out float5);
			if (closestAnyHitIndex > 0 && closestAnyHitIndex != this.m_SamplePoints.Count - 1 && this.m_LineType == XRRayInteractor.LineType.ProjectileCurve)
			{
				numPoints = this.m_SamplePoints.Count;
				XRRayInteractor.EnsureCapacity(ref linePoints, numPoints);
				NativeArray<float3> nativeArray = linePoints.Reinterpret<float3>();
				nativeArray[0] = value;
				float num = 1f / (float)closestAnyHitIndex;
				for (int j = 1; j <= closestAnyHitIndex; j++)
				{
					float t = (float)j * num;
					float3 value2;
					CurveUtility.SampleCubicBezierPoint(value, @float, float4, float5, t, out value2);
					nativeArray[j] = value2;
				}
				for (int k = closestAnyHitIndex + 1; k < this.m_SamplePoints.Count; k++)
				{
					nativeArray[k] = this.m_SamplePoints[k].position;
				}
			}
			else
			{
				numPoints = this.m_SampleFrequency;
				XRRayInteractor.EnsureCapacity(ref linePoints, numPoints);
				NativeArray<float3> nativeArray = linePoints.Reinterpret<float3>();
				nativeArray[0] = value;
				float num2 = 1f / (float)(this.m_SampleFrequency - 1);
				for (int l = 1; l < this.m_SampleFrequency; l++)
				{
					float t2 = (float)l * num2;
					float3 value3;
					CurveUtility.SampleCubicBezierPoint(value, @float, float4, float5, t2, out value3);
					nativeArray[l] = value3;
				}
			}
			return true;
		}

		public bool GetLinePoints(ref Vector3[] linePoints, out int numPoints)
		{
			if (linePoints == null)
			{
				linePoints = Array.Empty<Vector3>();
			}
			NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(linePoints, Allocator.Temp);
			bool linePoints2 = this.GetLinePoints(ref nativeArray, out numPoints, null);
			int length = nativeArray.Length;
			if (linePoints.Length != length)
			{
				linePoints = new Vector3[length];
			}
			nativeArray.CopyTo(linePoints);
			nativeArray.Dispose();
			return linePoints2;
		}

		public void GetLineOriginAndDirection(out Vector3 origin, out Vector3 direction)
		{
			XRRayInteractor.GetLineOriginAndDirection(this.effectiveRayOrigin, out origin, out direction);
		}

		private void GetLineOriginAndDirection(Ray? rayOriginOverride, out Vector3 origin, out Vector3 direction)
		{
			if (rayOriginOverride != null)
			{
				Ray value = rayOriginOverride.Value;
				origin = value.origin;
				direction = value.direction;
				return;
			}
			this.GetLineOriginAndDirection(out origin, out direction);
		}

		private static void GetLineOriginAndDirection(Transform rayOrigin, out Vector3 origin, out Vector3 direction)
		{
			origin = rayOrigin.position;
			direction = rayOrigin.forward;
		}

		private static void EnsureCapacity(ref NativeArray<Vector3> linePoints, int numPoints)
		{
			if (linePoints.IsCreated && linePoints.Length < numPoints)
			{
				linePoints.Dispose();
				linePoints = new NativeArray<Vector3>(numPoints, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				return;
			}
			if (!linePoints.IsCreated)
			{
				linePoints = new NativeArray<Vector3>(numPoints, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
		}

		public bool TryGetHitInfo(out Vector3 position, out Vector3 normal, out int positionInLine, out bool isValidTarget)
		{
			position = default(Vector3);
			normal = default(Vector3);
			positionInLine = 0;
			isValidTarget = false;
			RaycastHit? raycastHit;
			int num;
			RaycastResult? raycastResult;
			int num2;
			bool flag;
			if (!this.TryGetCurrentRaycast(out raycastHit, out num, out raycastResult, out num2, out flag))
			{
				return false;
			}
			RaycastHit value;
			RaycastHit value2;
			if (base.hasSelection && this.m_InteractableRaycastHits.TryGetValue(base.interactablesSelected[0], out value))
			{
				raycastHit = new RaycastHit?(value);
			}
			else if (this.m_ValidTargets.Count > 0 && this.m_InteractableRaycastHits.TryGetValue(this.m_ValidTargets[0], out value2))
			{
				raycastHit = new RaycastHit?(value2);
			}
			if (raycastResult != null && flag)
			{
				position = raycastResult.Value.worldPosition;
				normal = raycastResult.Value.worldNormal;
				positionInLine = num2;
				isValidTarget = (raycastResult.Value.gameObject != null);
			}
			else if (raycastHit != null)
			{
				position = raycastHit.Value.point;
				normal = raycastHit.Value.normal;
				positionInLine = num;
				IXRInteractable interactable;
				isValidTarget = (base.interactionManager.TryGetInteractableForCollider(raycastHit.Value.collider, out interactable) && base.IsHovering(interactable));
			}
			return true;
		}

		public virtual void UpdateUIModel(ref TrackedDeviceModel model)
		{
			if (!base.isActiveAndEnabled || this.m_SamplePoints == null || (this.m_EnableUIInteraction && this.m_BlockUIOnInteractableSelection && base.hasSelection) || this.IsBlockedByInteractionWithinGroup())
			{
				model.Reset(false);
				return;
			}
			Transform effectiveRayOrigin = this.effectiveRayOrigin;
			bool select;
			if (base.forceDeprecatedInput)
			{
				select = this.isUISelectActive;
			}
			else if (this.m_HoverToSelect && this.m_HoverUISelectActive)
			{
				select = base.allowSelect;
			}
			else
			{
				select = this.m_UIPressInput.ReadIsPerformed();
			}
			Vector2 scrollDelta;
			if (base.forceDeprecatedInput)
			{
				scrollDelta = base.uiScrollValue;
			}
			else
			{
				scrollDelta = this.m_UIScrollInput.ReadValue();
			}
			Pose worldPose = effectiveRayOrigin.GetWorldPose();
			model.position = worldPose.position;
			model.orientation = worldPose.rotation;
			model.select = select;
			model.scrollDelta = scrollDelta;
			model.raycastLayerMask = this.m_RaycastMask;
			model.interactionType = UIInteractionType.Ray;
			List<Vector3> raycastPoints = model.raycastPoints;
			raycastPoints.Clear();
			this.UpdateSamplePointsIfNecessary();
			int count = this.m_SamplePoints.Count;
			if (count > 0)
			{
				if (raycastPoints.Capacity < count)
				{
					raycastPoints.Capacity = count;
				}
				for (int i = 0; i < count; i++)
				{
					raycastPoints.Add(this.m_SamplePoints[i].position);
				}
			}
		}

		public bool TryGetUIModel(out TrackedDeviceModel model)
		{
			if (this.m_RegisteredUIInteractorCache == null)
			{
				model = TrackedDeviceModel.invalid;
				return false;
			}
			return this.m_RegisteredUIInteractorCache.TryGetUIModel(out model);
		}

		public bool TryGetCurrent3DRaycastHit(out RaycastHit raycastHit)
		{
			int num;
			return this.TryGetCurrent3DRaycastHit(out raycastHit, out num);
		}

		public bool TryGetCurrent3DRaycastHit(out RaycastHit raycastHit, out int raycastEndpointIndex)
		{
			if (this.m_RaycastHitsCount > 0)
			{
				raycastHit = this.m_RaycastHits[0];
				raycastEndpointIndex = this.m_RaycastHitEndpointIndex;
				return true;
			}
			raycastHit = default(RaycastHit);
			raycastEndpointIndex = 0;
			return false;
		}

		public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult)
		{
			int num;
			return this.TryGetCurrentUIRaycastResult(out raycastResult, out num);
		}

		public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult, out int raycastEndpointIndex)
		{
			TrackedDeviceModel trackedDeviceModel;
			if (this.TryGetUIModel(out trackedDeviceModel) && trackedDeviceModel.currentRaycast.isValid)
			{
				raycastResult = trackedDeviceModel.currentRaycast;
				raycastEndpointIndex = trackedDeviceModel.currentRaycastEndpointIndex;
				return true;
			}
			raycastResult = default(RaycastResult);
			raycastEndpointIndex = 0;
			return false;
		}

		public bool TryGetCurrentRaycast(out RaycastHit? raycastHit, out int raycastHitIndex, out RaycastResult? uiRaycastHit, out int uiRaycastHitIndex, out bool isUIHitClosest)
		{
			raycastHit = new RaycastHit?(this.m_RaycastHit);
			raycastHitIndex = this.m_RaycastHitEndpointIndex;
			uiRaycastHit = new RaycastResult?(this.m_UIRaycastHit);
			uiRaycastHitIndex = this.m_UIRaycastHitEndpointIndex;
			isUIHitClosest = this.m_IsUIHitClosest;
			return this.m_RaycastHitOccurred;
		}

		private void CacheRaycastHit()
		{
			this.m_RaycastHit = default(RaycastHit);
			this.m_UIRaycastHit = default(RaycastResult);
			this.m_IsUIHitClosest = false;
			this.m_RaycastHitOccurred = false;
			this.rayEndTransform = null;
			this.m_RaycastInteractable = null;
			int num = int.MaxValue;
			float num2 = float.MaxValue;
			RaycastHit raycastHit;
			int num3;
			if (this.TryGetCurrent3DRaycastHit(out raycastHit, out num3))
			{
				this.m_RaycastHit = raycastHit;
				num = num3;
				num2 = raycastHit.distance;
				this.m_RaycastHitOccurred = true;
			}
			RaycastResult raycastResult;
			if (this.TryGetCurrentUIRaycastResult(out raycastResult, out this.m_UIRaycastHitEndpointIndex))
			{
				this.m_UIRaycastHit = raycastResult;
				this.m_IsUIHitClosest = (this.m_UIRaycastHitEndpointIndex > 0 && (this.m_UIRaycastHitEndpointIndex < num || (this.m_UIRaycastHitEndpointIndex == num && raycastResult.distance <= num2)));
				this.m_RaycastHitOccurred = true;
			}
			if (!this.m_RaycastHitOccurred)
			{
				this.UpdateSamplePointsIfNecessary();
				this.rayEndPoint = this.m_SamplePoints[this.m_SamplePoints.Count - 1].position;
				return;
			}
			if (this.m_IsUIHitClosest)
			{
				this.rayEndPoint = this.m_UIRaycastHit.worldPosition;
				this.rayEndTransform = this.m_UIRaycastHit.gameObject.transform;
				return;
			}
			this.rayEndPoint = this.m_RaycastHit.point;
			this.rayEndTransform = (base.interactionManager.TryGetInteractableForCollider(this.m_RaycastHit.collider, out this.m_RaycastInteractable) ? this.m_RaycastInteractable.GetAttachTransform(this) : this.m_RaycastHit.transform);
		}

		private void UpdateUIHover()
		{
			float num = Time.time - this.m_LastTimeHoveredUIChanged;
			if (this.m_IsUIHitClosest && num > this.m_HoverTimeToSelect && (num < this.m_HoverTimeToSelect + this.m_TimeToAutoDeselect || this.m_BlockUIAutoDeselect))
			{
				this.m_HoverUISelectActive = true;
				return;
			}
			this.m_HoverUISelectActive = false;
		}

		private void UpdateBezierControlPoints(in float3 lineOrigin, in float3 lineDirection, in float3 curveReferenceUp)
		{
			this.m_ControlPoints[0] = lineOrigin;
			this.m_ControlPoints[1] = this.m_ControlPoints[0] + lineDirection * this.m_ControlPointDistance + curveReferenceUp * this.m_ControlPointHeight;
			this.m_ControlPoints[2] = this.m_ControlPoints[0] + lineDirection * this.m_EndPointDistance + curveReferenceUp * this.m_EndPointHeight;
		}

		private float GetProjectileAngle(Vector3 lineDirection)
		{
			Vector3 referenceUp = this.referenceUp;
			Vector3 to = Vector3.ProjectOnPlane(lineDirection, referenceUp);
			if (!Mathf.Approximately(Vector3.Angle(lineDirection, to), 0f))
			{
				return Vector3.SignedAngle(lineDirection, to, Vector3.Cross(referenceUp, lineDirection));
			}
			return 0f;
		}

		[BurstCompile]
		private void CalculateProjectileParameters(in float3 lineOrigin, in float3 lineDirection, out float3 initialVelocity, out float3 constantAcceleration, out float flightTime)
		{
			initialVelocity = lineDirection * this.m_Velocity;
			float3 @float = this.referenceUp;
			float3 lhs = this.referencePosition;
			constantAcceleration = @float * -this.m_Acceleration;
			float angleRad = math.sin(this.GetProjectileAngle(lineDirection) * 0.017453292f);
			float height = math.length(math.project(lhs - lineOrigin, @float)) + this.m_AdditionalGroundHeight;
			CurveUtility.CalculateProjectileFlightTime(this.m_Velocity, this.m_Acceleration, angleRad, height, this.m_AdditionalFlightTime, out flightTime);
		}

		protected virtual void RotateAttachTransform(Transform attach, float directionAmount)
		{
			if (Mathf.Approximately(directionAmount, 0f))
			{
				return;
			}
			float angle = directionAmount * (this.m_RotateSpeed * Time.deltaTime);
			if (this.m_RotateReferenceFrame != null)
			{
				attach.Rotate(this.m_RotateReferenceFrame.up, angle, Space.World);
				return;
			}
			attach.Rotate(Vector3.up, angle);
		}

		protected virtual void RotateAttachTransform(Transform attach, Vector2 direction, Quaternion referenceRotation)
		{
			if (Mathf.Approximately(direction.sqrMagnitude, 0f))
			{
				return;
			}
			Quaternion rhs = Quaternion.AngleAxis(Mathf.Atan2(direction.x, direction.y) * 57.29578f, Vector3.up);
			attach.rotation = referenceRotation * rhs;
		}

		protected virtual void TranslateAttachTransform(Transform rayOrigin, Transform attach, float directionAmount)
		{
			if (Mathf.Approximately(directionAmount, 0f))
			{
				return;
			}
			Vector3 vector;
			Vector3 vector2;
			XRRayInteractor.GetLineOriginAndDirection(rayOrigin, out vector, out vector2);
			Vector3 vector3 = attach.position + vector2 * (directionAmount * this.m_TranslateSpeed * Time.deltaTime);
			float num = Vector3.Dot(vector3 - vector, vector2);
			attach.position = ((num > 0f) ? vector3 : vector);
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.UpdateSamplePointsIfNecessary();
				if (this.m_SamplePoints != null && this.m_SamplePoints.Count >= 2)
				{
					this.UpdateRaycastHits();
					this.CacheRaycastHit();
					this.UpdateUIHover();
					this.CreateBezierCurve(this.m_SamplePoints, this.closestAnyHitIndex, this.m_HitChordControlPoints, null);
				}
				this.GetValidTargets(this.m_ValidTargets);
				IXRInteractable ixrinteractable = (this.m_ValidTargets.Count > 0) ? this.m_ValidTargets[0] : null;
				if (ixrinteractable != this.currentNearestValidTarget && !base.hasSelection)
				{
					this.currentNearestValidTarget = ixrinteractable;
					this.m_LastTimeHoveredObjectChanged = Time.time;
					this.m_PassedHoverTimeToSelect = false;
				}
				else if (!this.m_PassedHoverTimeToSelect && ixrinteractable != null && Mathf.Clamp01((Time.time - this.m_LastTimeHoveredObjectChanged) / this.GetHoverTimeToSelect(this.currentNearestValidTarget)) >= 1f && !base.hasSelection)
				{
					this.m_PassedHoverTimeToSelect = true;
				}
				if (this.m_AutoDeselect && base.hasSelection && !this.m_PassedTimeToAutoDeselect && Mathf.Clamp01((Time.time - this.m_LastTimeAutoSelected) / this.GetTimeToAutoDeselect(this.currentNearestValidTarget)) >= 1f)
				{
					this.m_PassedTimeToAutoDeselect = true;
				}
			}
		}

		public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.scaleValue = 0f;
				if (this.m_ManipulateAttachTransform && base.hasSelection)
				{
					if (base.forceDeprecatedInput)
					{
						this.ProcessManipulationInputDeviceBasedController();
						this.ProcessManipulationInputActionBasedController();
						this.ProcessManipulationInputScreenSpaceController();
						return;
					}
					this.ProcessManipulationInput();
				}
			}
		}

		private void ProcessManipulationInput()
		{
			if (this.m_ScaleToggleInput.ReadWasPerformedThisFrame())
			{
				this.m_ScaleInputActive = !this.m_ScaleInputActive;
			}
			Vector2 vector3;
			if (!this.m_ScaleInputActive)
			{
				XRRayInteractor.RotateMode rotateMode = this.m_RotateMode;
				Vector2 vector;
				if (rotateMode != XRRayInteractor.RotateMode.RotateOverTime)
				{
					if (rotateMode == XRRayInteractor.RotateMode.MatchDirection)
					{
						Vector2 direction;
						if (this.m_DirectionalManipulationInput.TryReadValue(out direction))
						{
							Quaternion referenceRotation = (this.m_RotateReferenceFrame != null) ? this.m_RotateReferenceFrame.rotation : this.effectiveRayOrigin.rotation;
							this.RotateAnchor(base.attachTransform, direction, referenceRotation);
						}
					}
				}
				else if (this.m_RotateManipulationInput.TryReadValue(out vector))
				{
					this.RotateAnchor(base.attachTransform, vector.x);
				}
				Vector2 vector2;
				if (this.m_TranslateManipulationInput.TryReadValue(out vector2))
				{
					this.TranslateAnchor(this.effectiveRayOrigin, base.attachTransform, vector2.y);
				}
			}
			else if (this.m_ScaleMode == ScaleMode.ScaleOverTime && this.m_ScaleOverTimeInput.TryReadValue(out vector3))
			{
				this.scaleValue = vector3.y;
			}
			float scaleValue;
			if (this.m_ScaleMode == ScaleMode.DistanceDelta && this.m_ScaleDistanceDeltaInput.TryReadValue(out scaleValue))
			{
				this.scaleValue = scaleValue;
			}
		}

		public override void GetValidTargets(List<IXRInteractable> targets)
		{
			targets.Clear();
			this.m_InteractableRaycastHits.Clear();
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.m_RaycastHitsCount > 0)
			{
				RaycastResult raycastResult;
				int num;
				bool flag = this.TryGetCurrentUIRaycastResult(out raycastResult, out num);
				for (int i = 0; i < this.m_RaycastHitsCount; i++)
				{
					RaycastHit value = this.m_RaycastHits[i];
					bool flag2 = raycastResult.gameObject != value.collider.gameObject;
					IXRInteractable ixrinteractable;
					if ((flag && flag2 && num > 0 && (num < this.m_RaycastHitEndpointIndex || (num == this.m_RaycastHitEndpointIndex && raycastResult.distance <= value.distance))) || !base.interactionManager.TryGetInteractableForCollider(value.collider, out ixrinteractable))
					{
						break;
					}
					if (!targets.Contains(ixrinteractable))
					{
						targets.Add(ixrinteractable);
						this.m_InteractableRaycastHits.Add(ixrinteractable, value);
						if (this.m_HitClosestOnly)
						{
							break;
						}
					}
				}
			}
			IXRTargetFilter targetFilter = base.targetFilter;
			if (targetFilter != null && targetFilter.canProcess)
			{
				targetFilter.Process(this, targets, XRRayInteractor.s_Results);
				targets.Clear();
				targets.AddRange(XRRayInteractor.s_Results);
			}
		}

		private void CreateSamplePointsListsIfNecessary()
		{
			if (this.m_SamplePoints != null && XRRayInteractor.s_ScratchSamplePoints != null)
			{
				return;
			}
			int capacity = (this.m_LineType == XRRayInteractor.LineType.StraightLine) ? 2 : this.m_SampleFrequency;
			if (this.m_SamplePoints == null)
			{
				this.m_SamplePoints = new List<XRRayInteractor.SamplePoint>(capacity);
			}
			if (XRRayInteractor.s_ScratchSamplePoints == null)
			{
				XRRayInteractor.s_ScratchSamplePoints = new List<XRRayInteractor.SamplePoint>(capacity);
			}
		}

		private void UpdateSamplePointsIfNecessary()
		{
			this.CreateSamplePointsListsIfNecessary();
			if (this.m_SamplePointsFrameUpdated != Time.frameCount)
			{
				this.UpdateSamplePoints(this.m_SampleFrequency, this.m_SamplePoints, null);
				this.m_SamplePointsFrameUpdated = Time.frameCount;
			}
		}

		private void UpdateSamplePoints(int count, List<XRRayInteractor.SamplePoint> samplePoints, Ray? rayOriginOverride = null)
		{
			Vector3 v;
			Vector3 v2;
			this.GetLineOriginAndDirection(rayOriginOverride, out v, out v2);
			samplePoints.Clear();
			XRRayInteractor.SamplePoint item = new XRRayInteractor.SamplePoint
			{
				position = v,
				parameter = 0f
			};
			samplePoints.Add(item);
			switch (this.m_LineType)
			{
			case XRRayInteractor.LineType.StraightLine:
				item.position = samplePoints[0].position + v2 * this.m_MaxRaycastDistance;
				item.parameter = 1f;
				samplePoints.Add(item);
				return;
			case XRRayInteractor.LineType.ProjectileCurve:
			{
				float3 @float = v;
				float3 float2 = v2;
				float3 float3;
				float3 float4;
				float num;
				this.CalculateProjectileParameters(@float, float2, out float3, out float4, out num);
				float num2 = num / (float)(count - 1);
				for (int i = 1; i < count; i++)
				{
					float num3 = (float)i * num2;
					float3 position;
					CurveUtility.SampleProjectilePoint(@float, float3, float4, num3, out position);
					item.position = position;
					item.parameter = num3;
					samplePoints.Add(item);
				}
				return;
			}
			case XRRayInteractor.LineType.BezierCurve:
			{
				float3 float2 = v;
				float3 float5 = v2;
				float3 float6 = this.referenceUp;
				this.UpdateBezierControlPoints(float2, float5, float6);
				float3 float7 = this.m_ControlPoints[0];
				float3 float8 = this.m_ControlPoints[1];
				float3 float9 = this.m_ControlPoints[2];
				float num4 = 1f / (float)(count - 1);
				for (int j = 1; j < count; j++)
				{
					float num5 = (float)j * num4;
					float3 position2;
					CurveUtility.SampleQuadraticBezierPoint(float7, float8, float9, num5, out position2);
					item.position = position2;
					item.parameter = num5;
					samplePoints.Add(item);
				}
				return;
			}
			default:
				return;
			}
		}

		private void UpdateRaycastHits()
		{
			this.m_RaycastHitsCount = 0;
			this.m_RaycastHitEndpointIndex = 0;
			this.m_ConeCastDebugInfo.Clear();
			float num = 0f;
			bool flag = false;
			for (int i = 1; i < this.m_SamplePoints.Count; i++)
			{
				float3 position = this.m_SamplePoints[0].position;
				float3 position2 = this.m_SamplePoints[i - 1].position;
				float3 position3 = this.m_SamplePoints[i].position;
				this.CheckCollidersBetweenPoints(position2, position3, position);
				if (this.m_RaycastHitsCount > 0 && !flag)
				{
					for (int j = 0; j < this.m_RaycastHitsCount; j++)
					{
						RaycastHit[] raycastHits = this.m_RaycastHits;
						int num2 = j;
						raycastHits[num2].distance = raycastHits[num2].distance + num;
					}
					this.m_RaycastHitEndpointIndex = i;
					flag = true;
				}
				if (flag)
				{
					break;
				}
			}
		}

		private void CheckCollidersBetweenPoints(Vector3 from, Vector3 to, Vector3 origin)
		{
			Array.Clear(this.m_RaycastHits, 0, 10);
			this.m_RaycastHitsCount = 0;
			Vector3 normalized = (to - from).normalized;
			float maxDistance = Vector3.Distance(to, from);
			QueryTriggerInteraction queryTriggerInteraction = (this.m_RaycastSnapVolumeInteraction == XRRayInteractor.QuerySnapVolumeInteraction.Collide) ? QueryTriggerInteraction.Collide : this.m_RaycastTriggerInteraction;
			switch (this.m_HitDetectionType)
			{
			case XRRayInteractor.HitDetectionType.Raycast:
				this.m_RaycastHitsCount = this.m_LocalPhysicsScene.Raycast(from, normalized, this.m_RaycastHits, maxDistance, this.m_RaycastMask, queryTriggerInteraction);
				break;
			case XRRayInteractor.HitDetectionType.SphereCast:
				this.m_RaycastHitsCount = this.m_LocalPhysicsScene.SphereCast(from, this.m_SphereCastRadius, normalized, this.m_RaycastHits, maxDistance, this.m_RaycastMask, queryTriggerInteraction);
				break;
			case XRRayInteractor.HitDetectionType.ConeCast:
				this.m_RaycastHitsCount = this.FilteredConecast(from, normalized, origin, this.m_RaycastHits, maxDistance, this.m_RaycastMask, queryTriggerInteraction);
				break;
			}
			if (this.m_RaycastHitsCount > 0)
			{
				if (this.m_HitDetectionType != XRRayInteractor.HitDetectionType.ConeCast)
				{
					this.m_RaycastHitsCount = this.FilterOutTriggerColliders(base.interactionManager, this.m_RaycastHits, this.m_RaycastHitsCount);
				}
				SortingHelpers.Sort<RaycastHit>(this.m_RaycastHits, this.m_RaycastHitComparer, this.m_RaycastHitsCount);
			}
		}

		private int FilteredConecast(in Vector3 from, in Vector3 direction, in Vector3 origin, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			XRRayInteractor.s_OptimalHits.Clear();
			float num = math.min(maxDistance, 1000f);
			int num2 = 0;
			int num3 = this.m_LocalPhysicsScene.Raycast(from, direction, XRRayInteractor.s_SpherecastScratch, maxDistance, layerMask, queryTriggerInteraction);
			if (num3 > 0)
			{
				num3 = this.FilterOutTriggerColliders(base.interactionManager, XRRayInteractor.s_SpherecastScratch, num3);
				for (int i = 0; i < num3; i++)
				{
					RaycastHit raycastHit = XRRayInteractor.s_SpherecastScratch[i];
					if (raycastHit.distance <= num)
					{
						IXRInteractable ixrinteractable;
						if (!base.interactionManager.TryGetInteractableForCollider(raycastHit.collider, out ixrinteractable))
						{
							num = math.min(raycastHit.distance, num);
							raycastHit.distance += 1.5f;
						}
						results[num2] = raycastHit;
						XRRayInteractor.s_OptimalHits.Add(raycastHit.collider);
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
				int num7 = this.m_LocalPhysicsScene.SphereCast(from + b, num5, direction, XRRayInteractor.s_SpherecastScratch, num6, layerMask, queryTriggerInteraction);
				if (num7 > 0)
				{
					num7 = this.FilterOutTriggerColliders(base.interactionManager, XRRayInteractor.s_SpherecastScratch, num7);
					int num8 = 0;
					while (num8 < num7 && num2 < results.Length)
					{
						RaycastHit raycastHit2 = XRRayInteractor.s_SpherecastScratch[num8];
						IXRInteractable ixrinteractable;
						if (num4 + raycastHit2.distance <= num && !XRRayInteractor.s_OptimalHits.Contains(raycastHit2.collider) && base.interactionManager.TryGetInteractableForCollider(raycastHit2.collider, out ixrinteractable))
						{
							if (Mathf.Approximately(raycastHit2.distance, 0f))
							{
								vector = raycastHit2.point;
								Vector3 zero = Vector3.zero;
								if (BurstMathUtility.FastVectorEquals(vector, zero, 0.0001f))
								{
									goto IL_2A3;
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
						IL_2A3:
						num8++;
					}
				}
				num4 += num6;
			}
			XRRayInteractor.s_OptimalHits.Clear();
			Array.Clear(XRRayInteractor.s_SpherecastScratch, 0, 10);
			return num2;
		}

		private int FilterOutTriggerColliders(XRInteractionManager manager, RaycastHit[] raycastHits, int raycastHitCount)
		{
			bool flag = this.m_RaycastTriggerInteraction == QueryTriggerInteraction.Collide || (this.m_RaycastTriggerInteraction == QueryTriggerInteraction.UseGlobal && Physics.queriesHitTriggers);
			if (this.m_RaycastSnapVolumeInteraction == XRRayInteractor.QuerySnapVolumeInteraction.Ignore && flag)
			{
				raycastHitCount = XRRayInteractor.FilterTriggerColliders(manager, raycastHits, raycastHitCount, (XRInteractableSnapVolume snapVolume) => snapVolume != null);
			}
			else if (this.m_RaycastSnapVolumeInteraction == XRRayInteractor.QuerySnapVolumeInteraction.Collide && !flag)
			{
				raycastHitCount = XRRayInteractor.FilterTriggerColliders(manager, raycastHits, raycastHitCount, (XRInteractableSnapVolume snapVolume) => snapVolume == null);
			}
			return raycastHitCount;
		}

		private static int FilterTriggerColliders(XRInteractionManager interactionManager, RaycastHit[] raycastHits, int count, Func<XRInteractableSnapVolume, bool> removeRule)
		{
			for (int i = 0; i < count; i++)
			{
				Collider collider = raycastHits[i].collider;
				if (collider.isTrigger)
				{
					IXRInteractable ixrinteractable;
					XRInteractableSnapVolume arg;
					interactionManager.TryGetInteractableForCollider(collider, out ixrinteractable, out arg);
					if (removeRule(arg))
					{
						XRRayInteractor.RemoveAt<RaycastHit>(raycastHits, i, count);
						count--;
						i--;
					}
				}
			}
			return count;
		}

		private static void RemoveAt<T>(T[] array, int index, int count) where T : struct
		{
			Array.Copy(array, index + 1, array, index, count - index - 1);
			Array.Clear(array, count - 1, 1);
		}

		private void CreateBezierCurve(List<XRRayInteractor.SamplePoint> samplePoints, int endSamplePointIndex, float3[] quadraticControlPoints, Ray? rayOriginOverride = null)
		{
			XRRayInteractor.SamplePoint samplePoint = (endSamplePointIndex > 0 && endSamplePointIndex < samplePoints.Count) ? samplePoints[endSamplePointIndex] : samplePoints[samplePoints.Count - 1];
			float3 position = samplePoint.position;
			float3 position2 = samplePoints[0].position;
			float3 @float = 0.5f * (position2 + position);
			switch (this.m_LineType)
			{
			case XRRayInteractor.LineType.StraightLine:
				quadraticControlPoints[0] = position2;
				quadraticControlPoints[1] = @float;
				quadraticControlPoints[2] = position;
				return;
			case XRRayInteractor.LineType.ProjectileCurve:
			{
				Vector3 v;
				Vector3 v2;
				this.GetLineOriginAndDirection(rayOriginOverride, out v, out v2);
				float3 float2 = v;
				float3 float3 = v2;
				float3 float4;
				float3 float5;
				float num;
				this.CalculateProjectileParameters(float2, float3, out float4, out float5, out num);
				float time = 0.5f * samplePoint.parameter;
				float3 lhs;
				CurveUtility.SampleProjectilePoint(position2, float4, float5, time, out lhs);
				float3 float6 = @float + 2f * (lhs - @float);
				quadraticControlPoints[0] = position2;
				quadraticControlPoints[1] = float6;
				quadraticControlPoints[2] = position;
				return;
			}
			case XRRayInteractor.LineType.BezierCurve:
				quadraticControlPoints[0] = this.m_ControlPoints[0];
				quadraticControlPoints[1] = this.m_ControlPoints[1];
				quadraticControlPoints[2] = this.m_ControlPoints[2];
				return;
			default:
				return;
			}
		}

		public override bool isSelectActive
		{
			get
			{
				if (this.m_BlockInteractionsWithScreenSpaceUI && !base.hasSelection && this.IsOverScreenSpaceCanvas())
				{
					return false;
				}
				if (this.m_HoverToSelect && this.m_PassedHoverTimeToSelect)
				{
					return base.allowSelect;
				}
				return base.isSelectActive;
			}
		}

		public override bool CanHover(IXRHoverInteractable interactable)
		{
			return base.CanHover(interactable) && (!base.hasSelection || base.IsSelecting(interactable)) && (!base.forceDeprecatedInput || !this.m_IsScreenSpaceController || this.m_ScreenSpaceController.currentControllerState.isTracked);
		}

		public override bool CanSelect(IXRSelectInteractable interactable)
		{
			return (this.currentNearestValidTarget != interactable || !this.m_AutoDeselect || !base.hasSelection || !this.m_PassedHoverTimeToSelect || !this.m_PassedTimeToAutoDeselect) && (!this.m_HoverToSelect || !this.m_PassedHoverTimeToSelect || this.currentNearestValidTarget == interactable) && base.CanSelect(interactable) && (!base.hasSelection || base.IsSelecting(interactable));
		}

		protected virtual float GetHoverTimeToSelect(IXRInteractable interactable)
		{
			return this.m_HoverTimeToSelect;
		}

		protected virtual float GetTimeToAutoDeselect(IXRInteractable interactable)
		{
			return this.m_TimeToAutoDeselect;
		}

		protected override void OnSelectEntering(SelectEnterEventArgs args)
		{
			base.OnSelectEntering(args);
			if (this.m_AutoDeselect && this.m_PassedHoverTimeToSelect)
			{
				this.m_LastTimeAutoSelected = Time.time;
				this.m_PassedTimeToAutoDeselect = false;
			}
			if (base.interactablesSelected.Count == 1)
			{
				bool flag = !this.m_UseForceGrab;
				IFarAttachProvider farAttachProvider = args.interactableObject as IFarAttachProvider;
				if (farAttachProvider != null && farAttachProvider.farAttachMode != InteractableFarAttachMode.DeferToInteractor)
				{
					flag = (farAttachProvider.farAttachMode == InteractableFarAttachMode.Far);
				}
				RaycastHit raycastHit;
				if (flag && this.TryGetCurrent3DRaycastHit(out raycastHit))
				{
					base.attachTransform.position = raycastHit.point;
				}
			}
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			base.OnSelectExiting(args);
			this.m_PassedHoverTimeToSelect = false;
			this.m_LastTimeHoveredObjectChanged = Time.time;
			this.m_PassedTimeToAutoDeselect = false;
			if (!base.hasSelection)
			{
				this.RestoreAttachTransform();
			}
		}

		void IUIHoverInteractor.OnUIHoverEntered(UIHoverEventArgs args)
		{
			this.OnUIHoverEntered(args);
		}

		void IUIHoverInteractor.OnUIHoverExited(UIHoverEventArgs args)
		{
			this.OnUIHoverExited(args);
		}

		protected virtual void OnUIHoverEntered(UIHoverEventArgs args)
		{
			GameObject selectableObject = args.deviceModel.selectableObject;
			if (this.m_LastUIObject != selectableObject)
			{
				this.m_LastUIObject = selectableObject;
				if (selectableObject != null)
				{
					this.m_LastTimeHoveredUIChanged = Time.time;
					this.m_BlockUIAutoDeselect = (this.m_LastUIObject.GetComponent<UnityEngine.UI.Slider>() != null);
				}
				else
				{
					this.m_BlockUIAutoDeselect = false;
				}
				this.m_HoverUISelectActive = false;
			}
			UIHoverEnterEvent uihoverEntered = this.m_UIHoverEntered;
			if (uihoverEntered == null)
			{
				return;
			}
			uihoverEntered.Invoke(args);
		}

		protected virtual void OnUIHoverExited(UIHoverEventArgs args)
		{
			GameObject selectableObject = args.deviceModel.selectableObject;
			if (this.m_LastUIObject != selectableObject)
			{
				this.m_LastUIObject = null;
				this.m_LastTimeHoveredUIChanged = Time.time;
				this.m_BlockUIAutoDeselect = false;
				this.m_HoverUISelectActive = false;
			}
			UIHoverExitEvent uihoverExited = this.m_UIHoverExited;
			if (uihoverExited == null)
			{
				return;
			}
			uihoverExited.Invoke(args);
		}

		private void RestoreAttachTransform()
		{
			Pose localAttachPoseOnSelect = base.GetLocalAttachPoseOnSelect(base.firstInteractableSelected);
			base.attachTransform.SetLocalPose(localAttachPoseOnSelect);
		}

		private static int SanitizeSampleFrequency(int value)
		{
			return Mathf.Max(value, 2);
		}

		[Obsolete("Velocity has been deprecated. Use velocity instead. (UnityUpgradable) -> velocity", true)]
		public float Velocity
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("Acceleration has been deprecated. Use acceleration instead. (UnityUpgradable) -> acceleration", true)]
		public float Acceleration
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("AdditionalFlightTime has been deprecated. Use additionalFlightTime instead. (UnityUpgradable) -> additionalFlightTime", true)]
		public float AdditionalFlightTime
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("Angle has been deprecated. Use angle instead. (UnityUpgradable) -> angle", true)]
		public float Angle
		{
			get
			{
				return 0f;
			}
		}

		[Obsolete("originalAttachTransform has been deprecated. Use rayOriginTransform instead. (UnityUpgradable) -> rayOriginTransform", true)]
		protected Transform originalAttachTransform
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("GetLinePoints with ref int parameter has been deprecated. Use signature with out int parameter instead.", true)]
		public bool GetLinePoints(ref Vector3[] linePoints, ref int numPoints, int _ = 0)
		{
			return false;
		}

		[Obsolete("TryGetHitInfo with ref parameters has been deprecated. Use signature with out parameters instead.", true)]
		public bool TryGetHitInfo(ref Vector3 position, ref Vector3 normal, ref int positionInLine, ref bool isValidTarget, int _ = 0)
		{
			return false;
		}

		[Obsolete("GetCurrentRaycastHit has been deprecated. Use TryGetCurrent3DRaycastHit instead. (UnityUpgradable) -> TryGetCurrent3DRaycastHit(*)", true)]
		public bool GetCurrentRaycastHit(out RaycastHit raycastHit)
		{
			raycastHit = default(RaycastHit);
			return false;
		}

		[Obsolete("allowAnchorControl has been renamed in version 3.0.0. Use manipulateAttachTransform instead. (UnityUpgradable) -> manipulateAttachTransform")]
		public bool allowAnchorControl
		{
			get
			{
				return this.manipulateAttachTransform;
			}
			set
			{
				this.manipulateAttachTransform = value;
			}
		}

		[Obsolete("anchorRotateReferenceFrame has been renamed in version 3.0.0. Use rotateReferenceFrame instead. (UnityUpgradable) -> rotateReferenceFrame")]
		public Transform anchorRotateReferenceFrame
		{
			get
			{
				return this.rotateReferenceFrame;
			}
			set
			{
				this.rotateReferenceFrame = value;
			}
		}

		[Obsolete("anchorRotationMode has been deprecated in version 3.0.0. Use rotateMode instead.")]
		public XRRayInteractor.AnchorRotationMode anchorRotationMode
		{
			get
			{
				return (XRRayInteractor.AnchorRotationMode)this.rotateMode;
			}
			set
			{
				this.rotateMode = (XRRayInteractor.RotateMode)value;
			}
		}

		[Obsolete("isUISelectActive has been deprecated in version 3.0.0. Use uiPressInput to read button input instead.")]
		protected override bool isUISelectActive
		{
			get
			{
				if (this.m_HoverToSelect && this.m_HoverUISelectActive)
				{
					return base.allowSelect;
				}
				return base.isUISelectActive;
			}
		}

		[Obsolete("ProcessManipulationInputDeviceBasedController has been deprecated in version 3.0.0.")]
		private void ProcessManipulationInputDeviceBasedController()
		{
			if (!this.m_IsDeviceBasedController || !this.m_DeviceBasedController.inputDevice.isValid)
			{
				return;
			}
			bool flag;
			this.m_DeviceBasedController.inputDevice.IsPressed(this.m_DeviceBasedController.moveObjectIn, out flag, this.m_DeviceBasedController.axisToPressThreshold);
			bool flag2;
			this.m_DeviceBasedController.inputDevice.IsPressed(this.m_DeviceBasedController.moveObjectOut, out flag2, this.m_DeviceBasedController.axisToPressThreshold);
			if (flag || flag2)
			{
				float directionAmount = flag ? 1f : -1f;
				this.TranslateAnchor(this.effectiveRayOrigin, base.attachTransform, directionAmount);
			}
			XRRayInteractor.RotateMode rotateMode = this.m_RotateMode;
			if (rotateMode != XRRayInteractor.RotateMode.RotateOverTime)
			{
				if (rotateMode != XRRayInteractor.RotateMode.MatchDirection)
				{
					return;
				}
				Vector2 direction;
				if (this.m_DeviceBasedController.inputDevice.TryReadAxis2DValue(this.m_DeviceBasedController.directionalAnchorRotation, out direction))
				{
					Quaternion referenceRotation = (this.m_RotateReferenceFrame != null) ? this.m_RotateReferenceFrame.rotation : this.effectiveRayOrigin.rotation;
					this.RotateAnchor(base.attachTransform, direction, referenceRotation);
				}
			}
			else
			{
				bool flag3;
				this.m_DeviceBasedController.inputDevice.IsPressed(this.m_DeviceBasedController.rotateObjectLeft, out flag3, this.m_DeviceBasedController.axisToPressThreshold);
				bool flag4;
				this.m_DeviceBasedController.inputDevice.IsPressed(this.m_DeviceBasedController.rotateObjectRight, out flag4, this.m_DeviceBasedController.axisToPressThreshold);
				if (flag3 || flag4)
				{
					float directionAmount2 = flag3 ? -1f : 1f;
					this.RotateAnchor(base.attachTransform, directionAmount2);
					return;
				}
			}
		}

		[Obsolete("ProcessManipulationInputActionBasedController has been deprecated in version 3.0.0.")]
		private void ProcessManipulationInputActionBasedController()
		{
			if (!this.m_IsActionBasedController)
			{
				return;
			}
			if (XRRayInteractor.TryReadButton(this.m_ActionBasedController.scaleToggleAction.action))
			{
				this.m_ScaleInputActive = !this.m_ScaleInputActive;
			}
			Vector2 vector3;
			if (!this.m_ScaleInputActive)
			{
				XRRayInteractor.RotateMode rotateMode = this.m_RotateMode;
				Vector2 vector;
				if (rotateMode != XRRayInteractor.RotateMode.RotateOverTime)
				{
					if (rotateMode == XRRayInteractor.RotateMode.MatchDirection)
					{
						Vector2 direction;
						if (XRRayInteractor.TryRead2DAxis(this.m_ActionBasedController.directionalAnchorRotationAction.action, out direction))
						{
							Quaternion referenceRotation = (this.m_RotateReferenceFrame != null) ? this.m_RotateReferenceFrame.rotation : this.effectiveRayOrigin.rotation;
							this.RotateAnchor(base.attachTransform, direction, referenceRotation);
						}
					}
				}
				else if (XRRayInteractor.TryRead2DAxis(this.m_ActionBasedController.rotateAnchorAction.action, out vector))
				{
					this.RotateAnchor(base.attachTransform, vector.x);
				}
				Vector2 vector2;
				if (XRRayInteractor.TryRead2DAxis(this.m_ActionBasedController.translateAnchorAction.action, out vector2))
				{
					this.TranslateAnchor(this.effectiveRayOrigin, base.attachTransform, vector2.y);
					return;
				}
			}
			else if (this.m_ScaleMode == ScaleMode.ScaleOverTime && XRRayInteractor.TryRead2DAxis(this.m_ActionBasedController.scaleDeltaAction.action, out vector3))
			{
				this.scaleValue = vector3.y;
			}
		}

		[Obsolete("ProcessManipulationInputScreenSpaceController has been deprecated in version 3.0.0.")]
		private void ProcessManipulationInputScreenSpaceController()
		{
			if (!this.m_IsScreenSpaceController)
			{
				return;
			}
			XRRayInteractor.RotateMode rotateMode = this.m_RotateMode;
			if (rotateMode != XRRayInteractor.RotateMode.RotateOverTime && rotateMode == XRRayInteractor.RotateMode.MatchDirection)
			{
				if (this.m_ScreenSpaceController.twistDeltaRotationAction.action != null && this.m_ScreenSpaceController.twistDeltaRotationAction.action.phase.IsInProgress())
				{
					float directionAmount = -this.m_ScreenSpaceController.twistDeltaRotationAction.action.ReadValue<float>();
					this.RotateAnchor(base.attachTransform, directionAmount);
				}
				else if (this.m_ScreenSpaceController.dragDeltaAction.action != null && this.m_ScreenSpaceController.dragDeltaAction.action.phase.IsInProgress())
				{
					InputAction action = this.m_ScreenSpaceController.screenTouchCountAction.action;
					if (action != null && action.ReadValue<int>() > 1)
					{
						Vector2 v = this.m_ScreenSpaceController.dragDeltaAction.action.ReadValue<Vector2>();
						float directionAmount2 = (Quaternion.Inverse(Quaternion.LookRotation(base.attachTransform.forward, Vector3.up)) * base.attachTransform.rotation * v).x / Screen.dpi * -50f;
						this.RotateAnchor(base.attachTransform, directionAmount2);
					}
				}
			}
			if (this.m_ScaleMode == ScaleMode.DistanceDelta)
			{
				this.scaleValue = this.m_ScreenSpaceController.scaleDelta;
			}
		}

		[Obsolete("RotateAnchor has been renamed in version 3.0.0. Use RotateAttachTransform instead.")]
		protected virtual void RotateAnchor(Transform anchor, float directionAmount)
		{
			this.RotateAttachTransform(anchor, directionAmount);
		}

		[Obsolete("RotateAnchor has been renamed in version 3.0.0. Use RotateAttachTransform instead.")]
		protected virtual void RotateAnchor(Transform anchor, Vector2 direction, Quaternion referenceRotation)
		{
			this.RotateAttachTransform(anchor, direction, referenceRotation);
		}

		[Obsolete("TranslateAnchor has been renamed in version 3.0.0. Use TranslateAttachTransform instead.")]
		protected virtual void TranslateAnchor(Transform rayOrigin, Transform anchor, float directionAmount)
		{
			this.TranslateAttachTransform(rayOrigin, anchor, directionAmount);
		}

		[Obsolete("TryRead2DAxis has been deprecated in version 3.0.0.")]
		private static bool TryRead2DAxis(InputAction action, out Vector2 output)
		{
			if (action != null)
			{
				output = action.ReadValue<Vector2>();
				return true;
			}
			output = default(Vector2);
			return false;
		}

		[Obsolete("TryReadButton has been deprecated in version 3.0.0.")]
		private static bool TryReadButton(InputAction action)
		{
			return action != null && action.WasPerformedThisFrame();
		}

		private protected override void OnXRControllerChanged()
		{
			base.OnXRControllerChanged();
			XRBaseController xrController = base.xrController;
			this.m_ActionBasedController = (xrController as ActionBasedController);
			this.m_IsActionBasedController = (this.m_ActionBasedController != null);
			this.m_DeviceBasedController = (xrController as XRController);
			this.m_IsDeviceBasedController = (this.m_DeviceBasedController != null);
			this.m_ScreenSpaceController = (xrController as XRScreenSpaceController);
			this.m_IsScreenSpaceController = (this.m_ScreenSpaceController != null);
			if (base.forceDeprecatedInput && this.m_IsScreenSpaceController && this.m_ManipulateAttachTransform && this.m_RotateMode == XRRayInteractor.RotateMode.RotateOverTime)
			{
				Debug.LogWarning("Rotate Over Time is not a valid value for Rotation Mode when using XR Screen Space Controller. This XR Ray Interactor will not be able to rotate the anchor using screen touches.", this);
			}
		}

		private const int k_MaxRaycastHits = 10;

		private const int k_MaxSpherecastHits = 10;

		private const int k_MinSampleFrequency = 2;

		private const int k_MaxSampleFrequency = 100;

		private static readonly List<IXRInteractable> s_Results = new List<IXRInteractable>();

		private static readonly RaycastHit[] s_SpherecastScratch = new RaycastHit[10];

		private static readonly HashSet<Collider> s_OptimalHits = new HashSet<Collider>();

		private readonly List<Tuple<Vector3, float>> m_ConeCastDebugInfo = new List<Tuple<Vector3, float>>();

		[SerializeField]
		private XRRayInteractor.LineType m_LineType;

		[SerializeField]
		private bool m_BlendVisualLinePoints = true;

		[SerializeField]
		private float m_MaxRaycastDistance = 30f;

		[SerializeField]
		private Transform m_RayOriginTransform;

		[SerializeField]
		private Transform m_ReferenceFrame;

		[SerializeField]
		private float m_Velocity = 16f;

		[SerializeField]
		private float m_Acceleration = 9.8f;

		[SerializeField]
		private float m_AdditionalGroundHeight = 0.1f;

		[SerializeField]
		private float m_AdditionalFlightTime = 0.5f;

		[SerializeField]
		private float m_EndPointDistance = 30f;

		[SerializeField]
		private float m_EndPointHeight = -10f;

		[SerializeField]
		private float m_ControlPointDistance = 10f;

		[SerializeField]
		private float m_ControlPointHeight = 5f;

		[SerializeField]
		[Range(2f, 100f)]
		private int m_SampleFrequency = 20;

		[SerializeField]
		private XRRayInteractor.HitDetectionType m_HitDetectionType;

		[SerializeField]
		[Range(0.01f, 0.25f)]
		private float m_SphereCastRadius = 0.1f;

		[SerializeField]
		[Range(0f, 180f)]
		private float m_ConeCastAngle = 6f;

		private float m_CachedConeCastAngle;

		private float m_CachedConeCastRadius;

		[SerializeField]
		private bool m_LiveConeCastDebugVisuals;

		[SerializeField]
		private LayerMask m_RaycastMask = -1;

		[SerializeField]
		private QueryTriggerInteraction m_RaycastTriggerInteraction = QueryTriggerInteraction.Ignore;

		[SerializeField]
		private XRRayInteractor.QuerySnapVolumeInteraction m_RaycastSnapVolumeInteraction = XRRayInteractor.QuerySnapVolumeInteraction.Collide;

		[SerializeField]
		private bool m_HitClosestOnly;

		[SerializeField]
		private bool m_HoverToSelect;

		[SerializeField]
		private float m_HoverTimeToSelect = 0.5f;

		[SerializeField]
		private bool m_AutoDeselect;

		[SerializeField]
		private float m_TimeToAutoDeselect = 3f;

		[SerializeField]
		private bool m_EnableUIInteraction = true;

		[SerializeField]
		private bool m_BlockInteractionsWithScreenSpaceUI;

		[SerializeField]
		private bool m_BlockUIOnInteractableSelection = true;

		[FormerlySerializedAs("m_AllowAnchorControl")]
		[SerializeField]
		private bool m_ManipulateAttachTransform = true;

		[SerializeField]
		private bool m_UseForceGrab;

		[SerializeField]
		private float m_RotateSpeed = 180f;

		[SerializeField]
		private float m_TranslateSpeed = 1f;

		[FormerlySerializedAs("m_AnchorRotateReferenceFrame")]
		[SerializeField]
		private Transform m_RotateReferenceFrame;

		[FormerlySerializedAs("m_AnchorRotationMode")]
		[SerializeField]
		private XRRayInteractor.RotateMode m_RotateMode;

		[SerializeField]
		private UIHoverEnterEvent m_UIHoverEntered = new UIHoverEnterEvent();

		[SerializeField]
		private UIHoverExitEvent m_UIHoverExited = new UIHoverExitEvent();

		[SerializeField]
		private bool m_EnableARRaycasting;

		[SerializeField]
		private bool m_OccludeARHitsWith3DObjects;

		[SerializeField]
		private bool m_OccludeARHitsWith2DObjects;

		[SerializeField]
		private ScaleMode m_ScaleMode;

		[SerializeField]
		private XRInputButtonReader m_UIPressInput = new XRInputButtonReader("UI Press", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_UIScrollInput = new XRInputValueReader<Vector2>("UI Scroll", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_TranslateManipulationInput = new XRInputValueReader<Vector2>("Translate Manipulation", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_RotateManipulationInput = new XRInputValueReader<Vector2>("Rotate Manipulation", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_DirectionalManipulationInput = new XRInputValueReader<Vector2>("Directional Manipulation", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputButtonReader m_ScaleToggleInput = new XRInputButtonReader("Scale Toggle", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_ScaleOverTimeInput = new XRInputValueReader<Vector2>("Scale Over Time", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<float> m_ScaleDistanceDeltaInput = new XRInputValueReader<float>("Scale Distance Delta", XRInputValueReader.InputSourceMode.Unused);

		private bool m_HasRayOriginTransform;

		private bool m_HasReferenceFrame;

		private bool m_ScaleInputActive;

		private readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();

		private readonly Dictionary<IXRInteractable, RaycastHit> m_InteractableRaycastHits = new Dictionary<IXRInteractable, RaycastHit>();

		private float m_LastTimeHoveredObjectChanged;

		private bool m_PassedHoverTimeToSelect;

		private float m_LastTimeAutoSelected;

		private bool m_PassedTimeToAutoDeselect;

		private GameObject m_LastUIObject;

		private float m_LastTimeHoveredUIChanged;

		private bool m_HoverUISelectActive;

		private bool m_BlockUIAutoDeselect;

		private readonly RaycastHit[] m_RaycastHits = new RaycastHit[10];

		private int m_RaycastHitsCount;

		private readonly XRRayInteractor.RaycastHitComparer m_RaycastHitComparer = new XRRayInteractor.RaycastHitComparer();

		private List<XRRayInteractor.SamplePoint> m_SamplePoints;

		private int m_SamplePointsFrameUpdated = -1;

		private int m_RaycastHitEndpointIndex;

		private int m_UIRaycastHitEndpointIndex;

		private readonly float3[] m_ControlPoints = new float3[3];

		private readonly float3[] m_HitChordControlPoints = new float3[3];

		private static List<XRRayInteractor.SamplePoint> s_ScratchSamplePoints;

		private static readonly float3[] s_ScratchControlPoints = new float3[3];

		private PhysicsScene m_LocalPhysicsScene;

		private RegisteredUIInteractorCache m_RegisteredUIInteractorCache;

		private bool m_RaycastHitOccurred;

		private RaycastHit m_RaycastHit;

		private RaycastResult m_UIRaycastHit;

		private bool m_IsUIHitClosest;

		private IXRInteractable m_RaycastInteractable;

		[Obsolete("m_ActionBasedController has been deprecated in version 3.0.0.")]
		private ActionBasedController m_ActionBasedController;

		[Obsolete("m_DeviceBasedController has been deprecated in version 3.0.0.")]
		private XRController m_DeviceBasedController;

		[Obsolete("m_ScreenSpaceController has been deprecated in version 3.0.0.")]
		private XRScreenSpaceController m_ScreenSpaceController;

		[Obsolete("m_IsActionBasedController has been deprecated in version 3.0.0.")]
		private bool m_IsActionBasedController;

		[Obsolete("m_IsDeviceBasedController has been deprecated in version 3.0.0.")]
		private bool m_IsDeviceBasedController;

		[Obsolete("m_IsScreenSpaceController has been deprecated in version 3.0.0.")]
		private bool m_IsScreenSpaceController;

		protected sealed class RaycastHitComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit a, RaycastHit b)
			{
				float num = (a.collider != null) ? a.distance : float.MaxValue;
				float value = (b.collider != null) ? b.distance : float.MaxValue;
				return num.CompareTo(value);
			}
		}

		public enum LineType
		{
			StraightLine,
			ProjectileCurve,
			BezierCurve
		}

		public enum QuerySnapVolumeInteraction
		{
			Ignore,
			Collide
		}

		public enum HitDetectionType
		{
			Raycast,
			SphereCast,
			ConeCast
		}

		public enum RotateMode
		{
			RotateOverTime,
			MatchDirection
		}

		private struct SamplePoint
		{
			public float3 position { readonly get; set; }

			public float parameter { readonly get; set; }
		}

		[Obsolete("AnchorRotationMode has been deprecated in version 3.0.0. Use RotateMode instead.")]
		public enum AnchorRotationMode
		{
			RotateOverTime,
			MatchDirection
		}
	}
}
