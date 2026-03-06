using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public abstract class DataModifier<TData> : DataSource<TData> where TData : class, ICopyFrom<TData>, new()
	{
		private static TData InvalidAsset { get; } = Activator.CreateInstance<TData>();

		protected override TData DataAsset
		{
			get
			{
				return this._currentDataAsset;
			}
		}

		public virtual IDataSource<TData> ModifyDataFromSource
		{
			get
			{
				if (this._modifyDataFromSource != null)
				{
					return this._modifyDataFromSource;
				}
				return this._modifyDataFromSource = (this._iModifyDataFromSourceMono as IDataSource<TData>);
			}
		}

		public override int CurrentDataVersion
		{
			get
			{
				if (!this._applyModifier)
				{
					return this.ModifyDataFromSource.CurrentDataVersion;
				}
				return base.CurrentDataVersion;
			}
		}

		public void ResetSources(IDataSource<TData> modifyDataFromSource, IDataSource updateAfter, DataSource<TData>.UpdateModeFlags updateMode)
		{
			base.ResetUpdateAfter(updateAfter, updateMode);
			this._modifyDataFromSource = modifyDataFromSource;
			this._currentDataAsset = DataModifier<TData>.InvalidAsset;
		}

		protected override void UpdateData()
		{
			if (this._applyModifier)
			{
				if (this._thisDataAsset == null)
				{
					this._thisDataAsset = Activator.CreateInstance<TData>();
				}
				this._thisDataAsset.CopyFrom(this.ModifyDataFromSource.GetData());
				this._currentDataAsset = this._thisDataAsset;
				this.Apply(this._currentDataAsset);
				return;
			}
			this._currentDataAsset = this.ModifyDataFromSource.GetData();
		}

		protected abstract void Apply(TData data);

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		public void InjectAllDataModifier(DataSource<TData>.UpdateModeFlags updateMode, IDataSource updateAfter, IDataSource<TData> modifyDataFromSource, bool applyModifier)
		{
			base.InjectAllDataSource(updateMode, updateAfter);
			this.InjectModifyDataFromSource(modifyDataFromSource);
			this.InjectApplyModifier(applyModifier);
		}

		public void InjectModifyDataFromSource(IDataSource<TData> modifyDataFromSource)
		{
			this._modifyDataFromSource = modifyDataFromSource;
			this._iModifyDataFromSourceMono = (modifyDataFromSource as Object);
		}

		public void InjectApplyModifier(bool applyModifier)
		{
			this._applyModifier = applyModifier;
		}

		[Header("Data Modifier")]
		[SerializeField]
		[Interface("_modifyDataFromSource")]
		protected Object _iModifyDataFromSourceMono;

		private IDataSource<TData> _modifyDataFromSource;

		[SerializeField]
		[Tooltip("If this is false, then this modifier will simply pass through data without performing any modification. This saves on memory and computation")]
		protected bool _applyModifier = true;

		private TData _thisDataAsset;

		private TData _currentDataAsset = DataModifier<TData>.InvalidAsset;
	}
}
