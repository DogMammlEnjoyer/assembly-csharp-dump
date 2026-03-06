using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace System.Security.Principal
{
	/// <summary>Represents a generic user.</summary>
	[Serializable]
	public class GenericIdentity : ClaimsIdentity
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.GenericIdentity" /> class representing the user with the specified name.</summary>
		/// <param name="name">The name of the user on whose behalf the code is running.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		public GenericIdentity(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.m_name = name;
			this.m_type = "";
			this.AddNameClaim();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.GenericIdentity" /> class representing the user with the specified name and authentication type.</summary>
		/// <param name="name">The name of the user on whose behalf the code is running.</param>
		/// <param name="type">The type of authentication used to identify the user.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="type" /> parameter is <see langword="null" />.</exception>
		public GenericIdentity(string name, string type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.m_name = name;
			this.m_type = type;
			this.AddNameClaim();
		}

		private GenericIdentity()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.GenericIdentity" /> class by using the specified <see cref="T:System.Security.Principal.GenericIdentity" /> object.</summary>
		/// <param name="identity">The object from which to construct the new instance of <see cref="T:System.Security.Principal.GenericIdentity" />.</param>
		protected GenericIdentity(GenericIdentity identity) : base(identity)
		{
			this.m_name = identity.m_name;
			this.m_type = identity.m_type;
		}

		/// <summary>Creates a new object that is a copy of the current instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		public override ClaimsIdentity Clone()
		{
			return new GenericIdentity(this);
		}

		/// <summary>Gets all claims for the user represented by this generic identity.</summary>
		/// <returns>A collection of claims for this <see cref="T:System.Security.Principal.GenericIdentity" /> object.</returns>
		public override IEnumerable<Claim> Claims
		{
			get
			{
				return base.Claims;
			}
		}

		/// <summary>Gets the user's name.</summary>
		/// <returns>The name of the user on whose behalf the code is being run.</returns>
		public override string Name
		{
			get
			{
				return this.m_name;
			}
		}

		/// <summary>Gets the type of authentication used to identify the user.</summary>
		/// <returns>The type of authentication used to identify the user.</returns>
		public override string AuthenticationType
		{
			get
			{
				return this.m_type;
			}
		}

		/// <summary>Gets a value indicating whether the user has been authenticated.</summary>
		/// <returns>
		///   <see langword="true" /> if the user was has been authenticated; otherwise, <see langword="false" />.</returns>
		public override bool IsAuthenticated
		{
			get
			{
				return !this.m_name.Equals("");
			}
		}

		private void AddNameClaim()
		{
			if (this.m_name != null)
			{
				base.AddClaim(new Claim(base.NameClaimType, this.m_name, "http://www.w3.org/2001/XMLSchema#string", "LOCAL AUTHORITY", "LOCAL AUTHORITY", this));
			}
		}

		private readonly string m_name;

		private readonly string m_type;
	}
}
