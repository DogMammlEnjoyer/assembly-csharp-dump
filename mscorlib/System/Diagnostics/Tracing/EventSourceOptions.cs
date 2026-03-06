using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies overrides of default event settings such as the log level, keywords and operation code when the <see cref="M:System.Diagnostics.Tracing.EventSource.Write``1(System.String,System.Diagnostics.Tracing.EventSourceOptions,``0)" /> method is called.</summary>
	public struct EventSourceOptions
	{
		/// <summary>Gets or sets the event level applied to the event.</summary>
		/// <returns>The event level for the event. If not set, the default is Verbose (5).</returns>
		public EventLevel Level
		{
			get
			{
				return (EventLevel)this.level;
			}
			set
			{
				this.level = checked((byte)value);
				this.valuesSet |= 4;
			}
		}

		/// <summary>Gets or sets the operation code to use for the specified event.</summary>
		/// <returns>The operation code to use for the specified event. If not set, the default is <see langword="Info" /> (0).</returns>
		public EventOpcode Opcode
		{
			get
			{
				return (EventOpcode)this.opcode;
			}
			set
			{
				this.opcode = checked((byte)value);
				this.valuesSet |= 8;
			}
		}

		internal bool IsOpcodeSet
		{
			get
			{
				return (this.valuesSet & 8) > 0;
			}
		}

		/// <summary>Gets or sets the keywords applied to the event. If this property is not set, the event's keywords will be <see langword="None" />.</summary>
		/// <returns>The keywords applied to the event, or <see langword="None" /> if no keywords are set.</returns>
		public EventKeywords Keywords
		{
			get
			{
				return this.keywords;
			}
			set
			{
				this.keywords = value;
				this.valuesSet |= 1;
			}
		}

		/// <summary>The event tags defined for this event source.</summary>
		/// <returns>Returns <see cref="T:System.Diagnostics.Tracing.EventTags" />.</returns>
		public EventTags Tags
		{
			get
			{
				return this.tags;
			}
			set
			{
				this.tags = value;
				this.valuesSet |= 2;
			}
		}

		/// <summary>The activity options defined for this event source.</summary>
		/// <returns>Returns <see cref="T:System.Diagnostics.Tracing.EventActivityOptions" />.</returns>
		public EventActivityOptions ActivityOptions
		{
			get
			{
				return this.activityOptions;
			}
			set
			{
				this.activityOptions = value;
				this.valuesSet |= 16;
			}
		}

		internal EventKeywords keywords;

		internal EventTags tags;

		internal EventActivityOptions activityOptions;

		internal byte level;

		internal byte opcode;

		internal byte valuesSet;

		internal const byte keywordsSet = 1;

		internal const byte tagsSet = 2;

		internal const byte levelSet = 4;

		internal const byte opcodeSet = 8;

		internal const byte activityOptionsSet = 16;
	}
}
