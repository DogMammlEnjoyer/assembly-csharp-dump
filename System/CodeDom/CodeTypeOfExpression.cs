using System;

namespace System.CodeDom
{
	/// <summary>Represents a <see langword="typeof" /> expression, an expression that returns a <see cref="T:System.Type" /> for a specified type name.</summary>
	[Serializable]
	public class CodeTypeOfExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeOfExpression" /> class.</summary>
		public CodeTypeOfExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeOfExpression" /> class.</summary>
		/// <param name="type">A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type for the <see langword="typeof" /> expression.</param>
		public CodeTypeOfExpression(CodeTypeReference type)
		{
			this.Type = type;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeOfExpression" /> class using the specified type.</summary>
		/// <param name="type">The name of the data type for the <see langword="typeof" /> expression.</param>
		public CodeTypeOfExpression(string type)
		{
			this.Type = new CodeTypeReference(type);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeOfExpression" /> class using the specified type.</summary>
		/// <param name="type">The data type of the data type of the <see langword="typeof" /> expression.</param>
		public CodeTypeOfExpression(Type type)
		{
			this.Type = new CodeTypeReference(type);
		}

		/// <summary>Gets or sets the data type referenced by the <see langword="typeof" /> expression.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type referenced by the <see langword="typeof" /> expression. This property will never return <see langword="null" />, and defaults to the <see cref="T:System.Void" /> type.</returns>
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
