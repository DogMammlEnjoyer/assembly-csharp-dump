using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Curves;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Visual/Curve Visual Controller", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.CurveVisualController.html")]
	[BurstCompile]
	public class CurveVisualController : MonoBehaviour
	{
		public LineRenderer lineRenderer
		{
			get
			{
				return this.m_LineRenderer;
			}
			set
			{
				this.m_LineRenderer = value;
				this.m_LineRenderer.useWorldSpace = false;
			}
		}

		public ICurveInteractionDataProvider curveInteractionDataProvider
		{
			get
			{
				return this.m_CurveDataProviderObjectRef.Get(this.m_CurveVisualObject);
			}
			set
			{
				this.m_CurveDataProviderObjectRef.Set(ref this.m_CurveVisualObject, value);
			}
		}

		public bool overrideLineOrigin
		{
			get
			{
				return this.m_OverrideLineOrigin;
			}
			set
			{
				this.m_OverrideLineOrigin = value;
			}
		}

		public Transform lineOriginTransform
		{
			get
			{
				return this.m_LineOriginTransform;
			}
			set
			{
				this.m_LineOriginTransform = value;
				this.m_UseCustomOrigin = (value != null);
			}
		}

		public int visualPointCount
		{
			get
			{
				return this.m_VisualPointCount;
			}
			set
			{
				this.m_VisualPointCount = value;
			}
		}

		public float maxVisualCurveDistance
		{
			get
			{
				return this.m_MaxVisualCurveDistance;
			}
			set
			{
				this.m_MaxVisualCurveDistance = value;
			}
		}

		public float restingVisualLineLength
		{
			get
			{
				return this.m_RestingVisualLineLength;
			}
			set
			{
				this.m_RestingVisualLineLength = value;
			}
		}

		public LineDynamicsMode lineDynamicsMode
		{
			get
			{
				return this.m_LineDynamicsMode;
			}
			set
			{
				this.m_LineDynamicsMode = value;
			}
		}

		public float retractDelay
		{
			get
			{
				return this.m_RetractDelay;
			}
			set
			{
				this.m_RetractDelay = value;
			}
		}

		public float retractDuration
		{
			get
			{
				return this.m_RetractDuration;
			}
			set
			{
				this.m_RetractDuration = value;
			}
		}

		public bool extendLineToEmptyHit
		{
			get
			{
				return this.m_ExtendLineToEmptyHit;
			}
			set
			{
				this.m_ExtendLineToEmptyHit = value;
			}
		}

		public float extensionRate
		{
			get
			{
				return this.m_ExtensionRate;
			}
			set
			{
				this.m_ExtensionRate = Mathf.Clamp(value, 0f, 30f);
			}
		}

		public float endPointExpansionRate
		{
			get
			{
				return this.m_EndPointExpansionRate;
			}
			set
			{
				this.m_EndPointExpansionRate = value;
			}
		}

		public bool computeMidPointWithComplexCurves
		{
			get
			{
				return this.m_ComputeMidPointWithComplexCurves;
			}
			set
			{
				this.m_ComputeMidPointWithComplexCurves = value;
			}
		}

		public bool snapToSelectedAttachIfAvailable
		{
			get
			{
				return this.m_SnapToSelectedAttachIfAvailable;
			}
			set
			{
				this.m_SnapToSelectedAttachIfAvailable = value;
			}
		}

		public bool snapToSnapVolumeIfAvailable
		{
			get
			{
				return this.m_SnapToSnapVolumeIfAvailable;
			}
			set
			{
				this.m_SnapToSnapVolumeIfAvailable = value;
			}
		}

		public float curveStartOffset
		{
			get
			{
				return this.m_CurveStartOffset;
			}
			set
			{
				this.m_CurveStartOffset = value;
			}
		}

		public float curveEndOffset
		{
			get
			{
				return this.m_CurveEndOffset;
			}
			set
			{
				this.m_CurveEndOffset = value;
			}
		}

		public bool customizeLinePropertiesForState
		{
			get
			{
				return this.m_CustomizeLinePropertiesForState;
			}
			set
			{
				this.m_CustomizeLinePropertiesForState = value;
			}
		}

		public float linePropertyAnimationSpeed
		{
			get
			{
				return this.m_LinePropertyAnimationSpeed;
			}
			set
			{
				this.m_LinePropertyAnimationSpeed = value;
			}
		}

		public LineProperties noValidHitProperties
		{
			get
			{
				return this.m_NoValidHitProperties;
			}
			set
			{
				this.m_NoValidHitProperties = value;
			}
		}

		public LineProperties uiHitProperties
		{
			get
			{
				return this.m_UIHitProperties;
			}
			set
			{
				this.m_UIHitProperties = value;
			}
		}

		public LineProperties uiPressHitProperties
		{
			get
			{
				return this.m_UIPressHitProperties;
			}
			set
			{
				this.m_UIPressHitProperties = value;
			}
		}

		public LineProperties selectHitProperties
		{
			get
			{
				return this.m_SelectHitProperties;
			}
			set
			{
				this.m_SelectHitProperties = value;
			}
		}

		public LineProperties hoverHitProperties
		{
			get
			{
				return this.m_HoverHitProperties;
			}
			set
			{
				this.m_HoverHitProperties = value;
			}
		}

		public bool renderLineInWorldSpace
		{
			get
			{
				return this.m_RenderLineInWorldSpace;
			}
			set
			{
				this.m_RenderLineInWorldSpace = value;
				if (this.m_LineRenderer != null)
				{
					this.m_LineRenderer.useWorldSpace = value;
				}
			}
		}

		public bool swapMaterials
		{
			get
			{
				return this.m_SwapMaterials;
			}
			set
			{
				this.m_SwapMaterials = value;
			}
		}

		public Material baseLineMaterial
		{
			get
			{
				return this.m_BaseLineMaterial;
			}
			set
			{
				this.m_BaseLineMaterial = value;
			}
		}

		public Material emptyHitMaterial
		{
			get
			{
				return this.m_EmptyHitMaterial;
			}
			set
			{
				this.m_EmptyHitMaterial = value;
			}
		}

		protected void Awake()
		{
			if (this.m_LineRenderer == null)
			{
				this.m_LineRenderer = base.GetComponentInChildren<LineRenderer>();
				if (this.m_LineRenderer == null)
				{
					Debug.LogError(string.Format("Missing Line Renderer component on Curve Caster Visual Controller {0}.", this), this);
					base.enabled = false;
					return;
				}
			}
			if (this.curveInteractionDataProvider == null)
			{
				Debug.LogError(string.Format("Missing {0} Disabling {1}.", typeof(ICurveInteractionDataProvider), this), this);
				base.enabled = false;
				return;
			}
			this.m_LineRenderer.useWorldSpace = this.m_RenderLineInWorldSpace;
			this.m_ParentTransform = base.transform.parent;
			this.m_FallBackSamplePoints = new NativeArray<Vector3>(3, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			if (this.m_OverrideLineOrigin && this.m_LineOriginTransform == null)
			{
				this.m_LineOriginTransform = base.transform;
			}
			this.m_UseCustomOrigin = (this.m_LineOriginTransform != null);
			this.m_CanSwapMaterials = (this.m_SwapMaterials && this.m_EmptyHitMaterial != null && this.m_BaseLineMaterial != null);
			this.m_LastLineStartWidth = this.m_LineRenderer.startWidth;
			this.m_LastLineEndWidth = this.m_LineRenderer.endWidth;
			this.m_LerpGradient = this.m_LineRenderer.colorGradient;
		}

		protected void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRenderLineVisual;
		}

		protected void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRenderLineVisual;
		}

		protected void OnDestroy()
		{
			if (this.m_FallBackSamplePoints.IsCreated)
			{
				this.m_FallBackSamplePoints.Dispose();
			}
			if (this.m_InternalSamplePoints.IsCreated)
			{
				this.m_InternalSamplePoints.Dispose();
			}
		}

		protected void LateUpdate()
		{
		}

		[BeforeRenderOrder(101)]
		private void OnBeforeRenderLineVisual()
		{
			this.UpdateLineVisual();
		}

		private void UpdateLineVisual()
		{
			ICurveInteractionDataProvider curveInteractionDataProvider = this.curveInteractionDataProvider;
			if (!curveInteractionDataProvider.isActive)
			{
				this.m_LineRenderer.enabled = false;
				return;
			}
			this.m_LineRenderer.enabled = true;
			this.ValidatePointCount();
			Vector3 vector;
			Vector3 worldDirection;
			this.GetLineOriginAndDirection(out vector, out worldDirection);
			float maxVisualCurveDistance = this.m_MaxVisualCurveDistance;
			Vector3 vector2;
			EndPointType endpointInformation = this.GetEndpointInformation(vector, worldDirection, ref maxVisualCurveDistance, out vector2);
			float num = this.UpdateTargetDistance(endpointInformation, maxVisualCurveDistance, this.m_RestingVisualLineLength, this.m_MaxVisualCurveDistance, this.m_LineDynamicsMode == LineDynamicsMode.RetractOnHitLoss, this.m_RetractDelay, this.m_RetractDuration, this.m_ExtensionRate);
			if (num < maxVisualCurveDistance)
			{
				float3 @float = vector;
				float3 float2 = vector2;
				float3 v;
				CurveVisualController.GetAdjustedEndPointForMaxDistance(@float, float2, num, out v);
				vector2 = v;
			}
			if (this.CheckIfVisualStateChanged(endpointInformation, curveInteractionDataProvider.hasValidSelect))
			{
				this.SwapMaterials(endpointInformation);
			}
			float startOffset;
			float endOffset;
			this.DetermineOffsets(endpointInformation, num, out startOffset, out endOffset);
			this.UpdateLineWidth(endpointInformation, num);
			this.UpdateGradient(endpointInformation);
			this.UpdateLinePoints(endpointInformation, vector, vector2, worldDirection, startOffset, endOffset, false);
		}

		private bool CheckIfVisualStateChanged(EndPointType newPointType, bool hasValidSelect)
		{
			if (newPointType == this.m_LastEndPointType && this.m_LastValidSelectState == hasValidSelect)
			{
				return false;
			}
			this.m_EndPointTypeChangeTime = Time.unscaledTime;
			this.m_LastEndPointType = newPointType;
			this.m_LastValidSelectState = hasValidSelect;
			return true;
		}

		private void GetLineOriginAndDirection(out Vector3 worldOrigin, out Vector3 worldDirection)
		{
			if (this.m_UseCustomOrigin)
			{
				worldOrigin = this.m_LineOriginTransform.position;
				worldDirection = this.m_LineOriginTransform.forward;
				return;
			}
			worldOrigin = this.curveInteractionDataProvider.curveOrigin.position;
			worldDirection = this.curveInteractionDataProvider.curveOrigin.forward;
		}

		private EndPointType GetEndpointInformation(Vector3 worldOrigin, Vector3 worldDirection, ref float validHitDistance, out Vector3 endPoint)
		{
			Vector3 vector;
			EndPointType endPointType = this.curveInteractionDataProvider.TryGetCurveEndPoint(out vector, this.m_SnapToSelectedAttachIfAvailable, this.m_SnapToSnapVolumeIfAvailable);
			if (endPointType == EndPointType.AttachPoint || endPointType == EndPointType.UI)
			{
				validHitDistance = math.distance(worldOrigin, vector);
				endPoint = vector;
			}
			else if (endPointType == EndPointType.EmptyCastHit || endPointType == EndPointType.ValidCastHit)
			{
				float3 @float = worldOrigin;
				float3 float2 = worldDirection;
				float3 float3 = vector;
				float3 float4 = this.curveInteractionDataProvider.lastSamplePoint;
				float3 v;
				CurveVisualController.AdjustCastHitEndPoint(@float, float2, float3, float4, out validHitDistance, out v);
				endPoint = v;
			}
			else
			{
				endPoint = this.curveInteractionDataProvider.lastSamplePoint;
			}
			return endPointType;
		}

		private void UpdateLinePoints(EndPointType endPointType, Vector3 worldOrigin, Vector3 worldEndPoint, Vector3 worldDirection, float startOffset = 0f, float endOffset = 0f, bool forceStraightLineFallback = false)
		{
			NativeArray<float3> nativeArray = this.m_InternalSamplePoints.Reinterpret<float3>();
			float lineBendRatio = this.GetLineBendRatio(endPointType);
			bool flag = forceStraightLineFallback || lineBendRatio < 1f;
			bool flag2 = false;
			Vector3 v = this.m_RenderLineInWorldSpace ? worldOrigin : this.m_ParentTransform.InverseTransformPoint(worldOrigin);
			Vector3 v2 = this.m_RenderLineInWorldSpace ? worldEndPoint : this.m_ParentTransform.InverseTransformPoint(worldEndPoint);
			float3 @float;
			float3 float2;
			if (flag)
			{
				float3 float3;
				if (this.m_ComputeMidPointWithComplexCurves)
				{
					ICurveInteractionDataProvider curveInteractionDataProvider = this.curveInteractionDataProvider;
					Vector3 vector;
					if (CurveVisualController.TryGetMidPointFromCurveSamples(curveInteractionDataProvider, out vector))
					{
						Vector3 v3 = this.m_RenderLineInWorldSpace ? vector : this.m_ParentTransform.InverseTransformPoint(vector);
						int visualPointCount = this.m_VisualPointCount;
						@float = v;
						float2 = v3;
						float3 = v2;
						flag2 = CurveUtility.TryGenerateCubicBezierCurve(visualPointCount, @float, float2, float3, ref nativeArray, 0.06f, startOffset, endOffset);
						goto IL_11D;
					}
				}
				Vector3 v4 = this.m_RenderLineInWorldSpace ? worldDirection : this.m_ParentTransform.InverseTransformDirection(worldDirection);
				int visualPointCount2 = this.m_VisualPointCount;
				float curveRatio = lineBendRatio;
				@float = v;
				float2 = v4;
				float3 = v2;
				flag2 = CurveUtility.TryGenerateCubicBezierCurve(visualPointCount2, curveRatio, @float, float2, float3, ref nativeArray, 0.06f, startOffset, endOffset);
			}
			IL_11D:
			if (flag2)
			{
				this.SetLinePositions(this.m_InternalSamplePoints, this.m_VisualPointCount);
				return;
			}
			NativeArray<float3> nativeArray2 = this.m_FallBackSamplePoints.Reinterpret<float3>();
			@float = v;
			float2 = v2;
			if (CurveVisualController.ComputeFallBackLine(@float, float2, startOffset, endOffset, ref nativeArray2))
			{
				this.SetLinePositions(this.m_FallBackSamplePoints, 3);
				return;
			}
			this.m_LineRenderer.enabled = false;
		}

		private static bool TryGetMidPointFromCurveSamples(in ICurveInteractionDataProvider curveInteractionDataProvider, out Vector3 midPoint)
		{
			int length = curveInteractionDataProvider.samplePoints.Length;
			if (length > 2)
			{
				int index = length / 2;
				midPoint = curveInteractionDataProvider.samplePoints[index];
				return true;
			}
			if (length == 2)
			{
				midPoint = (curveInteractionDataProvider.samplePoints[0] + curveInteractionDataProvider.samplePoints[1]) / 2f;
				return true;
			}
			midPoint = default(Vector3);
			return false;
		}

		private bool TryGetLineProperties(EndPointType endPointType, out LineProperties properties)
		{
			if (!this.m_CustomizeLinePropertiesForState)
			{
				properties = null;
				return false;
			}
			LineProperties lineProperties;
			switch (endPointType)
			{
			case EndPointType.None:
				lineProperties = this.m_NoValidHitProperties;
				break;
			case EndPointType.EmptyCastHit:
				lineProperties = this.m_NoValidHitProperties;
				break;
			case EndPointType.ValidCastHit:
				lineProperties = (this.curveInteractionDataProvider.hasValidSelect ? this.m_SelectHitProperties : this.m_HoverHitProperties);
				break;
			case EndPointType.AttachPoint:
				lineProperties = this.m_SelectHitProperties;
				break;
			case EndPointType.UI:
				lineProperties = (this.curveInteractionDataProvider.hasValidSelect ? this.m_UIPressHitProperties : this.m_UIHitProperties);
				break;
			default:
				lineProperties = this.m_NoValidHitProperties;
				break;
			}
			properties = lineProperties;
			return true;
		}

		private float GetLineBendRatio(EndPointType endPointType)
		{
			LineProperties lineProperties;
			if (!this.TryGetLineProperties(endPointType, out lineProperties))
			{
				return 0.5f;
			}
			if (!lineProperties.smoothlyCurveLine)
			{
				return 1f;
			}
			if (this.m_LinePropertyAnimationSpeed > 0f)
			{
				this.m_LastBendRatio = Mathf.Lerp(this.m_LastBendRatio, lineProperties.lineBendRatio, Time.unscaledDeltaTime * this.m_LinePropertyAnimationSpeed);
				return this.m_LastBendRatio;
			}
			return lineProperties.lineBendRatio;
		}

		private void DetermineOffsets(EndPointType endPointType, float lineDistance, out float startOffset, out float endOffset)
		{
			startOffset = this.m_CurveStartOffset;
			endOffset = this.m_CurveEndOffset;
			if (this.m_LineDynamicsMode != LineDynamicsMode.ExpandFromHitPoint)
			{
				return;
			}
			float curveEndOffset = this.m_CurveEndOffset;
			float t = Time.unscaledDeltaTime * this.m_EndPointExpansionRate;
			float end = this.m_RenderLengthMultiplier;
			LineProperties lineProperties;
			if (this.TryGetLineProperties(endPointType, out lineProperties))
			{
				if (lineProperties.customizeExpandLineDrawPercent)
				{
					end = Mathf.Clamp01(1f - lineProperties.expandModeLineDrawPercent);
				}
			}
			else if (endPointType == EndPointType.AttachPoint)
			{
				end = 0.25f;
			}
			else if (endPointType == EndPointType.ValidCastHit)
			{
				end = 0.75f;
			}
			else
			{
				end = 1f;
			}
			this.m_RenderLengthMultiplier = BurstLerpUtility.BezierLerp(this.m_RenderLengthMultiplier, end, t, 0.5f);
			float num = lineDistance * this.m_RenderLengthMultiplier;
			startOffset = Mathf.Max(num - (curveEndOffset + 0.001f), startOffset);
			endOffset = curveEndOffset;
		}

		private void UpdateLineWidth(EndPointType endPointType, float targetDistance)
		{
			LineProperties lineProperties;
			if (!this.TryGetLineProperties(endPointType, out lineProperties) || !lineProperties.adjustWidth)
			{
				return;
			}
			if (Mathf.Approximately(this.m_LastLineStartWidth, lineProperties.starWidth) && Mathf.Approximately(this.m_LastLineEndWidth, lineProperties.endWidth))
			{
				return;
			}
			float starWidth = lineProperties.starWidth;
			float num = (lineProperties.endWidthScaleDistanceFactor > 0f) ? (1f + lineProperties.endWidthScaleDistanceFactor * targetDistance / this.maxVisualCurveDistance) : 1f;
			float num2 = lineProperties.endWidth * num;
			if (this.m_LinePropertyAnimationSpeed > 0f)
			{
				if (Mathf.Abs(this.m_LastLineStartWidth - starWidth) < 0.0001f && Mathf.Abs(this.m_LastLineEndWidth - num2) < 0.0001f)
				{
					this.m_LastLineStartWidth = starWidth;
					this.m_LastLineEndWidth = num2;
				}
				else
				{
					float t = Time.unscaledDeltaTime * this.m_LinePropertyAnimationSpeed;
					this.m_LastLineStartWidth = Mathf.Lerp(this.m_LastLineStartWidth, starWidth, t);
					this.m_LastLineEndWidth = Mathf.Lerp(this.m_LastLineEndWidth, num2, t);
				}
			}
			else
			{
				this.m_LastLineStartWidth = starWidth;
				this.m_LastLineEndWidth = num2;
			}
			this.m_LineRenderer.startWidth = this.m_LastLineStartWidth;
			this.m_LineRenderer.endWidth = this.m_LastLineEndWidth;
		}

		private void UpdateGradient(EndPointType endPointType)
		{
			LineProperties lineProperties;
			if (!this.TryGetLineProperties(endPointType, out lineProperties) || !lineProperties.adjustGradient)
			{
				return;
			}
			float num = Time.unscaledTime - this.m_EndPointTypeChangeTime;
			if (this.m_LinePropertyAnimationSpeed > 0f && num < 1f)
			{
				GradientUtility.Lerp(this.m_LerpGradient, lineProperties.gradient, this.m_LerpGradient, Time.unscaledDeltaTime * this.m_LinePropertyAnimationSpeed, true, true);
			}
			else
			{
				GradientUtility.CopyGradient(lineProperties.gradient, this.m_LerpGradient);
			}
			this.m_LineRenderer.colorGradient = this.m_LerpGradient;
		}

		private void SetLinePositions(NativeArray<Vector3> targetPoints, int numPoints)
		{
			if (numPoints != this.m_LastPosCount)
			{
				this.m_LineRenderer.positionCount = numPoints;
				this.m_LastPosCount = numPoints;
			}
			this.m_LineRenderer.SetPositions(targetPoints);
		}

		private float UpdateTargetDistance(EndPointType endPointType, float validHitDistance, float minLength, float maxLength, bool retractOnHitLoss, float retractionDelay, float retractionDuration, float curveExtensionRate)
		{
			float unscaledTime = Time.unscaledTime;
			if (endPointType != EndPointType.None)
			{
				if (endPointType != EndPointType.EmptyCastHit)
				{
					this.m_LastHitTime = unscaledTime;
				}
				if (!this.m_ExtendLineToEmptyHit && endPointType == EndPointType.EmptyCastHit)
				{
					this.m_LengthToLastHit = Mathf.Min(validHitDistance, this.m_LengthToLastHit);
				}
				else
				{
					this.m_LengthToLastHit = Mathf.Min(validHitDistance, maxLength);
				}
				this.m_LengthToLastHit = Mathf.Max(this.m_LengthToLastHit, minLength);
				if (this.m_LineLength > this.m_LengthToLastHit)
				{
					this.m_LineLength = this.m_LengthToLastHit;
					return this.m_LineLength;
				}
			}
			float num = unscaledTime - this.m_LastHitTime;
			if (retractOnHitLoss && num > retractionDelay)
			{
				float num2 = num - retractionDelay;
				if (num2 > retractionDuration)
				{
					this.m_LineLength = minLength;
					return this.m_LineLength;
				}
				this.m_LineLength = BurstLerpUtility.BezierLerp(this.m_LengthToLastHit, minLength, Mathf.Clamp01(num2 / retractionDuration), 0.5f);
			}
			else
			{
				float num3 = Mathf.Max(this.m_LengthToLastHit, minLength);
				this.m_LineLength = ((curveExtensionRate > 0f) ? BurstLerpUtility.BezierLerp(this.m_LineLength, num3, Time.unscaledDeltaTime * curveExtensionRate, 0.5f) : num3);
			}
			return this.m_LineLength;
		}

		private void SwapMaterials(EndPointType endPointType)
		{
			if (!this.m_CanSwapMaterials || !this.m_SwapMaterials)
			{
				return;
			}
			this.m_LineRenderer.sharedMaterial = ((endPointType == EndPointType.EmptyCastHit) ? this.m_EmptyHitMaterial : this.m_BaseLineMaterial);
		}

		private void ValidatePointCount()
		{
			bool isCreated = this.m_InternalSamplePoints.IsCreated;
			if (isCreated && this.m_InternalSamplePoints.Length == this.m_VisualPointCount)
			{
				return;
			}
			if (isCreated)
			{
				this.m_InternalSamplePoints.Dispose();
			}
			this.m_InternalSamplePoints = new NativeArray<Vector3>(this.m_VisualPointCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_LineRenderer.positionCount = this.m_VisualPointCount;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$PostfixBurstDelegate))]
		private static void GetAdjustedEndPointForMaxDistance(in float3 origin, in float3 endPoint, float maxDistance, out float3 newEndPoint)
		{
			CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall.Invoke(origin, endPoint, maxDistance, out newEndPoint);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveVisualController.GetClosestPointOnLine_00000D25$PostfixBurstDelegate))]
		private static void GetClosestPointOnLine(in float3 origin, in float3 direction, in float3 point, out float3 newPoint)
		{
			CurveVisualController.GetClosestPointOnLine_00000D25$BurstDirectCall.Invoke(origin, direction, point, out newPoint);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveVisualController.AdjustCastHitEndPoint_00000D26$PostfixBurstDelegate))]
		private static void AdjustCastHitEndPoint(in float3 worldOrigin, in float3 worldDirection, in float3 hitEndPoint, in float3 sampleEndPoint, out float validHitDistance, out float3 endPoint)
		{
			CurveVisualController.AdjustCastHitEndPoint_00000D26$BurstDirectCall.Invoke(worldOrigin, worldDirection, hitEndPoint, sampleEndPoint, out validHitDistance, out endPoint);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveVisualController.ComputeFallBackLine_00000D27$PostfixBurstDelegate))]
		private static bool ComputeFallBackLine(in float3 curveOrigin, in float3 endPoint, float startOffset, float endOffset, ref NativeArray<float3> fallBackTargetPoints)
		{
			return CurveVisualController.ComputeFallBackLine_00000D27$BurstDirectCall.Invoke(curveOrigin, endPoint, startOffset, endOffset, ref fallBackTargetPoints);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetAdjustedEndPointForMaxDistance$BurstManaged(in float3 origin, in float3 endPoint, float maxDistance, out float3 newEndPoint)
		{
			float3 lhs = math.normalize(endPoint - origin);
			newEndPoint = origin + lhs * maxDistance;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetClosestPointOnLine$BurstManaged(in float3 origin, in float3 direction, in float3 point, out float3 newPoint)
		{
			float3 rhs = math.dot(point - origin, direction) * direction;
			newPoint = origin + rhs;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void AdjustCastHitEndPoint$BurstManaged(in float3 worldOrigin, in float3 worldDirection, in float3 hitEndPoint, in float3 sampleEndPoint, out float validHitDistance, out float3 endPoint)
		{
			float3 lhs;
			CurveVisualController.GetClosestPointOnLine(worldOrigin, worldDirection, hitEndPoint, out lhs);
			validHitDistance = math.length(lhs - worldOrigin);
			float3 lhs2 = math.normalize(sampleEndPoint - worldOrigin);
			endPoint = worldOrigin + lhs2 * validHitDistance;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool ComputeFallBackLine$BurstManaged(in float3 curveOrigin, in float3 endPoint, float startOffset, float endOffset, ref NativeArray<float3> fallBackTargetPoints)
		{
			float3 @float = endPoint - curveOrigin;
			float num = math.lengthsq(@float);
			if (num < 0.0001f)
			{
				return false;
			}
			float3 lhs = math.rsqrt(num) * @float;
			float3 float2 = curveOrigin + lhs * startOffset;
			float3 float3 = endPoint - lhs * endOffset;
			float3 x = math.normalize(endPoint - float2);
			float3 y = math.normalize(float3 - float2);
			if (math.dot(x, y) < 0f)
			{
				return false;
			}
			fallBackTargetPoints[0] = float2;
			fallBackTargetPoints[1] = math.lerp(float2, float3, 0.5f);
			fallBackTargetPoints[2] = float3;
			return true;
		}

		[SerializeField]
		private LineRenderer m_LineRenderer;

		[SerializeField]
		[RequireInterface(typeof(ICurveInteractionDataProvider))]
		private Object m_CurveVisualObject;

		private readonly UnityObjectReferenceCache<ICurveInteractionDataProvider, Object> m_CurveDataProviderObjectRef = new UnityObjectReferenceCache<ICurveInteractionDataProvider, Object>();

		[SerializeField]
		private bool m_OverrideLineOrigin = true;

		[SerializeField]
		private Transform m_LineOriginTransform;

		[SerializeField]
		private int m_VisualPointCount = 20;

		[SerializeField]
		private float m_MaxVisualCurveDistance = 10f;

		[SerializeField]
		private float m_RestingVisualLineLength = 0.15f;

		[SerializeField]
		private LineDynamicsMode m_LineDynamicsMode;

		[SerializeField]
		private float m_RetractDelay = 1f;

		[SerializeField]
		private float m_RetractDuration = 0.5f;

		[SerializeField]
		private bool m_ExtendLineToEmptyHit;

		[SerializeField]
		[Range(0f, 30f)]
		private float m_ExtensionRate = 10f;

		[SerializeField]
		private float m_EndPointExpansionRate = 10f;

		[SerializeField]
		private bool m_ComputeMidPointWithComplexCurves;

		[SerializeField]
		private bool m_SnapToSelectedAttachIfAvailable = true;

		[SerializeField]
		private bool m_SnapToSnapVolumeIfAvailable = true;

		[SerializeField]
		private float m_CurveStartOffset;

		[SerializeField]
		private float m_CurveEndOffset = 0.005f;

		[SerializeField]
		private bool m_CustomizeLinePropertiesForState;

		[SerializeField]
		private float m_LinePropertyAnimationSpeed = 8f;

		[SerializeField]
		private LineProperties m_NoValidHitProperties;

		[SerializeField]
		private LineProperties m_UIHitProperties;

		[SerializeField]
		private LineProperties m_UIPressHitProperties;

		[SerializeField]
		private LineProperties m_SelectHitProperties;

		[SerializeField]
		private LineProperties m_HoverHitProperties;

		[SerializeField]
		private bool m_RenderLineInWorldSpace = true;

		[SerializeField]
		private bool m_SwapMaterials;

		[SerializeField]
		private Material m_BaseLineMaterial;

		[SerializeField]
		private Material m_EmptyHitMaterial;

		private const float k_CurveFallbackLength = 0.06f;

		private const float k_DisableSquaredLength = 0.0001f;

		private const int k_FallBackLinePointCount = 3;

		private NativeArray<Vector3> m_InternalSamplePoints;

		private NativeArray<Vector3> m_FallBackSamplePoints;

		private Transform m_ParentTransform;

		private float m_LastHitTime;

		private float m_LengthToLastHit;

		private float m_LineLength;

		private int m_LastPosCount;

		private float m_RenderLengthMultiplier;

		private bool m_CanSwapMaterials;

		private float m_LastLineStartWidth;

		private float m_LastLineEndWidth;

		private float m_EndPointTypeChangeTime;

		private float m_LastBendRatio = 0.5f;

		private bool m_UseCustomOrigin;

		private EndPointType m_LastEndPointType;

		private bool m_LastValidSelectState;

		private Gradient m_LerpGradient;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetAdjustedEndPointForMaxDistance_00000D24$PostfixBurstDelegate(in float3 origin, in float3 endPoint, float maxDistance, out float3 newEndPoint);

		internal static class GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall.Pointer == 0)
				{
					CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$PostfixBurstDelegate>(new CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$PostfixBurstDelegate(CurveVisualController.GetAdjustedEndPointForMaxDistance)).Value;
				}
				A_0 = CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 origin, in float3 endPoint, float maxDistance, out float3 newEndPoint)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveVisualController.GetAdjustedEndPointForMaxDistance_00000D24$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&), ref origin, ref endPoint, maxDistance, ref newEndPoint, functionPointer);
						return;
					}
				}
				CurveVisualController.GetAdjustedEndPointForMaxDistance$BurstManaged(origin, endPoint, maxDistance, out newEndPoint);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetClosestPointOnLine_00000D25$PostfixBurstDelegate(in float3 origin, in float3 direction, in float3 point, out float3 newPoint);

		internal static class GetClosestPointOnLine_00000D25$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveVisualController.GetClosestPointOnLine_00000D25$BurstDirectCall.Pointer == 0)
				{
					CurveVisualController.GetClosestPointOnLine_00000D25$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveVisualController.GetClosestPointOnLine_00000D25$PostfixBurstDelegate>(new CurveVisualController.GetClosestPointOnLine_00000D25$PostfixBurstDelegate(CurveVisualController.GetClosestPointOnLine)).Value;
				}
				A_0 = CurveVisualController.GetClosestPointOnLine_00000D25$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveVisualController.GetClosestPointOnLine_00000D25$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 origin, in float3 direction, in float3 point, out float3 newPoint)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveVisualController.GetClosestPointOnLine_00000D25$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref origin, ref direction, ref point, ref newPoint, functionPointer);
						return;
					}
				}
				CurveVisualController.GetClosestPointOnLine$BurstManaged(origin, direction, point, out newPoint);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void AdjustCastHitEndPoint_00000D26$PostfixBurstDelegate(in float3 worldOrigin, in float3 worldDirection, in float3 hitEndPoint, in float3 sampleEndPoint, out float validHitDistance, out float3 endPoint);

		internal static class AdjustCastHitEndPoint_00000D26$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveVisualController.AdjustCastHitEndPoint_00000D26$BurstDirectCall.Pointer == 0)
				{
					CurveVisualController.AdjustCastHitEndPoint_00000D26$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveVisualController.AdjustCastHitEndPoint_00000D26$PostfixBurstDelegate>(new CurveVisualController.AdjustCastHitEndPoint_00000D26$PostfixBurstDelegate(CurveVisualController.AdjustCastHitEndPoint)).Value;
				}
				A_0 = CurveVisualController.AdjustCastHitEndPoint_00000D26$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveVisualController.AdjustCastHitEndPoint_00000D26$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 worldOrigin, in float3 worldDirection, in float3 hitEndPoint, in float3 sampleEndPoint, out float validHitDistance, out float3 endPoint)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveVisualController.AdjustCastHitEndPoint_00000D26$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single&,Unity.Mathematics.float3&), ref worldOrigin, ref worldDirection, ref hitEndPoint, ref sampleEndPoint, ref validHitDistance, ref endPoint, functionPointer);
						return;
					}
				}
				CurveVisualController.AdjustCastHitEndPoint$BurstManaged(worldOrigin, worldDirection, hitEndPoint, sampleEndPoint, out validHitDistance, out endPoint);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool ComputeFallBackLine_00000D27$PostfixBurstDelegate(in float3 curveOrigin, in float3 endPoint, float startOffset, float endOffset, ref NativeArray<float3> fallBackTargetPoints);

		internal static class ComputeFallBackLine_00000D27$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveVisualController.ComputeFallBackLine_00000D27$BurstDirectCall.Pointer == 0)
				{
					CurveVisualController.ComputeFallBackLine_00000D27$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveVisualController.ComputeFallBackLine_00000D27$PostfixBurstDelegate>(new CurveVisualController.ComputeFallBackLine_00000D27$PostfixBurstDelegate(CurveVisualController.ComputeFallBackLine)).Value;
				}
				A_0 = CurveVisualController.ComputeFallBackLine_00000D27$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveVisualController.ComputeFallBackLine_00000D27$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(in float3 curveOrigin, in float3 endPoint, float startOffset, float endOffset, ref NativeArray<float3> fallBackTargetPoints)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveVisualController.ComputeFallBackLine_00000D27$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,System.Single,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&), ref curveOrigin, ref endPoint, startOffset, endOffset, ref fallBackTargetPoints, functionPointer);
					}
				}
				return CurveVisualController.ComputeFallBackLine$BurstManaged(curveOrigin, endPoint, startOffset, endOffset, ref fallBackTargetPoints);
			}

			private static IntPtr Pointer;
		}
	}
}
