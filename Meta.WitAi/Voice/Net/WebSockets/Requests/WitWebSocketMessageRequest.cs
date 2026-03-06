using System;
using System.Collections.Generic;
using Meta.WitAi;
using Meta.WitAi.Json;

namespace Meta.Voice.Net.WebSockets.Requests
{
	public class WitWebSocketMessageRequest : WitWebSocketJsonRequest
	{
		public string Endpoint { get; }

		public bool EndWithFullTranscription { get; }

		public event Action<WitResponseNode> OnDecodedResponse;

		public WitWebSocketMessageRequest(WitResponseNode externalPostData, string requestId, string clientUserId, string operationId, bool endWithFullTranscription = false) : base(externalPostData, requestId, clientUserId, operationId)
		{
			this.Endpoint = "external";
			this.EndWithFullTranscription = endWithFullTranscription;
			this.SetResponseData(externalPostData);
			base.WaitForTimeout().WrapErrors();
		}

		public WitWebSocketMessageRequest(string endpoint, Dictionary<string, string> parameters, string requestId = null, string clientUserId = null, string operationId = null, bool endWithFullTranscription = false) : base(WitWebSocketMessageRequest.GetPostData(endpoint, parameters), requestId, clientUserId, operationId)
		{
			this.Endpoint = endpoint;
			this.EndWithFullTranscription = endWithFullTranscription;
		}

		public override string ToString()
		{
			return base.ToString() + "\nEndpoint: " + this.Endpoint;
		}

		public static WitResponseClass GetPostData(string endpoint, Dictionary<string, string> parameters)
		{
			WitResponseClass witResponseClass = new WitResponseClass();
			WitResponseClass witResponseClass2 = new WitResponseClass();
			WitResponseClass witResponseClass3 = new WitResponseClass();
			if (parameters != null)
			{
				foreach (string text in parameters.Keys)
				{
					if (!string.Equals(text, "tag"))
					{
						string text2 = parameters[text];
						if (!string.IsNullOrEmpty(text2))
						{
							if (string.Equals(text, "context"))
							{
								witResponseClass[text] = JsonConvert.DeserializeToken(text2);
							}
							else
							{
								if (text2[0].Equals('['))
								{
									string text3 = text2;
									if (text3[text3.Length - 1].Equals(']'))
									{
										text2 = text2.Substring(1, text2.Length - 2);
										WitResponseArray witResponseArray = new WitResponseArray();
										string[] array = text2.Split(',', StringSplitOptions.None);
										for (int i = 0; i < array.Length; i++)
										{
											witResponseArray[i] = new WitResponseData(array[i]);
										}
										witResponseClass3[text] = witResponseArray;
										continue;
									}
								}
								witResponseClass3[text] = new WitResponseData(text2);
							}
						}
					}
				}
			}
			witResponseClass2[endpoint] = witResponseClass3;
			witResponseClass["data"] = witResponseClass2;
			return witResponseClass;
		}

		public override void HandleDownload(string jsonString, WitResponseNode jsonData, byte[] binaryData)
		{
			if (base.IsComplete)
			{
				return;
			}
			if (!base.IsDownloading)
			{
				this.HandleDownloadBegin();
			}
			this.ReturnRawResponse(jsonString);
			this.SetResponseData(jsonData);
			if (!string.IsNullOrEmpty(base.Error))
			{
				this.HandleComplete();
				return;
			}
			if (this.IsEndOfStream(base.ResponseData))
			{
				this.HandleComplete();
			}
		}

		protected virtual bool IsEndOfStream(WitResponseNode responseData)
		{
			return (responseData != null && responseData["is_final"].AsBool) || (this.EndWithFullTranscription && responseData != null && responseData["end_transcription"].AsBool);
		}

		protected override void SetResponseData(WitResponseNode newResponseData)
		{
			base.SetResponseData(newResponseData);
			Action<WitResponseNode> onDecodedResponse = this.OnDecodedResponse;
			if (onDecodedResponse == null)
			{
				return;
			}
			onDecodedResponse(base.ResponseData);
		}
	}
}
