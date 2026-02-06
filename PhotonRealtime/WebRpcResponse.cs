using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public class WebRpcResponse
	{
		public string Name { get; private set; }

		public int ResultCode { get; private set; }

		[Obsolete("Use ResultCode instead")]
		public int ReturnCode
		{
			get
			{
				return this.ResultCode;
			}
		}

		public string Message { get; private set; }

		[Obsolete("Use Message instead")]
		public string DebugMessage
		{
			get
			{
				return this.Message;
			}
		}

		public Dictionary<string, object> Parameters { get; private set; }

		public WebRpcResponse(OperationResponse response)
		{
			object obj;
			if (response.Parameters.TryGetValue(209, out obj))
			{
				this.Name = (obj as string);
			}
			this.ResultCode = -1;
			if (response.Parameters.TryGetValue(207, out obj))
			{
				this.ResultCode = (int)((byte)obj);
			}
			if (response.Parameters.TryGetValue(208, out obj))
			{
				this.Parameters = (obj as Dictionary<string, object>);
			}
			if (response.Parameters.TryGetValue(206, out obj))
			{
				this.Message = (obj as string);
			}
		}

		public string ToStringFull()
		{
			return string.Format("{0}={2}: {1} \"{3}\"", new object[]
			{
				this.Name,
				SupportClass.DictionaryToString(this.Parameters, true),
				this.ResultCode,
				this.Message
			});
		}
	}
}
