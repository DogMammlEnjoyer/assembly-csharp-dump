using System;
using System.IO;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Lib
{
	public class MicDebug : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this._micSource == null)
			{
				this._micSource = base.gameObject.GetComponentInChildren<IAudioInputSource>();
			}
			if (this._micSource != null)
			{
				this._micSource.OnStartRecording += this.OnStartRecording;
				this._micSource.OnSampleReady += this.OnSampleReady;
				this._micSource.OnStopRecording += this.OnStopRecording;
			}
		}

		private void OnDisable()
		{
			if (this._micSource != null)
			{
				this._micSource.OnStartRecording -= this.OnStartRecording;
				this._micSource.OnSampleReady -= this.OnSampleReady;
				this._micSource.OnStopRecording -= this.OnStopRecording;
			}
		}

		private void OnDestroy()
		{
			this.UnloadStream();
		}

		private void OnStartRecording()
		{
			string text = Application.temporaryCachePath;
			text = text + "/" + this.fileDirectory;
			if (text.EndsWith("/"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			DateTime now = DateTime.Now;
			text = string.Format("{0}/{1}{2:0000}{3:00}{4:00}_{5:00}{6:00}{7:00}.pcm", new object[]
			{
				text,
				this.fileName,
				now.Year,
				now.Month,
				now.Day,
				now.Hour,
				now.Minute,
				now.Second
			});
			VLog.D("MicDebug - Writing recording to file: " + text);
			this._fileStream = File.Open(text, FileMode.Create);
		}

		private void OnSampleReady(int sampleCount, float[] sample, float levelMax)
		{
			if (this._fileStream == null || sample == null)
			{
				return;
			}
			if (this._buffer == null || this._buffer.Length != sample.Length * 2)
			{
				this._buffer = new byte[sample.Length * 2];
			}
			for (int i = 0; i < sample.Length; i++)
			{
				short num = (short)(sample[i] * 32767f);
				this._buffer[i * 2] = (byte)num;
				this._buffer[i * 2 + 1] = (byte)(num >> 8);
			}
			this._fileStream.Write(this._buffer, 0, this._buffer.Length);
		}

		private void OnStopRecording()
		{
			this.UnloadStream();
		}

		private void UnloadStream()
		{
			if (this._fileStream == null)
			{
				return;
			}
			this._fileStream.Close();
			this._fileStream.Dispose();
			this._fileStream = null;
		}

		[SerializeField]
		private IAudioInputSource _micSource;

		[SerializeField]
		private string fileDirectory = "MicClips";

		[SerializeField]
		private string fileName = "mic_debug_";

		private FileStream _fileStream;

		private byte[] _buffer;

		private const int FLOAT_TO_SHORT = 32767;

		private const int BYTES_PER_SHORT = 2;
	}
}
