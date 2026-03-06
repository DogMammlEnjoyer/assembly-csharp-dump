using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	/// <summary>Discovers the attributes of a local variable and provides access to local variable metadata.</summary>
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public class LocalVariableInfo
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.LocalVariableInfo" /> class.</summary>
		protected LocalVariableInfo()
		{
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the object referred to by the local variable is pinned in memory.</summary>
		/// <returns>
		///   <see langword="true" /> if the object referred to by the variable is pinned in memory; otherwise, <see langword="false" />.</returns>
		public virtual bool IsPinned
		{
			get
			{
				return this.is_pinned;
			}
		}

		/// <summary>Gets the index of the local variable within the method body.</summary>
		/// <returns>An integer value that represents the order of declaration of the local variable within the method body.</returns>
		public virtual int LocalIndex
		{
			get
			{
				return (int)this.position;
			}
		}

		/// <summary>Gets the type of the local variable.</summary>
		/// <returns>The type of the local variable.</returns>
		public virtual Type LocalType
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>Returns a user-readable string that describes the local variable.</summary>
		/// <returns>A string that displays information about the local variable, including the type name, index, and pinned status.</returns>
		public override string ToString()
		{
			if (this.is_pinned)
			{
				return string.Format("{0} ({1}) (pinned)", this.type, this.position);
			}
			return string.Format("{0} ({1})", this.type, this.position);
		}

		internal Type type;

		internal bool is_pinned;

		internal ushort position;
	}
}
