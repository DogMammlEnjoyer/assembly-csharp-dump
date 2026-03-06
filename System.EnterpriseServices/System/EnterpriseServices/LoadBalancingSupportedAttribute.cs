using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Determines whether the component participates in load balancing, if the component load balancing service is installed and enabled on the server.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class LoadBalancingSupportedAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.LoadBalancingSupportedAttribute" /> class, specifying load balancing support.</summary>
		public LoadBalancingSupportedAttribute() : this(true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.LoadBalancingSupportedAttribute" /> class, optionally disabling load balancing support.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable load balancing support; otherwise, <see langword="false" />.</param>
		public LoadBalancingSupportedAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets a value that indicates whether load balancing support is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if load balancing support is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
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
