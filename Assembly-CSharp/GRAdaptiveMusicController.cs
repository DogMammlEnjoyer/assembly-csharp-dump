using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRAdaptiveMusicController : MonoBehaviour
{
	private void Start()
	{
		this.cachedSourcePosition = this.AudioSources[0].transform.position;
	}

	private void PlayCurrentTrack()
	{
		if (this.trackIndex < 0 || this.trackIndex >= this.Tracks.Count)
		{
			return;
		}
		GRAdaptiveMusicController.SingleTrack singleTrack = this.Tracks[this.trackIndex];
		AudioSource currentAudioSource = this.GetCurrentAudioSource();
		currentAudioSource.clip = singleTrack.IntroClip;
		currentAudioSource.Play();
		AudioSource nextAudioSource = this.GetNextAudioSource();
		nextAudioSource.clip = singleTrack.LoopedClip;
		nextAudioSource.loop = true;
		double num = AudioSettings.dspTime + (double)singleTrack.IntroClip.length;
		currentAudioSource.SetScheduledEndTime(num);
		nextAudioSource.PlayScheduled(num);
		this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		this.CurrentTrack = singleTrack;
	}

	[ContextMenu("Transition Next Track")]
	public void TransitionToNextTrack()
	{
		this.GoToTrack(this.trackIndex + 1, false);
	}

	public void TransitionToLastTrack()
	{
		this.GoToTrack(this.Tracks.Count - 1, false);
	}

	public void GoToTrack(int nextIndex, bool force = false)
	{
		if (!force && (nextIndex < 0 || nextIndex >= this.Tracks.Count || this.trackIndex == nextIndex))
		{
			return;
		}
		Debug.Log(string.Format("GRAdaptiveMusicController - Going to track {0}.", nextIndex));
		GRAdaptiveMusicController.SingleTrack singleTrack = this.Tracks[nextIndex];
		AudioSource audioSource = this.GetCurrentAudioSource();
		AudioSource nextAudioSource = this.GetNextAudioSource();
		double num = (double)audioSource.timeSamples / (double)audioSource.clip.frequency % GRAdaptiveMusicController.BAR_DURATION;
		double num2 = AudioSettings.dspTime + (GRAdaptiveMusicController.BAR_DURATION - num);
		nextAudioSource.Stop();
		nextAudioSource.clip = singleTrack.IntroClip;
		nextAudioSource.loop = false;
		audioSource.SetScheduledEndTime(num2);
		nextAudioSource.PlayScheduled(num2);
		this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		if (singleTrack.LoopedClip != null)
		{
			audioSource = nextAudioSource;
			nextAudioSource = this.GetNextAudioSource();
			nextAudioSource.clip = singleTrack.LoopedClip;
			nextAudioSource.loop = true;
			double num3 = num2 + (double)singleTrack.IntroClip.length;
			audioSource.SetScheduledEndTime(num3);
			nextAudioSource.PlayScheduled(num3);
			this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		}
		else
		{
			this.Finish(singleTrack.IntroClip.length + 1f);
		}
		this.trackIndex = nextIndex;
		this.CurrentTrack = singleTrack;
	}

	[ContextMenu("Restart")]
	public void Restart()
	{
		Debug.Log("Restarting AdaptiveMusicController.");
		this.cachedSourceVolume = this.AudioSources[0].volume;
		this.synchedMusicController.enabled = false;
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.AdjustedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.RepositionAudioSourcePoint.position);
		}
		this.trackIndex = 0;
		this.currentAudioSourceIndex = 0;
		this.PlayCurrentTrack();
	}

	public void RestartAt(int index)
	{
		Debug.Log(string.Format("Restarting AdaptiveMusicController at index {0}.", index));
		this.cachedSourceVolume = this.AudioSources[0].volume;
		this.synchedMusicController.enabled = false;
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.AdjustedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.RepositionAudioSourcePoint.position);
		}
		this.trackIndex = index;
		this.currentAudioSourceIndex = 0;
		this.GoToTrack(this.trackIndex, true);
	}

	private AudioSource GetCurrentAudioSource()
	{
		return this.AudioSources[this.currentAudioSourceIndex];
	}

	private AudioSource GetNextAudioSource()
	{
		return this.AudioSources[this.NextAudioSourceIndex];
	}

	private int NextAudioSourceIndex
	{
		get
		{
			return (this.currentAudioSourceIndex + 1) % this.AudioSources.Count;
		}
	}

	private void StopAllAudioSources()
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].Stop();
		}
	}

	private void UpdateAudioSourcesVolume(float volume)
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].mute = false;
			this.AudioSources[i].volume = volume;
		}
	}

	private void UpdateAudioSourcesPosition(Vector3 position)
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].transform.position = position;
		}
	}

	private void Finish(float delay)
	{
		if (this.finishCoroutine != null)
		{
			return;
		}
		this.finishCoroutine = base.StartCoroutine(this.TryFinish(delay));
	}

	private IEnumerator TryFinish(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.cachedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.cachedSourcePosition);
		}
		this.synchedMusicController.enabled = true;
		yield break;
	}

	private static double BAR_DURATION = 1.6551724137931034;

	public List<GRAdaptiveMusicController.SingleTrack> Tracks;

	public GRAdaptiveMusicController.SingleTrack CurrentTrack;

	[SerializeField]
	private int trackIndex;

	public List<AudioSource> AudioSources;

	public Transform RepositionAudioSourcePoint;

	public float AdjustedSourceVolume = 0.035f;

	private int currentAudioSourceIndex;

	private float cachedSourceVolume = 0.1f;

	private Vector3 cachedSourcePosition = Vector3.zero;

	[SerializeField]
	private SynchedMusicController synchedMusicController;

	private Coroutine finishCoroutine;

	[Serializable]
	public class SingleTrack
	{
		public AudioClip IntroClip;

		public AudioClip LoopedClip;
	}
}
