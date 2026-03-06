using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	[BurstCompile]
	internal static class GPUResidentDrawerBurst
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(GPUResidentDrawerBurst.ClassifyMaterials_000000EA$PostfixBurstDelegate))]
		public static void ClassifyMaterials(in NativeArray<int> materialIDs, in NativeParallelHashMap<int, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<int> supportedMaterialIDs, ref NativeList<int> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			GPUResidentDrawerBurst.ClassifyMaterials_000000EA$BurstDirectCall.Invoke(materialIDs, batchMaterialHash, ref supportedMaterialIDs, ref unsupportedMaterialIDs, ref supportedPackedMaterialDatas);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$PostfixBurstDelegate))]
		public static void FindUnsupportedRenderers(in NativeArray<int> unsupportedMaterials, in NativeArray<SmallIntegerArray>.ReadOnly materialIDArrays, in NativeArray<int>.ReadOnly rendererGroups, ref NativeList<int> unsupportedRenderers)
		{
			GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$BurstDirectCall.Invoke(unsupportedMaterials, materialIDArrays, rendererGroups, ref unsupportedRenderers);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$PostfixBurstDelegate))]
		public static void GetMaterialsWithChangedPackedMaterial(in NativeArray<int> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<int> filteredMaterials)
		{
			GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall.Invoke(materialIDs, packedMaterialDatas, packedMaterialHash, ref filteredMaterials);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ClassifyMaterials$BurstManaged(in NativeArray<int> materialIDs, in NativeParallelHashMap<int, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<int> supportedMaterialIDs, ref NativeList<int> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			NativeList<int> nativeList = new NativeList<int>(4, Allocator.Temp);
			NativeArray<int> nativeArray = materialIDs;
			foreach (int key in nativeArray)
			{
				if (batchMaterialHash.ContainsKey(key))
				{
					nativeList.Add(key);
				}
			}
			if (nativeList.IsEmpty)
			{
				nativeList.Dispose();
				return;
			}
			unsupportedMaterialIDs.Resize(nativeList.Length, NativeArrayOptions.UninitializedMemory);
			supportedMaterialIDs.Resize(nativeList.Length, NativeArrayOptions.UninitializedMemory);
			supportedPackedMaterialDatas.Resize(nativeList.Length, NativeArrayOptions.UninitializedMemory);
			int num = GPUDrivenProcessor.ClassifyMaterials(nativeList.AsArray(), unsupportedMaterialIDs.AsArray(), supportedMaterialIDs.AsArray(), supportedPackedMaterialDatas.AsArray());
			unsupportedMaterialIDs.Resize(num, NativeArrayOptions.ClearMemory);
			supportedMaterialIDs.Resize(nativeList.Length - num, NativeArrayOptions.ClearMemory);
			supportedPackedMaterialDatas.Resize(supportedMaterialIDs.Length, NativeArrayOptions.ClearMemory);
			nativeList.Dispose();
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FindUnsupportedRenderers$BurstManaged(in NativeArray<int> unsupportedMaterials, in NativeArray<SmallIntegerArray>.ReadOnly materialIDArrays, in NativeArray<int>.ReadOnly rendererGroups, ref NativeList<int> unsupportedRenderers)
		{
			int num = 0;
			for (;;)
			{
				int num2 = num;
				NativeArray<SmallIntegerArray>.ReadOnly readOnly = materialIDArrays;
				if (num2 >= readOnly.Length)
				{
					break;
				}
				readOnly = materialIDArrays;
				SmallIntegerArray smallIntegerArray = readOnly[num];
				NativeArray<int>.ReadOnly readOnly2 = rendererGroups;
				int num3 = readOnly2[num];
				for (int i = 0; i < smallIntegerArray.Length; i++)
				{
					int value = smallIntegerArray[i];
					if (unsupportedMaterials.Contains(value))
					{
						unsupportedRenderers.Add(num3);
						break;
					}
				}
				num++;
			}
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetMaterialsWithChangedPackedMaterial$BurstManaged(in NativeArray<int> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<int> filteredMaterials)
		{
			int num = 0;
			for (;;)
			{
				int num2 = num;
				NativeArray<int> nativeArray = materialIDs;
				if (num2 >= nativeArray.Length)
				{
					break;
				}
				nativeArray = materialIDs;
				int num3 = nativeArray[num];
				NativeArray<GPUDrivenPackedMaterialData> nativeArray2 = packedMaterialDatas;
				GPUDrivenPackedMaterialData other = nativeArray2[num];
				GPUDrivenPackedMaterialData gpudrivenPackedMaterialData;
				if (!packedMaterialHash.TryGetValue(num3, out gpudrivenPackedMaterialData) || !gpudrivenPackedMaterialData.Equals(other))
				{
					filteredMaterials.Add(num3);
				}
				num++;
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ClassifyMaterials_000000EA$PostfixBurstDelegate(in NativeArray<int> materialIDs, in NativeParallelHashMap<int, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<int> supportedMaterialIDs, ref NativeList<int> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas);

		internal static class ClassifyMaterials_000000EA$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (GPUResidentDrawerBurst.ClassifyMaterials_000000EA$BurstDirectCall.Pointer == 0)
				{
					GPUResidentDrawerBurst.ClassifyMaterials_000000EA$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<GPUResidentDrawerBurst.ClassifyMaterials_000000EA$PostfixBurstDelegate>(new GPUResidentDrawerBurst.ClassifyMaterials_000000EA$PostfixBurstDelegate(GPUResidentDrawerBurst.ClassifyMaterials)).Value;
				}
				A_0 = GPUResidentDrawerBurst.ClassifyMaterials_000000EA$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				GPUResidentDrawerBurst.ClassifyMaterials_000000EA$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in NativeArray<int> materialIDs, in NativeParallelHashMap<int, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<int> supportedMaterialIDs, ref NativeList<int> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = GPUResidentDrawerBurst.ClassifyMaterials_000000EA$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeParallelHashMap`2/ReadOnly<System.Int32,UnityEngine.Rendering.BatchMaterialID>&,Unity.Collections.NativeList`1<System.Int32>&,Unity.Collections.NativeList`1<System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.GPUDrivenPackedMaterialData>&), ref materialIDs, ref batchMaterialHash, ref supportedMaterialIDs, ref unsupportedMaterialIDs, ref supportedPackedMaterialDatas, functionPointer);
						return;
					}
				}
				GPUResidentDrawerBurst.ClassifyMaterials$BurstManaged(materialIDs, batchMaterialHash, ref supportedMaterialIDs, ref unsupportedMaterialIDs, ref supportedPackedMaterialDatas);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void FindUnsupportedRenderers_000000EB$PostfixBurstDelegate(in NativeArray<int> unsupportedMaterials, in NativeArray<SmallIntegerArray>.ReadOnly materialIDArrays, in NativeArray<int>.ReadOnly rendererGroups, ref NativeList<int> unsupportedRenderers);

		internal static class FindUnsupportedRenderers_000000EB$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$BurstDirectCall.Pointer == 0)
				{
					GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$PostfixBurstDelegate>(new GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$PostfixBurstDelegate(GPUResidentDrawerBurst.FindUnsupportedRenderers)).Value;
				}
				A_0 = GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in NativeArray<int> unsupportedMaterials, in NativeArray<SmallIntegerArray>.ReadOnly materialIDArrays, in NativeArray<int>.ReadOnly rendererGroups, ref NativeList<int> unsupportedRenderers)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = GPUResidentDrawerBurst.FindUnsupportedRenderers_000000EB$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1/ReadOnly<UnityEngine.Rendering.SmallIntegerArray>&,Unity.Collections.NativeArray`1/ReadOnly<System.Int32>&,Unity.Collections.NativeList`1<System.Int32>&), ref unsupportedMaterials, ref materialIDArrays, ref rendererGroups, ref unsupportedRenderers, functionPointer);
						return;
					}
				}
				GPUResidentDrawerBurst.FindUnsupportedRenderers$BurstManaged(unsupportedMaterials, materialIDArrays, rendererGroups, ref unsupportedRenderers);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetMaterialsWithChangedPackedMaterial_000000EC$PostfixBurstDelegate(in NativeArray<int> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<int> filteredMaterials);

		internal static class GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall.Pointer == 0)
				{
					GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$PostfixBurstDelegate>(new GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$PostfixBurstDelegate(GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial)).Value;
				}
				A_0 = GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in NativeArray<int> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<int> filteredMaterials)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial_000000EC$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.GPUDrivenPackedMaterialData>&,Unity.Collections.NativeParallelHashMap`2/ReadOnly<System.Int32,UnityEngine.Rendering.GPUDrivenPackedMaterialData>&,Unity.Collections.NativeHashSet`1<System.Int32>&), ref materialIDs, ref packedMaterialDatas, ref packedMaterialHash, ref filteredMaterials, functionPointer);
						return;
					}
				}
				GPUResidentDrawerBurst.GetMaterialsWithChangedPackedMaterial$BurstManaged(materialIDs, packedMaterialDatas, packedMaterialHash, ref filteredMaterials);
			}

			private static IntPtr Pointer;
		}
	}
}
