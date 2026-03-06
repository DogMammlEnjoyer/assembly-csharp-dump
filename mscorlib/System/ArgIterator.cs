using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	/// <summary>Represents a variable-length argument list; that is, the parameters of a function that takes a variable number of arguments.</summary>
	[StructLayout(LayoutKind.Auto)]
	public struct ArgIterator
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Setup(IntPtr argsp, IntPtr start);

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgIterator" /> structure using the specified argument list.</summary>
		/// <param name="arglist">An argument list consisting of mandatory and optional arguments.</param>
		public ArgIterator(RuntimeArgumentHandle arglist)
		{
			this.sig = IntPtr.Zero;
			this.args = IntPtr.Zero;
			this.next_arg = (this.num_args = 0);
			if (arglist.args == IntPtr.Zero)
			{
				throw new PlatformNotSupportedException();
			}
			this.Setup(arglist.args, IntPtr.Zero);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ArgIterator" /> structure using the specified argument list and a pointer to an item in the list.</summary>
		/// <param name="arglist">An argument list consisting of mandatory and optional arguments.</param>
		/// <param name="ptr">A pointer to the argument in <paramref name="arglist" /> to access first, or the first mandatory argument in <paramref name="arglist" /> if <paramref name="ptr" /> is <see langword="null" />.</param>
		[CLSCompliant(false)]
		public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr)
		{
			this.sig = IntPtr.Zero;
			this.args = IntPtr.Zero;
			this.next_arg = (this.num_args = 0);
			if (arglist.args == IntPtr.Zero)
			{
				throw new PlatformNotSupportedException();
			}
			this.Setup(arglist.args, (IntPtr)ptr);
		}

		/// <summary>Concludes processing of the variable-length argument list represented by this instance.</summary>
		public void End()
		{
			this.next_arg = this.num_args;
		}

		/// <summary>This method is not supported, and always throws <see cref="T:System.NotSupportedException" />.</summary>
		/// <param name="o">An object to be compared to this instance.</param>
		/// <returns>This comparison is not supported. No value is returned.</returns>
		/// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
		public override bool Equals(object o)
		{
			throw new NotSupportedException("ArgIterator does not support Equals.");
		}

		/// <summary>Returns the hash code of this object.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.sig.GetHashCode();
		}

		/// <summary>Returns the next argument in a variable-length argument list.</summary>
		/// <returns>The next argument as a <see cref="T:System.TypedReference" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read beyond the end of the list.</exception>
		[CLSCompliant(false)]
		public unsafe TypedReference GetNextArg()
		{
			if (this.num_args == this.next_arg)
			{
				throw new InvalidOperationException("Invalid iterator position.");
			}
			TypedReference result = default(TypedReference);
			this.IntGetNextArg((void*)(&result));
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void IntGetNextArg(void* res);

		/// <summary>Returns the next argument in a variable-length argument list that has a specified type.</summary>
		/// <param name="rth">A runtime type handle that identifies the type of the argument to retrieve.</param>
		/// <returns>The next argument as a <see cref="T:System.TypedReference" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read beyond the end of the list.</exception>
		/// <exception cref="T:System.ArgumentNullException">The pointer to the remaining arguments is zero.</exception>
		[CLSCompliant(false)]
		public unsafe TypedReference GetNextArg(RuntimeTypeHandle rth)
		{
			if (this.num_args == this.next_arg)
			{
				throw new InvalidOperationException("Invalid iterator position.");
			}
			TypedReference result = default(TypedReference);
			this.IntGetNextArgWithType((void*)(&result), rth.Value);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void IntGetNextArgWithType(void* res, IntPtr rth);

		/// <summary>Returns the type of the next argument.</summary>
		/// <returns>The type of the next argument.</returns>
		public RuntimeTypeHandle GetNextArgType()
		{
			if (this.num_args == this.next_arg)
			{
				throw new InvalidOperationException("Invalid iterator position.");
			}
			return new RuntimeTypeHandle(this.IntGetNextArgType());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr IntGetNextArgType();

		/// <summary>Returns the number of arguments remaining in the argument list.</summary>
		/// <returns>The number of remaining arguments.</returns>
		public int GetRemainingCount()
		{
			return this.num_args - this.next_arg;
		}

		private IntPtr sig;

		private IntPtr args;

		private int next_arg;

		private int num_args;
	}
}
