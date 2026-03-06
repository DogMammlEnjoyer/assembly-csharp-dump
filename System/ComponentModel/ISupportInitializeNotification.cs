using System;

namespace System.ComponentModel
{
	/// <summary>Allows coordination of initialization for a component and its dependent properties.</summary>
	public interface ISupportInitializeNotification : ISupportInitialize
	{
		/// <summary>Gets a value indicating whether the component is initialized.</summary>
		/// <returns>
		///   <see langword="true" /> to indicate the component has completed initialization; otherwise, <see langword="false" />.</returns>
		bool IsInitialized { get; }

		/// <summary>Occurs when initialization of the component is completed.</summary>
		event EventHandler Initialized;
	}
}
