using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	/// <summary>Provides the ability to control access to native objects without direct manipulation of Access Control Lists (ACLs). Native object types are defined by the <see cref="T:System.Security.AccessControl.ResourceType" /> enumeration.</summary>
	public abstract class NativeObjectSecurity : CommonObjectSecurity
	{
		internal NativeObjectSecurity(CommonSecurityDescriptor securityDescriptor, ResourceType resourceType) : base(securityDescriptor)
		{
			this.resource_type = resourceType;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> class with the specified values.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is a container object.</param>
		/// <param name="resourceType">The type of securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType) : this(isContainer, resourceType, null, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> class by using the specified values.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is a container object.</param>
		/// <param name="resourceType">The type of securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="exceptionFromErrorCode">A delegate implemented by integrators that provides custom exceptions.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : base(isContainer)
		{
			this.exception_from_error_code = exceptionFromErrorCode;
			this.resource_type = resourceType;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> class with the specified values. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is a container object.</param>
		/// <param name="resourceType">The type of securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="handle">The handle of the securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to include in this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object.</param>
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections) : this(isContainer, resourceType, handle, includeSections, null, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> class with the specified values. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is a container object.</param>
		/// <param name="resourceType">The type of securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="name">The name of the securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to include in this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object.</param>
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections) : this(isContainer, resourceType, name, includeSections, null, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> class with the specified values. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is a container object.</param>
		/// <param name="resourceType">The type of securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="handle">The handle of the securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to include in this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object.</param>
		/// <param name="exceptionFromErrorCode">A delegate implemented by integrators that provides custom exceptions.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : this(isContainer, resourceType, exceptionFromErrorCode, exceptionContext)
		{
			this.RaiseExceptionOnFailure(this.InternalGet(handle, includeSections), null, handle, exceptionContext);
			this.ClearAccessControlSectionsModified();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> class with the specified values. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is a container object.</param>
		/// <param name="resourceType">The type of securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="name">The name of the securable object with which the new <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to include in this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object.</param>
		/// <param name="exceptionFromErrorCode">A delegate implemented by integrators that provides custom exceptions.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : this(isContainer, resourceType, exceptionFromErrorCode, exceptionContext)
		{
			this.RaiseExceptionOnFailure(this.InternalGet(name, includeSections), name, null, exceptionContext);
			this.ClearAccessControlSectionsModified();
		}

		private void ClearAccessControlSectionsModified()
		{
			base.WriteLock();
			try
			{
				base.AccessControlSectionsModified = AccessControlSections.None;
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object to permanent storage. We recommend.persist that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="handle">The handle of the securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		/// <exception cref="T:System.IO.FileNotFoundException">The securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated is either a directory or a file, and that directory or file could not be found.</exception>
		protected sealed override void Persist(SafeHandle handle, AccessControlSections includeSections)
		{
			this.Persist(handle, includeSections, null);
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object to permanent storage. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="name">The name of the securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		/// <exception cref="T:System.IO.FileNotFoundException">The securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated is either a directory or a file, and that directory or file could not be found.</exception>
		protected sealed override void Persist(string name, AccessControlSections includeSections)
		{
			this.Persist(name, includeSections, null);
		}

		internal void PersistModifications(SafeHandle handle)
		{
			base.WriteLock();
			try
			{
				this.Persist(handle, base.AccessControlSectionsModified, null);
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object to permanent storage. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="handle">The handle of the securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		/// <exception cref="T:System.IO.FileNotFoundException">The securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated is either a directory or a file, and that directory or file could not be found.</exception>
		protected void Persist(SafeHandle handle, AccessControlSections includeSections, object exceptionContext)
		{
			base.WriteLock();
			try
			{
				this.RaiseExceptionOnFailure(this.InternalSet(handle, includeSections), null, handle, exceptionContext);
				base.AccessControlSectionsModified &= ~includeSections;
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		internal void PersistModifications(string name)
		{
			base.WriteLock();
			try
			{
				this.Persist(name, base.AccessControlSectionsModified, null);
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object to permanent storage. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="name">The name of the securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		/// <exception cref="T:System.IO.FileNotFoundException">The securable object with which this <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated is either a directory or a file, and that directory or file could not be found.</exception>
		protected void Persist(string name, AccessControlSections includeSections, object exceptionContext)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			base.WriteLock();
			try
			{
				this.RaiseExceptionOnFailure(this.InternalSet(name, includeSections), name, null, exceptionContext);
				base.AccessControlSectionsModified &= ~includeSections;
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		internal static Exception DefaultExceptionFromErrorCode(int errorCode, string name, SafeHandle handle, object context)
		{
			switch (errorCode)
			{
			case 2:
				return new FileNotFoundException();
			case 3:
				return new DirectoryNotFoundException();
			case 4:
				break;
			case 5:
				return new UnauthorizedAccessException();
			default:
				if (errorCode == 1314)
				{
					return new PrivilegeNotHeldException();
				}
				break;
			}
			return new InvalidOperationException("OS error code " + errorCode.ToString());
		}

		private void RaiseExceptionOnFailure(int errorCode, string name, SafeHandle handle, object context)
		{
			if (errorCode == 0)
			{
				return;
			}
			throw (this.exception_from_error_code ?? new NativeObjectSecurity.ExceptionFromErrorCode(NativeObjectSecurity.DefaultExceptionFromErrorCode))(errorCode, name, handle, context);
		}

		internal virtual int InternalGet(SafeHandle handle, AccessControlSections includeSections)
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				throw new PlatformNotSupportedException();
			}
			return this.Win32GetHelper(delegate(SecurityInfos securityInfos, out IntPtr owner, out IntPtr group, out IntPtr dacl, out IntPtr sacl, out IntPtr descriptor)
			{
				return NativeObjectSecurity.GetSecurityInfo(handle, this.ResourceType, securityInfos, out owner, out group, out dacl, out sacl, out descriptor);
			}, includeSections);
		}

		internal virtual int InternalGet(string name, AccessControlSections includeSections)
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				throw new PlatformNotSupportedException();
			}
			return this.Win32GetHelper(delegate(SecurityInfos securityInfos, out IntPtr owner, out IntPtr group, out IntPtr dacl, out IntPtr sacl, out IntPtr descriptor)
			{
				return NativeObjectSecurity.GetNamedSecurityInfo(this.Win32FixName(name), this.ResourceType, securityInfos, out owner, out group, out dacl, out sacl, out descriptor);
			}, includeSections);
		}

		internal virtual int InternalSet(SafeHandle handle, AccessControlSections includeSections)
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				throw new PlatformNotSupportedException();
			}
			return this.Win32SetHelper((SecurityInfos securityInfos, byte[] owner, byte[] group, byte[] dacl, byte[] sacl) => NativeObjectSecurity.SetSecurityInfo(handle, this.ResourceType, securityInfos, owner, group, dacl, sacl), includeSections);
		}

		internal virtual int InternalSet(string name, AccessControlSections includeSections)
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				throw new PlatformNotSupportedException();
			}
			return this.Win32SetHelper((SecurityInfos securityInfos, byte[] owner, byte[] group, byte[] dacl, byte[] sacl) => NativeObjectSecurity.SetNamedSecurityInfo(this.Win32FixName(name), this.ResourceType, securityInfos, owner, group, dacl, sacl), includeSections);
		}

		internal ResourceType ResourceType
		{
			get
			{
				return this.resource_type;
			}
		}

		private int Win32GetHelper(NativeObjectSecurity.GetSecurityInfoNativeCall nativeCall, AccessControlSections includeSections)
		{
			bool flag = (includeSections & AccessControlSections.Owner) > AccessControlSections.None;
			bool flag2 = (includeSections & AccessControlSections.Group) > AccessControlSections.None;
			bool flag3 = (includeSections & AccessControlSections.Access) > AccessControlSections.None;
			bool flag4 = (includeSections & AccessControlSections.Audit) > AccessControlSections.None;
			SecurityInfos securityInfos = (SecurityInfos)0;
			if (flag)
			{
				securityInfos |= SecurityInfos.Owner;
			}
			if (flag2)
			{
				securityInfos |= SecurityInfos.Group;
			}
			if (flag3)
			{
				securityInfos |= SecurityInfos.DiscretionaryAcl;
			}
			if (flag4)
			{
				securityInfos |= SecurityInfos.SystemAcl;
			}
			IntPtr intPtr;
			IntPtr intPtr2;
			IntPtr intPtr3;
			IntPtr intPtr4;
			IntPtr intPtr5;
			int num = nativeCall(securityInfos, out intPtr, out intPtr2, out intPtr3, out intPtr4, out intPtr5);
			if (num != 0)
			{
				return num;
			}
			try
			{
				int num2 = 0;
				if (NativeObjectSecurity.IsValidSecurityDescriptor(intPtr5))
				{
					num2 = NativeObjectSecurity.GetSecurityDescriptorLength(intPtr5);
				}
				byte[] array = new byte[num2];
				Marshal.Copy(intPtr5, array, 0, num2);
				base.SetSecurityDescriptorBinaryForm(array, includeSections);
			}
			finally
			{
				NativeObjectSecurity.LocalFree(intPtr5);
			}
			return 0;
		}

		private int Win32SetHelper(NativeObjectSecurity.SetSecurityInfoNativeCall nativeCall, AccessControlSections includeSections)
		{
			if (includeSections == AccessControlSections.None)
			{
				return 0;
			}
			SecurityInfos securityInfos = (SecurityInfos)0;
			byte[] array = null;
			byte[] array2 = null;
			byte[] array3 = null;
			byte[] array4 = null;
			if ((includeSections & AccessControlSections.Owner) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.Owner;
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)base.GetOwner(typeof(SecurityIdentifier));
				if (null != securityIdentifier)
				{
					array = new byte[securityIdentifier.BinaryLength];
					securityIdentifier.GetBinaryForm(array, 0);
				}
			}
			if ((includeSections & AccessControlSections.Group) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.Group;
				SecurityIdentifier securityIdentifier2 = (SecurityIdentifier)base.GetGroup(typeof(SecurityIdentifier));
				if (null != securityIdentifier2)
				{
					array2 = new byte[securityIdentifier2.BinaryLength];
					securityIdentifier2.GetBinaryForm(array2, 0);
				}
			}
			if ((includeSections & AccessControlSections.Access) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.DiscretionaryAcl;
				if (base.AreAccessRulesProtected)
				{
					securityInfos |= (SecurityInfos)(-2147483648);
				}
				else
				{
					securityInfos |= (SecurityInfos)536870912;
				}
				array3 = new byte[this.descriptor.DiscretionaryAcl.BinaryLength];
				this.descriptor.DiscretionaryAcl.GetBinaryForm(array3, 0);
			}
			if ((includeSections & AccessControlSections.Audit) != AccessControlSections.None && this.descriptor.SystemAcl != null)
			{
				securityInfos |= SecurityInfos.SystemAcl;
				if (base.AreAuditRulesProtected)
				{
					securityInfos |= (SecurityInfos)1073741824;
				}
				else
				{
					securityInfos |= (SecurityInfos)268435456;
				}
				array4 = new byte[this.descriptor.SystemAcl.BinaryLength];
				this.descriptor.SystemAcl.GetBinaryForm(array4, 0);
			}
			return nativeCall(securityInfos, array, array2, array3, array4);
		}

		private string Win32FixName(string name)
		{
			if (this.ResourceType == ResourceType.RegistryKey)
			{
				if (!name.StartsWith("HKEY_"))
				{
					throw new InvalidOperationException();
				}
				name = name.Substring("HKEY_".Length);
			}
			return name;
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetSecurityInfo(SafeHandle handle, ResourceType resourceType, SecurityInfos securityInfos, out IntPtr owner, out IntPtr group, out IntPtr dacl, out IntPtr sacl, out IntPtr descriptor);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetNamedSecurityInfo(string name, ResourceType resourceType, SecurityInfos securityInfos, out IntPtr owner, out IntPtr group, out IntPtr dacl, out IntPtr sacl, out IntPtr descriptor);

		[DllImport("kernel32.dll")]
		private static extern IntPtr LocalFree(IntPtr handle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int SetSecurityInfo(SafeHandle handle, ResourceType resourceType, SecurityInfos securityInfos, byte[] owner, byte[] group, byte[] dacl, byte[] sacl);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int SetNamedSecurityInfo(string name, ResourceType resourceType, SecurityInfos securityInfos, byte[] owner, byte[] group, byte[] dacl, byte[] sacl);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetSecurityDescriptorLength(IntPtr descriptor);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsValidSecurityDescriptor(IntPtr descriptor);

		private NativeObjectSecurity.ExceptionFromErrorCode exception_from_error_code;

		private ResourceType resource_type;

		/// <summary>Provides a way for integrators to map numeric error codes to specific exceptions that they create.</summary>
		/// <param name="errorCode">The numeric error code.</param>
		/// <param name="name">The name of the securable object with which the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="handle">The handle of the securable object with which the <see cref="T:System.Security.AccessControl.NativeObjectSecurity" /> object is associated.</param>
		/// <param name="context">An object that contains contextual information about the source or destination of the exception.</param>
		/// <returns>The <see cref="T:System.Exception" /> this delegate creates.</returns>
		protected internal delegate Exception ExceptionFromErrorCode(int errorCode, string name, SafeHandle handle, object context);

		private delegate int GetSecurityInfoNativeCall(SecurityInfos securityInfos, out IntPtr owner, out IntPtr group, out IntPtr dacl, out IntPtr sacl, out IntPtr descriptor);

		private delegate int SetSecurityInfoNativeCall(SecurityInfos securityInfos, byte[] owner, byte[] group, byte[] dacl, byte[] sacl);
	}
}
