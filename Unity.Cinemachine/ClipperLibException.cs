using System;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	internal class ClipperLibException : Exception
	{
		[NullableContext(1)]
		public ClipperLibException(string description) : base(description)
		{
		}
	}
}
