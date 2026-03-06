using System;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.XPath
{
	/// <summary>Provides the exception thrown when an error occurs while processing an XPath expression. </summary>
	[Serializable]
	public class XPathException : SystemException
	{
		/// <summary>Uses the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> objects to initialize a new instance of the <see cref="T:System.Xml.XPath.XPathException" /> class.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains all the properties of an <see cref="T:System.Xml.XPath.XPathException" />. </param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> object. </param>
		protected XPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.res = (string)info.GetValue("res", typeof(string));
			this.args = (string[])info.GetValue("args", typeof(string[]));
			string text = null;
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "version")
				{
					text = (string)serializationEntry.Value;
				}
			}
			if (text == null)
			{
				this.message = XPathException.CreateMessage(this.res, this.args);
				return;
			}
			this.message = null;
		}

		/// <summary>Streams all the <see cref="T:System.Xml.XPath.XPathException" /> properties into the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> class for the specified <see cref="T:System.Runtime.Serialization.StreamingContext" />.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> object.</param>
		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("res", this.res);
			info.AddValue("args", this.args);
			info.AddValue("version", "2.0");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathException" /> class.</summary>
		public XPathException() : this(string.Empty, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathException" /> class with the specified exception message.</summary>
		/// <param name="message">The description of the error condition.</param>
		public XPathException(string message) : this(message, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathException" /> class using the specified exception message and <see cref="T:System.Exception" /> object.</summary>
		/// <param name="message">The description of the error condition. </param>
		/// <param name="innerException">The <see cref="T:System.Exception" /> that threw the <see cref="T:System.Xml.XPath.XPathException" />, if any. This value can be <see langword="null" />. </param>
		public XPathException(string message, Exception innerException) : this("{0}", new string[]
		{
			message
		}, innerException)
		{
		}

		internal static XPathException Create(string res)
		{
			return new XPathException(res, null);
		}

		internal static XPathException Create(string res, string arg)
		{
			return new XPathException(res, new string[]
			{
				arg
			});
		}

		internal static XPathException Create(string res, string arg, string arg2)
		{
			return new XPathException(res, new string[]
			{
				arg,
				arg2
			});
		}

		internal static XPathException Create(string res, string arg, Exception innerException)
		{
			return new XPathException(res, new string[]
			{
				arg
			}, innerException);
		}

		private XPathException(string res, string[] args) : this(res, args, null)
		{
		}

		private XPathException(string res, string[] args, Exception inner) : base(XPathException.CreateMessage(res, args), inner)
		{
			base.HResult = -2146231997;
			this.res = res;
			this.args = args;
		}

		private static string CreateMessage(string res, string[] args)
		{
			string result;
			try
			{
				string text = Res.GetString(res, args);
				if (text == null)
				{
					text = "UNKNOWN(" + res + ")";
				}
				result = text;
			}
			catch (MissingManifestResourceException)
			{
				result = "UNKNOWN(" + res + ")";
			}
			return result;
		}

		/// <summary>Gets the description of the error condition for this exception.</summary>
		/// <returns>The <see langword="string" /> description of the error condition for this exception.</returns>
		public override string Message
		{
			get
			{
				if (this.message != null)
				{
					return this.message;
				}
				return base.Message;
			}
		}

		private string res;

		private string[] args;

		private string message;
	}
}
