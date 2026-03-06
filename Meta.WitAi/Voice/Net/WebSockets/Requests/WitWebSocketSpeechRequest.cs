using System;
using System.Collections.Generic;
using Meta.WitAi.Json;

namespace Meta.Voice.Net.WebSockets.Requests
{
	public class WitWebSocketSpeechRequest : WitWebSocketMessageRequest
	{
		public bool IsReadyForInput { get; private set; }

		public bool HasSentAudio { get; private set; }

		public event Action OnReadyForInput;

		public WitWebSocketSpeechRequest(string endpoint, Dictionary<string, string> parameters, string requestId = null, string clientUserId = null, string operationId = null, bool endWithFullTranscription = false) : base(endpoint, parameters, requestId, clientUserId, operationId, endWithFullTranscription)
		{
		}

		public override void HandleDownload(string jsonString, WitResponseNode jsonData, byte[] binaryData)
		{
			bool flag = false;
			if (!base.IsComplete && !this.IsReadyForInput)
			{
				string value = jsonData["type"].Value;
				this.IsReadyForInput = string.Equals(value, "INITIALIZED");
				flag = this.IsReadyForInput;
			}
			base.HandleDownload(jsonString, jsonData, binaryData);
			if (flag)
			{
				Action onReadyForInput = this.OnReadyForInput;
				if (onReadyForInput == null)
				{
					return;
				}
				onReadyForInput();
			}
		}

		public void SendAudioData(byte[] buffer, int offset, int length)
		{
			if (!base.IsUploading || !this.IsReadyForInput)
			{
				return;
			}
			byte[] array = buffer;
			if (offset != 0 || length != buffer.Length)
			{
				array = new byte[length];
				Array.Copy(buffer, offset, array, 0, length);
			}
			base.UploadChunk(this.GetAdditionalPostJson(), array);
			if (!this.HasSentAudio && length > 0)
			{
				this.HasSentAudio = true;
			}
		}

		public virtual void CloseAudioStream()
		{
			if (!base.IsUploading || !this.IsReadyForInput)
			{
				return;
			}
			this.IsReadyForInput = false;
			WitResponseClass asObject = this.GetAdditionalPostJson().AsObject;
			WitResponseClass witResponseClass = new WitResponseClass();
			witResponseClass["end_stream"] = new WitResponseClass();
			asObject["data"] = witResponseClass;
			base.UploadChunk(asObject, null);
		}

		private WitResponseNode GetAdditionalPostJson()
		{
			return new WitResponseClass();
		}
	}
}
