using System;
using System.Collections.Generic;

namespace Fusion.Protocol
{
	internal abstract class CommunicatorBase : ICommunicator
	{
		public abstract int CommunicatorID { get; }

		public bool Poll()
		{
			return this.RecvQueue.Count > 0;
		}

		public void PushPackage(int senderActor, int eventCode, object data)
		{
			senderActor = ((senderActor <= 0) ? 0 : senderActor);
			switch (eventCode)
			{
			case 100:
				this.HandleProtocolPackage(senderActor, data);
				break;
			case 101:
				this.RecvQueue.Enqueue(new ValueTuple<int, object>(senderActor, data));
				break;
			case 102:
			{
				TraceLogStream logTraceDummyTraffic = InternalLogStreams.LogTraceDummyTraffic;
				if (logTraceDummyTraffic != null)
				{
					logTraceDummyTraffic.Log(string.Format("Received Dummy Traffic from [{0}]", senderActor));
				}
				break;
			}
			}
		}

		public unsafe void SendMessage(int targetActor, IMessage message)
		{
			BitStream bitStream;
			bool flag = this._protocolSerializer.ConvertToBuffer((Message)message, out bitStream);
			if (flag)
			{
				byte[] array;
				byte* buffer;
				if ((array = bitStream.Data) == null || array.Length == 0)
				{
					buffer = null;
				}
				else
				{
					buffer = &array[0];
				}
				bool flag2 = this.SendPackage(100, targetActor, true, buffer, bitStream.BytesRequired);
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(string.Format("Sending to [{0}]: {1}", targetActor, message));
					}
				}
				else
				{
					this.MessageSendQueue.Enqueue(new ValueTuple<int, Message>(targetActor, (Message)message));
				}
				array = null;
			}
			else
			{
				this.MessageSendQueue.Enqueue(new ValueTuple<int, Message>(targetActor, (Message)message));
			}
		}

		public virtual void Service()
		{
			bool flag = this.MessageSendQueue.Count <= 0;
			if (!flag)
			{
				ValueTuple<int, Message> valueTuple = this.MessageSendQueue.Dequeue();
				int item = valueTuple.Item1;
				Message item2 = valueTuple.Item2;
				this.SendMessage(item, item2);
			}
		}

		private void HandleProtocolPackage(int actorNr, object data)
		{
			byte[] array;
			int num;
			this.ConvertData(data, out array, out num);
			bool flag = array != null && this._protocolSerializer.ConvertToMessages(array, this._messageList);
			if (flag)
			{
				foreach (Message message in this._messageList)
				{
					Action<int, IMessage> action;
					bool flag2 = this.Callbacks.TryGetValue(message.GetType(), out action);
					if (flag2)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Log(string.Format("Received from [{0}] :: {1}", actorNr, message));
						}
						action(actorNr, message);
					}
				}
			}
		}

		public unsafe int ReceivePackage(out int senderActor, byte* buffer, int bufferLength)
		{
			bool flag = this.Poll();
			if (flag)
			{
				ValueTuple<int, object> valueTuple = this.RecvQueue.Dequeue();
				int item = valueTuple.Item1;
				object item2 = valueTuple.Item2;
				byte[] array;
				int num;
				this.ConvertData(item2, out array, out num);
				bool flag2 = num > 0;
				if (flag2)
				{
					senderActor = item;
					Assert.Always(num <= bufferLength, "ReceivePackage overflow");
					byte[] array2;
					byte* source;
					if ((array2 = array) == null || array2.Length == 0)
					{
						source = null;
					}
					else
					{
						source = &array2[0];
					}
					Native.MemCpy((void*)buffer, (void*)source, num);
					array2 = null;
					return num;
				}
			}
			senderActor = -1;
			return -1;
		}

		public unsafe abstract bool SendPackage(byte code, int targetActor, bool reliable, byte* buffer, int bufferLength);

		protected abstract void ConvertData(object data, out byte[] dataBuffer, out int maxLength);

		public void RegisterPackageCallback<K>(Action<int, K> callback) where K : IMessage
		{
			this.Callbacks.Add(typeof(K), delegate(int actor, IMessage msg)
			{
				callback(actor, (K)((object)msg));
			});
		}

		protected readonly Dictionary<Type, Action<int, IMessage>> Callbacks = new Dictionary<Type, Action<int, IMessage>>();

		protected readonly Queue<ValueTuple<int, Message>> MessageSendQueue = new Queue<ValueTuple<int, Message>>(64);

		protected readonly Queue<ValueTuple<int, object>> RecvQueue = new Queue<ValueTuple<int, object>>();

		private readonly List<Message> _messageList = new List<Message>(64);

		private readonly ProtocolSerializer _protocolSerializer = new ProtocolSerializer();
	}
}
