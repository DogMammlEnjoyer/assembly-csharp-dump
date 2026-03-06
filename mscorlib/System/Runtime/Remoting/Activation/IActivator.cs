using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Activation
{
	/// <summary>Provides the basic functionality for a remoting activator class.</summary>
	[ComVisible(true)]
	public interface IActivator
	{
		/// <summary>Gets the <see cref="T:System.Runtime.Remoting.Activation.ActivatorLevel" /> where this activator is active.</summary>
		/// <returns>The <see cref="T:System.Runtime.Remoting.Activation.ActivatorLevel" /> where this activator is active.</returns>
		ActivatorLevel Level { get; }

		/// <summary>Gets or sets the next activator in the chain.</summary>
		/// <returns>The next activator in the chain.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		IActivator NextActivator { get; set; }

		/// <summary>Creates an instance of the object that is specified in the provided <see cref="T:System.Runtime.Remoting.Activation.IConstructionCallMessage" />.</summary>
		/// <param name="msg">The information about the object that is needed to activate it, stored in a <see cref="T:System.Runtime.Remoting.Activation.IConstructionCallMessage" />.</param>
		/// <returns>Status of the object activation contained in a <see cref="T:System.Runtime.Remoting.Activation.IConstructionReturnMessage" />.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		IConstructionReturnMessage Activate(IConstructionCallMessage msg);
	}
}
