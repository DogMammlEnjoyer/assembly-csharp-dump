using System;

namespace System.ComponentModel
{
	/// <summary>Provides data for the <see cref="E:System.ComponentModel.INotifyDataErrorInfo.ErrorsChanged" /> event.</summary>
	public class DataErrorsChangedEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataErrorsChangedEventArgs" /> class.</summary>
		/// <param name="propertyName">The name of the property that has an error.  <see langword="null" /> or <see cref="F:System.String.Empty" /> if the error is object-level.</param>
		public DataErrorsChangedEventArgs(string propertyName)
		{
			this._propertyName = propertyName;
		}

		/// <summary>Gets the name of the property that has an error.</summary>
		/// <returns>The name of the property that has an error. <see langword="null" /> or <see cref="F:System.String.Empty" /> if the error is object-level.</returns>
		public virtual string PropertyName
		{
			get
			{
				return this._propertyName;
			}
		}

		private readonly string _propertyName;
	}
}
