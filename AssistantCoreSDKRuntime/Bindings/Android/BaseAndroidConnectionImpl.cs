using System;
using UnityEngine;

namespace Oculus.Voice.Core.Bindings.Android
{
	public class BaseAndroidConnectionImpl<T> where T : BaseServiceBinding
	{
		public bool IsConnected
		{
			get
			{
				return this.serviceConnection.IsConnected;
			}
		}

		public BaseAndroidConnectionImpl(string className)
		{
			this.fragmentClassName = className;
			this.serviceConnection = new AndroidServiceConnection(className, "getService");
		}

		public virtual void Connect(string version)
		{
			this.serviceConnection.Connect(version);
			AndroidJavaObject androidJavaObject = this.serviceConnection.GetService();
			if (androidJavaObject == null)
			{
				return;
			}
			this.service = (T)((object)Activator.CreateInstance(typeof(T), new object[]
			{
				androidJavaObject
			}));
		}

		public virtual void Disconnect()
		{
			this.service.Shutdown();
			this.serviceConnection.Disconnect();
			this.service = default(T);
		}

		private string fragmentClassName;

		protected T service;

		protected readonly AndroidServiceConnection serviceConnection;
	}
}
