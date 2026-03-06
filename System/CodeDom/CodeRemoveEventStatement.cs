using System;

namespace System.CodeDom
{
	/// <summary>Represents a statement that removes an event handler.</summary>
	[Serializable]
	public class CodeRemoveEventStatement : CodeStatement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeRemoveEventStatement" /> class.</summary>
		public CodeRemoveEventStatement()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeRemoveEventStatement" /> class with the specified event and event handler.</summary>
		/// <param name="eventRef">A <see cref="T:System.CodeDom.CodeEventReferenceExpression" /> that indicates the event to detach the event handler from.</param>
		/// <param name="listener">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the event handler to remove.</param>
		public CodeRemoveEventStatement(CodeEventReferenceExpression eventRef, CodeExpression listener)
		{
			this._eventRef = eventRef;
			this.Listener = listener;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeRemoveEventStatement" /> class using the specified target object, event name, and event handler.</summary>
		/// <param name="targetObject">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object that contains the event.</param>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="listener">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the event handler to remove.</param>
		public CodeRemoveEventStatement(CodeExpression targetObject, string eventName, CodeExpression listener)
		{
			this._eventRef = new CodeEventReferenceExpression(targetObject, eventName);
			this.Listener = listener;
		}

		/// <summary>Gets or sets the event to remove a listener from.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeEventReferenceExpression" /> that indicates the event to remove a listener from.</returns>
		public CodeEventReferenceExpression Event
		{
			get
			{
				CodeEventReferenceExpression result;
				if ((result = this._eventRef) == null)
				{
					result = (this._eventRef = new CodeEventReferenceExpression());
				}
				return result;
			}
			set
			{
				this._eventRef = value;
			}
		}

		/// <summary>Gets or sets the event handler to remove.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the event handler to remove.</returns>
		public CodeExpression Listener { get; set; }

		private CodeEventReferenceExpression _eventRef;
	}
}
