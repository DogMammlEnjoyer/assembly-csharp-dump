using System;

namespace ICSharpCode.SharpZipLib.Checksum
{
	public interface IChecksum
	{
		void Reset();

		long Value { get; }

		void Update(int bval);

		void Update(byte[] buffer);

		void Update(ArraySegment<byte> segment);
	}
}
