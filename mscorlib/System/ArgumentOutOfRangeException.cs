using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	/// <summary>The exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.</summary>
	[Serializable]
	public class ArgumentOutOfRangeException : ArgumentException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ArgumentOutOfRangeException" /> class.</summary>
		public ArgumentOutOfRangeException() : base("Specified argument was out of the range of valid values.")
		{
			base.HResult = -2146233086;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgumentOutOfRangeException" /> class with the name of the parameter that causes this exception.</summary>
		/// <param name="paramName">The name of the parameter that causes this exception.</param>
		public ArgumentOutOfRangeException(string paramName) : base("Specified argument was out of the range of valid values.", paramName)
		{
			base.HResult = -2146233086;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgumentOutOfRangeException" /> class with the name of the parameter that causes this exception and a specified error message.</summary>
		/// <param name="paramName">The name of the parameter that caused the exception.</param>
		/// <param name="message">The message that describes the error.</param>
		public ArgumentOutOfRangeException(string paramName, string message) : base(message, paramName)
		{
			base.HResult = -2146233086;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgumentOutOfRangeException" /> class with a specified error message and the exception that is the cause of this exception.</summary>
		/// <param name="message">The error message that explains the reason for this exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
		public ArgumentOutOfRangeException(string message, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2146233086;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgumentOutOfRangeException" /> class with the parameter name, the value of the argument, and a specified error message.</summary>
		/// <param name="paramName">The name of the parameter that caused the exception.</param>
		/// <param name="actualValue">The value of the argument that causes this exception.</param>
		/// <param name="message">The message that describes the error.</param>
		public ArgumentOutOfRangeException(string paramName, object actualValue, string message) : base(message, paramName)
		{
			this._actualValue = actualValue;
			base.HResult = -2146233086;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgumentOutOfRangeException" /> class with serialized data.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">An object that describes the source or destination of the serialized data.</param>
		protected ArgumentOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this._actualValue = info.GetValue("ActualValue", typeof(object));
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the invalid argument value and additional exception information.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">An object that describes the source or destination of the serialized data.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> object is <see langword="null" />.</exception>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ActualValue", this._actualValue, typeof(object));
		}

		/// <summary>Gets the error message and the string representation of the invalid argument value, or only the error message if the argument value is null.</summary>
		/// <returns>The text message for this exception. The value of this property takes one of two forms, as follows.  
		///   Condition  
		///
		///   Value  
		///
		///   The <paramref name="actualValue" /> is <see langword="null" />.  
		///
		///   The <paramref name="message" /> string passed to the constructor.  
		///
		///   The <paramref name="actualValue" /> is not <see langword="null" />.  
		///
		///   The <paramref name="message" /> string appended with the string representation of the invalid argument value.</returns>
		public override string Message
		{
			get
			{
				string message = base.Message;
				if (this._actualValue == null)
				{
					return message;
				}
				string text = SR.Format("Actual value was {0}.", this._actualValue.ToString());
				if (message == null)
				{
					return text;
				}
				return message + Environment.NewLine + text;
			}
		}

		/// <summary>Gets the argument value that causes this exception.</summary>
		/// <returns>An <see langword="Object" /> that contains the value of the parameter that caused the current <see cref="T:System.Exception" />.</returns>
		public virtual object ActualValue
		{
			get
			{
				return this._actualValue;
			}
		}

		private object _actualValue;
	}
}
