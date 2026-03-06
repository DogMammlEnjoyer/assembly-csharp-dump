using System;
using System.Collections.Generic;

namespace System.ComponentModel.Design.Serialization
{
	/// <summary>Provides the base class for relating one member to another.</summary>
	public abstract class MemberRelationshipService
	{
		/// <summary>Establishes a relationship between a source and target object.</summary>
		/// <param name="source">The source relationship. This is the left-hand side of a relationship assignment.</param>
		/// <returns>The current relationship associated with <paramref name="source" />, or <see cref="F:System.ComponentModel.Design.Serialization.MemberRelationship.Empty" /> if there is no relationship.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="source" /> is empty, or the relationship is not supported by the service.</exception>
		public MemberRelationship this[MemberRelationship source]
		{
			get
			{
				if (source.Owner == null)
				{
					throw new ArgumentNullException("Owner");
				}
				if (source.Member == null)
				{
					throw new ArgumentNullException("Member");
				}
				return this.GetRelationship(source);
			}
			set
			{
				if (source.Owner == null)
				{
					throw new ArgumentNullException("Owner");
				}
				if (source.Member == null)
				{
					throw new ArgumentNullException("Member");
				}
				this.SetRelationship(source, value);
			}
		}

		/// <summary>Establishes a relationship between a source and target object.</summary>
		/// <param name="sourceOwner">The owner of a source relationship.</param>
		/// <param name="sourceMember">The member of a source relationship.</param>
		/// <returns>A <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structure encapsulating the relationship between a source and target object, or <see langword="null" /> if there is no relationship.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="sourceOwner" /> or <paramref name="sourceMember" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="sourceOwner" /> or <paramref name="sourceMember" /> is empty, or the relationship is not supported by the service.</exception>
		public MemberRelationship this[object sourceOwner, MemberDescriptor sourceMember]
		{
			get
			{
				if (sourceOwner == null)
				{
					throw new ArgumentNullException("sourceOwner");
				}
				if (sourceMember == null)
				{
					throw new ArgumentNullException("sourceMember");
				}
				return this.GetRelationship(new MemberRelationship(sourceOwner, sourceMember));
			}
			set
			{
				if (sourceOwner == null)
				{
					throw new ArgumentNullException("sourceOwner");
				}
				if (sourceMember == null)
				{
					throw new ArgumentNullException("sourceMember");
				}
				this.SetRelationship(new MemberRelationship(sourceOwner, sourceMember), value);
			}
		}

		/// <summary>Gets a relationship to the given source relationship.</summary>
		/// <param name="source">The source relationship.</param>
		/// <returns>A relationship to <paramref name="source" />, or <see cref="F:System.ComponentModel.Design.Serialization.MemberRelationship.Empty" /> if no relationship exists.</returns>
		protected virtual MemberRelationship GetRelationship(MemberRelationship source)
		{
			MemberRelationshipService.RelationshipEntry relationshipEntry;
			if (this._relationships != null && this._relationships.TryGetValue(new MemberRelationshipService.RelationshipEntry(source), out relationshipEntry) && relationshipEntry.Owner.IsAlive)
			{
				return new MemberRelationship(relationshipEntry.Owner.Target, relationshipEntry.Member);
			}
			return MemberRelationship.Empty;
		}

		/// <summary>Creates a relationship between the source object and target relationship.</summary>
		/// <param name="source">The source relationship.</param>
		/// <param name="relationship">The relationship to set into the source.</param>
		/// <exception cref="T:System.ArgumentException">The relationship is not supported by the service.</exception>
		protected virtual void SetRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			if (!relationship.IsEmpty && !this.SupportsRelationship(source, relationship))
			{
				string text = TypeDescriptor.GetComponentName(source.Owner);
				string text2 = TypeDescriptor.GetComponentName(relationship.Owner);
				if (text == null)
				{
					text = source.Owner.ToString();
				}
				if (text2 == null)
				{
					text2 = relationship.Owner.ToString();
				}
				throw new ArgumentException(SR.Format("Relationships between {0}.{1} and {2}.{3} are not supported.", new object[]
				{
					text,
					source.Member.Name,
					text2,
					relationship.Member.Name
				}));
			}
			if (this._relationships == null)
			{
				this._relationships = new Dictionary<MemberRelationshipService.RelationshipEntry, MemberRelationshipService.RelationshipEntry>();
			}
			this._relationships[new MemberRelationshipService.RelationshipEntry(source)] = new MemberRelationshipService.RelationshipEntry(relationship);
		}

		/// <summary>Gets a value indicating whether the given relationship is supported.</summary>
		/// <param name="source">The source relationship.</param>
		/// <param name="relationship">The relationship to set into the source.</param>
		/// <returns>
		///   <see langword="true" /> if a relationship between the given two objects is supported; otherwise, <see langword="false" />.</returns>
		public abstract bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship);

		private Dictionary<MemberRelationshipService.RelationshipEntry, MemberRelationshipService.RelationshipEntry> _relationships = new Dictionary<MemberRelationshipService.RelationshipEntry, MemberRelationshipService.RelationshipEntry>();

		private struct RelationshipEntry
		{
			internal RelationshipEntry(MemberRelationship rel)
			{
				this.Owner = new WeakReference(rel.Owner);
				this.Member = rel.Member;
				this._hashCode = ((rel.Owner == null) ? 0 : rel.Owner.GetHashCode());
			}

			public override bool Equals(object o)
			{
				if (o is MemberRelationshipService.RelationshipEntry)
				{
					MemberRelationshipService.RelationshipEntry re = (MemberRelationshipService.RelationshipEntry)o;
					return this == re;
				}
				return false;
			}

			public static bool operator ==(MemberRelationshipService.RelationshipEntry re1, MemberRelationshipService.RelationshipEntry re2)
			{
				object obj = re1.Owner.IsAlive ? re1.Owner.Target : null;
				object obj2 = re2.Owner.IsAlive ? re2.Owner.Target : null;
				return obj == obj2 && re1.Member.Equals(re2.Member);
			}

			public static bool operator !=(MemberRelationshipService.RelationshipEntry re1, MemberRelationshipService.RelationshipEntry re2)
			{
				return !(re1 == re2);
			}

			public override int GetHashCode()
			{
				return this._hashCode;
			}

			internal WeakReference Owner;

			internal MemberDescriptor Member;

			private int _hashCode;
		}
	}
}
