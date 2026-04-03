using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class LightArray : MonoBehaviour
{
	private void ToggleDynamicLighting()
	{
		GameLightingManager.instance.ToggleCustomDynamicLightingEnabled();
	}

	public void SetCascadeTime(int ct)
	{
		this.cascadeTime = ct;
	}

	public void SetSubArraysCascadeTime(int ct)
	{
		for (int i = 0; i < this.subArrays.Length; i++)
		{
			this.subArrays[i].cascadeTime = ct;
		}
	}

	public void SetPreset(int i)
	{
		if (this.presets == null)
		{
			return;
		}
		LightArrayPresets.LightArrayPreset preset = this.presets.GetPreset(i);
		if (preset == null)
		{
			return;
		}
		this.SetColorAndIntensity(preset.color, preset.intensity);
	}

	public void SetPreset(string n)
	{
		if (this.presets == null)
		{
			return;
		}
		LightArrayPresets.LightArrayPreset preset = this.presets.GetPreset(n);
		if (preset == null)
		{
			return;
		}
		this.SetColorAndIntensity(preset.color, preset.intensity);
	}

	public void SetColorAndIntensity(string RRGGBBF)
	{
		this.SetColorAndIntensity(this.GetColor(RRGGBBF), float.Parse(RRGGBBF.Substring(6).ToString()));
	}

	private void SetColorAndIntensity(Color c, float intensity)
	{
		LightArray.<SetColorAndIntensity>d__10 <SetColorAndIntensity>d__;
		<SetColorAndIntensity>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetColorAndIntensity>d__.<>4__this = this;
		<SetColorAndIntensity>d__.c = c;
		<SetColorAndIntensity>d__.intensity = intensity;
		<SetColorAndIntensity>d__.<>1__state = -1;
		<SetColorAndIntensity>d__.<>t__builder.Start<LightArray.<SetColorAndIntensity>d__10>(ref <SetColorAndIntensity>d__);
	}

	public void SetColor(string RRGGBB)
	{
		this.SetColor(this.GetColor(RRGGBB));
	}

	private void SetColor(Color c)
	{
		LightArray.<SetColor>d__12 <SetColor>d__;
		<SetColor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetColor>d__.<>4__this = this;
		<SetColor>d__.c = c;
		<SetColor>d__.<>1__state = -1;
		<SetColor>d__.<>t__builder.Start<LightArray.<SetColor>d__12>(ref <SetColor>d__);
	}

	public void SetIntensity(float intensity)
	{
		LightArray.<SetIntensity>d__13 <SetIntensity>d__;
		<SetIntensity>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetIntensity>d__.<>4__this = this;
		<SetIntensity>d__.intensity = intensity;
		<SetIntensity>d__.<>1__state = -1;
		<SetIntensity>d__.<>t__builder.Start<LightArray.<SetIntensity>d__13>(ref <SetIntensity>d__);
	}

	private Task SetLightColorAndIntensity(Color c, float intensity, int i)
	{
		LightArray.<SetLightColorAndIntensity>d__14 <SetLightColorAndIntensity>d__;
		<SetLightColorAndIntensity>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetLightColorAndIntensity>d__.<>4__this = this;
		<SetLightColorAndIntensity>d__.c = c;
		<SetLightColorAndIntensity>d__.intensity = intensity;
		<SetLightColorAndIntensity>d__.i = i;
		<SetLightColorAndIntensity>d__.<>1__state = -1;
		<SetLightColorAndIntensity>d__.<>t__builder.Start<LightArray.<SetLightColorAndIntensity>d__14>(ref <SetLightColorAndIntensity>d__);
		return <SetLightColorAndIntensity>d__.<>t__builder.Task;
	}

	private Task SetLightColor(Color c, int i)
	{
		LightArray.<SetLightColor>d__15 <SetLightColor>d__;
		<SetLightColor>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetLightColor>d__.<>4__this = this;
		<SetLightColor>d__.c = c;
		<SetLightColor>d__.i = i;
		<SetLightColor>d__.<>1__state = -1;
		<SetLightColor>d__.<>t__builder.Start<LightArray.<SetLightColor>d__15>(ref <SetLightColor>d__);
		return <SetLightColor>d__.<>t__builder.Task;
	}

	private Task SetLightIntensity(float intensity, int i)
	{
		LightArray.<SetLightIntensity>d__16 <SetLightIntensity>d__;
		<SetLightIntensity>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetLightIntensity>d__.<>4__this = this;
		<SetLightIntensity>d__.intensity = intensity;
		<SetLightIntensity>d__.i = i;
		<SetLightIntensity>d__.<>1__state = -1;
		<SetLightIntensity>d__.<>t__builder.Start<LightArray.<SetLightIntensity>d__16>(ref <SetLightIntensity>d__);
		return <SetLightIntensity>d__.<>t__builder.Task;
	}

	private Color GetColor(string RRGGBB)
	{
		return new Color((float)int.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
	}

	[SerializeField]
	private LightArrayPresets presets;

	[SerializeField]
	private GameLight[] lights;

	[SerializeField]
	private LightArray[] subArrays;

	[SerializeField]
	private int cascadeTime;
}
