using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class UxmlObjectAsset : UxmlAsset
	{
		public bool isField
		{
			get
			{
				return this.m_IsField;
			}
		}

		public UxmlObjectAsset(string fullTypeNameOrFieldName, bool isField, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition)) : base(fullTypeNameOrFieldName, xmlNamespace)
		{
			this.m_IsField = isField;
		}

		public override string ToString()
		{
			return this.isField ? string.Format("Reference: {0} (id:{1} parent:{2})", base.fullTypeName, base.id, base.parentId) : base.ToString();
		}

		[SerializeField]
		private bool m_IsField;
	}
}
