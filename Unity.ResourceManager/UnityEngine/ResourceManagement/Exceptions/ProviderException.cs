using System;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.Exceptions
{
	public class ProviderException : OperationException
	{
		public ProviderException(string message, IResourceLocation location = null, Exception innerException = null) : base(message, innerException)
		{
			this.Location = location;
		}

		public IResourceLocation Location { get; }
	}
}
