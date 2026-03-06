using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Activation
{
	/// <summary>Represents the construction call request of an object.</summary>
	[ComVisible(true)]
	public interface IConstructionCallMessage : IMessage, IMethodCallMessage, IMethodMessage
	{
		/// <summary>Gets the type of the remote object to activate.</summary>
		/// <returns>The type of the remote object to activate.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		Type ActivationType { get; }

		/// <summary>Gets the full type name of the remote type to activate.</summary>
		/// <returns>The full type name of the remote type to activate.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		string ActivationTypeName { get; }

		/// <summary>Gets or sets the activator that activates the remote object.</summary>
		/// <returns>The activator that activates the remote object.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		IActivator Activator { get; set; }

		/// <summary>Gets the call site activation attributes.</summary>
		/// <returns>The call site activation attributes.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		object[] CallSiteActivationAttributes { get; }

		/// <summary>Gets a list of context properties that define the context in which the object is to be created.</summary>
		/// <returns>A list of properties of the context in which to construct the object.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		IList ContextProperties { get; }
	}
}
