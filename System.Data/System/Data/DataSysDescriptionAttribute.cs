using System;
using System.ComponentModel;

namespace System.Data
{
	/// <summary>Marks a property, event, or extender with a description. Visual designers can display this description when referencing the member.</summary>
	[Obsolete("DataSysDescriptionAttribute has been deprecated.  https://go.microsoft.com/fwlink/?linkid=14202", false)]
	[AttributeUsage(AttributeTargets.All)]
	public class DataSysDescriptionAttribute : DescriptionAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataSysDescriptionAttribute" /> class using the specified description string.</summary>
		/// <param name="description">The description string.</param>
		[Obsolete("DataSysDescriptionAttribute has been deprecated.  https://go.microsoft.com/fwlink/?linkid=14202", false)]
		public DataSysDescriptionAttribute(string description) : base(description)
		{
		}

		/// <summary>Gets the text for the description.</summary>
		/// <returns>The description string.</returns>
		public override string Description
		{
			get
			{
				if (!this._replaced)
				{
					this._replaced = true;
					base.DescriptionValue = base.Description;
				}
				return base.Description;
			}
		}

		private bool _replaced;
	}
}
