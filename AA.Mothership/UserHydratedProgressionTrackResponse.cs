using System;
using System.Runtime.InteropServices;

public class UserHydratedProgressionTrackResponse : MothershipResponse
{
	internal UserHydratedProgressionTrackResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserHydratedProgressionTrackResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserHydratedProgressionTrackResponse obj)
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
					MothershipApiPINVOKE.delete_UserHydratedProgressionTrackResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public ProgressionTrack Track
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Track_get(this.swigCPtr);
			ProgressionTrack result = (intPtr == IntPtr.Zero) ? null : new ProgressionTrack(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Track_set(this.swigCPtr, ProgressionTrack.getCPtr(value));
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
			IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Triggers_get(this.swigCPtr);
			TrackTriggerVector result = (intPtr == IntPtr.Zero) ? null : new TrackTriggerVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Triggers_set(this.swigCPtr, TrackTriggerVector.getCPtr(value));
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
			IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Levels_get(this.swigCPtr);
			TrackLevelVector result = (intPtr == IntPtr.Zero) ? null : new TrackLevelVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Levels_set(this.swigCPtr, TrackLevelVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public int Progress
	{
		get
		{
			int result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Progress_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_Progress_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string CurrentLevelName
	{
		get
		{
			string result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_CurrentLevelName_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_CurrentLevelName_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string CurrentLevelId
	{
		get
		{
			string result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_CurrentLevelId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_CurrentLevelId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string PlayerId
	{
		get
		{
			string result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_PlayerId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_PlayerId_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string LastUpdated
	{
		get
		{
			string result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_LastUpdated_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_LastUpdated_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public bool InventoryRefreshRequired
	{
		get
		{
			bool result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_InventoryRefreshRequired_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_InventoryRefreshRequired_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public static UserHydratedProgressionTrackResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UserHydratedProgressionTrackResponse result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTrackResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public override bool ParseFromResponseString(string string_)
	{
		bool result = MothershipApiPINVOKE.UserHydratedProgressionTrackResponse_ParseFromResponseString(this.swigCPtr, string_);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public UserHydratedProgressionTrackResponse() : this(MothershipApiPINVOKE.new_UserHydratedProgressionTrackResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
