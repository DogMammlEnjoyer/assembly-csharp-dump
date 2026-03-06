using System;
using System.ComponentModel;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Text Data Provider")]
	public class TextDataProvider : ResourceProviderBase
	{
		public bool IgnoreFailures { get; set; }

		public virtual object Convert(Type type, string text)
		{
			return text;
		}

		public override void Provide(ProvideHandle provideHandle)
		{
			new TextDataProvider.InternalOp().Start(provideHandle, this);
		}

		internal class InternalOp
		{
			private float GetPercentComplete()
			{
				if (this.m_RequestOperation == null)
				{
					return 0f;
				}
				return this.m_RequestOperation.progress;
			}

			public void Start(ProvideHandle provideHandle, TextDataProvider rawProvider)
			{
				this.m_PI = provideHandle;
				this.m_PI.SetWaitForCompletionCallback(new Func<bool>(this.WaitForCompletionHandler));
				provideHandle.SetProgressCallback(new Func<float>(this.GetPercentComplete));
				this.m_Provider = rawProvider;
				ProviderLoadRequestOptions providerLoadRequestOptions = this.m_PI.Location.Data as ProviderLoadRequestOptions;
				if (providerLoadRequestOptions != null)
				{
					this.m_IgnoreFailures = providerLoadRequestOptions.IgnoreFailures;
					this.m_Timeout = providerLoadRequestOptions.WebRequestTimeout;
				}
				else
				{
					this.m_IgnoreFailures = rawProvider.IgnoreFailures;
					this.m_Timeout = 0;
				}
				string text = this.m_PI.ResourceManager.TransformInternalId(this.m_PI.Location);
				if (ResourceManagerConfig.ShouldPathUseWebRequest(text))
				{
					this.SendWebRequest(text);
					return;
				}
				if (File.Exists(text))
				{
					string text2 = File.ReadAllText(text);
					object obj = this.ConvertText(text2);
					this.m_PI.Complete<object>(obj, obj != null, (obj == null) ? new Exception(string.Format("Unable to load asset of type {0} from location {1}.", this.m_PI.Type, this.m_PI.Location)) : null);
					this.m_Complete = true;
					return;
				}
				Exception exception = null;
				if (this.m_IgnoreFailures)
				{
					this.m_PI.Complete<object>(null, true, exception);
					this.m_Complete = true;
					return;
				}
				exception = new Exception(string.Format("Invalid path in TextDataProvider : '{0}'.", text));
				this.m_PI.Complete<object>(null, false, exception);
				this.m_Complete = true;
			}

			private bool WaitForCompletionHandler()
			{
				if (this.m_Complete)
				{
					return true;
				}
				if (this.m_RequestOperation != null)
				{
					if (this.m_RequestOperation.isDone && !this.m_Complete)
					{
						this.RequestOperation_completed(this.m_RequestOperation);
					}
					else if (!this.m_RequestOperation.isDone)
					{
						return false;
					}
				}
				return this.m_Complete;
			}

			private void RequestOperation_completed(AsyncOperation op)
			{
				if (this.m_Complete)
				{
					return;
				}
				UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = op as UnityWebRequestAsyncOperation;
				string text = null;
				Exception exception = null;
				if (unityWebRequestAsyncOperation != null)
				{
					UnityWebRequest webRequest = unityWebRequestAsyncOperation.webRequest;
					UnityWebRequestResult uwrResult;
					if (!UnityWebRequestUtilities.RequestHasErrors(webRequest, out uwrResult))
					{
						text = webRequest.downloadHandler.text;
					}
					else
					{
						exception = new RemoteProviderException("TextDataProvider : unable to load from url : " + webRequest.url, this.m_PI.Location, uwrResult, null);
					}
					webRequest.Dispose();
				}
				else
				{
					exception = new RemoteProviderException("TextDataProvider unable to load from unknown url", this.m_PI.Location, null, null);
				}
				this.CompleteOperation(text, exception);
			}

			protected void CompleteOperation(string text, Exception exception)
			{
				object obj = null;
				if (!string.IsNullOrEmpty(text))
				{
					obj = this.ConvertText(text);
				}
				this.m_PI.Complete<object>(obj, obj != null || this.m_IgnoreFailures, exception);
				this.m_Complete = true;
			}

			private object ConvertText(string text)
			{
				object result;
				try
				{
					result = this.m_Provider.Convert(this.m_PI.Type, text);
				}
				catch (Exception exception)
				{
					if (!this.m_IgnoreFailures)
					{
						Debug.LogException(exception);
					}
					result = null;
				}
				return result;
			}

			protected virtual void SendWebRequest(string path)
			{
				try
				{
					path = path.Replace('\\', '/');
					UnityWebRequest unityWebRequest = new UnityWebRequest(path, "GET", new DownloadHandlerBuffer(), null);
					if (this.m_Timeout > 0)
					{
						unityWebRequest.timeout = this.m_Timeout;
					}
					Action<UnityWebRequest> webRequestOverride = this.m_PI.ResourceManager.WebRequestOverride;
					if (webRequestOverride != null)
					{
						webRequestOverride(unityWebRequest);
					}
					this.m_RequestQueueOperation = WebRequestQueue.QueueRequest(unityWebRequest);
					if (this.m_RequestQueueOperation.IsDone)
					{
						this.m_RequestOperation = this.m_RequestQueueOperation.Result;
						if (this.m_RequestOperation.isDone)
						{
							this.RequestOperation_completed(this.m_RequestOperation);
						}
						else
						{
							this.m_RequestOperation.completed += this.RequestOperation_completed;
						}
					}
					else
					{
						WebRequestQueueOperation requestQueueOperation = this.m_RequestQueueOperation;
						requestQueueOperation.OnComplete = (Action<UnityWebRequestAsyncOperation>)Delegate.Combine(requestQueueOperation.OnComplete, new Action<UnityWebRequestAsyncOperation>(delegate(UnityWebRequestAsyncOperation asyncOperation)
						{
							this.m_RequestOperation = asyncOperation;
							this.m_RequestOperation.completed += this.RequestOperation_completed;
						}));
					}
				}
				catch (UriFormatException e)
				{
					throw new UriFormatException("Invalid path '" + path + "'", e);
				}
			}

			private TextDataProvider m_Provider;

			private UnityWebRequestAsyncOperation m_RequestOperation;

			private WebRequestQueueOperation m_RequestQueueOperation;

			private ProvideHandle m_PI;

			private bool m_IgnoreFailures;

			private bool m_Complete;

			private int m_Timeout;
		}
	}
}
