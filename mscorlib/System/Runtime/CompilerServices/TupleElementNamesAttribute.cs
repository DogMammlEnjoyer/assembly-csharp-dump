using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
	/// <summary>Indicates that the use of a value tuple on a member is meant to be treated as a tuple with element names.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	[CLSCompliant(false)]
	public sealed class TupleElementNamesAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.TupleElementNamesAttribute" /> class.</summary>
		/// <param name="transformNames">A string array that specifies, in a pre-order depth-first traversal of a type's construction, which value tuple occurrences are meant to carry element names.</param>
		public TupleElementNamesAttribute(string[] transformNames)
		{
			if (transformNames == null)
			{
				throw new ArgumentNullException("transformNames");
			}
			this._transformNames = transformNames;
		}

		/// <summary>Specifies, in a pre-order depth-first traversal of a type's construction, which value tuple elements are meant to carry element names.</summary>
		/// <returns>An array that indicates which value tuple elements are meant to carry element names.</returns>
		public IList<string> TransformNames
		{
			get
			{
				return this._transformNames;
			}
		}

		private readonly string[] _transformNames;
	}
}
