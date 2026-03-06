using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement
{
	public static class WebRequestQueue
	{
		public static void SetMaxConcurrentRequests(int maxRequests)
		{
			if (maxRequests < 1)
			{
				throw new ArgumentException("MaxRequests must be 1 or greater.", "maxRequests");
			}
			WebRequestQueue.s_MaxRequest = maxRequests;
		}

		public static WebRequestQueueOperation QueueRequest(UnityWebRequest request)
		{
			WebRequestQueueOperation webRequestQueueOperation = new WebRequestQueueOperation(request);
			if (WebRequestQueue.s_ActiveRequests.Count < WebRequestQueue.s_MaxRequest)
			{
				WebRequestQueue.BeginWebRequest(webRequestQueueOperation);
			}
			else
			{
				WebRequestQueue.s_QueuedOperations.Enqueue(webRequestQueueOperation);
			}
			return webRequestQueueOperation;
		}

		public static void WaitForRequestToBeActive(WebRequestQueueOperation request, int millisecondsTimeout)
		{
			List<UnityWebRequestAsyncOperation> list = new List<UnityWebRequestAsyncOperation>();
			while (WebRequestQueue.s_QueuedOperations.Contains(request))
			{
				list.Clear();
				foreach (UnityWebRequestAsyncOperation unityWebRequestAsyncOperation in WebRequestQueue.s_ActiveRequests)
				{
					if (UnityWebRequestUtilities.IsAssetBundleDownloaded(unityWebRequestAsyncOperation))
					{
						list.Add(unityWebRequestAsyncOperation);
					}
				}
				foreach (UnityWebRequestAsyncOperation unityWebRequestAsyncOperation2 in list)
				{
					bool flag = WebRequestQueue.s_QueuedOperations.Peek() == request;
					unityWebRequestAsyncOperation2.completed -= WebRequestQueue.OnWebAsyncOpComplete;
					WebRequestQueue.OnWebAsyncOpComplete(unityWebRequestAsyncOperation2);
					if (flag)
					{
						return;
					}
				}
				Thread.Sleep(millisecondsTimeout);
			}
		}

		internal static void DequeueRequest(UnityWebRequestAsyncOperation operation)
		{
			operation.completed -= WebRequestQueue.OnWebAsyncOpComplete;
			WebRequestQueue.OnWebAsyncOpComplete(operation);
		}

		private static void OnWebAsyncOpComplete(AsyncOperation operation)
		{
			WebRequestQueue.OnWebAsyncOpComplete(operation as UnityWebRequestAsyncOperation);
		}

		private static void OnWebAsyncOpComplete(UnityWebRequestAsyncOperation operation)
		{
			if (WebRequestQueue.s_ActiveRequests.Remove(operation) && WebRequestQueue.s_QueuedOperations.Count > 0)
			{
				WebRequestQueue.BeginWebRequest(WebRequestQueue.s_QueuedOperations.Dequeue());
			}
		}

		private static void BeginWebRequest(WebRequestQueueOperation queueOperation)
		{
			UnityWebRequest webRequest = queueOperation.m_WebRequest;
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = null;
			try
			{
				unityWebRequestAsyncOperation = webRequest.SendWebRequest();
				if (unityWebRequestAsyncOperation != null)
				{
					WebRequestQueue.s_ActiveRequests.Add(unityWebRequestAsyncOperation);
					if (unityWebRequestAsyncOperation.isDone)
					{
						WebRequestQueue.OnWebAsyncOpComplete(unityWebRequestAsyncOperation);
					}
					else
					{
						unityWebRequestAsyncOperation.completed += WebRequestQueue.OnWebAsyncOpComplete;
					}
				}
				else
				{
					WebRequestQueue.OnWebAsyncOpComplete(null);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			queueOperation.Complete(unityWebRequestAsyncOperation);
		}

		internal static int s_MaxRequest = 3;

		internal static Queue<WebRequestQueueOperation> s_QueuedOperations = new Queue<WebRequestQueueOperation>();

		internal static List<UnityWebRequestAsyncOperation> s_ActiveRequests = new List<UnityWebRequestAsyncOperation>();
	}
}
