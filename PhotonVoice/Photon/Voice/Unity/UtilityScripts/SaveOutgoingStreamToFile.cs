using System;
using System.IO;
using CSCore;
using CSCore.Codecs.WAV;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	[RequireComponent(typeof(Recorder))]
	[DisallowMultipleComponent]
	public class SaveOutgoingStreamToFile : VoiceComponent
	{
		private void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			VoiceInfo info = photonVoiceCreatedParams.Voice.Info;
			int bits = 32;
			if (photonVoiceCreatedParams.Voice is LocalVoiceAudioShort)
			{
				bits = 16;
			}
			string filePath = this.GetFilePath();
			this.wavWriter = new WaveWriter(filePath, new WaveFormat(info.SamplingRate, bits, info.Channels));
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Outgoing stream, output file path: {0}", new object[]
				{
					filePath
				});
			}
			if (photonVoiceCreatedParams.Voice is LocalVoiceAudioFloat)
			{
				(photonVoiceCreatedParams.Voice as LocalVoiceAudioFloat).AddPreProcessor(new IProcessor<float>[]
				{
					new SaveOutgoingStreamToFile.OutgoingStreamSaverFloat(this.wavWriter)
				});
				return;
			}
			if (photonVoiceCreatedParams.Voice is LocalVoiceAudioShort)
			{
				(photonVoiceCreatedParams.Voice as LocalVoiceAudioShort).AddPreProcessor(new IProcessor<short>[]
				{
					new SaveOutgoingStreamToFile.OutgoingStreamSaverShort(this.wavWriter)
				});
			}
		}

		private string GetFilePath()
		{
			string path = string.Format("out_{0}_{1}.wav", DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-ffff"), Random.Range(0, 1000));
			return Path.Combine(Application.persistentDataPath, path);
		}

		private void PhotonVoiceRemoved()
		{
			this.wavWriter.Dispose();
			if (base.Logger.IsInfoEnabled)
			{
				base.Logger.LogInfo("Recording stopped: Saving wav file.", Array.Empty<object>());
			}
		}

		private WaveWriter wavWriter;

		private class OutgoingStreamSaverFloat : IProcessor<float>, IDisposable
		{
			public OutgoingStreamSaverFloat(WaveWriter waveWriter)
			{
				this.wavWriter = waveWriter;
			}

			public float[] Process(float[] buf)
			{
				this.wavWriter.WriteSamples(buf, 0, buf.Length);
				return buf;
			}

			public void Dispose()
			{
				if (!this.wavWriter.IsDisposed && !this.wavWriter.IsDisposing)
				{
					this.wavWriter.Dispose();
				}
			}

			private WaveWriter wavWriter;
		}

		private class OutgoingStreamSaverShort : IProcessor<short>, IDisposable
		{
			public OutgoingStreamSaverShort(WaveWriter waveWriter)
			{
				this.wavWriter = waveWriter;
			}

			public short[] Process(short[] buf)
			{
				for (int i = 0; i < buf.Length; i++)
				{
					this.wavWriter.Write(buf[i]);
				}
				return buf;
			}

			public void Dispose()
			{
				if (!this.wavWriter.IsDisposed && !this.wavWriter.IsDisposing)
				{
					this.wavWriter.Dispose();
				}
			}

			private WaveWriter wavWriter;
		}
	}
}
