using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	/// <summary>Specifies the default interface of a managed Windows Runtime class.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class DefaultInterfaceAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.WindowsRuntime.DefaultInterfaceAttribute" /> class.</summary>
		/// <param name="defaultInterface">The interface type that is specified as the default interface for the class the attribute is applied to.</param>
		public DefaultInterfaceAttribute(Type defaultInterface)
		{
			this.m_defaultInterface = defaultInterface;
		}

		/// <summary>Gets the type of the default interface.</summary>
		/// <returns>The type of the default interface.</returns>
		public Type DefaultInterface
		{
			get
			{
				return this.m_defaultInterface;
			}
		}

		private Type m_defaultInterface;
	}
}
