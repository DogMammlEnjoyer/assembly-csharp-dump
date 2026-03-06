using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	[BurstCompile]
	internal static class InstanceCullerBurst
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(InstanceCullerBurst.SetupCullingJobInput_0000014D$PostfixBurstDelegate))]
		public unsafe static void SetupCullingJobInput(float lodBias, float meshLodThreshold, BatchCullingContext* context, ReceiverPlanes* receiverPlanes, ReceiverSphereCuller* receiverSphereCuller, FrustumPlaneCuller* frustumPlaneCuller, float* screenRelativeMetric, float* meshLodConstant)
		{
			InstanceCullerBurst.SetupCullingJobInput_0000014D$BurstDirectCall.Invoke(lodBias, meshLodThreshold, context, receiverPlanes, receiverSphereCuller, frustumPlaneCuller, screenRelativeMetric, meshLodConstant);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void SetupCullingJobInput$BurstManaged(float lodBias, float meshLodThreshold, BatchCullingContext* context, ReceiverPlanes* receiverPlanes, ReceiverSphereCuller* receiverSphereCuller, FrustumPlaneCuller* frustumPlaneCuller, float* screenRelativeMetric, float* meshLodConstant)
		{
			*receiverPlanes = ReceiverPlanes.Create(*context, Allocator.TempJob);
			*receiverSphereCuller = ReceiverSphereCuller.Create(*context, Allocator.TempJob);
			*frustumPlaneCuller = FrustumPlaneCuller.Create(*context, receiverPlanes->planes.AsArray(), *receiverSphereCuller, Allocator.TempJob);
			*screenRelativeMetric = LODRenderingUtils.CalculateScreenRelativeMetricNoBias(context->lodParameters);
			*meshLodConstant = LODRenderingUtils.CalculateMeshLodConstant(context->lodParameters, *screenRelativeMetric, meshLodThreshold);
			*screenRelativeMetric /= lodBias;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate void SetupCullingJobInput_0000014D$PostfixBurstDelegate(float lodBias, float meshLodThreshold, BatchCullingContext* context, ReceiverPlanes* receiverPlanes, ReceiverSphereCuller* receiverSphereCuller, FrustumPlaneCuller* frustumPlaneCuller, float* screenRelativeMetric, float* meshLodConstant);

		internal static class SetupCullingJobInput_0000014D$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InstanceCullerBurst.SetupCullingJobInput_0000014D$BurstDirectCall.Pointer == 0)
				{
					InstanceCullerBurst.SetupCullingJobInput_0000014D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InstanceCullerBurst.SetupCullingJobInput_0000014D$PostfixBurstDelegate>(new InstanceCullerBurst.SetupCullingJobInput_0000014D$PostfixBurstDelegate(InstanceCullerBurst.SetupCullingJobInput)).Value;
				}
				A_0 = InstanceCullerBurst.SetupCullingJobInput_0000014D$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InstanceCullerBurst.SetupCullingJobInput_0000014D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public unsafe static void Invoke(float lodBias, float meshLodThreshold, BatchCullingContext* context, ReceiverPlanes* receiverPlanes, ReceiverSphereCuller* receiverSphereCuller, FrustumPlaneCuller* frustumPlaneCuller, float* screenRelativeMetric, float* meshLodConstant)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InstanceCullerBurst.SetupCullingJobInput_0000014D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Single,System.Single,UnityEngine.Rendering.BatchCullingContext*,UnityEngine.Rendering.ReceiverPlanes*,UnityEngine.Rendering.ReceiverSphereCuller*,UnityEngine.Rendering.FrustumPlaneCuller*,System.Single*,System.Single*), lodBias, meshLodThreshold, context, receiverPlanes, receiverSphereCuller, frustumPlaneCuller, screenRelativeMetric, meshLodConstant, functionPointer);
						return;
					}
				}
				InstanceCullerBurst.SetupCullingJobInput$BurstManaged(lodBias, meshLodThreshold, context, receiverPlanes, receiverSphereCuller, frustumPlaneCuller, screenRelativeMetric, meshLodConstant);
			}

			private static IntPtr Pointer;
		}
	}
}
