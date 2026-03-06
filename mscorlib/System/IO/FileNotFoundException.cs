using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.IO
{
	/// <summary>The exception that is thrown when an attempt to access a file that does not exist on disk fails.</summary>
	[Serializable]
	public class FileNotFoundException : IOException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileNotFoundException" /> class with its message string set to a system-supplied message.</summary>
		public FileNotFoundException() : base("Unable to find the specified file.")
		{
			base.HResult = -2147024894;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileNotFoundException" /> class with a specified error message.</summary>
		/// <param name="message">A description of the error. The content of <paramref name="message" /> is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
		public FileNotFoundException(string message) : base(message)
		{
			base.HResult = -2147024894;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileNotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">A description of the error. The content of <paramref name="message" /> is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not <see langword="null" />, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public FileNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2147024894;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileNotFoundException" /> class with a specified error message, and the file name that cannot be found.</summary>
		/// <param name="message">A description of the error. The content of <paramref name="message" /> is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
		/// <param name="fileName">The full name of the file with the invalid image.</param>
		public FileNotFoundException(string message, string fileName) : base(message)
		{
			base.HResult = -2147024894;
			this.FileName = fileName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileNotFoundException" /> class with a specified error message, the file name that cannot be found, and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="fileName">The full name of the file with the invalid image.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not <see langword="null" />, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public FileNotFoundException(string message, string fileName, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2147024894;
			this.FileName = fileName;
		}

		/// <summary>Gets the error message that explains the reason for the exception.</summary>
		/// <returns>The error message.</returns>
		public override string Message
		{
			get
			{
				this.SetMessageField();
				return this._message;
			}
		}

		private void SetMessageField()
		{
			if (this._message == null)
			{
				if (this.FileName == null && base.HResult == -2146233088)
				{
					this._message = "Unable to find the specified file.";
					return;
				}
				if (this.FileName != null)
				{
					this._message = FileLoadException.FormatFileLoadExceptionMessage(this.FileName, base.HResult);
				}
			}
		}

		/// <summary>Gets the name of the file that cannot be found.</summary>
		/// <returns>The name of the file, or <see langword="null" /> if no file name was passed to the constructor for this instance.</returns>
		public string FileName { get; }

		/// <summary>Gets the log file that describes why loading of an assembly failed.</summary>
		/// <returns>The errors reported by the assembly cache.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public string FusionLog { get; }

		/// <summary>Returns the fully qualified name of this exception and possibly the error message, the name of the inner exception, and the stack trace.</summary>
		/// <returns>The fully qualified name of this exception and possibly the error message, the name of the inner exception, and the stack trace.</returns>
		public override string ToString()
		{
			string text = base.GetType().ToString() + ": " + this.Message;
			if (this.FileName != null && this.FileName.Length != 0)
			{
				text = text + Environment.NewLine + SR.Format("File name: '{0}'", this.FileName);
			}
			if (base.InnerException != null)
			{
				text = text + " ---> " + base.InnerException.ToString();
			}
			if (this.StackTrace != null)
			{
				text = text + Environment.NewLine + this.StackTrace;
			}
			if (this.FusionLog != null)
			{
				if (text == null)
				{
					text = " ";
				}
				text += Environment.NewLine;
				text += Environment.NewLine;
				text += this.FusionLog;
			}
			return text;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileNotFoundException" /> class with the specified serialization and context information.</summary>
		/// <param name="info">An object that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">An object that contains contextual information about the source or destination.</param>
		protected FileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.FileName = info.GetString("FileNotFound_FileName");
			this.FusionLog = info.GetString("FileNotFound_FusionLog");
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the file name and additional exception information.</summary>
		/// <param name="info">The object that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The object that contains contextual information about the source or destination.</param>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FileNotFound_FileName", this.FileName, typeof(string));
			info.AddValue("FileNotFound_FusionLog", this.FusionLog, typeof(string));
		}
	}
}
