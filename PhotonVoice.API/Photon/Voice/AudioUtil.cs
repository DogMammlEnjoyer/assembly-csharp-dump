using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace Photon.Voice
{
	public static class AudioUtil
	{
		public static void Resample<T>(T[] src, T[] dst, int dstCount, int channels)
		{
			if (channels == 1)
			{
				for (int i = 0; i < dstCount; i++)
				{
					dst[i] = src[i * src.Length / dstCount];
				}
				return;
			}
			if (channels == 2)
			{
				for (int j = 0; j < dstCount / 2; j++)
				{
					int num = j * src.Length / dstCount;
					int num2 = j * 2;
					int num3 = num * 2;
					dst[num2++] = src[num3++];
					dst[num2] = src[num3];
				}
				return;
			}
			for (int k = 0; k < dstCount / channels; k++)
			{
				int num4 = k * src.Length / dstCount;
				int num5 = k * channels;
				int num6 = num4 * channels;
				for (int l = 0; l < channels; l++)
				{
					dst[num5++] = src[num6++];
				}
			}
		}

		public static void Resample<T>(T[] src, int srcOffset, int srcCount, T[] dst, int dstOffset, int dstCount, int channels)
		{
			if (channels == 1)
			{
				for (int i = 0; i < dstCount; i++)
				{
					dst[dstOffset + i] = src[srcOffset + i * srcCount / dstCount];
				}
				return;
			}
			if (channels == 2)
			{
				for (int j = 0; j < dstCount / 2; j++)
				{
					int num = j * srcCount / dstCount;
					int num2 = j * 2;
					int num3 = num * 2;
					dst[dstOffset + num2++] = src[srcOffset + num3++];
					dst[dstOffset + num2] = src[srcOffset + num3];
				}
				return;
			}
			for (int k = 0; k < dstCount / channels; k++)
			{
				int num4 = k * srcCount / dstCount;
				int num5 = k * channels;
				int num6 = num4 * channels;
				for (int l = 0; l < channels; l++)
				{
					dst[dstOffset + num5++] = src[srcOffset + num6++];
				}
			}
		}

		public static void Resample<T>(T[] src, int srcOffset, int srcCount, int srcChannels, T[] dst, int dstOffset, int dstCount, int dstChannels)
		{
			if (srcChannels == dstChannels)
			{
				AudioUtil.Resample<T>(src, srcOffset, srcCount, dst, dstOffset, dstCount, dstChannels);
				return;
			}
			if (srcChannels == 1 && dstChannels == 2)
			{
				int i = 0;
				int num = 0;
				while (i < dstCount / 2)
				{
					T t = src[srcOffset + i * srcCount * 2 / dstCount];
					dst[dstOffset + num++] = t;
					dst[dstOffset + num++] = t;
					i++;
				}
				return;
			}
			if (srcChannels == 2 && dstChannels == 1)
			{
				for (int j = 0; j < dstCount; j++)
				{
					dst[dstOffset + j] = src[srcOffset + j * srcCount / dstCount / 2 * 2];
				}
				return;
			}
			int k = 0;
			int num2 = 0;
			while (k < dstCount / dstChannels)
			{
				int num3 = srcOffset + k * srcCount * dstChannels / dstCount / srcChannels * srcChannels;
				if (srcChannels >= dstChannels)
				{
					for (int l = 0; l < dstChannels; l++)
					{
						dst[dstOffset + num2++] = src[num3 + l];
					}
				}
				else
				{
					for (int m = 0; m < srcChannels; m++)
					{
						dst[dstOffset + num2++] = src[num3 + m];
					}
					num2 += dstChannels - srcChannels;
				}
				k++;
			}
		}

		public static void ResampleAndConvert(short[] src, float[] dst, int dstCount, int channels)
		{
			if (channels == 1)
			{
				for (int i = 0; i < dstCount; i++)
				{
					dst[i] = (float)src[i * src.Length / dstCount] / 32767f;
				}
				return;
			}
			if (channels == 2)
			{
				for (int j = 0; j < dstCount / 2; j++)
				{
					int num = j * src.Length / dstCount;
					int num2 = j * 2;
					int num3 = num * 2;
					dst[num2++] = (float)src[num3++] / 32767f;
					dst[num2] = (float)src[num3] / 32767f;
				}
				return;
			}
			for (int k = 0; k < dstCount / channels; k++)
			{
				int num4 = k * src.Length / dstCount;
				int num5 = k * channels;
				int num6 = num4 * channels;
				for (int l = 0; l < channels; l++)
				{
					dst[num5++] = (float)src[num6++] / 32767f;
				}
			}
		}

		public static void ResampleAndConvert(float[] src, short[] dst, int dstCount, int channels)
		{
			if (channels == 1)
			{
				for (int i = 0; i < dstCount; i++)
				{
					dst[i] = (short)(src[i * src.Length / dstCount] * 32767f);
				}
				return;
			}
			if (channels == 2)
			{
				for (int j = 0; j < dstCount / 2; j++)
				{
					int num = j * src.Length / dstCount;
					int num2 = j * 2;
					int num3 = num * 2;
					dst[num2++] = (short)(src[num3++] * 32767f);
					dst[num2] = (short)(src[num3] * 32767f);
				}
				return;
			}
			for (int k = 0; k < dstCount / channels; k++)
			{
				int num4 = k * src.Length / dstCount;
				int num5 = k * channels;
				int num6 = num4 * channels;
				for (int l = 0; l < channels; l++)
				{
					dst[num5++] = (short)(src[num6++] * 32767f);
				}
			}
		}

		public static void Convert(float[] src, short[] dst, int dstCount)
		{
			for (int i = 0; i < dstCount; i++)
			{
				dst[i] = (short)(src[i] * 32767f);
			}
		}

		public static void Convert(short[] src, float[] dst, int dstCount)
		{
			for (int i = 0; i < dstCount; i++)
			{
				dst[i] = (float)src[i] / 32767f;
			}
		}

		public static void ForceToStereo<T>(T[] src, T[] dst, int srcChannels)
		{
			int num = 0;
			for (int i = 0; i < dst.Length - 1; i += 2)
			{
				dst[i] = src[num];
				dst[i + 1] = ((srcChannels > 1) ? src[num + 1] : src[num]);
				num += srcChannels;
			}
		}

		internal static string tostr<T>(T[] x, int lim = 10)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ((x.Length < lim) ? x.Length : lim); i++)
			{
				stringBuilder.Append("-");
				stringBuilder.Append(x[i]);
			}
			return stringBuilder.ToString();
		}

		public class ToneAudioReader<T> : IAudioReader<T>, IDataReader<T>, IDisposable, IAudioDesc
		{
			public ToneAudioReader(Func<double> clockSec = null, double frequency = 440.0, int samplingRate = 48000, int channels = 2)
			{
				Func<double> func;
				if (clockSec != null)
				{
					func = clockSec;
				}
				else
				{
					func = (() => (double)DateTime.Now.Ticks / 10000000.0);
				}
				this.clockSec = func;
				this.samplingRate = samplingRate;
				this.channels = channels;
				this.k = 6.283185307179586 * frequency / (double)this.SamplingRate;
			}

			public int Channels
			{
				get
				{
					return this.channels;
				}
			}

			public int SamplingRate
			{
				get
				{
					return this.samplingRate;
				}
			}

			public string Error { get; private set; }

			public void Dispose()
			{
			}

			public bool Read(T[] buf)
			{
				int num = buf.Length / this.Channels;
				long num2 = (long)(this.clockSec() * (double)this.SamplingRate);
				long num3 = num2 - this.timeSamples;
				if (Math.Abs(num3) > (long)(this.SamplingRate / 4))
				{
					num3 = (long)num;
					this.timeSamples = num2 - (long)num;
				}
				if (num3 < (long)num)
				{
					return false;
				}
				int num4 = 0;
				if (buf is float[])
				{
					for (int i = 0; i < num; i++)
					{
						float[] array = buf as float[];
						long num5 = this.timeSamples;
						this.timeSamples = num5 + 1L;
						float num6 = (float)(Math.Sin((double)num5 * this.k) * 0.20000000298023224);
						for (int j = 0; j < this.Channels; j++)
						{
							array[num4++] = num6;
						}
					}
				}
				else if (buf is short[])
				{
					short[] array2 = buf as short[];
					for (int k = 0; k < num; k++)
					{
						long num5 = this.timeSamples;
						this.timeSamples = num5 + 1L;
						short num7 = (short)(Math.Sin((double)num5 * this.k) * 6553.39990234375);
						for (int l = 0; l < this.Channels; l++)
						{
							array2[num4++] = num7;
						}
					}
				}
				return true;
			}

			private double k;

			private long timeSamples;

			private Func<double> clockSec;

			private int samplingRate;

			private int channels;
		}

		public class ToneAudioPusher<T> : IAudioPusher<T>, IAudioDesc, IDisposable
		{
			public ToneAudioPusher(int frequency = 440, int bufSizeMs = 100, int samplingRate = 48000, int channels = 2)
			{
				this.samplingRate = samplingRate;
				this.channels = channels;
				this.bufSizeSamples = bufSizeMs * this.SamplingRate / 1000;
				this.k = 6.283185307179586 * (double)frequency / (double)this.SamplingRate;
			}

			public void SetCallback(Action<T[]> callback, ObjectFactory<T[], int> bufferFactory)
			{
				if (this.timer != null)
				{
					this.Dispose();
				}
				this.callback = callback;
				this.bufferFactory = bufferFactory;
				this.timer = new Timer(1000.0 * (double)this.bufSizeSamples / (double)this.SamplingRate);
				this.timer.Elapsed += this.OnTimedEvent;
				this.timer.Enabled = true;
			}

			private void OnTimedEvent(object source, ElapsedEventArgs e)
			{
				T[] array = this.bufferFactory.New(this.bufSizeSamples * this.Channels);
				int num = 0;
				if (array is float[])
				{
					float[] array2 = array as float[];
					for (int i = 0; i < this.bufSizeSamples; i++)
					{
						float num2 = (float)(Math.Sin((double)(this.posSamples + i) * this.k) / 2.0);
						for (int j = 0; j < this.Channels; j++)
						{
							array2[num++] = num2;
						}
					}
				}
				else if (array is short[])
				{
					short[] array3 = array as short[];
					for (int k = 0; k < this.bufSizeSamples; k++)
					{
						short num3 = (short)(Math.Sin((double)(this.posSamples + k) * this.k) * 32767.0 / 2.0);
						for (int l = 0; l < this.Channels; l++)
						{
							array3[num++] = num3;
						}
					}
				}
				this.cntFrame++;
				this.posSamples += this.bufSizeSamples;
				this.callback(array);
			}

			public int Channels
			{
				get
				{
					return this.channels;
				}
			}

			public int SamplingRate
			{
				get
				{
					return this.samplingRate;
				}
			}

			public string Error { get; private set; }

			public void Dispose()
			{
				if (this.timer != null)
				{
					this.timer.Close();
				}
			}

			private double k;

			private Timer timer;

			private Action<T[]> callback;

			private ObjectFactory<T[], int> bufferFactory;

			private int cntFrame;

			private int posSamples;

			private int bufSizeSamples;

			private int samplingRate;

			private int channels;
		}

		public class TempoUp<T>
		{
			public void Begin(int channels, int changePerc, int skipGroup)
			{
				this.channels = channels;
				this.skipFactor = 100 / changePerc;
				this.skipGroup = skipGroup;
				this.sign = 0;
				this.skipping = false;
				this.waveCnt = 0;
			}

			public int Process(T[] s, T[] d)
			{
				if (this.sizeofT == 2)
				{
					return this.processShort(s as short[], d as short[]);
				}
				return this.processFloat(s as float[], d as float[]);
			}

			public int End(T[] s)
			{
				if (!this.skipping)
				{
					return 0;
				}
				if (this.sizeofT == 2)
				{
					return this.endShort(s as short[]);
				}
				return this.endFloat(s as float[]);
			}

			private int processFloat(float[] s, float[] d)
			{
				int num = 0;
				if (this.channels == 1)
				{
					for (int i = 0; i < s.Length; i++)
					{
						if (s[i] < 0f)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							this.sign = 0;
						}
						if (!this.skipping)
						{
							d[num++] = s[i];
						}
					}
				}
				else if (this.channels == 2)
				{
					for (int j = 0; j < s.Length; j += 2)
					{
						if (s[j] + s[j + 1] < 0f)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							this.sign = 0;
						}
						if (!this.skipping)
						{
							d[num++] = s[j];
							d[num++] = s[j + 1];
						}
					}
				}
				else
				{
					for (int k = 0; k < s.Length; k += this.channels)
					{
						float num2 = s[k] + s[k + 1];
						int num3 = 2;
						while (k < this.channels)
						{
							num2 += s[k + num3];
							num3++;
						}
						if (num2 < 0f)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							this.sign = 0;
						}
						if (!this.skipping)
						{
							d[num++] = s[k];
							d[num++] = s[k + 1];
							int num4 = 2;
							while (k < this.channels)
							{
								d[num++] += s[k + num4];
								num4++;
							}
						}
					}
				}
				return num / this.channels;
			}

			public int endFloat(float[] s)
			{
				if (this.channels == 1)
				{
					for (int i = 0; i < s.Length; i++)
					{
						if (s[i] < 0f)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							if (!this.skipping)
							{
								return i;
							}
							this.sign = 0;
						}
					}
				}
				else if (this.channels == 2)
				{
					for (int j = 0; j < s.Length; j += 2)
					{
						if (s[j] + s[j + 1] < 0f)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							if (!this.skipping)
							{
								return j / 2;
							}
							this.sign = 0;
						}
					}
				}
				else
				{
					for (int k = 0; k < s.Length; k += this.channels)
					{
						float num = s[k] + s[k + 1];
						int num2 = 2;
						while (k < this.channels)
						{
							num += s[k + num2];
							num2++;
						}
						if (num < 0f)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							if (!this.skipping)
							{
								return k / this.channels;
							}
							this.sign = 0;
						}
					}
				}
				return 0;
			}

			private int processShort(short[] s, short[] d)
			{
				int num = 0;
				if (this.channels == 1)
				{
					for (int i = 0; i < s.Length; i++)
					{
						if (s[i] < 0)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							this.sign = 0;
						}
						if (!this.skipping)
						{
							d[num++] = s[i];
						}
					}
				}
				else if (this.channels == 2)
				{
					for (int j = 0; j < s.Length; j += 2)
					{
						if (s[j] + s[j + 1] < 0)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							this.sign = 0;
						}
						if (!this.skipping)
						{
							d[num++] = s[j];
							d[num++] = s[j + 1];
						}
					}
				}
				else
				{
					for (int k = 0; k < s.Length; k += this.channels)
					{
						int num2 = (int)(s[k] + s[k + 1]);
						int num3 = 2;
						while (k < this.channels)
						{
							num2 += (int)s[k + num3];
							num3++;
						}
						if (num2 < 0)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							this.sign = 0;
						}
						if (!this.skipping)
						{
							d[num++] = s[k];
							d[num++] = s[k + 1];
							int num4 = 2;
							while (k < this.channels)
							{
								int num5 = num++;
								d[num5] += s[k + num4];
								num4++;
							}
						}
					}
				}
				return num / this.channels;
			}

			public int endShort(short[] s)
			{
				if (this.channels == 1)
				{
					for (int i = 0; i < s.Length; i++)
					{
						if (s[i] < 0)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							if (!this.skipping)
							{
								return i;
							}
							this.sign = 0;
						}
					}
				}
				else if (this.channels == 2)
				{
					for (int j = 0; j < s.Length; j += 2)
					{
						if (s[j] + s[j + 1] < 0)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							if (!this.skipping)
							{
								return j / 2;
							}
							this.sign = 0;
						}
					}
				}
				else
				{
					for (int k = 0; k < s.Length; k += this.channels)
					{
						int num = (int)(s[k] + s[k + 1]);
						int num2 = 2;
						while (k < this.channels)
						{
							num += (int)s[k + num2];
							num2++;
						}
						if (num < 0)
						{
							this.sign = -1;
						}
						else if (this.sign < 0)
						{
							this.waveCnt++;
							this.skipping = (this.waveCnt % (this.skipGroup * this.skipFactor) < this.skipGroup);
							if (!this.skipping)
							{
								return k / this.channels;
							}
							this.sign = 0;
						}
					}
				}
				return 0;
			}

			private readonly int sizeofT = Marshal.SizeOf<T>(default(T));

			private int channels;

			private int skipGroup;

			private int skipFactor;

			private int sign;

			private int waveCnt;

			private bool skipping;
		}

		public class Resampler<T> : IProcessor<T>, IDisposable
		{
			public Resampler(int dstSize, int channels)
			{
				this.frameResampled = new T[dstSize];
				this.channels = channels;
			}

			public T[] Process(T[] buf)
			{
				AudioUtil.Resample<T>(buf, this.frameResampled, this.frameResampled.Length, this.channels);
				return this.frameResampled;
			}

			public void Dispose()
			{
			}

			protected T[] frameResampled;

			private int channels;
		}

		public interface ILevelMeter
		{
			float CurrentAvgAmp { get; }

			float CurrentPeakAmp { get; }

			float AccumAvgPeakAmp { get; }

			void ResetAccumAvgPeakAmp();
		}

		public class LevelMeterDummy : AudioUtil.ILevelMeter
		{
			public float CurrentAvgAmp
			{
				get
				{
					return 0f;
				}
			}

			public float CurrentPeakAmp
			{
				get
				{
					return 0f;
				}
			}

			public float AccumAvgPeakAmp
			{
				get
				{
					return 0f;
				}
			}

			public void ResetAccumAvgPeakAmp()
			{
			}
		}

		public abstract class LevelMeter<T> : IProcessor<T>, IDisposable, AudioUtil.ILevelMeter
		{
			internal LevelMeter(int samplingRate, int numChannels)
			{
				this.bufferSize = samplingRate * numChannels / 2;
				this.prevValues = new float[this.bufferSize];
			}

			public float CurrentAvgAmp
			{
				get
				{
					return this.ampSum / (float)this.bufferSize * this.norm;
				}
			}

			public float CurrentPeakAmp
			{
				get
				{
					return this.currentPeakAmp * this.norm;
				}
				protected set
				{
					this.currentPeakAmp = value / this.norm;
				}
			}

			public float AccumAvgPeakAmp
			{
				get
				{
					if (this.accumAvgPeakAmpCount != 0)
					{
						return this.accumAvgPeakAmpSum / (float)this.accumAvgPeakAmpCount * this.norm;
					}
					return 0f;
				}
			}

			public void ResetAccumAvgPeakAmp()
			{
				this.accumAvgPeakAmpSum = 0f;
				this.accumAvgPeakAmpCount = 0;
				this.ampPeak = 0f;
			}

			public abstract T[] Process(T[] buf);

			public void Dispose()
			{
			}

			protected float ampSum;

			protected float ampPeak;

			protected int bufferSize;

			protected float[] prevValues;

			protected int prevValuesHead;

			protected float accumAvgPeakAmpSum;

			protected int accumAvgPeakAmpCount;

			protected float currentPeakAmp;

			protected float norm;
		}

		public class LevelMeterFloat : AudioUtil.LevelMeter<float>
		{
			public LevelMeterFloat(int samplingRate, int numChannels) : base(samplingRate, numChannels)
			{
				this.norm = 1f;
			}

			public override float[] Process(float[] buf)
			{
				foreach (float num in buf)
				{
					if (num < 0f)
					{
						num = -num;
					}
					this.ampSum = this.ampSum + num - this.prevValues[this.prevValuesHead];
					this.prevValues[this.prevValuesHead] = num;
					if (this.ampPeak < num)
					{
						this.ampPeak = num;
					}
					if (this.prevValuesHead == 0)
					{
						this.currentPeakAmp = this.ampPeak;
						this.ampPeak = 0f;
						this.accumAvgPeakAmpSum += this.currentPeakAmp;
						this.accumAvgPeakAmpCount++;
					}
					this.prevValuesHead = (this.prevValuesHead + 1) % this.bufferSize;
				}
				return buf;
			}
		}

		public class LevelMeterShort : AudioUtil.LevelMeter<short>
		{
			public LevelMeterShort(int samplingRate, int numChannels) : base(samplingRate, numChannels)
			{
				this.norm = 3.051851E-05f;
			}

			public override short[] Process(short[] buf)
			{
				foreach (short num in buf)
				{
					if (num < 0)
					{
						num = -num;
					}
					this.ampSum = this.ampSum + (float)num - this.prevValues[this.prevValuesHead];
					this.prevValues[this.prevValuesHead] = (float)num;
					if (this.ampPeak < (float)num)
					{
						this.ampPeak = (float)num;
					}
					if (this.prevValuesHead == 0)
					{
						this.currentPeakAmp = this.ampPeak;
						this.ampPeak = 0f;
						this.accumAvgPeakAmpSum += this.currentPeakAmp;
						this.accumAvgPeakAmpCount++;
					}
					this.prevValuesHead = (this.prevValuesHead + 1) % this.bufferSize;
				}
				return buf;
			}
		}

		public interface IVoiceDetector
		{
			bool On { get; set; }

			float Threshold { get; set; }

			bool Detected { get; }

			DateTime DetectedTime { get; }

			event Action OnDetected;

			int ActivityDelayMs { get; set; }
		}

		public class VoiceDetectorCalibration<T> : IProcessor<T>, IDisposable
		{
			public bool IsCalibrating
			{
				get
				{
					return this.calibrateCount > 0;
				}
			}

			public VoiceDetectorCalibration(AudioUtil.IVoiceDetector voiceDetector, AudioUtil.ILevelMeter levelMeter, int samplingRate, int channels)
			{
				this.valuesPerSec = samplingRate * channels;
				this.voiceDetector = voiceDetector;
				this.levelMeter = levelMeter;
			}

			public void Calibrate(int durationMs, Action<float> onCalibrated = null)
			{
				this.calibrateCount = this.valuesPerSec * durationMs / 1000;
				this.onCalibrated = onCalibrated;
				this.levelMeter.ResetAccumAvgPeakAmp();
			}

			public T[] Process(T[] buf)
			{
				if (this.calibrateCount != 0)
				{
					this.calibrateCount -= buf.Length;
					if (this.calibrateCount <= 0)
					{
						this.calibrateCount = 0;
						this.voiceDetector.Threshold = this.levelMeter.AccumAvgPeakAmp * 2f;
						if (this.onCalibrated != null)
						{
							this.onCalibrated(this.voiceDetector.Threshold);
						}
					}
				}
				return buf;
			}

			public void Dispose()
			{
			}

			private AudioUtil.IVoiceDetector voiceDetector;

			private AudioUtil.ILevelMeter levelMeter;

			private int valuesPerSec;

			protected int calibrateCount;

			private Action<float> onCalibrated;
		}

		public class VoiceDetectorDummy : AudioUtil.IVoiceDetector
		{
			public bool On
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public float Threshold
			{
				get
				{
					return 0f;
				}
				set
				{
				}
			}

			public bool Detected
			{
				get
				{
					return false;
				}
			}

			public int ActivityDelayMs
			{
				get
				{
					return 0;
				}
				set
				{
				}
			}

			public DateTime DetectedTime { get; private set; }

			public event Action OnDetected
			{
				add
				{
				}
				remove
				{
				}
			}
		}

		public abstract class VoiceDetector<T> : IProcessor<T>, IDisposable, AudioUtil.IVoiceDetector
		{
			public bool On { get; set; }

			public float Threshold
			{
				get
				{
					return this.threshold * this.norm;
				}
				set
				{
					this.threshold = value / this.norm;
				}
			}

			public bool Detected
			{
				get
				{
					return this.detected;
				}
				protected set
				{
					if (this.detected != value)
					{
						this.detected = value;
						this.DetectedTime = DateTime.Now;
						if (this.detected && this.OnDetected != null)
						{
							this.OnDetected();
						}
					}
				}
			}

			public DateTime DetectedTime { get; private set; }

			public int ActivityDelayMs
			{
				get
				{
					return this.activityDelay;
				}
				set
				{
					this.activityDelay = value;
					this.activityDelayValuesCount = value * this.valuesCountPerSec / 1000;
				}
			}

			public event Action OnDetected;

			internal VoiceDetector(int samplingRate, int numChannels)
			{
				this.valuesCountPerSec = samplingRate * numChannels;
				this.ActivityDelayMs = 500;
				this.On = true;
			}

			public abstract T[] Process(T[] buf);

			public void Dispose()
			{
			}

			protected float norm;

			protected float threshold;

			private bool detected;

			protected int activityDelay;

			protected int autoSilenceCounter;

			protected int valuesCountPerSec;

			protected int activityDelayValuesCount;
		}

		public class VoiceDetectorFloat : AudioUtil.VoiceDetector<float>
		{
			public VoiceDetectorFloat(int samplingRate, int numChannels) : base(samplingRate, numChannels)
			{
				this.norm = 1f;
			}

			public override float[] Process(float[] buffer)
			{
				if (!base.On)
				{
					return buffer;
				}
				for (int i = 0; i < buffer.Length; i++)
				{
					if (buffer[i] > this.threshold)
					{
						base.Detected = true;
						this.autoSilenceCounter = 0;
					}
					else
					{
						this.autoSilenceCounter++;
					}
				}
				if (this.autoSilenceCounter > this.activityDelayValuesCount)
				{
					base.Detected = false;
				}
				if (!base.Detected)
				{
					return null;
				}
				return buffer;
			}
		}

		public class VoiceDetectorShort : AudioUtil.VoiceDetector<short>
		{
			public VoiceDetectorShort(int samplingRate, int numChannels) : base(samplingRate, numChannels)
			{
				this.norm = 3.051851E-05f;
			}

			public override short[] Process(short[] buffer)
			{
				if (!base.On)
				{
					return buffer;
				}
				for (int i = 0; i < buffer.Length; i++)
				{
					if ((float)buffer[i] > this.threshold)
					{
						base.Detected = true;
						this.autoSilenceCounter = 0;
					}
					else
					{
						this.autoSilenceCounter++;
					}
				}
				if (this.autoSilenceCounter > this.activityDelayValuesCount)
				{
					base.Detected = false;
				}
				if (!base.Detected)
				{
					return null;
				}
				return buffer;
			}
		}

		public class VoiceLevelDetectCalibrate<T> : IProcessor<T>, IDisposable
		{
			public AudioUtil.ILevelMeter LevelMeter { get; private set; }

			public AudioUtil.IVoiceDetector VoiceDetector { get; private set; }

			public VoiceLevelDetectCalibrate(int samplingRate, int channels)
			{
				T[] array = new T[1];
				if (array[0] is float)
				{
					this.LevelMeter = new AudioUtil.LevelMeterFloat(samplingRate, channels);
					this.VoiceDetector = new AudioUtil.VoiceDetectorFloat(samplingRate, channels);
				}
				else
				{
					if (!(array[0] is short))
					{
						string str = "VoiceLevelDetectCalibrate: type not supported: ";
						Type type = array[0].GetType();
						throw new Exception(str + ((type != null) ? type.ToString() : null));
					}
					this.LevelMeter = new AudioUtil.LevelMeterShort(samplingRate, channels);
					this.VoiceDetector = new AudioUtil.VoiceDetectorShort(samplingRate, channels);
				}
				this.calibration = new AudioUtil.VoiceDetectorCalibration<T>(this.VoiceDetector, this.LevelMeter, samplingRate, channels);
			}

			public void Calibrate(int durationMs, Action<float> onCalibrated = null)
			{
				this.calibration.Calibrate(durationMs, onCalibrated);
			}

			public bool IsCalibrating
			{
				get
				{
					return this.calibration.IsCalibrating;
				}
			}

			public T[] Process(T[] buf)
			{
				buf = (this.LevelMeter as IProcessor<T>).Process(buf);
				buf = ((IProcessor<T>)this.calibration).Process(buf);
				buf = (this.VoiceDetector as IProcessor<T>).Process(buf);
				return buf;
			}

			public void Dispose()
			{
				(this.LevelMeter as IProcessor<T>).Dispose();
				(this.VoiceDetector as IProcessor<T>).Dispose();
				this.calibration.Dispose();
			}

			private AudioUtil.VoiceDetectorCalibration<T> calibration;
		}
	}
}
