using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to a local variable.</summary>
	[Serializable]
	public class CodeVariableReferenceExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeVariableReferenceExpression" /> class.</summary>
		public CodeVariableReferenceExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeVariableReferenceExpression" /> class using the specified local variable name.</summary>
		/// <param name="variableName">The name of the local variable to reference.</param>
		public CodeVariableReferenceExpression(string variableName)
		{
			this._variableName = variableName;
		}

		/// <summary>Gets or sets the name of the local variable to reference.</summary>
		/// <returns>The name of the local variable to reference.</returns>
		public string VariableName
		{
			get
			{
				return this._variableName ?? string.Empty;
			}
			set
			{
				this._variableName = value;
			}
		}

		private string _variableName;
	}
}
