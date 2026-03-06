using System;

namespace System.Data
{
	/// <summary>Provides data for the state change event of a .NET Framework data provider.</summary>
	public sealed class StateChangeEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.StateChangeEventArgs" /> class, when given the original state and the current state of the object.</summary>
		/// <param name="originalState">One of the <see cref="T:System.Data.ConnectionState" /> values.</param>
		/// <param name="currentState">One of the <see cref="T:System.Data.ConnectionState" /> values.</param>
		public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
		{
			this._originalState = originalState;
			this._currentState = currentState;
		}

		/// <summary>Gets the new state of the connection. The connection object will be in the new state already when the event is fired.</summary>
		/// <returns>One of the <see cref="T:System.Data.ConnectionState" /> values.</returns>
		public ConnectionState CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		/// <summary>Gets the original state of the connection.</summary>
		/// <returns>One of the <see cref="T:System.Data.ConnectionState" /> values.</returns>
		public ConnectionState OriginalState
		{
			get
			{
				return this._originalState;
			}
		}

		private ConnectionState _originalState;

		private ConnectionState _currentState;
	}
}
