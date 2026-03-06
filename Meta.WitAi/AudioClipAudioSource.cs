using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.Voice.Logging;
using Meta.WitAi;
using Meta.WitAi.Data;
using Meta.WitAi.Interfaces;
using UnityEngine;

public class AudioClipAudioSource : MonoBehaviour, IAudioInputSource
{
	public IVLogger _log { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Audio, null);

	public virtual bool IsMuted { get; private set; }

	public event Action OnMicMuted;

	public event Action OnMicUnmuted;

	protected virtual void SetMuted(bool muted)
	{
		if (this.IsMuted != muted)
		{
			this.IsMuted = muted;
			if (this.IsMuted)
			{
				Action onMicMuted = this.OnMicMuted;
				if (onMicMuted == null)
				{
					return;
				}
				onMicMuted();
				return;
			}
			else
			{
				Action onMicUnmuted = this.OnMicUnmuted;
				if (onMicUnmuted == null)
				{
					return;
				}
				onMicUnmuted();
			}
		}
	}

	private void Start()
	{
		foreach (AudioClip audioClip in this._audioClips)
		{
			this.AddClipData(audioClip);
			VLog.D("Added " + audioClip.name + " to queue");
		}
	}

	public event Action OnStartRecording;

	public event Action OnStartRecordingFailed;

	public event Action<int, float[], float> OnSampleReady;

	public event Action OnStopRecording;

	public void StartRecording(int sampleLen)
	{
		if (!this._isRecording)
		{
			this._isRecording = true;
			this.PlayNextClip();
			return;
		}
		Action onStartRecordingFailed = this.OnStartRecordingFailed;
		if (onStartRecordingFailed == null)
		{
			return;
		}
		onStartRecordingFailed();
	}

	private void PlayNextClip()
	{
		if (this.clipIndex >= this._audioClips.Count && this._loopRequests)
		{
			this.clipIndex = 0;
		}
		if (this.clipIndex < this._audioClips.Count)
		{
			VLog.D(string.Format("Starting clip {0}", this.clipIndex));
			this._isRecording = true;
			VLog.D("Playing " + this._audioClips[this.clipIndex].name);
			this._audioSource.PlayOneShot(this._audioClips[this.clipIndex]);
			Action onStartRecording = this.OnStartRecording;
			if (onStartRecording != null)
			{
				onStartRecording();
			}
			this.TransmitAudio(this.clipData[this.clipIndex]).WrapErrors();
			return;
		}
		Action onStartRecordingFailed = this.OnStartRecordingFailed;
		if (onStartRecordingFailed == null)
		{
			return;
		}
		onStartRecordingFailed();
	}

	private Task TransmitAudio(float[] samples)
	{
		AudioClipAudioSource.<TransmitAudio>d__38 <TransmitAudio>d__;
		<TransmitAudio>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<TransmitAudio>d__.<>4__this = this;
		<TransmitAudio>d__.samples = samples;
		<TransmitAudio>d__.<>1__state = -1;
		<TransmitAudio>d__.<>t__builder.Start<AudioClipAudioSource.<TransmitAudio>d__38>(ref <TransmitAudio>d__);
		return <TransmitAudio>d__.<>t__builder.Task;
	}

	public void StopRecording()
	{
		this._isRecording = false;
		Action onStopRecording = this.OnStopRecording;
		if (onStopRecording == null)
		{
			return;
		}
		onStopRecording();
	}

	public bool IsRecording
	{
		get
		{
			return this._isRecording;
		}
	}

	public AudioEncoding AudioEncoding
	{
		get
		{
			return this._audioEncoding;
		}
	}

	public bool IsInputAvailable
	{
		get
		{
			return true;
		}
	}

	public void CheckForInput()
	{
	}

	public bool SetActiveClip(string clipName)
	{
		int num = this._audioClips.FindIndex(0, (AudioClip clip) => clip.name == clipName);
		if (num < 0 || num >= this._audioClips.Count)
		{
			VLog.D("Couldn't find clip " + clipName);
			return false;
		}
		this.clipIndex = num;
		return true;
	}

	public void AddClip(AudioClip clip)
	{
		this._audioClips.Add(clip);
		this.AddClipData(clip);
		VLog.D("Clip added " + clip.name);
	}

	private void AddClipData(AudioClip clip)
	{
		float[] array = new float[clip.samples];
		clip.GetData(array, 0);
		float[] item = AudioClipAudioSource.QuickResample(array, clip.channels, clip.frequency, this.AudioEncoding.numChannels, this.AudioEncoding.samplerate);
		this.clipData.Add(item);
	}

	public static float[] QuickResample(float[] oldSamples, int oldChannels, int oldSampleRate, int newChannels, int newSampleRate)
	{
		if (oldSampleRate == newSampleRate && oldChannels == newChannels)
		{
			return oldSamples;
		}
		float num = (float)oldSampleRate / (float)newSampleRate;
		num *= (float)oldChannels / (float)newChannels;
		int num2 = (int)((float)oldSamples.Length / num);
		float[] array = new float[num2];
		for (int i = 0; i < num2; i++)
		{
			int num3 = (int)((float)i * num);
			array[i] = oldSamples[num3];
		}
		return array;
	}

	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private List<AudioClip> _audioClips;

	[Tooltip("If true, the associated clips will be played again from the beginning with multiple requests after the clip queue has been exhausted.")]
	[SerializeField]
	private bool _loopRequests;

	private bool _isRecording;

	private Queue<int> _audioQueue = new Queue<int>();

	private int clipIndex;

	private List<float[]> clipData = new List<float[]>();

	private float[] _buffer;

	private const float _samplesPerFrame = 0.01f;

	[SerializeField]
	private AudioEncoding _audioEncoding = new AudioEncoding();
}
