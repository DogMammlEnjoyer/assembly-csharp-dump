using System;

namespace System.ComponentModel.Design.Serialization
{
	/// <summary>Provides a basic designer loader interface that can be used to implement a custom designer loader.</summary>
	public abstract class DesignerLoader
	{
		/// <summary>Gets a value indicating whether the loader is currently loading a document.</summary>
		/// <returns>
		///   <see langword="true" /> if the loader is currently loading a document; otherwise, <see langword="false" />.</returns>
		public virtual bool Loading
		{
			get
			{
				return false;
			}
		}

		/// <summary>Begins loading a designer.</summary>
		/// <param name="host">The loader host through which this loader loads components.</param>
		public abstract void BeginLoad(IDesignerLoaderHost host);

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Design.Serialization.DesignerLoader" />.</summary>
		public abstract void Dispose();

		/// <summary>Writes cached changes to the location that the designer was loaded from.</summary>
		public virtual void Flush()
		{
		}
	}
}
