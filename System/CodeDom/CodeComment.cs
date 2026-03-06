using System;

namespace System.CodeDom
{
	/// <summary>Represents a comment.</summary>
	[Serializable]
	public class CodeComment : CodeObject
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeComment" /> class.</summary>
		public CodeComment()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeComment" /> class with the specified text as contents.</summary>
		/// <param name="text">The contents of the comment.</param>
		public CodeComment(string text)
		{
			this.Text = text;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeComment" /> class using the specified text and documentation comment flag.</summary>
		/// <param name="text">The contents of the comment.</param>
		/// <param name="docComment">
		///   <see langword="true" /> if the comment is a documentation comment; otherwise, <see langword="false" />.</param>
		public CodeComment(string text, bool docComment)
		{
			this.Text = text;
			this.DocComment = docComment;
		}

		/// <summary>Gets or sets a value that indicates whether the comment is a documentation comment.</summary>
		/// <returns>
		///   <see langword="true" /> if the comment is a documentation comment; otherwise, <see langword="false" />.</returns>
		public bool DocComment { get; set; }

		/// <summary>Gets or sets the text of the comment.</summary>
		/// <returns>A string containing the comment text.</returns>
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
