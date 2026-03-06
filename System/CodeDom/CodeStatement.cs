using System;

namespace System.CodeDom
{
	/// <summary>Represents the <see langword="abstract" /> base class from which all code statements derive.</summary>
	[Serializable]
	public class CodeStatement : CodeObject
	{
		/// <summary>Gets or sets the line on which the code statement occurs.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeLinePragma" /> object that indicates the context of the code statement.</returns>
		public CodeLinePragma LinePragma { get; set; }

		/// <summary>Gets a <see cref="T:System.CodeDom.CodeDirectiveCollection" /> object that contains start directives.</summary>
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

		/// <summary>Gets a <see cref="T:System.CodeDom.CodeDirectiveCollection" /> object that contains end directives.</summary>
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

		private CodeDirectiveCollection _startDirectives;

		private CodeDirectiveCollection _endDirectives;
	}
}
