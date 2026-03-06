using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Meta.Voice.Audio.Decoding
{
	[Preserve]
	public class AudioDecoderWav : AudioDecoderPcm
	{
		[Preserve]
		public AudioDecoderWav(int sampleBufferLength = 720) : base(AudioDecoderPcmType.Int16, sampleBufferLength)
		{
		}

		public override void Decode(byte[] buffer, int bufferOffset, int bufferLength, AudioSampleDecodeDelegate onSamplesDecoded)
		{
			while (bufferLength > 0)
			{
				if (this._subChunkLength == 0)
				{
					int num = this.DecodeSubChunkHeader(buffer, bufferOffset, bufferLength);
					bufferOffset += num;
					bufferLength -= num;
				}
				else
				{
					int num2 = Mathf.Min(this._subChunkLength - this._subChunkOffset, bufferLength);
					if (this._subChunkIsData)
					{
						base.Decode(buffer, bufferOffset, num2, onSamplesDecoded);
					}
					this._subChunkOffset += num2;
					bufferOffset += num2;
					bufferLength -= num2;
					if (this._subChunkOffset >= this._subChunkLength)
					{
						this._subChunkOffset = 0;
						this._subChunkLength = 0;
					}
				}
			}
		}

		private int DecodeSubChunkHeader(byte[] buffer, int bufferOffset, int bufferLength)
		{
			int num = Mathf.Min(this._subChunkHeader.Length - this._subChunkOffset, bufferLength);
			Array.Copy(buffer, bufferOffset, this._subChunkHeader, this._subChunkOffset, num);
			this._subChunkOffset += num;
			if (this._subChunkOffset >= this._subChunkHeader.Length)
			{
				this._subChunkOffset = 0;
				this._subChunkIsData = AudioDecoderWav.SubArrayEquals<byte>(this._subChunkHeader, 0, AudioDecoderWav.DataDescriptor, 0, AudioDecoderWav.DataDescriptor.Length);
				this._subChunkLength = (int)BitConverter.ToUInt32(this._subChunkHeader, 4);
			}
			return num;
		}

		private static bool SubArrayEquals<T>(T[] array1, int offset1, T[] array2, int offset2, int length)
		{
			if (array1 == null || array2 == null || array1.Length < offset1 + length || array2.Length < offset2 + length)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				if (!array1[offset1 + i].Equals(array2[offset2 + i]))
				{
					return false;
				}
			}
			return true;
		}

		private int _subChunkOffset;

		private readonly byte[] _subChunkHeader = new byte[8];

		private bool _subChunkIsData;

		private int _subChunkLength = 12;

		private static readonly byte[] DataDescriptor = new byte[]
		{
			100,
			97,
			116,
			97
		};
	}
}
