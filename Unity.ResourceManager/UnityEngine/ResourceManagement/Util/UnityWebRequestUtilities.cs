using System;
using System.Diagnostics;
using UnityEngine.Networking;

namespace UnityEngine.ResourceManagement.Util
{
	public class UnityWebRequestUtilities
	{
		public static bool RequestHasErrors(UnityWebRequest webReq, out UnityWebRequestResult result)
		{
			result = null;
			if (webReq == null || !webReq.isDone)
			{
				return false;
			}
			UnityWebRequest.Result result2 = webReq.result;
			if (result2 <= UnityWebRequest.Result.Success)
			{
				return false;
			}
			if (result2 - UnityWebRequest.Result.ConnectionError > 2)
			{
				throw new NotImplementedException(string.Format("Cannot determine whether UnityWebRequest succeeded or not from result : {0}", webReq.result));
			}
			result = new UnityWebRequestResult(webReq);
			return true;
		}

		public static bool IsAssetBundleDownloaded(UnityWebRequestAsyncOperation op)
		{
			DownloadHandlerAssetBundle downloadHandlerAssetBundle = (DownloadHandlerAssetBundle)op.webRequest.downloadHandler;
			if (downloadHandlerAssetBundle != null && downloadHandlerAssetBundle.autoLoadAssetBundle)
			{
				return downloadHandlerAssetBundle.isDownloadComplete;
			}
			return op.isDone;
		}

		internal static void LogOperationResult(AsyncOperation op)
		{
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = op as UnityWebRequestAsyncOperation;
			if (unityWebRequestAsyncOperation != null)
			{
				UnityWebRequestResult unityWebRequestResult = new UnityWebRequestResult(unityWebRequestAsyncOperation.webRequest);
				if (unityWebRequestResult.Result == UnityWebRequest.Result.Success)
				{
					return;
				}
				UnityWebRequestUtilities.LogError(unityWebRequestResult.ToString());
			}
		}

		[Conditional("ADDRESSABLES_LOG_ALL")]
		internal static void Log(string msg)
		{
			Debug.Log(msg);
		}

		internal static void LogError(string msg)
		{
			Debug.LogError(msg);
		}

		private const string k_AddressablesLogConditional = "ADDRESSABLES_LOG_ALL";
	}
}
