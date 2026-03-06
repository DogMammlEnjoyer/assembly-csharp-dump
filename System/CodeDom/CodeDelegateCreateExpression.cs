using System;

namespace System.CodeDom
{
	/// <summary>Represents an expression that creates a delegate.</summary>
	[Serializable]
	public class CodeDelegateCreateExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeDelegateCreateExpression" /> class.</summary>
		public CodeDelegateCreateExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeDelegateCreateExpression" /> class.</summary>
		/// <param name="delegateType">A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type of the delegate.</param>
		/// <param name="targetObject">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object containing the event-handler method.</param>
		/// <param name="methodName">The name of the event-handler method.</param>
		public CodeDelegateCreateExpression(CodeTypeReference delegateType, CodeExpression targetObject, string methodName)
		{
			this._delegateType = delegateType;
			this.TargetObject = targetObject;
			this._methodName = methodName;
		}

		/// <summary>Gets or sets the data type of the delegate.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type of the delegate.</returns>
		public CodeTypeReference DelegateType
		{
			get
			{
				CodeTypeReference result;
				if ((result = this._delegateType) == null)
				{
					result = (this._delegateType = new CodeTypeReference(""));
				}
				return result;
			}
			set
			{
				this._delegateType = value;
			}
		}

		/// <summary>Gets or sets the object that contains the event-handler method.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object containing the event-handler method.</returns>
		public CodeExpression TargetObject { get; set; }

		/// <summary>Gets or sets the name of the event handler method.</summary>
		/// <returns>The name of the event handler method.</returns>
		public string MethodName
		{
			get
			{
				return this._methodName ?? string.Empty;
			}
			set
			{
				this._methodName = value;
			}
		}

		private CodeTypeReference _delegateType;

		private string _methodName;
	}
}
