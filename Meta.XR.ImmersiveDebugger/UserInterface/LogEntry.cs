using System;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class LogEntry
	{
		public static Action<LogEntry> OnDisplayDetails { get; set; }

		public void Setup(string label, string callstack, SeverityEntry severity)
		{
			this.Label = label;
			this.Callstack = callstack;
			this.Severity = severity;
			this.Line = null;
			this.Count = 1;
		}

		public string Label { get; private set; }

		public string Callstack { get; private set; }

		public SeverityEntry Severity { get; private set; }

		public int Count { get; set; }

		public ProxyConsoleLine Line { get; set; }

		public bool Shown
		{
			get
			{
				return this.Line != null;
			}
		}

		public void DisplayDetails()
		{
			Action<LogEntry> onDisplayDetails = LogEntry.OnDisplayDetails;
			if (onDisplayDetails == null)
			{
				return;
			}
			onDisplayDetails(this);
		}
	}
}
