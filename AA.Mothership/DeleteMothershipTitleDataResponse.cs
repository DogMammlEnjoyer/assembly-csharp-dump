using System;
using System.Runtime.InteropServices;

public class DeleteMothershipTitleDataResponse : MothershipResponse
{
	internal DeleteMothershipTitleDataResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.DeleteMothershipTitleDataResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(DeleteMothershipTitleDataResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(DeleteMothershipTitleDataResponse obj)
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
					MothershipApiPINVOKE.delete_DeleteMothershipTitleDataResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public MothershipTitleData TitleData
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.DeleteMothershipTitleDataResponse_TitleData_get(this.swigCPtr);
			MothershipTitleData result = (intPtr == IntPtr.Zero) ? null : new MothershipTitleData(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.DeleteMothershipTitleDataResponse_TitleData_set(this.swigCPtr, MothershipTitleData.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.DeleteMothershipTitleDataResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static DeleteMothershipTitleDataResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.DeleteMothershipTitleDataResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		DeleteMothershipTitleDataResponse result = (intPtr == IntPtr.Zero) ? null : new DeleteMothershipTitleDataResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public DeleteMothershipTitleDataResponse() : this(MothershipApiPINVOKE.new_DeleteMothershipTitleDataResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
