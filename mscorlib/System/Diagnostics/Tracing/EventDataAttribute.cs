using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies a type to be passed to the <see cref="M:System.Diagnostics.Tracing.EventSource.Write``1(System.String,System.Diagnostics.Tracing.EventSourceOptions,``0)" /> method.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class EventDataAttribute : Attribute
	{
		/// <summary>Gets or sets the name to apply to an event if the event type or property is not explicitly named.</summary>
		/// <returns>The name to apply to the event or property.</returns>
		[MonoTODO]
		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
