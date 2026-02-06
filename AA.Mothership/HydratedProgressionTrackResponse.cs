using System;
using System.Runtime.InteropServices;

public class HydratedProgressionTrackResponse : MothershipResponse
{
	internal HydratedProgressionTrackResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.HydratedProgressionTrackResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(HydratedProgressionTrackResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(HydratedProgressionTrackResponse obj)
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
					MothershipApiPINVOKE.delete_HydratedProgressionTrackResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.HydratedProgressionTrackResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static HydratedProgressionTrackResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTrackResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		HydratedProgressionTrackResponse result = (intPtr == IntPtr.Zero) ? null : new HydratedProgressionTrackResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ProgressionTrack Track
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTrackResponse_Track_get(this.swigCPtr);
			ProgressionTrack result = (intPtr == IntPtr.Zero) ? null : new ProgressionTrack(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.HydratedProgressionTrackResponse_Track_set(this.swigCPtr, ProgressionTrack.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public TrackTriggerVector Triggers
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTrackResponse_Triggers_get(this.swigCPtr);
			TrackTriggerVector result = (intPtr == IntPtr.Zero) ? null : new TrackTriggerVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.HydratedProgressionTrackResponse_Triggers_set(this.swigCPtr, TrackTriggerVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public TrackLevelVector Levels
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTrackResponse_Levels_get(this.swigCPtr);
			TrackLevelVector result = (intPtr == IntPtr.Zero) ? null : new TrackLevelVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.HydratedProgressionTrackResponse_Levels_set(this.swigCPtr, TrackLevelVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public HydratedProgressionTrackResponse() : this(MothershipApiPINVOKE.new_HydratedProgressionTrackResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
