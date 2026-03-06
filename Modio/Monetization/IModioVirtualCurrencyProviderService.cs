using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Modio.Monetization
{
	public interface IModioVirtualCurrencyProviderService
	{
		[return: TupleElementNames(new string[]
		{
			"error",
			"skus"
		})]
		Task<ValueTuple<Error, PortalSku[]>> GetCurrencyPackSkus();

		Task<Error> OpenCheckoutFlow(PortalSku sku);
	}
}
