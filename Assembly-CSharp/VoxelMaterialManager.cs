using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMaterialManager : MonoBehaviour
{
	private void Update()
	{
		if (this._timeOfDayIndex != BetterDayNightManager.instance.currentTimeIndex)
		{
			this.UpdateMaterial();
		}
	}

	private void UpdateMaterial()
	{
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		if (string.IsNullOrEmpty(currentTimeOfDay))
		{
			return;
		}
		int num = this.lightmapNames.IndexOf(currentTimeOfDay);
		if (num < 0 || num >= this.lightingProfiles.Count)
		{
			return;
		}
		Debug.Log(string.Format("[V] Setting lighting profile {0} ({1})", num, currentTimeOfDay));
		VoxelMaterialManager.LightingProfile lightingProfile = this.lightingProfiles[num];
		Shader.SetGlobalVector("_Light_Direction", lightingProfile.direction);
		Shader.SetGlobalColor("_Light_Color", lightingProfile.color);
		Shader.SetGlobalColor("_Shadow_Color", lightingProfile.color * 0.32f);
		Shader.SetGlobalColor("_Rim_Color", lightingProfile.color * 0.1f);
		Shader.SetGlobalColor("_Backlight_Color", lightingProfile.color * 0.07f);
		this._timeOfDayIndex = BetterDayNightManager.instance.currentTimeIndex;
	}

	public Material[] voxelMats;

	public List<string> lightmapNames;

	public List<VoxelMaterialManager.LightingProfile> lightingProfiles;

	private int _timeOfDayIndex = -1;

	[Serializable]
	public struct LightingProfile
	{
		public Color color;

		public Vector3 direction;
	}
}
