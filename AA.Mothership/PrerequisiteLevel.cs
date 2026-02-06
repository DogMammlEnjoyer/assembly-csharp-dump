using System;
using System.Runtime.InteropServices;

public class PrerequisiteLevel : IDisposable
{
	internal PrerequisiteLevel(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(PrerequisiteLevel obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(PrerequisiteLevel obj)
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

	~PrerequisiteLevel()
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
					MothershipApiPINVOKE.delete_PrerequisiteLevel(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public string track_id
	{
		get
		{
			string result = MothershipApiPINVOKE.PrerequisiteLevel_track_id_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.PrerequisiteLevel_track_id_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string level_id
	{
		get
		{
			string result = MothershipApiPINVOKE.PrerequisiteLevel_level_id_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.PrerequisiteLevel_level_id_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string track_name
	{
		get
		{
			string result = MothershipApiPINVOKE.PrerequisiteLevel_track_name_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.PrerequisiteLevel_track_name_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public string level_name
	{
		get
		{
			string result = MothershipApiPINVOKE.PrerequisiteLevel_level_name_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.PrerequisiteLevel_level_name_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public int required_progression
	{
		get
		{
			int result = MothershipApiPINVOKE.PrerequisiteLevel_required_progression_get(this.swigCPtr);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.PrerequisiteLevel_required_progression_set(this.swigCPtr, value);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public bool ParseFromString(string response)
	{
		bool result = MothershipApiPINVOKE.PrerequisiteLevel_ParseFromString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool ToJson(SWIGTYPE_p_rapidjson__Value prerequisiteLevel, SWIGTYPE_p_rapidjson__Document body)
	{
		bool result = MothershipApiPINVOKE.PrerequisiteLevel_ToJson(this.swigCPtr, SWIGTYPE_p_rapidjson__Value.getCPtr(prerequisiteLevel), SWIGTYPE_p_rapidjson__Document.getCPtr(body));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public PrerequisiteLevel() : this(MothershipApiPINVOKE.new_PrerequisiteLevel(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;
}
