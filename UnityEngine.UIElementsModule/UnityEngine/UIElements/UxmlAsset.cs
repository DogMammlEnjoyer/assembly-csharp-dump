using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class UxmlAsset : IUxmlAttributes
	{
		public UxmlAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		{
			this.m_FullTypeName = fullTypeName;
			this.m_XmlNamespace = xmlNamespace;
		}

		public string fullTypeName
		{
			get
			{
				return this.m_FullTypeName;
			}
			set
			{
				this.m_FullTypeName = value;
			}
		}

		public UxmlNamespaceDefinition xmlNamespace
		{
			get
			{
				return this.m_XmlNamespace;
			}
			set
			{
				this.m_XmlNamespace = value;
			}
		}

		public int id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
			}
		}

		public bool isNull
		{
			get
			{
				return this.fullTypeName == "null";
			}
		}

		public int orderInDocument
		{
			get
			{
				return this.m_OrderInDocument;
			}
			set
			{
				this.m_OrderInDocument = value;
			}
		}

		public int parentId
		{
			get
			{
				return this.m_ParentId;
			}
			set
			{
				this.m_ParentId = value;
			}
		}

		public List<UxmlNamespaceDefinition> namespaceDefinitions
		{
			get
			{
				List<UxmlNamespaceDefinition> result;
				if ((result = this.m_NamespaceDefinitions) == null)
				{
					result = (this.m_NamespaceDefinitions = new List<UxmlNamespaceDefinition>());
				}
				return result;
			}
		}

		public List<string> GetProperties()
		{
			return this.m_Properties;
		}

		public bool HasParent()
		{
			return this.m_ParentId != 0;
		}

		public bool HasAttribute(string attributeName)
		{
			bool flag = this.m_Properties == null || this.m_Properties.Count <= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.m_Properties.Count; i += 2)
				{
					string a = this.m_Properties[i];
					bool flag2 = a == attributeName;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public string GetAttributeValue(string attributeName)
		{
			string result;
			this.TryGetAttributeValue(attributeName, out result);
			return result;
		}

		public bool TryGetAttributeValue(string propertyName, out string value)
		{
			bool flag = this.m_Properties == null;
			bool result;
			if (flag)
			{
				value = null;
				result = false;
			}
			else
			{
				for (int i = 0; i < this.m_Properties.Count - 1; i += 2)
				{
					bool flag2 = this.m_Properties[i] == propertyName;
					if (flag2)
					{
						value = this.m_Properties[i + 1];
						return true;
					}
				}
				value = null;
				result = false;
			}
			return result;
		}

		public void AddUxmlNamespace(string prefix, string resolvedNamespace)
		{
			this.namespaceDefinitions.Add(new UxmlNamespaceDefinition
			{
				prefix = prefix,
				resolvedNamespace = resolvedNamespace
			});
		}

		public void SetAttribute(string name, string value)
		{
			this.SetOrAddProperty(name, value);
		}

		public void RemoveAttribute(string attributeName)
		{
			bool flag = this.m_Properties == null || this.m_Properties.Count <= 0;
			if (!flag)
			{
				for (int i = 0; i < this.m_Properties.Count; i += 2)
				{
					string a = this.m_Properties[i];
					bool flag2 = a != attributeName;
					if (!flag2)
					{
						this.m_Properties.RemoveAt(i);
						this.m_Properties.RemoveAt(i);
						break;
					}
				}
			}
		}

		private void SetOrAddProperty(string propertyName, string propertyValue)
		{
			bool flag = this.m_Properties == null;
			if (flag)
			{
				this.m_Properties = new List<string>();
			}
			for (int i = 0; i < this.m_Properties.Count - 1; i += 2)
			{
				bool flag2 = this.m_Properties[i] == propertyName;
				if (flag2)
				{
					this.m_Properties[i + 1] = propertyValue;
					return;
				}
			}
			this.m_Properties.Add(propertyName);
			this.m_Properties.Add(propertyValue);
		}

		public override string ToString()
		{
			return string.Format("{0}(id:{1})", this.fullTypeName, this.id);
		}

		public const string NullNodeType = "null";

		[SerializeField]
		private string m_FullTypeName;

		[SerializeField]
		private UxmlNamespaceDefinition m_XmlNamespace;

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private int m_OrderInDocument;

		[SerializeField]
		private int m_ParentId;

		[SerializeField]
		private List<UxmlNamespaceDefinition> m_NamespaceDefinitions;

		[SerializeField]
		protected List<string> m_Properties;
	}
}
