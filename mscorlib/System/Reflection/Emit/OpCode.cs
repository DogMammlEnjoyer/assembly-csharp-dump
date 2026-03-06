using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>Describes an intermediate language (IL) instruction.</summary>
	[ComVisible(true)]
	public readonly struct OpCode : IEquatable<OpCode>
	{
		internal OpCode(int p, int q)
		{
			this.op1 = (byte)(p & 255);
			this.op2 = (byte)(p >> 8 & 255);
			this.push = (byte)(p >> 16 & 255);
			this.pop = (byte)(p >> 24 & 255);
			this.size = (byte)(q & 255);
			this.type = (byte)(q >> 8 & 255);
			this.args = (byte)(q >> 16 & 255);
			this.flow = (byte)(q >> 24 & 255);
		}

		/// <summary>Returns the generated hash code for this <see langword="Opcode" />.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		/// <summary>Tests whether the given object is equal to this <see langword="Opcode" />.</summary>
		/// <param name="obj">The object to compare to this object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see langword="Opcode" /> and is equal to this object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is OpCode))
			{
				return false;
			}
			OpCode opCode = (OpCode)obj;
			return opCode.op1 == this.op1 && opCode.op2 == this.op2;
		}

		/// <summary>Indicates whether the current instance is equal to the specified <see cref="T:System.Reflection.Emit.OpCode" />.</summary>
		/// <param name="obj">The <see cref="T:System.Reflection.Emit.OpCode" /> to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="obj" /> is equal to the value of the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(OpCode obj)
		{
			return obj.op1 == this.op1 && obj.op2 == this.op2;
		}

		/// <summary>Returns this <see langword="Opcode" /> as a <see cref="T:System.String" />.</summary>
		/// <returns>A string containing the name of this <see langword="Opcode" />.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>The name of the intermediate language (IL) instruction.</summary>
		/// <returns>Read-only. The name of the IL instruction.</returns>
		public string Name
		{
			get
			{
				if (this.op1 == 255)
				{
					return OpCodeNames.names[(int)this.op2];
				}
				return OpCodeNames.names[256 + (int)this.op2];
			}
		}

		/// <summary>The size of the intermediate language (IL) instruction.</summary>
		/// <returns>Read-only. The size of the IL instruction.</returns>
		public int Size
		{
			get
			{
				return (int)this.size;
			}
		}

		/// <summary>The type of intermediate language (IL) instruction.</summary>
		/// <returns>Read-only. The type of intermediate language (IL) instruction.</returns>
		public OpCodeType OpCodeType
		{
			get
			{
				return (OpCodeType)this.type;
			}
		}

		/// <summary>The operand type of an intermediate language (IL) instruction.</summary>
		/// <returns>Read-only. The operand type of an IL instruction.</returns>
		public OperandType OperandType
		{
			get
			{
				return (OperandType)this.args;
			}
		}

		/// <summary>The flow control characteristics of the intermediate language (IL) instruction.</summary>
		/// <returns>Read-only. The type of flow control.</returns>
		public FlowControl FlowControl
		{
			get
			{
				return (FlowControl)this.flow;
			}
		}

		/// <summary>How the intermediate language (IL) instruction pops the stack.</summary>
		/// <returns>Read-only. The way the IL instruction pops the stack.</returns>
		public StackBehaviour StackBehaviourPop
		{
			get
			{
				return (StackBehaviour)this.pop;
			}
		}

		/// <summary>How the intermediate language (IL) instruction pushes operand onto the stack.</summary>
		/// <returns>Read-only. The way the IL instruction pushes operand onto the stack.</returns>
		public StackBehaviour StackBehaviourPush
		{
			get
			{
				return (StackBehaviour)this.push;
			}
		}

		/// <summary>Gets the numeric value of the intermediate language (IL) instruction.</summary>
		/// <returns>Read-only. The numeric value of the IL instruction.</returns>
		public short Value
		{
			get
			{
				if (this.size == 1)
				{
					return (short)this.op2;
				}
				return (short)((int)this.op1 << 8 | (int)this.op2);
			}
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.OpCode" /> structures are equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.OpCode" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.OpCode" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(OpCode a, OpCode b)
		{
			return a.op1 == b.op1 && a.op2 == b.op2;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.OpCode" /> structures are not equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.OpCode" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.OpCode" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(OpCode a, OpCode b)
		{
			return a.op1 != b.op1 || a.op2 != b.op2;
		}

		internal readonly byte op1;

		internal readonly byte op2;

		private readonly byte push;

		private readonly byte pop;

		private readonly byte size;

		private readonly byte type;

		private readonly byte args;

		private readonly byte flow;
	}
}
