using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cysharp.Threading.Tasks
{
	public class UnityWebRequestException : Exception
	{
		public UnityWebRequest UnityWebRequest { get; }

		public UnityWebRequest.Result Result { get; }

		public string Error { get; }

		public string Text { get; }

		public long ResponseCode { get; }

		public Dictionary<string, string> ResponseHeaders { get; }

		public UnityWebRequestException(UnityWebRequest unityWebRequest)
		{
			this.UnityWebRequest = unityWebRequest;
			this.Result = unityWebRequest.result;
			this.Error = unityWebRequest.error;
			this.ResponseCode = unityWebRequest.responseCode;
			if (this.UnityWebRequest.downloadHandler != null)
			{
				DownloadHandlerBuffer downloadHandlerBuffer = unityWebRequest.downloadHandler as DownloadHandlerBuffer;
				if (downloadHandlerBuffer != null)
				{
					this.Text = downloadHandlerBuffer.text;
				}
			}
			this.ResponseHeaders = unityWebRequest.GetResponseHeaders();
		}

		public override string Message
		{
			get
			{
				if (this.msg == null)
				{
					if (!string.IsNullOrWhiteSpace(this.Text))
					{
						this.msg = this.Error + Environment.NewLine + this.Text;
					}
					else
					{
						this.msg = this.Error;
					}
				}
				return this.msg;
			}
		}

		private string msg;
	}
}
