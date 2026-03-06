using System;

namespace UnityEngine.UIElements.Experimental
{
	internal class EventDebuggerTrace
	{
		public EventDebuggerEventRecord eventBase { get; }

		public IEventHandler focusedElement { get; }

		public IEventHandler mouseCapture { get; }

		public long duration { get; set; }

		public EventDebuggerTrace(IPanel panel, EventBase evt, long duration, IEventHandler mouseCapture)
		{
			this.eventBase = new EventDebuggerEventRecord(evt);
			object obj;
			if (panel == null)
			{
				obj = null;
			}
			else
			{
				FocusController focusController = panel.focusController;
				obj = ((focusController != null) ? focusController.focusedElement : null);
			}
			this.focusedElement = obj;
			this.mouseCapture = mouseCapture;
			this.duration = duration;
		}
	}
}
