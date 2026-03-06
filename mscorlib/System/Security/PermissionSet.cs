using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using Unity;

namespace System.Security
{
	/// <summary>Represents a collection that can contain many different types of permissions.</summary>
	[ComVisible(true)]
	[MonoTODO("CAS support is experimental (and unsupported).")]
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293")]
	[Serializable]
	public class PermissionSet : ISecurityEncodable, ICollection, IEnumerable, IStackWalk, IDeserializationCallback
	{
		internal PermissionSet()
		{
			this.list = new ArrayList();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.PermissionSet" /> class with the specified <see cref="T:System.Security.Permissions.PermissionState" />.</summary>
		/// <param name="state">One of the enumeration values that specifies the permission set's access to resources.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="state" /> parameter is not a valid <see cref="T:System.Security.Permissions.PermissionState" />.</exception>
		public PermissionSet(PermissionState state) : this()
		{
			this.state = CodeAccessPermission.CheckPermissionState(state, true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.PermissionSet" /> class with initial values taken from the <paramref name="permSet" /> parameter.</summary>
		/// <param name="permSet">The set from which to take the value of the new <see cref="T:System.Security.PermissionSet" />, or <see langword="null" /> to create an empty <see cref="T:System.Security.PermissionSet" />.</param>
		public PermissionSet(PermissionSet permSet) : this()
		{
			if (permSet != null)
			{
				this.state = permSet.state;
				foreach (object obj in permSet.list)
				{
					IPermission value = (IPermission)obj;
					this.list.Add(value);
				}
			}
		}

		internal PermissionSet(string xml) : this()
		{
			this.state = PermissionState.None;
			if (xml != null)
			{
				SecurityElement et = SecurityElement.FromString(xml);
				this.FromXml(et);
			}
		}

		internal PermissionSet(IPermission perm) : this()
		{
			if (perm != null)
			{
				this.list.Add(perm);
			}
		}

		/// <summary>Adds a specified permission to the <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <param name="perm">The permission to add.</param>
		/// <returns>The union of the permission added and any permission of the same type that already exists in the <see cref="T:System.Security.PermissionSet" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The method is called from a <see cref="T:System.Security.ReadOnlyPermissionSet" />.</exception>
		public IPermission AddPermission(IPermission perm)
		{
			if (perm == null || this._readOnly)
			{
				return perm;
			}
			if (this.state == PermissionState.Unrestricted)
			{
				return (IPermission)Activator.CreateInstance(perm.GetType(), PermissionSet.psUnrestricted);
			}
			IPermission permission = this.RemovePermission(perm.GetType());
			if (permission != null)
			{
				perm = perm.Union(permission);
			}
			this.list.Add(perm);
			return perm;
		}

		/// <summary>Declares that the calling code can access the resource protected by a permission demand through the code that calls this method, even if callers higher in the stack have not been granted permission to access the resource. Using <see cref="M:System.Security.PermissionSet.Assert" /> can create security vulnerabilities.</summary>
		/// <exception cref="T:System.Security.SecurityException">The <see cref="T:System.Security.PermissionSet" /> instance asserted has not been granted to the asserting code.  
		///  -or-  
		///  There is already an active <see cref="M:System.Security.PermissionSet.Assert" /> for the current frame.</exception>
		[SecuritySafeCritical]
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		[SecurityPermission(SecurityAction.Demand, Assertion = true)]
		public void Assert()
		{
			int num = this.Count;
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				if (permission is IStackWalk)
				{
					if (!SecurityManager.IsGranted(permission))
					{
						return;
					}
				}
				else
				{
					num--;
				}
			}
			if (SecurityManager.SecurityEnabled && num > 0)
			{
				throw new NotSupportedException("Currently only declarative Assert are supported.");
			}
		}

		internal void Clear()
		{
			this.list.Clear();
		}

		/// <summary>Creates a copy of the <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <returns>A copy of the <see cref="T:System.Security.PermissionSet" />.</returns>
		public virtual PermissionSet Copy()
		{
			return new PermissionSet(this);
		}

		/// <summary>Copies the permission objects of the set to the indicated location in an <see cref="T:System.Array" />.</summary>
		/// <param name="array">The target array to which to copy.</param>
		/// <param name="index">The starting position in the array to begin copying (zero based).</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="array" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="array" /> parameter has more than one dimension.</exception>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="index" /> parameter is out of the range of the <paramref name="array" /> parameter.</exception>
		public virtual void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (this.list.Count > 0)
			{
				if (array.Rank > 1)
				{
					throw new ArgumentException(Locale.GetText("Array has more than one dimension"));
				}
				if (index < 0 || index >= array.Length)
				{
					throw new IndexOutOfRangeException("index");
				}
				this.list.CopyTo(array, index);
			}
		}

		/// <summary>Forces a <see cref="T:System.Security.SecurityException" /> at run time if all callers higher in the call stack have not been granted the permissions specified by the current instance.</summary>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call chain does not have the permission demanded.</exception>
		[SecuritySafeCritical]
		public void Demand()
		{
			if (this.IsEmpty())
			{
				return;
			}
			int count = this.list.Count;
			if (this._ignored == null || this._ignored.Length != count)
			{
				this._ignored = new bool[count];
			}
			bool flag = this.IsUnrestricted();
			for (int i = 0; i < count; i++)
			{
				IPermission permission = (IPermission)this.list[i];
				if (permission.GetType().IsSubclassOf(typeof(CodeAccessPermission)))
				{
					this._ignored[i] = false;
					flag = true;
				}
				else
				{
					this._ignored[i] = true;
					permission.Demand();
				}
			}
			if (flag && SecurityManager.SecurityEnabled)
			{
				this.CasOnlyDemand(this._declsec ? 5 : 3);
			}
		}

		internal void CasOnlyDemand(int skip)
		{
			if (this._ignored == null)
			{
				this._ignored = new bool[this.list.Count];
			}
		}

		/// <summary>Causes any <see cref="M:System.Security.PermissionSet.Demand" /> that passes through the calling code for a permission that has an intersection with a permission of a type contained in the current <see cref="T:System.Security.PermissionSet" /> to fail.</summary>
		/// <exception cref="T:System.Security.SecurityException">A previous call to <see cref="M:System.Security.PermissionSet.Deny" /> has already restricted the permissions for the current stack frame.</exception>
		[SecuritySafeCritical]
		[Obsolete("Deny is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Deny()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			using (IEnumerator enumerator = this.list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((IPermission)enumerator.Current) is IStackWalk)
					{
						throw new NotSupportedException("Currently only declarative Deny are supported.");
					}
				}
			}
		}

		/// <summary>Reconstructs a security object with a specified state from an XML encoding.</summary>
		/// <param name="et">The XML encoding to use to reconstruct the security object.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="et" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="et" /> parameter is not a valid permission element.  
		///  -or-  
		///  The <paramref name="et" /> parameter's version number is not supported.</exception>
		public virtual void FromXml(SecurityElement et)
		{
			if (et == null)
			{
				throw new ArgumentNullException("et");
			}
			if (et.Tag != "PermissionSet")
			{
				throw new ArgumentException(string.Format("Invalid tag {0} expected {1}", et.Tag, "PermissionSet"), "et");
			}
			this.list.Clear();
			if (CodeAccessPermission.IsUnrestricted(et))
			{
				this.state = PermissionState.Unrestricted;
				return;
			}
			this.state = PermissionState.None;
			if (et.Children != null)
			{
				foreach (object obj in et.Children)
				{
					SecurityElement securityElement = (SecurityElement)obj;
					string text = securityElement.Attribute("class");
					if (text == null)
					{
						throw new ArgumentException(Locale.GetText("No permission class is specified."));
					}
					if (this.Resolver != null)
					{
						text = this.Resolver.ResolveClassName(text);
					}
					this.list.Add(PermissionBuilder.Create(text, securityElement));
				}
			}
		}

		/// <summary>Returns an enumerator for the permissions of the set.</summary>
		/// <returns>An enumerator object for the permissions of the set.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		/// <summary>Determines whether the current <see cref="T:System.Security.PermissionSet" /> is a subset of the specified <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <param name="target">The permission set to test for the subset relationship. This must be either a <see cref="T:System.Security.PermissionSet" /> or a <see cref="T:System.Security.NamedPermissionSet" />.</param>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Security.PermissionSet" /> is a subset of the <paramref name="target" /> parameter; otherwise, <see langword="false" />.</returns>
		public bool IsSubsetOf(PermissionSet target)
		{
			if (target == null || target.IsEmpty())
			{
				return this.IsEmpty();
			}
			if (target.IsUnrestricted())
			{
				return true;
			}
			if (this.IsUnrestricted())
			{
				return false;
			}
			if (this.IsUnrestricted() && (target == null || !target.IsUnrestricted()))
			{
				return false;
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				Type type = permission.GetType();
				IPermission target2;
				if (target.IsUnrestricted() && permission is CodeAccessPermission && permission is IUnrestrictedPermission)
				{
					target2 = (IPermission)Activator.CreateInstance(type, PermissionSet.psUnrestricted);
				}
				else
				{
					target2 = target.GetPermission(type);
				}
				if (!permission.IsSubsetOf(target2))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Causes any <see cref="M:System.Security.PermissionSet.Demand" /> that passes through the calling code for any <see cref="T:System.Security.PermissionSet" /> that is not a subset of the current <see cref="T:System.Security.PermissionSet" /> to fail.</summary>
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		[SecuritySafeCritical]
		public void PermitOnly()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			using (IEnumerator enumerator = this.list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((IPermission)enumerator.Current) is IStackWalk)
					{
						throw new NotSupportedException("Currently only declarative Deny are supported.");
					}
				}
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Security.PermissionSet" /> contains permissions that are not derived from <see cref="T:System.Security.CodeAccessPermission" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.PermissionSet" /> contains permissions that are not derived from <see cref="T:System.Security.CodeAccessPermission" />; otherwise, <see langword="false" />.</returns>
		public bool ContainsNonCodeAccessPermissions()
		{
			if (this.list.Count > 0)
			{
				using (IEnumerator enumerator = this.list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!((IPermission)enumerator.Current).GetType().IsSubclassOf(typeof(CodeAccessPermission)))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		/// <summary>Converts an encoded <see cref="T:System.Security.PermissionSet" /> from one XML encoding format to another XML encoding format.</summary>
		/// <param name="inFormat">A string representing one of the following encoding formats: ASCII, Unicode, or Binary. Possible values are "XMLASCII" or "XML", "XMLUNICODE", and "BINARY".</param>
		/// <param name="inData">An XML-encoded permission set.</param>
		/// <param name="outFormat">A string representing one of the following encoding formats: ASCII, Unicode, or Binary. Possible values are "XMLASCII" or "XML", "XMLUNICODE", and "BINARY".</param>
		/// <returns>An encrypted permission set with the specified output format.</returns>
		/// <exception cref="T:System.NotImplementedException">In all cases.</exception>
		public static byte[] ConvertPermissionSet(string inFormat, byte[] inData, string outFormat)
		{
			if (inFormat == null)
			{
				throw new ArgumentNullException("inFormat");
			}
			if (outFormat == null)
			{
				throw new ArgumentNullException("outFormat");
			}
			if (inData == null)
			{
				return null;
			}
			if (inFormat == outFormat)
			{
				return inData;
			}
			PermissionSet permissionSet = null;
			if (inFormat == "BINARY")
			{
				if (outFormat.StartsWith("XML"))
				{
					using (MemoryStream memoryStream = new MemoryStream(inData))
					{
						permissionSet = (PermissionSet)new BinaryFormatter().Deserialize(memoryStream);
						memoryStream.Close();
					}
					string s = permissionSet.ToString();
					if (outFormat == "XML" || outFormat == "XMLASCII")
					{
						return Encoding.ASCII.GetBytes(s);
					}
					if (outFormat == "XMLUNICODE")
					{
						return Encoding.Unicode.GetBytes(s);
					}
				}
			}
			else
			{
				if (!inFormat.StartsWith("XML"))
				{
					return null;
				}
				if (outFormat == "BINARY")
				{
					string text = null;
					if (!(inFormat == "XML") && !(inFormat == "XMLASCII"))
					{
						if (inFormat == "XMLUNICODE")
						{
							text = Encoding.Unicode.GetString(inData);
						}
					}
					else
					{
						text = Encoding.ASCII.GetString(inData);
					}
					if (text != null)
					{
						permissionSet = new PermissionSet(PermissionState.None);
						permissionSet.FromXml(SecurityElement.FromString(text));
						MemoryStream memoryStream2 = new MemoryStream();
						new BinaryFormatter().Serialize(memoryStream2, permissionSet);
						memoryStream2.Close();
						return memoryStream2.ToArray();
					}
				}
				else if (outFormat.StartsWith("XML"))
				{
					throw new XmlSyntaxException(string.Format(Locale.GetText("Can't convert from {0} to {1}"), inFormat, outFormat));
				}
			}
			throw new SerializationException(string.Format(Locale.GetText("Unknown output format {0}."), outFormat));
		}

		/// <summary>Gets a permission object of the specified type, if it exists in the set.</summary>
		/// <param name="permClass">The type of the desired permission object.</param>
		/// <returns>A copy of the permission object of the type specified by the <paramref name="permClass" /> parameter contained in the <see cref="T:System.Security.PermissionSet" />, or <see langword="null" /> if none exists.</returns>
		public IPermission GetPermission(Type permClass)
		{
			if (permClass == null || this.list.Count == 0)
			{
				return null;
			}
			foreach (object obj in this.list)
			{
				if (obj != null && obj.GetType().Equals(permClass))
				{
					return (IPermission)obj;
				}
			}
			return null;
		}

		/// <summary>Creates and returns a permission set that is the intersection of the current <see cref="T:System.Security.PermissionSet" /> and the specified <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <param name="other">A permission set to intersect with the current <see cref="T:System.Security.PermissionSet" />.</param>
		/// <returns>A new permission set that represents the intersection of the current <see cref="T:System.Security.PermissionSet" /> and the specified target. This object is <see langword="null" /> if the intersection is empty.</returns>
		public PermissionSet Intersect(PermissionSet other)
		{
			if (other == null || other.IsEmpty() || this.IsEmpty())
			{
				return null;
			}
			PermissionState permissionState = PermissionState.None;
			if (this.IsUnrestricted() && other.IsUnrestricted())
			{
				permissionState = PermissionState.Unrestricted;
			}
			PermissionSet permissionSet;
			if (permissionState == PermissionState.Unrestricted)
			{
				permissionSet = new PermissionSet(permissionState);
			}
			else if (this.IsUnrestricted())
			{
				permissionSet = other.Copy();
			}
			else if (other.IsUnrestricted())
			{
				permissionSet = this.Copy();
			}
			else
			{
				permissionSet = new PermissionSet(permissionState);
				this.InternalIntersect(permissionSet, this, other, false);
			}
			return permissionSet;
		}

		internal void InternalIntersect(PermissionSet intersect, PermissionSet a, PermissionSet b, bool unrestricted)
		{
			foreach (object obj in b.list)
			{
				IPermission permission = (IPermission)obj;
				IPermission permission2 = a.GetPermission(permission.GetType());
				if (permission2 != null)
				{
					intersect.AddPermission(permission.Intersect(permission2));
				}
				else if (unrestricted)
				{
					intersect.AddPermission(permission);
				}
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Security.PermissionSet" /> is empty.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.PermissionSet" /> is empty; otherwise, <see langword="false" />.</returns>
		public bool IsEmpty()
		{
			if (this.state == PermissionState.Unrestricted)
			{
				return false;
			}
			if (this.list == null || this.list.Count == 0)
			{
				return true;
			}
			using (IEnumerator enumerator = this.list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!((IPermission)enumerator.Current).IsSubsetOf(null))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>Determines whether the <see cref="T:System.Security.PermissionSet" /> is <see langword="Unrestricted" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.PermissionSet" /> is <see langword="Unrestricted" />; otherwise, <see langword="false" />.</returns>
		public bool IsUnrestricted()
		{
			return this.state == PermissionState.Unrestricted;
		}

		/// <summary>Removes a permission of a certain type from the set.</summary>
		/// <param name="permClass">The type of permission to delete.</param>
		/// <returns>The permission removed from the set.</returns>
		/// <exception cref="T:System.InvalidOperationException">The method is called from a <see cref="T:System.Security.ReadOnlyPermissionSet" />.</exception>
		public IPermission RemovePermission(Type permClass)
		{
			if (permClass == null || this._readOnly)
			{
				return null;
			}
			foreach (object obj in this.list)
			{
				if (obj.GetType().Equals(permClass))
				{
					this.list.Remove(obj);
					return (IPermission)obj;
				}
			}
			return null;
		}

		/// <summary>Sets a permission to the <see cref="T:System.Security.PermissionSet" />, replacing any existing permission of the same type.</summary>
		/// <param name="perm">The permission to set.</param>
		/// <returns>The set permission.</returns>
		/// <exception cref="T:System.InvalidOperationException">The method is called from a <see cref="T:System.Security.ReadOnlyPermissionSet" />.</exception>
		public IPermission SetPermission(IPermission perm)
		{
			if (perm == null || this._readOnly)
			{
				return perm;
			}
			IUnrestrictedPermission unrestrictedPermission = perm as IUnrestrictedPermission;
			if (unrestrictedPermission == null)
			{
				this.state = PermissionState.None;
			}
			else
			{
				this.state = (unrestrictedPermission.IsUnrestricted() ? this.state : PermissionState.None);
			}
			this.RemovePermission(perm.GetType());
			this.list.Add(perm);
			return perm;
		}

		/// <summary>Returns a string representation of the <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <returns>A representation of the <see cref="T:System.Security.PermissionSet" />.</returns>
		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		/// <summary>Creates an XML encoding of the security object and its current state.</summary>
		/// <returns>An XML encoding of the security object, including any state information.</returns>
		public virtual SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("PermissionSet");
			securityElement.AddAttribute("class", base.GetType().FullName);
			securityElement.AddAttribute("version", 1.ToString());
			if (this.state == PermissionState.Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				securityElement.AddChild(permission.ToXml());
			}
			return securityElement;
		}

		/// <summary>Creates a <see cref="T:System.Security.PermissionSet" /> that is the union of the current <see cref="T:System.Security.PermissionSet" /> and the specified <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <param name="other">The permission set to form the union with the current <see cref="T:System.Security.PermissionSet" />.</param>
		/// <returns>A new permission set that represents the union of the current <see cref="T:System.Security.PermissionSet" /> and the specified <see cref="T:System.Security.PermissionSet" />.</returns>
		public PermissionSet Union(PermissionSet other)
		{
			if (other == null)
			{
				return this.Copy();
			}
			PermissionSet permissionSet = null;
			if (this.IsUnrestricted() || other.IsUnrestricted())
			{
				return new PermissionSet(PermissionState.Unrestricted);
			}
			permissionSet = this.Copy();
			foreach (object obj in other.list)
			{
				IPermission perm = (IPermission)obj;
				permissionSet.AddPermission(perm);
			}
			return permissionSet;
		}

		/// <summary>Gets the number of permission objects contained in the permission set.</summary>
		/// <returns>The number of permission objects contained in the <see cref="T:System.Security.PermissionSet" />.</returns>
		public virtual int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		/// <summary>Gets a value indicating whether the collection is guaranteed to be thread safe.</summary>
		/// <returns>Always <see langword="false" />.</returns>
		public virtual bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		/// <summary>Gets a value indicating whether the collection is read-only.</summary>
		/// <returns>Always <see langword="false" />.</returns>
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets the root object of the current collection.</summary>
		/// <returns>The root object of the current collection.</returns>
		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal bool DeclarativeSecurity
		{
			get
			{
				return this._declsec;
			}
			set
			{
				this._declsec = value;
			}
		}

		/// <summary>Runs when the entire object graph has been deserialized.</summary>
		/// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
		[MonoTODO("may not be required")]
		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		/// <summary>Determines whether the specified <see cref="T:System.Security.PermissionSet" /> or <see cref="T:System.Security.NamedPermissionSet" /> object is equal to the current <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <param name="obj">The object to compare with the current <see cref="T:System.Security.PermissionSet" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is equal to the current <see cref="T:System.Security.PermissionSet" /> object; otherwise, <see langword="false" />.</returns>
		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			PermissionSet permissionSet = obj as PermissionSet;
			if (permissionSet == null)
			{
				return false;
			}
			if (this.state != permissionSet.state)
			{
				return false;
			}
			if (this.list.Count != permissionSet.Count)
			{
				return false;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				bool flag = false;
				int num = 0;
				while (i < permissionSet.list.Count)
				{
					if (this.list[i].Equals(permissionSet.list[num]))
					{
						flag = true;
						break;
					}
					num++;
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Gets a hash code for the <see cref="T:System.Security.PermissionSet" /> object that is suitable for use in hashing algorithms and data structures such as a hash table.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Security.PermissionSet" /> object.</returns>
		[ComVisible(false)]
		public override int GetHashCode()
		{
			if (this.list.Count != 0)
			{
				return base.GetHashCode();
			}
			return (int)this.state;
		}

		/// <summary>Causes any previous <see cref="M:System.Security.CodeAccessPermission.Assert" /> for the current frame to be removed and no longer be in effect.</summary>
		/// <exception cref="T:System.InvalidOperationException">There is no previous <see cref="M:System.Security.CodeAccessPermission.Assert" /> for the current frame.</exception>
		public static void RevertAssert()
		{
			CodeAccessPermission.RevertAssert();
		}

		internal PolicyLevel Resolver
		{
			get
			{
				return this._policyLevel;
			}
			set
			{
				this._policyLevel = value;
			}
		}

		internal void SetReadOnly(bool value)
		{
			this._readOnly = value;
		}

		private bool AllIgnored()
		{
			if (this._ignored == null)
			{
				throw new NotSupportedException("bad bad bad");
			}
			for (int i = 0; i < this._ignored.Length; i++)
			{
				if (!this._ignored[i])
				{
					return false;
				}
			}
			return true;
		}

		internal static PermissionSet CreateFromBinaryFormat(byte[] data)
		{
			if (data == null || data[0] != 46 || data.Length < 2)
			{
				throw new SecurityException(Locale.GetText("Invalid data in 2.0 metadata format."));
			}
			int num = 1;
			int num2 = PermissionSet.ReadEncodedInt(data, ref num);
			PermissionSet permissionSet = new PermissionSet(PermissionState.None);
			for (int i = 0; i < num2; i++)
			{
				IPermission permission = PermissionSet.ProcessAttribute(data, ref num);
				if (permission == null)
				{
					throw new SecurityException(Locale.GetText("Unsupported data found in 2.0 metadata format."));
				}
				permissionSet.AddPermission(permission);
			}
			return permissionSet;
		}

		internal static int ReadEncodedInt(byte[] data, ref int position)
		{
			int result;
			if ((data[position] & 128) == 0)
			{
				result = (int)data[position];
				position++;
			}
			else if ((data[position] & 64) == 0)
			{
				result = ((int)(data[position] & 63) << 8 | (int)data[position + 1]);
				position += 2;
			}
			else
			{
				result = ((int)(data[position] & 31) << 24 | (int)data[position + 1] << 16 | (int)data[position + 2] << 8 | (int)data[position + 3]);
				position += 4;
			}
			return result;
		}

		internal static IPermission ProcessAttribute(byte[] data, ref int position)
		{
			int num = PermissionSet.ReadEncodedInt(data, ref position);
			string @string = Encoding.UTF8.GetString(data, position, num);
			position += num;
			Type type = Type.GetType(@string);
			SecurityAttribute securityAttribute = Activator.CreateInstance(type, PermissionSet.action) as SecurityAttribute;
			if (securityAttribute == null)
			{
				return null;
			}
			PermissionSet.ReadEncodedInt(data, ref position);
			int num2 = PermissionSet.ReadEncodedInt(data, ref position);
			for (int i = 0; i < num2; i++)
			{
				int num3 = position;
				position = num3 + 1;
				byte b = data[num3];
				bool flag;
				if (b != 83)
				{
					if (b != 84)
					{
						return null;
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
				bool flag2 = false;
				num3 = position;
				position = num3 + 1;
				byte b2 = data[num3];
				if (b2 == 29)
				{
					flag2 = true;
					num3 = position;
					position = num3 + 1;
					b2 = data[num3];
				}
				int num4 = PermissionSet.ReadEncodedInt(data, ref position);
				string string2 = Encoding.UTF8.GetString(data, position, num4);
				position += num4;
				int num5 = 1;
				if (flag2)
				{
					num5 = BitConverter.ToInt32(data, position);
					position += 4;
				}
				object[] index = null;
				for (int j = 0; j < num5; j++)
				{
					object value;
					switch (b2)
					{
					case 2:
						num3 = position;
						position = num3 + 1;
						value = Convert.ToBoolean(data[num3]);
						break;
					case 3:
						value = Convert.ToChar(data[position]);
						position += 2;
						break;
					case 4:
						num3 = position;
						position = num3 + 1;
						value = Convert.ToSByte(data[num3]);
						break;
					case 5:
						num3 = position;
						position = num3 + 1;
						value = Convert.ToByte(data[num3]);
						break;
					case 6:
						value = Convert.ToInt16(data[position]);
						position += 2;
						break;
					case 7:
						value = Convert.ToUInt16(data[position]);
						position += 2;
						break;
					case 8:
						value = Convert.ToInt32(data[position]);
						position += 4;
						break;
					case 9:
						value = Convert.ToUInt32(data[position]);
						position += 4;
						break;
					case 10:
						value = Convert.ToInt64(data[position]);
						position += 8;
						break;
					case 11:
						value = Convert.ToUInt64(data[position]);
						position += 8;
						break;
					case 12:
						value = Convert.ToSingle(data[position]);
						position += 4;
						break;
					case 13:
						value = Convert.ToDouble(data[position]);
						position += 8;
						break;
					case 14:
					{
						string text = null;
						if (data[position] != 255)
						{
							int num6 = PermissionSet.ReadEncodedInt(data, ref position);
							text = Encoding.UTF8.GetString(data, position, num6);
							position += num6;
						}
						else
						{
							position++;
						}
						value = text;
						break;
					}
					default:
					{
						if (b2 != 80)
						{
							return null;
						}
						int num7 = PermissionSet.ReadEncodedInt(data, ref position);
						value = Type.GetType(Encoding.UTF8.GetString(data, position, num7));
						position += num7;
						break;
					}
					}
					if (flag)
					{
						type.GetProperty(string2).SetValue(securityAttribute, value, index);
					}
					else
					{
						type.GetField(string2).SetValue(securityAttribute, value);
					}
				}
			}
			return securityAttribute.CreatePermission();
		}

		/// <summary>Adds a specified permission to the <see cref="T:System.Security.PermissionSet" />.</summary>
		/// <param name="perm">The permission to add.</param>
		/// <returns>The union of the permission added and any permission of the same type that already exists in the <see cref="T:System.Security.PermissionSet" />, or <see langword="null" /> if <paramref name="perm" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The method is called from a <see cref="T:System.Security.ReadOnlyPermissionSet" />.</exception>
		protected virtual IPermission AddPermissionImpl(IPermission perm)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Returns an enumerator for the permissions of the set.</summary>
		/// <returns>An enumerator object for the permissions of the set.</returns>
		protected virtual IEnumerator GetEnumeratorImpl()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Gets a permission object of the specified type, if it exists in the set.</summary>
		/// <param name="permClass">The type of the permission object.</param>
		/// <returns>A copy of the permission object, of the type specified by the <paramref name="permClass" /> parameter, contained in the <see cref="T:System.Security.PermissionSet" />, or <see langword="null" /> if none exists.</returns>
		protected virtual IPermission GetPermissionImpl(Type permClass)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Removes a permission of a certain type from the set.</summary>
		/// <param name="permClass">The type of the permission to remove.</param>
		/// <returns>The permission removed from the set.</returns>
		/// <exception cref="T:System.InvalidOperationException">The method is called from a <see cref="T:System.Security.ReadOnlyPermissionSet" />.</exception>
		protected virtual IPermission RemovePermissionImpl(Type permClass)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Sets a permission to the <see cref="T:System.Security.PermissionSet" />, replacing any existing permission of the same type.</summary>
		/// <param name="perm">The permission to set.</param>
		/// <returns>The set permission.</returns>
		/// <exception cref="T:System.InvalidOperationException">The method is called from a <see cref="T:System.Security.ReadOnlyPermissionSet" />.</exception>
		protected virtual IPermission SetPermissionImpl(IPermission perm)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		private const string tagName = "PermissionSet";

		private const int version = 1;

		private static object[] psUnrestricted = new object[]
		{
			PermissionState.Unrestricted
		};

		private PermissionState state;

		private ArrayList list;

		private PolicyLevel _policyLevel;

		private bool _declsec;

		private bool _readOnly;

		private bool[] _ignored;

		private static object[] action = new object[]
		{
			(SecurityAction)0
		};
	}
}
