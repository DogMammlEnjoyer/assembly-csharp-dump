using System;

namespace System.Reflection
{
	/// <summary>Attaches a modifier to parameters so that binding can work with parameter signatures in which the types have been modified.</summary>
	public readonly struct ParameterModifier
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.ParameterModifier" /> structure representing the specified number of parameters.</summary>
		/// <param name="parameterCount">The number of parameters.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="parameterCount" /> is negative.</exception>
		public ParameterModifier(int parameterCount)
		{
			if (parameterCount <= 0)
			{
				throw new ArgumentException("Must specify one or more parameters.");
			}
			this._byRef = new bool[parameterCount];
		}

		/// <summary>Gets or sets a value that specifies whether the parameter at the specified index position is to be modified by the current <see cref="T:System.Reflection.ParameterModifier" />.</summary>
		/// <param name="index">The index position of the parameter whose modification status is being examined or set.</param>
		/// <returns>
		///   <see langword="true" /> if the parameter at this index position is to be modified by this <see cref="T:System.Reflection.ParameterModifier" />; otherwise, <see langword="false" />.</returns>
		public bool this[int index]
		{
			get
			{
				return this._byRef[index];
			}
			set
			{
				this._byRef[index] = value;
			}
		}

		private readonly bool[] _byRef;
	}
}
