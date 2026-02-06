using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class WebRpcResponse
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
			bool flag = response.Parameters.TryGetValue(209, out obj);
			if (flag)
			{
				this.Name = (obj as string);
			}
			this.ResultCode = -1;
			bool flag2 = response.Parameters.TryGetValue(207, out obj);
			if (flag2)
			{
				this.ResultCode = (int)((byte)obj);
			}
			bool flag3 = response.Parameters.TryGetValue(208, out obj);
			if (flag3)
			{
				this.Parameters = (obj as Dictionary<string, object>);
			}
			bool flag4 = response.Parameters.TryGetValue(206, out obj);
			if (flag4)
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
