using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Contexts
{
	/// <summary>Gathers naming information from the context property and determines whether the new context is ok for the context property.</summary>
	[ComVisible(true)]
	public interface IContextProperty
	{
		/// <summary>Gets the name of the property under which it will be added to the context.</summary>
		/// <returns>The name of the property.</returns>
		string Name { get; }

		/// <summary>Called when the context is frozen.</summary>
		/// <param name="newContext">The context to freeze.</param>
		void Freeze(Context newContext);

		/// <summary>Returns a Boolean value indicating whether the context property is compatible with the new context.</summary>
		/// <param name="newCtx">The new context in which the <see cref="T:System.Runtime.Remoting.Contexts.ContextProperty" /> has been created.</param>
		/// <returns>
		///   <see langword="true" /> if the context property can coexist with the other context properties in the given context; otherwise, <see langword="false" />.</returns>
		bool IsNewContextOK(Context newCtx);
	}
}
