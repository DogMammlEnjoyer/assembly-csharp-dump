using System;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using UnityEngine;

namespace Liv.Lck
{
	public class LckDiscreetAudioController : MonoBehaviour
	{
		private void Awake()
		{
			this.InitializeAudioClipDictionary();
		}

		private void InitializeAudioClipDictionary()
		{
			this._allAudioClips = new Dictionary<LckDiscreetAudioController.AudioClip, LckDiscreetAudioController.AudioClipAndVolume>
			{
				{
					LckDiscreetAudioController.AudioClip.RecordingStart,
					new LckDiscreetAudioController.AudioClipAndVolume(this._recordingStart, this._recordingStartVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.RecordingSaved,
					new LckDiscreetAudioController.AudioClipAndVolume(this._recordingSaved, this._recordingSavedVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.ClickDown,
					new LckDiscreetAudioController.AudioClipAndVolume(this._clickDown, this._clickDownVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.ClickUp,
					new LckDiscreetAudioController.AudioClipAndVolume(this._clickUp, this._clickUpVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.HoverSound,
					new LckDiscreetAudioController.AudioClipAndVolume(this._hoverSound, this._hoverSoundVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.CameraShutterSound,
					new LckDiscreetAudioController.AudioClipAndVolume(this._cameraShutterSound, this._cameraShutterSoundVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.ScreenshotBeepSound,
					new LckDiscreetAudioController.AudioClipAndVolume(this._screenshotBeepSound, this._screenshotBeepSoundVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.StreamingStarted,
					new LckDiscreetAudioController.AudioClipAndVolume(this._streamingStarted, this._streamingStartedVolume)
				},
				{
					LckDiscreetAudioController.AudioClip.StreamingStopped,
					new LckDiscreetAudioController.AudioClipAndVolume(this._streamingStopped, this._streamingStoppedVolume)
				}
			};
		}

		private void Start()
		{
			foreach (KeyValuePair<LckDiscreetAudioController.AudioClip, LckDiscreetAudioController.AudioClipAndVolume> keyValuePair in this._allAudioClips)
			{
				this._lckService.PreloadDiscreetAudio(keyValuePair.Value.clip, keyValuePair.Value.volume, false);
			}
		}

		public void PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip clip)
		{
			this._lckService.PlayDiscreetAudioClip(this._allAudioClips[clip].clip);
		}

		[InjectLck]
		private ILckService _lckService;

		private Dictionary<LckDiscreetAudioController.AudioClip, LckDiscreetAudioController.AudioClipAndVolume> _allAudioClips = new Dictionary<LckDiscreetAudioController.AudioClip, LckDiscreetAudioController.AudioClipAndVolume>();

		[Header("Audio Clips")]
		[SerializeField]
		private UnityEngine.AudioClip _recordingStart;

		[SerializeField]
		private UnityEngine.AudioClip _recordingSaved;

		[SerializeField]
		private UnityEngine.AudioClip _clickDown;

		[SerializeField]
		private UnityEngine.AudioClip _clickUp;

		[SerializeField]
		private UnityEngine.AudioClip _hoverSound;

		[SerializeField]
		private UnityEngine.AudioClip _cameraShutterSound;

		[SerializeField]
		private UnityEngine.AudioClip _screenshotBeepSound;

		[SerializeField]
		private UnityEngine.AudioClip _streamingStarted;

		[SerializeField]
		private UnityEngine.AudioClip _streamingStopped;

		[Header("Audio Volumes")]
		[SerializeField]
		[Range(0f, 1f)]
		private float _recordingStartVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _recordingSavedVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _clickDownVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _clickUpVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _hoverSoundVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _cameraShutterSoundVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _screenshotBeepSoundVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _streamingStartedVolume;

		[SerializeField]
		[Range(0f, 1f)]
		private float _streamingStoppedVolume;

		private struct AudioClipAndVolume
		{
			public AudioClipAndVolume(UnityEngine.AudioClip clip, float volume = 0.2f)
			{
				this.clip = clip;
				this.volume = volume;
			}

			public UnityEngine.AudioClip clip;

			public float volume;
		}

		public enum AudioClip
		{
			RecordingStart,
			RecordingSaved,
			ClickDown,
			ClickUp,
			HoverSound,
			CameraShutterSound,
			ScreenshotBeepSound,
			StreamingStarted,
			StreamingStopped
		}
	}
}
