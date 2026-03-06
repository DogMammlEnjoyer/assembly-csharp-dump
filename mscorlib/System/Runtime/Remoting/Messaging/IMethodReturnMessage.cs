using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	/// <summary>Defines the method call return message interface.</summary>
	[ComVisible(true)]
	public interface IMethodReturnMessage : IMethodMessage, IMessage
	{
		/// <summary>Gets the exception thrown during the method call.</summary>
		/// <returns>The exception object for the method call, or <see langword="null" /> if the method did not throw an exception.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		Exception Exception { get; }

		/// <summary>Gets the number of arguments in the method call marked as <see langword="ref" /> or <see langword="out" /> parameters.</summary>
		/// <returns>The number of arguments in the method call marked as <see langword="ref" /> or <see langword="out" /> parameters.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		int OutArgCount { get; }

		/// <summary>Returns the specified argument marked as a <see langword="ref" /> or an <see langword="out" /> parameter.</summary>
		/// <returns>The specified argument marked as a <see langword="ref" /> or an <see langword="out" /> parameter.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		object[] OutArgs { get; }

		/// <summary>Gets the return value of the method call.</summary>
		/// <returns>The return value of the method call.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		object ReturnValue { get; }

		/// <summary>Returns the specified argument marked as a <see langword="ref" /> or an <see langword="out" /> parameter.</summary>
		/// <param name="argNum">The number of the requested argument.</param>
		/// <returns>The specified argument marked as a <see langword="ref" /> or an <see langword="out" /> parameter.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		object GetOutArg(int argNum);

		/// <summary>Returns the name of the specified argument marked as a <see langword="ref" /> or an <see langword="out" /> parameter.</summary>
		/// <param name="index">The number of the requested argument name.</param>
		/// <returns>The argument name, or <see langword="null" /> if the current method is not implemented.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller makes the call through a reference to the interface and does not have infrastructure permission.</exception>
		string GetOutArgName(int index);
	}
}
