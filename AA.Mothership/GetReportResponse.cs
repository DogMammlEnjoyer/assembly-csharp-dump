using System;
using System.Runtime.InteropServices;

public class GetReportResponse : MothershipResponse
{
	internal GetReportResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetReportResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetReportResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetReportResponse obj)
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
					MothershipApiPINVOKE.delete_GetReportResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public MothershipReportData Report
	{
		get
		{
			IntPtr reportResponse_Report_get = MothershipApiPINVOKE.GetReportResponse_Report_get(this.swigCPtr);
			MothershipReportData result = (reportResponse_Report_get == IntPtr.Zero) ? null : new MothershipReportData(reportResponse_Report_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetReportResponse_Report_set(this.swigCPtr, MothershipReportData.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool reportResponse_ParseFromResponseString = MothershipApiPINVOKE.GetReportResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return reportResponse_ParseFromResponseString;
	}

	public static GetReportResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr reportResponse_FromMothershipResponse = MothershipApiPINVOKE.GetReportResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetReportResponse result = (reportResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetReportResponse(reportResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public GetReportResponse() : this(MothershipApiPINVOKE.new_GetReportResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
