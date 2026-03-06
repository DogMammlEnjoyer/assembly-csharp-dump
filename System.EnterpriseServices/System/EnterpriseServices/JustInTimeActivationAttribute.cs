using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Turns just-in-time (JIT) activation on or off. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class JustInTimeActivationAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.JustInTimeActivationAttribute" /> class. The default constructor enables just-in-time (JIT) activation.</summary>
		public JustInTimeActivationAttribute() : this(true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.JustInTimeActivationAttribute" /> class, optionally allowing the disabling of just-in-time (JIT) activation by passing <see langword="false" /> as the parameter.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable JIT activation; otherwise, <see langword="false" />.</param>
		public JustInTimeActivationAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets the value of the <see cref="T:System.EnterpriseServices.JustInTimeActivationAttribute" /> setting.</summary>
		/// <returns>
		///   <see langword="true" /> if JIT activation is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
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
