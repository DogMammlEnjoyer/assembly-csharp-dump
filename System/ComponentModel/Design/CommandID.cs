using System;
using System.Globalization;

namespace System.ComponentModel.Design
{
	/// <summary>Represents a unique command identifier that consists of a numeric command ID and a GUID menu group identifier.</summary>
	public class CommandID
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.CommandID" /> class using the specified menu group GUID and command ID number.</summary>
		/// <param name="menuGroup">The GUID of the group that this menu command belongs to.</param>
		/// <param name="commandID">The numeric identifier of this menu command.</param>
		public CommandID(Guid menuGroup, int commandID)
		{
			this.Guid = menuGroup;
			this.ID = commandID;
		}

		/// <summary>Gets the numeric command ID.</summary>
		/// <returns>The command ID number.</returns>
		public virtual int ID { get; }

		/// <summary>Determines whether two <see cref="T:System.ComponentModel.Design.CommandID" /> instances are equal.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is equivalent to this one; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is CommandID))
			{
				return false;
			}
			CommandID commandID = (CommandID)obj;
			return commandID.Guid.Equals(this.Guid) && commandID.ID == this.ID;
		}

		/// <summary>Serves as a hash function for a particular type.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return this.Guid.GetHashCode() << 2 | this.ID;
		}

		/// <summary>Gets the GUID of the menu group that the menu command identified by this <see cref="T:System.ComponentModel.Design.CommandID" /> belongs to.</summary>
		/// <returns>The GUID of the command group for this command.</returns>
		public virtual Guid Guid { get; }

		/// <summary>Returns a <see cref="T:System.String" /> that represents the current object.</summary>
		/// <returns>A string that contains the command ID information, both the GUID and integer identifier.</returns>
		public override string ToString()
		{
			return this.Guid.ToString() + " : " + this.ID.ToString(CultureInfo.CurrentCulture);
		}
	}
}
