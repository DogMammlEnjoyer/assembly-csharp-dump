using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal struct UxmlNamespaceDefinition : IEquatable<UxmlNamespaceDefinition>
	{
		public static UxmlNamespaceDefinition Empty { get; } = default(UxmlNamespaceDefinition);

		public string Export()
		{
			bool flag = string.IsNullOrEmpty(this.prefix);
			string result;
			if (flag)
			{
				result = "xmlns=\"" + this.resolvedNamespace + "\"";
			}
			else
			{
				result = string.Concat(new string[]
				{
					"xmlns:",
					this.prefix,
					"=\"",
					this.resolvedNamespace,
					"\""
				});
			}
			return result;
		}

		public static bool operator ==(UxmlNamespaceDefinition lhs, UxmlNamespaceDefinition rhs)
		{
			return string.Compare(lhs.prefix, rhs.prefix, StringComparison.Ordinal) == 0 && string.Compare(lhs.resolvedNamespace, rhs.resolvedNamespace, StringComparison.Ordinal) == 0;
		}

		public static bool operator !=(UxmlNamespaceDefinition lhs, UxmlNamespaceDefinition rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(UxmlNamespaceDefinition other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is UxmlNamespaceDefinition)
			{
				UxmlNamespaceDefinition other = (UxmlNamespaceDefinition)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<string, string>(this.prefix, this.resolvedNamespace);
		}

		public string prefix;

		public string resolvedNamespace;
	}
}
