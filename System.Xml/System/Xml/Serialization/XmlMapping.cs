using System;
using Unity;

namespace System.Xml.Serialization
{
	/// <summary>Supports mappings between .NET Framework types and XML Schema data types. </summary>
	public abstract class XmlMapping
	{
		internal XmlMapping(TypeScope scope, ElementAccessor accessor) : this(scope, accessor, XmlMappingAccess.Read | XmlMappingAccess.Write)
		{
		}

		internal XmlMapping(TypeScope scope, ElementAccessor accessor, XmlMappingAccess access)
		{
			this.scope = scope;
			this.accessor = accessor;
			this.access = access;
			this.shallow = (scope == null);
		}

		internal ElementAccessor Accessor
		{
			get
			{
				return this.accessor;
			}
		}

		internal TypeScope Scope
		{
			get
			{
				return this.scope;
			}
		}

		/// <summary>Get the name of the mapped element.</summary>
		/// <returns>The name of the mapped element.</returns>
		public string ElementName
		{
			get
			{
				return System.Xml.Serialization.Accessor.UnescapeName(this.Accessor.Name);
			}
		}

		/// <summary>Gets the name of the XSD element of the mapping.</summary>
		/// <returns>The XSD element name.</returns>
		public string XsdElementName
		{
			get
			{
				return this.Accessor.Name;
			}
		}

		/// <summary>Gets the namespace of the mapped element.</summary>
		/// <returns>The namespace of the mapped element.</returns>
		public string Namespace
		{
			get
			{
				return this.accessor.Namespace;
			}
		}

		internal bool GenerateSerializer
		{
			get
			{
				return this.generateSerializer;
			}
			set
			{
				this.generateSerializer = value;
			}
		}

		internal bool IsReadable
		{
			get
			{
				return (this.access & XmlMappingAccess.Read) > XmlMappingAccess.None;
			}
		}

		internal bool IsWriteable
		{
			get
			{
				return (this.access & XmlMappingAccess.Write) > XmlMappingAccess.None;
			}
		}

		internal bool IsSoap
		{
			get
			{
				return this.isSoap;
			}
			set
			{
				this.isSoap = value;
			}
		}

		/// <summary>Sets the key used to look up the mapping.</summary>
		/// <param name="key">A <see cref="T:System.String" /> that contains the lookup key.</param>
		public void SetKey(string key)
		{
			this.SetKeyInternal(key);
		}

		internal void SetKeyInternal(string key)
		{
			this.key = key;
		}

		internal static string GenerateKey(Type type, XmlRootAttribute root, string ns)
		{
			if (root == null)
			{
				root = (XmlRootAttribute)XmlAttributes.GetAttr(type, typeof(XmlRootAttribute));
			}
			return string.Concat(new string[]
			{
				type.FullName,
				":",
				(root == null) ? string.Empty : root.Key,
				":",
				(ns == null) ? string.Empty : ns
			});
		}

		internal string Key
		{
			get
			{
				return this.key;
			}
		}

		internal void CheckShallow()
		{
			if (this.shallow)
			{
				throw new InvalidOperationException(Res.GetString("This mapping was not crated by reflection importer and cannot be used in this context."));
			}
		}

		internal static bool IsShallow(XmlMapping[] mappings)
		{
			for (int i = 0; i < mappings.Length; i++)
			{
				if (mappings[i] == null || mappings[i].shallow)
				{
					return true;
				}
			}
			return false;
		}

		internal XmlMapping()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private TypeScope scope;

		private bool generateSerializer;

		private bool isSoap;

		private ElementAccessor accessor;

		private string key;

		private bool shallow;

		private XmlMappingAccess access;
	}
}
