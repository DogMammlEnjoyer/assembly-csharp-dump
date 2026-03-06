using System;

namespace UnityEngine.Rendering.Universal
{
	public static class LightExtensions
	{
		public static UniversalAdditionalLightData GetUniversalAdditionalLightData(this Light light)
		{
			GameObject gameObject = light.gameObject;
			UniversalAdditionalLightData result;
			if (!gameObject.TryGetComponent<UniversalAdditionalLightData>(out result))
			{
				result = gameObject.AddComponent<UniversalAdditionalLightData>();
			}
			return result;
		}
	}
}
