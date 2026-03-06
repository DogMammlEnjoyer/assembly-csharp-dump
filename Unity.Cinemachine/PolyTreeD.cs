using System;

namespace Unity.Cinemachine
{
	internal class PolyTreeD : PolyPathD
	{
		public new double Scale
		{
			get
			{
				return base.Scale;
			}
		}

		public PolyTreeD() : base(null)
		{
		}
	}
}
