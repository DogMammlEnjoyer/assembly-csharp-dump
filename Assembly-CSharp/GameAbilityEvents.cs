using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameAbilityEvents
{
	public void Reset()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].Reset();
		}
	}

	public void OnAbilityStart(float abilityTime, AudioSource audioSource)
	{
		this.startEvent.TryPlay(abilityTime, (this.startEvent.sound.audioSource == null) ? audioSource : this.startEvent.sound.audioSource);
	}

	public void OnAbilityStop(float abilityTime, AudioSource audioSource)
	{
		this.stopEvent.TryPlay(abilityTime, (this.stopEvent.sound.audioSource == null) ? audioSource : this.stopEvent.sound.audioSource);
	}

	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].TryPlay(abilityTime, (this.events[i].sound.audioSource == null) ? audioSource : this.events[i].sound.audioSource);
		}
	}

	public GameAbilityEvent startEvent;

	public GameAbilityEvent stopEvent;

	public List<GameAbilityEvent> events;
}
