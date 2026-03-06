using System;

namespace UnityEngine.Rendering
{
	public interface IDefaultVolumeProfileSettings : IRenderPipelineGraphicsSettings
	{
		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		VolumeProfile volumeProfile { get; set; }
	}
}
