using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	/// <summary>The exception that is thrown when there is an attempt to dynamically access a class member that does not exist or that is not declared as public. If a member in a class library has been removed or renamed, recompile any assemblies that reference that library.</summary>
	[Serializable]
	public class MissingMemberException : MemberAccessException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.MissingMemberException" /> class.</summary>
		public MissingMemberException() : base("Attempted to access a missing member.")
		{
			base.HResult = -2146233070;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.MissingMemberException" /> class with a specified error message.</summary>
		/// <param name="message">The message that describes the error.</param>
		public MissingMemberException(string message) : base(message)
		{
			base.HResult = -2146233070;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.MissingMemberException" /> class with a specified error message and a reference to the inner exception that is the root cause of this exception.</summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">An instance of <see cref="T:System.Exception" /> that is the cause of the current <see langword="Exception" />. If <paramref name="inner" /> is not a null reference (<see langword="Nothing" /> in Visual Basic), then the current <see langword="Exception" /> is raised in a catch block handling <paramref name="inner" />.</param>
		public MissingMemberException(string message, Exception inner) : base(message, inner)
		{
			base.HResult = -2146233070;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.MissingMemberException" /> class with the specified class name and member name.</summary>
		/// <param name="className">The name of the class in which access to a nonexistent member was attempted.</param>
		/// <param name="memberName">The name of the member that cannot be accessed.</param>
		public MissingMemberException(string className, string memberName)
		{
			this.ClassName = className;
			this.MemberName = memberName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.MissingMemberException" /> class with serialized data.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected MissingMemberException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ClassName = info.GetString("MMClassName");
			this.MemberName = info.GetString("MMMemberName");
			this.Signature = (byte[])info.GetValue("MMSignature", typeof(byte[]));
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the class name, the member name, the signature of the missing member, and additional exception information.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> object is <see langword="null" />.</exception>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MMClassName", this.ClassName, typeof(string));
			info.AddValue("MMMemberName", this.MemberName, typeof(string));
			info.AddValue("MMSignature", this.Signature, typeof(byte[]));
		}

		/// <summary>Gets the text string showing the class name, the member name, and the signature of the missing member.</summary>
		/// <returns>The error message string.</returns>
		public override string Message
		{
			[SecuritySafeCritical]
			get
			{
				if (this.ClassName == null)
				{
					return base.Message;
				}
				return SR.Format("Member '{0}' not found.", this.ClassName + "." + this.MemberName + ((this.Signature != null) ? (" " + MissingMemberException.FormatSignature(this.Signature)) : string.Empty));
			}
		}

		internal static string FormatSignature(byte[] signature)
		{
			return string.Empty;
		}

		/// <summary>Holds the class name of the missing member.</summary>
		protected string ClassName;

		/// <summary>Holds the name of the missing member.</summary>
		protected string MemberName;

		/// <summary>Holds the signature of the missing member.</summary>
		protected byte[] Signature;
	}
}
