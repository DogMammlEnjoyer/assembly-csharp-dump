using System;
using System.Runtime.InteropServices;

public class HydratedProgressionTreeResponse : MothershipResponse
{
	internal HydratedProgressionTreeResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.HydratedProgressionTreeResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(HydratedProgressionTreeResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(HydratedProgressionTreeResponse obj)
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
					MothershipApiPINVOKE.delete_HydratedProgressionTreeResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public static HydratedProgressionTreeResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTreeResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		HydratedProgressionTreeResponse result = (intPtr == IntPtr.Zero) ? null : new HydratedProgressionTreeResponse(intPtr, false);
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
			IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTreeResponse_Tree_get(this.swigCPtr);
			ProgressionTree result = (intPtr == IntPtr.Zero) ? null : new ProgressionTree(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.HydratedProgressionTreeResponse_Tree_set(this.swigCPtr, ProgressionTree.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public TreeNodeVector Nodes
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTreeResponse_Nodes_get(this.swigCPtr);
			TreeNodeVector result = (intPtr == IntPtr.Zero) ? null : new TreeNodeVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.HydratedProgressionTreeResponse_Nodes_set(this.swigCPtr, TreeNodeVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public ProgressionTrack Track
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.HydratedProgressionTreeResponse_Track_get(this.swigCPtr);
			ProgressionTrack result = (intPtr == IntPtr.Zero) ? null : new ProgressionTrack(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.HydratedProgressionTreeResponse_Track_set(this.swigCPtr, ProgressionTrack.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.HydratedProgressionTreeResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public HydratedProgressionTreeResponse() : this(MothershipApiPINVOKE.new_HydratedProgressionTreeResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
