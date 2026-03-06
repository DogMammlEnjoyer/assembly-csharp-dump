using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	/// <summary>Represents a <see cref="T:System.Reflection.MemberInfo" /> object that does not load assemblies or create objects until requested.</summary>
	public struct LazyMemberInfo
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.ReflectionModel.LazyMemberInfo" /> class, representing the specified member.</summary>
		/// <param name="member">The member to represent.</param>
		public LazyMemberInfo(MemberInfo member)
		{
			Requires.NotNull<MemberInfo>(member, "member");
			LazyMemberInfo.EnsureSupportedMemberType(member.MemberType, "member");
			this._accessorsCreator = null;
			this._memberType = member.MemberType;
			MemberTypes memberType = this._memberType;
			if (memberType == MemberTypes.Event)
			{
				EventInfo eventInfo = (EventInfo)member;
				this._accessors = new MemberInfo[]
				{
					eventInfo.GetRaiseMethod(true),
					eventInfo.GetAddMethod(true),
					eventInfo.GetRemoveMethod(true)
				};
				return;
			}
			if (memberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				Assumes.NotNull<PropertyInfo>(propertyInfo);
				this._accessors = new MemberInfo[]
				{
					propertyInfo.GetGetMethod(true),
					propertyInfo.GetSetMethod(true)
				};
				return;
			}
			this._accessors = new MemberInfo[]
			{
				member
			};
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.ReflectionModel.LazyMemberInfo" /> class for a member of the specified type with the specified accessors.</summary>
		/// <param name="memberType">The type of the represented member.</param>
		/// <param name="accessors">An array of the accessors for the represented member.</param>
		/// <exception cref="T:System.ArgumentException">One or more of the objects in <paramref name="accessors" /> are not valid accessors for this member.</exception>
		public LazyMemberInfo(MemberTypes memberType, params MemberInfo[] accessors)
		{
			LazyMemberInfo.EnsureSupportedMemberType(memberType, "memberType");
			Requires.NotNull<MemberInfo[]>(accessors, "accessors");
			string message;
			if (!LazyMemberInfo.AreAccessorsValid(memberType, accessors, out message))
			{
				throw new ArgumentException(message, "accessors");
			}
			this._memberType = memberType;
			this._accessors = accessors;
			this._accessorsCreator = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.ReflectionModel.LazyMemberInfo" /> class for a member of the specified type with the specified accessors.</summary>
		/// <param name="memberType">The type of the represented member.</param>
		/// <param name="accessorsCreator">A function whose return value is a collection of the accessors for the represented member.</param>
		public LazyMemberInfo(MemberTypes memberType, Func<MemberInfo[]> accessorsCreator)
		{
			LazyMemberInfo.EnsureSupportedMemberType(memberType, "memberType");
			Requires.NotNull<Func<MemberInfo[]>>(accessorsCreator, "accessorsCreator");
			this._memberType = memberType;
			this._accessors = null;
			this._accessorsCreator = accessorsCreator;
		}

		/// <summary>Gets the type of the represented member.</summary>
		/// <returns>The type of the represented member.</returns>
		public MemberTypes MemberType
		{
			get
			{
				return this._memberType;
			}
		}

		/// <summary>Gets an array of the accessors for the represented member.</summary>
		/// <returns>An array of the accessors for the represented member.</returns>
		/// <exception cref="T:System.ArgumentException">One or more of the accessors in this object are invalid.</exception>
		public MemberInfo[] GetAccessors()
		{
			if (this._accessors == null && this._accessorsCreator != null)
			{
				MemberInfo[] accessors = this._accessorsCreator();
				string message;
				if (!LazyMemberInfo.AreAccessorsValid(this.MemberType, accessors, out message))
				{
					throw new InvalidOperationException(message);
				}
				this._accessors = accessors;
			}
			return this._accessors;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			if (this._accessorsCreator != null)
			{
				return this.MemberType.GetHashCode() ^ this._accessorsCreator.GetHashCode();
			}
			Assumes.NotNull<MemberInfo[]>(this._accessors);
			Assumes.NotNull<MemberInfo>(this._accessors[0]);
			return this.MemberType.GetHashCode() ^ this._accessors[0].GetHashCode();
		}

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			LazyMemberInfo lazyMemberInfo = (LazyMemberInfo)obj;
			if (this._memberType != lazyMemberInfo._memberType)
			{
				return false;
			}
			if (this._accessorsCreator != null || lazyMemberInfo._accessorsCreator != null)
			{
				return object.Equals(this._accessorsCreator, lazyMemberInfo._accessorsCreator);
			}
			Assumes.NotNull<MemberInfo[]>(this._accessors);
			Assumes.NotNull<MemberInfo[]>(lazyMemberInfo._accessors);
			return this._accessors.SequenceEqual(lazyMemberInfo._accessors);
		}

		/// <summary>Determines whether the two specified <see cref="T:System.ComponentModel.Composition.ReflectionModel.LazyMemberInfo" /> objects are equal.</summary>
		/// <param name="left">The first object to test.</param>
		/// <param name="right">The second object to test.</param>
		/// <returns>
		///   <see langword="true" /> if the objects are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(LazyMemberInfo left, LazyMemberInfo right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether the two specified <see cref="T:System.ComponentModel.Composition.ReflectionModel.LazyMemberInfo" /> objects are not equal.</summary>
		/// <param name="left">The first object to test.</param>
		/// <param name="right">The second object to test.</param>
		/// <returns>
		///   <see langword="true" /> if the objects are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(LazyMemberInfo left, LazyMemberInfo right)
		{
			return !left.Equals(right);
		}

		private static void EnsureSupportedMemberType(MemberTypes memberType, string argument)
		{
			MemberTypes enumFlagSet = MemberTypes.All;
			Requires.IsInMembertypeSet(memberType, argument, enumFlagSet);
		}

		private static bool AreAccessorsValid(MemberTypes memberType, MemberInfo[] accessors, out string errorMessage)
		{
			errorMessage = string.Empty;
			if (accessors == null)
			{
				errorMessage = Strings.LazyMemberInfo_AccessorsNull;
				return false;
			}
			if (accessors.All((MemberInfo accessor) => accessor == null))
			{
				errorMessage = Strings.LazyMemberInfo_NoAccessors;
				return false;
			}
			if (memberType != MemberTypes.Event)
			{
				if (memberType == MemberTypes.Property)
				{
					if (accessors.Length != 2)
					{
						errorMessage = Strings.LazyMemberInfo_InvalidPropertyAccessors_Cardinality;
						return false;
					}
					if ((from accessor in accessors
					where accessor != null && accessor.MemberType != MemberTypes.Method
					select accessor).Any<MemberInfo>())
					{
						errorMessage = Strings.LazyMemberinfo_InvalidPropertyAccessors_AccessorType;
						return false;
					}
				}
				else if (accessors.Length != 1 || (accessors.Length == 1 && accessors[0].MemberType != memberType))
				{
					errorMessage = string.Format(CultureInfo.CurrentCulture, Strings.LazyMemberInfo_InvalidAccessorOnSimpleMember, memberType);
					return false;
				}
			}
			else
			{
				if (accessors.Length != 3)
				{
					errorMessage = Strings.LazyMemberInfo_InvalidEventAccessors_Cardinality;
					return false;
				}
				if ((from accessor in accessors
				where accessor != null && accessor.MemberType != MemberTypes.Method
				select accessor).Any<MemberInfo>())
				{
					errorMessage = Strings.LazyMemberinfo_InvalidEventAccessors_AccessorType;
					return false;
				}
			}
			return true;
		}

		private readonly MemberTypes _memberType;

		private MemberInfo[] _accessors;

		private readonly Func<MemberInfo[]> _accessorsCreator;
	}
}
