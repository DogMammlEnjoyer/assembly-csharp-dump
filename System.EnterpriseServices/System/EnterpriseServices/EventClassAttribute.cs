using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Marks the attributed class as an event class. This class cannot be inherited.</summary>
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EventClassAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.EventClassAttribute" /> class.</summary>
		public EventClassAttribute()
		{
			this.allowInProcSubscribers = true;
			this.fireInParallel = false;
			this.publisherFilter = null;
		}

		/// <summary>Gets or sets a value that indicates whether subscribers can be activated in the publisher's process.</summary>
		/// <returns>
		///   <see langword="true" /> if subscribers can be activated in the publisher's process; otherwise, <see langword="false" />.</returns>
		public bool AllowInprocSubscribers
		{
			get
			{
				return this.allowInProcSubscribers;
			}
			set
			{
				this.allowInProcSubscribers = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether events are to be delivered to subscribers in parallel.</summary>
		/// <returns>
		///   <see langword="true" /> if events are to be delivered to subscribers in parallel; otherwise, <see langword="false" />.</returns>
		public bool FireInParallel
		{
			get
			{
				return this.fireInParallel;
			}
			set
			{
				this.fireInParallel = value;
			}
		}

		/// <summary>Gets or sets a publisher filter for an event method.</summary>
		/// <returns>The publisher filter.</returns>
		public string PublisherFilter
		{
			get
			{
				return this.publisherFilter;
			}
			set
			{
				this.publisherFilter = value;
			}
		}

		private bool allowInProcSubscribers;

		private bool fireInParallel;

		private string publisherFilter;
	}
}
