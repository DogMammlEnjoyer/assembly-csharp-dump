using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;

namespace System.Runtime.Remoting.Contexts
{
	/// <summary>Indicates that the implementing property is interested in participating in activation and might not have provided a message sink.</summary>
	[ComVisible(true)]
	public interface IContextPropertyActivator
	{
		/// <summary>Called on each client context property that has this interface, before the construction request leaves the client.</summary>
		/// <param name="msg">An <see cref="T:System.Runtime.Remoting.Activation.IConstructionCallMessage" />.</param>
		void CollectFromClientContext(IConstructionCallMessage msg);

		/// <summary>Called on each server context property that has this interface, before the construction response leaves the server for the client.</summary>
		/// <param name="msg">An <see cref="T:System.Runtime.Remoting.Activation.IConstructionReturnMessage" />.</param>
		void CollectFromServerContext(IConstructionReturnMessage msg);

		/// <summary>Called on each client context property that has this interface, when the construction request returns to the client from the server.</summary>
		/// <param name="msg">An <see cref="T:System.Runtime.Remoting.Activation.IConstructionCallMessage" />.</param>
		/// <returns>
		///   <see langword="true" /> if successful; otherwise, <see langword="false" />.</returns>
		bool DeliverClientContextToServerContext(IConstructionCallMessage msg);

		/// <summary>Called on each client context property that has this interface, when the construction request returns to the client from the server.</summary>
		/// <param name="msg">An <see cref="T:System.Runtime.Remoting.Activation.IConstructionReturnMessage" />.</param>
		/// <returns>
		///   <see langword="true" /> if successful; otherwise, <see langword="false" />.</returns>
		bool DeliverServerContextToClientContext(IConstructionReturnMessage msg);

		/// <summary>Indicates whether it is all right to activate the object type indicated in the <paramref name="msg" /> parameter.</summary>
		/// <param name="msg">An <see cref="T:System.Runtime.Remoting.Activation.IConstructionCallMessage" />.</param>
		/// <returns>A Boolean value indicating whether the requested type can be activated.</returns>
		bool IsOKToActivate(IConstructionCallMessage msg);
	}
}
