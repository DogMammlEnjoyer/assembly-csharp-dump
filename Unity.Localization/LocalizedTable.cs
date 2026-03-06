using System;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization
{
	[Serializable]
	public abstract class LocalizedTable<TTable, TEntry> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		protected abstract LocalizedDatabase<TTable, TEntry> Database { get; }

		public AsyncOperationHandle<TTable> CurrentLoadingOperationHandle { get; internal set; }

		public TableReference TableReference
		{
			get
			{
				return this.m_TableReference;
			}
			set
			{
				if (value.Equals(this.m_TableReference))
				{
					return;
				}
				this.m_TableReference = value;
				this.ForceUpdate();
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.TableReference.ReferenceType == TableReference.Type.Empty;
			}
		}

		public event LocalizedTable<TTable, TEntry>.ChangeHandler TableChanged
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.m_ChangeHandler.Add(value, 5);
				if (this.m_ChangeHandler.Length == 1)
				{
					LocalizationSettings.ValidateSettingsExist("");
					LocalizationSettings.SelectedLocaleChanged += this.m_SelectedLocaleChanged;
					this.ForceUpdate();
					return;
				}
				if (this.CurrentLoadingOperationHandle.IsValid() && this.CurrentLoadingOperationHandle.IsDone)
				{
					value(this.CurrentLoadingOperationHandle.Result);
				}
			}
			remove
			{
				this.m_ChangeHandler.RemoveByMovingTail(value);
				if (this.m_ChangeHandler.Length == 0)
				{
					LocalizationSettings.SelectedLocaleChanged -= this.m_SelectedLocaleChanged;
					this.ClearLoadingOperation();
				}
			}
		}

		public LocalizedTable()
		{
			this.m_SelectedLocaleChanged = new Action<Locale>(this.HandleLocaleChange);
		}

		public AsyncOperationHandle<TTable> GetTableAsync()
		{
			return this.Database.GetTableAsync(this.TableReference, null);
		}

		public TTable GetTable()
		{
			return this.GetTableAsync().WaitForCompletion();
		}

		protected void ForceUpdate()
		{
			if (this.m_ChangeHandler.Length != 0)
			{
				this.HandleLocaleChange(null);
			}
		}

		private void InvokeChangeHandler(TTable value)
		{
			try
			{
				this.m_ChangeHandler.LockForChanges();
				int length = this.m_ChangeHandler.Length;
				if (length == 1)
				{
					this.m_ChangeHandler.SingleDelegate(value);
				}
				else if (length > 1)
				{
					LocalizedTable<TTable, TEntry>.ChangeHandler[] multiDelegates = this.m_ChangeHandler.MultiDelegates;
					for (int i = 0; i < length; i++)
					{
						multiDelegates[i](value);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			this.m_ChangeHandler.UnlockForChanges();
		}

		private void HandleLocaleChange(Locale _)
		{
			this.ClearLoadingOperation();
			if (this.IsEmpty)
			{
				return;
			}
			this.CurrentLoadingOperationHandle = this.GetTableAsync();
			if (this.CurrentLoadingOperationHandle.IsDone)
			{
				this.AutomaticLoadingCompleted(this.CurrentLoadingOperationHandle);
				return;
			}
			this.CurrentLoadingOperationHandle.Completed += this.AutomaticLoadingCompleted;
		}

		private void AutomaticLoadingCompleted(AsyncOperationHandle<TTable> loadOperation)
		{
			if (loadOperation.Status != AsyncOperationStatus.Succeeded)
			{
				this.CurrentLoadingOperationHandle = default(AsyncOperationHandle<TTable>);
				return;
			}
			this.InvokeChangeHandler(loadOperation.Result);
		}

		private void ClearLoadingOperation()
		{
			if (this.CurrentLoadingOperationHandle.IsValid())
			{
				if (!this.CurrentLoadingOperationHandle.IsDone)
				{
					this.CurrentLoadingOperationHandle.Completed -= this.AutomaticLoadingCompleted;
				}
				this.CurrentLoadingOperationHandle = default(AsyncOperationHandle<TTable>);
			}
		}

		public override string ToString()
		{
			return this.TableReference;
		}

		[Obsolete("CurrentLoadingOperation is deprecated, use CurrentLoadingOperationHandle instead.")]
		public AsyncOperationHandle<TTable>? CurrentLoadingOperation
		{
			get
			{
				return new AsyncOperationHandle<TTable>?(this.CurrentLoadingOperationHandle.IsValid() ? this.CurrentLoadingOperationHandle : default(AsyncOperationHandle<TTable>));
			}
		}

		[SerializeField]
		private TableReference m_TableReference;

		private CallbackArray<LocalizedTable<TTable, TEntry>.ChangeHandler> m_ChangeHandler;

		private Action<Locale> m_SelectedLocaleChanged;

		public delegate void ChangeHandler(TTable value);
	}
}
