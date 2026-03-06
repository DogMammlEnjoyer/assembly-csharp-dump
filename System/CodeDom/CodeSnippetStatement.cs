using System;

namespace System.CodeDom
{
	/// <summary>Represents a statement using a literal code fragment.</summary>
	[Serializable]
	public class CodeSnippetStatement : CodeStatement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeSnippetStatement" /> class.</summary>
		public CodeSnippetStatement()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeSnippetStatement" /> class using the specified code fragment.</summary>
		/// <param name="value">The literal code fragment of the statement to represent.</param>
		public CodeSnippetStatement(string value)
		{
			this.Value = value;
		}

		/// <summary>Gets or sets the literal code fragment statement.</summary>
		/// <returns>The literal code fragment statement.</returns>
		public string Value
		{
			get
			{
				return this._value ?? string.Empty;
			}
			set
			{
				this._value = value;
			}
		}

		private string _value;
	}
}
