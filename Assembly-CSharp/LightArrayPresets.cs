using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightArrayPresets", menuName = "Scriptable Objects/LightArrayPresets")]
public class LightArrayPresets : ScriptableObject
{
	private void initLookup()
	{
		this.lookup = new Dictionary<string, LightArrayPresets.LightArrayPreset>();
		for (int i = 0; i < this.presets.Length; i++)
		{
			this.lookup.Add(this.presets[i].name, this.presets[i]);
		}
	}

	public LightArrayPresets.LightArrayPreset GetPreset(int i)
	{
		return this.presets[i];
	}

	public LightArrayPresets.LightArrayPreset GetPreset(string n)
	{
		if (this.lookup == null)
		{
			this.initLookup();
		}
		return this.lookup[n];
	}

	private Dictionary<string, LightArrayPresets.LightArrayPreset> lookup;

	[SerializeField]
	private LightArrayPresets.LightArrayPreset[] presets;

	[Serializable]
	public class LightArrayPreset
	{
		public string name = "Color";

		public Color color = Color.white;

		public float intensity = 1f;
	}
}
