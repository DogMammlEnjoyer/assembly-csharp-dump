using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to an index of an array.</summary>
	[Serializable]
	public class CodeArrayIndexerExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeArrayIndexerExpression" /> class.</summary>
		public CodeArrayIndexerExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeArrayIndexerExpression" /> class using the specified target object and indexes.</summary>
		/// <param name="targetObject">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the array the indexer targets.</param>
		/// <param name="indices">The index or indexes to reference.</param>
		public CodeArrayIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
		{
			this.TargetObject = targetObject;
			this.Indices.AddRange(indices);
		}

		/// <summary>Gets or sets the target object of the array indexer.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeExpression" /> that represents the array being indexed.</returns>
		public CodeExpression TargetObject { get; set; }

		/// <summary>Gets or sets the index or indexes of the indexer expression.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeExpressionCollection" /> that indicates the index or indexes of the indexer expression.</returns>
		public CodeExpressionCollection Indices
		{
			get
			{
				CodeExpressionCollection result;
				if ((result = this._indices) == null)
				{
					result = (this._indices = new CodeExpressionCollection());
				}
				return result;
			}
		}

		private CodeExpressionCollection _indices;
	}
}
