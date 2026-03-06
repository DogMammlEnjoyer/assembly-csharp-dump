using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML attribute.</summary>
	public class XAttribute : XObject
	{
		/// <summary>Gets an empty collection of attributes.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XAttribute" /> containing an empty collection.</returns>
		public static IEnumerable<XAttribute> EmptySequence
		{
			get
			{
				return Array.Empty<XAttribute>();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XAttribute" /> class from the specified name and value.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> of the attribute.</param>
		/// <param name="value">An <see cref="T:System.Object" /> containing the value of the attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> or <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public XAttribute(XName name, object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string stringValue = XContainer.GetStringValue(value);
			XAttribute.ValidateAttribute(name, stringValue);
			this.name = name;
			this.value = stringValue;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XAttribute" /> class from another <see cref="T:System.Xml.Linq.XAttribute" /> object.</summary>
		/// <param name="other">An <see cref="T:System.Xml.Linq.XAttribute" /> object to copy from.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="other" /> parameter is <see langword="null" />.</exception>
		public XAttribute(XAttribute other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			this.value = other.value;
		}

		/// <summary>Determines if this attribute is a namespace declaration.</summary>
		/// <returns>
		///   <see langword="true" /> if this attribute is a namespace declaration; otherwise <see langword="false" />.</returns>
		public bool IsNamespaceDeclaration
		{
			get
			{
				string namespaceName = this.name.NamespaceName;
				if (namespaceName.Length == 0)
				{
					return this.name.LocalName == "xmlns";
				}
				return namespaceName == "http://www.w3.org/2000/xmlns/";
			}
		}

		/// <summary>Gets the expanded name of this attribute.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XName" /> containing the name of this attribute.</returns>
		public XName Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the next attribute of the parent element.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XAttribute" /> containing the next attribute of the parent element.</returns>
		public XAttribute NextAttribute
		{
			get
			{
				if (this.parent == null || ((XElement)this.parent).lastAttr == this)
				{
					return null;
				}
				return this.next;
			}
		}

		/// <summary>Gets the node type for this node.</summary>
		/// <returns>The node type. For <see cref="T:System.Xml.Linq.XAttribute" /> objects, this value is <see cref="F:System.Xml.XmlNodeType.Attribute" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Attribute;
			}
		}

		/// <summary>Gets the previous attribute of the parent element.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XAttribute" /> containing the previous attribute of the parent element.</returns>
		public XAttribute PreviousAttribute
		{
			get
			{
				if (this.parent == null)
				{
					return null;
				}
				XAttribute lastAttr = ((XElement)this.parent).lastAttr;
				while (lastAttr.next != this)
				{
					lastAttr = lastAttr.next;
				}
				if (lastAttr == ((XElement)this.parent).lastAttr)
				{
					return null;
				}
				return lastAttr;
			}
		}

		/// <summary>Gets or sets the value of this attribute.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the value of this attribute.</returns>
		/// <exception cref="T:System.ArgumentNullException">When setting, the <paramref name="value" /> is <see langword="null" />.</exception>
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				XAttribute.ValidateAttribute(this.name, value);
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.value = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		/// <summary>Removes this attribute from its parent element.</summary>
		/// <exception cref="T:System.InvalidOperationException">The parent element is <see langword="null" />.</exception>
		public void Remove()
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			((XElement)this.parent).RemoveAttribute(this);
		}

		/// <summary>Sets the value of this attribute.</summary>
		/// <param name="value">The value to assign to this attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="value" /> is an <see cref="T:System.Xml.Linq.XObject" />.</exception>
		public void SetValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Value = XContainer.GetStringValue(value);
		}

		/// <summary>Converts the current <see cref="T:System.Xml.Linq.XAttribute" /> object to a string representation.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the XML text representation of an attribute and its value.</returns>
		public override string ToString()
		{
			string result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					ConformanceLevel = ConformanceLevel.Fragment
				}))
				{
					xmlWriter.WriteAttributeString(this.GetPrefixOfNamespace(this.name.Namespace), this.name.LocalName, this.name.NamespaceName, this.value);
				}
				result = stringWriter.ToString().Trim();
			}
			return result;
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.String" />.</param>
		/// <returns>A <see cref="T:System.String" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		[CLSCompliant(false)]
		public static explicit operator string(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return attribute.value;
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Boolean" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Boolean" />.</param>
		/// <returns>A <see cref="T:System.Boolean" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Boolean" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator bool(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToBoolean(attribute.value.ToLowerInvariant());
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Boolean" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator bool?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new bool?(XmlConvert.ToBoolean(attribute.value.ToLowerInvariant()));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to an <see cref="T:System.Int32" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Int32" />.</param>
		/// <returns>A <see cref="T:System.Int32" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Int32" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator int(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToInt32(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		[CLSCompliant(false)]
		public static explicit operator int?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new int?(XmlConvert.ToInt32(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.UInt32" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.UInt32" />.</param>
		/// <returns>A <see cref="T:System.UInt32" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.UInt32" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator uint(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToUInt32(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.UInt32" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator uint?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new uint?(XmlConvert.ToUInt32(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to an <see cref="T:System.Int64" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Int64" />.</param>
		/// <returns>A <see cref="T:System.Int64" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Int64" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator long(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToInt64(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Int64" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator long?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new long?(XmlConvert.ToInt64(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.UInt64" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.UInt64" />.</param>
		/// <returns>A <see cref="T:System.UInt64" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.UInt64" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator ulong(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToUInt64(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.UInt64" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator ulong?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new ulong?(XmlConvert.ToUInt64(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Single" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Single" />.</param>
		/// <returns>A <see cref="T:System.Single" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Single" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator float(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToSingle(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Single" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator float?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new float?(XmlConvert.ToSingle(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Double" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Double" />.</param>
		/// <returns>A <see cref="T:System.Double" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Double" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator double(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToDouble(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Double" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator double?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new double?(XmlConvert.ToDouble(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Decimal" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Decimal" />.</param>
		/// <returns>A <see cref="T:System.Decimal" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Decimal" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator decimal(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToDecimal(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Decimal" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator decimal?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new decimal?(XmlConvert.ToDecimal(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.DateTime" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.DateTime" />.</param>
		/// <returns>A <see cref="T:System.DateTime" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.DateTime" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTime(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return DateTime.Parse(attribute.value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.DateTime" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTime?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new DateTime?(DateTime.Parse(attribute.value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.DateTimeOffset" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.DateTimeOffset" />.</param>
		/// <returns>A <see cref="T:System.DateTimeOffset" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.DateTimeOffset" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTimeOffset(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToDateTimeOffset(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.DateTimeOffset" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTimeOffset?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new DateTimeOffset?(XmlConvert.ToDateTimeOffset(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.TimeSpan" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.TimeSpan" />.</param>
		/// <returns>A <see cref="T:System.TimeSpan" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.TimeSpan" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator TimeSpan(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToTimeSpan(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.TimeSpan" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator TimeSpan?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new TimeSpan?(XmlConvert.ToTimeSpan(attribute.value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Guid" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to <see cref="T:System.Guid" />.</param>
		/// <returns>A <see cref="T:System.Guid" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Guid" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="attribute" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator Guid(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToGuid(attribute.value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" />.</summary>
		/// <param name="attribute">The <see cref="T:System.Xml.Linq.XAttribute" /> to cast to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" /> that contains the content of this <see cref="T:System.Xml.Linq.XAttribute" />.</returns>
		/// <exception cref="T:System.FormatException">The attribute does not contain a valid <see cref="T:System.Guid" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator Guid?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return new Guid?(XmlConvert.ToGuid(attribute.value));
		}

		internal int GetDeepHashCode()
		{
			return this.name.GetHashCode() ^ this.value.GetHashCode();
		}

		internal string GetPrefixOfNamespace(XNamespace ns)
		{
			string namespaceName = ns.NamespaceName;
			if (namespaceName.Length == 0)
			{
				return string.Empty;
			}
			if (this.parent != null)
			{
				return ((XElement)this.parent).GetPrefixOfNamespace(ns);
			}
			if (namespaceName == "http://www.w3.org/XML/1998/namespace")
			{
				return "xml";
			}
			if (namespaceName == "http://www.w3.org/2000/xmlns/")
			{
				return "xmlns";
			}
			return null;
		}

		private static void ValidateAttribute(XName name, string value)
		{
			string namespaceName = name.NamespaceName;
			if (namespaceName == "http://www.w3.org/2000/xmlns/")
			{
				if (value.Length == 0)
				{
					throw new ArgumentException(SR.Format("The prefix '{0}' cannot be bound to the empty namespace name.", name.LocalName));
				}
				if (value == "http://www.w3.org/XML/1998/namespace")
				{
					if (name.LocalName != "xml")
					{
						throw new ArgumentException("The prefix 'xml' is bound to the namespace name 'http://www.w3.org/XML/1998/namespace'. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.");
					}
				}
				else
				{
					if (value == "http://www.w3.org/2000/xmlns/")
					{
						throw new ArgumentException("The prefix 'xmlns' is bound to the namespace name 'http://www.w3.org/2000/xmlns/'. It must not be declared. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.");
					}
					string localName = name.LocalName;
					if (localName == "xml")
					{
						throw new ArgumentException("The prefix 'xml' is bound to the namespace name 'http://www.w3.org/XML/1998/namespace'. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.");
					}
					if (localName == "xmlns")
					{
						throw new ArgumentException("The prefix 'xmlns' is bound to the namespace name 'http://www.w3.org/2000/xmlns/'. It must not be declared. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.");
					}
				}
			}
			else if (namespaceName.Length == 0 && name.LocalName == "xmlns")
			{
				if (value == "http://www.w3.org/XML/1998/namespace")
				{
					throw new ArgumentException("The prefix 'xml' is bound to the namespace name 'http://www.w3.org/XML/1998/namespace'. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.");
				}
				if (value == "http://www.w3.org/2000/xmlns/")
				{
					throw new ArgumentException("The prefix 'xmlns' is bound to the namespace name 'http://www.w3.org/2000/xmlns/'. It must not be declared. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.");
				}
			}
		}

		internal XAttribute next;

		internal XName name;

		internal string value;
	}
}
