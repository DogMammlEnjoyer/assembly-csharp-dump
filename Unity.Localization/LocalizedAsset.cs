using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Localization.Operations;
using UnityEngine.Localization.Settings;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	[Serializable]
	public class LocalizedAsset<TObject> : LocalizedAssetBase, IDisposable where TObject : Object
	{
		public override bool WaitForCompletion
		{
			set
			{
				if (value == this.WaitForCompletion)
				{
					return;
				}
				base.WaitForCompletion = value;
				if (value && this.CurrentLoadingOperationHandle.IsValid() && !this.CurrentLoadingOperationHandle.IsDone)
				{
					this.CurrentLoadingOperationHandle.WaitForCompletion();
				}
			}
		}

		internal override bool ForceSynchronous
		{
			get
			{
				return this.WaitForCompletion || LocalizationSettings.AssetDatabase.AsynchronousBehaviour == AsynchronousBehaviour.ForceSynchronous;
			}
		}

		public AsyncOperationHandle<TObject> CurrentLoadingOperationHandle { get; internal set; }

		public event LocalizedAsset<TObject>.ChangeHandler AssetChanged
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
					this.ForceUpdate();
					LocalizationSettings.SelectedLocaleChanged += this.m_SelectedLocaleChanged;
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

		public bool HasChangeHandler
		{
			get
			{
				return this.m_ChangeHandler.Length != 0;
			}
		}

		public LocalizedAsset()
		{
			this.m_SelectedLocaleChanged = new Action<Locale>(this.HandleLocaleChange);
			this.m_AutomaticLoadingCompleted = new Action<AsyncOperationHandle<TObject>>(this.AutomaticLoadingCompleted);
		}

		public AsyncOperationHandle<TObject> LoadAssetAsync()
		{
			return this.LoadAssetAsync<TObject>();
		}

		public override AsyncOperationHandle<T> LoadAssetAsync<T>()
		{
			LocalizationSettings.ValidateSettingsExist("Can not Load Asset.");
			return LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<T>(base.TableReference, base.TableEntryReference, base.LocaleOverride, FallbackBehavior.UseProjectSettings);
		}

		public override AsyncOperationHandle<Object> LoadAssetAsObjectAsync()
		{
			AsyncOperationHandle<TObject> operation = this.LoadAssetAsync();
			LocalizedAsset<TObject>.ConvertToObjectOperation convertToObjectOperation = LocalizedAsset<TObject>.ConvertToObjectOperation.Pool.Get();
			convertToObjectOperation.Init(operation);
			return AddressablesInterface.ResourceManager.StartOperation<Object>(convertToObjectOperation, default(AsyncOperationHandle));
		}

		public TObject LoadAsset()
		{
			return this.LoadAssetAsync().WaitForCompletion();
		}

		protected internal override void ForceUpdate()
		{
			if (this.m_ChangeHandler.Length != 0)
			{
				this.HandleLocaleChange(null);
			}
		}

		private void HandleLocaleChange(Locale locale)
		{
			if (base.IsEmpty)
			{
				this.ClearLoadingOperation();
				return;
			}
			this.m_PreviousLoadingOperation = this.CurrentLoadingOperationHandle;
			this.CurrentLoadingOperationHandle = this.LoadAssetAsync();
			AddressablesInterface.Acquire(this.CurrentLoadingOperationHandle);
			if (!this.CurrentLoadingOperationHandle.IsDone)
			{
				if (!this.WaitForCompletion && LocalizationSettings.AssetDatabase.AsynchronousBehaviour != AsynchronousBehaviour.ForceSynchronous)
				{
					this.CurrentLoadingOperationHandle.Completed += this.m_AutomaticLoadingCompleted;
					return;
				}
				this.CurrentLoadingOperationHandle.WaitForCompletion();
			}
			this.AutomaticLoadingCompleted(this.CurrentLoadingOperationHandle);
		}

		private void AutomaticLoadingCompleted(AsyncOperationHandle<TObject> loadOperation)
		{
			if (loadOperation.Status != AsyncOperationStatus.Succeeded)
			{
				this.CurrentLoadingOperationHandle = default(AsyncOperationHandle<TObject>);
				return;
			}
			this.InvokeChangeHandler(loadOperation.Result);
			this.ClearPreviousLoadingOperation();
		}

		private void InvokeChangeHandler(TObject value)
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
					LocalizedAsset<TObject>.ChangeHandler[] multiDelegates = this.m_ChangeHandler.MultiDelegates;
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

		internal void ClearLoadingOperation()
		{
			this.ClearLoadingOperation(this.CurrentLoadingOperationHandle);
			this.CurrentLoadingOperationHandle = default(AsyncOperationHandle<TObject>);
		}

		private void ClearPreviousLoadingOperation()
		{
			this.ClearLoadingOperation(this.m_PreviousLoadingOperation);
			this.m_PreviousLoadingOperation = default(AsyncOperationHandle<TObject>);
		}

		private void ClearLoadingOperation(AsyncOperationHandle<TObject> operationHandle)
		{
			if (operationHandle.IsValid())
			{
				if (!operationHandle.IsDone)
				{
					operationHandle.Completed -= this.m_AutomaticLoadingCompleted;
				}
				AddressablesInterface.Release(operationHandle);
			}
		}

		protected override void Reset()
		{
			this.ClearLoadingOperation();
		}

		~LocalizedAsset()
		{
			this.ClearLoadingOperation();
		}

		void IDisposable.Dispose()
		{
			this.m_ChangeHandler.Clear();
			LocalizationSettings.SelectedLocaleChanged -= this.m_SelectedLocaleChanged;
			this.ClearLoadingOperation();
			GC.SuppressFinalize(this);
		}

		protected override void Initialize()
		{
			this.AssetChanged += this.UpdateBindingValue;
		}

		protected override void Cleanup()
		{
			this.AssetChanged -= this.UpdateBindingValue;
		}

		protected override BindingResult Update(in BindingContext context)
		{
			if (base.IsEmpty)
			{
				return new BindingResult(BindingStatus.Success, null);
			}
			if (!this.CurrentLoadingOperationHandle.IsDone)
			{
				return new BindingResult(BindingStatus.Pending, null);
			}
			return this.ApplyDataBindingValue(context, this.m_CurrentValue);
		}

		protected virtual BindingResult ApplyDataBindingValue(in BindingContext context, TObject value)
		{
			return this.SetDataBindingValue<TObject>(context, value);
		}

		internal BindingResult SetDataBindingValue<T>(in BindingContext context, T value)
		{
			VisualElement targetElement = context.targetElement;
			BindingId bindingId = context.bindingId;
			PropertyPath propertyPath = bindingId;
			VisitReturnCode errorCode;
			if (ConverterGroups.TrySetValueGlobal<VisualElement, T>(ref targetElement, propertyPath, value, out errorCode))
			{
				return new BindingResult(BindingStatus.Success, null);
			}
			return base.CreateErrorResult(context, errorCode, typeof(TObject));
		}

		private void UpdateBindingValue(TObject value)
		{
			this.m_CurrentValue = value;
			base.MarkDirty();
		}

		private CallbackArray<LocalizedAsset<TObject>.ChangeHandler> m_ChangeHandler;

		private Action<Locale> m_SelectedLocaleChanged;

		private Action<AsyncOperationHandle<TObject>> m_AutomaticLoadingCompleted;

		private AsyncOperationHandle<TObject> m_PreviousLoadingOperation;

		private TObject m_CurrentValue;

		public delegate void ChangeHandler(TObject value);

		private class ConvertToObjectOperation : WaitForCurrentOperationAsyncOperationBase<Object>
		{
			public void Init(AsyncOperationHandle<TObject> operation)
			{
				AddressablesInterface.ResourceManager.Acquire<TObject>(operation);
				this.m_Operation = operation;
				base.CurrentOperation = operation;
			}

			protected override void Execute()
			{
				if (this.m_Operation.IsDone)
				{
					this.OnCompleted(this.m_Operation);
					return;
				}
				this.m_Operation.Completed += this.OnCompleted;
			}

			private void OnCompleted(AsyncOperationHandle<TObject> op)
			{
				base.Complete(op.Result, op.Status == AsyncOperationStatus.Succeeded, null);
			}

			protected override void Destroy()
			{
				AddressablesInterface.Release(this.m_Operation);
				LocalizedAsset<TObject>.ConvertToObjectOperation.Pool.Release(this);
			}

			public static readonly ObjectPool<LocalizedAsset<TObject>.ConvertToObjectOperation> Pool = new ObjectPool<LocalizedAsset<TObject>.ConvertToObjectOperation>(() => new LocalizedAsset<TObject>.ConvertToObjectOperation(), null, null, null, false, 10, 10000);

			private AsyncOperationHandle<TObject> m_Operation;
		}

		[CompilerGenerated]
		[Serializable]
		public new class UxmlSerializedData : LocalizedAssetBase.UxmlSerializedData
		{
			public override object CreateInstance()
			{
				return new LocalizedAsset<TObject>();
			}
		}
	}
}
