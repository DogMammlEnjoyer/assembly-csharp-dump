using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Xml;

namespace System.Security
{
	/// <summary>Represents the XML object model for encoding security objects. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[Serializable]
	public sealed class SecurityElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.SecurityElement" /> class with the specified tag.</summary>
		/// <param name="tag">The tag name of an XML element.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="tag" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="tag" /> parameter is invalid in XML.</exception>
		public SecurityElement(string tag) : this(tag, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.SecurityElement" /> class with the specified tag and text.</summary>
		/// <param name="tag">The tag name of the XML element.</param>
		/// <param name="text">The text content within the element.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="tag" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="tag" /> parameter or <paramref name="text" /> parameter is invalid in XML.</exception>
		public SecurityElement(string tag, string text)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (!SecurityElement.IsValidTag(tag))
			{
				throw new ArgumentException(Locale.GetText("Invalid XML string") + ": " + tag);
			}
			this.tag = tag;
			this.Text = text;
		}

		internal SecurityElement(SecurityElement se)
		{
			this.Tag = se.Tag;
			this.Text = se.Text;
			if (se.attributes != null)
			{
				foreach (object obj in se.attributes)
				{
					SecurityElement.SecurityAttribute securityAttribute = (SecurityElement.SecurityAttribute)obj;
					this.AddAttribute(securityAttribute.Name, securityAttribute.Value);
				}
			}
			if (se.children != null)
			{
				foreach (object obj2 in se.children)
				{
					SecurityElement child = (SecurityElement)obj2;
					this.AddChild(child);
				}
			}
		}

		/// <summary>Gets or sets the attributes of an XML element as name/value pairs.</summary>
		/// <returns>The <see cref="T:System.Collections.Hashtable" /> object for the attribute values of the XML element.</returns>
		/// <exception cref="T:System.InvalidCastException">The name or value of the <see cref="T:System.Collections.Hashtable" /> object is invalid.</exception>
		/// <exception cref="T:System.ArgumentException">The name is not a valid XML attribute name.</exception>
		public Hashtable Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					return null;
				}
				Hashtable hashtable = new Hashtable(this.attributes.Count);
				foreach (object obj in this.attributes)
				{
					SecurityElement.SecurityAttribute securityAttribute = (SecurityElement.SecurityAttribute)obj;
					hashtable.Add(securityAttribute.Name, securityAttribute.Value);
				}
				return hashtable;
			}
			set
			{
				if (value == null || value.Count == 0)
				{
					this.attributes.Clear();
					return;
				}
				if (this.attributes == null)
				{
					this.attributes = new ArrayList();
				}
				else
				{
					this.attributes.Clear();
				}
				IDictionaryEnumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					this.attributes.Add(new SecurityElement.SecurityAttribute((string)enumerator.Key, (string)enumerator.Value));
				}
			}
		}

		/// <summary>Gets or sets the array of child elements of the XML element.</summary>
		/// <returns>The ordered child elements of the XML element as security elements.</returns>
		/// <exception cref="T:System.ArgumentException">A child of the XML parent node is <see langword="null" />.</exception>
		public ArrayList Children
		{
			get
			{
				return this.children;
			}
			set
			{
				if (value != null)
				{
					using (IEnumerator enumerator = value.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current == null)
							{
								throw new ArgumentNullException();
							}
						}
					}
				}
				this.children = value;
			}
		}

		/// <summary>Gets or sets the tag name of an XML element.</summary>
		/// <returns>The tag name of an XML element.</returns>
		/// <exception cref="T:System.ArgumentNullException">The tag is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The tag is not valid in XML.</exception>
		public string Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Tag");
				}
				if (!SecurityElement.IsValidTag(value))
				{
					throw new ArgumentException(Locale.GetText("Invalid XML string") + ": " + value);
				}
				this.tag = value;
			}
		}

		/// <summary>Gets or sets the text within an XML element.</summary>
		/// <returns>The value of the text within an XML element.</returns>
		/// <exception cref="T:System.ArgumentException">The text is not valid in XML.</exception>
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				if (value != null && !SecurityElement.IsValidText(value))
				{
					throw new ArgumentException(Locale.GetText("Invalid XML string") + ": " + value);
				}
				this.text = SecurityElement.Unescape(value);
			}
		}

		/// <summary>Adds a name/value attribute to an XML element.</summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter or <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="name" /> parameter or <paramref name="value" /> parameter is invalid in XML.  
		///  -or-  
		///  An attribute with the name specified by the <paramref name="name" /> parameter already exists.</exception>
		public void AddAttribute(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.GetAttribute(name) != null)
			{
				throw new ArgumentException(Locale.GetText("Duplicate attribute : " + name));
			}
			if (this.attributes == null)
			{
				this.attributes = new ArrayList();
			}
			this.attributes.Add(new SecurityElement.SecurityAttribute(name, value));
		}

		/// <summary>Adds a child element to the XML element.</summary>
		/// <param name="child">The child element to add.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="child" /> parameter is <see langword="null" />.</exception>
		public void AddChild(SecurityElement child)
		{
			if (child == null)
			{
				throw new ArgumentNullException("child");
			}
			if (this.children == null)
			{
				this.children = new ArrayList();
			}
			this.children.Add(child);
		}

		/// <summary>Finds an attribute by name in an XML element.</summary>
		/// <param name="name">The name of the attribute for which to search.</param>
		/// <returns>The value associated with the named attribute, or <see langword="null" /> if no attribute with <paramref name="name" /> exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		public string Attribute(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			SecurityElement.SecurityAttribute attribute = this.GetAttribute(name);
			if (attribute != null)
			{
				return attribute.Value;
			}
			return null;
		}

		/// <summary>Creates and returns an identical copy of the current <see cref="T:System.Security.SecurityElement" /> object.</summary>
		/// <returns>A copy of the current <see cref="T:System.Security.SecurityElement" /> object.</returns>
		[ComVisible(false)]
		public SecurityElement Copy()
		{
			return new SecurityElement(this);
		}

		/// <summary>Compares two XML element objects for equality.</summary>
		/// <param name="other">An XML element object to which to compare the current XML element object.</param>
		/// <returns>
		///   <see langword="true" /> if the tag, attribute names and values, child elements, and text fields in the current XML element are identical to their counterparts in the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool Equal(SecurityElement other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (this.text != other.text)
			{
				return false;
			}
			if (this.tag != other.tag)
			{
				return false;
			}
			if (this.attributes == null && other.attributes != null && other.attributes.Count != 0)
			{
				return false;
			}
			if (other.attributes == null && this.attributes != null && this.attributes.Count != 0)
			{
				return false;
			}
			if (this.attributes != null && other.attributes != null)
			{
				if (this.attributes.Count != other.attributes.Count)
				{
					return false;
				}
				foreach (object obj in this.attributes)
				{
					SecurityElement.SecurityAttribute securityAttribute = (SecurityElement.SecurityAttribute)obj;
					SecurityElement.SecurityAttribute attribute = other.GetAttribute(securityAttribute.Name);
					if (attribute == null || securityAttribute.Value != attribute.Value)
					{
						return false;
					}
				}
			}
			if (this.children == null && other.children != null && other.children.Count != 0)
			{
				return false;
			}
			if (other.children == null && this.children != null && this.children.Count != 0)
			{
				return false;
			}
			if (this.children != null && other.children != null)
			{
				if (this.children.Count != other.children.Count)
				{
					return false;
				}
				for (int i = 0; i < this.children.Count; i++)
				{
					if (!((SecurityElement)this.children[i]).Equal((SecurityElement)other.children[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>Replaces invalid XML characters in a string with their valid XML equivalent.</summary>
		/// <param name="str">The string within which to escape invalid characters.</param>
		/// <returns>The input string with invalid characters replaced.</returns>
		public static string Escape(string str)
		{
			if (str == null)
			{
				return null;
			}
			if (str.IndexOfAny(SecurityElement.invalid_chars) == -1)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = str.Length;
			int i = 0;
			while (i < length)
			{
				char c = str[i];
				if (c <= '&')
				{
					if (c != '"')
					{
						if (c != '&')
						{
							goto IL_96;
						}
						stringBuilder.Append("&amp;");
					}
					else
					{
						stringBuilder.Append("&quot;");
					}
				}
				else if (c != '\'')
				{
					if (c != '<')
					{
						if (c != '>')
						{
							goto IL_96;
						}
						stringBuilder.Append("&gt;");
					}
					else
					{
						stringBuilder.Append("&lt;");
					}
				}
				else
				{
					stringBuilder.Append("&apos;");
				}
				IL_9E:
				i++;
				continue;
				IL_96:
				stringBuilder.Append(c);
				goto IL_9E;
			}
			return stringBuilder.ToString();
		}

		private static string Unescape(string str)
		{
			if (str == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(str);
			stringBuilder.Replace("&lt;", "<");
			stringBuilder.Replace("&gt;", ">");
			stringBuilder.Replace("&amp;", "&");
			stringBuilder.Replace("&quot;", "\"");
			stringBuilder.Replace("&apos;", "'");
			return stringBuilder.ToString();
		}

		/// <summary>Creates a security element from an XML-encoded string.</summary>
		/// <param name="xml">The XML-encoded string from which to create the security element.</param>
		/// <returns>A <see cref="T:System.Security.SecurityElement" /> created from the XML.</returns>
		/// <exception cref="T:System.Security.XmlSyntaxException">
		///   <paramref name="xml" /> contains one or more single quotation mark characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="xml" /> is <see langword="null" />.</exception>
		public static SecurityElement FromString(string xml)
		{
			if (xml == null)
			{
				throw new ArgumentNullException("xml");
			}
			if (xml.Length == 0)
			{
				throw new XmlSyntaxException(Locale.GetText("Empty string."));
			}
			SecurityElement result;
			try
			{
				SecurityParser securityParser = new SecurityParser();
				securityParser.LoadXml(xml);
				result = securityParser.ToXml();
			}
			catch (Exception inner)
			{
				throw new XmlSyntaxException(Locale.GetText("Invalid XML."), inner);
			}
			return result;
		}

		/// <summary>Determines whether a string is a valid attribute name.</summary>
		/// <param name="name">The attribute name to test for validity.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="name" /> parameter is a valid XML attribute name; otherwise, <see langword="false" />.</returns>
		public static bool IsValidAttributeName(string name)
		{
			return name != null && name.IndexOfAny(SecurityElement.invalid_attr_name_chars) == -1;
		}

		/// <summary>Determines whether a string is a valid attribute value.</summary>
		/// <param name="value">The attribute value to test for validity.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="value" /> parameter is a valid XML attribute value; otherwise, <see langword="false" />.</returns>
		public static bool IsValidAttributeValue(string value)
		{
			return value != null && value.IndexOfAny(SecurityElement.invalid_attr_value_chars) == -1;
		}

		/// <summary>Determines whether a string is a valid tag.</summary>
		/// <param name="tag">The tag to test for validity.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="tag" /> parameter is a valid XML tag; otherwise, <see langword="false" />.</returns>
		public static bool IsValidTag(string tag)
		{
			return tag != null && tag.IndexOfAny(SecurityElement.invalid_tag_chars) == -1;
		}

		/// <summary>Determines whether a string is valid as text within an XML element.</summary>
		/// <param name="text">The text to test for validity.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="text" /> parameter is a valid XML text element; otherwise, <see langword="false" />.</returns>
		public static bool IsValidText(string text)
		{
			return text != null && text.IndexOfAny(SecurityElement.invalid_text_chars) == -1;
		}

		/// <summary>Finds a child by its tag name.</summary>
		/// <param name="tag">The tag for which to search in child elements.</param>
		/// <returns>The first child XML element with the specified tag value, or <see langword="null" /> if no child element with <paramref name="tag" /> exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="tag" /> parameter is <see langword="null" />.</exception>
		public SecurityElement SearchForChildByTag(string tag)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (this.children == null)
			{
				return null;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				SecurityElement securityElement = (SecurityElement)this.children[i];
				if (securityElement.tag == tag)
				{
					return securityElement;
				}
			}
			return null;
		}

		/// <summary>Finds a child by its tag name and returns the contained text.</summary>
		/// <param name="tag">The tag for which to search in child elements.</param>
		/// <returns>The text contents of the first child element with the specified tag value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="tag" /> is <see langword="null" />.</exception>
		public string SearchForTextOfTag(string tag)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			if (this.tag == tag)
			{
				return this.text;
			}
			if (this.children == null)
			{
				return null;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				string text = ((SecurityElement)this.children[i]).SearchForTextOfTag(tag);
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}

		/// <summary>Produces a string representation of an XML element and its constituent attributes, child elements, and text.</summary>
		/// <returns>The XML element and its contents.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ToXml(ref stringBuilder, 0);
			return stringBuilder.ToString();
		}

		private void ToXml(ref StringBuilder s, int level)
		{
			s.Append("<");
			s.Append(this.tag);
			if (this.attributes != null)
			{
				s.Append(" ");
				for (int i = 0; i < this.attributes.Count; i++)
				{
					SecurityElement.SecurityAttribute securityAttribute = (SecurityElement.SecurityAttribute)this.attributes[i];
					s.Append(securityAttribute.Name).Append("=\"").Append(SecurityElement.Escape(securityAttribute.Value)).Append("\"");
					if (i != this.attributes.Count - 1)
					{
						s.Append(Environment.NewLine);
					}
				}
			}
			if ((this.text == null || this.text == string.Empty) && (this.children == null || this.children.Count == 0))
			{
				s.Append("/>").Append(Environment.NewLine);
				return;
			}
			s.Append(">").Append(SecurityElement.Escape(this.text));
			if (this.children != null)
			{
				s.Append(Environment.NewLine);
				foreach (object obj in this.children)
				{
					((SecurityElement)obj).ToXml(ref s, level + 1);
				}
			}
			s.Append("</").Append(this.tag).Append(">").Append(Environment.NewLine);
		}

		internal SecurityElement.SecurityAttribute GetAttribute(string name)
		{
			if (this.attributes != null)
			{
				foreach (object obj in this.attributes)
				{
					SecurityElement.SecurityAttribute securityAttribute = (SecurityElement.SecurityAttribute)obj;
					if (securityAttribute.Name == name)
					{
						return securityAttribute;
					}
				}
			}
			return null;
		}

		internal string m_strTag
		{
			get
			{
				return this.tag;
			}
		}

		internal string m_strText
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		internal ArrayList m_lAttributes
		{
			get
			{
				return this.attributes;
			}
		}

		internal ArrayList InternalChildren
		{
			get
			{
				return this.children;
			}
		}

		internal string SearchForTextOfLocalName(string strLocalName)
		{
			if (strLocalName == null)
			{
				throw new ArgumentNullException("strLocalName");
			}
			if (this.tag == null)
			{
				return null;
			}
			if (this.tag.Equals(strLocalName) || this.tag.EndsWith(":" + strLocalName, StringComparison.Ordinal))
			{
				return SecurityElement.Unescape(this.text);
			}
			if (this.children == null)
			{
				return null;
			}
			foreach (object obj in this.children)
			{
				string text = ((SecurityElement)obj).SearchForTextOfLocalName(strLocalName);
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}

		private string text;

		private string tag;

		private ArrayList attributes;

		private ArrayList children;

		private static readonly char[] invalid_tag_chars = new char[]
		{
			' ',
			'<',
			'>'
		};

		private static readonly char[] invalid_text_chars = new char[]
		{
			'<',
			'>'
		};

		private static readonly char[] invalid_attr_name_chars = new char[]
		{
			' ',
			'<',
			'>'
		};

		private static readonly char[] invalid_attr_value_chars = new char[]
		{
			'"',
			'<',
			'>'
		};

		private static readonly char[] invalid_chars = new char[]
		{
			'<',
			'>',
			'"',
			'\'',
			'&'
		};

		internal class SecurityAttribute
		{
			public SecurityAttribute(string name, string value)
			{
				if (!SecurityElement.IsValidAttributeName(name))
				{
					throw new ArgumentException(Locale.GetText("Invalid XML attribute name") + ": " + name);
				}
				if (!SecurityElement.IsValidAttributeValue(value))
				{
					throw new ArgumentException(Locale.GetText("Invalid XML attribute value") + ": " + value);
				}
				this._name = name;
				this._value = SecurityElement.Unescape(value);
			}

			public string Name
			{
				get
				{
					return this._name;
				}
			}

			public string Value
			{
				get
				{
					return this._value;
				}
			}

			private string _name;

			private string _value;
		}
	}
}
