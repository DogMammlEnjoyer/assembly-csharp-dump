using System;
using UnityEngine;

namespace Meta.WitAi.Data
{
	public class AudioBufferPrefabProvider : MonoBehaviour, IAudioBufferProvider
	{
		private void Awake()
		{
			AudioBuffer.AudioBufferProvider = this;
		}

		public AudioBuffer InstantiateAudioBuffer()
		{
			if (this._audioBufferPrefab == null)
			{
				return null;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this._audioBufferPrefab.gameObject, null, true);
			gameObject.name = this._audioBufferPrefab.gameObject.name;
			return gameObject.GetComponent<AudioBuffer>();
		}

		[SerializeField]
		private AudioBuffer _audioBufferPrefab;
	}
}
