using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Gaze
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/XR Gaze Assistance", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Gaze.XRGazeAssistance.html")]
	[DefaultExecutionOrder(-29980)]
	[BurstCompile]
	public class XRGazeAssistance : MonoBehaviour, IXRAimAssist
	{
		public XRGazeInteractor gazeInteractor
		{
			get
			{
				return this.m_GazeInteractor;
			}
			set
			{
				this.m_GazeInteractor = value;
			}
		}

		public float fallbackDivergence
		{
			get
			{
				return this.m_FallbackDivergence;
			}
			set
			{
				this.m_FallbackDivergence = Mathf.Clamp(value, 0f, 90f);
			}
		}

		public bool hideCursorWithNoActiveRays
		{
			get
			{
				return this.m_HideCursorWithNoActiveRays;
			}
			set
			{
				this.m_HideCursorWithNoActiveRays = value;
			}
		}

		public List<XRGazeAssistance.InteractorData> rayInteractors
		{
			get
			{
				return this.m_RayInteractors;
			}
			set
			{
				this.m_RayInteractors = value;
			}
		}

		public float aimAssistRequiredAngle
		{
			get
			{
				return this.m_AimAssistRequiredAngle;
			}
			set
			{
				this.m_AimAssistRequiredAngle = Mathf.Clamp(value, 0f, 90f);
			}
		}

		public float aimAssistRequiredSpeed
		{
			get
			{
				return this.m_AimAssistRequiredSpeed;
			}
			set
			{
				this.m_AimAssistRequiredSpeed = value;
			}
		}

		public float aimAssistPercent
		{
			get
			{
				return this.m_AimAssistPercent;
			}
			set
			{
				this.m_AimAssistPercent = Mathf.Clamp01(value);
			}
		}

		public float aimAssistMaxSpeedPercent
		{
			get
			{
				return this.m_AimAssistMaxSpeedPercent;
			}
			set
			{
				this.m_AimAssistMaxSpeedPercent = value;
			}
		}

		private void Initialize()
		{
			if (this.m_GazeInteractor != null)
			{
				this.m_HasGazeReticleVisual = this.m_GazeInteractor.TryGetComponent<XRInteractorReticleVisual>(out this.m_GazeReticleVisual);
				for (int i = 0; i < this.m_RayInteractors.Count; i++)
				{
					this.m_RayInteractors[i].Initialize();
				}
				return;
			}
			Debug.LogError(string.Format("Gaze Interactor not set or missing on {0}. Disabling this XR Gaze Assistance component.", this), this);
			base.enabled = false;
		}

		protected void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRender;
		}

		protected void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRender;
		}

		protected void Start()
		{
			this.Initialize();
		}

		protected void Update()
		{
			Transform rayOriginTransform = this.m_GazeInteractor.rayOriginTransform;
			for (int i = 0; i < this.m_RayInteractors.Count; i++)
			{
				XRGazeAssistance.InteractorData interactorData = this.m_RayInteractors[i];
				interactorData.RestoreVisuals();
				interactorData.UpdateFallbackRayOrigin(rayOriginTransform);
			}
		}

		protected void LateUpdate()
		{
			if (!this.m_GazeInteractor.isActiveAndEnabled)
			{
				return;
			}
			Transform rayOriginTransform = this.m_GazeInteractor.rayOriginTransform;
			if (this.m_SelectingInteractorData != null && !this.m_SelectingInteractorData.UpdateFallbackState(rayOriginTransform, this.m_FallbackDivergence, false))
			{
				this.m_SelectingInteractorData = null;
			}
			bool flag = false;
			for (int i = 0; i < this.m_RayInteractors.Count; i++)
			{
				XRGazeAssistance.InteractorData interactorData = this.m_RayInteractors[i];
				if (interactorData.fallback)
				{
					flag = true;
				}
				if (interactorData != this.m_SelectingInteractorData && interactorData.UpdateFallbackState(rayOriginTransform, this.m_FallbackDivergence, this.m_SelectingInteractorData != null))
				{
					this.m_SelectingInteractorData = interactorData;
				}
			}
			if (this.m_HideCursorWithNoActiveRays && this.m_HasGazeReticleVisual)
			{
				bool flag2 = this.m_SelectingInteractorData != null;
				this.m_GazeReticleVisual.enabled = (flag && !flag2);
			}
		}

		[BeforeRenderOrder(95)]
		private void OnBeforeRender()
		{
			for (int i = 0; i < this.m_RayInteractors.Count; i++)
			{
				this.m_RayInteractors[i].UpdateLineVisualOrigin();
			}
		}

		public Vector3 GetAssistedVelocity(in Vector3 source, in Vector3 velocity, float gravity)
		{
			Vector3 rayEndPoint = this.m_GazeInteractor.rayEndPoint;
			Vector3 result;
			XRGazeAssistance.GetAssistedVelocityInternal(source, rayEndPoint, velocity, gravity, this.m_AimAssistRequiredAngle, this.m_AimAssistRequiredSpeed, this.m_AimAssistMaxSpeedPercent, this.m_AimAssistPercent, Mathf.Epsilon, out result);
			return result;
		}

		public Vector3 GetAssistedVelocity(in Vector3 source, in Vector3 velocity, float gravity, float maxAngle)
		{
			Vector3 rayEndPoint = this.m_GazeInteractor.rayEndPoint;
			Vector3 result;
			XRGazeAssistance.GetAssistedVelocityInternal(source, rayEndPoint, velocity, gravity, maxAngle, this.m_AimAssistRequiredSpeed, this.m_AimAssistMaxSpeedPercent, this.m_AimAssistPercent, Mathf.Epsilon, out result);
			return result;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGazeAssistance.GetAssistedVelocityInternal_0000101E$PostfixBurstDelegate))]
		private static void GetAssistedVelocityInternal(in Vector3 source, in Vector3 target, in Vector3 velocity, float gravity, float maxAngle, float requiredSpeed, float maxSpeedPercent, float assistPercent, float epsilon, out Vector3 adjustedVelocity)
		{
			XRGazeAssistance.GetAssistedVelocityInternal_0000101E$BurstDirectCall.Invoke(source, target, velocity, gravity, maxAngle, requiredSpeed, maxSpeedPercent, assistPercent, epsilon, out adjustedVelocity);
		}

		Vector3 IXRAimAssist.GetAssistedVelocity(in Vector3 source, in Vector3 velocity, float gravity)
		{
			return this.GetAssistedVelocity(source, velocity, gravity);
		}

		Vector3 IXRAimAssist.GetAssistedVelocity(in Vector3 source, in Vector3 velocity, float gravity, float maxAngle)
		{
			return this.GetAssistedVelocity(source, velocity, gravity, maxAngle);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetAssistedVelocityInternal$BurstManaged(in Vector3 source, in Vector3 target, in Vector3 velocity, float gravity, float maxAngle, float requiredSpeed, float maxSpeedPercent, float assistPercent, float epsilon, out Vector3 adjustedVelocity)
		{
			Vector3 vector = target - source;
			float num = math.length(velocity);
			float3 v = math.normalize(velocity);
			float3 @float = math.normalize(vector);
			if (Vector3.Angle(v, @float) > maxAngle)
			{
				adjustedVelocity = velocity;
				return;
			}
			if (gravity < epsilon)
			{
				adjustedVelocity = @float * num;
				return;
			}
			if (num < requiredSpeed)
			{
				adjustedVelocity = velocity;
				return;
			}
			float3 x = vector;
			x.y = 0f;
			float num2 = math.length(x);
			if (num2 < epsilon)
			{
				adjustedVelocity = velocity;
				return;
			}
			float2 float2 = new float2(math.sqrt(0.5f * gravity * (num2 * num2) / (num2 - vector.y)), 0f);
			float2.y = float2.x;
			float2 float3 = new float2(float2.x, 0f);
			if (vector.y < 0f)
			{
				float3.x = math.sqrt(0.5f * gravity * num2 * num2 / -vector.y);
			}
			else
			{
				float3.x *= 2f;
				float3.y = float3.x * (vector.y + 0.5f * gravity * (num2 / float3.x) * (num2 / float3.x)) / num2;
			}
			float num3 = math.length(float2);
			float num4 = math.length(float3);
			float num5 = math.abs(num3 - num);
			float num6 = math.abs(num4 - num);
			if (velocity.y <= 0f)
			{
				num6 *= 0.25f;
			}
			float2 float4 = (num5 < num6) ? float2 : float3;
			float4 = math.normalize(float4) * math.min(math.length(float4), maxSpeedPercent * num);
			float3 x2 = math.normalize(x) * float4.x;
			x2.y = float4.y;
			adjustedVelocity = Vector3.Slerp(v, math.normalize(x2), assistPercent) * math.lerp(num, math.length(x2), assistPercent);
		}

		private const float k_MinAttachDistance = 0.5f;

		private const float k_MinFallbackDivergence = 0f;

		private const float k_MaxFallbackDivergence = 90f;

		private const float k_MinAimAssistRequiredAngle = 0f;

		private const float k_MaxAimAssistRequiredAngle = 90f;

		[SerializeField]
		[Tooltip("Eye data source used as fallback data and to determine if fallback data should be used.")]
		private XRGazeInteractor m_GazeInteractor;

		[SerializeField]
		[Range(0f, 90f)]
		[Tooltip("How far an interactor must point away from the user's view area before eye gaze will be used instead.")]
		private float m_FallbackDivergence = 60f;

		[SerializeField]
		[Tooltip("If the eye reticle should be hidden when all interactors are using their original data.")]
		private bool m_HideCursorWithNoActiveRays = true;

		[SerializeField]
		[Tooltip("Interactors that can fall back to gaze data.")]
		private List<XRGazeAssistance.InteractorData> m_RayInteractors = new List<XRGazeAssistance.InteractorData>();

		[SerializeField]
		[Tooltip("How far projectiles can aim outside of eye gaze and still be considered for aim assist.")]
		[Range(0f, 90f)]
		private float m_AimAssistRequiredAngle = 30f;

		[SerializeField]
		[Tooltip("How fast a projectile must be moving to be considered for aim assist.")]
		private float m_AimAssistRequiredSpeed = 0.25f;

		[SerializeField]
		[Tooltip("How much of the corrected aim velocity to use, as a percentage.")]
		[Range(0f, 1f)]
		private float m_AimAssistPercent = 0.8f;

		[SerializeField]
		[Tooltip("How much additional speed a projectile can receive from aim assistance, as a percentage.")]
		private float m_AimAssistMaxSpeedPercent = 10f;

		private XRGazeAssistance.InteractorData m_SelectingInteractorData;

		private XRInteractorReticleVisual m_GazeReticleVisual;

		private bool m_HasGazeReticleVisual;

		[Serializable]
		public sealed class InteractorData
		{
			public Object interactor
			{
				get
				{
					return this.m_Interactor;
				}
				set
				{
					this.m_Interactor = value;
				}
			}

			public bool teleportRay
			{
				get
				{
					return this.m_TeleportRay;
				}
				set
				{
					this.m_TeleportRay = value;
				}
			}

			public bool fallback { get; private set; }

			internal void Initialize()
			{
				if (this.m_Initialized)
				{
					return;
				}
				this.m_RayProvider = (this.m_Interactor as IXRRayProvider);
				this.m_SelectInteractor = (this.m_Interactor as IXRSelectInteractor);
				if (this.m_RayProvider == null || this.m_SelectInteractor == null)
				{
					Debug.LogWarning("No ray and select interactor found!");
					return;
				}
				this.m_OriginalRayOrigin = this.m_RayProvider.GetOrCreateRayOrigin();
				this.m_OriginalAttach = this.m_RayProvider.GetOrCreateAttachTransform();
				Transform transform = this.m_SelectInteractor.transform;
				string name = transform.gameObject.name;
				this.m_FallbackRayOrigin = new GameObject("Gaze Assistance [" + name + "] Ray Origin").transform;
				this.m_FallbackAttach = new GameObject("Gaze Assistance [" + name + "] Attach").transform;
				this.m_FallbackRayOrigin.parent = this.m_OriginalRayOrigin.parent;
				this.m_FallbackAttach.parent = this.m_FallbackRayOrigin;
				this.m_HasLineVisual = transform.TryGetComponent<XRInteractorLineVisual>(out this.m_LineVisual);
				if (this.m_HasLineVisual)
				{
					this.m_FallbackVisualLineOrigin = new GameObject("Gaze Assistance [" + name + "] Visual Origin").transform;
					this.m_FallbackVisualLineOrigin.parent = this.m_FallbackRayOrigin.parent;
				}
				this.m_Initialized = true;
			}

			internal void UpdateFallbackRayOrigin(Transform gazeTransform)
			{
				if (!this.m_Initialized)
				{
					return;
				}
				if (this.fallback)
				{
					this.m_FallbackRayOrigin.SetWorldPose(gazeTransform.GetWorldPose());
				}
			}

			internal void UpdateLineVisualOrigin()
			{
				if (!this.m_Initialized)
				{
					return;
				}
				if (this.m_HasLineVisual && this.fallback)
				{
					Pose pose;
					if (this.m_OriginalOverrideVisualLineOrigin && this.m_OriginalVisualLineOrigin != null)
					{
						pose = (this.m_TeleportRay ? new Pose(this.m_OriginalVisualLineOrigin.position, this.m_FallbackRayOrigin.rotation) : this.m_OriginalVisualLineOrigin.GetWorldPose());
					}
					else
					{
						pose = (this.m_TeleportRay ? new Pose(this.m_OriginalRayOrigin.position, this.m_FallbackRayOrigin.rotation) : this.m_OriginalRayOrigin.GetWorldPose());
					}
					this.m_FallbackVisualLineOrigin.SetWorldPose(pose);
				}
			}

			internal bool UpdateFallbackState(Transform gazeTransform, float fallbackDivergence, bool selectionLocked)
			{
				if (!this.m_Initialized)
				{
					return false;
				}
				bool flag = !selectionLocked && Vector3.Angle(gazeTransform.forward, this.m_OriginalRayOrigin.forward) > fallbackDivergence;
				if (!this.m_SelectInteractor.isSelectActive)
				{
					if (flag && !this.fallback)
					{
						if (this.m_HasLineVisual)
						{
							this.m_OriginalOverrideVisualLineOrigin = this.m_LineVisual.overrideInteractorLineOrigin;
							this.m_OriginalVisualLineOrigin = this.m_LineVisual.lineOriginTransform;
							this.m_LineVisual.overrideInteractorLineOrigin = true;
							this.m_LineVisual.lineOriginTransform = this.m_FallbackVisualLineOrigin;
						}
						this.m_RayProvider.SetRayOrigin(this.m_FallbackRayOrigin);
						this.m_RayProvider.SetAttachTransform(this.m_FallbackAttach);
					}
					else if (!flag && this.fallback)
					{
						if (this.m_HasLineVisual)
						{
							this.m_LineVisual.overrideInteractorLineOrigin = this.m_OriginalOverrideVisualLineOrigin;
							this.m_LineVisual.lineOriginTransform = this.m_OriginalVisualLineOrigin;
						}
						this.m_RayProvider.SetRayOrigin(this.m_OriginalRayOrigin);
						this.m_RayProvider.SetAttachTransform(this.m_OriginalAttach);
						if (!this.m_TeleportRay)
						{
							this.m_RestoreVisuals = true;
						}
					}
					this.fallback = flag;
				}
				if (this.fallback)
				{
					Pose worldPose = gazeTransform.GetWorldPose();
					if (!this.m_TeleportRay && this.m_SelectInteractor.isSelectActive && this.m_SelectInteractor.hasSelection)
					{
						float t = Mathf.Clamp01((this.m_FallbackAttach.position - worldPose.position).magnitude / 0.5f);
						Pose worldPose2 = this.m_OriginalRayOrigin.GetWorldPose();
						this.m_FallbackRayOrigin.SetPositionAndRotation(Vector3.Lerp(worldPose2.position, worldPose.position, t), Quaternion.Lerp(worldPose2.rotation, worldPose.rotation, t));
						if (this.m_HasLineVisual)
						{
							this.m_LineVisual.enabled = true;
						}
						return true;
					}
					if (this.m_HasLineVisual && !this.m_TeleportRay)
					{
						this.m_LineVisual.enabled = false;
					}
				}
				return false;
			}

			internal void RestoreVisuals()
			{
				if (this.m_RestoreVisuals && this.m_HasLineVisual && !this.fallback)
				{
					this.m_LineVisual.enabled = true;
				}
				this.m_RestoreVisuals = false;
			}

			[SerializeField]
			[RequireInterface(typeof(IXRRayProvider))]
			[Tooltip("The interactor that can fall back to gaze data.")]
			private Object m_Interactor;

			[SerializeField]
			[Tooltip("Changes mediation behavior to account for teleportation controls.")]
			private bool m_TeleportRay;

			private bool m_Initialized;

			private IXRRayProvider m_RayProvider;

			private IXRSelectInteractor m_SelectInteractor;

			private bool m_RestoreVisuals;

			private XRInteractorLineVisual m_LineVisual;

			private bool m_HasLineVisual;

			private Transform m_OriginalRayOrigin;

			private Transform m_OriginalAttach;

			private Transform m_OriginalVisualLineOrigin;

			private bool m_OriginalOverrideVisualLineOrigin;

			private Transform m_FallbackRayOrigin;

			private Transform m_FallbackAttach;

			private Transform m_FallbackVisualLineOrigin;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetAssistedVelocityInternal_0000101E$PostfixBurstDelegate(in Vector3 source, in Vector3 target, in Vector3 velocity, float gravity, float maxAngle, float requiredSpeed, float maxSpeedPercent, float assistPercent, float epsilon, out Vector3 adjustedVelocity);

		internal static class GetAssistedVelocityInternal_0000101E$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGazeAssistance.GetAssistedVelocityInternal_0000101E$BurstDirectCall.Pointer == 0)
				{
					XRGazeAssistance.GetAssistedVelocityInternal_0000101E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGazeAssistance.GetAssistedVelocityInternal_0000101E$PostfixBurstDelegate>(new XRGazeAssistance.GetAssistedVelocityInternal_0000101E$PostfixBurstDelegate(XRGazeAssistance.GetAssistedVelocityInternal)).Value;
				}
				A_0 = XRGazeAssistance.GetAssistedVelocityInternal_0000101E$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGazeAssistance.GetAssistedVelocityInternal_0000101E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 source, in Vector3 target, in Vector3 velocity, float gravity, float maxAngle, float requiredSpeed, float maxSpeedPercent, float assistPercent, float epsilon, out Vector3 adjustedVelocity)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGazeAssistance.GetAssistedVelocityInternal_0000101E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,UnityEngine.Vector3&), ref source, ref target, ref velocity, gravity, maxAngle, requiredSpeed, maxSpeedPercent, assistPercent, epsilon, ref adjustedVelocity, functionPointer);
						return;
					}
				}
				XRGazeAssistance.GetAssistedVelocityInternal$BurstManaged(source, target, velocity, gravity, maxAngle, requiredSpeed, maxSpeedPercent, assistPercent, epsilon, out adjustedVelocity);
			}

			private static IntPtr Pointer;
		}
	}
}
