using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	/// <summary>Provides the ability to control access to objects without direct manipulation of Access Control Lists (ACLs); also grants the ability to type-cast access rights.</summary>
	/// <typeparam name="T">The access rights for the object.</typeparam>
	public abstract class ObjectSecurity<T> : NativeObjectSecurity where T : struct
	{
		/// <summary>Initializes a new instance of the ObjectSecurity`1 class.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is a container object.</param>
		/// <param name="resourceType">The type of resource.</param>
		protected ObjectSecurity(bool isContainer, ResourceType resourceType) : base(isContainer, resourceType)
		{
		}

		/// <summary>Initializes a new instance of the ObjectSecurity`1 class.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is a container object.</param>
		/// <param name="resourceType">The type of resource.</param>
		/// <param name="safeHandle">A handle.</param>
		/// <param name="includeSections">The sections to include.</param>
		protected ObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle safeHandle, AccessControlSections includeSections) : base(isContainer, resourceType, safeHandle, includeSections)
		{
		}

		/// <summary>Initializes a new instance of the ObjectSecurity`1 class.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is a container object.</param>
		/// <param name="resourceType">The type of resource.</param>
		/// <param name="name">The name of the securable object with which the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is associated.</param>
		/// <param name="includeSections">The sections to include.</param>
		protected ObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections) : base(isContainer, resourceType, name, includeSections)
		{
		}

		/// <summary>Initializes a new instance of the ObjectSecurity`1 class.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is a container object.</param>
		/// <param name="resourceType">The type of resource.</param>
		/// <param name="safeHandle">A handle.</param>
		/// <param name="includeSections">The sections to include.</param>
		/// <param name="exceptionFromErrorCode">A delegate implemented by integrators that provides custom exceptions.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		protected ObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle safeHandle, AccessControlSections includeSections, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : base(isContainer, resourceType, safeHandle, includeSections, exceptionFromErrorCode, exceptionContext)
		{
		}

		/// <summary>Initializes a new instance of the ObjectSecurity`1 class.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is a container object.</param>
		/// <param name="resourceType">The type of resource.</param>
		/// <param name="name">The name of the securable object with which the new <see cref="T:System.Security.AccessControl.ObjectSecurity`1" /> object is associated.</param>
		/// <param name="includeSections">The sections to include.</param>
		/// <param name="exceptionFromErrorCode">A delegate implemented by integrators that provides custom exceptions.</param>
		/// <param name="exceptionContext">An object that contains contextual information about the source or destination of the exception.</param>
		protected ObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections, NativeObjectSecurity.ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext) : base(isContainer, resourceType, name, includeSections, exceptionFromErrorCode, exceptionContext)
		{
		}

		/// <summary>Gets the Type of the securable object associated with this ObjectSecurity`1 object.</summary>
		/// <returns>The type of the securable object associated with the current instance.</returns>
		public override Type AccessRightType
		{
			get
			{
				return typeof(T);
			}
		}

		/// <summary>Gets the Type of the object associated with the access rules of this ObjectSecurity`1 object.</summary>
		/// <returns>The Type of the object associated with the access rules of the current instance.</returns>
		public override Type AccessRuleType
		{
			get
			{
				return typeof(AccessRule<T>);
			}
		}

		/// <summary>Gets the Type object associated with the audit rules of this ObjectSecurity`1 object.</summary>
		/// <returns>The Type object associated with the audit rules of the current instance.</returns>
		public override Type AuditRuleType
		{
			get
			{
				return typeof(AuditRule<T>);
			}
		}

		/// <summary>Initializes a new instance of the ObjectAccessRule class that represents a new access control rule for the associated security object.</summary>
		/// <param name="identityReference">Represents a user account.</param>
		/// <param name="accessMask">The access type.</param>
		/// <param name="isInherited">
		///   <see langword="true" /> if the access rule is inherited; otherwise, <see langword="false" />.</param>
		/// <param name="inheritanceFlags">Specifies how to propagate access masks to child objects.</param>
		/// <param name="propagationFlags">Specifies how to propagate Access Control Entries (ACEs) to child objects.</param>
		/// <param name="type">Specifies whether access is allowed or denied.</param>
		/// <returns>Represents a new access control rule for the specified user, with the specified access rights, access control, and flags.</returns>
		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new AccessRule<T>(identityReference, accessMask, isInherited, inheritanceFlags, propagationFlags, type);
		}

		/// <summary>Adds the specified access rule to the Discretionary Access Control List (DACL) associated with this ObjectSecurity`1 object.</summary>
		/// <param name="rule">The rule to add.</param>
		public virtual void AddAccessRule(AccessRule<T> rule)
		{
			base.AddAccessRule(rule);
		}

		/// <summary>Removes access rules that contain the same security identifier and access mask as the specified access rule from the Discretionary Access Control List (DACL) associated with this ObjectSecurity`1 object.</summary>
		/// <param name="rule">The rule to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the access rule was successfully removed; otherwise, <see langword="false" />.</returns>
		public virtual bool RemoveAccessRule(AccessRule<T> rule)
		{
			return base.RemoveAccessRule(rule);
		}

		/// <summary>Removes all access rules that have the same security identifier as the specified access rule from the Discretionary Access Control List (DACL) associated with this ObjectSecurity`1 object.</summary>
		/// <param name="rule">The access rule to remove.</param>
		public virtual void RemoveAccessRuleAll(AccessRule<T> rule)
		{
			base.RemoveAccessRuleAll(rule);
		}

		/// <summary>Removes all access rules that exactly match the specified access rule from the Discretionary Access Control List (DACL) associated with this ObjectSecurity`1 object</summary>
		/// <param name="rule">The access rule to remove.</param>
		public virtual void RemoveAccessRuleSpecific(AccessRule<T> rule)
		{
			base.RemoveAccessRuleSpecific(rule);
		}

		/// <summary>Removes all access rules in the Discretionary Access Control List (DACL) associated with this ObjectSecurity`1 object and then adds the specified access rule.</summary>
		/// <param name="rule">The access rule to reset.</param>
		public virtual void ResetAccessRule(AccessRule<T> rule)
		{
			base.ResetAccessRule(rule);
		}

		/// <summary>Removes all access rules that contain the same security identifier and qualifier as the specified access rule in the Discretionary Access Control List (DACL) associated with this ObjectSecurity`1 object and then adds the specified access rule.</summary>
		/// <param name="rule">The access rule to set.</param>
		public virtual void SetAccessRule(AccessRule<T> rule)
		{
			base.SetAccessRule(rule);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuditRule" /> class representing the specified audit rule for the specified user.</summary>
		/// <param name="identityReference">Represents a user account.</param>
		/// <param name="accessMask">An integer that specifies an access type.</param>
		/// <param name="isInherited">
		///   <see langword="true" /> if the access rule is inherited; otherwise, <see langword="false" />.</param>
		/// <param name="inheritanceFlags">Specifies how to propagate access masks to child objects.</param>
		/// <param name="propagationFlags">Specifies how to propagate Access Control Entries (ACEs) to child objects.</param>
		/// <param name="flags">Describes the type of auditing to perform.</param>
		/// <returns>The specified audit rule for the specified user.</returns>
		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new AuditRule<T>(identityReference, accessMask, isInherited, inheritanceFlags, propagationFlags, flags);
		}

		/// <summary>Adds the specified audit rule to the System Access Control List (SACL) associated with this ObjectSecurity`1 object.</summary>
		/// <param name="rule">The audit rule to add.</param>
		public virtual void AddAuditRule(AuditRule<T> rule)
		{
			base.AddAuditRule(rule);
		}

		/// <summary>Removes audit rules that contain the same security identifier and access mask as the specified audit rule from the System Access Control List (SACL) associated with this ObjectSecurity`1 object.</summary>
		/// <param name="rule">The audit rule to remove</param>
		/// <returns>
		///   <see langword="true" /> if the object was removed; otherwise, <see langword="false" />.</returns>
		public virtual bool RemoveAuditRule(AuditRule<T> rule)
		{
			return base.RemoveAuditRule(rule);
		}

		/// <summary>Removes all audit rules that have the same security identifier as the specified audit rule from the System Access Control List (SACL) associated with this ObjectSecurity`1 object.</summary>
		/// <param name="rule">The audit rule to remove.</param>
		public virtual void RemoveAuditRuleAll(AuditRule<T> rule)
		{
			base.RemoveAuditRuleAll(rule);
		}

		/// <summary>Removes all audit rules that exactly match the specified audit rule from the System Access Control List (SACL) associated with this ObjectSecurity`1 object</summary>
		/// <param name="rule">The audit rule to remove.</param>
		public virtual void RemoveAuditRuleSpecific(AuditRule<T> rule)
		{
			base.RemoveAuditRuleSpecific(rule);
		}

		/// <summary>Removes all audit rules that contain the same security identifier and qualifier as the specified audit rule in the System Access Control List (SACL) associated with this ObjectSecurity`1 object and then adds the specified audit rule.</summary>
		/// <param name="rule">The audit rule to set.</param>
		public virtual void SetAuditRule(AuditRule<T> rule)
		{
			base.SetAuditRule(rule);
		}

		/// <summary>Saves the security descriptor associated with this ObjectSecurity`1 object to permanent storage, using the specified handle.</summary>
		/// <param name="handle">The handle of the securable object with which this ObjectSecurity`1 object is associated.</param>
		protected void Persist(SafeHandle handle)
		{
			base.WriteLock();
			try
			{
				this.Persist(handle, base.AccessControlSectionsModified);
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		/// <summary>Saves the security descriptor associated with this ObjectSecurity`1 object to permanent storage, using the specified name.</summary>
		/// <param name="name">The name of the securable object with which this ObjectSecurity`1 object is associated.</param>
		protected void Persist(string name)
		{
			base.WriteLock();
			try
			{
				this.Persist(name, base.AccessControlSectionsModified);
			}
			finally
			{
				base.WriteUnlock();
			}
		}
	}
}
