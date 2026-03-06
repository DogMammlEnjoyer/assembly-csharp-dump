using System;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.Exceptions
{
	public class RemoteProviderException : ProviderException
	{
		public RemoteProviderException(string message, IResourceLocation location = null, UnityWebRequestResult uwrResult = null, Exception innerException = null) : base(message, location, innerException)
		{
			this.WebRequestResult = uwrResult;
		}

		public override string Message
		{
			get
			{
				return this.ToString();
			}
		}

		public UnityWebRequestResult WebRequestResult { get; }

		public override string ToString()
		{
			if (this.WebRequestResult != null)
			{
				return string.Format("{0} : {1}\nUnityWebRequest result : {2}\n{3}", new object[]
				{
					base.GetType().Name,
					base.Message,
					this.WebRequestResult,
					base.InnerException
				});
			}
			return base.ToString();
		}
	}
}
