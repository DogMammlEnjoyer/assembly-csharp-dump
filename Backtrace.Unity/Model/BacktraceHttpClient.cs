using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Backtrace.Unity.Extensions;
using Backtrace.Unity.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Backtrace.Unity.Model
{
	internal sealed class BacktraceHttpClient : IBacktraceHttpClient
	{
		public bool IgnoreSslValidation { get; set; }

		public void Post(string submissionUrl, BacktraceJObject jObject, Action<long, bool, string> onComplete)
		{
			UnityWebRequest request = new UnityWebRequest(submissionUrl, "POST")
			{
				timeout = 15000
			};
			request.IgnoreSsl(this.IgnoreSslValidation);
			byte[] bytes = Encoding.UTF8.GetBytes(jObject.ToJson());
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetJsonContentType();
			request.SendWebRequest().completed += delegate(AsyncOperation operation)
			{
				long responseCode = request.responseCode;
				string text = request.downloadHandler.text;
				bool arg = request.ReceivedNetworkError();
				request.Dispose();
				if (onComplete != null)
				{
					onComplete(responseCode, arg, text);
				}
			};
		}

		public UnityWebRequest Post(string submissionUrl, string json, IEnumerable<string> attachments, IDictionary<string, string> attributes)
		{
			return this.Post(submissionUrl, this.CreateJsonFormData(Encoding.UTF8.GetBytes(json), attachments, attributes));
		}

		public UnityWebRequest Post(string submissionUrl, byte[] minidump, IEnumerable<string> attachments, IDictionary<string, string> attributes)
		{
			return this.Post(submissionUrl, this.CreateMinidumpFormData(minidump, attachments, attributes));
		}

		private UnityWebRequest Post(string submissionUrl, List<IMultipartFormSection> formData)
		{
			byte[] array = UnityWebRequest.GenerateBoundary();
			UnityWebRequest unityWebRequest = UnityWebRequest.Post(submissionUrl, formData, array);
			unityWebRequest.timeout = 15000;
			unityWebRequest.IgnoreSsl(this.IgnoreSslValidation);
			unityWebRequest.SetMultipartFormData(array);
			return unityWebRequest;
		}

		private List<IMultipartFormSection> CreateJsonFormData(byte[] json, IEnumerable<string> attachments, IDictionary<string, string> attributes)
		{
			List<IMultipartFormSection> list = new List<IMultipartFormSection>
			{
				new MultipartFormFileSection("upload_file", json, string.Format("{0}.json", "upload_file"), "application/json")
			};
			this.AddAttributesToFormData(list, attributes);
			this.AddAttachmentToFormData(list, attachments);
			return list;
		}

		private List<IMultipartFormSection> CreateMinidumpFormData(byte[] minidump, IEnumerable<string> attachments, IDictionary<string, string> attributes)
		{
			List<IMultipartFormSection> list = new List<IMultipartFormSection>
			{
				new MultipartFormFileSection("upload_file", minidump)
			};
			this.AddAttributesToFormData(list, attributes);
			this.AddAttachmentToFormData(list, attachments);
			return list;
		}

		private void AddAttributesToFormData(List<IMultipartFormSection> formData, IDictionary<string, string> attributes)
		{
			if (attributes == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in attributes)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					formData.Add(new MultipartFormDataSection(keyValuePair.Key, keyValuePair.Value));
				}
			}
		}

		private void AddAttachmentToFormData(List<IMultipartFormSection> formData, IEnumerable<string> attachments)
		{
			if (attachments == null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>(attachments.Reverse<string>());
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (string text in hashSet)
			{
				if (!string.IsNullOrEmpty(text) && File.Exists(text))
				{
					long length = new FileInfo(text).Length;
					if (length <= 10485760L && length != 0L)
					{
						string text2 = Path.GetFileName(text);
						if (dictionary.ContainsKey(text2))
						{
							Dictionary<string, int> dictionary2 = dictionary;
							string key = text2;
							int num = dictionary2[key];
							dictionary2[key] = num + 1;
							text2 = string.Format("{0}({1}){2}", Path.GetFileName(text2), dictionary[text2], Path.GetExtension(text2));
						}
						else
						{
							dictionary[text2] = 0;
						}
						formData.Add(new MultipartFormFileSection(string.Format("{0}{1}", "attachment_", text2), File.ReadAllBytes(text)));
					}
				}
			}
		}

		private const string DiagnosticFileName = "upload_file";

		private const int RequestTimeout = 15000;
	}
}
