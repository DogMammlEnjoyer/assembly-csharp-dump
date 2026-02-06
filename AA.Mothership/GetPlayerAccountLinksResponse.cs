using System;
using System.Runtime.InteropServices;

public class GetPlayerAccountLinksResponse : MothershipResponse
{
	internal GetPlayerAccountLinksResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetPlayerAccountLinksResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetPlayerAccountLinksResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetPlayerAccountLinksResponse obj)
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

	protected override void Dispose(bool disposing)
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					MothershipApiPINVOKE.delete_GetPlayerAccountLinksResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool playerAccountLinksResponse_ParseFromResponseString = MothershipApiPINVOKE.GetPlayerAccountLinksResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return playerAccountLinksResponse_ParseFromResponseString;
	}

	public static GetPlayerAccountLinksResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr playerAccountLinksResponse_FromMothershipResponse = MothershipApiPINVOKE.GetPlayerAccountLinksResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetPlayerAccountLinksResponse result = (playerAccountLinksResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetPlayerAccountLinksResponse(playerAccountLinksResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public PlayerIdentityVector Identities
	{
		get
		{
			IntPtr playerAccountLinksResponse_Identities_get = MothershipApiPINVOKE.GetPlayerAccountLinksResponse_Identities_get(this.swigCPtr);
			PlayerIdentityVector result = (playerAccountLinksResponse_Identities_get == IntPtr.Zero) ? null : new PlayerIdentityVector(playerAccountLinksResponse_Identities_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetPlayerAccountLinksResponse_Identities_set(this.swigCPtr, PlayerIdentityVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetPlayerAccountLinksResponse() : this(MothershipApiPINVOKE.new_GetPlayerAccountLinksResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string identities_name = MothershipApiPINVOKE.GetPlayerAccountLinksResponse_identities_name_get();
}
