using System;
using System.Collections.Generic;
using System.Text;

namespace PlayFab
{
	public class PlayFabError
	{
		public override string ToString()
		{
			return this.GenerateErrorReport();
		}

		public string GenerateErrorReport()
		{
			if (PlayFabError._tempSb == null)
			{
				PlayFabError._tempSb = new StringBuilder();
			}
			PlayFabError._tempSb.Length = 0;
			if (string.IsNullOrEmpty(this.ErrorMessage))
			{
				PlayFabError._tempSb.Append(this.ApiEndpoint).Append(": ").Append("Http Code: ").Append(this.HttpCode.ToString()).Append("\nHttp Status: ").Append(this.HttpStatus).Append("\nError: ").Append(this.Error.ToString()).Append("\n");
			}
			else
			{
				PlayFabError._tempSb.Append(this.ApiEndpoint).Append(": ").Append(this.ErrorMessage);
			}
			if (this.ErrorDetails != null)
			{
				foreach (KeyValuePair<string, List<string>> keyValuePair in this.ErrorDetails)
				{
					foreach (string value in keyValuePair.Value)
					{
						PlayFabError._tempSb.Append("\n").Append(keyValuePair.Key).Append(": ").Append(value);
					}
				}
			}
			return PlayFabError._tempSb.ToString();
		}

		public string ApiEndpoint;

		public int HttpCode;

		public string HttpStatus;

		public PlayFabErrorCode Error;

		public string ErrorMessage;

		public Dictionary<string, List<string>> ErrorDetails;

		public object CustomData;

		[ThreadStatic]
		private static StringBuilder _tempSb;
	}
}
