using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.Globalization
{
	/// <summary>The exception that is thrown when a method attempts to construct a culture that is not available.</summary>
	[Serializable]
	public class CultureNotFoundException : ArgumentException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with its message string set to a system-supplied message.</summary>
		public CultureNotFoundException() : base(CultureNotFoundException.DefaultMessage)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with the specified error message.</summary>
		/// <param name="message">The error message to display with this exception.</param>
		public CultureNotFoundException(string message) : base(message)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with a specified error message and the name of the parameter that is the cause this exception.</summary>
		/// <param name="paramName">The name of the parameter that is the cause of the current exception.</param>
		/// <param name="message">The error message to display with this exception.</param>
		public CultureNotFoundException(string paramName, string message) : base(message, paramName)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message to display with this exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public CultureNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with a specified error message, the invalid Culture Name, and the name of the parameter that is the cause this exception.</summary>
		/// <param name="paramName">The name of the parameter that is the cause the current exception.</param>
		/// <param name="invalidCultureName">The Culture Name that cannot be found.</param>
		/// <param name="message">The error message to display with this exception.</param>
		public CultureNotFoundException(string paramName, string invalidCultureName, string message) : base(message, paramName)
		{
			this._invalidCultureName = invalidCultureName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with a specified error message, the invalid Culture Name, and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message to display with this exception.</param>
		/// <param name="invalidCultureName">The Culture Name that cannot be found.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public CultureNotFoundException(string message, string invalidCultureName, Exception innerException) : base(message, innerException)
		{
			this._invalidCultureName = invalidCultureName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with a specified error message, the invalid Culture ID, and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message to display with this exception.</param>
		/// <param name="invalidCultureId">The Culture ID that cannot be found.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public CultureNotFoundException(string message, int invalidCultureId, Exception innerException) : base(message, innerException)
		{
			this._invalidCultureId = new int?(invalidCultureId);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class with a specified error message, the invalid Culture ID, and the name of the parameter that is the cause this exception.</summary>
		/// <param name="paramName">The name of the parameter that is the cause the current exception.</param>
		/// <param name="invalidCultureId">The Culture ID that cannot be found.</param>
		/// <param name="message">The error message to display with this exception.</param>
		public CultureNotFoundException(string paramName, int invalidCultureId, string message) : base(message, paramName)
		{
			this._invalidCultureId = new int?(invalidCultureId);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Globalization.CultureNotFoundException" /> class using the specified serialization data and context.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected CultureNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this._invalidCultureId = (int?)info.GetValue("InvalidCultureId", typeof(int?));
			this._invalidCultureName = (string)info.GetValue("InvalidCultureName", typeof(string));
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the parameter name and additional exception information.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InvalidCultureId", this._invalidCultureId, typeof(int?));
			info.AddValue("InvalidCultureName", this._invalidCultureName, typeof(string));
		}

		/// <summary>Gets the culture identifier that cannot be found.</summary>
		/// <returns>The invalid culture identifier.</returns>
		public virtual int? InvalidCultureId
		{
			get
			{
				return this._invalidCultureId;
			}
		}

		/// <summary>Gets the culture name that cannot be found.</summary>
		/// <returns>The invalid culture name.</returns>
		public virtual string InvalidCultureName
		{
			get
			{
				return this._invalidCultureName;
			}
		}

		private static string DefaultMessage
		{
			get
			{
				return "Culture is not supported.";
			}
		}

		private string FormatedInvalidCultureId
		{
			get
			{
				if (this.InvalidCultureId == null)
				{
					return this.InvalidCultureName;
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} (0x{0:x4})", this.InvalidCultureId.Value);
			}
		}

		/// <summary>Gets the error message that explains the reason for the exception.</summary>
		/// <returns>A text string describing the details of the exception.</returns>
		public override string Message
		{
			get
			{
				string message = base.Message;
				if (this._invalidCultureId == null && this._invalidCultureName == null)
				{
					return message;
				}
				string text = SR.Format("{0} is an invalid culture identifier.", this.FormatedInvalidCultureId);
				if (message == null)
				{
					return text;
				}
				return message + Environment.NewLine + text;
			}
		}

		private string _invalidCultureName;

		private int? _invalidCultureId;
	}
}
