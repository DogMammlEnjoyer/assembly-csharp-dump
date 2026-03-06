using System;
using Modio.API;

namespace Modio.Monetization
{
	public struct PortalSku
	{
		public PortalSku(ModioAPI.Portal portal, string sku, string name, string formattedPrice, int value)
		{
			this.Portal = portal;
			this.Sku = sku;
			this.Name = name;
			this.FormattedPrice = formattedPrice;
			this.Value = value;
		}

		public readonly ModioAPI.Portal Portal;

		public readonly string Sku;

		public readonly string Name;

		public readonly string FormattedPrice;

		public readonly int Value;
	}
}
