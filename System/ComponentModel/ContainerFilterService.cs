using System;

namespace System.ComponentModel
{
	/// <summary>Provides a base class for the container filter service.</summary>
	public abstract class ContainerFilterService
	{
		/// <summary>Filters the component collection.</summary>
		/// <param name="components">The component collection to filter.</param>
		/// <returns>A <see cref="T:System.ComponentModel.ComponentCollection" /> that represents a modified collection.</returns>
		public virtual ComponentCollection FilterComponents(ComponentCollection components)
		{
			return components;
		}
	}
}
