using System;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Utilities
{
	internal class ForDeviceEventObservable : IObservable<InputEventPtr>
	{
		public ForDeviceEventObservable(IObservable<InputEventPtr> source, Type deviceType, InputDevice device)
		{
			this.m_Source = source;
			this.m_DeviceType = deviceType;
			this.m_Device = device;
		}

		public IDisposable Subscribe(IObserver<InputEventPtr> observer)
		{
			return this.m_Source.Subscribe(new ForDeviceEventObservable.ForDevice(this.m_DeviceType, this.m_Device, observer));
		}

		private IObservable<InputEventPtr> m_Source;

		private InputDevice m_Device;

		private Type m_DeviceType;

		private class ForDevice : IObserver<InputEventPtr>
		{
			public ForDevice(Type deviceType, InputDevice device, IObserver<InputEventPtr> observer)
			{
				this.m_Device = device;
				this.m_DeviceType = deviceType;
				this.m_Observer = observer;
			}

			public void OnCompleted()
			{
			}

			public void OnError(Exception error)
			{
				Debug.LogException(error);
			}

			public void OnNext(InputEventPtr value)
			{
				if (this.m_DeviceType != null)
				{
					InputDevice deviceById = InputSystem.GetDeviceById(value.deviceId);
					if (deviceById == null)
					{
						return;
					}
					if (!this.m_DeviceType.IsInstanceOfType(deviceById))
					{
						return;
					}
				}
				if (this.m_Device != null && value.deviceId != this.m_Device.deviceId)
				{
					return;
				}
				this.m_Observer.OnNext(value);
			}

			private IObserver<InputEventPtr> m_Observer;

			private InputDevice m_Device;

			private Type m_DeviceType;
		}
	}
}
