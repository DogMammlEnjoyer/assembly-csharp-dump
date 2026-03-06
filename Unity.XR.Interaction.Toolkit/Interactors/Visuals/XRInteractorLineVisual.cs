using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Curves;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[AddComponentMenu("XR/Visual/XR Interactor Line Visual", 11)]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LineRenderer))]
	[DefaultExecutionOrder(100)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual.html")]
	[BurstCompile]
	public class XRInteractorLineVisual : MonoBehaviour, IXRCustomReticleProvider
	{
		public float lineWidth
		{
			get
			{
				return this.m_LineWidth;
			}
			set
			{
				this.m_LineWidth = value;
				this.m_PerformSetup = true;
				this.m_UserScaleVar.BroadcastValue();
			}
		}

		public bool overrideInteractorLineLength
		{
			get
			{
				return this.m_OverrideInteractorLineLength;
			}
			set
			{
				this.m_OverrideInteractorLineLength = value;
			}
		}

		public float lineLength
		{
			get
			{
				return this.m_LineLength;
			}
			set
			{
				this.m_LineLength = value;
			}
		}

		public bool autoAdjustLineLength
		{
			get
			{
				return this.m_AutoAdjustLineLength;
			}
			set
			{
				this.m_AutoAdjustLineLength = value;
			}
		}

		public float minLineLength
		{
			get
			{
				return this.m_MinLineLength;
			}
			set
			{
				this.m_MinLineLength = value;
			}
		}

		public bool useDistanceToHitAsMaxLineLength
		{
			get
			{
				return this.m_UseDistanceToHitAsMaxLineLength;
			}
			set
			{
				this.m_UseDistanceToHitAsMaxLineLength = value;
			}
		}

		public float lineRetractionDelay
		{
			get
			{
				return this.m_LineRetractionDelay;
			}
			set
			{
				this.m_LineRetractionDelay = value;
			}
		}

		public float lineLengthChangeSpeed
		{
			get
			{
				return this.m_LineLengthChangeSpeed;
			}
			set
			{
				this.m_LineLengthChangeSpeed = value;
			}
		}

		public AnimationCurve widthCurve
		{
			get
			{
				return this.m_WidthCurve;
			}
			set
			{
				this.m_WidthCurve = value;
				this.m_PerformSetup = true;
			}
		}

		public bool setLineColorGradient
		{
			get
			{
				return this.m_SetLineColorGradient;
			}
			set
			{
				this.m_SetLineColorGradient = value;
			}
		}

		public Gradient validColorGradient
		{
			get
			{
				return this.m_ValidColorGradient;
			}
			set
			{
				this.m_ValidColorGradient = value;
			}
		}

		public Gradient invalidColorGradient
		{
			get
			{
				return this.m_InvalidColorGradient;
			}
			set
			{
				this.m_InvalidColorGradient = value;
			}
		}

		public Gradient blockedColorGradient
		{
			get
			{
				return this.m_BlockedColorGradient;
			}
			set
			{
				this.m_BlockedColorGradient = value;
			}
		}

		public bool treatSelectionAsValidState
		{
			get
			{
				return this.m_TreatSelectionAsValidState;
			}
			set
			{
				this.m_TreatSelectionAsValidState = value;
			}
		}

		public bool smoothMovement
		{
			get
			{
				return this.m_SmoothMovement;
			}
			set
			{
				this.m_SmoothMovement = value;
			}
		}

		public float followTightness
		{
			get
			{
				return this.m_FollowTightness;
			}
			set
			{
				this.m_FollowTightness = value;
			}
		}

		public float snapThresholdDistance
		{
			get
			{
				return this.m_SnapThresholdDistance;
			}
			set
			{
				this.m_SnapThresholdDistance = value;
				this.m_SquareSnapThresholdDistance = this.m_SnapThresholdDistance * this.m_SnapThresholdDistance;
			}
		}

		public GameObject reticle
		{
			get
			{
				return this.m_Reticle;
			}
			set
			{
				this.m_Reticle = value;
				if (Application.isPlaying)
				{
					this.SetupReticle();
				}
			}
		}

		public GameObject blockedReticle
		{
			get
			{
				return this.m_BlockedReticle;
			}
			set
			{
				this.m_BlockedReticle = value;
				if (Application.isPlaying)
				{
					this.SetupBlockedReticle();
				}
			}
		}

		public bool stopLineAtFirstRaycastHit
		{
			get
			{
				return this.m_StopLineAtFirstRaycastHit;
			}
			set
			{
				this.m_StopLineAtFirstRaycastHit = value;
			}
		}

		public bool stopLineAtSelection
		{
			get
			{
				return this.m_StopLineAtSelection;
			}
			set
			{
				this.m_StopLineAtSelection = value;
			}
		}

		public bool snapEndpointIfAvailable
		{
			get
			{
				return this.m_SnapEndpointIfAvailable;
			}
			set
			{
				this.m_SnapEndpointIfAvailable = value;
			}
		}

		public float lineBendRatio
		{
			get
			{
				return this.m_LineBendRatio;
			}
			set
			{
				this.m_LineBendRatio = Mathf.Clamp(value, 0.01f, 1f);
			}
		}

		public InteractionLayerMask bendingEnabledInteractionLayers
		{
			get
			{
				return this.m_BendingEnabledInteractionLayers;
			}
			set
			{
				this.m_BendingEnabledInteractionLayers = value;
			}
		}

		public bool overrideInteractorLineOrigin
		{
			get
			{
				return this.m_OverrideInteractorLineOrigin;
			}
			set
			{
				this.m_OverrideInteractorLineOrigin = value;
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
			}
		}

		public float lineOriginOffset
		{
			get
			{
				return this.m_LineOriginOffset;
			}
			set
			{
				this.m_LineOriginOffset = value;
			}
		}

		protected void Reset()
		{
		}

		protected void OnValidate()
		{
			if (Application.isPlaying)
			{
				this.UpdateSettings();
			}
		}

		protected void Awake()
		{
			this.m_LineRenderable = base.GetComponent<ILineRenderable>();
			this.m_AdvancedLineRenderable = (this.m_LineRenderable as IAdvancedLineRenderable);
			this.m_HasAdvancedLineRenderable = (this.m_AdvancedLineRenderable != null);
			if (this.m_LineRenderable != null)
			{
				XRBaseInteractor xrbaseInteractor = this.m_LineRenderable as XRBaseInteractor;
				if (xrbaseInteractor != null)
				{
					this.m_LineRenderableAsBaseInteractor = xrbaseInteractor;
					this.m_HasBaseInteractor = true;
				}
				IXRSelectInteractor ixrselectInteractor = this.m_LineRenderable as IXRSelectInteractor;
				if (ixrselectInteractor != null)
				{
					this.m_LineRenderableAsSelectInteractor = ixrselectInteractor;
					this.m_HasSelectInteractor = true;
				}
				IXRHoverInteractor ixrhoverInteractor = this.m_LineRenderable as IXRHoverInteractor;
				if (ixrhoverInteractor != null)
				{
					this.m_LineRenderableAsHoverInteractor = ixrhoverInteractor;
					this.m_HasHoverInteractor = true;
				}
				XRRayInteractor xrrayInteractor = this.m_LineRenderable as XRRayInteractor;
				if (xrrayInteractor != null)
				{
					this.m_LineRenderableAsRayInteractor = xrrayInteractor;
					this.m_HasRayInteractor = true;
				}
			}
			this.FindXROrigin();
			this.SetupReticle();
			this.SetupBlockedReticle();
			this.ClearLineRenderer();
			this.UpdateSettings();
		}

		protected void OnEnable()
		{
			if (this.m_LineRenderer == null)
			{
				XRLoggingUtils.LogError(string.Format("Missing Line Renderer component on {0}. Disabling line visual.", this), this);
				base.enabled = false;
				return;
			}
			if (this.m_LineRenderable == null)
			{
				XRLoggingUtils.LogError(string.Format("Missing {0} / Ray Interactor component on {1}. Disabling line visual.", "ILineRenderable", this), this);
				base.enabled = false;
				this.m_LineRenderer.enabled = false;
				return;
			}
			this.m_SnapCurve = true;
			if (this.m_ReticleToUse != null)
			{
				this.m_ReticleToUse.SetActive(false);
				this.m_ReticleToUse = null;
			}
			this.m_BindingsGroup.AddBinding(this.m_UserScaleVar.Subscribe(delegate(float userScale)
			{
				this.m_LineRenderer.widthMultiplier = userScale * Mathf.Clamp(this.m_LineWidth, 0.0001f, 0.05f);
			}));
			Application.onBeforeRender += this.OnBeforeRenderLineVisual;
		}

		protected void OnDisable()
		{
			this.m_BindingsGroup.Clear();
			if (this.m_LineRenderer != null)
			{
				this.m_LineRenderer.enabled = false;
			}
			if (this.m_ReticleToUse != null)
			{
				this.m_ReticleToUse.SetActive(false);
				this.m_ReticleToUse = null;
			}
			Application.onBeforeRender -= this.OnBeforeRenderLineVisual;
		}

		protected void OnDestroy()
		{
			if (this.m_TargetPoints.IsCreated)
			{
				this.m_TargetPoints.Dispose();
			}
			if (this.m_RenderPoints.IsCreated)
			{
				this.m_RenderPoints.Dispose();
			}
			if (this.m_PreviousRenderPoints.IsCreated)
			{
				this.m_PreviousRenderPoints.Dispose();
			}
			this.m_LineLengthOverrideTweenableVariable.Dispose();
		}

		protected void LateUpdate()
		{
			if (this.m_PerformSetup)
			{
				this.UpdateSettings();
				this.m_PerformSetup = false;
			}
			if (this.m_LineRenderer.useWorldSpace && this.m_XROrigin != null)
			{
				GameObject origin = this.m_XROrigin.Origin;
				float value = (origin != null) ? origin.transform.localScale.x : 1f;
				this.m_UserScaleVar.Value = value;
			}
		}

		[BeforeRenderOrder(101)]
		private void OnBeforeRenderLineVisual()
		{
			this.UpdateLineVisual();
		}

		internal void UpdateLineVisual()
		{
			if (this.m_LineRenderableAsBaseInteractor != null && this.m_LineRenderableAsBaseInteractor.disableVisualsWhenBlockedInGroup && this.m_LineRenderableAsBaseInteractor.IsBlockedByInteractionWithinGroup())
			{
				this.m_LineRenderer.enabled = false;
				return;
			}
			this.m_NumRenderPoints = 0;
			if (!this.GetLinePoints(ref this.m_TargetPoints, out this.m_NumTargetPoints) || this.m_NumTargetPoints == 0)
			{
				this.m_LineRenderer.enabled = false;
				return;
			}
			bool flag = this.m_HasSelectInteractor && this.m_LineRenderableAsSelectInteractor.hasSelection;
			bool flag2 = this.m_HasRayInteractor && this.m_LineRenderableAsRayInteractor.lineType == XRRayInteractor.LineType.StraightLine;
			Vector3 b;
			Vector3 vector;
			this.GetLineOriginAndDirection(ref this.m_TargetPoints, this.m_NumTargetPoints, flag2, out b, out vector);
			Vector3 vector2;
			bool flag3;
			this.m_ValidHit = this.ExtractHitInformation(ref this.m_TargetPoints, this.m_NumTargetPoints, out vector2, out flag3);
			bool flag4 = false;
			if (flag)
			{
				for (int i = 0; i < this.m_LineRenderableAsSelectInteractor.interactablesSelected.Count; i++)
				{
					flag4 = ((this.bendingEnabledInteractionLayers & this.m_LineRenderableAsSelectInteractor.interactablesSelected[i].interactionLayers) != 0);
					if (flag4)
					{
						break;
					}
				}
			}
			bool flag5 = flag && flag2 && flag4;
			bool flag6 = this.m_OverrideInteractorLineOrigin && this.m_ValidHit && flag2;
			bool flag7 = (flag3 || flag5 || flag6) && this.m_LineBendRatio < 1f;
			if (flag7)
			{
				this.m_NumTargetPoints = 20;
				this.m_EndPositionInLine = this.m_NumTargetPoints - 1;
				if (flag5)
				{
					this.FindClosestInteractableAttachPoint(b, out vector2);
				}
			}
			XRInteractorLineVisual.EnsureSize(ref this.m_TargetPoints, this.m_NumTargetPoints);
			if (!XRInteractorLineVisual.EnsureSize(ref this.m_RenderPoints, this.m_NumTargetPoints))
			{
				this.m_NumRenderPoints = 0;
			}
			if (!XRInteractorLineVisual.EnsureSize(ref this.m_PreviousRenderPoints, this.m_NumTargetPoints))
			{
				this.m_NumPreviousRenderPoints = 0;
			}
			if (flag7)
			{
				if (this.m_SmoothMovement)
				{
					if (this.m_PreviousShouldBendLine && this.m_NumPreviousRenderPoints > 0)
					{
						float t = this.m_FollowTightness * Time.deltaTime;
						vector = Vector3.Lerp(this.m_PreviousLineDirection, vector, t);
						b = Vector3.Lerp(this.m_PreviousRenderPoints[0], b, t);
					}
					this.m_PreviousLineDirection = vector;
				}
				XRInteractorLineVisual.CalculateLineCurveRenderPoints(this.m_NumTargetPoints, this.m_LineBendRatio, b, vector, vector2, ref this.m_TargetPoints);
			}
			this.m_PreviousShouldBendLine = flag7;
			if (this.m_NumPreviousRenderPoints != this.m_NumTargetPoints)
			{
				this.m_SnapCurve = true;
			}
			else if (this.m_SmoothMovement && this.m_NumPreviousRenderPoints > 0 && this.m_NumPreviousRenderPoints <= this.m_PreviousRenderPoints.Length && this.m_NumTargetPoints > 0 && this.m_NumTargetPoints <= this.m_TargetPoints.Length)
			{
				int index = this.m_NumPreviousRenderPoints - 1;
				int index2 = this.m_NumTargetPoints - 1;
				this.m_SnapCurve = (Vector3.SqrMagnitude(this.m_PreviousRenderPoints[index] - this.m_TargetPoints[index2]) > this.m_SquareSnapThresholdDistance);
			}
			this.AdjustLineAndReticle(flag, flag7, b, vector2);
			bool flag8 = !flag7 && this.m_SmoothMovement && this.m_NumPreviousRenderPoints == this.m_NumTargetPoints && !this.m_SnapCurve;
			if (this.m_OverrideInteractorLineLength || flag8)
			{
				NativeArray<float3> nativeArray = this.m_TargetPoints.Reinterpret<float3>();
				NativeArray<float3> nativeArray2 = this.m_PreviousRenderPoints.Reinterpret<float3>();
				NativeArray<float3> nativeArray3 = this.m_RenderPoints.Reinterpret<float3>();
				float targetLineLength = (this.m_OverrideInteractorLineLength && this.m_AutoAdjustLineLength) ? this.UpdateTargetLineLength(b, vector2, this.m_MinLineLength, this.m_LineLength, this.m_LineRetractionDelay, this.m_LineLengthChangeSpeed, this.m_ValidHit || flag, this.m_UseDistanceToHitAsMaxLineLength) : this.m_LineLength;
				this.m_NumRenderPoints = XRInteractorLineVisual.ComputeNewRenderPoints(this.m_NumRenderPoints, this.m_NumTargetPoints, targetLineLength, flag8, this.m_OverrideInteractorLineLength, this.m_FollowTightness * Time.deltaTime, ref nativeArray, ref nativeArray2, ref nativeArray3);
			}
			else
			{
				NativeArray<Vector3>.Copy(this.m_TargetPoints, 0, this.m_RenderPoints, 0, this.m_NumTargetPoints);
				this.m_NumRenderPoints = this.m_NumTargetPoints;
			}
			if (this.m_ValidHit || (this.m_TreatSelectionAsValidState && flag))
			{
				bool flag9 = false;
				if (!flag && this.m_HasBaseInteractor && this.m_LineRenderableAsBaseInteractor.hasHover)
				{
					XRInteractionManager interactionManager = this.m_LineRenderableAsBaseInteractor.interactionManager;
					bool flag10 = false;
					foreach (IXRHoverInteractable ixrhoverInteractable in this.m_LineRenderableAsBaseInteractor.interactablesHovered)
					{
						IXRSelectInteractable ixrselectInteractable = ixrhoverInteractable as IXRSelectInteractable;
						if (ixrselectInteractable != null && interactionManager.IsSelectPossible(this.m_LineRenderableAsBaseInteractor, ixrselectInteractable))
						{
							flag10 = true;
							break;
						}
					}
					flag9 = !flag10;
				}
				this.SetColorGradient(flag9 ? this.m_BlockedColorGradient : this.m_ValidColorGradient);
				this.AssignReticle(flag9);
			}
			else
			{
				this.ClearReticle();
				this.SetColorGradient(this.m_InvalidColorGradient);
			}
			if (this.m_NumRenderPoints >= 2)
			{
				this.m_LineRenderer.enabled = true;
				this.m_LineRenderer.positionCount = this.m_NumRenderPoints;
				this.m_LineRenderer.SetPositions(this.m_RenderPoints);
				NativeArray<Vector3>.Copy(this.m_RenderPoints, 0, this.m_PreviousRenderPoints, 0, this.m_NumRenderPoints);
				this.m_NumPreviousRenderPoints = this.m_NumRenderPoints;
				this.m_SnapCurve = false;
				return;
			}
			this.m_LineRenderer.enabled = false;
		}

		private bool GetLinePoints(ref NativeArray<Vector3> linePoints, out int numPoints)
		{
			if (this.m_HasAdvancedLineRenderable)
			{
				Ray? rayOriginOverride = null;
				if (this.m_OverrideInteractorLineOrigin && this.m_LineOriginTransform != null)
				{
					Pose worldPose = this.m_LineOriginTransform.GetWorldPose();
					rayOriginOverride = new Ray?(new Ray(worldPose.position, worldPose.forward));
				}
				return this.m_AdvancedLineRenderable.GetLinePoints(ref linePoints, out numPoints, rayOriginOverride);
			}
			bool linePoints2 = this.m_LineRenderable.GetLinePoints(ref this.m_TargetPointsFallback, out numPoints);
			XRInteractorLineVisual.EnsureSize(ref linePoints, numPoints);
			NativeArray<Vector3>.Copy(this.m_TargetPointsFallback, linePoints, numPoints);
			return linePoints2;
		}

		private void AdjustLineAndReticle(bool hasSelection, bool bendLine, in Vector3 lineOrigin, in Vector3 targetEndPoint)
		{
			if (this.m_HasHitInfo)
			{
				this.m_ReticlePos = targetEndPoint;
				if ((this.m_ValidHit || this.m_StopLineAtFirstRaycastHit) && this.m_EndPositionInLine > 0 && this.m_EndPositionInLine < this.m_NumTargetPoints)
				{
					Vector3 vector = this.m_TargetPoints[this.m_EndPositionInLine - 1];
					Vector3 vector2 = this.m_TargetPoints[this.m_EndPositionInLine] - vector;
					Vector3 vector3 = Vector3.Project(this.m_ReticlePos - vector, vector2);
					if (Vector3.Dot(vector3, vector2) < 0f)
					{
						vector3 = Vector3.zero;
					}
					this.m_ReticlePos = vector + vector3;
					this.m_TargetPoints[this.m_EndPositionInLine] = this.m_ReticlePos;
					this.m_NumTargetPoints = this.m_EndPositionInLine + 1;
				}
			}
			if (this.m_StopLineAtSelection && hasSelection && !bendLine)
			{
				float num = Vector3.SqrMagnitude(targetEndPoint - lineOrigin);
				float num2 = Vector3.SqrMagnitude(this.m_TargetPoints[this.m_EndPositionInLine] - lineOrigin);
				if (num < num2 || this.m_EndPositionInLine == 0)
				{
					int num3 = 1;
					float num4 = Vector3.SqrMagnitude(this.m_TargetPoints[num3] - targetEndPoint);
					for (int i = 2; i < this.m_NumTargetPoints; i++)
					{
						float num5 = Vector3.SqrMagnitude(this.m_TargetPoints[i] - targetEndPoint);
						if (num5 >= num4)
						{
							break;
						}
						num3 = i;
						num4 = num5;
					}
					this.m_EndPositionInLine = num3;
					this.m_NumTargetPoints = this.m_EndPositionInLine + 1;
					this.m_ReticlePos = targetEndPoint;
					if (!this.m_HasHitInfo)
					{
						this.m_ReticleNormal = Vector3.Normalize(this.m_TargetPoints[this.m_EndPositionInLine - 1] - this.m_ReticlePos);
					}
					this.m_TargetPoints[this.m_EndPositionInLine] = this.m_ReticlePos;
				}
			}
		}

		private void FindClosestInteractableAttachPoint(in Vector3 lineOrigin, out Vector3 closestPoint)
		{
			List<IXRSelectInteractable> interactablesSelected = this.m_LineRenderableAsSelectInteractor.interactablesSelected;
			closestPoint = interactablesSelected[0].GetAttachTransform(this.m_LineRenderableAsSelectInteractor).position;
			if (interactablesSelected.Count > 1)
			{
				float num = Vector3.SqrMagnitude(closestPoint - lineOrigin);
				for (int i = 1; i < interactablesSelected.Count; i++)
				{
					Vector3 position = interactablesSelected[i].GetAttachTransform(this.m_LineRenderableAsSelectInteractor).position;
					float num2 = Vector3.SqrMagnitude(position - lineOrigin);
					if (num2 < num)
					{
						closestPoint = position;
						num = num2;
					}
				}
			}
		}

		private static bool EnsureSize(ref NativeArray<Vector3> array, int targetSize)
		{
			if (array.IsCreated && array.Length >= targetSize)
			{
				return true;
			}
			if (array.IsCreated)
			{
				array.Dispose();
			}
			array = new NativeArray<Vector3>(targetSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			return false;
		}

		private void GetLineOriginAndDirection(ref NativeArray<Vector3> targetPoints, int numTargetPoints, bool isLineStraight, out Vector3 lineOrigin, out Vector3 lineDirection)
		{
			if (this.m_OverrideInteractorLineOrigin && this.m_LineOriginTransform != null)
			{
				Pose worldPose = this.m_LineOriginTransform.GetWorldPose();
				lineOrigin = worldPose.position;
				lineDirection = worldPose.forward;
			}
			else if (this.m_HasAdvancedLineRenderable)
			{
				this.m_AdvancedLineRenderable.GetLineOriginAndDirection(out lineOrigin, out lineDirection);
			}
			else
			{
				lineOrigin = targetPoints[0];
				Vector3 a = targetPoints[numTargetPoints - 1];
				lineDirection = (a - lineOrigin).normalized;
			}
			if (isLineStraight && this.m_LineOriginOffset > 0f && (!this.m_OverrideInteractorLineLength || this.m_LineOriginOffset < this.m_LineLength))
			{
				lineOrigin += lineDirection * this.m_LineOriginOffset;
			}
			targetPoints[0] = lineOrigin;
		}

		private bool ExtractHitInformation(ref NativeArray<Vector3> targetPoints, int numTargetPoints, out Vector3 targetEndPoint, out bool hitSnapVolume)
		{
			Collider collider = null;
			hitSnapVolume = false;
			targetEndPoint = targetPoints[numTargetPoints - 1];
			bool flag;
			this.m_HasHitInfo = this.m_LineRenderable.TryGetHitInfo(out this.m_CurrentHitPoint, out this.m_ReticleNormal, out this.m_EndPositionInLine, out flag);
			if (this.m_HasHitInfo)
			{
				targetEndPoint = this.m_CurrentHitPoint;
				RaycastHit? raycastHit;
				int num;
				RaycastResult? raycastResult;
				int num2;
				bool flag2;
				if (flag && this.m_SnapEndpointIfAvailable && this.m_HasRayInteractor && this.m_LineRenderableAsRayInteractor.TryGetCurrentRaycast(out raycastHit, out num, out raycastResult, out num2, out flag2) && !flag2)
				{
					if (raycastHit != null)
					{
						collider = raycastHit.Value.collider;
					}
					if (collider != this.m_PreviousCollider && collider != null)
					{
						IXRInteractable ixrinteractable;
						this.m_LineRenderableAsBaseInteractor.interactionManager.TryGetInteractableForCollider(collider, out ixrinteractable, out this.m_XRInteractableSnapVolume);
					}
					if (this.m_XRInteractableSnapVolume != null)
					{
						targetEndPoint = (this.m_LineRenderableAsRayInteractor.hasSelection ? this.m_XRInteractableSnapVolume.GetClosestPointOfAttachTransform(this.m_LineRenderableAsRayInteractor) : this.m_XRInteractableSnapVolume.GetClosestPoint(targetEndPoint));
						this.m_EndPositionInLine = 19;
						hitSnapVolume = true;
					}
				}
			}
			if (collider == null)
			{
				this.m_XRInteractableSnapVolume = null;
			}
			this.m_PreviousCollider = collider;
			return flag;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$PostfixBurstDelegate))]
		private static void CalculateLineCurveRenderPoints(int numTargetPoints, float curveRatio, in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 endPoint, ref NativeArray<Vector3> targetPoints)
		{
			XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall.Invoke(numTargetPoints, curveRatio, lineOrigin, lineDirection, endPoint, ref targetPoints);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$PostfixBurstDelegate))]
		private static int ComputeNewRenderPoints(int numRenderPoints, int numTargetPoints, float targetLineLength, bool shouldSmoothPoints, bool shouldOverwritePoints, float pointSmoothIncrement, ref NativeArray<float3> targetPoints, ref NativeArray<float3> previousRenderPoints, ref NativeArray<float3> renderPoints)
		{
			return XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$BurstDirectCall.Invoke(numRenderPoints, numTargetPoints, targetLineLength, shouldSmoothPoints, shouldOverwritePoints, pointSmoothIncrement, ref targetPoints, ref previousRenderPoints, ref renderPoints);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$PostfixBurstDelegate))]
		private static bool EvaluateLineEndPoint(float targetLineLength, bool shouldSmoothPoint, in float3 unsmoothedTargetPoint, in float3 lastRenderPoint, ref float3 newRenderPoint, ref float lineLength)
		{
			return XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$BurstDirectCall.Invoke(targetLineLength, shouldSmoothPoint, unsmoothedTargetPoint, lastRenderPoint, ref newRenderPoint, ref lineLength);
		}

		private float UpdateTargetLineLength(in Vector3 lineOrigin, in Vector3 hitPoint, float minimumLineLength, float maximumLineLength, float lineRetractionDelaySeconds, float lineRetractionScalar, bool hasHit, bool deriveMaxLineLength)
		{
			float unscaledTime = Time.unscaledTime;
			if (hasHit)
			{
				this.m_LastValidHitTime = Time.unscaledTime;
				this.m_LastValidLineLength = (deriveMaxLineLength ? Mathf.Min(Vector3.Distance(lineOrigin, hitPoint), maximumLineLength) : maximumLineLength);
			}
			float num = unscaledTime - this.m_LastValidHitTime;
			if (num > lineRetractionDelaySeconds)
			{
				this.m_LineLengthOverrideTweenableVariable.target = minimumLineLength;
				float num2 = (num - lineRetractionDelaySeconds) * lineRetractionScalar;
				this.m_LineLengthOverrideTweenableVariable.HandleTween(Time.unscaledDeltaTime * num2);
			}
			else
			{
				this.m_LineLengthOverrideTweenableVariable.target = Mathf.Max(this.m_LastValidLineLength, minimumLineLength);
				this.m_LineLengthOverrideTweenableVariable.HandleTween(Time.unscaledDeltaTime * lineRetractionScalar);
			}
			return this.m_LineLengthOverrideTweenableVariable.Value;
		}

		private void AssignReticle(bool useBlockedVisuals)
		{
			GameObject reticleToUse = this.m_ReticleToUse;
			GameObject gameObject = useBlockedVisuals ? this.m_BlockedReticle : this.m_Reticle;
			this.m_ReticleToUse = (this.m_CustomReticleAttached ? this.m_CustomReticle : gameObject);
			if (reticleToUse != null && reticleToUse != this.m_ReticleToUse)
			{
				reticleToUse.SetActive(false);
			}
			if (this.m_ReticleToUse != null)
			{
				if (this.m_HasHoverInteractor)
				{
					IXRReticleDirectionProvider ixrreticleDirectionProvider = this.m_LineRenderableAsHoverInteractor.GetOldestInteractableHovered() as IXRReticleDirectionProvider;
					if (ixrreticleDirectionProvider != null)
					{
						Vector3 vector;
						Vector3? vector2;
						ixrreticleDirectionProvider.GetReticleDirection(this.m_LineRenderableAsHoverInteractor, this.m_ReticleNormal, out vector, out vector2);
						Quaternion rotation;
						if (vector2 != null)
						{
							Vector3 vector3 = vector2.Value;
							BurstMathUtility.LookRotationWithForwardProjectedOnPlane(vector3, vector, out rotation);
						}
						else
						{
							Vector3 vector3 = this.m_ReticleToUse.transform.forward;
							BurstMathUtility.LookRotationWithForwardProjectedOnPlane(vector3, vector, out rotation);
						}
						this.m_ReticleToUse.transform.SetWorldPose(new Pose(this.m_ReticlePos, rotation));
						goto IL_11A;
					}
				}
				this.m_ReticleToUse.transform.SetWorldPose(new Pose(this.m_ReticlePos, Quaternion.LookRotation(-this.m_ReticleNormal)));
				IL_11A:
				this.m_ReticleToUse.SetActive(true);
			}
		}

		private void ClearReticle()
		{
			if (this.m_ReticleToUse != null)
			{
				this.m_ReticleToUse.SetActive(false);
				this.m_ReticleToUse = null;
			}
		}

		private void SetColorGradient(Gradient colorGradient)
		{
			if (!this.m_SetLineColorGradient)
			{
				return;
			}
			this.m_LineRenderer.colorGradient = colorGradient;
		}

		private void UpdateSettings()
		{
			this.m_SquareSnapThresholdDistance = this.m_SnapThresholdDistance * this.m_SnapThresholdDistance;
			if (this.TryFindLineRenderer())
			{
				this.m_LineRenderer.widthMultiplier = Mathf.Clamp(this.m_LineWidth, 0.0001f, 0.05f);
				this.m_LineRenderer.widthCurve = this.m_WidthCurve;
				this.m_SnapCurve = true;
			}
			this.m_LineLengthOverrideTweenableVariable.target = this.lineLength;
			this.m_LineLengthOverrideTweenableVariable.HandleTween(1f);
		}

		private bool TryFindLineRenderer()
		{
			this.m_LineRenderer = base.GetComponent<LineRenderer>();
			if (this.m_LineRenderer == null)
			{
				Debug.LogWarning("No Line Renderer found for Interactor Line Visual.", this);
				base.enabled = false;
				return false;
			}
			return true;
		}

		private void ClearLineRenderer()
		{
			if (this.TryFindLineRenderer())
			{
				this.m_LineRenderer.SetPositions(this.m_ClearArray);
				this.m_LineRenderer.positionCount = 0;
			}
		}

		private void FindXROrigin()
		{
			if (this.m_XROrigin == null)
			{
				ComponentLocatorUtility<XROrigin>.TryFindComponent(out this.m_XROrigin);
			}
		}

		private void SetupReticle()
		{
			if (this.m_Reticle == null)
			{
				return;
			}
			if (!this.m_Reticle.scene.IsValid())
			{
				this.m_Reticle = Object.Instantiate<GameObject>(this.m_Reticle);
			}
			this.m_Reticle.SetActive(false);
		}

		private void SetupBlockedReticle()
		{
			if (this.m_BlockedReticle == null)
			{
				return;
			}
			if (!this.m_BlockedReticle.scene.IsValid())
			{
				this.m_BlockedReticle = Object.Instantiate<GameObject>(this.m_BlockedReticle);
			}
			this.m_BlockedReticle.SetActive(false);
		}

		public bool AttachCustomReticle(GameObject reticleInstance)
		{
			this.m_CustomReticle = reticleInstance;
			this.m_CustomReticleAttached = true;
			return true;
		}

		public bool RemoveCustomReticle()
		{
			this.m_CustomReticle = null;
			this.m_CustomReticleAttached = false;
			return true;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CalculateLineCurveRenderPoints$BurstManaged(int numTargetPoints, float curveRatio, in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 endPoint, ref NativeArray<Vector3> targetPoints)
		{
			NativeArray<float3> nativeArray = targetPoints.Reinterpret<float3>();
			float3 @float = lineOrigin;
			float3 float2 = lineDirection;
			float3 float3 = endPoint;
			CurveUtility.GenerateCubicBezierCurve(numTargetPoints, curveRatio, @float, float2, float3, ref nativeArray);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int ComputeNewRenderPoints$BurstManaged(int numRenderPoints, int numTargetPoints, float targetLineLength, bool shouldSmoothPoints, bool shouldOverwritePoints, float pointSmoothIncrement, ref NativeArray<float3> targetPoints, ref NativeArray<float3> previousRenderPoints, ref NativeArray<float3> renderPoints)
		{
			float num = 0f;
			int length = renderPoints.Length;
			int num2 = numRenderPoints;
			int num3 = 0;
			while (num3 < numTargetPoints && num2 < length)
			{
				float3 @float = targetPoints[num3];
				float3 value = (!shouldSmoothPoints) ? @float : math.lerp(previousRenderPoints[num3], @float, pointSmoothIncrement);
				if (shouldOverwritePoints && num2 > 0 && length > 0)
				{
					float3 float2 = renderPoints[num2 - 1];
					if (XRInteractorLineVisual.EvaluateLineEndPoint(targetLineLength, shouldSmoothPoints, @float, float2, ref value, ref num))
					{
						renderPoints[num2] = value;
						num2++;
						break;
					}
				}
				renderPoints[num2] = value;
				num2++;
				num3++;
			}
			return num2;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool EvaluateLineEndPoint$BurstManaged(float targetLineLength, bool shouldSmoothPoint, in float3 unsmoothedTargetPoint, in float3 lastRenderPoint, ref float3 newRenderPoint, ref float lineLength)
		{
			float3 x = newRenderPoint - lastRenderPoint;
			float num = math.length(x);
			if (shouldSmoothPoint)
			{
				float num2 = math.distance(lastRenderPoint, unsmoothedTargetPoint);
				if (num2 < num)
				{
					newRenderPoint = lastRenderPoint + math.normalize(x) * num2;
					num = num2;
				}
			}
			lineLength += num;
			if (lineLength <= targetLineLength)
			{
				return false;
			}
			float num3 = lineLength - targetLineLength;
			float t = 1f - num3 / num;
			newRenderPoint = math.lerp(lastRenderPoint, newRenderPoint, t);
			return true;
		}

		private const float k_MinLineWidth = 0.0001f;

		private const float k_MaxLineWidth = 0.05f;

		private const float k_MinLineBendRatio = 0.01f;

		private const float k_MaxLineBendRatio = 1f;

		[SerializeField]
		[Range(0.0001f, 0.05f)]
		private float m_LineWidth = 0.005f;

		[SerializeField]
		private bool m_OverrideInteractorLineLength = true;

		[SerializeField]
		private float m_LineLength = 10f;

		[SerializeField]
		private bool m_AutoAdjustLineLength;

		[SerializeField]
		private float m_MinLineLength = 0.5f;

		[SerializeField]
		private bool m_UseDistanceToHitAsMaxLineLength = true;

		[SerializeField]
		private float m_LineRetractionDelay = 0.5f;

		[SerializeField]
		private float m_LineLengthChangeSpeed = 12f;

		[SerializeField]
		private AnimationCurve m_WidthCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		[SerializeField]
		private bool m_SetLineColorGradient = true;

		[SerializeField]
		private Gradient m_ValidColorGradient = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(Color.white, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		[SerializeField]
		private Gradient m_InvalidColorGradient = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(Color.red, 0f),
				new GradientColorKey(Color.red, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		[SerializeField]
		private Gradient m_BlockedColorGradient = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(Color.yellow, 0f),
				new GradientColorKey(Color.yellow, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		[SerializeField]
		private bool m_TreatSelectionAsValidState;

		[SerializeField]
		private bool m_SmoothMovement;

		[SerializeField]
		private float m_FollowTightness = 10f;

		[SerializeField]
		private float m_SnapThresholdDistance = 10f;

		[SerializeField]
		private GameObject m_Reticle;

		[SerializeField]
		private GameObject m_BlockedReticle;

		[SerializeField]
		private bool m_StopLineAtFirstRaycastHit = true;

		[SerializeField]
		private bool m_StopLineAtSelection;

		[SerializeField]
		private bool m_SnapEndpointIfAvailable = true;

		[SerializeField]
		[Range(0.01f, 1f)]
		private float m_LineBendRatio = 0.5f;

		[SerializeField]
		private InteractionLayerMask m_BendingEnabledInteractionLayers = -1;

		[SerializeField]
		private bool m_OverrideInteractorLineOrigin = true;

		[SerializeField]
		private Transform m_LineOriginTransform;

		[SerializeField]
		private float m_LineOriginOffset;

		private float m_SquareSnapThresholdDistance;

		private Vector3 m_ReticlePos;

		private Vector3 m_ReticleNormal;

		private int m_EndPositionInLine;

		private bool m_SnapCurve = true;

		private bool m_PerformSetup;

		private GameObject m_ReticleToUse;

		private LineRenderer m_LineRenderer;

		private ILineRenderable m_LineRenderable;

		private IAdvancedLineRenderable m_AdvancedLineRenderable;

		private bool m_HasAdvancedLineRenderable;

		private IXRSelectInteractor m_LineRenderableAsSelectInteractor;

		private IXRHoverInteractor m_LineRenderableAsHoverInteractor;

		private XRBaseInteractor m_LineRenderableAsBaseInteractor;

		private XRRayInteractor m_LineRenderableAsRayInteractor;

		private NativeArray<Vector3> m_TargetPoints;

		private int m_NumTargetPoints = -1;

		private Vector3[] m_TargetPointsFallback = Array.Empty<Vector3>();

		private NativeArray<Vector3> m_RenderPoints;

		private int m_NumRenderPoints = -1;

		private NativeArray<Vector3> m_PreviousRenderPoints;

		private int m_NumPreviousRenderPoints = -1;

		private readonly Vector3[] m_ClearArray = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero
		};

		private GameObject m_CustomReticle;

		private bool m_CustomReticleAttached;

		private XRInteractableSnapVolume m_XRInteractableSnapVolume;

		private const int k_NumberOfSegmentsForBendableLine = 20;

		private bool m_PreviousShouldBendLine;

		private Vector3 m_PreviousLineDirection;

		private Vector3 m_CurrentHitPoint;

		private bool m_HasHitInfo;

		private bool m_ValidHit;

		private float m_LastValidHitTime;

		private float m_LastValidLineLength;

		private Collider m_PreviousCollider;

		private XROrigin m_XROrigin;

		private bool m_HasRayInteractor;

		private bool m_HasBaseInteractor;

		private bool m_HasHoverInteractor;

		private bool m_HasSelectInteractor;

		private readonly BindableVariable<float> m_UserScaleVar = new BindableVariable<float>(0f, true, null, false);

		private readonly FloatTweenableVariable m_LineLengthOverrideTweenableVariable = new FloatTweenableVariable();

		private readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateLineCurveRenderPoints_00000D7C$PostfixBurstDelegate(int numTargetPoints, float curveRatio, in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 endPoint, ref NativeArray<Vector3> targetPoints);

		internal static class CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall.Pointer == 0)
				{
					XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$PostfixBurstDelegate>(new XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$PostfixBurstDelegate(XRInteractorLineVisual.CalculateLineCurveRenderPoints)).Value;
				}
				A_0 = XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(int numTargetPoints, float curveRatio, in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 endPoint, ref NativeArray<Vector3> targetPoints)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRInteractorLineVisual.CalculateLineCurveRenderPoints_00000D7C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Int32,System.Single,UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&,Unity.Collections.NativeArray`1<UnityEngine.Vector3>&), numTargetPoints, curveRatio, ref lineOrigin, ref lineDirection, ref endPoint, ref targetPoints, functionPointer);
						return;
					}
				}
				XRInteractorLineVisual.CalculateLineCurveRenderPoints$BurstManaged(numTargetPoints, curveRatio, lineOrigin, lineDirection, endPoint, ref targetPoints);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int ComputeNewRenderPoints_00000D7D$PostfixBurstDelegate(int numRenderPoints, int numTargetPoints, float targetLineLength, bool shouldSmoothPoints, bool shouldOverwritePoints, float pointSmoothIncrement, ref NativeArray<float3> targetPoints, ref NativeArray<float3> previousRenderPoints, ref NativeArray<float3> renderPoints);

		internal static class ComputeNewRenderPoints_00000D7D$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$BurstDirectCall.Pointer == 0)
				{
					XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$PostfixBurstDelegate>(new XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$PostfixBurstDelegate(XRInteractorLineVisual.ComputeNewRenderPoints)).Value;
				}
				A_0 = XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static int Invoke(int numRenderPoints, int numTargetPoints, float targetLineLength, bool shouldSmoothPoints, bool shouldOverwritePoints, float pointSmoothIncrement, ref NativeArray<float3> targetPoints, ref NativeArray<float3> previousRenderPoints, ref NativeArray<float3> renderPoints)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRInteractorLineVisual.ComputeNewRenderPoints_00000D7D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(System.Int32,System.Int32,System.Single,System.Boolean,System.Boolean,System.Single,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&), numRenderPoints, numTargetPoints, targetLineLength, shouldSmoothPoints, shouldOverwritePoints, pointSmoothIncrement, ref targetPoints, ref previousRenderPoints, ref renderPoints, functionPointer);
					}
				}
				return XRInteractorLineVisual.ComputeNewRenderPoints$BurstManaged(numRenderPoints, numTargetPoints, targetLineLength, shouldSmoothPoints, shouldOverwritePoints, pointSmoothIncrement, ref targetPoints, ref previousRenderPoints, ref renderPoints);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool EvaluateLineEndPoint_00000D7E$PostfixBurstDelegate(float targetLineLength, bool shouldSmoothPoint, in float3 unsmoothedTargetPoint, in float3 lastRenderPoint, ref float3 newRenderPoint, ref float lineLength);

		internal static class EvaluateLineEndPoint_00000D7E$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$BurstDirectCall.Pointer == 0)
				{
					XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$PostfixBurstDelegate>(new XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$PostfixBurstDelegate(XRInteractorLineVisual.EvaluateLineEndPoint)).Value;
				}
				A_0 = XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(float targetLineLength, bool shouldSmoothPoint, in float3 unsmoothedTargetPoint, in float3 lastRenderPoint, ref float3 newRenderPoint, ref float lineLength)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRInteractorLineVisual.EvaluateLineEndPoint_00000D7E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(System.Single,System.Boolean,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single&), targetLineLength, shouldSmoothPoint, ref unsmoothedTargetPoint, ref lastRenderPoint, ref newRenderPoint, ref lineLength, functionPointer);
					}
				}
				return XRInteractorLineVisual.EvaluateLineEndPoint$BurstManaged(targetLineLength, shouldSmoothPoint, unsmoothedTargetPoint, lastRenderPoint, ref newRenderPoint, ref lineLength);
			}

			private static IntPtr Pointer;
		}
	}
}
