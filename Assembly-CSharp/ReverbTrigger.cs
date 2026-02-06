using System;
using UnityEngine;
using UnityEngine.Audio;

public class ReverbTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			this.targetSnapshot.TransitionTo(this.transitionTime);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			this.normalSnapshot.TransitionTo(this.transitionTime);
		}
	}

	[SerializeField]
	private AudioMixer mixer;

	[SerializeField]
	private AudioMixerSnapshot targetSnapshot;

	[SerializeField]
	private AudioMixerSnapshot normalSnapshot;

	[SerializeField]
	private Collider reverbTrigger;

	[SerializeField]
	private float transitionTime = 1f;
}
