using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Liv.NativeAudioBridge
{
	public class NativeAudioPlayerWindows : INativeAudioPlayer, IDisposable
	{
		[DllImport("winmm.dll")]
		private static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, NativeAudioPlayerWindows.WaveFormat lpFormat, NativeAudioPlayerWindows.WaveOutProc dwCallback, int dwInstance, int dwFlags);

		[DllImport("winmm.dll")]
		private static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref NativeAudioPlayerWindows.WaveHdr lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		private static extern int waveOutWrite(IntPtr hWaveOut, ref NativeAudioPlayerWindows.WaveHdr lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		private static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref NativeAudioPlayerWindows.WaveHdr lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		private static extern int waveOutClose(IntPtr hWaveOut);

		public void PreloadAudioClip(AudioClip audioClip, float volume, bool forceReload = false)
		{
			this.ValidateAudioClipForPreloading(audioClip);
			this.PreloadAudioClip(audioClip.GetHashCode(), NativeAudioPlayerWindows.PrepareAudioData(audioClip, volume), audioClip.frequency, audioClip.channels, 16, forceReload);
		}

		public void PlayAudioClip(AudioClip audioClip, float volume = 1f)
		{
			NativeAudioPlayerWindows.<>c__DisplayClass15_0 CS$<>8__locals1 = new NativeAudioPlayerWindows.<>c__DisplayClass15_0();
			CS$<>8__locals1.<>4__this = this;
			if (!audioClip)
			{
				throw new InvalidOperationException("LCK: Native Audio can not play AudioClip, audio clip is null.");
			}
			CS$<>8__locals1.audioClipId = audioClip.GetHashCode();
			if (!this._audioClips.ContainsKey(CS$<>8__locals1.audioClipId))
			{
				this.PreloadAudioClip(CS$<>8__locals1.audioClipId, NativeAudioPlayerWindows.PrepareAudioData(audioClip, volume), audioClip.frequency, audioClip.channels, 16, false);
			}
			Task.Run(delegate()
			{
				NativeAudioPlayerWindows.<>c__DisplayClass15_0.<<PlayAudioClip>b__0>d <<PlayAudioClip>b__0>d;
				<<PlayAudioClip>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<PlayAudioClip>b__0>d.<>4__this = CS$<>8__locals1;
				<<PlayAudioClip>b__0>d.<>1__state = -1;
				<<PlayAudioClip>b__0>d.<>t__builder.Start<NativeAudioPlayerWindows.<>c__DisplayClass15_0.<<PlayAudioClip>b__0>d>(ref <<PlayAudioClip>b__0>d);
				return <<PlayAudioClip>b__0>d.<>t__builder.Task;
			});
		}

		public void StopAllAudio()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			foreach (KeyValuePair<int, NativeAudioPlayerWindows.PreloadedAudio> keyValuePair in this._audioClips)
			{
				NativeAudioPlayerWindows.PreloadedAudio value = keyValuePair.Value;
				if (value.DataHandle.IsAllocated)
				{
					value = keyValuePair.Value;
					value.DataHandle.Free();
				}
			}
			this._audioClips.Clear();
			this._disposed = true;
		}

		~NativeAudioPlayerWindows()
		{
			this.Dispose(false);
		}

		private void ValidateAudioClipForPreloading(AudioClip audioClip)
		{
			if (!audioClip)
			{
				throw new InvalidOperationException("Native Audio can not preload AudioClip, audio clip is null.");
			}
		}

		private void PreloadAudioClip(int key, byte[] audioData, int sampleRate, int channels, int bitsPerSample, bool forceReload)
		{
			if (!forceReload && this._audioClips.ContainsKey(key))
			{
				return;
			}
			if (forceReload && this._audioClips.ContainsKey(key))
			{
				this.UnloadAudioClip(key);
			}
			NativeAudioPlayerWindows.WaveFormat format = new NativeAudioPlayerWindows.WaveFormat
			{
				wFormatTag = 1,
				nChannels = (short)channels,
				nSamplesPerSec = sampleRate,
				wBitsPerSample = (short)bitsPerSample,
				nBlockAlign = (short)(channels * bitsPerSample / 8),
				nAvgBytesPerSec = sampleRate * channels * bitsPerSample / 8,
				cbSize = 0
			};
			GCHandle dataHandle = GCHandle.Alloc(audioData, GCHandleType.Pinned);
			this._audioClips[key] = new NativeAudioPlayerWindows.PreloadedAudio
			{
				DataHandle = dataHandle,
				BufferLength = audioData.Length,
				Format = format
			};
		}

		private void UnloadAudioClip(int audioClipKey)
		{
			int hashCode = audioClipKey.GetHashCode();
			if (!this._audioClips.ContainsKey(hashCode))
			{
				throw new InvalidOperationException(string.Format("LCK: Native Audio cannot unload AudioClip ({0}), it is not preloaded.", audioClipKey));
			}
			NativeAudioPlayerWindows.PreloadedAudio preloadedAudio = this._audioClips[hashCode];
			if (preloadedAudio.DataHandle.IsAllocated)
			{
				preloadedAudio.DataHandle.Free();
			}
			this._audioClips.Remove(hashCode);
		}

		private static byte[] PrepareAudioData(AudioClip clip, float volume)
		{
			return NativeAudioPlayerWindows.ConvertAudioClipToByteArray(clip, volume);
		}

		private static byte[] ConvertAudioClipToByteArray(AudioClip clip, float volume)
		{
			float[] array = new float[clip.samples * clip.channels];
			clip.GetData(array, 0);
			byte[] array2 = new byte[array.Length * 2];
			int num = 32767;
			for (int i = 0; i < array.Length; i++)
			{
				short num2 = (short)(Mathf.Clamp(array[i] * volume, -1f, 1f) * (float)num);
				array2[i * 2] = (byte)(num2 & 255);
				array2[i * 2 + 1] = (byte)(((int)num2 & 65280) >> 8);
			}
			return array2;
		}

		private Task PlayAudio(int audioClipId)
		{
			NativeAudioPlayerWindows.PreloadedAudio preloadedAudio = this._audioClips[audioClipId];
			IntPtr hWaveOut;
			if (NativeAudioPlayerWindows.waveOutOpen(out hWaveOut, -1, preloadedAudio.Format, null, 0, 0) != 0)
			{
				throw new InvalidOperationException("Failed to open waveform audio device.");
			}
			NativeAudioPlayerWindows.WaveHdr waveHdr = new NativeAudioPlayerWindows.WaveHdr
			{
				lpData = preloadedAudio.DataHandle.AddrOfPinnedObject(),
				dwBufferLength = preloadedAudio.BufferLength,
				dwFlags = 0,
				dwLoops = 0,
				dwUser = GCHandle.ToIntPtr(preloadedAudio.DataHandle)
			};
			NativeAudioPlayerWindows.waveOutPrepareHeader(hWaveOut, ref waveHdr, Marshal.SizeOf<NativeAudioPlayerWindows.WaveHdr>(waveHdr));
			NativeAudioPlayerWindows.waveOutWrite(hWaveOut, ref waveHdr, Marshal.SizeOf<NativeAudioPlayerWindows.WaveHdr>(waveHdr));
			while ((waveHdr.dwFlags & 1) != 1)
			{
				Thread.Sleep(100);
			}
			NativeAudioPlayerWindows.waveOutUnprepareHeader(hWaveOut, ref waveHdr, Marshal.SizeOf<NativeAudioPlayerWindows.WaveHdr>(waveHdr));
			NativeAudioPlayerWindows.waveOutClose(hWaveOut);
			return Task.CompletedTask;
		}

		private static byte[] _audioByteDataArray;

		private const int BitsPerSample = 16;

		private const string Lib = "winmm.dll";

		private bool _disposed;

		private Dictionary<int, NativeAudioPlayerWindows.PreloadedAudio> _audioClips = new Dictionary<int, NativeAudioPlayerWindows.PreloadedAudio>();

		private delegate void WaveOutProc(IntPtr hwo, int uMsg, int dwInstance, int dwParam1, int dwParam2);

		private struct PreloadedAudio
		{
			public GCHandle DataHandle;

			public NativeAudioPlayerWindows.WaveFormat Format;

			public int BufferLength;
		}

		private struct WaveFormat
		{
			public short wFormatTag;

			public short nChannels;

			public int nSamplesPerSec;

			public int nAvgBytesPerSec;

			public short nBlockAlign;

			public short wBitsPerSample;

			public short cbSize;
		}

		private struct WaveHdr
		{
			public IntPtr lpData;

			public int dwBufferLength;

			public int dwBytesRecorded;

			public IntPtr dwUser;

			public int dwFlags;

			public int dwLoops;

			public IntPtr lpNext;

			public int reserved;
		}
	}
}
