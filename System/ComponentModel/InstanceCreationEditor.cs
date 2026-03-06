using System;

namespace System.ComponentModel
{
	/// <summary>Creates an instance of a particular type of property from a drop-down box within the <see cref="T:System.Windows.Forms.PropertyGrid" />.</summary>
	public abstract class InstanceCreationEditor
	{
		/// <summary>Gets the specified text.</summary>
		/// <returns>The specified text.</returns>
		public virtual string Text
		{
			get
			{
				return "(New...)";
			}
		}

		/// <summary>When overridden in a derived class, returns an instance of the specified type.</summary>
		/// <param name="context">The context information.</param>
		/// <param name="instanceType">The specified type.</param>
		/// <returns>An instance of the specified type or <see langword="null" />.</returns>
		public abstract object CreateInstance(ITypeDescriptorContext context, Type instanceType);
	}
}
