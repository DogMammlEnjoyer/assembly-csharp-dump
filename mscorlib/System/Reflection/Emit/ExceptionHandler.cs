using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>Represents an exception handler in a byte array of IL to be passed to a method such as <see cref="M:System.Reflection.Emit.MethodBuilder.SetMethodBody(System.Byte[],System.Int32,System.Byte[],System.Collections.Generic.IEnumerable{System.Reflection.Emit.ExceptionHandler},System.Collections.Generic.IEnumerable{System.Int32})" />.</summary>
	[ComVisible(false)]
	public readonly struct ExceptionHandler : IEquatable<ExceptionHandler>
	{
		/// <summary>Gets the token of the exception type handled by this handler.</summary>
		/// <returns>The token of the exception type handled by this handler, or 0 if none exists.</returns>
		public int ExceptionTypeToken
		{
			get
			{
				return this.m_exceptionClass;
			}
		}

		/// <summary>Gets the byte offset at which the code that is protected by this exception handler begins.</summary>
		/// <returns>The byte offset at which the code that is protected by this exception handler begins.</returns>
		public int TryOffset
		{
			get
			{
				return this.m_tryStartOffset;
			}
		}

		/// <summary>Gets the length, in bytes, of the code protected by this exception handler.</summary>
		/// <returns>The length, in bytes, of the code protected by this exception handler.</returns>
		public int TryLength
		{
			get
			{
				return this.m_tryEndOffset - this.m_tryStartOffset;
			}
		}

		/// <summary>Gets the byte offset at which the filter code for the exception handler begins.</summary>
		/// <returns>The byte offset at which the filter code begins, or 0 if no filter  is present.</returns>
		public int FilterOffset
		{
			get
			{
				return this.m_filterOffset;
			}
		}

		/// <summary>Gets the byte offset of the first instruction of the exception handler.</summary>
		/// <returns>The byte offset of the first instruction of the exception handler.</returns>
		public int HandlerOffset
		{
			get
			{
				return this.m_handlerStartOffset;
			}
		}

		/// <summary>Gets the length, in bytes, of the exception handler.</summary>
		/// <returns>The length, in bytes, of the exception handler.</returns>
		public int HandlerLength
		{
			get
			{
				return this.m_handlerEndOffset - this.m_handlerStartOffset;
			}
		}

		/// <summary>Gets a value that represents the kind of exception handler this object represents.</summary>
		/// <returns>One of the enumeration values that specifies the kind of exception handler.</returns>
		public ExceptionHandlingClauseOptions Kind
		{
			get
			{
				return this.m_kind;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.Emit.ExceptionHandler" /> class with the specified parameters.</summary>
		/// <param name="tryOffset">The byte offset of the first instruction protected by this exception handler.</param>
		/// <param name="tryLength">The number of bytes protected by this exception handler.</param>
		/// <param name="filterOffset">The byte offset of the beginning of the filter code. The filter code ends at the first instruction of the handler block. For non-filter exception handlers, specify 0 (zero) for this parameter.</param>
		/// <param name="handlerOffset">The byte offset of the first instruction of this exception handler.</param>
		/// <param name="handlerLength">The number of bytes in this exception handler.</param>
		/// <param name="kind">One of the enumeration values that specifies the kind of exception handler.</param>
		/// <param name="exceptionTypeToken">The token of the exception type handled by this exception handler. If not applicable, specify 0 (zero).</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="tryOffset" />, <paramref name="filterOffset" />, <paramref name="handlerOffset" />, <paramref name="tryLength" />, or <paramref name="handlerLength" /> are negative.</exception>
		public ExceptionHandler(int tryOffset, int tryLength, int filterOffset, int handlerOffset, int handlerLength, ExceptionHandlingClauseOptions kind, int exceptionTypeToken)
		{
			if (tryOffset < 0)
			{
				throw new ArgumentOutOfRangeException("tryOffset", Environment.GetResourceString("Non-negative number required."));
			}
			if (tryLength < 0)
			{
				throw new ArgumentOutOfRangeException("tryLength", Environment.GetResourceString("Non-negative number required."));
			}
			if (filterOffset < 0)
			{
				throw new ArgumentOutOfRangeException("filterOffset", Environment.GetResourceString("Non-negative number required."));
			}
			if (handlerOffset < 0)
			{
				throw new ArgumentOutOfRangeException("handlerOffset", Environment.GetResourceString("Non-negative number required."));
			}
			if (handlerLength < 0)
			{
				throw new ArgumentOutOfRangeException("handlerLength", Environment.GetResourceString("Non-negative number required."));
			}
			if ((long)tryOffset + (long)tryLength > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("tryLength", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					0,
					int.MaxValue - tryOffset
				}));
			}
			if ((long)handlerOffset + (long)handlerLength > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("handlerLength", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					0,
					int.MaxValue - handlerOffset
				}));
			}
			if (kind == ExceptionHandlingClauseOptions.Clause && (exceptionTypeToken & 16777215) == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Token {0:x} is not a valid Type token.", new object[]
				{
					exceptionTypeToken
				}), "exceptionTypeToken");
			}
			if (!ExceptionHandler.IsValidKind(kind))
			{
				throw new ArgumentOutOfRangeException("kind", Environment.GetResourceString("Enum value was out of legal range."));
			}
			this.m_tryStartOffset = tryOffset;
			this.m_tryEndOffset = tryOffset + tryLength;
			this.m_filterOffset = filterOffset;
			this.m_handlerStartOffset = handlerOffset;
			this.m_handlerEndOffset = handlerOffset + handlerLength;
			this.m_kind = kind;
			this.m_exceptionClass = exceptionTypeToken;
		}

		internal ExceptionHandler(int tryStartOffset, int tryEndOffset, int filterOffset, int handlerStartOffset, int handlerEndOffset, int kind, int exceptionTypeToken)
		{
			this.m_tryStartOffset = tryStartOffset;
			this.m_tryEndOffset = tryEndOffset;
			this.m_filterOffset = filterOffset;
			this.m_handlerStartOffset = handlerStartOffset;
			this.m_handlerEndOffset = handlerEndOffset;
			this.m_kind = (ExceptionHandlingClauseOptions)kind;
			this.m_exceptionClass = exceptionTypeToken;
		}

		private static bool IsValidKind(ExceptionHandlingClauseOptions kind)
		{
			return kind <= ExceptionHandlingClauseOptions.Finally || kind == ExceptionHandlingClauseOptions.Fault;
		}

		/// <summary>Serves as the default hash function.</summary>
		/// <returns>The hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.m_exceptionClass ^ this.m_tryStartOffset ^ this.m_tryEndOffset ^ this.m_filterOffset ^ this.m_handlerStartOffset ^ this.m_handlerEndOffset ^ (int)this.m_kind;
		}

		/// <summary>Indicates whether this instance of the <see cref="T:System.Reflection.Emit.ExceptionHandler" /> object is equal to a specified object.</summary>
		/// <param name="obj">The object to compare this instance to.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> and this instance are equal; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is ExceptionHandler && this.Equals((ExceptionHandler)obj);
		}

		/// <summary>Indicates whether this instance of the <see cref="T:System.Reflection.Emit.ExceptionHandler" /> object is equal to another <see cref="T:System.Reflection.Emit.ExceptionHandler" /> object.</summary>
		/// <param name="other">The exception handler object to compare this instance to.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="other" /> and this instance are equal; otherwise, <see langword="false" />.</returns>
		public bool Equals(ExceptionHandler other)
		{
			return other.m_exceptionClass == this.m_exceptionClass && other.m_tryStartOffset == this.m_tryStartOffset && other.m_tryEndOffset == this.m_tryEndOffset && other.m_filterOffset == this.m_filterOffset && other.m_handlerStartOffset == this.m_handlerStartOffset && other.m_handlerEndOffset == this.m_handlerEndOffset && other.m_kind == this.m_kind;
		}

		/// <summary>Determines whether two specified instances of <see cref="T:System.Reflection.Emit.ExceptionHandler" /> are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(ExceptionHandler left, ExceptionHandler right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether two specified instances of <see cref="T:System.Reflection.Emit.ExceptionHandler" /> are not equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(ExceptionHandler left, ExceptionHandler right)
		{
			return !left.Equals(right);
		}

		internal readonly int m_exceptionClass;

		internal readonly int m_tryStartOffset;

		internal readonly int m_tryEndOffset;

		internal readonly int m_filterOffset;

		internal readonly int m_handlerStartOffset;

		internal readonly int m_handlerEndOffset;

		internal readonly ExceptionHandlingClauseOptions m_kind;
	}
}
