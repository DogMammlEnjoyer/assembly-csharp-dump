using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	/// <summary>Supports all classes in the .NET Framework class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all classes in the .NET Framework; it is the root of the type hierarchy.</summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	[Serializable]
	public class Object
	{
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
		public virtual bool Equals(object obj)
		{
			return this == obj;
		}

		/// <summary>Determines whether the specified object instances are considered equal.</summary>
		/// <param name="objA">The first object to compare.</param>
		/// <param name="objB">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the objects are considered equal; otherwise, <see langword="false" />. If both <paramref name="objA" /> and <paramref name="objB" /> are null, the method returns <see langword="true" />.</returns>
		public static bool Equals(object objA, object objB)
		{
			return objA == objB || (objA != null && objB != null && objA.Equals(objB));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public Object()
		{
		}

		/// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected virtual void Finalize()
		{
		}

		/// <summary>Serves as the default hash function.</summary>
		/// <returns>A hash code for the current object.</returns>
		public virtual int GetHashCode()
		{
			return object.InternalGetHashCode(this);
		}

		/// <summary>Gets the <see cref="T:System.Type" /> of the current instance.</summary>
		/// <returns>The exact runtime type of the current instance.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetType();

		/// <summary>Creates a shallow copy of the current <see cref="T:System.Object" />.</summary>
		/// <returns>A shallow copy of the current <see cref="T:System.Object" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected extern object MemberwiseClone();

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public virtual string ToString()
		{
			return this.GetType().ToString();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> instances are the same instance.</summary>
		/// <param name="objA">The first object to compare.</param>
		/// <param name="objB">The second object  to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="objA" /> is the same instance as <paramref name="objB" /> or if both are null; otherwise, <see langword="false" />.</returns>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool ReferenceEquals(object objA, object objB)
		{
			return objA == objB;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalGetHashCode(object o);

		private void FieldGetter(string typeName, string fieldName, ref object val)
		{
		}

		private void FieldSetter(string typeName, string fieldName, object val)
		{
		}
	}
}
