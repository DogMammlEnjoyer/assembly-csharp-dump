using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public sealed class InputRemoting : IObservable<InputRemoting.Message>, IObserver<InputRemoting.Message>
	{
		public bool sending
		{
			get
			{
				return (this.m_Flags & InputRemoting.Flags.Sending) == InputRemoting.Flags.Sending;
			}
			private set
			{
				if (value)
				{
					this.m_Flags |= InputRemoting.Flags.Sending;
					return;
				}
				this.m_Flags &= ~InputRemoting.Flags.Sending;
			}
		}

		internal InputRemoting(InputManager manager, bool startSendingOnConnect = false)
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			this.m_LocalManager = manager;
			if (startSendingOnConnect)
			{
				this.m_Flags |= InputRemoting.Flags.StartSendingOnConnect;
			}
		}

		public void StartSending()
		{
			if (this.sending)
			{
				return;
			}
			this.m_LocalManager.onEvent += this.SendEvent;
			this.m_LocalManager.onDeviceChange += this.SendDeviceChange;
			this.m_LocalManager.onLayoutChange += this.SendLayoutChange;
			this.sending = true;
			this.SendInitialMessages();
		}

		public void StopSending()
		{
			if (!this.sending)
			{
				return;
			}
			this.m_LocalManager.onEvent -= this.SendEvent;
			this.m_LocalManager.onDeviceChange -= this.SendDeviceChange;
			this.m_LocalManager.onLayoutChange -= this.SendLayoutChange;
			this.sending = false;
		}

		void IObserver<InputRemoting.Message>.OnNext(InputRemoting.Message msg)
		{
			switch (msg.type)
			{
			case InputRemoting.MessageType.Connect:
				InputRemoting.ConnectMsg.Process(this);
				return;
			case InputRemoting.MessageType.Disconnect:
				InputRemoting.DisconnectMsg.Process(this, msg);
				return;
			case InputRemoting.MessageType.NewLayout:
				InputRemoting.NewLayoutMsg.Process(this, msg);
				return;
			case InputRemoting.MessageType.NewDevice:
				InputRemoting.NewDeviceMsg.Process(this, msg);
				return;
			case InputRemoting.MessageType.NewEvents:
				InputRemoting.NewEventsMsg.Process(this, msg);
				return;
			case InputRemoting.MessageType.RemoveDevice:
				InputRemoting.RemoveDeviceMsg.Process(this, msg);
				return;
			case InputRemoting.MessageType.RemoveLayout:
				break;
			case InputRemoting.MessageType.ChangeUsages:
				InputRemoting.ChangeUsageMsg.Process(this, msg);
				return;
			case InputRemoting.MessageType.StartSending:
				InputRemoting.StartSendingMsg.Process(this);
				return;
			case InputRemoting.MessageType.StopSending:
				InputRemoting.StopSendingMsg.Process(this);
				break;
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

		public IDisposable Subscribe(IObserver<InputRemoting.Message> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}
			InputRemoting.Subscriber subscriber = new InputRemoting.Subscriber
			{
				owner = this,
				observer = observer
			};
			ArrayHelpers.Append<InputRemoting.Subscriber>(ref this.m_Subscribers, subscriber);
			return subscriber;
		}

		private void SendInitialMessages()
		{
			this.SendAllGeneratedLayouts();
			this.SendAllDevices();
		}

		private void SendAllGeneratedLayouts()
		{
			foreach (KeyValuePair<InternedString, Func<InputControlLayout>> keyValuePair in this.m_LocalManager.m_Layouts.layoutBuilders)
			{
				this.SendLayout(keyValuePair.Key);
			}
		}

		private void SendLayout(string layoutName)
		{
			if (this.m_Subscribers == null)
			{
				return;
			}
			InputRemoting.Message? message = InputRemoting.NewLayoutMsg.Create(this, layoutName);
			if (message != null)
			{
				this.Send(message.Value);
			}
		}

		private void SendAllDevices()
		{
			foreach (InputDevice device in this.m_LocalManager.devices)
			{
				this.SendDevice(device);
			}
		}

		private void SendDevice(InputDevice device)
		{
			if (this.m_Subscribers == null)
			{
				return;
			}
			if (device.remote)
			{
				return;
			}
			InputRemoting.Message msg = InputRemoting.NewDeviceMsg.Create(device);
			this.Send(msg);
			InputRemoting.Message msg2 = InputRemoting.NewEventsMsg.CreateStateEvent(device);
			this.Send(msg2);
		}

		private void SendEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (this.m_Subscribers == null)
			{
				return;
			}
			if (device != null && device.remote)
			{
				return;
			}
			InputRemoting.Message msg = InputRemoting.NewEventsMsg.Create(eventPtr.data, 1);
			this.Send(msg);
		}

		private void SendDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (this.m_Subscribers == null)
			{
				return;
			}
			if (device.remote)
			{
				return;
			}
			InputRemoting.Message msg;
			if (change != InputDeviceChange.Added)
			{
				if (change != InputDeviceChange.Removed)
				{
					switch (change)
					{
					case InputDeviceChange.UsageChanged:
						msg = InputRemoting.ChangeUsageMsg.Create(device);
						break;
					case InputDeviceChange.ConfigurationChanged:
						return;
					case InputDeviceChange.SoftReset:
						msg = InputRemoting.NewEventsMsg.CreateResetEvent(device, false);
						break;
					case InputDeviceChange.HardReset:
						msg = InputRemoting.NewEventsMsg.CreateResetEvent(device, true);
						break;
					default:
						return;
					}
				}
				else
				{
					msg = InputRemoting.RemoveDeviceMsg.Create(device);
				}
			}
			else
			{
				msg = InputRemoting.NewDeviceMsg.Create(device);
			}
			this.Send(msg);
		}

		private void SendLayoutChange(string layout, InputControlLayoutChange change)
		{
			if (this.m_Subscribers == null)
			{
				return;
			}
			if (!this.m_LocalManager.m_Layouts.IsGeneratedLayout(new InternedString(layout)))
			{
				return;
			}
			if (change != InputControlLayoutChange.Added && change != InputControlLayoutChange.Replaced)
			{
				return;
			}
			InputRemoting.Message? message = InputRemoting.NewLayoutMsg.Create(this, layout);
			if (message != null)
			{
				this.Send(message.Value);
			}
		}

		private void Send(InputRemoting.Message msg)
		{
			InputRemoting.Subscriber[] subscribers = this.m_Subscribers;
			for (int i = 0; i < subscribers.Length; i++)
			{
				subscribers[i].observer.OnNext(msg);
			}
		}

		private int FindOrCreateSenderRecord(int senderId)
		{
			if (this.m_Senders != null)
			{
				int num = this.m_Senders.Length;
				for (int i = 0; i < num; i++)
				{
					if (this.m_Senders[i].senderId == senderId)
					{
						return i;
					}
				}
			}
			InputRemoting.RemoteSender value = new InputRemoting.RemoteSender
			{
				senderId = senderId
			};
			return ArrayHelpers.Append<InputRemoting.RemoteSender>(ref this.m_Senders, value);
		}

		private static InternedString BuildLayoutNamespace(int senderId)
		{
			return new InternedString(string.Format("Remote::{0}", senderId));
		}

		private int FindLocalDeviceId(int remoteDeviceId, int senderIndex)
		{
			InputRemoting.RemoteInputDevice[] devices = this.m_Senders[senderIndex].devices;
			if (devices != null)
			{
				int num = devices.Length;
				for (int i = 0; i < num; i++)
				{
					if (devices[i].remoteId == remoteDeviceId)
					{
						return devices[i].localId;
					}
				}
			}
			return 0;
		}

		private InputDevice TryGetDeviceByRemoteId(int remoteDeviceId, int senderIndex)
		{
			int id = this.FindLocalDeviceId(remoteDeviceId, senderIndex);
			return this.m_LocalManager.TryGetDeviceById(id);
		}

		internal InputManager manager
		{
			get
			{
				return this.m_LocalManager;
			}
		}

		public void RemoveRemoteDevices(int participantId)
		{
			int num = this.FindOrCreateSenderRecord(participantId);
			InputRemoting.RemoteInputDevice[] devices = this.m_Senders[num].devices;
			if (devices != null)
			{
				foreach (InputRemoting.RemoteInputDevice remoteInputDevice in devices)
				{
					InputDevice inputDevice = this.m_LocalManager.TryGetDeviceById(remoteInputDevice.localId);
					if (inputDevice != null)
					{
						this.m_LocalManager.RemoveDevice(inputDevice, false);
					}
				}
			}
			ArrayHelpers.EraseAt<InputRemoting.RemoteSender>(ref this.m_Senders, num);
		}

		private static byte[] SerializeData<TData>(TData data)
		{
			string s = JsonUtility.ToJson(data);
			return Encoding.UTF8.GetBytes(s);
		}

		private static TData DeserializeData<TData>(byte[] data)
		{
			return JsonUtility.FromJson<TData>(Encoding.UTF8.GetString(data));
		}

		private InputRemoting.Flags m_Flags;

		private InputManager m_LocalManager;

		private InputRemoting.Subscriber[] m_Subscribers;

		private InputRemoting.RemoteSender[] m_Senders;

		public enum MessageType
		{
			Connect,
			Disconnect,
			NewLayout,
			NewDevice,
			NewEvents,
			RemoveDevice,
			RemoveLayout,
			ChangeUsages,
			StartSending,
			StopSending
		}

		public struct Message
		{
			public int participantId;

			public InputRemoting.MessageType type;

			public byte[] data;
		}

		[Flags]
		private enum Flags
		{
			Sending = 1,
			StartSendingOnConnect = 2
		}

		[Serializable]
		internal struct RemoteSender
		{
			public int senderId;

			public InternedString[] layouts;

			public InputRemoting.RemoteInputDevice[] devices;
		}

		[Serializable]
		internal struct RemoteInputDevice
		{
			public int remoteId;

			public int localId;

			public InputDeviceDescription description;
		}

		internal class Subscriber : IDisposable
		{
			public void Dispose()
			{
				ArrayHelpers.Erase<InputRemoting.Subscriber>(ref this.owner.m_Subscribers, this);
			}

			public InputRemoting owner;

			public IObserver<InputRemoting.Message> observer;
		}

		private static class ConnectMsg
		{
			public static void Process(InputRemoting receiver)
			{
				if (receiver.sending)
				{
					receiver.SendInitialMessages();
					return;
				}
				if ((receiver.m_Flags & InputRemoting.Flags.StartSendingOnConnect) == InputRemoting.Flags.StartSendingOnConnect)
				{
					receiver.StartSending();
				}
			}
		}

		private static class StartSendingMsg
		{
			public static void Process(InputRemoting receiver)
			{
				receiver.StartSending();
			}
		}

		private static class StopSendingMsg
		{
			public static void Process(InputRemoting receiver)
			{
				receiver.StopSending();
			}
		}

		private static class DisconnectMsg
		{
			public static void Process(InputRemoting receiver, InputRemoting.Message msg)
			{
				Debug.Log("DisconnectMsg.Process");
				receiver.RemoveRemoteDevices(msg.participantId);
				receiver.StopSending();
			}
		}

		private static class NewLayoutMsg
		{
			public static InputRemoting.Message? Create(InputRemoting sender, string layoutName)
			{
				InputControlLayout inputControlLayout;
				try
				{
					inputControlLayout = sender.m_LocalManager.TryLoadControlLayout(new InternedString(layoutName));
					if (inputControlLayout == null)
					{
						Debug.Log(string.Format("Could not find layout '{0}' meant to be sent through remote connection; this should not happen", layoutName));
						return null;
					}
				}
				catch (Exception arg)
				{
					Debug.Log(string.Format("Could not load layout '{0}'; not sending to remote listeners (exception: {1})", layoutName, arg));
					return null;
				}
				InputRemoting.NewLayoutMsg.Data data = new InputRemoting.NewLayoutMsg.Data
				{
					name = layoutName,
					layoutJson = inputControlLayout.ToJson(),
					isOverride = inputControlLayout.isOverride
				};
				return new InputRemoting.Message?(new InputRemoting.Message
				{
					type = InputRemoting.MessageType.NewLayout,
					data = InputRemoting.SerializeData<InputRemoting.NewLayoutMsg.Data>(data)
				});
			}

			public static void Process(InputRemoting receiver, InputRemoting.Message msg)
			{
				InputRemoting.NewLayoutMsg.Data data = InputRemoting.DeserializeData<InputRemoting.NewLayoutMsg.Data>(msg.data);
				int num = receiver.FindOrCreateSenderRecord(msg.participantId);
				InternedString value = new InternedString(data.name);
				receiver.m_LocalManager.RegisterControlLayout(data.layoutJson, data.name, data.isOverride);
				ArrayHelpers.Append<InternedString>(ref receiver.m_Senders[num].layouts, value);
			}

			[Serializable]
			public struct Data
			{
				public string name;

				public string layoutJson;

				public bool isOverride;
			}
		}

		private static class NewDeviceMsg
		{
			public static InputRemoting.Message Create(InputDevice device)
			{
				InputRemoting.NewDeviceMsg.Data data = default(InputRemoting.NewDeviceMsg.Data);
				data.name = device.name;
				data.layout = device.layout;
				data.deviceId = device.deviceId;
				data.description = device.description;
				data.usages = (from x in device.usages
				select x.ToString()).ToArray<string>();
				InputRemoting.NewDeviceMsg.Data data2 = data;
				return new InputRemoting.Message
				{
					type = InputRemoting.MessageType.NewDevice,
					data = InputRemoting.SerializeData<InputRemoting.NewDeviceMsg.Data>(data2)
				};
			}

			public static void Process(InputRemoting receiver, InputRemoting.Message msg)
			{
				int num = receiver.FindOrCreateSenderRecord(msg.participantId);
				InputRemoting.NewDeviceMsg.Data data = InputRemoting.DeserializeData<InputRemoting.NewDeviceMsg.Data>(msg.data);
				InputRemoting.RemoteInputDevice[] devices = receiver.m_Senders[num].devices;
				if (devices != null)
				{
					InputRemoting.RemoteInputDevice[] array = devices;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].remoteId == data.deviceId)
						{
							Debug.LogError(string.Format("Already received device with id {0} (layout '{1}', description '{3}) from remote {2}", new object[]
							{
								data.deviceId,
								data.layout,
								msg.participantId,
								data.description
							}));
							return;
						}
					}
				}
				InputDevice inputDevice;
				try
				{
					InternedString str = new InternedString(data.layout);
					inputDevice = receiver.m_LocalManager.AddDevice(str, data.name, default(InternedString));
					inputDevice.m_ParticipantId = msg.participantId;
				}
				catch (Exception arg)
				{
					Debug.LogError(string.Format("Could not create remote device '{0}' with layout '{1}' locally (exception: {2})", data.description, data.layout, arg));
					return;
				}
				inputDevice.m_Description = data.description;
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Remote;
				foreach (string text in data.usages)
				{
					receiver.m_LocalManager.AddDeviceUsage(inputDevice, new InternedString(text));
				}
				InputRemoting.RemoteInputDevice value = new InputRemoting.RemoteInputDevice
				{
					remoteId = data.deviceId,
					localId = inputDevice.deviceId,
					description = data.description
				};
				ArrayHelpers.Append<InputRemoting.RemoteInputDevice>(ref receiver.m_Senders[num].devices, value);
			}

			[Serializable]
			public struct Data
			{
				public string name;

				public string layout;

				public int deviceId;

				public string[] usages;

				public InputDeviceDescription description;
			}
		}

		private static class NewEventsMsg
		{
			public unsafe static InputRemoting.Message CreateResetEvent(InputDevice device, bool isHardReset)
			{
				DeviceResetEvent deviceResetEvent = DeviceResetEvent.Create(device.deviceId, isHardReset, -1.0);
				return InputRemoting.NewEventsMsg.Create((InputEvent*)UnsafeUtility.AddressOf<DeviceResetEvent>(ref deviceResetEvent), 1);
			}

			public static InputRemoting.Message CreateStateEvent(InputDevice device)
			{
				InputEventPtr inputEventPtr;
				InputRemoting.Message result;
				using (StateEvent.From(device, out inputEventPtr, Allocator.Temp))
				{
					result = InputRemoting.NewEventsMsg.Create(inputEventPtr.data, 1);
				}
				return result;
			}

			public unsafe static InputRemoting.Message Create(InputEvent* events, int eventCount)
			{
				uint num = 0U;
				InputEventPtr inputEventPtr = new InputEventPtr(events);
				int i = 0;
				while (i < eventCount)
				{
					num = num.AlignToMultipleOf(4U) + inputEventPtr.sizeInBytes;
					i++;
					inputEventPtr = inputEventPtr.Next();
				}
				byte[] array = new byte[num];
				byte[] array2;
				byte* destination;
				if ((array2 = array) == null || array2.Length == 0)
				{
					destination = null;
				}
				else
				{
					destination = &array2[0];
				}
				UnsafeUtility.MemCpy((void*)destination, (void*)events, (long)((ulong)num));
				array2 = null;
				return new InputRemoting.Message
				{
					type = InputRemoting.MessageType.NewEvents,
					data = array
				};
			}

			public unsafe static void Process(InputRemoting receiver, InputRemoting.Message msg)
			{
				InputManager localManager = receiver.m_LocalManager;
				byte[] array;
				byte* ptr;
				if ((array = msg.data) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				IntPtr intPtr = new IntPtr((void*)(ptr + msg.data.Length));
				int num = 0;
				InputEventPtr ptr2 = new InputEventPtr((InputEvent*)ptr);
				int senderIndex = receiver.FindOrCreateSenderRecord(msg.participantId);
				while (ptr2.data < (InputEvent*)intPtr.ToPointer())
				{
					int deviceId = ptr2.deviceId;
					int num2 = receiver.FindLocalDeviceId(deviceId, senderIndex);
					ptr2.deviceId = num2;
					if (num2 != 0)
					{
						localManager.QueueEvent(ptr2);
					}
					num++;
					ptr2 = ptr2.Next();
				}
				array = null;
			}
		}

		private static class ChangeUsageMsg
		{
			public static InputRemoting.Message Create(InputDevice device)
			{
				InputRemoting.ChangeUsageMsg.Data data = default(InputRemoting.ChangeUsageMsg.Data);
				data.deviceId = device.deviceId;
				data.usages = (from x in device.usages
				select x.ToString()).ToArray<string>();
				InputRemoting.ChangeUsageMsg.Data data2 = data;
				return new InputRemoting.Message
				{
					type = InputRemoting.MessageType.ChangeUsages,
					data = InputRemoting.SerializeData<InputRemoting.ChangeUsageMsg.Data>(data2)
				};
			}

			public static void Process(InputRemoting receiver, InputRemoting.Message msg)
			{
				int senderIndex = receiver.FindOrCreateSenderRecord(msg.participantId);
				InputRemoting.ChangeUsageMsg.Data data = InputRemoting.DeserializeData<InputRemoting.ChangeUsageMsg.Data>(msg.data);
				InputDevice inputDevice = receiver.TryGetDeviceByRemoteId(data.deviceId, senderIndex);
				if (inputDevice != null)
				{
					foreach (InternedString str in inputDevice.usages)
					{
						if (!data.usages.Contains(str))
						{
							receiver.m_LocalManager.RemoveDeviceUsage(inputDevice, new InternedString(str));
						}
					}
					foreach (string text in data.usages)
					{
						InternedString value = new InternedString(text);
						if (!inputDevice.usages.Contains(value))
						{
							receiver.m_LocalManager.AddDeviceUsage(inputDevice, new InternedString(text));
						}
					}
				}
			}

			[Serializable]
			public struct Data
			{
				public int deviceId;

				public string[] usages;
			}
		}

		private static class RemoveDeviceMsg
		{
			public static InputRemoting.Message Create(InputDevice device)
			{
				return new InputRemoting.Message
				{
					type = InputRemoting.MessageType.RemoveDevice,
					data = BitConverter.GetBytes(device.deviceId)
				};
			}

			public static void Process(InputRemoting receiver, InputRemoting.Message msg)
			{
				int senderIndex = receiver.FindOrCreateSenderRecord(msg.participantId);
				int remoteDeviceId = BitConverter.ToInt32(msg.data, 0);
				InputDevice inputDevice = receiver.TryGetDeviceByRemoteId(remoteDeviceId, senderIndex);
				if (inputDevice != null)
				{
					receiver.m_LocalManager.RemoveDevice(inputDevice, false);
				}
			}
		}
	}
}
