using System;
using System.Runtime.InteropServices;

public class RemoveSharedGroupMembersRequest : MothershipRequestShared
{
	internal RemoveSharedGroupMembersRequest(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.RemoveSharedGroupMembersRequest_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(RemoveSharedGroupMembersRequest obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(RemoveSharedGroupMembersRequest obj)
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
					MothershipApiPINVOKE.delete_RemoveSharedGroupMembersRequest(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public string sharedGroupId
	{
		get
		{
			string result = MothershipApiPINVOKE.RemoveSharedGroupMembersRequest_sharedGroupId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.RemoveSharedGroupMembersRequest_sharedGroupId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public StringVector mothershipIds
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.RemoveSharedGroupMembersRequest_mothershipIds_get(this.swigCPtr);
			StringVector result = (intPtr == IntPtr.Zero) ? null : new StringVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.RemoveSharedGroupMembersRequest_mothershipIds_set(this.swigCPtr, StringVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t ToHttpRequest()
	{
		SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t result = new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t(MothershipApiPINVOKE.RemoveSharedGroupMembersRequest_ToHttpRequest(this.swigCPtr), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public RemoveSharedGroupMembersRequest() : this(MothershipApiPINVOKE.new_RemoveSharedGroupMembersRequest(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
