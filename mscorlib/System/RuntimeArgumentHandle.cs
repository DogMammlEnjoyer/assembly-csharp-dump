using System;

namespace System
{
	/// <summary>References a variable-length argument list.</summary>
	public ref struct RuntimeArgumentHandle
	{
		internal IntPtr args;
	}
}
