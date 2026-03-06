using System;

namespace System.CodeDom
{
	/// <summary>Provides a base class for a member of a type. Type members include fields, methods, properties, constructors and nested types.</summary>
	[Serializable]
	public class CodeTypeMember : CodeObject
	{
		/// <summary>Gets or sets the name of the member.</summary>
		/// <returns>The name of the member.</returns>
		public string Name
		{
			get
			{
				return this._name ?? string.Empty;
			}
			set
			{
				this._name = value;
			}
		}

		/// <summary>Gets or sets the attributes of the member.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.CodeDom.MemberAttributes" /> values used to indicate the attributes of the member. The default value is <see cref="F:System.CodeDom.MemberAttributes.Private" /> | <see cref="F:System.CodeDom.MemberAttributes.Final" />.</returns>
		public MemberAttributes Attributes { get; set; } = (MemberAttributes)20482;

		/// <summary>Gets or sets the custom attributes of the member.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeAttributeDeclarationCollection" /> that indicates the custom attributes of the member.</returns>
		public CodeAttributeDeclarationCollection CustomAttributes
		{
			get
			{
				CodeAttributeDeclarationCollection result;
				if ((result = this._customAttributes) == null)
				{
					result = (this._customAttributes = new CodeAttributeDeclarationCollection());
				}
				return result;
			}
			set
			{
				this._customAttributes = value;
			}
		}

		/// <summary>Gets or sets the line on which the type member statement occurs.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeLinePragma" /> object that indicates the location of the type member declaration.</returns>
		public CodeLinePragma LinePragma { get; set; }

		/// <summary>Gets the collection of comments for the type member.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeCommentStatementCollection" /> that indicates the comments for the member.</returns>
		public CodeCommentStatementCollection Comments { get; } = new CodeCommentStatementCollection();

		/// <summary>Gets the start directives for the member.</summary>
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

		/// <summary>Gets the end directives for the member.</summary>
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

		private string _name;

		private CodeAttributeDeclarationCollection _customAttributes;

		private CodeDirectiveCollection _startDirectives;

		private CodeDirectiveCollection _endDirectives;
	}
}
