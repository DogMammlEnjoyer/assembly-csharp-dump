using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ArrayLengthAttribute : DecoratingPropertyAttribute
	{
		public ArrayLengthAttribute(int length)
		{
			this.MaxLength = length;
			this.MinLength = length;
		}

		public ArrayLengthAttribute(int minLength, int maxLength)
		{
			this.MinLength = minLength;
			this.MaxLength = maxLength;
		}

		public int MinLength { get; }

		public int MaxLength { get; }
	}
}
