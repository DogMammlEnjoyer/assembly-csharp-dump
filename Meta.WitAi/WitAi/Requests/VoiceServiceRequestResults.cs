using System;
using System.Collections.Generic;
using Meta.Voice;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.WitAi.Requests
{
	public class VoiceServiceRequestResults : INLPRequestResults<WitResponseNode>, ITranscriptionRequestResults, IVoiceRequestResults
	{
		public int StatusCode { get; private set; } = 200;

		public string Message { get; private set; }

		public string Transcription { get; private set; }

		public string[] FinalTranscriptions { get; private set; }

		public WitResponseNode ResponseData { get; internal set; }

		[Preserve]
		public VoiceServiceRequestResults()
		{
		}

		public void SetCancel(string reason)
		{
			this.StatusCode = -6;
			this.Message = reason;
		}

		public void SetError(int errorStatusCode, string error)
		{
			this.StatusCode = errorStatusCode;
			this.Message = error;
		}

		public void SetTranscription(string transcription, bool full)
		{
			this.Transcription = transcription;
			if (full)
			{
				List<string> list = new List<string>();
				if (this.FinalTranscriptions != null)
				{
					list.AddRange(this.FinalTranscriptions);
				}
				list.Add(this.Transcription);
				this.FinalTranscriptions = list.ToArray();
			}
		}

		public void SetResponseData(WitResponseNode responseData)
		{
			this.ResponseData = responseData;
		}
	}
}
