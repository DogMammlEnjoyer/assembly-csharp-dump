using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	[BurstCompile]
	internal static class InstanceDataSystemBurst
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(InstanceDataSystemBurst.ReallocateInstances_000002A0$PostfixBurstDelegate))]
		public static void ReallocateInstances(bool implicitInstanceIndices, in NativeArray<int> rendererGroupIDs, in NativeArray<GPUDrivenPackedRendererData> packedRendererData, in NativeArray<int> instanceOffsets, in NativeArray<int> instanceCounts, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeArray<InstanceHandle> instances, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
		{
			InstanceDataSystemBurst.ReallocateInstances_000002A0$BurstDirectCall.Invoke(implicitInstanceIndices, rendererGroupIDs, packedRendererData, instanceOffsets, instanceCounts, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref instances, ref rendererGroupInstanceMultiHash);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$PostfixBurstDelegate))]
		public static void FreeRendererGroupInstances(in NativeArray<int>.ReadOnly rendererGroupsID, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
		{
			InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$BurstDirectCall.Invoke(rendererGroupsID, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref rendererGroupInstanceMultiHash);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(InstanceDataSystemBurst.FreeInstances_000002A2$PostfixBurstDelegate))]
		public static void FreeInstances(in NativeArray<InstanceHandle>.ReadOnly instances, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
		{
			InstanceDataSystemBurst.FreeInstances_000002A2$BurstDirectCall.Invoke(instances, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref rendererGroupInstanceMultiHash);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ReallocateInstances$BurstManaged(bool implicitInstanceIndices, in NativeArray<int> rendererGroupIDs, in NativeArray<GPUDrivenPackedRendererData> packedRendererData, in NativeArray<int> instanceOffsets, in NativeArray<int> instanceCounts, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeArray<InstanceHandle> instances, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
		{
			int num = 0;
			for (;;)
			{
				int num2 = num;
				NativeArray<int> nativeArray = rendererGroupIDs;
				if (num2 >= nativeArray.Length)
				{
					break;
				}
				nativeArray = rendererGroupIDs;
				int key = nativeArray[num];
				NativeArray<GPUDrivenPackedRendererData> nativeArray2 = packedRendererData;
				bool hasTree = nativeArray2[num].hasTree;
				int num3;
				int num4;
				if (implicitInstanceIndices)
				{
					num3 = 1;
					num4 = num;
				}
				else
				{
					nativeArray = instanceCounts;
					num3 = nativeArray[num];
					nativeArray = instanceOffsets;
					num4 = nativeArray[num];
				}
				InstanceHandle instance;
				NativeParallelMultiHashMapIterator<int> it;
				SharedInstanceHandle sharedInstanceHandle;
				if (rendererGroupInstanceMultiHash.TryGetFirstValue(key, out instance, out it))
				{
					sharedInstanceHandle = instanceData.Get_SharedInstance(instance);
					if (sharedInstanceData.Get_RefCount(sharedInstanceHandle) - num3 > 0)
					{
						bool flag = true;
						int num5 = 0;
						for (int i = 0; i < num3; i++)
						{
							flag = rendererGroupInstanceMultiHash.TryGetNextValue(out instance, ref it);
						}
						while (flag)
						{
							int index = instanceData.InstanceToIndex(instance);
							instanceData.Remove(instance);
							perCameraInstanceData.Remove(index);
							instanceAllocators.FreeInstance(instance);
							rendererGroupInstanceMultiHash.Remove(it);
							num5++;
							flag = rendererGroupInstanceMultiHash.TryGetNextValue(out instance, ref it);
						}
					}
				}
				else
				{
					sharedInstanceHandle = instanceAllocators.AllocateSharedInstance();
					sharedInstanceData.AddNoGrow(sharedInstanceHandle);
				}
				if (num3 > 0)
				{
					sharedInstanceData.Set_RefCount(sharedInstanceHandle, num3);
					for (int j = 0; j < num3; j++)
					{
						int index2 = num4 + j;
						if (!instances[index2].valid)
						{
							InstanceHandle instanceHandle;
							if (!hasTree)
							{
								instanceHandle = instanceAllocators.AllocateInstance(InstanceType.MeshRenderer);
							}
							else
							{
								instanceHandle = instanceAllocators.AllocateInstance(InstanceType.SpeedTree);
							}
							instanceData.AddNoGrow(instanceHandle);
							perCameraInstanceData.IncreaseInstanceCount();
							int index3 = instanceData.InstanceToIndex(instanceHandle);
							instanceData.sharedInstances[index3] = sharedInstanceHandle;
							instanceData.movedInCurrentFrameBits.Set(index3, false);
							instanceData.movedInPreviousFrameBits.Set(index3, false);
							instanceData.visibleInPreviousFrameBits.Set(index3, false);
							rendererGroupInstanceMultiHash.Add(key, instanceHandle);
							instances[index2] = instanceHandle;
						}
					}
				}
				else
				{
					sharedInstanceData.Remove(sharedInstanceHandle);
					instanceAllocators.FreeSharedInstance(sharedInstanceHandle);
				}
				num++;
			}
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FreeRendererGroupInstances$BurstManaged(in NativeArray<int>.ReadOnly rendererGroupsID, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
		{
			NativeArray<int>.ReadOnly readOnly = rendererGroupsID;
			foreach (int key in readOnly)
			{
				InstanceHandle instance;
				NativeParallelMultiHashMapIterator<int> nativeParallelMultiHashMapIterator;
				bool flag = rendererGroupInstanceMultiHash.TryGetFirstValue(key, out instance, out nativeParallelMultiHashMapIterator);
				while (flag)
				{
					SharedInstanceHandle instance2 = instanceData.Get_SharedInstance(instance);
					int index = sharedInstanceData.SharedInstanceToIndex(instance2);
					int num = sharedInstanceData.refCounts[index];
					if (num > 1)
					{
						sharedInstanceData.refCounts[index] = num - 1;
					}
					else
					{
						sharedInstanceData.Remove(instance2);
						instanceAllocators.FreeSharedInstance(instance2);
					}
					int index2 = instanceData.InstanceToIndex(instance);
					instanceData.Remove(instance);
					perCameraInstanceData.Remove(index2);
					instanceAllocators.FreeInstance(instance);
					flag = rendererGroupInstanceMultiHash.TryGetNextValue(out instance, ref nativeParallelMultiHashMapIterator);
				}
				rendererGroupInstanceMultiHash.Remove(key);
			}
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FreeInstances$BurstManaged(in NativeArray<InstanceHandle>.ReadOnly instances, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
		{
			NativeArray<InstanceHandle>.ReadOnly readOnly = instances;
			foreach (InstanceHandle instance in readOnly)
			{
				if (instanceData.IsValidInstance(instance))
				{
					int index = instanceData.InstanceToIndex(instance);
					SharedInstanceHandle instance2 = instanceData.sharedInstances[index];
					int index2 = sharedInstanceData.SharedInstanceToIndex(instance2);
					int num = sharedInstanceData.refCounts[index2];
					int key = sharedInstanceData.rendererGroupIDs[index2];
					if (num > 1)
					{
						sharedInstanceData.refCounts[index2] = num - 1;
					}
					else
					{
						sharedInstanceData.Remove(instance2);
						instanceAllocators.FreeSharedInstance(instance2);
					}
					int index3 = instanceData.InstanceToIndex(instance);
					instanceData.Remove(instance);
					perCameraInstanceData.Remove(index3);
					instanceAllocators.FreeInstance(instance);
					InstanceHandle other;
					NativeParallelMultiHashMapIterator<int> it;
					bool flag = rendererGroupInstanceMultiHash.TryGetFirstValue(key, out other, out it);
					while (flag)
					{
						if (instance.Equals(other))
						{
							rendererGroupInstanceMultiHash.Remove(it);
							break;
						}
						flag = rendererGroupInstanceMultiHash.TryGetNextValue(out other, ref it);
					}
				}
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ReallocateInstances_000002A0$PostfixBurstDelegate(bool implicitInstanceIndices, in NativeArray<int> rendererGroupIDs, in NativeArray<GPUDrivenPackedRendererData> packedRendererData, in NativeArray<int> instanceOffsets, in NativeArray<int> instanceCounts, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeArray<InstanceHandle> instances, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash);

		internal static class ReallocateInstances_000002A0$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InstanceDataSystemBurst.ReallocateInstances_000002A0$BurstDirectCall.Pointer == 0)
				{
					InstanceDataSystemBurst.ReallocateInstances_000002A0$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InstanceDataSystemBurst.ReallocateInstances_000002A0$PostfixBurstDelegate>(new InstanceDataSystemBurst.ReallocateInstances_000002A0$PostfixBurstDelegate(InstanceDataSystemBurst.ReallocateInstances)).Value;
				}
				A_0 = InstanceDataSystemBurst.ReallocateInstances_000002A0$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InstanceDataSystemBurst.ReallocateInstances_000002A0$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(bool implicitInstanceIndices, in NativeArray<int> rendererGroupIDs, in NativeArray<GPUDrivenPackedRendererData> packedRendererData, in NativeArray<int> instanceOffsets, in NativeArray<int> instanceCounts, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeArray<InstanceHandle> instances, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InstanceDataSystemBurst.ReallocateInstances_000002A0$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Boolean,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.GPUDrivenPackedRendererData>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<System.Int32>&,UnityEngine.Rendering.InstanceAllocators&,UnityEngine.Rendering.CPUInstanceData&,UnityEngine.Rendering.CPUPerCameraInstanceData&,UnityEngine.Rendering.CPUSharedInstanceData&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.InstanceHandle>&,Unity.Collections.NativeParallelMultiHashMap`2<System.Int32,UnityEngine.Rendering.InstanceHandle>&), implicitInstanceIndices, ref rendererGroupIDs, ref packedRendererData, ref instanceOffsets, ref instanceCounts, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref instances, ref rendererGroupInstanceMultiHash, functionPointer);
						return;
					}
				}
				InstanceDataSystemBurst.ReallocateInstances$BurstManaged(implicitInstanceIndices, rendererGroupIDs, packedRendererData, instanceOffsets, instanceCounts, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref instances, ref rendererGroupInstanceMultiHash);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void FreeRendererGroupInstances_000002A1$PostfixBurstDelegate(in NativeArray<int>.ReadOnly rendererGroupsID, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash);

		internal static class FreeRendererGroupInstances_000002A1$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$BurstDirectCall.Pointer == 0)
				{
					InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$PostfixBurstDelegate>(new InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$PostfixBurstDelegate(InstanceDataSystemBurst.FreeRendererGroupInstances)).Value;
				}
				A_0 = InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in NativeArray<int>.ReadOnly rendererGroupsID, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InstanceDataSystemBurst.FreeRendererGroupInstances_000002A1$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1/ReadOnly<System.Int32>&,UnityEngine.Rendering.InstanceAllocators&,UnityEngine.Rendering.CPUInstanceData&,UnityEngine.Rendering.CPUPerCameraInstanceData&,UnityEngine.Rendering.CPUSharedInstanceData&,Unity.Collections.NativeParallelMultiHashMap`2<System.Int32,UnityEngine.Rendering.InstanceHandle>&), ref rendererGroupsID, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref rendererGroupInstanceMultiHash, functionPointer);
						return;
					}
				}
				InstanceDataSystemBurst.FreeRendererGroupInstances$BurstManaged(rendererGroupsID, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref rendererGroupInstanceMultiHash);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void FreeInstances_000002A2$PostfixBurstDelegate(in NativeArray<InstanceHandle>.ReadOnly instances, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash);

		internal static class FreeInstances_000002A2$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InstanceDataSystemBurst.FreeInstances_000002A2$BurstDirectCall.Pointer == 0)
				{
					InstanceDataSystemBurst.FreeInstances_000002A2$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InstanceDataSystemBurst.FreeInstances_000002A2$PostfixBurstDelegate>(new InstanceDataSystemBurst.FreeInstances_000002A2$PostfixBurstDelegate(InstanceDataSystemBurst.FreeInstances)).Value;
				}
				A_0 = InstanceDataSystemBurst.FreeInstances_000002A2$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InstanceDataSystemBurst.FreeInstances_000002A2$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in NativeArray<InstanceHandle>.ReadOnly instances, ref InstanceAllocators instanceAllocators, ref CPUInstanceData instanceData, ref CPUPerCameraInstanceData perCameraInstanceData, ref CPUSharedInstanceData sharedInstanceData, ref NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InstanceDataSystemBurst.FreeInstances_000002A2$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1/ReadOnly<UnityEngine.Rendering.InstanceHandle>&,UnityEngine.Rendering.InstanceAllocators&,UnityEngine.Rendering.CPUInstanceData&,UnityEngine.Rendering.CPUPerCameraInstanceData&,UnityEngine.Rendering.CPUSharedInstanceData&,Unity.Collections.NativeParallelMultiHashMap`2<System.Int32,UnityEngine.Rendering.InstanceHandle>&), ref instances, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref rendererGroupInstanceMultiHash, functionPointer);
						return;
					}
				}
				InstanceDataSystemBurst.FreeInstances$BurstManaged(instances, ref instanceAllocators, ref instanceData, ref perCameraInstanceData, ref sharedInstanceData, ref rendererGroupInstanceMultiHash);
			}

			private static IntPtr Pointer;
		}
	}
}
