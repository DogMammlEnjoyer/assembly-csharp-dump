using System;
using UnityEngine.Networking;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	internal class UnityWebRequestOperation : AsyncOperationBase<UnityWebRequest>
	{
		public UnityWebRequestOperation(UnityWebRequest webRequest)
		{
			this.m_UWR = webRequest;
		}

		protected override void Execute()
		{
			this.m_UWR.SendWebRequest().completed += delegate(AsyncOperation request)
			{
				base.Complete(this.m_UWR, string.IsNullOrEmpty(this.m_UWR.error), this.m_UWR.error);
			};
		}

		private UnityWebRequest m_UWR;
	}
}
