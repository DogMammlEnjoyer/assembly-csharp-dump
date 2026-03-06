using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.UnifiedRayTracing;

[DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__16164947281921951637
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<ComputeTerrainMeshJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		__JobReflectionRegistrationOutput__16164947281921951637.CreateJobReflectionData();
	}
}
