using System;

namespace System.Reflection
{
	/// <summary>Defines a copyright custom attribute for an assembly manifest.</summary>
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCopyrightAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.AssemblyCopyrightAttribute" /> class.</summary>
		/// <param name="copyright">The copyright information.</param>
		public AssemblyCopyrightAttribute(string copyright)
		{
			this.Copyright = copyright;
		}

		/// <summary>Gets copyright information.</summary>
		/// <returns>A string containing the copyright information.</returns>
		public string Copyright { get; }
	}
}
