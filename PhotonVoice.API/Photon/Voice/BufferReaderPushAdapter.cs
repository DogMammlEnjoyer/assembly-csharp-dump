using System;

namespace Photon.Voice
{
	public class BufferReaderPushAdapter<T> : BufferReaderPushAdapterBase<T>
	{
		public BufferReaderPushAdapter(LocalVoice localVoice, IDataReader<T> reader) : base(reader)
		{
			this.buffer = new T[((LocalVoiceFramed<T>)localVoice).FrameSize];
		}

		public override void Service(LocalVoice localVoice)
		{
			while (this.reader.Read(this.buffer))
			{
				((LocalVoiceFramed<T>)localVoice).PushData(this.buffer);
			}
		}

		protected T[] buffer;
	}
}
