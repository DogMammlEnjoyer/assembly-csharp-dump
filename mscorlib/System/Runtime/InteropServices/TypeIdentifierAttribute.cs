using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Provides support for type equivalence.</summary>
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	public sealed class TypeIdentifierAttribute : Attribute
	{
		/// <summary>Creates a new instance of the <see cref="T:System.Runtime.InteropServices.TypeIdentifierAttribute" /> class.</summary>
		public TypeIdentifierAttribute()
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Runtime.InteropServices.TypeIdentifierAttribute" /> class with the specified scope and identifier.</summary>
		/// <param name="scope">The first type equivalence string.</param>
		/// <param name="identifier">The second type equivalence string.</param>
		public TypeIdentifierAttribute(string scope, string identifier)
		{
			this.Scope_ = scope;
			this.Identifier_ = identifier;
		}

		/// <summary>Gets the value of the <paramref name="scope" /> parameter that was passed to the <see cref="M:System.Runtime.InteropServices.TypeIdentifierAttribute.#ctor(System.String,System.String)" /> constructor.</summary>
		/// <returns>The value of the constructor's <paramref name="scope" /> parameter.</returns>
		public string Scope
		{
			get
			{
				return this.Scope_;
			}
		}

		/// <summary>Gets the value of the <paramref name="identifier" /> parameter that was passed to the <see cref="M:System.Runtime.InteropServices.TypeIdentifierAttribute.#ctor(System.String,System.String)" /> constructor.</summary>
		/// <returns>The value of the constructor's <paramref name="identifier" /> parameter.</returns>
		public string Identifier
		{
			get
			{
				return this.Identifier_;
			}
		}

		internal string Scope_;

		internal string Identifier_;
	}
}
