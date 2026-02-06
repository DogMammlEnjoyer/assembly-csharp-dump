using System;
using System.Collections.Generic;

namespace Photon.Voice
{
	public class Framer<T>
	{
		public Framer(int frameSize)
		{
			this.frame = new T[frameSize];
			T[] array = new T[1];
			if (array[0] is byte)
			{
				this.sizeofT = 1;
				return;
			}
			if (array[0] is short)
			{
				this.sizeofT = 2;
				return;
			}
			if (array[0] is float)
			{
				this.sizeofT = 4;
				return;
			}
			string str = "Input data type is not supported: ";
			Type type = array[0].GetType();
			throw new Exception(str + ((type != null) ? type.ToString() : null));
		}

		public int Count(int bufLen)
		{
			return (bufLen + this.framePos) / this.frame.Length;
		}

		public IEnumerable<T[]> Frame(T[] buf)
		{
			if (this.frame.Length == buf.Length && this.framePos == 0)
			{
				yield return buf;
			}
			else
			{
				int bufPos = 0;
				while (this.frame.Length - this.framePos <= buf.Length - bufPos)
				{
					int num = this.frame.Length - this.framePos;
					Buffer.BlockCopy(buf, bufPos * this.sizeofT, this.frame, this.framePos * this.sizeofT, num * this.sizeofT);
					bufPos += num;
					this.framePos = 0;
					yield return this.frame;
				}
				if (bufPos != buf.Length)
				{
					int num2 = buf.Length - bufPos;
					Buffer.BlockCopy(buf, bufPos * this.sizeofT, this.frame, this.framePos * this.sizeofT, num2 * this.sizeofT);
					this.framePos += num2;
				}
			}
			yield break;
		}

		private T[] frame;

		private int sizeofT;

		private int framePos;
	}
}
