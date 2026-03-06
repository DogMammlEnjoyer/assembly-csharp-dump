using System;
using System.Collections;
using Meta.WitAi.Data;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Lib
{
	public abstract class MicBase : MonoBehaviour, IAudioInputSource
	{
		public abstract string GetMicName();

		public abstract int GetMicSampleRate();

		public abstract AudioClip GetMicClip();

		public abstract int MicPosition { get; }

		public event Action OnStartRecording;

		public event Action OnStartRecordingFailed;

		public event Action OnStopRecording;

		public event Action<int, float[], float> OnSampleReady;

		public bool IsRecording { get; private set; }

		public virtual bool IsMicListening
		{
			get
			{
				return Microphone.IsRecording(this.GetMicName());
			}
		}

		public bool IsInputAvailable
		{
			get
			{
				return this.GetMicClip() != null;
			}
		}

		public AudioEncoding AudioEncoding { get; set; } = new AudioEncoding();

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

		public virtual void CheckForInput()
		{
		}

		public virtual void StartRecording(int sampleDurationMS)
		{
			if (this.IsRecording)
			{
				this.StopRecording();
			}
			if (!this.IsInputAvailable)
			{
				this.OnStartRecordingFailed();
				return;
			}
			this.IsRecording = true;
			this._reader = base.StartCoroutine(this.ReadRawAudio(sampleDurationMS));
		}

		protected virtual IEnumerator ReadRawAudio(int sampleDurationMS)
		{
			Action onStartRecording = this.OnStartRecording;
			if (onStartRecording != null)
			{
				onStartRecording();
			}
			AudioClip micClip = this.GetMicClip();
			this.GetMicName();
			int micSampleRate = this.GetMicSampleRate();
			int num = this.AudioEncoding.samplerate / 1000 * sampleDurationMS * micClip.channels;
			float[] sample = new float[num];
			int loops = 0;
			int readAbsPos = this.MicPosition;
			int prevPos = readAbsPos;
			int micTempTotal = micSampleRate / 1000 * sampleDurationMS * micClip.channels;
			int micDif = micTempTotal / num;
			float[] temp = new float[micTempTotal];
			while (micClip != null && this.IsMicListening && this.IsRecording)
			{
				bool flag = true;
				while (flag && micClip != null)
				{
					int micPosition = this.MicPosition;
					if (micPosition < prevPos)
					{
						int num2 = loops;
						loops = num2 + 1;
					}
					prevPos = micPosition;
					int num3 = loops * micClip.samples + micPosition;
					int num4 = readAbsPos + micTempTotal;
					if (num4 < num3)
					{
						micClip.GetData(temp, readAbsPos % micClip.samples);
						float num5 = 0f;
						int num6 = 0;
						for (int i = 0; i < temp.Length; i++)
						{
							float num7 = temp[i] * temp[i];
							if (num5 < num7)
							{
								num5 = num7;
							}
							if (i % micDif == 0 && num6 < sample.Length)
							{
								sample[num6] = temp[i];
								num6++;
							}
						}
						this._sampleCount++;
						Action<int, float[], float> onSampleReady = this.OnSampleReady;
						if (onSampleReady != null)
						{
							onSampleReady(this._sampleCount, sample, num5);
						}
						readAbsPos = num4;
					}
					else
					{
						flag = false;
					}
				}
				yield return null;
			}
			if (this.IsRecording)
			{
				this.StopRecording();
			}
			yield break;
		}

		public virtual void StopRecording()
		{
			if (!this.IsRecording)
			{
				return;
			}
			this.IsRecording = false;
			if (this._reader != null)
			{
				base.StopCoroutine(this._reader);
				this._reader = null;
			}
			Action onStopRecording = this.OnStopRecording;
			if (onStopRecording == null)
			{
				return;
			}
			onStopRecording();
		}

		private int _sampleCount;

		private Coroutine _reader;
	}
}
