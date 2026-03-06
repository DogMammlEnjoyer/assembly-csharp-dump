using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Backtrace.Unity.Common;
using Backtrace.Unity.Extensions;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Types;
using UnityEngine;
using UnityEngine.Networking;

namespace Backtrace.Unity.Services
{
	internal class BacktraceApi : IBacktraceApi
	{
		[Obsolete("RequestHandler is obsolete. BacktraceApi won't be able to provide BacktraceData in every situation")]
		public Func<string, BacktraceData, BacktraceResult> RequestHandler { get; set; }

		public Action<Exception> OnServerError { get; set; }

		public Action<BacktraceResult> OnServerResponse { get; set; }

		public bool EnablePerformanceStatistics { get; set; }

		public string ServerUrl
		{
			get
			{
				return this._serverUrl.ToString();
			}
		}

		public BacktraceApi(BacktraceCredentials credentials, bool ignoreSslValidation = false)
		{
			this._credentials = credentials;
			if (this._credentials == null)
			{
				throw new ArgumentException(string.Format("{0} cannot be null", "BacktraceCredentials"));
			}
			this._serverUrl = credentials.GetSubmissionUrl();
			this._minidumpUrl = credentials.GetMinidumpSubmissionUrl().ToString();
			this._httpClient.IgnoreSslValidation = ignoreSslValidation;
			this.EnablePerformanceStatistics = false;
		}

		public IEnumerator SendMinidump(string minidumpPath, IEnumerable<string> attachments, IDictionary<string, string> attributes, Action<BacktraceResult> callback = null)
		{
			if (attachments == null)
			{
				attachments = new HashSet<string>();
			}
			Stopwatch stopWatch = this.EnablePerformanceStatistics ? Stopwatch.StartNew() : new Stopwatch();
			byte[] array = File.ReadAllBytes(minidumpPath);
			if (array == null || array.Length == 0)
			{
				yield break;
			}
			using (UnityWebRequest request = this._httpClient.Post(this._minidumpUrl, array, attachments, attributes))
			{
				yield return request.SendWebRequest();
				BacktraceResult backtraceResult;
				if (!request.ReceivedNetworkError())
				{
					backtraceResult = BacktraceResult.FromJson(request.downloadHandler.text);
				}
				else
				{
					BacktraceResult backtraceResult2 = new BacktraceResult();
					backtraceResult2.Message = request.error;
					backtraceResult = backtraceResult2;
					backtraceResult2.Status = BacktraceResultStatus.ServerError;
				}
				BacktraceResult backtraceResult3 = backtraceResult;
				if (callback != null)
				{
					callback(backtraceResult3);
				}
				if (this.EnablePerformanceStatistics)
				{
					stopWatch.Stop();
					Debug.Log(string.Format("Backtrace - minidump send time: {0}μs", stopWatch.GetMicroseconds()));
				}
				yield return backtraceResult3;
			}
			UnityWebRequest request = null;
			yield break;
			yield break;
		}

		public IEnumerator Send(BacktraceData data, Action<BacktraceResult> callback = null)
		{
			if (this.RequestHandler != null)
			{
				yield return this.RequestHandler(this.ServerUrl, data);
			}
			else if (data != null)
			{
				string json = data.ToJson();
				yield return this.Send(json, data.Attachments, data.Deduplication, callback);
			}
			yield break;
		}

		public IEnumerator Send(string json, IEnumerable<string> attachments, int deduplication, Action<BacktraceResult> callback)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (deduplication > 0)
			{
				dictionary["_mod_duplicate"] = deduplication.ToString(CultureInfo.InvariantCulture);
			}
			yield return this.Send(json, attachments, dictionary, callback);
			yield break;
		}

		public IEnumerator Send(string json, IEnumerable<string> attachments, Dictionary<string, string> attributes, Action<BacktraceResult> callback)
		{
			Stopwatch stopWatch = this.EnablePerformanceStatistics ? Stopwatch.StartNew() : new Stopwatch();
			using (UnityWebRequest request = this._httpClient.Post(this.ServerUrl, json, attachments, attributes))
			{
				yield return request.SendWebRequest();
				BacktraceResult backtraceResult;
				if (request.responseCode == 429L)
				{
					backtraceResult = new BacktraceResult
					{
						Message = "Server report limit reached",
						Status = BacktraceResultStatus.LimitReached
					};
					if (this.OnServerResponse != null)
					{
						this.OnServerResponse(backtraceResult);
					}
				}
				else if (request.responseCode == 200L && !request.ReceivedNetworkError())
				{
					backtraceResult = BacktraceResult.FromJson(request.downloadHandler.text);
					this._shouldDisplayFailureMessage = true;
					if (this.OnServerResponse != null)
					{
						this.OnServerResponse(backtraceResult);
					}
				}
				else
				{
					this.PrintLog(request);
					Exception ex = new Exception(request.error);
					backtraceResult = BacktraceResult.OnNetworkError(ex);
					if (this.OnServerError != null)
					{
						this.OnServerError(ex);
					}
				}
				if (callback != null)
				{
					callback(backtraceResult);
				}
				if (this.EnablePerformanceStatistics)
				{
					stopWatch.Stop();
				}
				yield return backtraceResult;
			}
			UnityWebRequest request = null;
			yield break;
			yield break;
		}

		private void PrintLog(UnityWebRequest request)
		{
			if (!this._shouldDisplayFailureMessage)
			{
				return;
			}
			this._shouldDisplayFailureMessage = false;
			Debug.LogWarning(string.Format("{0}{1}", string.Format("[Backtrace]::Reponse code: {0}, Response text: {1}", request.responseCode, request.error), "\n Please check provided url to Backtrace service or learn more from our integration guide: https://support.backtrace.io/hc/en-us/articles/360040515991-Unity-Integration-Guide"));
		}

		private BacktraceHttpClient _httpClient = new BacktraceHttpClient();

		private bool _shouldDisplayFailureMessage = true;

		private readonly Uri _serverUrl;

		private readonly string _minidumpUrl;

		private readonly BacktraceCredentials _credentials;
	}
}
