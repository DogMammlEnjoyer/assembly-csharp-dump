using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Xml
{
	/// <summary>Implements a dictionary used to optimize Windows Communication Foundation (WCF)'s XML reader/writer implementations.</summary>
	public class XmlDictionary : IXmlDictionary
	{
		/// <summary>Gets a <see langword="static" /> empty <see cref="T:System.Xml.IXmlDictionary" />.</summary>
		/// <returns>A <see langword="static" /> empty <see cref="T:System.Xml.IXmlDictionary" />.</returns>
		public static IXmlDictionary Empty
		{
			get
			{
				if (XmlDictionary.empty == null)
				{
					XmlDictionary.empty = new XmlDictionary.EmptyDictionary();
				}
				return XmlDictionary.empty;
			}
		}

		/// <summary>Creates an empty <see cref="T:System.Xml.XmlDictionary" />.</summary>
		public XmlDictionary()
		{
			this.lookup = new Dictionary<string, XmlDictionaryString>();
			this.strings = null;
			this.nextId = 0;
		}

		/// <summary>Creates a <see cref="T:System.Xml.XmlDictionary" /> with an initial capacity.</summary>
		/// <param name="capacity">The initial size of the dictionary.</param>
		public XmlDictionary(int capacity)
		{
			this.lookup = new Dictionary<string, XmlDictionaryString>(capacity);
			this.strings = new XmlDictionaryString[capacity];
			this.nextId = 0;
		}

		/// <summary>Adds a string to the <see cref="T:System.Xml.XmlDictionary" />.</summary>
		/// <param name="value">String to add to the dictionary.</param>
		/// <returns>The <see cref="T:System.Xml.XmlDictionaryString" /> that was added.</returns>
		public virtual XmlDictionaryString Add(string value)
		{
			XmlDictionaryString xmlDictionaryString;
			if (!this.lookup.TryGetValue(value, out xmlDictionaryString))
			{
				if (this.strings == null)
				{
					this.strings = new XmlDictionaryString[4];
				}
				else if (this.nextId == this.strings.Length)
				{
					int num = this.nextId * 2;
					if (num == 0)
					{
						num = 4;
					}
					Array.Resize<XmlDictionaryString>(ref this.strings, num);
				}
				xmlDictionaryString = new XmlDictionaryString(this, value, this.nextId);
				this.strings[this.nextId] = xmlDictionaryString;
				this.lookup.Add(value, xmlDictionaryString);
				this.nextId++;
			}
			return xmlDictionaryString;
		}

		/// <summary>Checks the dictionary for a specified string value.</summary>
		/// <param name="value">String value being checked for.</param>
		/// <param name="result">The corresponding <see cref="T:System.Xml.XmlDictionaryString" />, if found; otherwise <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if value is in the dictionary; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public virtual bool TryLookup(string value, out XmlDictionaryString result)
		{
			return this.lookup.TryGetValue(value, out result);
		}

		/// <summary>Attempts to look up an entry in the dictionary.</summary>
		/// <param name="key">Key to look up.</param>
		/// <param name="result">If <paramref name="key" /> is defined, the <see cref="T:System.Xml.XmlDictionaryString" /> that is mapped to the key; otherwise <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if key is in the dictionary; otherwise, <see langword="false" />.</returns>
		public virtual bool TryLookup(int key, out XmlDictionaryString result)
		{
			if (key < 0 || key >= this.nextId)
			{
				result = null;
				return false;
			}
			result = this.strings[key];
			return true;
		}

		/// <summary>Checks the dictionary for a specified <see cref="T:System.Xml.XmlDictionaryString" />.</summary>
		/// <param name="value">The <see cref="T:System.Xml.XmlDictionaryString" /> being checked for.</param>
		/// <param name="result">The matching <see cref="T:System.Xml.XmlDictionaryString" />, if found; otherwise, <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <see cref="T:System.Xml.XmlDictionaryString" /> is in the dictionary; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public virtual bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (value.Dictionary != this)
			{
				result = null;
				return false;
			}
			result = value;
			return true;
		}

		private static IXmlDictionary empty;

		private Dictionary<string, XmlDictionaryString> lookup;

		private XmlDictionaryString[] strings;

		private int nextId;

		private class EmptyDictionary : IXmlDictionary
		{
			public bool TryLookup(string value, out XmlDictionaryString result)
			{
				result = null;
				return false;
			}

			public bool TryLookup(int key, out XmlDictionaryString result)
			{
				result = null;
				return false;
			}

			public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
			{
				result = null;
				return false;
			}
		}
	}
}
