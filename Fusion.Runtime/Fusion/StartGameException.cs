using System;

namespace Fusion
{
	internal class StartGameException : Exception
	{
		public ShutdownReason ShutdownReason { get; internal set; }

		internal StartGameException(ShutdownReason shutdownReason = ShutdownReason.Error, string customMsg = null) : base(customMsg ?? shutdownReason.ToString())
		{
			this.ShutdownReason = shutdownReason;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}: {2}, {3}: {4}]", new object[]
			{
				"StartGameException",
				"ShutdownReason",
				this.ShutdownReason,
				"Message",
				this.Message
			});
		}
	}
}
