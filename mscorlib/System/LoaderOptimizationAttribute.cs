using System;

namespace System
{
	/// <summary>Used to set the default loader optimization policy for the main method of an executable application.</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class LoaderOptimizationAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.LoaderOptimizationAttribute" /> class to the specified value.</summary>
		/// <param name="value">A value equivalent to a <see cref="T:System.LoaderOptimization" /> constant.</param>
		public LoaderOptimizationAttribute(byte value)
		{
			this._val = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.LoaderOptimizationAttribute" /> class to the specified value.</summary>
		/// <param name="value">A <see cref="T:System.LoaderOptimization" /> constant.</param>
		public LoaderOptimizationAttribute(LoaderOptimization value)
		{
			this._val = (byte)value;
		}

		/// <summary>Gets the current <see cref="T:System.LoaderOptimization" /> value for this instance.</summary>
		/// <returns>A <see cref="T:System.LoaderOptimization" /> constant.</returns>
		public LoaderOptimization Value
		{
			get
			{
				return (LoaderOptimization)this._val;
			}
		}

		private readonly byte _val;
	}
}
