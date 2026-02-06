using System;

namespace Photon.Voice
{
	public class BufferReaderPushAdapterAsyncPoolFloatToShort : BufferReaderPushAdapterBase<float>
	{
		public BufferReaderPushAdapterAsyncPoolFloatToShort(LocalVoice localVoice, IDataReader<float> reader) : base(reader)
		{
			this.buffer = new float[((LocalVoiceFramed<short>)localVoice).FrameSize];
		}

		public override void Service(LocalVoice localVoice)
		{
			LocalVoiceFramed<short> localVoiceFramed = (LocalVoiceFramed<short>)localVoice;
			short[] array = localVoiceFramed.BufferFactory.New();
			while (this.reader.Read(this.buffer))
			{
				AudioUtil.Convert(this.buffer, array, array.Length);
				localVoiceFramed.PushDataAsync(array);
				array = localVoiceFramed.BufferFactory.New();
			}
			localVoiceFramed.BufferFactory.Free(array, array.Length);
		}

		private float[] buffer;
	}
}
