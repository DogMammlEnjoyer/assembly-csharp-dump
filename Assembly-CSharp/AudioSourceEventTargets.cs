using System;
using UnityEngine;

public class AudioSourceEventTargets : MonoBehaviour
{
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.fadeVolume = this.audioSource.volume;
	}

	public void SetFadeSpeed(float arg)
	{
		this.fadeSpeed = Mathf.Max(arg, 0.01f);
	}

	public void StartFade(float arg)
	{
		this.fadeVolume = Mathf.Clamp01(arg);
	}

	public void Update()
	{
		if (this.audioSource.volume != this.fadeVolume)
		{
			this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, this.fadeVolume, this.fadeSpeed * Time.deltaTime);
		}
		if (this.lastValueWhenPlayed != this.ExternalTriggerPlay)
		{
			if (!this.lastExternalTriggerPlayMatched)
			{
				this.audioSource.Play();
				this.lastValueWhenPlayed = this.ExternalTriggerPlay;
				this.lastExternalTriggerPlayMatched = true;
			}
			else
			{
				this.ExternalTriggerPlay = this.lastValueWhenPlayed;
				this.lastExternalTriggerPlayMatched = false;
			}
		}
		else
		{
			this.lastExternalTriggerPlayMatched = true;
		}
		if (this.lastValueWhenStopped == this.ExternalTriggerStop)
		{
			this.lastExternalTriggerStopMatched = true;
			return;
		}
		if (!this.lastExternalTriggerStopMatched)
		{
			this.audioSource.Stop();
			this.lastValueWhenStopped = this.ExternalTriggerStop;
			this.lastExternalTriggerStopMatched = true;
			return;
		}
		this.ExternalTriggerStop = this.lastValueWhenStopped;
		this.lastExternalTriggerStopMatched = false;
	}

	private AudioSource audioSource;

	private float fadeVolume;

	private float fadeSpeed;

	[Header("Change Value To Trigger Play (false to true and true to false both work, but value must change the frame you want it played)")]
	public bool ExternalTriggerPlay;

	private bool lastExternalTriggerPlayMatched = true;

	private bool lastValueWhenPlayed;

	[Header("Change Value To Trigger Stop (false to true and true to false both work, but value must change the frame you want it stopped)")]
	public bool ExternalTriggerStop;

	private bool lastExternalTriggerStopMatched = true;

	private bool lastValueWhenStopped;
}
