using System;
using System.Diagnostics;
using System.Linq;

namespace Photon.Voice
{
	internal class SpacingProfile
	{
		public SpacingProfile(int capacity)
		{
			this.capacity = capacity;
		}

		public void Start()
		{
			if (this.watch == null)
			{
				this.buf = new short[this.capacity];
				this.info = new bool[this.capacity];
				this.watch = Stopwatch.StartNew();
			}
		}

		public void Update(bool lost, bool flush)
		{
			if (this.watch == null)
			{
				return;
			}
			if (this.flushed)
			{
				this.watchLast = this.watch.ElapsedMilliseconds;
			}
			long elapsedMilliseconds = this.watch.ElapsedMilliseconds;
			this.buf[this.ptr] = (short)(elapsedMilliseconds - this.watchLast);
			this.info[this.ptr] = lost;
			this.watchLast = elapsedMilliseconds;
			this.ptr++;
			if (this.ptr == this.buf.Length)
			{
				this.ptr = 0;
			}
			this.flushed = flush;
		}

		public string Dump
		{
			get
			{
				if (this.watch == null)
				{
					return "Error: Profiler not started.";
				}
				string[] value = this.buf.Select((short v, int i) => (this.info[i] ? "-" : "") + v.ToString()).ToArray<string>();
				return string.Concat(new string[]
				{
					"max=",
					this.Max.ToString(),
					" ",
					string.Join(",", value, this.ptr, this.buf.Length - this.ptr),
					", ",
					string.Join(",", value, 0, this.ptr)
				});
			}
		}

		public int Max
		{
			get
			{
				return (int)(from v in this.buf
				select Math.Abs(v)).Max<short>();
			}
		}

		private short[] buf;

		private bool[] info;

		private int capacity;

		private int ptr;

		private Stopwatch watch;

		private long watchLast;

		private bool flushed;
	}
}
