using System;
using System.Collections;
using System.IO;

namespace System.ComponentModel.Design.Serialization
{
	/// <summary>Provides the base class for storing serialization data for the <see cref="T:System.ComponentModel.Design.Serialization.ComponentSerializationService" />.</summary>
	public abstract class SerializationStore : IDisposable
	{
		/// <summary>Gets a collection of errors that occurred during serialization or deserialization.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains errors that occurred during serialization or deserialization.</returns>
		public abstract ICollection Errors { get; }

		/// <summary>Closes the serialization store.</summary>
		public abstract void Close();

		/// <summary>Saves the store to the given stream.</summary>
		/// <param name="stream">The stream to which the store will be serialized.</param>
		public abstract void Save(Stream stream);

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Design.Serialization.SerializationStore" />.</summary>
		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Design.Serialization.SerializationStore" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}
	}
}
