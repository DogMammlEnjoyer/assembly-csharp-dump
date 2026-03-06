using System;
using System.Collections;

namespace System.ComponentModel
{
	/// <summary>Defines members that data entity classes can implement to provide custom synchronous and asynchronous validation support.</summary>
	public interface INotifyDataErrorInfo
	{
		/// <summary>Gets a value that indicates whether the entity has validation errors.</summary>
		/// <returns>
		///   <see langword="true" /> if the entity currently has validation errors; otherwise, <see langword="false" />.</returns>
		bool HasErrors { get; }

		/// <summary>Gets the validation errors for a specified property or for the entire entity.</summary>
		/// <param name="propertyName">The name of the property to retrieve validation errors for; or <see langword="null" /> or <see cref="F:System.String.Empty" />, to retrieve entity-level errors.</param>
		/// <returns>The validation errors for the property or entity.</returns>
		IEnumerable GetErrors(string propertyName);

		/// <summary>Occurs when the validation errors have changed for a property or for the entire entity.</summary>
		event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
	}
}
