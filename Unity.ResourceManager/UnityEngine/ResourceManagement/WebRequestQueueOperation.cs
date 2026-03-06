using System;
using UnityEngine.Networking;

namespace UnityEngine.ResourceManagement
{
	public class WebRequestQueueOperation
	{
		public bool IsDone
		{
			get
			{
				return this.m_Completed || this.Result != null;
			}
		}

		public UnityWebRequest WebRequest
		{
			get
			{
				return this.m_WebRequest;
			}
			internal set
			{
				this.m_WebRequest = value;
			}
		}

		public WebRequestQueueOperation(UnityWebRequest request)
		{
			this.m_WebRequest = request;
		}

		internal void Complete(UnityWebRequestAsyncOperation asyncOp)
		{
			this.m_Completed = true;
			this.Result = asyncOp;
			Action<UnityWebRequestAsyncOperation> onComplete = this.OnComplete;
			if (onComplete == null)
			{
				return;
			}
			onComplete(this.Result);
		}

		private bool m_Completed;

		public UnityWebRequestAsyncOperation Result;

		public Action<UnityWebRequestAsyncOperation> OnComplete;

		internal UnityWebRequest m_WebRequest;
	}
}
