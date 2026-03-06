using System;
using System.Security.Principal;
using Unity;

namespace System.Security.AccessControl
{
	/// <summary>Represents an access control list (ACL) and is the base class for the <see cref="T:System.Security.AccessControl.DiscretionaryAcl" /> and <see cref="T:System.Security.AccessControl.SystemAcl" /> classes.</summary>
	public abstract class CommonAcl : GenericAcl
	{
		internal CommonAcl(bool isContainer, bool isDS, RawAcl rawAcl)
		{
			if (rawAcl == null)
			{
				rawAcl = new RawAcl(isDS ? GenericAcl.AclRevisionDS : GenericAcl.AclRevision, 10);
			}
			else
			{
				byte[] binaryForm = new byte[rawAcl.BinaryLength];
				rawAcl.GetBinaryForm(binaryForm, 0);
				rawAcl = new RawAcl(binaryForm, 0);
			}
			this.Init(isContainer, isDS, rawAcl);
		}

		internal CommonAcl(bool isContainer, bool isDS, byte revision, int capacity)
		{
			this.Init(isContainer, isDS, new RawAcl(revision, capacity));
		}

		internal CommonAcl(bool isContainer, bool isDS, int capacity) : this(isContainer, isDS, isDS ? GenericAcl.AclRevisionDS : GenericAcl.AclRevision, capacity)
		{
		}

		private void Init(bool isContainer, bool isDS, RawAcl rawAcl)
		{
			this.is_container = isContainer;
			this.is_ds = isDS;
			this.raw_acl = rawAcl;
			this.CanonicalizeAndClearAefa();
		}

		/// <summary>Gets the length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object. This length should be used before marshaling the access control list (ACL) into a binary array by using the <see cref="M:System.Security.AccessControl.CommonAcl.GetBinaryForm(System.Byte[],System.Int32)" /> method.</summary>
		/// <returns>The length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object.</returns>
		public sealed override int BinaryLength
		{
			get
			{
				return this.raw_acl.BinaryLength;
			}
		}

		/// <summary>Gets the number of access control entries (ACEs) in the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object.</summary>
		/// <returns>The number of ACEs in the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object.</returns>
		public sealed override int Count
		{
			get
			{
				return this.raw_acl.Count;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the access control entries (ACEs) in the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object are in canonical order.</summary>
		/// <returns>
		///   <see langword="true" /> if the ACEs in the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object are in canonical order; otherwise, <see langword="false" />.</returns>
		public bool IsCanonical
		{
			get
			{
				return this.is_canonical;
			}
		}

		/// <summary>Sets whether the <see cref="T:System.Security.AccessControl.CommonAcl" /> object is a container.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object is a container.</returns>
		public bool IsContainer
		{
			get
			{
				return this.is_container;
			}
		}

		/// <summary>Sets whether the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object is a directory object access control list (ACL).</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Security.AccessControl.CommonAcl" /> object is a directory object ACL.</returns>
		public bool IsDS
		{
			get
			{
				return this.is_ds;
			}
		}

		internal bool IsAefa
		{
			get
			{
				return this.is_aefa;
			}
			set
			{
				this.is_aefa = value;
			}
		}

		/// <summary>Gets the revision level of the <see cref="T:System.Security.AccessControl.CommonAcl" />.</summary>
		/// <returns>A byte value that specifies the revision level of the <see cref="T:System.Security.AccessControl.CommonAcl" />.</returns>
		public sealed override byte Revision
		{
			get
			{
				return this.raw_acl.Revision;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.AccessControl.CommonAce" /> at the specified index.</summary>
		/// <param name="index">The zero-based index of the <see cref="T:System.Security.AccessControl.CommonAce" /> to get or set.</param>
		/// <returns>The <see cref="T:System.Security.AccessControl.CommonAce" /> at the specified index.</returns>
		public sealed override GenericAce this[int index]
		{
			get
			{
				return CommonAcl.CopyAce(this.raw_acl[index]);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>Marshals the contents of the <see cref="T:System.Security.AccessControl.CommonAcl" /> object into the specified byte array beginning at the specified offset.</summary>
		/// <param name="binaryForm">The byte array into which the contents of the <see cref="T:System.Security.AccessControl.CommonAcl" /> is marshaled.</param>
		/// <param name="offset">The offset at which to start marshaling.</param>
		public sealed override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			this.raw_acl.GetBinaryForm(binaryForm, offset);
		}

		/// <summary>Removes all access control entries (ACEs) contained by this <see cref="T:System.Security.AccessControl.CommonAcl" /> object that are associated with the specified <see cref="T:System.Security.Principal.SecurityIdentifier" /> object.</summary>
		/// <param name="sid">The <see cref="T:System.Security.Principal.SecurityIdentifier" /> object to check for.</param>
		public void Purge(SecurityIdentifier sid)
		{
			this.RequireCanonicity();
			this.RemoveAces<KnownAce>((KnownAce ace) => ace.SecurityIdentifier == sid);
		}

		/// <summary>Removes all inherited access control entries (ACEs) from this <see cref="T:System.Security.AccessControl.CommonAcl" /> object.</summary>
		public void RemoveInheritedAces()
		{
			this.RequireCanonicity();
			this.RemoveAces<GenericAce>((GenericAce ace) => ace.IsInherited);
		}

		internal void RequireCanonicity()
		{
			if (!this.IsCanonical)
			{
				throw new InvalidOperationException("ACL is not canonical.");
			}
		}

		internal void CanonicalizeAndClearAefa()
		{
			this.RemoveAces<GenericAce>(new CommonAcl.RemoveAcesCallback<GenericAce>(this.IsAceMeaningless));
			this.is_canonical = this.TestCanonicity();
			if (this.IsCanonical)
			{
				this.ApplyCanonicalSortToExplicitAces();
				this.MergeExplicitAces();
			}
			this.IsAefa = false;
		}

		internal virtual bool IsAceMeaningless(GenericAce ace)
		{
			AceFlags aceFlags = ace.AceFlags;
			KnownAce knownAce = ace as KnownAce;
			if (knownAce != null)
			{
				if (knownAce.AccessMask == 0)
				{
					return true;
				}
				if ((aceFlags & AceFlags.InheritOnly) != AceFlags.None)
				{
					if (knownAce is ObjectAce)
					{
						return true;
					}
					if (!this.IsContainer)
					{
						return true;
					}
					if ((aceFlags & (AceFlags.ObjectInherit | AceFlags.ContainerInherit)) == AceFlags.None)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool TestCanonicity()
		{
			AceEnumerator enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current is QualifiedAce))
				{
					return false;
				}
			}
			bool flag = false;
			enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (((QualifiedAce)enumerator.Current).IsInherited)
				{
					flag = true;
				}
				else if (flag)
				{
					return false;
				}
			}
			bool flag2 = false;
			foreach (GenericAce genericAce in this)
			{
				QualifiedAce qualifiedAce = (QualifiedAce)genericAce;
				if (qualifiedAce.IsInherited)
				{
					break;
				}
				if (qualifiedAce.AceQualifier == AceQualifier.AccessAllowed)
				{
					flag2 = true;
				}
				else if (AceQualifier.AccessDenied == qualifiedAce.AceQualifier && flag2)
				{
					return false;
				}
			}
			return true;
		}

		internal int GetCanonicalExplicitDenyAceCount()
		{
			int num = 0;
			while (num < this.Count && !this.raw_acl[num].IsInherited)
			{
				QualifiedAce qualifiedAce = this.raw_acl[num] as QualifiedAce;
				if (qualifiedAce == null || qualifiedAce.AceQualifier != AceQualifier.AccessDenied)
				{
					break;
				}
				num++;
			}
			return num;
		}

		internal int GetCanonicalExplicitAceCount()
		{
			int num = 0;
			while (num < this.Count && !this.raw_acl[num].IsInherited)
			{
				num++;
			}
			return num;
		}

		private void MergeExplicitAces()
		{
			int num = this.GetCanonicalExplicitAceCount();
			int i = 0;
			while (i < num - 1)
			{
				GenericAce genericAce = this.MergeExplicitAcePair(this.raw_acl[i], this.raw_acl[i + 1]);
				if (null != genericAce)
				{
					this.raw_acl[i] = genericAce;
					this.raw_acl.RemoveAce(i + 1);
					num--;
				}
				else
				{
					i++;
				}
			}
		}

		private GenericAce MergeExplicitAcePair(GenericAce ace1, GenericAce ace2)
		{
			QualifiedAce qualifiedAce = ace1 as QualifiedAce;
			QualifiedAce qualifiedAce2 = ace2 as QualifiedAce;
			if (!(null != qualifiedAce) || !(null != qualifiedAce2))
			{
				return null;
			}
			if (qualifiedAce.AceQualifier != qualifiedAce2.AceQualifier)
			{
				return null;
			}
			if (!(qualifiedAce.SecurityIdentifier == qualifiedAce2.SecurityIdentifier))
			{
				return null;
			}
			AceFlags aceFlags = qualifiedAce.AceFlags;
			AceFlags aceFlags2 = qualifiedAce2.AceFlags;
			int accessMask = qualifiedAce.AccessMask;
			int accessMask2 = qualifiedAce2.AccessMask;
			if (!this.IsContainer)
			{
				aceFlags &= ~(AceFlags.ObjectInherit | AceFlags.ContainerInherit | AceFlags.NoPropagateInherit | AceFlags.InheritOnly);
				aceFlags2 &= ~(AceFlags.ObjectInherit | AceFlags.ContainerInherit | AceFlags.NoPropagateInherit | AceFlags.InheritOnly);
			}
			AceFlags aceFlags3;
			int accessMask3;
			if (aceFlags != aceFlags2)
			{
				if (accessMask != accessMask2)
				{
					return null;
				}
				if ((aceFlags & ~(AceFlags.ObjectInherit | AceFlags.ContainerInherit)) == (aceFlags2 & ~(AceFlags.ObjectInherit | AceFlags.ContainerInherit)))
				{
					aceFlags3 = (aceFlags | aceFlags2);
					accessMask3 = accessMask;
				}
				else
				{
					if ((aceFlags & ~(AceFlags.SuccessfulAccess | AceFlags.FailedAccess)) != (aceFlags2 & ~(AceFlags.SuccessfulAccess | AceFlags.FailedAccess)))
					{
						return null;
					}
					aceFlags3 = (aceFlags | aceFlags2);
					accessMask3 = accessMask;
				}
			}
			else
			{
				aceFlags3 = aceFlags;
				accessMask3 = (accessMask | accessMask2);
			}
			CommonAce commonAce = ace1 as CommonAce;
			CommonAce right = ace2 as CommonAce;
			if (null != commonAce && null != right)
			{
				return new CommonAce(aceFlags3, commonAce.AceQualifier, accessMask3, commonAce.SecurityIdentifier, commonAce.IsCallback, commonAce.GetOpaque());
			}
			ObjectAce objectAce = ace1 as ObjectAce;
			ObjectAce objectAce2 = ace2 as ObjectAce;
			if (null != objectAce && null != objectAce2)
			{
				Guid a;
				Guid a2;
				CommonAcl.GetObjectAceTypeGuids(objectAce, out a, out a2);
				Guid b;
				Guid b2;
				CommonAcl.GetObjectAceTypeGuids(objectAce2, out b, out b2);
				if (a == b && a2 == b2)
				{
					return new ObjectAce(aceFlags3, objectAce.AceQualifier, accessMask3, objectAce.SecurityIdentifier, objectAce.ObjectAceFlags, objectAce.ObjectAceType, objectAce.InheritedObjectAceType, objectAce.IsCallback, objectAce.GetOpaque());
				}
			}
			return null;
		}

		private static void GetObjectAceTypeGuids(ObjectAce ace, out Guid type, out Guid inheritedType)
		{
			type = Guid.Empty;
			inheritedType = Guid.Empty;
			if ((ace.ObjectAceFlags & ObjectAceFlags.ObjectAceTypePresent) != ObjectAceFlags.None)
			{
				type = ace.ObjectAceType;
			}
			if ((ace.ObjectAceFlags & ObjectAceFlags.InheritedObjectAceTypePresent) != ObjectAceFlags.None)
			{
				inheritedType = ace.InheritedObjectAceType;
			}
		}

		internal abstract void ApplyCanonicalSortToExplicitAces();

		internal void ApplyCanonicalSortToExplicitAces(int start, int count)
		{
			for (int i = start + 1; i < start + count; i++)
			{
				KnownAce knownAce = (KnownAce)this.raw_acl[i];
				SecurityIdentifier securityIdentifier = knownAce.SecurityIdentifier;
				int num = i;
				while (num > start && ((KnownAce)this.raw_acl[num - 1]).SecurityIdentifier.CompareTo(securityIdentifier) > 0)
				{
					this.raw_acl[num] = this.raw_acl[num - 1];
					num--;
				}
				this.raw_acl[num] = knownAce;
			}
		}

		internal override string GetSddlForm(ControlFlags sdFlags, bool isDacl)
		{
			return this.raw_acl.GetSddlForm(sdFlags, isDacl);
		}

		internal void RemoveAces<T>(CommonAcl.RemoveAcesCallback<T> callback) where T : GenericAce
		{
			int i = 0;
			while (i < this.raw_acl.Count)
			{
				if (this.raw_acl[i] is T && callback((T)((object)this.raw_acl[i])))
				{
					this.raw_acl.RemoveAce(i);
				}
				else
				{
					i++;
				}
			}
		}

		internal void AddAce(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
		{
			QualifiedAce newAce = this.AddAceGetQualifiedAce(aceQualifier, sid, accessMask, inheritanceFlags, propagationFlags, auditFlags);
			this.AddAce(newAce);
		}

		internal void AddAce(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			QualifiedAce newAce = this.AddAceGetQualifiedAce(aceQualifier, sid, accessMask, inheritanceFlags, propagationFlags, auditFlags, objectFlags, objectType, inheritedObjectType);
			this.AddAce(newAce);
		}

		private QualifiedAce AddAceGetQualifiedAce(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			if (!this.IsDS)
			{
				throw new InvalidOperationException("For this overload, IsDS must be true.");
			}
			if (objectFlags == ObjectAceFlags.None)
			{
				return this.AddAceGetQualifiedAce(aceQualifier, sid, accessMask, inheritanceFlags, propagationFlags, auditFlags);
			}
			return new ObjectAce(this.GetAceFlags(inheritanceFlags, propagationFlags, auditFlags), aceQualifier, accessMask, sid, objectFlags, objectType, inheritedObjectType, false, null);
		}

		private QualifiedAce AddAceGetQualifiedAce(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
		{
			return new CommonAce(this.GetAceFlags(inheritanceFlags, propagationFlags, auditFlags), aceQualifier, accessMask, sid, false, null);
		}

		private void AddAce(QualifiedAce newAce)
		{
			this.RequireCanonicity();
			int aceInsertPosition = this.GetAceInsertPosition(newAce.AceQualifier);
			this.raw_acl.InsertAce(aceInsertPosition, CommonAcl.CopyAce(newAce));
			this.CanonicalizeAndClearAefa();
		}

		private static GenericAce CopyAce(GenericAce ace)
		{
			byte[] binaryForm = new byte[ace.BinaryLength];
			ace.GetBinaryForm(binaryForm, 0);
			return GenericAce.CreateFromBinaryForm(binaryForm, 0);
		}

		internal abstract int GetAceInsertPosition(AceQualifier aceQualifier);

		private AceFlags GetAceFlags(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
		{
			if (inheritanceFlags != InheritanceFlags.None && !this.IsContainer)
			{
				throw new ArgumentException("Flags only work with containers.", "inheritanceFlags");
			}
			if (inheritanceFlags == InheritanceFlags.None && propagationFlags != PropagationFlags.None)
			{
				throw new ArgumentException("Propagation flags need inheritance flags.", "propagationFlags");
			}
			AceFlags aceFlags = AceFlags.None;
			if ((InheritanceFlags.ContainerInherit & inheritanceFlags) != InheritanceFlags.None)
			{
				aceFlags |= AceFlags.ContainerInherit;
			}
			if ((InheritanceFlags.ObjectInherit & inheritanceFlags) != InheritanceFlags.None)
			{
				aceFlags |= AceFlags.ObjectInherit;
			}
			if ((PropagationFlags.InheritOnly & propagationFlags) != PropagationFlags.None)
			{
				aceFlags |= AceFlags.InheritOnly;
			}
			if ((PropagationFlags.NoPropagateInherit & propagationFlags) != PropagationFlags.None)
			{
				aceFlags |= AceFlags.NoPropagateInherit;
			}
			if ((AuditFlags.Success & auditFlags) != AuditFlags.None)
			{
				aceFlags |= AceFlags.SuccessfulAccess;
			}
			if ((AuditFlags.Failure & auditFlags) != AuditFlags.None)
			{
				aceFlags |= AceFlags.FailedAccess;
			}
			return aceFlags;
		}

		internal void RemoveAceSpecific(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
		{
			this.RequireCanonicity();
			this.RemoveAces<CommonAce>((CommonAce ace) => ace.AccessMask == accessMask && ace.AceQualifier == aceQualifier && !(ace.SecurityIdentifier != sid) && ace.InheritanceFlags == inheritanceFlags && (inheritanceFlags == InheritanceFlags.None || ace.PropagationFlags == propagationFlags) && ace.AuditFlags == auditFlags);
			this.CanonicalizeAndClearAefa();
		}

		internal void RemoveAceSpecific(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			if (!this.IsDS)
			{
				throw new InvalidOperationException("For this overload, IsDS must be true.");
			}
			if (objectFlags == ObjectAceFlags.None)
			{
				this.RemoveAceSpecific(aceQualifier, sid, accessMask, inheritanceFlags, propagationFlags, auditFlags);
				return;
			}
			this.RequireCanonicity();
			this.RemoveAces<ObjectAce>((ObjectAce ace) => ace.AccessMask == accessMask && ace.AceQualifier == aceQualifier && !(ace.SecurityIdentifier != sid) && ace.InheritanceFlags == inheritanceFlags && (inheritanceFlags == InheritanceFlags.None || ace.PropagationFlags == propagationFlags) && ace.AuditFlags == auditFlags && ace.ObjectAceFlags == objectFlags && ((objectFlags & ObjectAceFlags.ObjectAceTypePresent) == ObjectAceFlags.None || !(ace.ObjectAceType != objectType)) && ((objectFlags & ObjectAceFlags.InheritedObjectAceTypePresent) == ObjectAceFlags.None || !(ace.InheritedObjectAceType != objectType)));
			this.CanonicalizeAndClearAefa();
		}

		internal void SetAce(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
		{
			QualifiedAce ace = this.AddAceGetQualifiedAce(aceQualifier, sid, accessMask, inheritanceFlags, propagationFlags, auditFlags);
			this.SetAce(ace);
		}

		internal void SetAce(AceQualifier aceQualifier, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			QualifiedAce ace = this.AddAceGetQualifiedAce(aceQualifier, sid, accessMask, inheritanceFlags, propagationFlags, auditFlags, objectFlags, objectType, inheritedObjectType);
			this.SetAce(ace);
		}

		private void SetAce(QualifiedAce newAce)
		{
			this.RequireCanonicity();
			this.RemoveAces<QualifiedAce>((QualifiedAce oldAce) => oldAce.AceQualifier == newAce.AceQualifier && oldAce.SecurityIdentifier == newAce.SecurityIdentifier);
			this.CanonicalizeAndClearAefa();
			this.AddAce(newAce);
		}

		internal CommonAcl()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private const int default_capacity = 10;

		private bool is_aefa;

		private bool is_canonical;

		private bool is_container;

		private bool is_ds;

		internal RawAcl raw_acl;

		internal delegate bool RemoveAcesCallback<T>(T ace);
	}
}
