using System;

namespace Photon.Voice
{
	public abstract class BufferReaderPushAdapterBase<T> : IServiceable
	{
		public abstract void Service(LocalVoice localVoice);

		public BufferReaderPushAdapterBase(IDataReader<T> reader)
		{
			this.reader = reader;
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

		protected IDataReader<T> reader;
	}
}
