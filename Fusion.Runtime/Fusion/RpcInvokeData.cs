using System;

namespace Fusion
{
	public struct RpcInvokeData
	{
		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]", new object[]
			{
				this.Key,
				this.Sources,
				this.Targets,
				this.Delegate
			});
		}

		public int Key;

		public int Sources;

		public int Targets;

		public RpcInvokeDelegate Delegate;
	}
}
