using System;
using System.Collections.Specialized;

namespace System.CodeDom
{
	/// <summary>Provides a container for a CodeDOM program graph.</summary>
	[Serializable]
	public class CodeCompileUnit : CodeObject
	{
		/// <summary>Gets the collection of namespaces.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeNamespaceCollection" /> that indicates the namespaces that the compile unit uses.</returns>
		public CodeNamespaceCollection Namespaces { get; } = new CodeNamespaceCollection();

		/// <summary>Gets the referenced assemblies.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.StringCollection" /> that contains the file names of the referenced assemblies.</returns>
		public StringCollection ReferencedAssemblies
		{
			get
			{
				StringCollection result;
				if ((result = this._assemblies) == null)
				{
					result = (this._assemblies = new StringCollection());
				}
				return result;
			}
		}

		/// <summary>Gets a collection of custom attributes for the generated assembly.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeAttributeDeclarationCollection" /> that indicates the custom attributes for the generated assembly.</returns>
		public CodeAttributeDeclarationCollection AssemblyCustomAttributes
		{
			get
			{
				CodeAttributeDeclarationCollection result;
				if ((result = this._attributes) == null)
				{
					result = (this._attributes = new CodeAttributeDeclarationCollection());
				}
				return result;
			}
		}

		/// <summary>Gets a <see cref="T:System.CodeDom.CodeDirectiveCollection" /> object containing start directives.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeDirectiveCollection" /> object containing start directives.</returns>
		public CodeDirectiveCollection StartDirectives
		{
			get
			{
				CodeDirectiveCollection result;
				if ((result = this._startDirectives) == null)
				{
					result = (this._startDirectives = new CodeDirectiveCollection());
				}
				return result;
			}
		}

		/// <summary>Gets a <see cref="T:System.CodeDom.CodeDirectiveCollection" /> object containing end directives.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeDirectiveCollection" /> object containing end directives.</returns>
		public CodeDirectiveCollection EndDirectives
		{
			get
			{
				CodeDirectiveCollection result;
				if ((result = this._endDirectives) == null)
				{
					result = (this._endDirectives = new CodeDirectiveCollection());
				}
				return result;
			}
		}

		private StringCollection _assemblies;

		private CodeAttributeDeclarationCollection _attributes;

		private CodeDirectiveCollection _startDirectives;

		private CodeDirectiveCollection _endDirectives;
	}
}
