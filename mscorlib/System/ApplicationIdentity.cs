using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	/// <summary>Provides the ability to uniquely identify a manifest-activated application. This class cannot be inherited.</summary>
	[ComVisible(false)]
	[Serializable]
	public sealed class ApplicationIdentity : ISerializable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ApplicationIdentity" /> class.</summary>
		/// <param name="applicationIdentityFullName">The full name of the application.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="applicationIdentityFullName" /> is <see langword="null" />.</exception>
		public ApplicationIdentity(string applicationIdentityFullName)
		{
			if (applicationIdentityFullName == null)
			{
				throw new ArgumentNullException("applicationIdentityFullName");
			}
			if (applicationIdentityFullName.IndexOf(", Culture=") == -1)
			{
				this._fullName = applicationIdentityFullName + ", Culture=neutral";
				return;
			}
			this._fullName = applicationIdentityFullName;
		}

		/// <summary>Gets the location of the deployment manifest as a URL.</summary>
		/// <returns>The URL of the deployment manifest.</returns>
		public string CodeBase
		{
			get
			{
				return this._codeBase;
			}
		}

		/// <summary>Gets the full name of the application.</summary>
		/// <returns>The full name of the application, also known as the display name.</returns>
		public string FullName
		{
			get
			{
				return this._fullName;
			}
		}

		/// <summary>Returns the full name of the manifest-activated application.</summary>
		/// <returns>The full name of the manifest-activated application.</returns>
		public override string ToString()
		{
			return this._fullName;
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data needed to serialize the target object.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />) structure for the serialization.</param>
		[MonoTODO("Missing serialization")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
		}

		private string _fullName;

		private string _codeBase;
	}
}
