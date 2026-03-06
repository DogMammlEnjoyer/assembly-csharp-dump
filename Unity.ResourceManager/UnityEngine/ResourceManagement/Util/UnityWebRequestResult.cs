using System;
using System.Text;
using UnityEngine.Networking;

namespace UnityEngine.ResourceManagement.Util
{
	public class UnityWebRequestResult
	{
		public UnityWebRequestResult(UnityWebRequest request)
		{
			string text = request.error;
			if (request.result == UnityWebRequest.Result.DataProcessingError && request.downloadHandler != null)
			{
				text = text + " : " + request.downloadHandler.error;
			}
			this.Result = request.result;
			this.Error = text;
			this.ResponseCode = request.responseCode;
			this.Method = request.method;
			this.Url = request.url;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("{0} : {1}", this.Result, this.Error));
			if (this.ResponseCode > 0L)
			{
				stringBuilder.AppendLine(string.Format("ResponseCode : {0}, Method : {1}", this.ResponseCode, this.Method));
			}
			stringBuilder.AppendLine("url : " + this.Url);
			return stringBuilder.ToString();
		}

		public string Error { get; set; }

		public long ResponseCode { get; }

		public UnityWebRequest.Result Result { get; }

		public string Method { get; }

		public string Url { get; }

		public bool ShouldRetryDownloadError()
		{
			return string.IsNullOrEmpty(this.Error) || (!(this.Error == "Request aborted") && !(this.Error == "Unable to write data") && !(this.Error == "Malformed URL") && !(this.Error == "Out of memory") && !(this.Error == "Encountered invalid redirect (missing Location header?)") && !(this.Error == "Cannot modify request at this time") && !(this.Error == "Unsupported Protocol") && !(this.Error == "Destination host has an erroneous SSL certificate") && !(this.Error == "Unable to load SSL Cipher for verification") && !(this.Error == "SSL CA certificate error") && !(this.Error == "Unrecognized content-encoding") && !(this.Error == "Request already transmitted") && !(this.Error == "Invalid HTTP Method") && !(this.Error == "Header name contains invalid characters") && !(this.Error == "Header value contains invalid characters") && !(this.Error == "Cannot override system-specified headers") && !(this.Error == "Insecure connection not allowed"));
		}
	}
}
