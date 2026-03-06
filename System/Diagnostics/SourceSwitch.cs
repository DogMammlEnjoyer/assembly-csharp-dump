using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Provides a multilevel switch to control tracing and debug output without recompiling your code.</summary>
	public class SourceSwitch : Switch
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.SourceSwitch" /> class, specifying the name of the source.</summary>
		/// <param name="name">The name of the source.</param>
		public SourceSwitch(string name) : base(name, string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.SourceSwitch" /> class, specifying the display name and the default value for the source switch.</summary>
		/// <param name="displayName">The name of the source switch.</param>
		/// <param name="defaultSwitchValue">The default value for the switch.</param>
		public SourceSwitch(string displayName, string defaultSwitchValue) : base(displayName, string.Empty, defaultSwitchValue)
		{
		}

		/// <summary>Gets or sets the level of the switch.</summary>
		/// <returns>One of the <see cref="T:System.Diagnostics.SourceLevels" /> values that represents the event level of the switch.</returns>
		public SourceLevels Level
		{
			get
			{
				return (SourceLevels)base.SwitchSetting;
			}
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set
			{
				base.SwitchSetting = (int)value;
			}
		}

		/// <summary>Determines if trace listeners should be called, based on the trace event type.</summary>
		/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values.</param>
		/// <returns>
		///   <see langword="true" /> if the trace listeners should be called; otherwise, <see langword="false" />.</returns>
		public bool ShouldTrace(TraceEventType eventType)
		{
			return (base.SwitchSetting & (int)eventType) != 0;
		}

		/// <summary>Invoked when the value of the <see cref="P:System.Diagnostics.Switch.Value" /> property changes.</summary>
		/// <exception cref="T:System.ArgumentException">The new value of <see cref="P:System.Diagnostics.Switch.Value" /> is not one of the <see cref="T:System.Diagnostics.SourceLevels" /> values.</exception>
		protected override void OnValueChanged()
		{
			base.SwitchSetting = (int)Enum.Parse(typeof(SourceLevels), base.Value, true);
		}
	}
}
