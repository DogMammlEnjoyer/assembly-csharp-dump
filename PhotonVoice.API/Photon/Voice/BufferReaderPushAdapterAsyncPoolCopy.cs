using System;

namespace Photon.Voice
{
	public class BufferReaderPushAdapterAsyncPoolCopy<T> : BufferReaderPushAdapterBase<T>
	{
		public BufferReaderPushAdapterAsyncPoolCopy(LocalVoice localVoice, IDataReader<T> reader) : base(reader)
		{
			this.buffer = new T[((LocalVoiceFramedBase)localVoice).FrameSize];
		}

		public override void Service(LocalVoice localVoice)
		{
			while (this.reader.Read(this.buffer))
			{
				LocalVoiceFramed<T> localVoiceFramed = (LocalVoiceFramed<T>)localVoice;
				T[] array = localVoiceFramed.BufferFactory.New();
				Array.Copy(this.buffer, array, this.buffer.Length);
				localVoiceFramed.PushDataAsync(array);
			}
		}

		protected T[] buffer;
	}
}
