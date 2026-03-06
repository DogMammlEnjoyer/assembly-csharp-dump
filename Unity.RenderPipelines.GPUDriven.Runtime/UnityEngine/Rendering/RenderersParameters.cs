using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct RenderersParameters
	{
		public static GPUInstanceDataBuffer CreateInstanceDataBuffer(RenderersParameters.Flags flags, in InstanceNumInfo instanceNumInfo)
		{
			GPUInstanceDataBuffer result;
			using (GPUInstanceDataBufferBuilder gpuinstanceDataBufferBuilder = default(GPUInstanceDataBufferBuilder))
			{
				gpuinstanceDataBufferBuilder.AddComponent<Vector4>(RenderersParameters.ParamNames._BaseColor, false, false, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				gpuinstanceDataBufferBuilder.AddComponent<Vector4>(RenderersParameters.ParamNames.unity_SpecCube0_HDR, false, false, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				gpuinstanceDataBufferBuilder.AddComponent<SHCoefficients>(RenderersParameters.ParamNames.unity_SHCoefficients, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.LightProbe);
				gpuinstanceDataBufferBuilder.AddComponent<Vector4>(RenderersParameters.ParamNames.unity_LightmapST, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.Lightmap);
				gpuinstanceDataBufferBuilder.AddComponent<PackedMatrix>(RenderersParameters.ParamNames.unity_ObjectToWorld, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				gpuinstanceDataBufferBuilder.AddComponent<PackedMatrix>(RenderersParameters.ParamNames.unity_WorldToObject, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				gpuinstanceDataBufferBuilder.AddComponent<PackedMatrix>(RenderersParameters.ParamNames.unity_MatrixPreviousM, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				gpuinstanceDataBufferBuilder.AddComponent<PackedMatrix>(RenderersParameters.ParamNames.unity_MatrixPreviousMI, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				if ((flags & RenderersParameters.Flags.UseBoundingSphereParameter) != RenderersParameters.Flags.None)
				{
					gpuinstanceDataBufferBuilder.AddComponent<Vector4>(RenderersParameters.ParamNames.unity_WorldBoundingSphere, true, true, InstanceType.MeshRenderer, InstanceComponentGroup.Default);
				}
				for (int i = 0; i < 16; i++)
				{
					gpuinstanceDataBufferBuilder.AddComponent<Vector4>(RenderersParameters.ParamNames.DOTS_ST_WindParams[i], true, true, InstanceType.SpeedTree, InstanceComponentGroup.Wind);
				}
				for (int j = 0; j < 16; j++)
				{
					gpuinstanceDataBufferBuilder.AddComponent<Vector4>(RenderersParameters.ParamNames.DOTS_ST_WindHistoryParams[j], true, true, InstanceType.SpeedTree, InstanceComponentGroup.Wind);
				}
				result = gpuinstanceDataBufferBuilder.Build(instanceNumInfo);
			}
			return result;
		}

		public RenderersParameters(in GPUInstanceDataBuffer instanceDataBuffer)
		{
			this.lightmapScale = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_LightmapST, true);
			this.localToWorld = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_ObjectToWorld, true);
			this.worldToLocal = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_WorldToObject, true);
			this.matrixPreviousM = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_MatrixPreviousM, true);
			this.matrixPreviousMI = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_MatrixPreviousMI, true);
			this.shCoefficients = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_SHCoefficients, true);
			this.boundingSphere = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.unity_WorldBoundingSphere, false);
			this.windParams = new RenderersParameters.ParamInfo[16];
			this.windHistoryParams = new RenderersParameters.ParamInfo[16];
			for (int i = 0; i < 16; i++)
			{
				this.windParams[i] = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.DOTS_ST_WindParams[i], true);
				this.windHistoryParams[i] = RenderersParameters.<.ctor>g__GetParamInfo|14_0(instanceDataBuffer, RenderersParameters.ParamNames.DOTS_ST_WindHistoryParams[i], true);
			}
		}

		[CompilerGenerated]
		internal static RenderersParameters.ParamInfo <.ctor>g__GetParamInfo|14_0(in GPUInstanceDataBuffer instanceDataBuffer, int paramNameIdx, bool assertOnFail = true)
		{
			int gpuAddress = instanceDataBuffer.GetGpuAddress(paramNameIdx, assertOnFail);
			int propertyIndex = instanceDataBuffer.GetPropertyIndex(paramNameIdx, assertOnFail);
			return new RenderersParameters.ParamInfo
			{
				index = propertyIndex,
				gpuAddress = gpuAddress,
				uintOffset = gpuAddress / RenderersParameters.s_uintSize
			};
		}

		private static int s_uintSize = UnsafeUtility.SizeOf<uint>();

		public RenderersParameters.ParamInfo lightmapScale;

		public RenderersParameters.ParamInfo localToWorld;

		public RenderersParameters.ParamInfo worldToLocal;

		public RenderersParameters.ParamInfo matrixPreviousM;

		public RenderersParameters.ParamInfo matrixPreviousMI;

		public RenderersParameters.ParamInfo shCoefficients;

		public RenderersParameters.ParamInfo boundingSphere;

		public RenderersParameters.ParamInfo[] windParams;

		public RenderersParameters.ParamInfo[] windHistoryParams;

		[Flags]
		public enum Flags
		{
			None = 0,
			UseBoundingSphereParameter = 1
		}

		public static class ParamNames
		{
			static ParamNames()
			{
				for (int i = 0; i < 16; i++)
				{
					RenderersParameters.ParamNames.DOTS_ST_WindParams[i] = Shader.PropertyToID(string.Format("DOTS_ST_WindParam{0}", i));
					RenderersParameters.ParamNames.DOTS_ST_WindHistoryParams[i] = Shader.PropertyToID(string.Format("DOTS_ST_WindHistoryParam{0}", i));
				}
			}

			public static readonly int _BaseColor = Shader.PropertyToID("_BaseColor");

			public static readonly int unity_SpecCube0_HDR = Shader.PropertyToID("unity_SpecCube0_HDR");

			public static readonly int unity_SHCoefficients = Shader.PropertyToID("unity_SHCoefficients");

			public static readonly int unity_LightmapST = Shader.PropertyToID("unity_LightmapST");

			public static readonly int unity_ObjectToWorld = Shader.PropertyToID("unity_ObjectToWorld");

			public static readonly int unity_WorldToObject = Shader.PropertyToID("unity_WorldToObject");

			public static readonly int unity_MatrixPreviousM = Shader.PropertyToID("unity_MatrixPreviousM");

			public static readonly int unity_MatrixPreviousMI = Shader.PropertyToID("unity_MatrixPreviousMI");

			public static readonly int unity_WorldBoundingSphere = Shader.PropertyToID("unity_WorldBoundingSphere");

			public static readonly int[] DOTS_ST_WindParams = new int[16];

			public static readonly int[] DOTS_ST_WindHistoryParams = new int[16];
		}

		public struct ParamInfo
		{
			public bool valid
			{
				get
				{
					return this.index != 0;
				}
			}

			public int index;

			public int gpuAddress;

			public int uintOffset;
		}
	}
}
