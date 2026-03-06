using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace System.Data.Common
{
	/// <summary>Associates a security action with a custom security attribute.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public abstract class DBDataPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DBDataPermissionAttribute" />.</summary>
		/// <param name="action">One of the security action values representing an action that can be performed by declarative security.</param>
		protected DBDataPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Gets or sets a value indicating whether a blank password is allowed.</summary>
		/// <returns>
		///   <see langword="true" /> if a blank password is allowed; otherwise <see langword="false" />.</returns>
		public bool AllowBlankPassword
		{
			get
			{
				return this._allowBlankPassword;
			}
			set
			{
				this._allowBlankPassword = value;
			}
		}

		/// <summary>Gets or sets a permitted connection string.</summary>
		/// <returns>A permitted connection string.</returns>
		public string ConnectionString
		{
			get
			{
				string connectionString = this._connectionString;
				if (connectionString == null)
				{
					return string.Empty;
				}
				return connectionString;
			}
			set
			{
				this._connectionString = value;
			}
		}

		/// <summary>Identifies whether the list of connection string parameters identified by the <see cref="P:System.Data.Common.DBDataPermissionAttribute.KeyRestrictions" /> property are the only connection string parameters allowed.</summary>
		/// <returns>One of the <see cref="T:System.Data.KeyRestrictionBehavior" /> values.</returns>
		public KeyRestrictionBehavior KeyRestrictionBehavior
		{
			get
			{
				return this._behavior;
			}
			set
			{
				if (value <= KeyRestrictionBehavior.PreventUsage)
				{
					this._behavior = value;
					return;
				}
				throw ADP.InvalidKeyRestrictionBehavior(value);
			}
		}

		/// <summary>Gets or sets connection string parameters that are allowed or disallowed.</summary>
		/// <returns>One or more connection string parameters that are allowed or disallowed.</returns>
		public string KeyRestrictions
		{
			get
			{
				string restrictions = this._restrictions;
				if (restrictions == null)
				{
					return ADP.StrEmpty;
				}
				return restrictions;
			}
			set
			{
				this._restrictions = value;
			}
		}

		/// <summary>Identifies whether the attribute should serialize the connection string.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute should serialize the connection string; otherwise <see langword="false" />.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConnectionString()
		{
			return this._connectionString != null;
		}

		/// <summary>Identifies whether the attribute should serialize the set of key restrictions.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute should serialize the set of key restrictions; otherwise <see langword="false" />.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeKeyRestrictions()
		{
			return this._restrictions != null;
		}

		private bool _allowBlankPassword;

		private string _connectionString;

		private string _restrictions;

		private KeyRestrictionBehavior _behavior;
	}
}
