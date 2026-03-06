using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal static class BackendHelpers
	{
		internal static string GetFileNameOfShader(RayTracingBackend backend, string fileName)
		{
			string text;
			if (backend != RayTracingBackend.Hardware)
			{
				if (backend != RayTracingBackend.Compute)
				{
					throw new ArgumentOutOfRangeException("backend", backend, null);
				}
				text = "compute";
			}
			else
			{
				text = "raytrace";
			}
			string str = text;
			return fileName + "." + str;
		}

		internal static Type GetTypeOfShader(RayTracingBackend backend)
		{
			Type typeFromHandle;
			if (backend != RayTracingBackend.Hardware)
			{
				if (backend != RayTracingBackend.Compute)
				{
					throw new ArgumentOutOfRangeException("backend", backend, null);
				}
				typeFromHandle = typeof(ComputeShader);
			}
			else
			{
				typeFromHandle = typeof(RayTracingShader);
			}
			return typeFromHandle;
		}
	}
}
