using System;

namespace Fusion
{
	[Serializable]
	public abstract class FusionMppmCommand
	{
		public abstract void Execute();

		public virtual bool NeedsAck
		{
			get
			{
				return false;
			}
		}

		public virtual string PersistentKey
		{
			get
			{
				return null;
			}
		}
	}
}
