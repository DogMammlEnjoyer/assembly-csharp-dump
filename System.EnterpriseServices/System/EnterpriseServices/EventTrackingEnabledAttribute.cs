using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Enables event tracking for a component. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class EventTrackingEnabledAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.EventTrackingEnabledAttribute" /> class, enabling event tracking.</summary>
		public EventTrackingEnabledAttribute()
		{
			this.val = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.EventTrackingEnabledAttribute" /> class, optionally disabling event tracking.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable event tracking; otherwise, <see langword="false" />.</param>
		public EventTrackingEnabledAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets the value of the <see cref="P:System.EnterpriseServices.EventTrackingEnabledAttribute.Value" /> property, which indicates whether tracking is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if tracking is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool Value
		{
			get
			{
				return this.val;
			}
		}

		private bool val;
	}
}
