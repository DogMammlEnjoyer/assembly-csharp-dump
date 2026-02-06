using System;

namespace Photon.Voice
{
	public class FrameOut<T>
	{
		public FrameOut(T[] buf, bool endOfStream)
		{
			this.Set(buf, endOfStream);
		}

		public FrameOut<T> Set(T[] buf, bool endOfStream)
		{
			this.Buf = buf;
			this.EndOfStream = endOfStream;
			return this;
		}

		public T[] Buf { get; private set; }

		public bool EndOfStream { get; private set; }
	}
}
