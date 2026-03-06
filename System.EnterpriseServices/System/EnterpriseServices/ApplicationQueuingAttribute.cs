using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Enables queuing support for the marked assembly and enables the application to read method calls from Message Queuing queues. This class cannot be inherited.</summary>
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ApplicationQueuingAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.ApplicationQueuingAttribute" /> class, enabling queuing support for the assembly and initializing <see cref="P:System.EnterpriseServices.ApplicationQueuingAttribute.Enabled" />, <see cref="P:System.EnterpriseServices.ApplicationQueuingAttribute.QueueListenerEnabled" />, and <see cref="P:System.EnterpriseServices.ApplicationQueuingAttribute.MaxListenerThreads" />.</summary>
		public ApplicationQueuingAttribute()
		{
			this.enabled = true;
			this.queueListenerEnabled = false;
			this.maxListenerThreads = 0;
		}

		/// <summary>Gets or sets a value indicating whether queuing support is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if queuing support is enabled; otherwise, <see langword="false" />. The default value set by the constructor is <see langword="true" />.</returns>
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		/// <summary>Gets or sets the number of threads used to extract messages from the queue and activate the corresponding component.</summary>
		/// <returns>The maximum number of threads to use for processing messages arriving in the queue. The default is zero.</returns>
		public int MaxListenerThreads
		{
			get
			{
				return this.maxListenerThreads;
			}
			set
			{
				this.maxListenerThreads = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the application will accept queued component calls from clients.</summary>
		/// <returns>
		///   <see langword="true" /> if the application accepts queued component calls; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool QueueListenerEnabled
		{
			get
			{
				return this.queueListenerEnabled;
			}
			set
			{
				this.queueListenerEnabled = value;
			}
		}

		private bool enabled;

		private int maxListenerThreads;

		private bool queueListenerEnabled;
	}
}
