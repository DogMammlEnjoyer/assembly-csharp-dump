using System;

namespace System.ComponentModel.Design.Serialization
{
	/// <summary>Represents a single relationship between an object and a member.</summary>
	public readonly struct MemberRelationship
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> class.</summary>
		/// <param name="owner">The object that owns <paramref name="member" />.</param>
		/// <param name="member">The member which is to be related to <paramref name="owner" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="owner" /> or <paramref name="member" /> is <see langword="null" />.</exception>
		public MemberRelationship(object owner, MemberDescriptor member)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			this.Owner = owner;
			this.Member = member;
		}

		/// <summary>Gets a value indicating whether this relationship is equal to the <see cref="F:System.ComponentModel.Design.Serialization.MemberRelationship.Empty" /> relationship.</summary>
		/// <returns>
		///   <see langword="true" /> if this relationship is equal to the <see cref="F:System.ComponentModel.Design.Serialization.MemberRelationship.Empty" /> relationship; otherwise, <see langword="false" />.</returns>
		public bool IsEmpty
		{
			get
			{
				return this.Owner == null;
			}
		}

		/// <summary>Gets the related member.</summary>
		/// <returns>The member that is passed in to the <see cref="M:System.ComponentModel.Design.Serialization.MemberRelationship.#ctor(System.Object,System.ComponentModel.MemberDescriptor)" />.</returns>
		public MemberDescriptor Member { get; }

		/// <summary>Gets the owning object.</summary>
		/// <returns>The owning object that is passed in to the <see cref="M:System.ComponentModel.Design.Serialization.MemberRelationship.#ctor(System.Object,System.ComponentModel.MemberDescriptor)" />.</returns>
		public object Owner { get; }

		/// <summary>Determines whether two <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> instances are equal.</summary>
		/// <param name="obj">The <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> to compare with the current <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" />.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> is equal to the current <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is MemberRelationship))
			{
				return false;
			}
			MemberRelationship memberRelationship = (MemberRelationship)obj;
			return memberRelationship.Owner == this.Owner && memberRelationship.Member == this.Member;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" />.</returns>
		public override int GetHashCode()
		{
			if (this.Owner == null)
			{
				return base.GetHashCode();
			}
			return this.Owner.GetHashCode() ^ this.Member.GetHashCode();
		}

		/// <summary>Tests whether two specified <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structures are equivalent.</summary>
		/// <param name="left">The <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structure that is to the left of the equality operator.</param>
		/// <param name="right">The <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structure that is to the right of the equality operator.</param>
		/// <returns>This operator returns <see langword="true" /> if the two <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structures are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(MemberRelationship left, MemberRelationship right)
		{
			return left.Owner == right.Owner && left.Member == right.Member;
		}

		/// <summary>Tests whether two specified <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structures are different.</summary>
		/// <param name="left">The <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structure that is to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structure that is to the right of the inequality operator.</param>
		/// <returns>This operator returns <see langword="true" /> if the two <see cref="T:System.ComponentModel.Design.Serialization.MemberRelationship" /> structures are different; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(MemberRelationship left, MemberRelationship right)
		{
			return !(left == right);
		}

		/// <summary>Represents the empty member relationship. This field is read-only.</summary>
		public static readonly MemberRelationship Empty;
	}
}
