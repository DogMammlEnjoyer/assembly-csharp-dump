using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	/// <summary>The exception that is thrown as a wrapper around the exception thrown by the class initializer. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class TypeInitializationException : SystemException
	{
		private TypeInitializationException() : base("Type constructor threw an exception.")
		{
			base.HResult = -2146233036;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.TypeInitializationException" /> class with the default error message, the specified type name, and a reference to the inner exception that is the root cause of this exception.</summary>
		/// <param name="fullTypeName">The fully qualified name of the type that fails to initialize.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference (<see langword="Nothing" /> in Visual Basic), the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public TypeInitializationException(string fullTypeName, Exception innerException) : this(fullTypeName, SR.Format("The type initializer for '{0}' threw an exception.", fullTypeName), innerException)
		{
		}

		internal TypeInitializationException(string message) : base(message)
		{
			base.HResult = -2146233036;
		}

		internal TypeInitializationException(string fullTypeName, string message, Exception innerException) : base(message, innerException)
		{
			this._typeName = fullTypeName;
			base.HResult = -2146233036;
		}

		internal TypeInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this._typeName = info.GetString("TypeName");
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the type name and additional exception information.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TypeName", this.TypeName, typeof(string));
		}

		/// <summary>Gets the fully qualified name of the type that fails to initialize.</summary>
		/// <returns>The fully qualified name of the type that fails to initialize.</returns>
		public string TypeName
		{
			get
			{
				if (this._typeName == null)
				{
					return string.Empty;
				}
				return this._typeName;
			}
		}

		private string _typeName;
	}
}
