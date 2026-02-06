using System;

namespace Photon.Voice
{
	public class BufferReaderPushAdapterAsyncPoolShortToFloat : BufferReaderPushAdapterBase<short>
	{
		public BufferReaderPushAdapterAsyncPoolShortToFloat(LocalVoice localVoice, IDataReader<short> reader) : base(reader)
		{
			this.buffer = new short[((LocalVoiceFramed<float>)localVoice).FrameSize];
		}

		public override void Service(LocalVoice localVoice)
		{
			LocalVoiceFramed<float> localVoiceFramed = (LocalVoiceFramed<float>)localVoice;
			float[] array = localVoiceFramed.BufferFactory.New();
			while (this.reader.Read(this.buffer))
			{
				AudioUtil.Convert(this.buffer, array, array.Length);
				localVoiceFramed.PushDataAsync(array);
				array = localVoiceFramed.BufferFactory.New();
			}
			localVoiceFramed.BufferFactory.Free(array, array.Length);
		}

		private short[] buffer;
	}
}
