using System;
using System.Runtime.InteropServices;

public class GetProgressionTrackValuesForPlayerRequest : MothershipRequestShared
{
	internal GetProgressionTrackValuesForPlayerRequest(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetProgressionTrackValuesForPlayerRequest obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetProgressionTrackValuesForPlayerRequest obj)
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
					MothershipApiPINVOKE.delete_GetProgressionTrackValuesForPlayerRequest(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public string playerId
	{
		get
		{
			string progressionTrackValuesForPlayerRequest_playerId_get = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_playerId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return progressionTrackValuesForPlayerRequest_playerId_get;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_playerId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string titleId
	{
		get
		{
			string progressionTrackValuesForPlayerRequest_titleId_get = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_titleId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return progressionTrackValuesForPlayerRequest_titleId_get;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_titleId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string envId
	{
		get
		{
			string progressionTrackValuesForPlayerRequest_envId_get = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_envId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return progressionTrackValuesForPlayerRequest_envId_get;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_envId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string deploymentId
	{
		get
		{
			string progressionTrackValuesForPlayerRequest_deploymentId_get = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_deploymentId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return progressionTrackValuesForPlayerRequest_deploymentId_get;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_deploymentId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t ToHttpRequest()
	{
		SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t result = new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t(MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerRequest_ToHttpRequest(this.swigCPtr), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public GetProgressionTrackValuesForPlayerRequest() : this(MothershipApiPINVOKE.new_GetProgressionTrackValuesForPlayerRequest(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
