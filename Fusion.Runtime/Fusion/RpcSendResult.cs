using System;
using System.Text;

namespace Fusion
{
	[Serializable]
	public struct RpcSendResult
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			stringBuilder.Append(this.Result.ToString());
			stringBuilder.Append(", Size: ");
			stringBuilder.Append(this.MessageSize);
			stringBuilder.Append("}");
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public RpcSendMessageResult Result;

		public int MessageSize;
	}
}
