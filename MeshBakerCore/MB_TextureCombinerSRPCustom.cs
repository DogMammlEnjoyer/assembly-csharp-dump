using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public static class MB_TextureCombinerSRPCustom
	{
		private static bool IsURPMaterial(Material m)
		{
			return m.HasProperty("_BaseMap");
		}

		internal static void ConfigureMaterialKeywordsIfNecessary(MB3_TextureCombinerPipeline.TexturePipelineData data)
		{
			if (MBVersion.DetectPipeline() == MBVersion.PipelineType.URP && MB_TextureCombinerSRPCustom.IsURPMaterial(data.resultMaterial))
			{
				MB_TextureCombinerSRPCustom_URP.ConfigureMaterialKeywords(data, data.resultMaterial);
			}
			if (MBVersion.DetectPipeline() == MBVersion.PipelineType.Default && data.resultMaterial != null && data.resultMaterial.name.Contains("Standard"))
			{
				MB_TextureCombinerSRPCustom_Standard.ConfigureMaterialKeywords(data, data.resultMaterial);
			}
		}
	}
}
