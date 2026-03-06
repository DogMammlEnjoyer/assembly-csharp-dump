using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[RequiredByNativeCode]
	internal static class GPUDrivenCallbacks
	{
		[RequiredByNativeCode(GenerateProxy = true)]
		public static void InvokeGPUDrivenLODGroupDataNativeCallback(GPUDrivenLODGroupDataNativeCallback callback, in GPUDrivenLODGroupDataNative lodGroupDataNative, GPUDrivenLODGroupDataCallback target)
		{
			callback(lodGroupDataNative, target);
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		public static void InvokeGPUDrivenRendererDataNativeCallback(GPUDrivenRendererDataNativeCallback callback, in GPUDrivenRendererGroupDataNative rendererDataNative, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback target)
		{
			callback(rendererDataNative, meshes, materials, target);
		}
	}
}
