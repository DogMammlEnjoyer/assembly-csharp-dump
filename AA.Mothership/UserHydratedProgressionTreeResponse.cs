using System;
using System.Runtime.InteropServices;

public class UserHydratedProgressionTreeResponse : MothershipResponse
{
	internal UserHydratedProgressionTreeResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserHydratedProgressionTreeResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserHydratedProgressionTreeResponse obj)
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
					MothershipApiPINVOKE.delete_UserHydratedProgressionTreeResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public static UserHydratedProgressionTreeResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UserHydratedProgressionTreeResponse result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTreeResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ProgressionTree Tree
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_Tree_get(this.swigCPtr);
			ProgressionTree result = (intPtr == IntPtr.Zero) ? null : new ProgressionTree(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_Tree_set(this.swigCPtr, ProgressionTree.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UserHydratedNodeVector Nodes
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_Nodes_get(this.swigCPtr);
			UserHydratedNodeVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedNodeVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_Nodes_set(this.swigCPtr, UserHydratedNodeVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UserHydratedProgressionTrackResponse Track
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_Track_get(this.swigCPtr);
			UserHydratedProgressionTrackResponse result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTrackResponse(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_Track_set(this.swigCPtr, UserHydratedProgressionTrackResponse.getCPtr(value));
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
			string result = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_PlayerId_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_PlayerId_set(this.swigCPtr, value);
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
			bool result = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_InventoryRefreshRequired_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_InventoryRefreshRequired_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.UserHydratedProgressionTreeResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public UserHydratedProgressionTreeResponse() : this(MothershipApiPINVOKE.new_UserHydratedProgressionTreeResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
