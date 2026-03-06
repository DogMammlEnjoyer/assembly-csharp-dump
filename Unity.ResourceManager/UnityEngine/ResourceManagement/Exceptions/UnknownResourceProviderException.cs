using System;
using System.Runtime.Serialization;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.Exceptions
{
	public class UnknownResourceProviderException : ResourceManagerException
	{
		public IResourceLocation Location { get; private set; }

		public UnknownResourceProviderException(IResourceLocation location)
		{
			this.Location = location;
		}

		public UnknownResourceProviderException()
		{
		}

		public UnknownResourceProviderException(string message) : base(message)
		{
		}

		public UnknownResourceProviderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UnknownResourceProviderException(SerializationInfo message, StreamingContext context) : base(message, context)
		{
		}

		public override string Message
		{
			get
			{
				string[] array = new string[5];
				array[0] = base.Message;
				array[1] = ", ProviderId=";
				array[2] = this.Location.ProviderId;
				array[3] = ", Location=";
				int num = 4;
				IResourceLocation location = this.Location;
				array[num] = ((location != null) ? location.ToString() : null);
				return string.Concat(array);
			}
		}

		public override string ToString()
		{
			return this.Message;
		}
	}
}
