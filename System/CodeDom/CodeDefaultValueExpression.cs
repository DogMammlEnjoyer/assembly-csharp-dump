using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to a default value.</summary>
	[Serializable]
	public class CodeDefaultValueExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeDefaultValueExpression" /> class.</summary>
		public CodeDefaultValueExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeDefaultValueExpression" /> class using the specified code type reference.</summary>
		/// <param name="type">A <see cref="T:System.CodeDom.CodeTypeReference" /> that specifies the reference to a value type.</param>
		public CodeDefaultValueExpression(CodeTypeReference type)
		{
			this._type = type;
		}

		/// <summary>Gets or sets the data type reference for a default value.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> object representing a data type that has a default value.</returns>
		public CodeTypeReference Type
		{
			get
			{
				CodeTypeReference result;
				if ((result = this._type) == null)
				{
					result = (this._type = new CodeTypeReference(""));
				}
				return result;
			}
			set
			{
				this._type = value;
			}
		}

		private CodeTypeReference _type;
	}
}
