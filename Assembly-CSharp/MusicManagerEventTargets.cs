using System;
using UnityEngine;

public class MusicManagerEventTargets : MonoBehaviour
{
	public void StopAllMusic()
	{
		this.StopAllMusic(null);
	}

	public void StopAllMusic(AudioClip clip)
	{
		MusicManager.StopAllMusic(clip);
	}
}
