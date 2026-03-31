using System;
using UnityEngine;

public class GameLightingManagerEventRelay : MonoBehaviour
{
	public void SetCustomDynamicLightingEnabled(bool value)
	{
		if (GameLightingManager.instance == null)
		{
			Debug.LogError("GameLightingManagerEventRelay :: GameLightingManager has not been instanced!");
			return;
		}
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(value);
	}

	public void SetNearsightedDimLightIntensity(float value)
	{
		if (GameLightingManager.instance == null)
		{
			Debug.LogError("GameLightingManagerEventRelay :: GameLightingManager has not been instanced!");
			return;
		}
		GameLightingManager.instance.GR_NearsightedDimLight.intensity = value;
	}
}
