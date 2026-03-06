using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>The <see cref="T:System.Diagnostics.Tracing.EventFieldAttribute" /> is placed on fields of user-defined types that are passed as <see cref="T:System.Diagnostics.Tracing.EventSource" /> payloads.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EventFieldAttribute : Attribute
	{
		/// <summary>Gets or sets the value that specifies how to format the value of a user-defined type.</summary>
		/// <returns>The value that specifies how to format the value of a user-defined type.</returns>
		[MonoTODO]
		public EventFieldFormat Format
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

		/// <summary>Gets or sets the user-defined <see cref="T:System.Diagnostics.Tracing.EventFieldTags" /> value that is required for fields that contain data that isn't one of the supported types.</summary>
		/// <returns>Returns <see cref="T:System.Diagnostics.Tracing.EventFieldTags" />.</returns>
		[MonoTODO]
		public EventFieldTags Tags
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
