using System;
using Oculus.Voice.Core.Bindings.Interfaces;
using UnityEngine;

namespace Oculus.Voice.Core.Bindings.Android
{
	public class AndroidServiceConnection : IConnection
	{
		public bool IsConnected
		{
			get
			{
				return this.mAssistantServiceConnection != null;
			}
		}

		public AndroidJavaObject AssistantServiceConnection
		{
			get
			{
				return this.mAssistantServiceConnection;
			}
		}

		public AndroidServiceConnection(string serviceFragmentClassName, string serviceGetterMethodName)
		{
			this.serviceFragmentClass = serviceFragmentClassName;
			this.serviceGetter = serviceGetterMethodName;
		}

		public void Connect(string version)
		{
			if (this.mAssistantServiceConnection == null)
			{
				try
				{
					AndroidJNIHelper.debug = true;
					AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
					using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(this.serviceFragmentClass))
					{
						this.mAssistantServiceConnection = androidJavaClass.CallStatic<AndroidJavaObject>("createAndAttach", new object[]
						{
							@static,
							version
						});
					}
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("AndroidServiceConnection Connect Failed\nService: {0}\nException:\n{1}\n\n", new object[]
					{
						this.serviceFragmentClass,
						ex
					});
				}
			}
		}

		public void Disconnect()
		{
			AndroidJavaObject androidJavaObject = this.mAssistantServiceConnection;
			if (androidJavaObject == null)
			{
				return;
			}
			androidJavaObject.Call("detach", Array.Empty<object>());
		}

		public AndroidJavaObject GetService()
		{
			AndroidJavaObject androidJavaObject = this.mAssistantServiceConnection;
			if (androidJavaObject == null)
			{
				return null;
			}
			return androidJavaObject.Call<AndroidJavaObject>(this.serviceGetter, Array.Empty<object>());
		}

		private AndroidJavaObject mAssistantServiceConnection;

		private string serviceFragmentClass;

		private string serviceGetter;
	}
}
