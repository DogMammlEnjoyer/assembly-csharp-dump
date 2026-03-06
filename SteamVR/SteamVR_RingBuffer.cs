using System;

namespace Valve.VR
{
	public class SteamVR_RingBuffer<T>
	{
		public SteamVR_RingBuffer(int size)
		{
			this.buffer = new T[size];
			this.currentIndex = 0;
		}

		public void Add(T newElement)
		{
			this.buffer[this.currentIndex] = newElement;
			this.StepForward();
		}

		public virtual void StepForward()
		{
			this.lastElement = this.buffer[this.currentIndex];
			this.currentIndex++;
			if (this.currentIndex >= this.buffer.Length)
			{
				this.currentIndex = 0;
			}
			this.cleared = false;
		}

		public virtual T GetAtIndex(int atIndex)
		{
			if (atIndex < 0)
			{
				atIndex += this.buffer.Length;
			}
			return this.buffer[atIndex];
		}

		public virtual T GetLast()
		{
			return this.lastElement;
		}

		public virtual int GetLastIndex()
		{
			int num = this.currentIndex - 1;
			if (num < 0)
			{
				num += this.buffer.Length;
			}
			return num;
		}

		public void Clear()
		{
			if (this.cleared)
			{
				return;
			}
			if (this.buffer == null)
			{
				return;
			}
			for (int i = 0; i < this.buffer.Length; i++)
			{
				this.buffer[i] = default(T);
			}
			this.lastElement = default(T);
			this.currentIndex = 0;
			this.cleared = true;
		}

		protected T[] buffer;

		protected int currentIndex;

		protected T lastElement;

		private bool cleared;
	}
}
