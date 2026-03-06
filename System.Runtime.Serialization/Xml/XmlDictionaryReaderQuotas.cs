using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Xml
{
	/// <summary>Contains configurable quota values for XmlDictionaryReaders.</summary>
	public sealed class XmlDictionaryReaderQuotas
	{
		/// <summary>Creates a new instance of this class.</summary>
		public XmlDictionaryReaderQuotas()
		{
			XmlDictionaryReaderQuotas.defaultQuota.CopyTo(this);
		}

		private XmlDictionaryReaderQuotas(int maxDepth, int maxStringContentLength, int maxArrayLength, int maxBytesPerRead, int maxNameTableCharCount, XmlDictionaryReaderQuotaTypes modifiedQuotas)
		{
			this.maxDepth = maxDepth;
			this.maxStringContentLength = maxStringContentLength;
			this.maxArrayLength = maxArrayLength;
			this.maxBytesPerRead = maxBytesPerRead;
			this.maxNameTableCharCount = maxNameTableCharCount;
			this.modifiedQuotas = modifiedQuotas;
			this.MakeReadOnly();
		}

		/// <summary>Gets an instance of this class with all properties set to maximum values.</summary>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> with properties set to <see cref="F:System.Int32.MaxValue" />.</returns>
		public static XmlDictionaryReaderQuotas Max
		{
			get
			{
				return XmlDictionaryReaderQuotas.maxQuota;
			}
		}

		/// <summary>Sets the properties on a passed-in quotas instance, based on the values in this instance.</summary>
		/// <param name="quotas">The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> instance to which to copy values.</param>
		/// <exception cref="T:System.InvalidOperationException">Trying to <see langword="set" /> the value, but quota values are read-only for the passed in instance.</exception>
		/// <exception cref="T:System.ArgumentNullException">Passed in target <paramref name="quotas" /> is <see langword="null" />.</exception>
		public void CopyTo(XmlDictionaryReaderQuotas quotas)
		{
			if (quotas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("quotas"));
			}
			if (quotas.readOnly)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("Cannot copy XmlDictionaryReaderQuotas. Target is readonly.")));
			}
			this.InternalCopyTo(quotas);
		}

		internal void InternalCopyTo(XmlDictionaryReaderQuotas quotas)
		{
			quotas.maxStringContentLength = this.maxStringContentLength;
			quotas.maxArrayLength = this.maxArrayLength;
			quotas.maxDepth = this.maxDepth;
			quotas.maxNameTableCharCount = this.maxNameTableCharCount;
			quotas.maxBytesPerRead = this.maxBytesPerRead;
			quotas.modifiedQuotas = this.modifiedQuotas;
		}

		/// <summary>Gets or sets the maximum string length returned by the reader.</summary>
		/// <returns>The maximum string length returned by the reader. The default is 8192.</returns>
		/// <exception cref="T:System.InvalidOperationException">Trying to <see langword="set" /> the value, but quota values are read-only for this instance.</exception>
		/// <exception cref="T:System.ArgumentException">Trying to <see langword="set" /> the value to less than zero.</exception>
		[DefaultValue(8192)]
		public int MaxStringContentLength
		{
			get
			{
				return this.maxStringContentLength;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[]
					{
						"MaxStringContentLength"
					})));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxStringContentLength = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxStringContentLength;
			}
		}

		/// <summary>Gets or sets the maximum allowed array length.</summary>
		/// <returns>The maximum allowed array length. The default is 16384.</returns>
		/// <exception cref="T:System.InvalidOperationException">Trying to <see langword="set" /> the value, but quota values are read-only for this instance.</exception>
		/// <exception cref="T:System.ArgumentException">Trying to <see langword="set" /> the value to less than zero.</exception>
		[DefaultValue(16384)]
		public int MaxArrayLength
		{
			get
			{
				return this.maxArrayLength;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[]
					{
						"MaxArrayLength"
					})));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxArrayLength = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxArrayLength;
			}
		}

		/// <summary>Gets or sets the maximum allowed bytes returned for each read.</summary>
		/// <returns>The maximum allowed bytes returned for each read. The default is 4096.</returns>
		/// <exception cref="T:System.InvalidOperationException">Trying to <see langword="set" /> the value, but quota values are read-only for this instance.</exception>
		/// <exception cref="T:System.ArgumentException">Trying to <see langword="set" /> the value to less than zero.</exception>
		[DefaultValue(4096)]
		public int MaxBytesPerRead
		{
			get
			{
				return this.maxBytesPerRead;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[]
					{
						"MaxBytesPerRead"
					})));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxBytesPerRead = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxBytesPerRead;
			}
		}

		/// <summary>Gets or sets the maximum nested node depth.</summary>
		/// <returns>The maximum nested node depth. The default is 32;</returns>
		/// <exception cref="T:System.InvalidOperationException">Trying to <see langword="set" /> the value and quota values are read-only for this instance.</exception>
		/// <exception cref="T:System.ArgumentException">Trying to <see langword="set" /> the value is less than zero.</exception>
		[DefaultValue(32)]
		public int MaxDepth
		{
			get
			{
				return this.maxDepth;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[]
					{
						"MaxDepth"
					})));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxDepth = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxDepth;
			}
		}

		/// <summary>Gets or sets the maximum characters allowed in a table name.</summary>
		/// <returns>The maximum characters allowed in a table name. The default is 16384.</returns>
		/// <exception cref="T:System.InvalidOperationException">Trying to <see langword="set" /> the value, but quota values are read-only for this instance.</exception>
		/// <exception cref="T:System.ArgumentException">Trying to <see langword="set" /> the value to less than zero.</exception>
		[DefaultValue(16384)]
		public int MaxNameTableCharCount
		{
			get
			{
				return this.maxNameTableCharCount;
			}
			set
			{
				if (this.readOnly)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("The '{0}' quota is readonly.", new object[]
					{
						"MaxNameTableCharCount"
					})));
				}
				if (value <= 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("Quota must be a positive value."), "value"));
				}
				this.maxNameTableCharCount = value;
				this.modifiedQuotas |= XmlDictionaryReaderQuotaTypes.MaxNameTableCharCount;
			}
		}

		/// <summary>Gets the modified quotas for the <see cref="T:System.Xml.XmlDictionaryReaderQuotas" />.</summary>
		/// <returns>The modified quotas for the <see cref="T:System.Xml.XmlDictionaryReaderQuotas" />.</returns>
		public XmlDictionaryReaderQuotaTypes ModifiedQuotas
		{
			get
			{
				return this.modifiedQuotas;
			}
		}

		internal void MakeReadOnly()
		{
			this.readOnly = true;
		}

		private bool readOnly;

		private int maxStringContentLength;

		private int maxArrayLength;

		private int maxDepth;

		private int maxNameTableCharCount;

		private int maxBytesPerRead;

		private XmlDictionaryReaderQuotaTypes modifiedQuotas;

		private const int DefaultMaxDepth = 32;

		private const int DefaultMaxStringContentLength = 8192;

		private const int DefaultMaxArrayLength = 16384;

		private const int DefaultMaxBytesPerRead = 4096;

		private const int DefaultMaxNameTableCharCount = 16384;

		private static XmlDictionaryReaderQuotas defaultQuota = new XmlDictionaryReaderQuotas(32, 8192, 16384, 4096, 16384, (XmlDictionaryReaderQuotaTypes)0);

		private static XmlDictionaryReaderQuotas maxQuota = new XmlDictionaryReaderQuotas(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, XmlDictionaryReaderQuotaTypes.MaxDepth | XmlDictionaryReaderQuotaTypes.MaxStringContentLength | XmlDictionaryReaderQuotaTypes.MaxArrayLength | XmlDictionaryReaderQuotaTypes.MaxBytesPerRead | XmlDictionaryReaderQuotaTypes.MaxNameTableCharCount);
	}
}
