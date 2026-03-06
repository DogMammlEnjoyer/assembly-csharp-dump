using System;
using System.Collections;
using System.Collections.Generic;
using Meta.Voice;
using Meta.Voice.Logging;
using Meta.WitAi.Attributes;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Lib;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.Data
{
	[LogCategory(LogCategory.Audio, LogCategory.Input)]
	public class AudioBuffer : MonoBehaviour
	{
		public static IVLogger _log { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Input, null);

		public void OnApplicationQuit()
		{
			AudioBuffer._isQuitting = true;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void SingletonInit()
		{
			AudioBuffer._isQuitting = false;
		}

		public static AudioBuffer Instance
		{
			get
			{
				if (!AudioBuffer._instance)
				{
					AudioBuffer._instance = Object.FindAnyObjectByType<AudioBuffer>();
					if (!AudioBuffer._instance && AudioBuffer.CanInstantiate())
					{
						if (AudioBuffer.AudioBufferProvider != null)
						{
							AudioBuffer._log.Verbose("No {0} found, creating using provider {1}.", "AudioBuffer", AudioBuffer.AudioBufferProvider.GetType().Name, null, null, "Instance", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 70);
							AudioBuffer._instance = AudioBuffer.AudioBufferProvider.InstantiateAudioBuffer();
						}
						if (!AudioBuffer._instance)
						{
							AudioBuffer._log.Verbose("No {0} found, creating using {0}.", "AudioBuffer", null, null, null, "Instance", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 75);
							AudioBuffer._instance = new GameObject("AudioBuffer").AddComponent<AudioBuffer>();
						}
					}
				}
				return AudioBuffer._instance;
			}
		}

		private static bool CanInstantiate()
		{
			return !AudioBuffer._isQuitting && Application.isPlaying;
		}

		public AudioEncoding AudioEncoding
		{
			get
			{
				return this.audioBufferConfiguration.encoding;
			}
		}

		public AudioBufferEvents Events
		{
			get
			{
				return this.events;
			}
		}

		public IAudioInputSource MicInput
		{
			get
			{
				return this._micInput as IAudioInputSource;
			}
			set
			{
				this.SetInputSource(value);
			}
		}

		private IAudioInputSource FindOrCreateInputSource()
		{
			IAudioInputSource audioInputSource = base.gameObject.GetComponentInChildren<IAudioInputSource>(true);
			if (audioInputSource != null)
			{
				return audioInputSource;
			}
			GameObject[] rootGameObjects = base.gameObject.scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				audioInputSource = rootGameObjects[i].GetComponentInChildren<IAudioInputSource>(true);
				if (audioInputSource != null)
				{
					return audioInputSource;
				}
			}
			if (AudioBuffer.instantiateMic && AudioBuffer.CanInstantiate())
			{
				AudioBuffer._log.Verbose("No input assigned or found, {0} will use Unity Mic Input.", "AudioBuffer", null, null, null, "FindOrCreateInputSource", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 161);
				this._instantiatedMic = base.gameObject.AddComponent<Mic>();
				audioInputSource = this._instantiatedMic;
			}
			return audioInputSource;
		}

		private void SetInputSource(IAudioInputSource newInput)
		{
			if (this.MicInput == newInput)
			{
				return;
			}
			if (this._instantiatedMic && !object.Equals(this._instantiatedMic, newInput))
			{
				AudioBuffer._log.Verbose("Replacing default mic.", null, null, null, null, "SetInputSource", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 180);
				Object.Destroy(this._instantiatedMic);
				this._instantiatedMic = null;
			}
			bool flag = this._recorders.Contains(this);
			if (flag)
			{
				this.StopRecording(this);
			}
			if (this._active)
			{
				this.SetInputDelegates(false);
			}
			Object @object = newInput as Object;
			if (@object != null)
			{
				this._micInput = @object;
				AudioBuffer._log.Verbose("{0} set input of type: {1}", "AudioBuffer", newInput.GetType().Name, null, null, "SetInputSource", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 196);
			}
			else if (newInput == null)
			{
				AudioBuffer._log.Warning("{0} setting MicInput to null instead of {1}", new object[]
				{
					"AudioBuffer",
					"IAudioInputSource"
				});
			}
			else
			{
				AudioBuffer._log.Error("{0} cannot set MicInput of type '{1}' since it does not inherit from {2}", new object[]
				{
					"AudioBuffer",
					newInput.GetType().Name,
					"Object"
				});
			}
			IAudioLevelRangeProvider audioLevelRangeProvider = this._micInput as IAudioLevelRangeProvider;
			if (audioLevelRangeProvider != null)
			{
				this._micLevelRange = audioLevelRangeProvider;
			}
			if (this._active)
			{
				this.SetInputDelegates(true);
			}
			if (flag)
			{
				this.StartRecording(this);
			}
		}

		private void SetInputDelegates(bool add)
		{
			IAudioInputSource micInput = this.MicInput;
			if (micInput == null)
			{
				return;
			}
			if (add)
			{
				micInput.OnStartRecording += this.OnMicRecordSuccess;
				micInput.OnStartRecordingFailed += this.OnMicRecordFailed;
				micInput.OnStopRecording += this.OnMicRecordStop;
				micInput.OnSampleReady += this.OnMicSampleReady;
				return;
			}
			micInput.OnStartRecording -= this.OnMicRecordSuccess;
			micInput.OnStartRecordingFailed -= this.OnMicRecordFailed;
			micInput.OnStopRecording -= this.OnMicRecordStop;
			micInput.OnSampleReady -= this.OnMicSampleReady;
		}

		public float MicMinAudioLevel
		{
			get
			{
				if (this._micLevelRange != null)
				{
					return this._micLevelRange.MinAudioLevel;
				}
				return 0.5f;
			}
		}

		public float MicMaxAudioLevel
		{
			get
			{
				if (this._micLevelRange != null)
				{
					return this._micLevelRange.MaxAudioLevel;
				}
				return 1f;
			}
		}

		public bool IsInputAvailable
		{
			get
			{
				return this.MicInput != null;
			}
		}

		public VoiceAudioInputState AudioState { get; private set; }

		public float MicMaxLevel { get; private set; } = -1f;

		public bool IsRecording(Component component)
		{
			return this._recorders.Contains(component);
		}

		private void Awake()
		{
			AudioBuffer._instance = this;
			this.InitializeMicDataBuffer();
		}

		private void OnDestroy()
		{
			if (AudioBuffer._instance == this)
			{
				AudioBuffer._instance = null;
			}
		}

		private void OnEnable()
		{
			if (AudioBuffer._instance && AudioBuffer._instance != this)
			{
				AudioBuffer._log.Error("Multiple {0} detected. This can lead to extra memory use and unexpected results. Duplicate was found on {1}", new object[]
				{
					"AudioBuffer",
					base.name
				});
			}
			if (base.name != "AudioBuffer")
			{
				AudioBuffer._log.Verbose("{0} active on {1}", "AudioBuffer", base.name, null, null, "OnEnable", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 332);
			}
			if (this.MicInput == null)
			{
				this.MicInput = this.FindOrCreateInputSource();
			}
			this._active = true;
			this.SetInputDelegates(true);
			if (this.alwaysRecording)
			{
				this.StartRecording(this);
			}
		}

		private void OnDisable()
		{
			if (this.alwaysRecording)
			{
				this.StopRecording(this);
			}
			this._active = false;
			this.SetInputDelegates(false);
		}

		private void SetAudioState(VoiceAudioInputState newAudioState)
		{
			this.AudioState = newAudioState;
			if (this.AudioState == VoiceAudioInputState.On)
			{
				this.StopUpdateVolume();
				this.MicMaxLevel = -1f;
				this._volumeUpdate = base.StartCoroutine(this.UpdateVolume());
			}
			else if (this.AudioState == VoiceAudioInputState.Off)
			{
				this.StopUpdateVolume();
			}
			Action<VoiceAudioInputState> onAudioStateChange = this.Events.OnAudioStateChange;
			if (onAudioStateChange == null)
			{
				return;
			}
			onAudioStateChange(this.AudioState);
		}

		public void StartRecording(Component component)
		{
			if (this._recorders.Contains(component))
			{
				return;
			}
			this._recorders.Add(component);
			if (this.AudioState != VoiceAudioInputState.Off && this.AudioState != VoiceAudioInputState.Deactivating)
			{
				if (this.AudioState == VoiceAudioInputState.On)
				{
					this.OnMicRecordStarted(component);
				}
				return;
			}
			this._totalSampleChunks = 0;
			this.SetAudioState(VoiceAudioInputState.Activating);
			if (!this.MicInput.IsRecording)
			{
				this.MicInput.StartRecording(this.audioBufferConfiguration.sampleLengthInMs);
				return;
			}
			this.OnMicRecordSuccess();
		}

		private void OnMicRecordSuccess()
		{
			this.SetAudioState(VoiceAudioInputState.On);
			foreach (Component component in this._recorders)
			{
				this.OnMicRecordStarted(component);
			}
		}

		private void OnMicRecordStarted(Component component)
		{
			IVoiceEventProvider voiceEventProvider = component as IVoiceEventProvider;
			if (voiceEventProvider != null)
			{
				UnityEvent onMicStartedListening = voiceEventProvider.VoiceEvents.OnMicStartedListening;
				if (onMicStartedListening == null)
				{
					return;
				}
				onMicStartedListening.Invoke();
			}
		}

		private void OnMicRecordFailed()
		{
			this.OnMicRecordStop();
		}

		public void StopRecording(Component component)
		{
			if (!this._recorders.Contains(component))
			{
				return;
			}
			if (this.AudioState != VoiceAudioInputState.On && this.AudioState != VoiceAudioInputState.Activating)
			{
				if (this.AudioState == VoiceAudioInputState.Off)
				{
					this.OnMicRecordStopped(component);
					this._recorders.Remove(component);
				}
				return;
			}
			this.SetAudioState(VoiceAudioInputState.Deactivating);
			if (this.MicInput.IsRecording)
			{
				this.MicInput.StopRecording();
				return;
			}
			this.OnMicRecordStop();
		}

		private void OnMicRecordStop()
		{
			HashSet<Component> recorders = this._recorders;
			this._recorders = new HashSet<Component>();
			foreach (Component component in recorders)
			{
				this.OnMicRecordStopped(component);
			}
			this.SetAudioState(VoiceAudioInputState.Off);
		}

		private void OnMicRecordStopped(Component component)
		{
			IVoiceEventProvider voiceEventProvider = component as IVoiceEventProvider;
			if (voiceEventProvider != null)
			{
				UnityEvent onMicStoppedListening = voiceEventProvider.VoiceEvents.OnMicStoppedListening;
				if (onMicStoppedListening == null)
				{
					return;
				}
				onMicStoppedListening.Invoke();
			}
		}

		private void InitializeMicDataBuffer()
		{
			if (this.AudioEncoding.numChannels != 1)
			{
				VLog.E(base.GetType().Name, string.Format("{0} audio channels are not currently supported", this.AudioEncoding.numChannels), null);
				this.AudioEncoding.numChannels = 1;
			}
			if (!string.Equals(this.AudioEncoding.encoding, "signed-integer") && !string.Equals(this.AudioEncoding.encoding, "unsigned-integer"))
			{
				VLog.E(base.GetType().Name, this.AudioEncoding.encoding + " encoding is not currently supported", null);
				this.AudioEncoding.encoding = "signed-integer";
			}
			if (this.AudioEncoding.bits != 8 && this.AudioEncoding.bits != 16 && this.AudioEncoding.bits != 32 && this.AudioEncoding.bits != 64)
			{
				VLog.E(base.GetType().Name, string.Format("{0} bit audio encoding is not currently supported", this.AudioEncoding.bits), null);
				this.AudioEncoding.bits = 16;
			}
			float num = Mathf.Max(10f, this.audioBufferConfiguration.micBufferLengthInSeconds * 1000f);
			if (this._outputBuffer == null)
			{
				int capacity = this.AudioEncoding.numChannels * this.AudioEncoding.samplerate * Mathf.CeilToInt((float)this.AudioEncoding.bits / 8f * num);
				this._outputBuffer = new RingBuffer<byte>(capacity);
			}
		}

		private void OnMicSampleReady(int sampleCount, float[] samples, float levelMax)
		{
			this.OnAudioSampleReady(samples, 0, samples.Length);
		}

		private void OnAudioSampleReady(float[] samples, int offset, int length)
		{
			if (this._sampleReadyCoroutine == null)
			{
				this._sampleReadyCoroutine = base.StartCoroutine(this.WaitForSampleReady());
			}
			if (this._sampleReadyMarker == null)
			{
				this._sampleReadyMarker = this.CreateMarker();
				this._sampleReadyMaxLevel = float.MinValue;
			}
			float num = this.EncodeAndPush(samples, offset, length);
			this.MicMaxLevel = Mathf.Max(num, this.MicMaxLevel);
			WitSampleEvent onSampleReceived = this.events.OnSampleReceived;
			if (onSampleReceived != null)
			{
				onSampleReceived.Invoke(samples, this._totalSampleChunks, num);
			}
			this._totalSampleChunks++;
			if (num > this._sampleReadyMaxLevel)
			{
				this._sampleReadyMaxLevel = num;
			}
		}

		private IEnumerator WaitForSampleReady()
		{
			while (this.AudioState == VoiceAudioInputState.On)
			{
				if (Application.isPlaying && !Application.isBatchMode)
				{
					yield return new WaitForEndOfFrame();
				}
				else
				{
					yield return null;
				}
				if (this._sampleReadyMarker != null)
				{
					RingBuffer<byte>.Marker sampleReadyMarker = this._sampleReadyMarker;
					this._sampleReadyMarker = null;
					this.CallSampleReady(sampleReadyMarker);
				}
			}
			this._sampleReadyCoroutine = null;
			yield break;
		}

		private void CallSampleReady(RingBuffer<byte>.Marker marker)
		{
			if (this.events.OnByteDataReady != null)
			{
				marker.Clone().ReadIntoWriters(new RingBuffer<byte>.ByteDataWriter[]
				{
					new RingBuffer<byte>.ByteDataWriter(this.events.OnByteDataReady.Invoke)
				});
			}
			AudioBufferEvents.OnSampleReadyEvent onSampleReady = this.events.OnSampleReady;
			if (onSampleReady == null)
			{
				return;
			}
			onSampleReady(marker, this._sampleReadyMaxLevel);
		}

		private IEnumerator UpdateVolume()
		{
			float volume = -1f;
			for (;;)
			{
				if (Application.isBatchMode)
				{
					yield return null;
				}
				else
				{
					yield return new WaitForEndOfFrame();
				}
				if (!volume.Equals(this.MicMaxLevel) && !this.MicMaxLevel.Equals(-1f))
				{
					volume = this.MicMaxLevel;
					WitMicLevelChangedEvent onMicLevelChanged = this.events.OnMicLevelChanged;
					if (onMicLevelChanged != null)
					{
						onMicLevelChanged.Invoke(volume);
					}
					this.MicMaxLevel = -1f;
				}
			}
			yield break;
		}

		private void StopUpdateVolume()
		{
			this.MicMaxLevel = -1f;
			if (this._volumeUpdate != null)
			{
				base.StopCoroutine(this._volumeUpdate);
				this._volumeUpdate = null;
			}
		}

		private float EncodeAndPush(float[] samples, int offset, int length)
		{
			if (this.MicInput.AudioEncoding.samplerate > 0)
			{
				IAudioVariableSampleRate audioVariableSampleRate = this.MicInput as IAudioVariableSampleRate;
				if (audioVariableSampleRate == null || !audioVariableSampleRate.NeedsSampleRateCalculation)
				{
					goto IL_4A;
				}
			}
			this.UpdateSampleRate(length);
			if (this.MicInput.AudioEncoding.samplerate <= 0)
			{
				return 0f;
			}
			IL_4A:
			AudioEncoding audioEncoding = this.MicInput.AudioEncoding;
			int numChannels = audioEncoding.numChannels;
			int samplerate = audioEncoding.samplerate;
			bool flag = string.Equals(audioEncoding.encoding, "signed-integer");
			AudioEncoding audioEncoding2 = this.AudioEncoding;
			int samplerate2 = audioEncoding2.samplerate;
			int num = Mathf.CeilToInt((float)audioEncoding2.bits / 8f);
			long num2;
			long num3;
			this.GetEncodingMinMax(audioEncoding2.bits, string.Equals(audioEncoding2.encoding, "signed-integer"), out num2, out num3);
			long num4 = num3 - num2;
			float num5 = (samplerate == samplerate2) ? 1f : ((float)samplerate / (float)samplerate2);
			num5 *= (float)numChannels;
			int num6 = (int)((float)length / num5);
			float num7 = 0f;
			for (int i = 0; i < num6; i++)
			{
				int num8 = offset + (int)((float)i * num5);
				float num9 = samples[num8];
				if (flag)
				{
					num9 = num9 / 2f + 0.5f;
				}
				if (num9 > num7)
				{
					num7 = num9;
				}
				long num10 = (long)((float)num2 + num9 * (float)num4);
				for (int j = 0; j < num; j++)
				{
					byte data = (byte)(num10 >> j * 8);
					this._outputBuffer.Push(data);
				}
			}
			float micMinAudioLevel = this.MicMinAudioLevel;
			float micMaxAudioLevel = this.MicMaxAudioLevel;
			if ((!micMinAudioLevel.Equals(0f) || !micMaxAudioLevel.Equals(1f)) && micMaxAudioLevel > micMinAudioLevel)
			{
				num7 = (num7 - micMinAudioLevel) / (micMaxAudioLevel - micMinAudioLevel);
			}
			return Mathf.Clamp01(num7);
		}

		private void GetEncodingMinMax(int bits, bool signed, out long encodingMin, out long encodingMax)
		{
			if (bits <= 16)
			{
				if (bits == 8)
				{
					encodingMin = 0L;
					encodingMax = 255L;
					return;
				}
				if (bits != 16)
				{
				}
			}
			else
			{
				if (bits == 32)
				{
					encodingMin = (signed ? -2147483648L : 0L);
					encodingMax = (long)(signed ? 2147483647UL : 18446744073709551615UL);
					return;
				}
				if (bits == 64)
				{
					encodingMin = long.MinValue;
					encodingMax = long.MaxValue;
					return;
				}
			}
			encodingMin = (signed ? -32768L : 0L);
			encodingMax = (signed ? 32767L : 65535L);
		}

		public RingBuffer<byte>.Marker CreateMarker()
		{
			return this._outputBuffer.CreateMarker(0);
		}

		public RingBuffer<byte>.Marker CreateMarker(float offset)
		{
			int offset2 = (int)((float)(this.AudioEncoding.numChannels * this.AudioEncoding.samplerate) * offset);
			return this._outputBuffer.CreateMarker(offset2);
		}

		private void UpdateSampleRate(int sampleLength)
		{
			if (sampleLength <= 0)
			{
				return;
			}
			long ticks = DateTimeOffset.Now.Ticks;
			long num = ticks - this._lastSampleTime;
			this._lastSampleTime = ticks;
			if (num > 500000L || this._startSampleTime == 0L)
			{
				this._startSampleTime = ticks;
				this._measureSampleTotal = 0L;
				return;
			}
			int numChannels = this.MicInput.AudioEncoding.numChannels;
			this._measureSampleTotal += (long)Mathf.FloorToInt((float)sampleLength / (float)numChannels);
			long num2 = ticks - this._startSampleTime;
			if (num2 < 2500000L)
			{
				return;
			}
			double num3 = (double)num2 / 10000000.0;
			double num4 = (double)this._measureSampleTotal / num3;
			int num5 = this._measuredSampleRateCount % 20;
			this._measuredSampleRates[num5] = num4;
			this._measuredSampleRateCount++;
			if (this._measuredSampleRateCount == 40)
			{
				this._measuredSampleRateCount -= 20;
			}
			double averageSampleRate = AudioBuffer.GetAverageSampleRate(this._measuredSampleRates, this._measuredSampleRateCount);
			int closestSampleRate = AudioBuffer.GetClosestSampleRate(averageSampleRate);
			if (this.MicInput.AudioEncoding.samplerate != closestSampleRate)
			{
				this.MicInput.AudioEncoding.samplerate = closestSampleRate;
				AudioBuffer._log.Info("Input SampleRate Set: {0}\nElapsed: {1:0.000} seconds\nAverage Samples per Second: {2}", closestSampleRate, num3, averageSampleRate, null, "UpdateSampleRate", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Data\\AudioBuffer.cs", 932);
			}
			this._startSampleTime = ticks;
			this._measureSampleTotal = 0L;
		}

		private static double GetAverageSampleRate(double[] sampleRates, int sampleRateCount)
		{
			int num = Mathf.Min(sampleRateCount, sampleRates.Length);
			if (num <= 0)
			{
				return 0.0;
			}
			double num2 = 0.0;
			for (int i = 0; i < num; i++)
			{
				num2 += sampleRates[i];
			}
			return num2 / (double)num;
		}

		private static int GetClosestSampleRate(double samplesPerSecond)
		{
			int result = 0;
			int num = int.MaxValue;
			int num2 = (int)Math.Round(samplesPerSecond);
			for (int i = 0; i < AudioBuffer.ALLOWED_SAMPLE_RATES.Length; i++)
			{
				int num3 = AudioBuffer.ALLOWED_SAMPLE_RATES[i];
				int num4 = Mathf.Abs(num3 - num2);
				if (num4 >= num)
				{
					return result;
				}
				result = num3;
				num = num4;
			}
			return result;
		}

		private const string DEFAULT_OBJECT_NAME = "AudioBuffer";

		private static bool _isQuitting = false;

		public static bool instantiateMic = true;

		private static AudioBuffer _instance;

		public static IAudioBufferProvider AudioBufferProvider;

		[Tooltip("If set to true, the audio buffer will always be recording.")]
		[SerializeField]
		private bool alwaysRecording;

		[Tooltip("Configuration settings for the audio buffer.")]
		[SerializeField]
		private AudioBufferConfiguration audioBufferConfiguration = new AudioBufferConfiguration();

		[TooltipBox("Events triggered when AudioBuffer processes and receives audio data.")]
		[SerializeField]
		private AudioBufferEvents events = new AudioBufferEvents();

		[ObjectType(typeof(IAudioInputSource), new Type[]
		{

		})]
		[SerializeField]
		private Object _micInput;

		private IAudioLevelRangeProvider _micLevelRange;

		private bool _active;

		private Mic _instantiatedMic;

		private int _totalSampleChunks;

		private RingBuffer<byte> _outputBuffer;

		private HashSet<Component> _recorders = new HashSet<Component>();

		private const float MIC_RESET = -1f;

		private Coroutine _volumeUpdate;

		private Coroutine _sampleReadyCoroutine;

		private RingBuffer<byte>.Marker _sampleReadyMarker;

		private float _sampleReadyMaxLevel;

		private long _lastSampleTime;

		private long _startSampleTime;

		private long _measureSampleTotal;

		private int _measuredSampleRateCount;

		private readonly double[] _measuredSampleRates = new double[20];

		private const int TIMEOUT_TICKS = 500000;

		private const int MEASURE_TICKS = 2500000;

		private const int MEASURE_AVERAGE_COUNT = 20;

		private static readonly int[] ALLOWED_SAMPLE_RATES = new int[]
		{
			8000,
			11025,
			16000,
			22050,
			32000,
			44100,
			48000,
			88200,
			96000,
			176400,
			192000
		};
	}
}
