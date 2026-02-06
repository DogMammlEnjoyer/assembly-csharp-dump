using System;
using System.Runtime.InteropServices;

public class MothershipApiClient : IDisposable
{
	internal MothershipApiClient(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipApiClient obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipApiClient obj)
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

	~MothershipApiClient()
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
					MothershipApiPINVOKE.delete_MothershipApiClient(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public virtual void SetHttpRequestDelegate(MothershipSendHTTPRequestDelegateWrapper inSendRequestDelegate)
	{
		MothershipApiPINVOKE.MothershipApiClient_SetHttpRequestDelegate(this.swigCPtr, MothershipSendHTTPRequestDelegateWrapper.getCPtr(inSendRequestDelegate));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void SetWebSocketDelegate(MothershipWebSocketDelegateWrapper inWebsocketDelegate)
	{
		MothershipApiPINVOKE.MothershipApiClient_SetWebSocketDelegate(this.swigCPtr, MothershipWebSocketDelegateWrapper.getCPtr(inWebsocketDelegate));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void ReceiveHttpResponse(MothershipHTTPResponse response)
	{
		MothershipApiPINVOKE.MothershipApiClient_ReceiveHttpResponse(this.swigCPtr, MothershipHTTPResponse.getCPtr(response));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void ReceiveWebsocketMessage(MothershipWebSocketResponse response)
	{
		MothershipApiPINVOKE.MothershipApiClient_ReceiveWebsocketMessage(this.swigCPtr, MothershipWebSocketResponse.getCPtr(response));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void TickRetryQueue(float deltaTimeInSeconds)
	{
		MothershipApiPINVOKE.MothershipApiClient_TickRetryQueue(this.swigCPtr, deltaTimeInSeconds);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void SetLogDelegate(MothershipLogDelegateWrapper logDelegate)
	{
		MothershipApiPINVOKE.MothershipApiClient_SetLogDelegate(this.swigCPtr, MothershipLogDelegateWrapper.getCPtr(logDelegate));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void Log(MothershipLogLevel level, string message)
	{
		MothershipApiPINVOKE.MothershipApiClient_Log(this.swigCPtr, (int)level, message);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public class MothershipInflightRequest : IDisposable
	{
		internal MothershipInflightRequest(IntPtr cPtr, bool cMemoryOwn)
		{
			this.swigCMemOwn = cMemoryOwn;
			this.swigCPtr = new HandleRef(this, cPtr);
		}

		internal static HandleRef getCPtr(MothershipApiClient.MothershipInflightRequest obj)
		{
			if (obj != null)
			{
				return obj.swigCPtr;
			}
			return new HandleRef(null, IntPtr.Zero);
		}

		internal static HandleRef swigRelease(MothershipApiClient.MothershipInflightRequest obj)
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

		~MothershipInflightRequest()
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
						MothershipApiPINVOKE.delete_MothershipApiClient_MothershipInflightRequest(this.swigCPtr);
					}
					this.swigCPtr = new HandleRef(null, IntPtr.Zero);
				}
			}
		}

		public SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequest_t InternalRequest
		{
			get
			{
				IntPtr intPtr = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_InternalRequest_get(this.swigCPtr);
				SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequest_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequest_t(intPtr, false);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_InternalRequest_set(this.swigCPtr, SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequest_t.getCPtr(value));
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t HttpRequest
		{
			get
			{
				IntPtr intPtr = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_HttpRequest_get(this.swigCPtr);
				SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t(intPtr, false);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_HttpRequest_set(this.swigCPtr, SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipHTTPRequest_t.getCPtr(value));
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequestCompleteDelegateWrapper_t CallbackWrapper
		{
			get
			{
				IntPtr intPtr = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_CallbackWrapper_get(this.swigCPtr);
				SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequestCompleteDelegateWrapper_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequestCompleteDelegateWrapper_t(intPtr, false);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_CallbackWrapper_set(this.swigCPtr, SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipRequestCompleteDelegateWrapper_t.getCPtr(value));
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t ResponseInstance
		{
			get
			{
				IntPtr intPtr = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_ResponseInstance_get(this.swigCPtr);
				SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t(intPtr, false);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_ResponseInstance_set(this.swigCPtr, SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t.getCPtr(value));
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public int retryCount
		{
			get
			{
				int result = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_retryCount_get(this.swigCPtr);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_retryCount_set(this.swigCPtr, value);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public float retryTime
		{
			get
			{
				float result = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_retryTime_get(this.swigCPtr);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_retryTime_set(this.swigCPtr, value);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public string playerId
		{
			get
			{
				string result = MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_playerId_get(this.swigCPtr);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipInflightRequest_playerId_set(this.swigCPtr, value);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public MothershipInflightRequest() : this(MothershipApiPINVOKE.new_MothershipApiClient_MothershipInflightRequest(), true)
		{
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		private HandleRef swigCPtr;

		protected bool swigCMemOwn;
	}

	public class MothershipActiveWebSocketInfo : IDisposable
	{
		internal MothershipActiveWebSocketInfo(IntPtr cPtr, bool cMemoryOwn)
		{
			this.swigCMemOwn = cMemoryOwn;
			this.swigCPtr = new HandleRef(this, cPtr);
		}

		internal static HandleRef getCPtr(MothershipApiClient.MothershipActiveWebSocketInfo obj)
		{
			if (obj != null)
			{
				return obj.swigCPtr;
			}
			return new HandleRef(null, IntPtr.Zero);
		}

		internal static HandleRef swigRelease(MothershipApiClient.MothershipActiveWebSocketInfo obj)
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

		~MothershipActiveWebSocketInfo()
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
						MothershipApiPINVOKE.delete_MothershipApiClient_MothershipActiveWebSocketInfo(this.swigCPtr);
					}
					this.swigCPtr = new HandleRef(null, IntPtr.Zero);
				}
			}
		}

		public MothershipApiClient.WebSocketStatus Status
		{
			get
			{
				MothershipApiClient.WebSocketStatus result = (MothershipApiClient.WebSocketStatus)MothershipApiPINVOKE.MothershipApiClient_MothershipActiveWebSocketInfo_Status_get(this.swigCPtr);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipActiveWebSocketInfo_Status_set(this.swigCPtr, (int)value);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipOpenWebSocketEventArgs_t InitialRequest
		{
			get
			{
				IntPtr intPtr = MothershipApiPINVOKE.MothershipApiClient_MothershipActiveWebSocketInfo_InitialRequest_get(this.swigCPtr);
				SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipOpenWebSocketEventArgs_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipOpenWebSocketEventArgs_t(intPtr, false);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipActiveWebSocketInfo_InitialRequest_set(this.swigCPtr, SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipOpenWebSocketEventArgs_t.getCPtr(value));
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipWebSocketMessageDelegateWrapper_t CallbackWrapper
		{
			get
			{
				IntPtr intPtr = MothershipApiPINVOKE.MothershipApiClient_MothershipActiveWebSocketInfo_CallbackWrapper_get(this.swigCPtr);
				SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipWebSocketMessageDelegateWrapper_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipWebSocketMessageDelegateWrapper_t(intPtr, false);
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
				return result;
			}
			set
			{
				MothershipApiPINVOKE.MothershipApiClient_MothershipActiveWebSocketInfo_CallbackWrapper_set(this.swigCPtr, SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipWebSocketMessageDelegateWrapper_t.getCPtr(value));
				if (MothershipApiPINVOKE.SWIGPendingException.Pending)
				{
					throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
				}
			}
		}

		public MothershipActiveWebSocketInfo() : this(MothershipApiPINVOKE.new_MothershipApiClient_MothershipActiveWebSocketInfo(), true)
		{
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}

		private HandleRef swigCPtr;

		protected bool swigCMemOwn;
	}

	public enum WebSocketStatus
	{
		INACTIVE,
		ACTIVE,
		CLOSING
	}
}
