using System;
using System.Runtime.InteropServices;

namespace Liv.Lck.Collections
{
	public class AudioBuffer
	{
		public int Count
		{
			get
			{
				return this._logicalCount;
			}
		}

		public int Capacity
		{
			get
			{
				return this._buffer.Length;
			}
		}

		public float this[int index]
		{
			get
			{
				return this._buffer[index];
			}
		}

		public AudioBuffer(int maxCapacity)
		{
			this._buffer = new float[maxCapacity];
			this._logicalCount = 0;
		}

		public float[] Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		public void Clear()
		{
			this._logicalCount = 0;
		}

		public bool TryAdd(float value)
		{
			if (this._logicalCount >= this._buffer.Length)
			{
				return false;
			}
			this._buffer[this._logicalCount] = value;
			this._logicalCount++;
			return true;
		}

		public bool TryCopyFrom(float[] source, int sourceIndex, int count)
		{
			if (count > this._buffer.Length)
			{
				return false;
			}
			Array.Copy(source, sourceIndex, this._buffer, 0, count);
			this._logicalCount = count;
			return true;
		}

		public bool TryCopyFrom(IntPtr source, int count)
		{
			if (count > this._buffer.Length)
			{
				return false;
			}
			Marshal.Copy(source, this._buffer, 0, count);
			this._logicalCount = count;
			return true;
		}

		public bool TryCopyFrom(AudioBuffer source)
		{
			if (source._logicalCount > this._buffer.Length)
			{
				return false;
			}
			Array.Copy(source._buffer, 0, this._buffer, 0, source._logicalCount);
			this._logicalCount = source._logicalCount;
			return true;
		}

		public bool TryExtendFrom(float[] source)
		{
			if (this._logicalCount + source.Length > this._buffer.Length)
			{
				return false;
			}
			Array.Copy(source, 0, this._buffer, this._logicalCount, source.Length);
			this._logicalCount += source.Length;
			return true;
		}

		public bool TryExtendFrom(AudioBuffer source)
		{
			if (this._logicalCount + source._logicalCount > this._buffer.Length)
			{
				return false;
			}
			Array.Copy(source._buffer, 0, this._buffer, this._logicalCount, source._logicalCount);
			this._logicalCount += source._logicalCount;
			return true;
		}

		public void OverrideCount(int newCount)
		{
			this._logicalCount = newCount;
		}

		public void PadAudioBuffer(int samplesToPad)
		{
			for (int i = 0; i < samplesToPad; i++)
			{
				this.TryAdd(0f);
			}
		}

		public void SkipAudioSamples(int samplesToSkip)
		{
			if (samplesToSkip >= this.Count)
			{
				this.Clear();
				return;
			}
			int num = this.Count - samplesToSkip;
			Array.Copy(this.Buffer, samplesToSkip, this.Buffer, 0, num);
			this.TryCopyFrom(this.Buffer, 0, num);
		}

		private float[] _buffer;

		private int _logicalCount;
	}
}
