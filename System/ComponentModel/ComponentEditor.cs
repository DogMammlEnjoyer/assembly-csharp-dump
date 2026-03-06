using System;

namespace System.ComponentModel
{
	/// <summary>Provides the base class for a custom component editor.</summary>
	public abstract class ComponentEditor
	{
		/// <summary>Edits the component and returns a value indicating whether the component was modified.</summary>
		/// <param name="component">The component to be edited.</param>
		/// <returns>
		///   <see langword="true" /> if the component was modified; otherwise, <see langword="false" />.</returns>
		public bool EditComponent(object component)
		{
			return this.EditComponent(null, component);
		}

		/// <summary>Edits the component and returns a value indicating whether the component was modified based upon a given context.</summary>
		/// <param name="context">An optional context object that can be used to obtain further information about the edit.</param>
		/// <param name="component">The component to be edited.</param>
		/// <returns>
		///   <see langword="true" /> if the component was modified; otherwise, <see langword="false" />.</returns>
		public abstract bool EditComponent(ITypeDescriptorContext context, object component);
	}
}
