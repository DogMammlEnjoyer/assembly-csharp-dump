using System;

namespace System.Data
{
	/// <summary>Provides data for the <see cref="E:System.Data.Common.DataAdapter.FillError" /> event of a <see cref="T:System.Data.Common.DbDataAdapter" />.</summary>
	public class FillErrorEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.FillErrorEventArgs" /> class.</summary>
		/// <param name="dataTable">The <see cref="T:System.Data.DataTable" /> being updated.</param>
		/// <param name="values">The values for the row being updated.</param>
		public FillErrorEventArgs(DataTable dataTable, object[] values)
		{
			this._dataTable = dataTable;
			this._values = values;
			if (this._values == null)
			{
				this._values = Array.Empty<object>();
			}
		}

		/// <summary>Gets or sets a value indicating whether to continue the fill operation despite the error.</summary>
		/// <returns>
		///   <see langword="true" /> if the fill operation should continue; otherwise, <see langword="false" />.</returns>
		public bool Continue
		{
			get
			{
				return this._continueFlag;
			}
			set
			{
				this._continueFlag = value;
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.DataTable" /> being updated when the error occurred.</summary>
		/// <returns>The <see cref="T:System.Data.DataTable" /> being updated.</returns>
		public DataTable DataTable
		{
			get
			{
				return this._dataTable;
			}
		}

		/// <summary>Gets the errors being handled.</summary>
		/// <returns>The errors being handled.</returns>
		public Exception Errors
		{
			get
			{
				return this._errors;
			}
			set
			{
				this._errors = value;
			}
		}

		/// <summary>Gets the values for the row being updated when the error occurred.</summary>
		/// <returns>The values for the row being updated.</returns>
		public object[] Values
		{
			get
			{
				object[] array = new object[this._values.Length];
				for (int i = 0; i < this._values.Length; i++)
				{
					array[i] = this._values[i];
				}
				return array;
			}
		}

		private bool _continueFlag;

		private DataTable _dataTable;

		private Exception _errors;

		private object[] _values;
	}
}
