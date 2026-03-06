using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	/// <summary>Represents errors that occur during application execution.</summary>
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Exception : ISerializable, _Exception
	{
		private void Init()
		{
			this._message = null;
			this._stackTrace = null;
			this._dynamicMethods = null;
			this.HResult = -2146233088;
			this._safeSerializationManager = new SafeSerializationManager();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class.</summary>
		public Exception()
		{
			this.Init();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.</summary>
		/// <param name="message">The message that describes the error.</param>
		public Exception(string message)
		{
			this.Init();
			this._message = message;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
		public Exception(string message, Exception innerException)
		{
			this.Init();
			this._message = message;
			this._innerException = innerException;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is <see langword="null" /> or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
		[SecuritySafeCritical]
		protected Exception(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this._className = info.GetString("ClassName");
			this._message = info.GetString("Message");
			this._data = (IDictionary)info.GetValueNoThrow("Data", typeof(IDictionary));
			this._innerException = (Exception)info.GetValue("InnerException", typeof(Exception));
			this._helpURL = info.GetString("HelpURL");
			this._stackTraceString = info.GetString("StackTraceString");
			this._remoteStackTraceString = info.GetString("RemoteStackTraceString");
			this._remoteStackIndex = info.GetInt32("RemoteStackIndex");
			this.HResult = info.GetInt32("HResult");
			this._source = info.GetString("Source");
			this._safeSerializationManager = (info.GetValueNoThrow("SafeSerializationManager", typeof(SafeSerializationManager)) as SafeSerializationManager);
			if (this._className == null || this.HResult == 0)
			{
				throw new SerializationException(Environment.GetResourceString("Insufficient state to return the real object."));
			}
			if (context.State == StreamingContextStates.CrossAppDomain)
			{
				this._remoteStackTraceString += this._stackTraceString;
				this._stackTraceString = null;
			}
		}

		/// <summary>Gets a message that describes the current exception.</summary>
		/// <returns>The error message that explains the reason for the exception, or an empty string ("").</returns>
		public virtual string Message
		{
			get
			{
				if (this._message == null)
				{
					if (this._className == null)
					{
						this._className = this.GetClassName();
					}
					return Environment.GetResourceString("Exception of type '{0}' was thrown.", new object[]
					{
						this._className
					});
				}
				return this._message;
			}
		}

		/// <summary>Gets a collection of key/value pairs that provide additional user-defined information about the exception.</summary>
		/// <returns>An object that implements the <see cref="T:System.Collections.IDictionary" /> interface and contains a collection of user-defined key/value pairs. The default is an empty collection.</returns>
		public virtual IDictionary Data
		{
			[SecuritySafeCritical]
			get
			{
				if (this._data == null)
				{
					this._data = new ListDictionaryInternal();
				}
				return this._data;
			}
		}

		private static bool IsImmutableAgileException(Exception e)
		{
			return false;
		}

		internal void AddExceptionDataForRestrictedErrorInfo(string restrictedError, string restrictedErrorReference, string restrictedCapabilitySid, object restrictedErrorObject, bool hasrestrictedLanguageErrorObject = false)
		{
			IDictionary data = this.Data;
			if (data != null)
			{
				data.Add("RestrictedDescription", restrictedError);
				data.Add("RestrictedErrorReference", restrictedErrorReference);
				data.Add("RestrictedCapabilitySid", restrictedCapabilitySid);
				data.Add("__RestrictedErrorObject", (restrictedErrorObject == null) ? null : new Exception.__RestrictedErrorObject(restrictedErrorObject));
				data.Add("__HasRestrictedLanguageErrorObject", hasrestrictedLanguageErrorObject);
			}
		}

		internal bool TryGetRestrictedLanguageErrorObject(out object restrictedErrorObject)
		{
			restrictedErrorObject = null;
			if (this.Data != null && this.Data.Contains("__HasRestrictedLanguageErrorObject"))
			{
				if (this.Data.Contains("__RestrictedErrorObject"))
				{
					Exception.__RestrictedErrorObject _RestrictedErrorObject = this.Data["__RestrictedErrorObject"] as Exception.__RestrictedErrorObject;
					if (_RestrictedErrorObject != null)
					{
						restrictedErrorObject = _RestrictedErrorObject.RealErrorObject;
					}
				}
				return (bool)this.Data["__HasRestrictedLanguageErrorObject"];
			}
			return false;
		}

		private string GetClassName()
		{
			if (this._className == null)
			{
				this._className = this.GetType().ToString();
			}
			return this._className;
		}

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Exception" /> that is the root cause of one or more subsequent exceptions.</summary>
		/// <returns>The first exception thrown in a chain of exceptions. If the <see cref="P:System.Exception.InnerException" /> property of the current exception is a null reference (<see langword="Nothing" /> in Visual Basic), this property returns the current exception.</returns>
		public virtual Exception GetBaseException()
		{
			Exception innerException = this.InnerException;
			Exception result = this;
			while (innerException != null)
			{
				result = innerException;
				innerException = innerException.InnerException;
			}
			return result;
		}

		/// <summary>Gets the <see cref="T:System.Exception" /> instance that caused the current exception.</summary>
		/// <returns>An object that describes the error that caused the current exception. The <see cref="P:System.Exception.InnerException" /> property returns the same value as was passed into the <see cref="M:System.Exception.#ctor(System.String,System.Exception)" /> constructor, or <see langword="null" /> if the inner exception value was not supplied to the constructor. This property is read-only.</returns>
		public Exception InnerException
		{
			get
			{
				return this._innerException;
			}
		}

		/// <summary>Gets the method that throws the current exception.</summary>
		/// <returns>The <see cref="T:System.Reflection.MethodBase" /> that threw the current exception.</returns>
		public MethodBase TargetSite
		{
			[SecuritySafeCritical]
			get
			{
				StackTrace stackTrace = new StackTrace(this, true);
				if (stackTrace.FrameCount > 0)
				{
					return stackTrace.GetFrame(0).GetMethod();
				}
				return null;
			}
		}

		/// <summary>Gets a string representation of the immediate frames on the call stack.</summary>
		/// <returns>A string that describes the immediate frames of the call stack.</returns>
		public virtual string StackTrace
		{
			get
			{
				return this.GetStackTrace(true);
			}
		}

		private string GetStackTrace(bool needFileInfo)
		{
			string text = this._stackTraceString;
			string text2 = this._remoteStackTraceString;
			if (!needFileInfo)
			{
				text = this.StripFileInfo(text, false);
				text2 = this.StripFileInfo(text2, true);
			}
			if (text != null)
			{
				return text2 + text;
			}
			if (this._stackTrace == null)
			{
				return text2;
			}
			string stackTrace = Environment.GetStackTrace(this, needFileInfo);
			return text2 + stackTrace;
		}

		internal void SetErrorCode(int hr)
		{
			this.HResult = hr;
		}

		/// <summary>Gets or sets a link to the help file associated with this exception.</summary>
		/// <returns>The Uniform Resource Name (URN) or Uniform Resource Locator (URL).</returns>
		public virtual string HelpLink
		{
			get
			{
				return this._helpURL;
			}
			set
			{
				this._helpURL = value;
			}
		}

		/// <summary>Gets or sets the name of the application or the object that causes the error.</summary>
		/// <returns>The name of the application or the object that causes the error.</returns>
		/// <exception cref="T:System.ArgumentException">The object must be a runtime <see cref="N:System.Reflection" /> object.</exception>
		public virtual string Source
		{
			get
			{
				if (this._source == null)
				{
					StackTrace stackTrace = new StackTrace(this, true);
					if (stackTrace.FrameCount > 0)
					{
						MethodBase method = stackTrace.GetFrame(0).GetMethod();
						if (method != null)
						{
							this._source = method.DeclaringType.Assembly.GetName().Name;
						}
					}
				}
				return this._source;
			}
			set
			{
				this._source = value;
			}
		}

		/// <summary>Creates and returns a string representation of the current exception.</summary>
		/// <returns>A string representation of the current exception.</returns>
		public override string ToString()
		{
			return this.ToString(true, true);
		}

		private string ToString(bool needFileLineInfo, bool needMessage)
		{
			string text = needMessage ? this.Message : null;
			string text2;
			if (text == null || text.Length <= 0)
			{
				text2 = this.GetClassName();
			}
			else
			{
				text2 = this.GetClassName() + ": " + text;
			}
			if (this._innerException != null)
			{
				text2 = string.Concat(new string[]
				{
					text2,
					" ---> ",
					this._innerException.ToString(needFileLineInfo, needMessage),
					Environment.NewLine,
					"   ",
					Environment.GetResourceString("--- End of inner exception stack trace ---")
				});
			}
			string stackTrace = this.GetStackTrace(needFileLineInfo);
			if (stackTrace != null)
			{
				text2 = text2 + Environment.NewLine + stackTrace;
			}
			return text2;
		}

		/// <summary>Occurs when an exception is serialized to create an exception state object that contains serialized data about the exception.</summary>
		protected event EventHandler<SafeSerializationEventArgs> SerializeObjectState
		{
			add
			{
				this._safeSerializationManager.SerializeObjectState += value;
			}
			remove
			{
				this._safeSerializationManager.SerializeObjectState -= value;
			}
		}

		/// <summary>When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			string text = this._stackTraceString;
			if (this._stackTrace != null && text == null)
			{
				text = Environment.GetStackTrace(this, true);
			}
			if (this._source == null)
			{
				this._source = this.Source;
			}
			info.AddValue("ClassName", this.GetClassName(), typeof(string));
			info.AddValue("Message", this._message, typeof(string));
			info.AddValue("Data", this._data, typeof(IDictionary));
			info.AddValue("InnerException", this._innerException, typeof(Exception));
			info.AddValue("HelpURL", this._helpURL, typeof(string));
			info.AddValue("StackTraceString", text, typeof(string));
			info.AddValue("RemoteStackTraceString", this._remoteStackTraceString, typeof(string));
			info.AddValue("RemoteStackIndex", this._remoteStackIndex, typeof(int));
			info.AddValue("ExceptionMethod", null);
			info.AddValue("HResult", this.HResult);
			info.AddValue("Source", this._source, typeof(string));
			if (this._safeSerializationManager != null && this._safeSerializationManager.IsActive)
			{
				info.AddValue("SafeSerializationManager", this._safeSerializationManager, typeof(SafeSerializationManager));
				this._safeSerializationManager.CompleteSerialization(this, info, context);
			}
		}

		internal Exception PrepForRemoting()
		{
			string remoteStackTraceString;
			if (this._remoteStackIndex == 0)
			{
				remoteStackTraceString = string.Concat(new string[]
				{
					Environment.NewLine,
					"Server stack trace: ",
					Environment.NewLine,
					this.StackTrace,
					Environment.NewLine,
					Environment.NewLine,
					"Exception rethrown at [",
					this._remoteStackIndex.ToString(),
					"]: ",
					Environment.NewLine
				});
			}
			else
			{
				remoteStackTraceString = string.Concat(new string[]
				{
					this.StackTrace,
					Environment.NewLine,
					Environment.NewLine,
					"Exception rethrown at [",
					this._remoteStackIndex.ToString(),
					"]: ",
					Environment.NewLine
				});
			}
			this._remoteStackTraceString = remoteStackTraceString;
			this._remoteStackIndex++;
			return this;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this._stackTrace = null;
			if (this._safeSerializationManager == null)
			{
				this._safeSerializationManager = new SafeSerializationManager();
				return;
			}
			this._safeSerializationManager.CompleteDeserialization(this);
		}

		internal void InternalPreserveStackTrace()
		{
			string stackTrace = this.StackTrace;
			if (stackTrace != null && stackTrace.Length > 0)
			{
				this._remoteStackTraceString = stackTrace + Environment.NewLine;
			}
			this._stackTrace = null;
			this._stackTraceString = null;
		}

		private string StripFileInfo(string stackTrace, bool isRemoteStackTrace)
		{
			return stackTrace;
		}

		internal string RemoteStackTrace
		{
			get
			{
				return this._remoteStackTraceString;
			}
		}

		[SecuritySafeCritical]
		internal void RestoreExceptionDispatchInfo(ExceptionDispatchInfo exceptionDispatchInfo)
		{
			this.captured_traces = (StackTrace[])exceptionDispatchInfo.BinaryStackTraceArray;
			this._stackTrace = null;
			this._stackTraceString = null;
		}

		/// <summary>Gets or sets HRESULT, a coded numerical value that is assigned to a specific exception.</summary>
		/// <returns>The HRESULT value.</returns>
		public int HResult
		{
			get
			{
				return this._HResult;
			}
			protected set
			{
				this._HResult = value;
			}
		}

		[SecurityCritical]
		internal virtual string InternalToString()
		{
			bool needFileLineInfo = true;
			return this.ToString(needFileLineInfo, true);
		}

		/// <summary>Gets the runtime type of the current instance.</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents the exact runtime type of the current instance.</returns>
		public new Type GetType()
		{
			return base.GetType();
		}

		internal bool IsTransient
		{
			[SecuritySafeCritical]
			get
			{
				return Exception.nIsTransient(this._HResult);
			}
		}

		private static bool nIsTransient(int hr)
		{
			throw new NotImplementedException();
		}

		[SecuritySafeCritical]
		internal static string GetMessageFromNativeResources(Exception.ExceptionMessageKind kind)
		{
			switch (kind)
			{
			case Exception.ExceptionMessageKind.ThreadAbort:
				return "Thread was being aborted.";
			case Exception.ExceptionMessageKind.ThreadInterrupted:
				return "Thread was interrupted from a waiting state.";
			case Exception.ExceptionMessageKind.OutOfMemory:
				return "Insufficient memory to continue the execution of the program.";
			default:
				return "";
			}
		}

		internal void SetMessage(string s)
		{
			this._message = s;
		}

		internal void SetStackTrace(string s)
		{
			this._stackTraceString = s;
		}

		internal Exception FixRemotingException()
		{
			string remoteStackTraceString = string.Format((this._remoteStackIndex == 0) ? "{0}{0}Server stack trace: {0}{1}{0}{0}Exception rethrown at [{2}]: {0}" : "{1}{0}{0}Exception rethrown at [{2}]: {0}", Environment.NewLine, this.StackTrace, this._remoteStackIndex);
			this._remoteStackTraceString = remoteStackTraceString;
			this._remoteStackIndex++;
			this._stackTraceString = null;
			return this;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReportUnhandledException(Exception exception);

		[OptionalField]
		private static object s_EDILock = new object();

		private string _className;

		internal string _message;

		private IDictionary _data;

		private Exception _innerException;

		private string _helpURL;

		private object _stackTrace;

		private string _stackTraceString;

		private string _remoteStackTraceString;

		private int _remoteStackIndex;

		private object _dynamicMethods;

		internal int _HResult;

		private string _source;

		[OptionalField(VersionAdded = 4)]
		private SafeSerializationManager _safeSerializationManager;

		internal StackTrace[] captured_traces;

		private IntPtr[] native_trace_ips;

		private int caught_in_unmanaged;

		private const int _COMPlusExceptionCode = -532462766;

		[Serializable]
		internal class __RestrictedErrorObject
		{
			internal __RestrictedErrorObject(object errorObject)
			{
				this._realErrorObject = errorObject;
			}

			public object RealErrorObject
			{
				get
				{
					return this._realErrorObject;
				}
			}

			[NonSerialized]
			private object _realErrorObject;
		}

		internal enum ExceptionMessageKind
		{
			ThreadAbort = 1,
			ThreadInterrupted,
			OutOfMemory
		}
	}
}
