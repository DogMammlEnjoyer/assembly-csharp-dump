using System;

namespace System.CodeDom
{
	/// <summary>Represents a member of a type using a literal code fragment.</summary>
	[Serializable]
	public class CodeSnippetTypeMember : CodeTypeMember
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeSnippetTypeMember" /> class.</summary>
		public CodeSnippetTypeMember()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeSnippetTypeMember" /> class using the specified text.</summary>
		/// <param name="text">The literal code fragment for the type member.</param>
		public CodeSnippetTypeMember(string text)
		{
			this.Text = text;
		}

		/// <summary>Gets or sets the literal code fragment for the type member.</summary>
		/// <returns>The literal code fragment for the type member.</returns>
		public string Text
		{
			get
			{
				return this._text ?? string.Empty;
			}
			set
			{
				this._text = value;
			}
		}

		private string _text;
	}
}
