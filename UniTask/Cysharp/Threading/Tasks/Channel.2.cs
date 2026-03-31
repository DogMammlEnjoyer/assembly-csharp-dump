using System;

namespace Cysharp.Threading.Tasks
{
	public abstract class Channel<TWrite, TRead>
	{
		public ChannelReader<TRead> Reader { get; protected set; }

		public ChannelWriter<TWrite> Writer { get; protected set; }

		public static implicit operator ChannelReader<TRead>(Channel<TWrite, TRead> channel)
		{
			return channel.Reader;
		}

		public static implicit operator ChannelWriter<TWrite>(Channel<TWrite, TRead> channel)
		{
			return channel.Writer;
		}
	}
}
