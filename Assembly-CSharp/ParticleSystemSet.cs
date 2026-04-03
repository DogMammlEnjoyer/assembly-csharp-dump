using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParticleSystemSet : MonoBehaviour
{
	private void Awake()
	{
		this.localScale = base.transform.localScale;
		this.ps = base.GetComponentsInChildren<ParticleSystem>();
		List<ParticleSystem.MainModule> list = new List<ParticleSystem.MainModule>();
		List<ParticleSystem.EmissionModule> list2 = new List<ParticleSystem.EmissionModule>();
		for (int i = 0; i < this.ps.Length; i++)
		{
			list.Add(this.ps[i].main);
			list2.Add(this.ps[i].emission);
		}
		this.psMains = list.ToArray();
		this.psEmits = list2.ToArray();
		this.SetPlayBackSpeed(0f);
	}

	public void SetFadeRate(float rate)
	{
		if (rate > 0f)
		{
			this.fadeRate = rate;
		}
	}

	public void SetPlayBackSpeed(float target)
	{
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].simulationSpeed = target;
		}
	}

	public void FadePlayBackSpeed(float target)
	{
		ParticleSystemSet.<FadePlayBackSpeed>d__9 <FadePlayBackSpeed>d__;
		<FadePlayBackSpeed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<FadePlayBackSpeed>d__.<>4__this = this;
		<FadePlayBackSpeed>d__.target = target;
		<FadePlayBackSpeed>d__.<>1__state = -1;
		<FadePlayBackSpeed>d__.<>t__builder.Start<ParticleSystemSet.<FadePlayBackSpeed>d__9>(ref <FadePlayBackSpeed>d__);
	}

	public void SetColor(string RRGGBB)
	{
		Color color = new Color((float)int.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
		float h;
		float s;
		float num;
		Color.RGBToHSV(color, out h, out s, out num);
		Color max = Color.HSVToRGB(h, s, num / 4f);
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].startColor = new ParticleSystem.MinMaxGradient(color, max);
		}
	}

	public void SetColors(string RRGGBBRRGGBB)
	{
		Color min = new Color((float)int.Parse(RRGGBBRRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
		Color max = new Color((float)int.Parse(RRGGBBRRGGBB.Substring(6, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(8, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(10, 2), NumberStyles.HexNumber) / 255f);
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].startColor = new ParticleSystem.MinMaxGradient(min, max);
		}
	}

	public void Pause()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Pause();
		}
	}

	public void StartEmission()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.psMains[i].prewarm = false;
			this.ps[i].Play();
		}
	}

	public void StopEmission()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Stop();
		}
	}

	public void Clear()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Clear();
		}
	}

	public void SetScaleXZ(float scaler)
	{
		base.transform.localScale = new Vector3(this.localScale.x * scaler, this.localScale.y, this.localScale.z * scaler);
	}

	public void FadeScaleXZ(float scaler)
	{
		ParticleSystemSet.<FadeScaleXZ>d__17 <FadeScaleXZ>d__;
		<FadeScaleXZ>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<FadeScaleXZ>d__.<>4__this = this;
		<FadeScaleXZ>d__.scaler = scaler;
		<FadeScaleXZ>d__.<>1__state = -1;
		<FadeScaleXZ>d__.<>t__builder.Start<ParticleSystemSet.<FadeScaleXZ>d__17>(ref <FadeScaleXZ>d__);
	}

	private Vector3 localScale = Vector3.one;

	private ParticleSystem[] ps;

	private ParticleSystem.MainModule[] psMains;

	private ParticleSystem.EmissionModule[] psEmits;

	private bool loop;

	private float fadeRate = 1f;
}
