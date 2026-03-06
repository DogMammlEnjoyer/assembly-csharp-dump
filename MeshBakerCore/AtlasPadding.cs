using System;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public struct AtlasPadding
	{
		public AtlasPadding(int p)
		{
			this.topBottom = p;
			this.leftRight = p;
		}

		public AtlasPadding(int px, int py)
		{
			this.topBottom = py;
			this.leftRight = px;
		}

		public int topBottom;

		public int leftRight;
	}
}
