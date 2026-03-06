using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	[BurstCompile]
	internal static class LODGroupDataPoolBurst
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(LODGroupDataPoolBurst.FreeLODGroupData_000002F1$PostfixBurstDelegate))]
		public static int FreeLODGroupData(in NativeArray<int> destroyedLODGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles)
		{
			return LODGroupDataPoolBurst.FreeLODGroupData_000002F1$BurstDirectCall.Invoke(destroyedLODGroupsID, ref lodGroupsData, ref lodGroupDataHash, ref freeLODGroupDataHandles);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$PostfixBurstDelegate))]
		public static int AllocateOrGetLODGroupDataInstances(in NativeArray<int> lodGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeList<LODGroupCullingData> lodGroupCullingData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles, ref NativeArray<GPUInstanceIndex> lodGroupInstances)
		{
			return LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall.Invoke(lodGroupsID, ref lodGroupsData, ref lodGroupCullingData, ref lodGroupDataHash, ref freeLODGroupDataHandles, ref lodGroupInstances);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int FreeLODGroupData$BurstManaged(in NativeArray<int> destroyedLODGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles)
		{
			int num = 0;
			NativeArray<int> nativeArray = destroyedLODGroupsID;
			foreach (int key in nativeArray)
			{
				GPUInstanceIndex gpuinstanceIndex;
				if (lodGroupDataHash.TryGetValue(key, out gpuinstanceIndex))
				{
					lodGroupDataHash.Remove(key);
					freeLODGroupDataHandles.Add(gpuinstanceIndex);
					ref LODGroupData ptr = ref lodGroupsData.ElementAt(gpuinstanceIndex.index);
					num += ptr.rendererCount;
					ptr.valid = false;
				}
			}
			return num;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int AllocateOrGetLODGroupDataInstances$BurstManaged(in NativeArray<int> lodGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeList<LODGroupCullingData> lodGroupCullingData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles, ref NativeArray<GPUInstanceIndex> lodGroupInstances)
		{
			int num = freeLODGroupDataHandles.Length;
			int length = lodGroupsData.Length;
			int num2 = 0;
			int num3 = 0;
			for (;;)
			{
				int num4 = num3;
				NativeArray<int> nativeArray = lodGroupsID;
				if (num4 >= nativeArray.Length)
				{
					break;
				}
				nativeArray = lodGroupsID;
				int key = nativeArray[num3];
				GPUInstanceIndex gpuinstanceIndex;
				if (!lodGroupDataHash.TryGetValue(key, out gpuinstanceIndex))
				{
					if (num == 0)
					{
						gpuinstanceIndex = new GPUInstanceIndex
						{
							index = length++
						};
					}
					else
					{
						gpuinstanceIndex = freeLODGroupDataHandles[--num];
					}
					lodGroupDataHash.TryAdd(key, gpuinstanceIndex);
				}
				else
				{
					num2 += lodGroupsData.ElementAt(gpuinstanceIndex.index).rendererCount;
				}
				lodGroupInstances[num3] = gpuinstanceIndex;
				num3++;
			}
			freeLODGroupDataHandles.ResizeUninitialized(num);
			lodGroupsData.ResizeUninitialized(length);
			lodGroupCullingData.ResizeUninitialized(length);
			return num2;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int FreeLODGroupData_000002F1$PostfixBurstDelegate(in NativeArray<int> destroyedLODGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles);

		internal static class FreeLODGroupData_000002F1$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (LODGroupDataPoolBurst.FreeLODGroupData_000002F1$BurstDirectCall.Pointer == 0)
				{
					LODGroupDataPoolBurst.FreeLODGroupData_000002F1$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<LODGroupDataPoolBurst.FreeLODGroupData_000002F1$PostfixBurstDelegate>(new LODGroupDataPoolBurst.FreeLODGroupData_000002F1$PostfixBurstDelegate(LODGroupDataPoolBurst.FreeLODGroupData)).Value;
				}
				A_0 = LODGroupDataPoolBurst.FreeLODGroupData_000002F1$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				LODGroupDataPoolBurst.FreeLODGroupData_000002F1$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static int Invoke(in NativeArray<int> destroyedLODGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = LODGroupDataPoolBurst.FreeLODGroupData_000002F1$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.LODGroupData>&,Unity.Collections.NativeParallelHashMap`2<System.Int32,UnityEngine.Rendering.GPUInstanceIndex>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.GPUInstanceIndex>&), ref destroyedLODGroupsID, ref lodGroupsData, ref lodGroupDataHash, ref freeLODGroupDataHandles, functionPointer);
					}
				}
				return LODGroupDataPoolBurst.FreeLODGroupData$BurstManaged(destroyedLODGroupsID, ref lodGroupsData, ref lodGroupDataHash, ref freeLODGroupDataHandles);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int AllocateOrGetLODGroupDataInstances_000002F2$PostfixBurstDelegate(in NativeArray<int> lodGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeList<LODGroupCullingData> lodGroupCullingData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles, ref NativeArray<GPUInstanceIndex> lodGroupInstances);

		internal static class AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall.Pointer == 0)
				{
					LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$PostfixBurstDelegate>(new LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$PostfixBurstDelegate(LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances)).Value;
				}
				A_0 = LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static int Invoke(in NativeArray<int> lodGroupsID, ref NativeList<LODGroupData> lodGroupsData, ref NativeList<LODGroupCullingData> lodGroupCullingData, ref NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash, ref NativeList<GPUInstanceIndex> freeLODGroupDataHandles, ref NativeArray<GPUInstanceIndex> lodGroupInstances)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances_000002F2$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.LODGroupData>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.LODGroupCullingData>&,Unity.Collections.NativeParallelHashMap`2<System.Int32,UnityEngine.Rendering.GPUInstanceIndex>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.GPUInstanceIndex>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.GPUInstanceIndex>&), ref lodGroupsID, ref lodGroupsData, ref lodGroupCullingData, ref lodGroupDataHash, ref freeLODGroupDataHandles, ref lodGroupInstances, functionPointer);
					}
				}
				return LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances$BurstManaged(lodGroupsID, ref lodGroupsData, ref lodGroupCullingData, ref lodGroupDataHash, ref freeLODGroupDataHandles, ref lodGroupInstances);
			}

			private static IntPtr Pointer;
		}
	}
}
