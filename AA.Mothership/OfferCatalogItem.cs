using System;
using System.Runtime.InteropServices;

public class OfferCatalogItem : IDisposable
{
	internal OfferCatalogItem(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(OfferCatalogItem obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(OfferCatalogItem obj)
	{
		if (obj == null)
		{
			return new HandleRef(null, IntPtr.Zero);
		}
		if (!obj.swigCMemOwn)
		{
			throw new ApplicationException("Cannot release ownership as memory is not owned");
		}
		HandleRef result = obj.swigCPtr;
		obj.swigCMemOwn = false;
		obj.Dispose();
		return result;
	}

	~OfferCatalogItem()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					MothershipApiPINVOKE.delete_OfferCatalogItem(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public string offer_id
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_offer_id_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_offer_id_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string title_id
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_title_id_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_title_id_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string env_id
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_env_id_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_env_id_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string name
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_name_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_name_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string transaction_id
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_transaction_id_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_transaction_id_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public OfferEntitlementMap bundle_pricing
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.OfferCatalogItem_bundle_pricing_get(this.swigCPtr);
			OfferEntitlementMap result = (intPtr == IntPtr.Zero) ? null : new OfferEntitlementMap(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_bundle_pricing_set(this.swigCPtr, OfferEntitlementMap.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public int discount_percent
	{
		get
		{
			int result = MothershipApiPINVOKE.OfferCatalogItem_discount_percent_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_discount_percent_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string created_time
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_created_time_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_created_time_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string last_updated_time
	{
		get
		{
			string result = MothershipApiPINVOKE.OfferCatalogItem_last_updated_time_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_last_updated_time_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public bool sunset
	{
		get
		{
			bool result = MothershipApiPINVOKE.OfferCatalogItem_sunset_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.OfferCatalogItem_sunset_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public bool ParseFromJson(SWIGTYPE_p_rapidjson__GenericObjectT_false_rapidjson__Value_t object_)
	{
		bool result = MothershipApiPINVOKE.OfferCatalogItem_ParseFromJson(this.swigCPtr, SWIGTYPE_p_rapidjson__GenericObjectT_false_rapidjson__Value_t.getCPtr(object_));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public OfferCatalogItem() : this(MothershipApiPINVOKE.new_OfferCatalogItem(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public static readonly string offer_id_name = MothershipApiPINVOKE.OfferCatalogItem_offer_id_name_get();

	public static readonly string title_id_name = MothershipApiPINVOKE.OfferCatalogItem_title_id_name_get();

	public static readonly string env_id_name = MothershipApiPINVOKE.OfferCatalogItem_env_id_name_get();

	public static readonly string name_name = MothershipApiPINVOKE.OfferCatalogItem_name_name_get();

	public static readonly string transaction_id_name = MothershipApiPINVOKE.OfferCatalogItem_transaction_id_name_get();

	public static readonly string bundle_pricing_name = MothershipApiPINVOKE.OfferCatalogItem_bundle_pricing_name_get();

	public static readonly string discount_percent_name = MothershipApiPINVOKE.OfferCatalogItem_discount_percent_name_get();

	public static readonly string created_time_name = MothershipApiPINVOKE.OfferCatalogItem_created_time_name_get();

	public static readonly string last_updated_time_name = MothershipApiPINVOKE.OfferCatalogItem_last_updated_time_name_get();

	public static readonly string sunset_name = MothershipApiPINVOKE.OfferCatalogItem_sunset_name_get();
}
