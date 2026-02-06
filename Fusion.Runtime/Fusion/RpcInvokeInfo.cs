using System;

namespace Fusion
{
	[Serializable]
	public struct RpcInvokeInfo
	{
		public override string ToString()
		{
			return string.Format("[Local: {0}, SendCull: {1}, Send: {2}]", this.LocalInvokeResult, this.SendCullResult, this.SendResult);
		}

		public RpcLocalInvokeResult LocalInvokeResult;

		public RpcSendCullResult SendCullResult;

		public RpcSendResult SendResult;
	}
}
