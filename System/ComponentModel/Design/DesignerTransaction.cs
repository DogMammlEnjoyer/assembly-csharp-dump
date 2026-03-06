using System;

namespace System.ComponentModel.Design
{
	/// <summary>Provides a way to group a series of design-time actions to improve performance and enable most types of changes to be undone.</summary>
	public abstract class DesignerTransaction : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.DesignerTransaction" /> class with no description.</summary>
		protected DesignerTransaction() : this("")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.DesignerTransaction" /> class using the specified transaction description.</summary>
		/// <param name="description">A description for this transaction.</param>
		protected DesignerTransaction(string description)
		{
			this.Description = description;
		}

		/// <summary>Gets a value indicating whether the transaction was canceled.</summary>
		/// <returns>
		///   <see langword="true" /> if the transaction was canceled; otherwise, <see langword="false" />.</returns>
		public bool Canceled { get; private set; }

		/// <summary>Gets a value indicating whether the transaction was committed.</summary>
		/// <returns>
		///   <see langword="true" /> if the transaction was committed; otherwise, <see langword="false" />.</returns>
		public bool Committed { get; private set; }

		/// <summary>Gets a description for the transaction.</summary>
		/// <returns>A description for the transaction.</returns>
		public string Description { get; }

		/// <summary>Cancels the transaction and attempts to roll back the changes made by the events of the transaction.</summary>
		public void Cancel()
		{
			if (!this.Canceled && !this.Committed)
			{
				this.Canceled = true;
				GC.SuppressFinalize(this);
				this._suppressedFinalization = true;
				this.OnCancel();
			}
		}

		/// <summary>Commits this transaction.</summary>
		public void Commit()
		{
			if (!this.Committed && !this.Canceled)
			{
				this.Committed = true;
				GC.SuppressFinalize(this);
				this._suppressedFinalization = true;
				this.OnCommit();
			}
		}

		/// <summary>Raises the <see langword="Cancel" /> event.</summary>
		protected abstract void OnCancel();

		/// <summary>Performs the actual work of committing a transaction.</summary>
		protected abstract void OnCommit();

		/// <summary>Releases the resources associated with this object. This override commits this transaction if it was not already committed.</summary>
		~DesignerTransaction()
		{
			this.Dispose(false);
		}

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Design.DesignerTransaction" />.</summary>
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			if (!this._suppressedFinalization)
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Design.DesignerTransaction" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			this.Cancel();
		}

		private bool _suppressedFinalization;
	}
}
