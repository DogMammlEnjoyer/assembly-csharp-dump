using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	/// <summary>The exception that is thrown when type-loading failures occur.</summary>
	[ComVisible(true)]
	[Serializable]
	public class TypeLoadException : SystemException, ISerializable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.TypeLoadException" /> class.</summary>
		public TypeLoadException() : base(Environment.GetResourceString("Failure has occurred while loading a type."))
		{
			base.SetErrorCode(-2146233054);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.TypeLoadException" /> class with a specified error message.</summary>
		/// <param name="message">The message that describes the error.</param>
		public TypeLoadException(string message) : base(message)
		{
			base.SetErrorCode(-2146233054);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.TypeLoadException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception. If the <paramref name="inner" /> parameter is not <see langword="null" />, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public TypeLoadException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233054);
		}

		/// <summary>Gets the error message for this exception.</summary>
		/// <returns>The error message string.</returns>
		public override string Message
		{
			[SecuritySafeCritical]
			get
			{
				this.SetMessageField();
				return this._message;
			}
		}

		[SecurityCritical]
		private void SetMessageField()
		{
			if (this._message == null)
			{
				if (this.ClassName == null && this.ResourceId == 0)
				{
					this._message = Environment.GetResourceString("Failure has occurred while loading a type.");
					return;
				}
				if (this.AssemblyName == null)
				{
					this.AssemblyName = Environment.GetResourceString("[Unknown]");
				}
				if (this.ClassName == null)
				{
					this.ClassName = Environment.GetResourceString("[Unknown]");
				}
				string format = "Could not load type '{0}' from assembly '{1}'.";
				this._message = string.Format(CultureInfo.CurrentCulture, format, this.ClassName, this.AssemblyName, this.MessageArg);
			}
		}

		/// <summary>Gets the fully qualified name of the type that causes the exception.</summary>
		/// <returns>The fully qualified type name.</returns>
		public string TypeName
		{
			get
			{
				if (this.ClassName == null)
				{
					return string.Empty;
				}
				return this.ClassName;
			}
		}

		private TypeLoadException(string className, string assemblyName) : this(className, assemblyName, null, 0)
		{
		}

		[SecurityCritical]
		private TypeLoadException(string className, string assemblyName, string messageArg, int resourceId) : base(null)
		{
			base.SetErrorCode(-2146233054);
			this.ClassName = className;
			this.AssemblyName = assemblyName;
			this.MessageArg = messageArg;
			this.ResourceId = resourceId;
			this.SetMessageField();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.TypeLoadException" /> class with serialized data.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> object is <see langword="null" />.</exception>
		protected TypeLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.ClassName = info.GetString("TypeLoadClassName");
			this.AssemblyName = info.GetString("TypeLoadAssemblyName");
			this.MessageArg = info.GetString("TypeLoadMessageArg");
			this.ResourceId = info.GetInt32("TypeLoadResourceID");
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the class name, method name, resource ID, and additional exception information.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> object is <see langword="null" />.</exception>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("TypeLoadClassName", this.ClassName, typeof(string));
			info.AddValue("TypeLoadAssemblyName", this.AssemblyName, typeof(string));
			info.AddValue("TypeLoadMessageArg", this.MessageArg, typeof(string));
			info.AddValue("TypeLoadResourceID", this.ResourceId);
		}

		private string ClassName;

		private string AssemblyName;

		private string MessageArg;

		internal int ResourceId;
	}
}
