using System;
using System.Collections;

namespace System.Security.Permissions
{
	/// <summary>Allows control of code access security permissions.</summary>
	[Serializable]
	public abstract class ResourcePermissionBase : CodeAccessPermission, IUnrestrictedPermission
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.ResourcePermissionBase" /> class.</summary>
		protected ResourcePermissionBase()
		{
			this._list = new ArrayList();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.ResourcePermissionBase" /> class with the specified level of access to resources at creation.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="state" /> parameter is not a valid value of <see cref="T:System.Security.Permissions.PermissionState" />.</exception>
		protected ResourcePermissionBase(PermissionState state) : this()
		{
			PermissionHelper.CheckPermissionState(state, true);
			this._unrestricted = (state == PermissionState.Unrestricted);
		}

		/// <summary>Gets or sets an enumeration value that describes the types of access that you are giving the resource.</summary>
		/// <returns>An enumeration value that is derived from <see cref="T:System.Type" /> and describes the types of access that you are giving the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The property value is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The property value is not an enumeration value.</exception>
		protected Type PermissionAccessType
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("PermissionAccessType");
				}
				if (!value.IsEnum)
				{
					throw new ArgumentException("!Enum", "PermissionAccessType");
				}
				this._type = value;
			}
		}

		/// <summary>Gets or sets an array of strings that identify the resource you are protecting.</summary>
		/// <returns>An array of strings that identify the resource you are trying to protect.</returns>
		/// <exception cref="T:System.ArgumentNullException">The property value is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The length of the array is 0.</exception>
		protected string[] TagNames
		{
			get
			{
				return this._tags;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("TagNames");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Length==0", "TagNames");
				}
				this._tags = value;
			}
		}

		/// <summary>Adds a permission entry to the permission.</summary>
		/// <param name="entry">The <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> to add.</param>
		/// <exception cref="T:System.ArgumentNullException">The specified <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The number of elements in the <see cref="P:System.Security.Permissions.ResourcePermissionBaseEntry.PermissionAccessPath" /> property is not equal to the number of elements in the <see cref="P:System.Security.Permissions.ResourcePermissionBase.TagNames" /> property.  
		///  -or-  
		///  The <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> is already included in the permission.</exception>
		protected void AddPermissionAccess(ResourcePermissionBaseEntry entry)
		{
			this.CheckEntry(entry);
			if (this.Exists(entry))
			{
				throw new InvalidOperationException(Locale.GetText("Entry already exists."));
			}
			this._list.Add(entry);
		}

		/// <summary>Clears the permission of the added permission entries.</summary>
		protected void Clear()
		{
			this._list.Clear();
		}

		/// <summary>Creates and returns an identical copy of the current permission object.</summary>
		/// <returns>A copy of the current permission object.</returns>
		public override IPermission Copy()
		{
			ResourcePermissionBase resourcePermissionBase = ResourcePermissionBase.CreateFromType(base.GetType(), this._unrestricted);
			if (this._tags != null)
			{
				resourcePermissionBase._tags = (string[])this._tags.Clone();
			}
			resourcePermissionBase._type = this._type;
			resourcePermissionBase._list.AddRange(this._list);
			return resourcePermissionBase;
		}

		/// <summary>Reconstructs a security object with a specified state from an XML encoding.</summary>
		/// <param name="securityElement">The XML encoding to use to reconstruct the security object.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="securityElement" /> parameter is not a valid permission element.  
		///  -or-  
		///  The version number of the <paramref name="securityElement" /> parameter is not supported.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="securityElement" /> parameter is <see langword="null" />.</exception>
		[MonoTODO("incomplete - need more test")]
		public override void FromXml(SecurityElement securityElement)
		{
			if (securityElement == null)
			{
				throw new ArgumentNullException("securityElement");
			}
			CodeAccessPermission.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			this._list.Clear();
			this._unrestricted = PermissionHelper.IsUnrestricted(securityElement);
			if (securityElement.Children == null || securityElement.Children.Count < 1)
			{
				return;
			}
			string[] array = new string[1];
			foreach (object obj in securityElement.Children)
			{
				SecurityElement securityElement2 = (SecurityElement)obj;
				array[0] = securityElement2.Attribute("name");
				ResourcePermissionBaseEntry entry = new ResourcePermissionBaseEntry((int)Enum.Parse(this.PermissionAccessType, securityElement2.Attribute("access")), array);
				this.AddPermissionAccess(entry);
			}
		}

		/// <summary>Returns an array of the <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> objects added to this permission.</summary>
		/// <returns>An array of <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> objects that were added to this permission.</returns>
		protected ResourcePermissionBaseEntry[] GetPermissionEntries()
		{
			ResourcePermissionBaseEntry[] array = new ResourcePermissionBaseEntry[this._list.Count];
			this._list.CopyTo(array, 0);
			return array;
		}

		/// <summary>Creates and returns a permission object that is the intersection of the current permission object and a target permission object.</summary>
		/// <param name="target">A permission object of the same type as the current permission object.</param>
		/// <returns>A new permission object that represents the intersection of the current object and the specified target. This object is <see langword="null" /> if the intersection is empty.</returns>
		/// <exception cref="T:System.ArgumentException">The target permission object is not of the same type as the current permission object.</exception>
		public override IPermission Intersect(IPermission target)
		{
			ResourcePermissionBase resourcePermissionBase = this.Cast(target);
			if (resourcePermissionBase == null)
			{
				return null;
			}
			bool flag = this.IsUnrestricted();
			bool flag2 = resourcePermissionBase.IsUnrestricted();
			if (this.IsEmpty() && !flag2)
			{
				return null;
			}
			if (resourcePermissionBase.IsEmpty() && !flag)
			{
				return null;
			}
			ResourcePermissionBase resourcePermissionBase2 = ResourcePermissionBase.CreateFromType(base.GetType(), flag && flag2);
			foreach (object obj in this._list)
			{
				ResourcePermissionBaseEntry entry = (ResourcePermissionBaseEntry)obj;
				if (flag2 || resourcePermissionBase.Exists(entry))
				{
					resourcePermissionBase2.AddPermissionAccess(entry);
				}
			}
			foreach (object obj2 in resourcePermissionBase._list)
			{
				ResourcePermissionBaseEntry entry2 = (ResourcePermissionBaseEntry)obj2;
				if ((flag || this.Exists(entry2)) && !resourcePermissionBase2.Exists(entry2))
				{
					resourcePermissionBase2.AddPermissionAccess(entry2);
				}
			}
			return resourcePermissionBase2;
		}

		/// <summary>Determines whether the current permission object is a subset of the specified permission.</summary>
		/// <param name="target">A permission object that is to be tested for the subset relationship.</param>
		/// <returns>
		///   <see langword="true" /> if the current permission object is a subset of the specified permission object; otherwise, <see langword="false" />.</returns>
		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return true;
			}
			ResourcePermissionBase resourcePermissionBase = target as ResourcePermissionBase;
			if (resourcePermissionBase == null)
			{
				return false;
			}
			if (resourcePermissionBase.IsUnrestricted())
			{
				return true;
			}
			if (this.IsUnrestricted())
			{
				return resourcePermissionBase.IsUnrestricted();
			}
			foreach (object obj in this._list)
			{
				ResourcePermissionBaseEntry entry = (ResourcePermissionBaseEntry)obj;
				if (!resourcePermissionBase.Exists(entry))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Gets a value indicating whether the permission is unrestricted.</summary>
		/// <returns>
		///   <see langword="true" /> if permission is unrestricted; otherwise, <see langword="false" />.</returns>
		public bool IsUnrestricted()
		{
			return this._unrestricted;
		}

		/// <summary>Removes a permission entry from the permission.</summary>
		/// <param name="entry">The <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">The specified <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The number of elements in the <see cref="P:System.Security.Permissions.ResourcePermissionBaseEntry.PermissionAccessPath" /> property is not equal to the number of elements in the <see cref="P:System.Security.Permissions.ResourcePermissionBase.TagNames" /> property.  
		///  -or-  
		///  The <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> is not in the permission.</exception>
		protected void RemovePermissionAccess(ResourcePermissionBaseEntry entry)
		{
			this.CheckEntry(entry);
			for (int i = 0; i < this._list.Count; i++)
			{
				ResourcePermissionBaseEntry entry2 = (ResourcePermissionBaseEntry)this._list[i];
				if (this.Equals(entry, entry2))
				{
					this._list.RemoveAt(i);
					return;
				}
			}
			throw new InvalidOperationException(Locale.GetText("Entry doesn't exists."));
		}

		/// <summary>Creates and returns an XML encoding of the security object and its current state.</summary>
		/// <returns>An XML encoding of the security object, including any state information.</returns>
		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(base.GetType(), 1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				foreach (object obj in this._list)
				{
					ResourcePermissionBaseEntry resourcePermissionBaseEntry = (ResourcePermissionBaseEntry)obj;
					SecurityElement securityElement2 = securityElement;
					string text = null;
					if (this.PermissionAccessType != null)
					{
						text = Enum.Format(this.PermissionAccessType, resourcePermissionBaseEntry.PermissionAccess, "g");
					}
					for (int i = 0; i < this._tags.Length; i++)
					{
						SecurityElement securityElement3 = new SecurityElement(this._tags[i]);
						securityElement3.AddAttribute("name", resourcePermissionBaseEntry.PermissionAccessPath[i]);
						if (text != null)
						{
							securityElement3.AddAttribute("access", text);
						}
						securityElement2.AddChild(securityElement3);
					}
				}
			}
			return securityElement;
		}

		/// <summary>Creates a permission object that combines the current permission object and the target permission object.</summary>
		/// <param name="target">A permission object to combine with the current permission object. It must be of the same type as the current permission object.</param>
		/// <returns>A new permission object that represents the union of the current permission object and the specified permission object.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> permission object is not of the same type as the current permission object.</exception>
		public override IPermission Union(IPermission target)
		{
			ResourcePermissionBase resourcePermissionBase = this.Cast(target);
			if (resourcePermissionBase == null)
			{
				return this.Copy();
			}
			if (this.IsEmpty() && resourcePermissionBase.IsEmpty())
			{
				return null;
			}
			if (resourcePermissionBase.IsEmpty())
			{
				return this.Copy();
			}
			if (this.IsEmpty())
			{
				return resourcePermissionBase.Copy();
			}
			bool flag = this.IsUnrestricted() || resourcePermissionBase.IsUnrestricted();
			ResourcePermissionBase resourcePermissionBase2 = ResourcePermissionBase.CreateFromType(base.GetType(), flag);
			if (!flag)
			{
				foreach (object obj in this._list)
				{
					ResourcePermissionBaseEntry entry = (ResourcePermissionBaseEntry)obj;
					resourcePermissionBase2.AddPermissionAccess(entry);
				}
				foreach (object obj2 in resourcePermissionBase._list)
				{
					ResourcePermissionBaseEntry entry2 = (ResourcePermissionBaseEntry)obj2;
					if (!resourcePermissionBase2.Exists(entry2))
					{
						resourcePermissionBase2.AddPermissionAccess(entry2);
					}
				}
			}
			return resourcePermissionBase2;
		}

		private bool IsEmpty()
		{
			return !this._unrestricted && this._list.Count == 0;
		}

		private ResourcePermissionBase Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			ResourcePermissionBase resourcePermissionBase = target as ResourcePermissionBase;
			if (resourcePermissionBase == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, typeof(ResourcePermissionBase));
			}
			return resourcePermissionBase;
		}

		internal void CheckEntry(ResourcePermissionBaseEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (entry.PermissionAccessPath == null || entry.PermissionAccessPath.Length != this._tags.Length)
			{
				throw new InvalidOperationException(Locale.GetText("Entry doesn't match TagNames"));
			}
		}

		internal bool Equals(ResourcePermissionBaseEntry entry1, ResourcePermissionBaseEntry entry2)
		{
			if (entry1.PermissionAccess != entry2.PermissionAccess)
			{
				return false;
			}
			if (entry1.PermissionAccessPath.Length != entry2.PermissionAccessPath.Length)
			{
				return false;
			}
			for (int i = 0; i < entry1.PermissionAccessPath.Length; i++)
			{
				if (entry1.PermissionAccessPath[i] != entry2.PermissionAccessPath[i])
				{
					return false;
				}
			}
			return true;
		}

		internal bool Exists(ResourcePermissionBaseEntry entry)
		{
			if (this._list.Count == 0)
			{
				return false;
			}
			foreach (object obj in this._list)
			{
				ResourcePermissionBaseEntry entry2 = (ResourcePermissionBaseEntry)obj;
				if (this.Equals(entry2, entry))
				{
					return true;
				}
			}
			return false;
		}

		internal static void ValidateMachineName(string name)
		{
			if (name == null || name.Length == 0 || name.IndexOfAny(ResourcePermissionBase.invalidChars) != -1)
			{
				string text = Locale.GetText("Invalid machine name '{0}'.");
				if (name == null)
				{
					name = "(null)";
				}
				throw new ArgumentException(string.Format(text, name), "MachineName");
			}
		}

		internal static ResourcePermissionBase CreateFromType(Type type, bool unrestricted)
		{
			return (ResourcePermissionBase)Activator.CreateInstance(type, new object[]
			{
				unrestricted ? PermissionState.Unrestricted : PermissionState.None
			});
		}

		private const int version = 1;

		private ArrayList _list;

		private bool _unrestricted;

		private Type _type;

		private string[] _tags;

		/// <summary>Specifies the character to be used to represent the any wildcard character.</summary>
		public const string Any = "*";

		/// <summary>Specifies the character to be used to represent a local reference.</summary>
		public const string Local = ".";

		private static char[] invalidChars = new char[]
		{
			'\t',
			'\n',
			'\v',
			'\f',
			'\r',
			' ',
			'\\',
			'Š'
		};
	}
}
