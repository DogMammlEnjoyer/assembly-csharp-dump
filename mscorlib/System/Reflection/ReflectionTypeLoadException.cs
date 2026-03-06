using System;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace System.Reflection
{
	/// <summary>The exception that is thrown by the <see cref="M:System.Reflection.Module.GetTypes" /> method if any of the classes in a module cannot be loaded. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class ReflectionTypeLoadException : SystemException, ISerializable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.ReflectionTypeLoadException" /> class with the given classes and their associated exceptions.</summary>
		/// <param name="classes">An array of type <see langword="Type" /> containing the classes that were defined in the module and loaded. This array can contain null reference (<see langword="Nothing" /> in Visual Basic) values.</param>
		/// <param name="exceptions">An array of type <see langword="Exception" /> containing the exceptions that were thrown by the class loader. The null reference (<see langword="Nothing" /> in Visual Basic) values in the <paramref name="classes" /> array line up with the exceptions in this <paramref name="exceptions" /> array.</param>
		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions) : base(null)
		{
			this.Types = classes;
			this.LoaderExceptions = exceptions;
			base.HResult = -2146232830;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.ReflectionTypeLoadException" /> class with the given classes, their associated exceptions, and exception descriptions.</summary>
		/// <param name="classes">An array of type <see langword="Type" /> containing the classes that were defined in the module and loaded. This array can contain null reference (<see langword="Nothing" /> in Visual Basic) values.</param>
		/// <param name="exceptions">An array of type <see langword="Exception" /> containing the exceptions that were thrown by the class loader. The null reference (<see langword="Nothing" /> in Visual Basic) values in the <paramref name="classes" /> array line up with the exceptions in this <paramref name="exceptions" /> array.</param>
		/// <param name="message">A <see langword="String" /> describing the reason the exception was thrown.</param>
		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions, string message) : base(message)
		{
			this.Types = classes;
			this.LoaderExceptions = exceptions;
			base.HResult = -2146232830;
		}

		private ReflectionTypeLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.LoaderExceptions = (Exception[])info.GetValue("Exceptions", typeof(Exception[]));
		}

		/// <summary>Provides an <see cref="T:System.Runtime.Serialization.ISerializable" /> implementation for serialized objects.</summary>
		/// <param name="info">The information and data needed to serialize or deserialize an object.</param>
		/// <param name="context">The context for the serialization.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see langword="info" /> is <see langword="null" />.</exception>
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Types", null, typeof(Type[]));
			info.AddValue("Exceptions", this.LoaderExceptions, typeof(Exception[]));
		}

		/// <summary>Gets the array of classes that were defined in the module and loaded.</summary>
		/// <returns>An array of type <see langword="Type" /> containing the classes that were defined in the module and loaded. This array can contain some <see langword="null" /> values.</returns>
		public Type[] Types { get; }

		/// <summary>Gets the array of exceptions thrown by the class loader.</summary>
		/// <returns>An array of type <see langword="Exception" /> containing the exceptions thrown by the class loader. The null values in the <see cref="P:System.Reflection.ReflectionTypeLoadException.Types" /> array of this instance line up with the exceptions in this array.</returns>
		public Exception[] LoaderExceptions { get; }

		public override string Message
		{
			get
			{
				return this.CreateString(true);
			}
		}

		public override string ToString()
		{
			return this.CreateString(false);
		}

		private string CreateString(bool isMessage)
		{
			string text = isMessage ? base.Message : base.ToString();
			Exception[] loaderExceptions = this.LoaderExceptions;
			if (loaderExceptions == null || loaderExceptions.Length == 0)
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder(text);
			foreach (Exception ex in loaderExceptions)
			{
				if (ex != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append(isMessage ? ex.Message : ex.ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
