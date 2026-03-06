using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	/// <summary>Enables a .NET Framework data provider to help ensure that a user has a security level adequate for accessing data.</summary>
	[SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
	[Serializable]
	public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		/// <summary>Initializes a new instance of a <see langword="DBDataPermission" /> class.</summary>
		[Obsolete("DBDataPermission() has been deprecated.  Use the DBDataPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
		protected DBDataPermission() : this(PermissionState.None)
		{
		}

		/// <summary>Initializes a new instance of a <see langword="DBDataPermission" /> class with the specified <see cref="T:System.Security.Permissions.PermissionState" /> value.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		protected DBDataPermission(PermissionState state)
		{
			this._keyvaluetree = NameValuePermission.Default;
			base..ctor();
			if (state == PermissionState.Unrestricted)
			{
				this._isUnrestricted = true;
				return;
			}
			if (state == PermissionState.None)
			{
				this._isUnrestricted = false;
				return;
			}
			throw ADP.InvalidPermissionState(state);
		}

		/// <summary>Initializes a new instance of a <see langword="DBDataPermission" /> class with the specified <see cref="T:System.Security.Permissions.PermissionState" /> value, and a value indicating whether a blank password is allowed.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		/// <param name="allowBlankPassword">
		///   <see langword="true" /> to indicate that a blank password is allowed; otherwise, <see langword="false" />.</param>
		[Obsolete("DBDataPermission(PermissionState state,Boolean allowBlankPassword) has been deprecated.  Use the DBDataPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
		protected DBDataPermission(PermissionState state, bool allowBlankPassword) : this(state)
		{
			this.AllowBlankPassword = allowBlankPassword;
		}

		/// <summary>Initializes a new instance of a <see langword="DBDataPermission" /> class using an existing <see langword="DBDataPermission" />.</summary>
		/// <param name="permission">An existing <see langword="DBDataPermission" /> used to create a new <see langword="DBDataPermission" />.</param>
		protected DBDataPermission(DBDataPermission permission)
		{
			this._keyvaluetree = NameValuePermission.Default;
			base..ctor();
			if (permission == null)
			{
				throw ADP.ArgumentNull("permissionAttribute");
			}
			this.CopyFrom(permission);
		}

		/// <summary>Initializes a new instance of a <see langword="DBDataPermission" /> class with the specified <see langword="DBDataPermissionAttribute" />.</summary>
		/// <param name="permissionAttribute">A security action associated with a custom security attribute.</param>
		protected DBDataPermission(DBDataPermissionAttribute permissionAttribute)
		{
			this._keyvaluetree = NameValuePermission.Default;
			base..ctor();
			if (permissionAttribute == null)
			{
				throw ADP.ArgumentNull("permissionAttribute");
			}
			this._isUnrestricted = permissionAttribute.Unrestricted;
			if (!this._isUnrestricted)
			{
				this._allowBlankPassword = permissionAttribute.AllowBlankPassword;
				if (permissionAttribute.ShouldSerializeConnectionString() || permissionAttribute.ShouldSerializeKeyRestrictions())
				{
					this.Add(permissionAttribute.ConnectionString, permissionAttribute.KeyRestrictions, permissionAttribute.KeyRestrictionBehavior);
				}
			}
		}

		internal DBDataPermission(DbConnectionOptions connectionOptions)
		{
			this._keyvaluetree = NameValuePermission.Default;
			base..ctor();
			if (connectionOptions != null)
			{
				this._allowBlankPassword = connectionOptions.HasBlankPassword;
				this.AddPermissionEntry(new DBConnectionString(connectionOptions));
			}
		}

		/// <summary>Gets a value indicating whether a blank password is allowed.</summary>
		/// <returns>
		///   <see langword="true" /> if a blank password is allowed, otherwise, <see langword="false" />.</returns>
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

		/// <summary>Adds access for the specified connection string to the existing state of the <see langword="DBDataPermission" />.</summary>
		/// <param name="connectionString">A permitted connection string.</param>
		/// <param name="restrictions">String that identifies connection string parameters that are allowed or disallowed.</param>
		/// <param name="behavior">One of the <see cref="T:System.Data.KeyRestrictionBehavior" /> properties.</param>
		public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			DBConnectionString entry = new DBConnectionString(connectionString, restrictions, behavior, null, false);
			this.AddPermissionEntry(entry);
		}

		internal void AddPermissionEntry(DBConnectionString entry)
		{
			if (this._keyvaluetree == null)
			{
				this._keyvaluetree = new NameValuePermission();
			}
			if (this._keyvalues == null)
			{
				this._keyvalues = new ArrayList();
			}
			NameValuePermission.AddEntry(this._keyvaluetree, this._keyvalues, entry);
			this._isUnrestricted = false;
		}

		/// <summary>Removes all permissions that were previous added using the <see cref="M:System.Data.Common.DBDataPermission.Add(System.String,System.String,System.Data.KeyRestrictionBehavior)" /> method.</summary>
		protected void Clear()
		{
			this._keyvaluetree = null;
			this._keyvalues = null;
		}

		/// <summary>Creates and returns an identical copy of the current permission object.</summary>
		/// <returns>A copy of the current permission object.</returns>
		public override IPermission Copy()
		{
			DBDataPermission dbdataPermission = this.CreateInstance();
			dbdataPermission.CopyFrom(this);
			return dbdataPermission;
		}

		private void CopyFrom(DBDataPermission permission)
		{
			this._isUnrestricted = permission.IsUnrestricted();
			if (!this._isUnrestricted)
			{
				this._allowBlankPassword = permission.AllowBlankPassword;
				if (permission._keyvalues != null)
				{
					this._keyvalues = (ArrayList)permission._keyvalues.Clone();
					if (permission._keyvaluetree != null)
					{
						this._keyvaluetree = permission._keyvaluetree.CopyNameValue();
					}
				}
			}
		}

		/// <summary>Creates a new instance of the <see langword="DBDataPermission" /> class.</summary>
		/// <returns>A new <see langword="DBDataPermission" /> object.</returns>
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		protected virtual DBDataPermission CreateInstance()
		{
			return Activator.CreateInstance(base.GetType(), BindingFlags.Instance | BindingFlags.Public, null, null, CultureInfo.InvariantCulture, null) as DBDataPermission;
		}

		/// <summary>Returns a new permission object representing the intersection of the current permission object and the specified permission object.</summary>
		/// <param name="target">A permission object to intersect with the current permission object. It must be of the same type as the current permission object.</param>
		/// <returns>A new permission object that represents the intersection of the current permission object and the specified permission object. This new permission object is a null reference (<see langword="Nothing" /> in Visual Basic) if the intersection is empty.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> parameter is not a null reference (<see langword="Nothing" /> in Visual Basic) and is not an instance of the same class as the current permission object.</exception>
		public override IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			if (target.GetType() != base.GetType())
			{
				throw ADP.PermissionTypeMismatch();
			}
			if (this.IsUnrestricted())
			{
				return target.Copy();
			}
			DBDataPermission dbdataPermission = (DBDataPermission)target;
			if (dbdataPermission.IsUnrestricted())
			{
				return this.Copy();
			}
			DBDataPermission dbdataPermission2 = (DBDataPermission)dbdataPermission.Copy();
			dbdataPermission2._allowBlankPassword &= this.AllowBlankPassword;
			if (this._keyvalues != null && dbdataPermission2._keyvalues != null)
			{
				dbdataPermission2._keyvalues.Clear();
				dbdataPermission2._keyvaluetree.Intersect(dbdataPermission2._keyvalues, this._keyvaluetree);
			}
			else
			{
				dbdataPermission2._keyvalues = null;
				dbdataPermission2._keyvaluetree = null;
			}
			if (dbdataPermission2.IsEmpty())
			{
				dbdataPermission2 = null;
			}
			return dbdataPermission2;
		}

		private bool IsEmpty()
		{
			ArrayList keyvalues = this._keyvalues;
			return !this.IsUnrestricted() && !this.AllowBlankPassword && (keyvalues == null || keyvalues.Count == 0);
		}

		/// <summary>Returns a value indicating whether the current permission object is a subset of the specified permission object.</summary>
		/// <param name="target">A permission object that is to be tested for the subset relationship. This object must be of the same type as the current permission object.</param>
		/// <returns>
		///   <see langword="true" /> if the current permission object is a subset of the specified permission object, otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> parameter is an object that is not of the same type as the current permission object.</exception>
		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return this.IsEmpty();
			}
			if (target.GetType() != base.GetType())
			{
				throw ADP.PermissionTypeMismatch();
			}
			DBDataPermission dbdataPermission = target as DBDataPermission;
			bool flag = dbdataPermission.IsUnrestricted();
			if (!flag && !this.IsUnrestricted() && (!this.AllowBlankPassword || dbdataPermission.AllowBlankPassword) && (this._keyvalues == null || dbdataPermission._keyvaluetree != null))
			{
				flag = true;
				if (this._keyvalues != null)
				{
					foreach (object obj in this._keyvalues)
					{
						DBConnectionString parsetable = (DBConnectionString)obj;
						if (!dbdataPermission._keyvaluetree.CheckValueForKeyPermit(parsetable))
						{
							flag = false;
							break;
						}
					}
				}
			}
			return flag;
		}

		/// <summary>Returns a value indicating whether the permission can be represented as unrestricted without any knowledge of the permission semantics.</summary>
		/// <returns>
		///   <see langword="true" /> if the permission can be represented as unrestricted.</returns>
		public bool IsUnrestricted()
		{
			return this._isUnrestricted;
		}

		/// <summary>Returns a new permission object that is the union of the current and specified permission objects.</summary>
		/// <param name="target">A permission object to combine with the current permission object. It must be of the same type as the current permission object.</param>
		/// <returns>A new permission object that represents the union of the current permission object and the specified permission object.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> object is not the same type as the current permission object.</exception>
		public override IPermission Union(IPermission target)
		{
			if (target == null)
			{
				return this.Copy();
			}
			if (target.GetType() != base.GetType())
			{
				throw ADP.PermissionTypeMismatch();
			}
			if (this.IsUnrestricted())
			{
				return this.Copy();
			}
			DBDataPermission dbdataPermission = (DBDataPermission)target.Copy();
			if (!dbdataPermission.IsUnrestricted())
			{
				dbdataPermission._allowBlankPassword |= this.AllowBlankPassword;
				if (this._keyvalues != null)
				{
					foreach (object obj in this._keyvalues)
					{
						DBConnectionString entry = (DBConnectionString)obj;
						dbdataPermission.AddPermissionEntry(entry);
					}
				}
			}
			if (!dbdataPermission.IsEmpty())
			{
				return dbdataPermission;
			}
			return null;
		}

		private string DecodeXmlValue(string value)
		{
			if (value != null && 0 < value.Length)
			{
				value = value.Replace("&quot;", "\"");
				value = value.Replace("&apos;", "'");
				value = value.Replace("&lt;", "<");
				value = value.Replace("&gt;", ">");
				value = value.Replace("&amp;", "&");
			}
			return value;
		}

		private string EncodeXmlValue(string value)
		{
			if (value != null && 0 < value.Length)
			{
				value = value.Replace('\0', ' ');
				value = value.Trim();
				value = value.Replace("&", "&amp;");
				value = value.Replace(">", "&gt;");
				value = value.Replace("<", "&lt;");
				value = value.Replace("'", "&apos;");
				value = value.Replace("\"", "&quot;");
			}
			return value;
		}

		/// <summary>Reconstructs a security object with a specified state from an XML encoding.</summary>
		/// <param name="securityElement">The XML encoding to use to reconstruct the security object.</param>
		public override void FromXml(SecurityElement securityElement)
		{
			if (securityElement == null)
			{
				throw ADP.ArgumentNull("securityElement");
			}
			string tag = securityElement.Tag;
			if (!tag.Equals("Permission") && !tag.Equals("IPermission"))
			{
				throw ADP.NotAPermissionElement();
			}
			string text = securityElement.Attribute("version");
			if (text != null && !text.Equals("1"))
			{
				throw ADP.InvalidXMLBadVersion();
			}
			string text2 = securityElement.Attribute("Unrestricted");
			this._isUnrestricted = (text2 != null && bool.Parse(text2));
			this.Clear();
			if (!this._isUnrestricted)
			{
				string text3 = securityElement.Attribute("AllowBlankPassword");
				this._allowBlankPassword = (text3 != null && bool.Parse(text3));
				ArrayList children = securityElement.Children;
				if (children == null)
				{
					return;
				}
				using (IEnumerator enumerator = children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						SecurityElement securityElement2 = (SecurityElement)obj;
						tag = securityElement2.Tag;
						if ("add" == tag || (tag != null && "add" == tag.ToLower(CultureInfo.InvariantCulture)))
						{
							string text4 = securityElement2.Attribute("ConnectionString");
							string text5 = securityElement2.Attribute("KeyRestrictions");
							string text6 = securityElement2.Attribute("KeyRestrictionBehavior");
							KeyRestrictionBehavior behavior = KeyRestrictionBehavior.AllowOnly;
							if (text6 != null)
							{
								behavior = (KeyRestrictionBehavior)Enum.Parse(typeof(KeyRestrictionBehavior), text6, true);
							}
							text4 = this.DecodeXmlValue(text4);
							text5 = this.DecodeXmlValue(text5);
							this.Add(text4, text5, behavior);
						}
					}
					return;
				}
			}
			this._allowBlankPassword = false;
		}

		/// <summary>Creates an XML encoding of the security object and its current state.</summary>
		/// <returns>An XML encoding of the security object, including any state information.</returns>
		public override SecurityElement ToXml()
		{
			Type type = base.GetType();
			SecurityElement securityElement = new SecurityElement("IPermission");
			securityElement.AddAttribute("class", type.AssemblyQualifiedName.Replace('"', '\''));
			securityElement.AddAttribute("version", "1");
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				securityElement.AddAttribute("AllowBlankPassword", this._allowBlankPassword.ToString(CultureInfo.InvariantCulture));
				if (this._keyvalues != null)
				{
					foreach (object obj in this._keyvalues)
					{
						DBConnectionString dbconnectionString = (DBConnectionString)obj;
						SecurityElement securityElement2 = new SecurityElement("add");
						string text = dbconnectionString.ConnectionString;
						text = this.EncodeXmlValue(text);
						if (!ADP.IsEmpty(text))
						{
							securityElement2.AddAttribute("ConnectionString", text);
						}
						text = dbconnectionString.Restrictions;
						text = this.EncodeXmlValue(text);
						if (text == null)
						{
							text = ADP.StrEmpty;
						}
						securityElement2.AddAttribute("KeyRestrictions", text);
						text = dbconnectionString.Behavior.ToString();
						securityElement2.AddAttribute("KeyRestrictionBehavior", text);
						securityElement.AddChild(securityElement2);
					}
				}
			}
			return securityElement;
		}

		private bool _isUnrestricted;

		private bool _allowBlankPassword;

		private NameValuePermission _keyvaluetree;

		private ArrayList _keyvalues;

		private static class XmlStr
		{
			internal const string _class = "class";

			internal const string _IPermission = "IPermission";

			internal const string _Permission = "Permission";

			internal const string _Unrestricted = "Unrestricted";

			internal const string _AllowBlankPassword = "AllowBlankPassword";

			internal const string _true = "true";

			internal const string _Version = "version";

			internal const string _VersionNumber = "1";

			internal const string _add = "add";

			internal const string _ConnectionString = "ConnectionString";

			internal const string _KeyRestrictions = "KeyRestrictions";

			internal const string _KeyRestrictionBehavior = "KeyRestrictionBehavior";
		}
	}
}
