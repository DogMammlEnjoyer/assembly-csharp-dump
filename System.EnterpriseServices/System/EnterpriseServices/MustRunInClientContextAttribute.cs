using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Forces the attributed object to be created in the context of the creator, if possible. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class MustRunInClientContextAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.MustRunInClientContextAttribute" /> class, requiring creation of the object in the context of the creator.</summary>
		public MustRunInClientContextAttribute() : this(true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.MustRunInClientContextAttribute" /> class, optionally not creating the object in the context of the creator.</summary>
		/// <param name="val">
		///   <see langword="true" /> to create the object in the context of the creator; otherwise, <see langword="false" />.</param>
		public MustRunInClientContextAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets a value that indicates whether the attributed object is to be created in the context of the creator.</summary>
		/// <returns>
		///   <see langword="true" /> if the object is to be created in the context of the creator; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
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
