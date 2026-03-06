using System;
using UnityEngine;

public class AudioSourceEventTargets : MonoBehaviour
{
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.fadeVolume = this.audioSource.volume;
		base.enabled = false;
	}

	public void SetFadeSpeed(float arg)
	{
		this.fadeSpeed = Mathf.Max(arg, 0.01f);
	}

	public void StartFade(float arg)
	{
		this.fadeVolume = Mathf.Clamp01(arg);
		base.enabled = true;
	}

	public void Update()
	{
		if (this.audioSource.volume != this.fadeVolume)
		{
			this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, this.fadeVolume, this.fadeSpeed * Time.deltaTime);
		}
		base.enabled = (this.audioSource.volume != this.fadeVolume);
	}

	private AudioSource audioSource;

	private float fadeVolume;

	private float fadeSpeed;
}
