using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.Interaction.Toolkit.Attachment;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[BurstCompile]
	internal class XRPokeLogic : IDisposable
	{
		private float interactionAxisLength { get; set; } = 1f;

		public IReadOnlyBindableVariable<PokeStateData> pokeStateData
		{
			get
			{
				return this.m_PokeStateData;
			}
		}

		public void Initialize(Transform associatedTransform, PokeThresholdData pokeThresholdData, Collider collider)
		{
			this.m_InitialTransform = associatedTransform;
			this.m_PokeThresholdData = pokeThresholdData;
			this.m_SelectEntranceVectorDotThreshold = pokeThresholdData.GetSelectEntranceVectorDotThreshold();
			if (collider != null)
			{
				this.interactionAxisLength = this.ComputeInteractionAxisLength(XRPokeLogic.ComputeBounds(collider, false, Space.World));
			}
			this.ResetPokeStateData(this.m_InitialTransform);
		}

		public void SetPokeDepth(float pokeDepth)
		{
			this.interactionAxisLength = pokeDepth;
		}

		public void Dispose()
		{
		}

		public bool MeetsRequirementsForSelectAction(object interactor, Vector3 pokableAttachPosition, Vector3 pokerAttachPosition, float pokeInteractionOffset, Transform pokedTransform)
		{
			if (!this.IsPokeDataValid(pokedTransform))
			{
				return false;
			}
			Vector3 vector = this.ComputeRotatedDepthEvaluationAxis(pokedTransform, true);
			float3 @float = this.CalculateInteractionPoint(pokerAttachPosition, vector, pokeInteractionOffset);
			float3 float2 = pokableAttachPosition;
			float3 float3 = vector;
			float interactionDepth;
			float num;
			XRPokeLogic.CalculatePokeParams(@float, float2, float3, out interactionDepth, out num);
			bool isOverObject = num > 0f;
			float value = this.CalculateDepthPercent(interactionDepth, num, this.interactionAxisLength);
			bool flag = this.CalculateHoverRequirements(interactor, isOverObject, vector);
			float clampedDepthPercent = (!flag) ? 1f : Mathf.Clamp01(value);
			bool flag2 = this.CalculateRequirements(ref flag, clampedDepthPercent, interactor);
			this.UpdatePokeStateData(flag2, flag, clampedDepthPercent, interactor, pokerAttachPosition, pokableAttachPosition, vector, pokedTransform);
			return flag2;
		}

		private bool IsPokeDataValid(Transform pokedTransform)
		{
			return this.m_PokeThresholdData != null && pokedTransform != null;
		}

		private Vector3 CalculateInteractionPoint(Vector3 pokerAttachPosition, Vector3 axisNormal, float pokeInteractionOffset)
		{
			float combinedPokeOffset = pokeInteractionOffset + this.m_PokeThresholdData.interactionDepthOffset;
			float3 @float = pokerAttachPosition;
			float3 float2 = axisNormal;
			float3 v;
			XRPokeLogic.CalculateInteractionPoint(@float, float2, combinedPokeOffset, out v);
			return v;
		}

		private bool CalculateHoverRequirements(object interactor, bool isOverObject, float3 axisNormal)
		{
			if (!this.m_PokeThresholdData.enablePokeAngleThreshold)
			{
				return true;
			}
			bool result = true;
			bool flag;
			if (!this.m_HoldingHoverCheck.TryGetValue(interactor, out flag) || !flag)
			{
				result = this.CheckVelocity(interactor, isOverObject, axisNormal);
			}
			return result;
		}

		private bool CheckVelocity(object interactor, bool isOverObject, Vector3 axisNormal)
		{
			if (!isOverObject)
			{
				return false;
			}
			IAttachPointVelocityProvider attachPointVelocityProvider = interactor as IAttachPointVelocityProvider;
			if (attachPointVelocityProvider == null)
			{
				return true;
			}
			Vector3 attachPointVelocity = attachPointVelocityProvider.GetAttachPointVelocity();
			float3 @float = attachPointVelocity;
			return XRPokeLogic.IsVelocitySufficient(@float, 0.0001f) && Vector3.Dot(-attachPointVelocity.normalized, axisNormal) > this.m_SelectEntranceVectorDotThreshold;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRPokeLogic.CalculatePokeParams_00001082$PostfixBurstDelegate))]
		private static void CalculatePokeParams(in float3 interactionPoint, in float3 pokableAttachPosition, in float3 axisNormal, out float interactionDepth, out float entranceVectorDot)
		{
			XRPokeLogic.CalculatePokeParams_00001082$BurstDirectCall.Invoke(interactionPoint, pokableAttachPosition, axisNormal, out interactionDepth, out entranceVectorDot);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRPokeLogic.CalculateInteractionPoint_00001083$PostfixBurstDelegate))]
		private static void CalculateInteractionPoint(in float3 pokerAttachPosition, in float3 axisNormal, float combinedPokeOffset, out float3 interactionPoint)
		{
			XRPokeLogic.CalculateInteractionPoint_00001083$BurstDirectCall.Invoke(pokerAttachPosition, axisNormal, combinedPokeOffset, out interactionPoint);
		}

		[BurstCompile]
		private float CalculateDepthPercent(float interactionDepth, float entranceVectorDot, float axisLength)
		{
			return math.sign(entranceVectorDot) * interactionDepth / axisLength;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRPokeLogic.IsVelocitySufficient_00001085$PostfixBurstDelegate))]
		private static bool IsVelocitySufficient(in float3 velocity, float threshold)
		{
			return XRPokeLogic.IsVelocitySufficient_00001085$BurstDirectCall.Invoke(velocity, threshold);
		}

		private bool CalculateRequirements(ref bool meetsHoverRequirements, float clampedDepthPercent, object interactor)
		{
			bool flag = meetsHoverRequirements && clampedDepthPercent < 0.025f;
			bool flag2;
			if (this.m_LastRequirementsMet.TryGetValue(interactor, out flag2) && flag2 && !flag)
			{
				meetsHoverRequirements = false;
			}
			return flag;
		}

		private void UpdatePokeStateData(bool meetsRequirements, bool meetsHoverRequirements, float clampedDepthPercent, object interactor, Vector3 pokerAttachPosition, Vector3 pokableAttachPosition, Vector3 axisNormal, Transform pokedTransform)
		{
			this.m_HoldingHoverCheck[interactor] = meetsHoverRequirements;
			this.m_LastRequirementsMet[interactor] = meetsRequirements;
			this.m_LastInteractorPressDepth[interactor] = clampedDepthPercent;
			HashSetList<object> hashSetList;
			if (!meetsRequirements && this.m_HoveredInteractorsOnThisTransform.TryGetValue(pokedTransform, out hashSetList))
			{
				int count = hashSetList.Count;
				if (count > 1)
				{
					IReadOnlyList<object> readOnlyList = hashSetList.AsList();
					for (int i = 0; i < count; i++)
					{
						object obj = readOnlyList[i];
						if (obj != interactor && this.m_LastInteractorPressDepth[obj] < clampedDepthPercent)
						{
							return;
						}
					}
				}
			}
			float num = (clampedDepthPercent < 1f && !meetsRequirements) ? this.m_PokeThresholdData.interactionDepthOffset : 0f;
			float d = Mathf.Clamp(clampedDepthPercent * this.interactionAxisLength + num, 0f, this.interactionAxisLength);
			this.m_PokeStateData.Value = new PokeStateData
			{
				meetsRequirements = meetsRequirements,
				pokeInteractionPoint = pokerAttachPosition,
				axisAlignedPokeInteractionPoint = pokableAttachPosition + d * axisNormal,
				interactionStrength = 1f - clampedDepthPercent,
				axisNormal = axisNormal,
				target = pokedTransform
			};
		}

		private Vector3 ComputeRotatedDepthEvaluationAxis(Transform associatedTransform, bool isWorldSpace = true)
		{
			if (this.m_PokeThresholdData == null || associatedTransform == null)
			{
				return Vector3.zero;
			}
			Vector3 vector = Vector3.zero;
			switch (this.m_PokeThresholdData.pokeDirection)
			{
			case PokeAxis.X:
			case PokeAxis.NegativeX:
				vector = (isWorldSpace ? associatedTransform.right : Vector3.right);
				break;
			case PokeAxis.Y:
			case PokeAxis.NegativeY:
				vector = (isWorldSpace ? associatedTransform.up : Vector3.up);
				break;
			case PokeAxis.Z:
			case PokeAxis.NegativeZ:
				vector = (isWorldSpace ? associatedTransform.forward : Vector3.forward);
				break;
			}
			PokeAxis pokeDirection = this.m_PokeThresholdData.pokeDirection;
			if (pokeDirection - PokeAxis.X <= 2)
			{
				vector = -vector;
			}
			return vector;
		}

		private float ComputeInteractionAxisLength(Bounds bounds)
		{
			if (this.m_PokeThresholdData == null || this.m_InitialTransform == null)
			{
				return 0f;
			}
			Vector3 size = bounds.size;
			Vector3 position = this.m_InitialTransform.position;
			float result = 0f;
			switch (this.m_PokeThresholdData.pokeDirection)
			{
			case PokeAxis.X:
			case PokeAxis.NegativeX:
			{
				float num = bounds.center.x - position.x;
				result = size.x / 2f + num;
				break;
			}
			case PokeAxis.Y:
			case PokeAxis.NegativeY:
			{
				float num = bounds.center.y - position.y;
				result = size.y / 2f + num;
				break;
			}
			case PokeAxis.Z:
			case PokeAxis.NegativeZ:
			{
				float num = bounds.center.z - position.z;
				result = size.z / 2f + num;
				break;
			}
			}
			return result;
		}

		public void OnHoverEntered(object interactor, Pose updatedPose, Transform pokedTransform)
		{
			this.m_LastHoveredTransform[interactor] = pokedTransform;
			this.m_LastInteractorPressDepth[interactor] = 1f;
			this.m_HoldingHoverCheck[interactor] = false;
			this.m_LastRequirementsMet[interactor] = false;
			HashSetList<object> hashSetList;
			if (!this.m_HoveredInteractorsOnThisTransform.TryGetValue(pokedTransform, out hashSetList))
			{
				hashSetList = new HashSetList<object>(0);
				this.m_HoveredInteractorsOnThisTransform[pokedTransform] = hashSetList;
			}
			hashSetList.Add(interactor);
		}

		public void OnHoverExited(object interactor)
		{
			this.m_HoldingHoverCheck[interactor] = false;
			this.m_LastInteractorPressDepth[interactor] = 1f;
			this.m_LastRequirementsMet[interactor] = false;
			Transform transform;
			if (this.m_LastHoveredTransform.TryGetValue(interactor, out transform))
			{
				HashSetList<object> hashSetList;
				if (this.m_HoveredInteractorsOnThisTransform.TryGetValue(transform, out hashSetList))
				{
					hashSetList.Remove(interactor);
				}
				this.ResetPokeStateData(transform);
				this.m_LastHoveredTransform.Remove(interactor);
				return;
			}
			if (this.m_LastHoveredTransform.Count == 0)
			{
				this.ResetPokeStateData(this.m_InitialTransform);
			}
		}

		private void ResetPokeStateData(Transform transform)
		{
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Vector3 a = this.ComputeRotatedDepthEvaluationAxis(transform, true);
			Vector3 vector = position + a * this.interactionAxisLength;
			this.m_PokeStateData.Value = new PokeStateData
			{
				meetsRequirements = false,
				pokeInteractionPoint = vector,
				axisAlignedPokeInteractionPoint = vector,
				interactionStrength = 0f,
				axisNormal = Vector3.zero,
				target = null
			};
		}

		private static Bounds ComputeBounds(Collider targetCollider, bool rotateBoundsScale = false, Space targetSpace = Space.World)
		{
			Bounds bounds = default(Bounds);
			BoxCollider boxCollider = targetCollider as BoxCollider;
			if (boxCollider != null)
			{
				bounds = new Bounds(boxCollider.center, boxCollider.size);
			}
			else
			{
				SphereCollider sphereCollider = targetCollider as SphereCollider;
				if (sphereCollider != null)
				{
					bounds = new Bounds(sphereCollider.center, Vector3.one * (sphereCollider.radius * 2f));
				}
				else
				{
					CapsuleCollider capsuleCollider = targetCollider as CapsuleCollider;
					if (capsuleCollider != null)
					{
						Vector3 zero = Vector3.zero;
						float num = capsuleCollider.radius * 2f;
						float height = capsuleCollider.height;
						switch (capsuleCollider.direction)
						{
						case 0:
							zero = new Vector3(height, num, num);
							break;
						case 1:
							zero = new Vector3(num, height, num);
							break;
						case 2:
							zero = new Vector3(num, num, height);
							break;
						}
						bounds = new Bounds(capsuleCollider.center, zero);
					}
				}
			}
			if (targetSpace == Space.Self)
			{
				return bounds;
			}
			return XRPokeLogic.BoundsLocalToWorld(bounds, targetCollider.transform, rotateBoundsScale);
		}

		private static Bounds BoundsLocalToWorld(Bounds targetBounds, Transform targetTransform, bool rotateBoundsScale = false)
		{
			Vector3 lossyScale = targetTransform.lossyScale;
			Vector3 vector = lossyScale.Multiply(targetBounds.size);
			Vector3 size = rotateBoundsScale ? (targetTransform.rotation * vector) : vector;
			return new Bounds(targetTransform.position + lossyScale.Multiply(targetBounds.center), size);
		}

		public void DrawGizmos()
		{
			if (this.m_PokeThresholdData == null || this.m_InitialTransform == null)
			{
				return;
			}
			Vector3 position = this.m_InitialTransform.position;
			Vector3 a = this.ComputeRotatedDepthEvaluationAxis(this.m_InitialTransform, true);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position, position + a * this.interactionAxisLength);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(position, position + a * this.m_PokeThresholdData.interactionDepthOffset);
			if (this.m_PokeStateData != null && this.m_PokeStateData.Value.interactionStrength > 0f)
			{
				Gizmos.color = (this.m_PokeStateData.Value.meetsRequirements ? Color.green : Color.yellow);
				Gizmos.DrawWireSphere(this.m_PokeStateData.Value.pokeInteractionPoint, 0.01f);
				Gizmos.DrawWireSphere(this.m_PokeStateData.Value.axisAlignedPokeInteractionPoint, 0.01f);
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CalculatePokeParams$BurstManaged(in float3 interactionPoint, in float3 pokableAttachPosition, in float3 axisNormal, out float interactionDepth, out float entranceVectorDot)
		{
			float3 @float = interactionPoint - pokableAttachPosition;
			float3 x = math.project(@float, axisNormal);
			interactionDepth = math.length(x);
			entranceVectorDot = math.dot(axisNormal, math.normalizesafe(@float, default(float3)));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CalculateInteractionPoint$BurstManaged(in float3 pokerAttachPosition, in float3 axisNormal, float combinedPokeOffset, out float3 interactionPoint)
		{
			float3 rhs = axisNormal * combinedPokeOffset;
			interactionPoint = pokerAttachPosition - rhs;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsVelocitySufficient$BurstManaged(in float3 velocity, float threshold)
		{
			return math.lengthsq(velocity) > threshold;
		}

		private readonly BindableVariable<PokeStateData> m_PokeStateData = new BindableVariable<PokeStateData>(default(PokeStateData), true, null, false);

		private Transform m_InitialTransform;

		private PokeThresholdData m_PokeThresholdData;

		private float m_SelectEntranceVectorDotThreshold;

		private readonly Dictionary<object, Transform> m_LastHoveredTransform = new Dictionary<object, Transform>();

		private readonly Dictionary<object, bool> m_HoldingHoverCheck = new Dictionary<object, bool>();

		private readonly Dictionary<Transform, HashSetList<object>> m_HoveredInteractorsOnThisTransform = new Dictionary<Transform, HashSetList<object>>();

		private readonly Dictionary<object, float> m_LastInteractorPressDepth = new Dictionary<object, float>();

		private readonly Dictionary<object, bool> m_LastRequirementsMet = new Dictionary<object, bool>();

		private const float k_DepthPercentActivationThreshold = 0.025f;

		private const float k_SquareVelocityHoverThreshold = 0.0001f;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculatePokeParams_00001082$PostfixBurstDelegate(in float3 interactionPoint, in float3 pokableAttachPosition, in float3 axisNormal, out float interactionDepth, out float entranceVectorDot);

		internal static class CalculatePokeParams_00001082$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRPokeLogic.CalculatePokeParams_00001082$BurstDirectCall.Pointer == 0)
				{
					XRPokeLogic.CalculatePokeParams_00001082$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRPokeLogic.CalculatePokeParams_00001082$PostfixBurstDelegate>(new XRPokeLogic.CalculatePokeParams_00001082$PostfixBurstDelegate(XRPokeLogic.CalculatePokeParams)).Value;
				}
				A_0 = XRPokeLogic.CalculatePokeParams_00001082$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRPokeLogic.CalculatePokeParams_00001082$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 interactionPoint, in float3 pokableAttachPosition, in float3 axisNormal, out float interactionDepth, out float entranceVectorDot)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRPokeLogic.CalculatePokeParams_00001082$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single&,System.Single&), ref interactionPoint, ref pokableAttachPosition, ref axisNormal, ref interactionDepth, ref entranceVectorDot, functionPointer);
						return;
					}
				}
				XRPokeLogic.CalculatePokeParams$BurstManaged(interactionPoint, pokableAttachPosition, axisNormal, out interactionDepth, out entranceVectorDot);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateInteractionPoint_00001083$PostfixBurstDelegate(in float3 pokerAttachPosition, in float3 axisNormal, float combinedPokeOffset, out float3 interactionPoint);

		internal static class CalculateInteractionPoint_00001083$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRPokeLogic.CalculateInteractionPoint_00001083$BurstDirectCall.Pointer == 0)
				{
					XRPokeLogic.CalculateInteractionPoint_00001083$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRPokeLogic.CalculateInteractionPoint_00001083$PostfixBurstDelegate>(new XRPokeLogic.CalculateInteractionPoint_00001083$PostfixBurstDelegate(XRPokeLogic.CalculateInteractionPoint)).Value;
				}
				A_0 = XRPokeLogic.CalculateInteractionPoint_00001083$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRPokeLogic.CalculateInteractionPoint_00001083$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 pokerAttachPosition, in float3 axisNormal, float combinedPokeOffset, out float3 interactionPoint)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRPokeLogic.CalculateInteractionPoint_00001083$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&), ref pokerAttachPosition, ref axisNormal, combinedPokeOffset, ref interactionPoint, functionPointer);
						return;
					}
				}
				XRPokeLogic.CalculateInteractionPoint$BurstManaged(pokerAttachPosition, axisNormal, combinedPokeOffset, out interactionPoint);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool IsVelocitySufficient_00001085$PostfixBurstDelegate(in float3 velocity, float threshold);

		internal static class IsVelocitySufficient_00001085$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRPokeLogic.IsVelocitySufficient_00001085$BurstDirectCall.Pointer == 0)
				{
					XRPokeLogic.IsVelocitySufficient_00001085$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRPokeLogic.IsVelocitySufficient_00001085$PostfixBurstDelegate>(new XRPokeLogic.IsVelocitySufficient_00001085$PostfixBurstDelegate(XRPokeLogic.IsVelocitySufficient)).Value;
				}
				A_0 = XRPokeLogic.IsVelocitySufficient_00001085$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRPokeLogic.IsVelocitySufficient_00001085$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(in float3 velocity, float threshold)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRPokeLogic.IsVelocitySufficient_00001085$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(Unity.Mathematics.float3&,System.Single), ref velocity, threshold, functionPointer);
					}
				}
				return XRPokeLogic.IsVelocitySufficient$BurstManaged(velocity, threshold);
			}

			private static IntPtr Pointer;
		}
	}
}
