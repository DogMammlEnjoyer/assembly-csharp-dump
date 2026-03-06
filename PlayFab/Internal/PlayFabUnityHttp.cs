using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;
using PlayFab.SharedModels;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayFab.Internal
{
	public class PlayFabUnityHttp : ITransportPlugin, IPlayFabPlugin
	{
		public bool IsInitialized
		{
			get
			{
				return this._isInitialized;
			}
		}

		public void Initialize()
		{
			this._isInitialized = true;
		}

		public void Update()
		{
		}

		public void OnDestroy()
		{
		}

		public void SimpleGetCall(string fullUrl, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(PlayFabUnityHttp.SimpleCallCoroutine("get", fullUrl, null, successCallback, errorCallback));
		}

		public void SimplePutCall(string fullUrl, byte[] payload, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(PlayFabUnityHttp.SimpleCallCoroutine("put", fullUrl, payload, successCallback, errorCallback));
		}

		public void SimplePostCall(string fullUrl, byte[] payload, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(PlayFabUnityHttp.SimpleCallCoroutine("post", fullUrl, payload, successCallback, errorCallback));
		}

		private static IEnumerator SimpleCallCoroutine(string method, string fullUrl, byte[] payload, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			if (payload == null)
			{
				using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
				{
					yield return www.SendWebRequest();
					if (!string.IsNullOrEmpty(www.error))
					{
						errorCallback(www.error);
					}
					else
					{
						successCallback(www.downloadHandler.data);
					}
				}
				UnityWebRequest www = null;
			}
			else
			{
				UnityWebRequest www;
				if (method == "put")
				{
					www = UnityWebRequest.Put(fullUrl, payload);
				}
				else
				{
					www = new UnityWebRequest(fullUrl, "POST");
					www.uploadHandler = new UploadHandlerRaw(payload);
					www.downloadHandler = new DownloadHandlerBuffer();
					www.SetRequestHeader("Content-Type", "application/json");
				}
				yield return www.SendWebRequest();
				if (www.isNetworkError || www.isHttpError)
				{
					errorCallback(www.error);
				}
				else
				{
					successCallback(www.downloadHandler.data);
				}
				www = null;
			}
			yield break;
			yield break;
		}

		public void MakeApiCall(object reqContainerObj)
		{
			CallRequestContainer callRequestContainer = (CallRequestContainer)reqContainerObj;
			callRequestContainer.RequestHeaders["Content-Type"] = "application/json";
			if (PlayFabSettings.CompressApiData)
			{
				callRequestContainer.RequestHeaders["Content-Encoding"] = "GZIP";
				callRequestContainer.RequestHeaders["Accept-Encoding"] = "GZIP";
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression))
					{
						gzipStream.Write(callRequestContainer.Payload, 0, callRequestContainer.Payload.Length);
					}
					callRequestContainer.Payload = memoryStream.ToArray();
				}
			}
			SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(this.Post(callRequestContainer));
		}

		private IEnumerator Post(CallRequestContainer reqContainer)
		{
			UnityWebRequest www = new UnityWebRequest(reqContainer.FullUrl)
			{
				uploadHandler = new UploadHandlerRaw(reqContainer.Payload),
				downloadHandler = new DownloadHandlerBuffer(),
				method = "POST"
			};
			foreach (KeyValuePair<string, string> keyValuePair in reqContainer.RequestHeaders)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Key) && !string.IsNullOrEmpty(keyValuePair.Value))
				{
					www.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					Debug.LogWarning("Null header: " + keyValuePair.Key + " = " + keyValuePair.Value);
				}
			}
			yield return www.SendWebRequest();
			if (!string.IsNullOrEmpty(www.error))
			{
				this.OnError(www.error, reqContainer);
			}
			else
			{
				try
				{
					byte[] data = www.downloadHandler.data;
					object obj = data != null && data[0] == 31 && data[1] == 139;
					string response = "Unexpected error: cannot decompress GZIP stream.";
					object obj2 = obj;
					if (obj2 == null && data != null)
					{
						response = Encoding.UTF8.GetString(data, 0, data.Length);
					}
					if (obj2 != null)
					{
						using (GZipStream gzipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, false))
						{
							byte[] array = new byte[4096];
							using (MemoryStream memoryStream = new MemoryStream())
							{
								int num;
								while ((num = gzipStream.Read(array, 0, array.Length)) > 0)
								{
									memoryStream.Write(array, 0, num);
								}
								memoryStream.Seek(0L, SeekOrigin.Begin);
								string response2 = new StreamReader(memoryStream).ReadToEnd();
								this.OnResponse(response2, reqContainer);
							}
							goto IL_228;
						}
					}
					this.OnResponse(response, reqContainer);
					IL_228:;
				}
				catch (Exception ex)
				{
					string str = "Unhandled error in PlayFabUnityHttp: ";
					Exception ex2 = ex;
					this.OnError(str + ((ex2 != null) ? ex2.ToString() : null), reqContainer);
				}
			}
			www.Dispose();
			yield break;
		}

		public int GetPendingMessages()
		{
			return this._pendingWwwMessages;
		}

		public void OnResponse(string response, CallRequestContainer reqContainer)
		{
			try
			{
				ISerializerPlugin plugin = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "");
				HttpResponseObject httpResponseObject = plugin.DeserializeObject<HttpResponseObject>(response);
				if (httpResponseObject.code == 200)
				{
					reqContainer.JsonResponse = plugin.SerializeObject(httpResponseObject.data);
					reqContainer.DeserializeResultJson();
					reqContainer.ApiResult.Request = reqContainer.ApiRequest;
					reqContainer.ApiResult.CustomData = reqContainer.CustomData;
					SingletonMonoBehaviour<PlayFabHttp>.instance.OnPlayFabApiResult(reqContainer);
					PlayFabDeviceUtil.OnPlayFabLogin(reqContainer.ApiResult, reqContainer.settings, reqContainer.instanceApi);
					try
					{
						PlayFabHttp.SendEvent(reqContainer.ApiEndpoint, reqContainer.ApiRequest, reqContainer.ApiResult, ApiProcessingEventType.Post);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
					try
					{
						reqContainer.InvokeSuccessCallback();
						goto IL_FD;
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						goto IL_FD;
					}
				}
				if (reqContainer.ErrorCallback != null)
				{
					reqContainer.Error = PlayFabHttp.GeneratePlayFabError(reqContainer.ApiEndpoint, response, reqContainer.CustomData);
					PlayFabHttp.SendErrorEvent(reqContainer.ApiRequest, reqContainer.Error);
					reqContainer.ErrorCallback(reqContainer.Error);
				}
				IL_FD:;
			}
			catch (Exception exception3)
			{
				Debug.LogException(exception3);
			}
		}

		public void OnError(string error, CallRequestContainer reqContainer)
		{
			reqContainer.JsonResponse = error;
			if (reqContainer.ErrorCallback != null)
			{
				reqContainer.Error = PlayFabHttp.GeneratePlayFabError(reqContainer.ApiEndpoint, reqContainer.JsonResponse, reqContainer.CustomData);
				PlayFabHttp.SendErrorEvent(reqContainer.ApiRequest, reqContainer.Error);
				reqContainer.ErrorCallback(reqContainer.Error);
			}
		}

		private bool _isInitialized;

		private readonly int _pendingWwwMessages;

		private int count;
	}
}
