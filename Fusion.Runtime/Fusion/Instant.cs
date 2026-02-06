using System;

namespace Fusion
{
	internal struct Instant
	{
		public double Input { readonly get; internal set; }

		public double Local { readonly get; internal set; }

		public double Remote { readonly get; internal set; }
	}
}
