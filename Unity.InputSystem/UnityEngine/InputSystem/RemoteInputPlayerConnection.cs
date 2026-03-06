using System;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Networking.PlayerConnection;

namespace UnityEngine.InputSystem
{
	[Serializable]
	internal class RemoteInputPlayerConnection : ScriptableObject, IObserver<InputRemoting.Message>, IObservable<InputRemoting.Message>
	{
		public void Bind(IEditorPlayerConnection connection, bool isConnected)
		{
			if (this.m_Connection == null)
			{
				connection.RegisterConnection(new UnityAction<int>(this.OnConnected));
				connection.RegisterDisconnection(new UnityAction<int>(this.OnDisconnected));
				connection.Register(RemoteInputPlayerConnection.kNewDeviceMsg, new UnityAction<MessageEventArgs>(this.OnNewDevice));
				connection.Register(RemoteInputPlayerConnection.kNewLayoutMsg, new UnityAction<MessageEventArgs>(this.OnNewLayout));
				connection.Register(RemoteInputPlayerConnection.kNewEventsMsg, new UnityAction<MessageEventArgs>(this.OnNewEvents));
				connection.Register(RemoteInputPlayerConnection.kRemoveDeviceMsg, new UnityAction<MessageEventArgs>(this.OnRemoveDevice));
				connection.Register(RemoteInputPlayerConnection.kChangeUsagesMsg, new UnityAction<MessageEventArgs>(this.OnChangeUsages));
				connection.Register(RemoteInputPlayerConnection.kStartSendingMsg, new UnityAction<MessageEventArgs>(this.OnStartSending));
				connection.Register(RemoteInputPlayerConnection.kStopSendingMsg, new UnityAction<MessageEventArgs>(this.OnStopSending));
				this.m_Connection = connection;
				if (isConnected)
				{
					this.OnConnected(0);
				}
				return;
			}
			if (this.m_Connection == connection)
			{
				return;
			}
			throw new InvalidOperationException("Already bound to an IEditorPlayerConnection");
		}

		public IDisposable Subscribe(IObserver<InputRemoting.Message> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}
			RemoteInputPlayerConnection.Subscriber subscriber = new RemoteInputPlayerConnection.Subscriber
			{
				owner = this,
				observer = observer
			};
			ArrayHelpers.Append<RemoteInputPlayerConnection.Subscriber>(ref this.m_Subscribers, subscriber);
			if (this.m_ConnectedIds != null)
			{
				foreach (int participantId in this.m_ConnectedIds)
				{
					observer.OnNext(new InputRemoting.Message
					{
						type = InputRemoting.MessageType.Connect,
						participantId = participantId
					});
				}
			}
			return subscriber;
		}

		private void OnConnected(int id)
		{
			if (this.m_ConnectedIds != null && ArrayHelpers.Contains<int>(this.m_ConnectedIds, id))
			{
				return;
			}
			ArrayHelpers.Append<int>(ref this.m_ConnectedIds, id);
			this.SendToSubscribers(InputRemoting.MessageType.Connect, new MessageEventArgs
			{
				playerId = id
			});
		}

		private void OnDisconnected(int id)
		{
			if (this.m_ConnectedIds == null || !ArrayHelpers.Contains<int>(this.m_ConnectedIds, id))
			{
				return;
			}
			ArrayHelpers.Erase<int>(ref this.m_ConnectedIds, id);
			this.SendToSubscribers(InputRemoting.MessageType.Disconnect, new MessageEventArgs
			{
				playerId = id
			});
		}

		private void OnNewDevice(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.NewDevice, args);
		}

		private void OnNewLayout(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.NewLayout, args);
		}

		private void OnNewEvents(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.NewEvents, args);
		}

		private void OnRemoveDevice(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.RemoveDevice, args);
		}

		private void OnChangeUsages(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.ChangeUsages, args);
		}

		private void OnStartSending(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.StartSending, args);
		}

		private void OnStopSending(MessageEventArgs args)
		{
			this.SendToSubscribers(InputRemoting.MessageType.StopSending, args);
		}

		private void SendToSubscribers(InputRemoting.MessageType type, MessageEventArgs args)
		{
			if (this.m_Subscribers == null)
			{
				return;
			}
			InputRemoting.Message value = new InputRemoting.Message
			{
				participantId = args.playerId,
				type = type,
				data = args.data
			};
			for (int i = 0; i < this.m_Subscribers.Length; i++)
			{
				this.m_Subscribers[i].observer.OnNext(value);
			}
		}

		void IObserver<InputRemoting.Message>.OnNext(InputRemoting.Message msg)
		{
			if (this.m_Connection == null)
			{
				return;
			}
			switch (msg.type)
			{
			case InputRemoting.MessageType.NewLayout:
				this.m_Connection.Send(RemoteInputPlayerConnection.kNewLayoutMsg, msg.data);
				return;
			case InputRemoting.MessageType.NewDevice:
				this.m_Connection.Send(RemoteInputPlayerConnection.kNewDeviceMsg, msg.data);
				return;
			case InputRemoting.MessageType.NewEvents:
				this.m_Connection.Send(RemoteInputPlayerConnection.kNewEventsMsg, msg.data);
				return;
			case InputRemoting.MessageType.RemoveDevice:
				this.m_Connection.Send(RemoteInputPlayerConnection.kRemoveDeviceMsg, msg.data);
				break;
			case InputRemoting.MessageType.RemoveLayout:
				break;
			case InputRemoting.MessageType.ChangeUsages:
				this.m_Connection.Send(RemoteInputPlayerConnection.kChangeUsagesMsg, msg.data);
				return;
			default:
				return;
			}
		}

		void IObserver<InputRemoting.Message>.OnError(Exception error)
		{
		}

		void IObserver<InputRemoting.Message>.OnCompleted()
		{
		}

		public static readonly Guid kNewDeviceMsg = new Guid("fcd9651ded40425995dfa6aeb78f1f1c");

		public static readonly Guid kNewLayoutMsg = new Guid("fccfec2b7369466d88502a9dd38505f4");

		public static readonly Guid kNewEventsMsg = new Guid("53546641df1347bc8aa315278a603586");

		public static readonly Guid kRemoveDeviceMsg = new Guid("e5e299b2d9e44255b8990bb71af8922d");

		public static readonly Guid kChangeUsagesMsg = new Guid("b9fe706dfc854d7ca109a5e38d7db730");

		public static readonly Guid kStartSendingMsg = new Guid("0d58e99045904672b3ef34b8797d23cb");

		public static readonly Guid kStopSendingMsg = new Guid("548716b2534a45369ab0c9323fc8b4a8");

		[SerializeField]
		private IEditorPlayerConnection m_Connection;

		[NonSerialized]
		private RemoteInputPlayerConnection.Subscriber[] m_Subscribers;

		[SerializeField]
		private int[] m_ConnectedIds;

		private class Subscriber : IDisposable
		{
			public void Dispose()
			{
				ArrayHelpers.Erase<RemoteInputPlayerConnection.Subscriber>(ref this.owner.m_Subscribers, this);
			}

			public RemoteInputPlayerConnection owner;

			public IObserver<InputRemoting.Message> observer;
		}
	}
}
