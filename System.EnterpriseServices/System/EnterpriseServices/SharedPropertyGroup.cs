using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.EnterpriseServices
{
	/// <summary>Represents a collection of shared properties. This class cannot be inherited.</summary>
	[ComVisible(false)]
	public sealed class SharedPropertyGroup
	{
		internal SharedPropertyGroup(ISharedPropertyGroup propertyGroup)
		{
			this.propertyGroup = propertyGroup;
		}

		/// <summary>Creates a property with the given name.</summary>
		/// <param name="name">The name of the new property.</param>
		/// <param name="fExists">Determines whether the property exists. Set to <see langword="true" /> on return if the property exists.</param>
		/// <returns>The requested <see cref="T:System.EnterpriseServices.SharedProperty" />.</returns>
		public SharedProperty CreateProperty(string name, out bool fExists)
		{
			return new SharedProperty(this.propertyGroup.CreateProperty(name, out fExists));
		}

		/// <summary>Creates a property at the given position.</summary>
		/// <param name="position">The index of the new property</param>
		/// <param name="fExists">Determines whether the property exists. Set to <see langword="true" /> on return if the property exists.</param>
		/// <returns>The requested <see cref="T:System.EnterpriseServices.SharedProperty" />.</returns>
		public SharedProperty CreatePropertyByPosition(int position, out bool fExists)
		{
			return new SharedProperty(this.propertyGroup.CreatePropertyByPosition(position, out fExists));
		}

		/// <summary>Returns the property with the given name.</summary>
		/// <param name="name">The name of requested property.</param>
		/// <returns>The requested <see cref="T:System.EnterpriseServices.SharedProperty" />.</returns>
		public SharedProperty Property(string name)
		{
			return new SharedProperty(this.propertyGroup.Property(name));
		}

		/// <summary>Returns the property at the given position.</summary>
		/// <param name="position">The index of the property.</param>
		/// <returns>The requested <see cref="T:System.EnterpriseServices.SharedProperty" />.</returns>
		public SharedProperty PropertyByPosition(int position)
		{
			return new SharedProperty(this.propertyGroup.PropertyByPosition(position));
		}

		internal SharedPropertyGroup()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ISharedPropertyGroup propertyGroup;
	}
}
