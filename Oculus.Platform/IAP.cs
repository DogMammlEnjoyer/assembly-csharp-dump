using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class IAP
	{
		public static Request ConsumePurchase(string sku)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_IAP_ConsumePurchase(sku));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<ProductList> GetProductsBySKU(string[] skus)
		{
			if (Core.IsInitialized())
			{
				return new Request<ProductList>(CAPI.ovr_IAP_GetProductsBySKU(skus, (skus != null) ? skus.Length : 0));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<PurchaseList> GetViewerPurchases()
		{
			if (Core.IsInitialized())
			{
				return new Request<PurchaseList>(CAPI.ovr_IAP_GetViewerPurchases());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<PurchaseList> GetViewerPurchasesDurableCache()
		{
			if (Core.IsInitialized())
			{
				return new Request<PurchaseList>(CAPI.ovr_IAP_GetViewerPurchasesDurableCache());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<Purchase> LaunchCheckoutFlow(string sku)
		{
			if (!Core.IsInitialized())
			{
				Debug.LogError(Core.PlatformUninitializedError);
				return null;
			}
			if (Application.isEditor)
			{
				throw new NotImplementedException("LaunchCheckoutFlow() is not implemented in the editor yet.");
			}
			return new Request<Purchase>(CAPI.ovr_IAP_LaunchCheckoutFlow(sku));
		}

		public static Request<ProductList> GetNextProductListPage(ProductList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextProductListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<ProductList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 467225263));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<PurchaseList> GetNextPurchaseListPage(PurchaseList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextPurchaseListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<PurchaseList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 1196886677));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
