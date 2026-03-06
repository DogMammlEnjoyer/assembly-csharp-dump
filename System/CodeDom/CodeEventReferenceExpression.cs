using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to an event.</summary>
	[Serializable]
	public class CodeEventReferenceExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeEventReferenceExpression" /> class.</summary>
		public CodeEventReferenceExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeEventReferenceExpression" /> class using the specified target object and event name.</summary>
		/// <param name="targetObject">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object that contains the event.</param>
		/// <param name="eventName">The name of the event to reference.</param>
		public CodeEventReferenceExpression(CodeExpression targetObject, string eventName)
		{
			this.TargetObject = targetObject;
			this._eventName = eventName;
		}

		/// <summary>Gets or sets the object that contains the event.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object that contains the event.</returns>
		public CodeExpression TargetObject { get; set; }

		/// <summary>Gets or sets the name of the event.</summary>
		/// <returns>The name of the event.</returns>
		public string EventName
		{
			get
			{
				return this._eventName ?? string.Empty;
			}
			set
			{
				this._eventName = value;
			}
		}

		private string _eventName;
	}
}
