using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public abstract class DataSource<TData> : MonoBehaviour, IDataSource<TData>, IDataSource where TData : class, ICopyFrom<TData>, new()
	{
		public bool Started
		{
			get
			{
				return this._started;
			}
		}

		public DataSource<TData>.UpdateModeFlags UpdateMode
		{
			get
			{
				return this._updateMode;
			}
		}

		protected bool UpdateModeAfterPrevious
		{
			get
			{
				return (this._updateMode & DataSource<TData>.UpdateModeFlags.AfterPreviousStep) > DataSource<TData>.UpdateModeFlags.Manual;
			}
		}

		public event Action InputDataAvailable = delegate()
		{
		};

		public virtual int CurrentDataVersion
		{
			get
			{
				return this._currentDataVersion;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this._updateAfter != null)
			{
				this.UpdateAfter = (this._updateAfter as IDataSource);
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started && this.UpdateModeAfterPrevious && this.UpdateAfter != null)
			{
				this.UpdateAfter.InputDataAvailable += this.MarkInputDataRequiresUpdate;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started && this.UpdateAfter != null)
			{
				this.UpdateAfter.InputDataAvailable -= this.MarkInputDataRequiresUpdate;
			}
		}

		protected virtual void Update()
		{
			if ((this._updateMode & DataSource<TData>.UpdateModeFlags.UnityUpdate) != DataSource<TData>.UpdateModeFlags.Manual)
			{
				this.MarkInputDataRequiresUpdate();
			}
		}

		protected virtual void FixedUpdate()
		{
			if ((this._updateMode & DataSource<TData>.UpdateModeFlags.UnityFixedUpdate) != DataSource<TData>.UpdateModeFlags.Manual)
			{
				this.MarkInputDataRequiresUpdate();
			}
		}

		protected virtual void LateUpdate()
		{
			if ((this._updateMode & DataSource<TData>.UpdateModeFlags.UnityLateUpdate) != DataSource<TData>.UpdateModeFlags.Manual)
			{
				this.MarkInputDataRequiresUpdate();
			}
		}

		protected void ResetUpdateAfter(IDataSource updateAfter, DataSource<TData>.UpdateModeFlags updateMode)
		{
			bool isActiveAndEnabled = base.isActiveAndEnabled;
			if (base.isActiveAndEnabled)
			{
				this.OnDisable();
			}
			this._updateMode = updateMode;
			this.UpdateAfter = updateAfter;
			this._requiresUpdate = true;
			this._currentDataVersion++;
			if (isActiveAndEnabled)
			{
				this.OnEnable();
			}
		}

		public TData GetData()
		{
			if (this.RequiresUpdate())
			{
				this.UpdateData();
				this._requiresUpdate = false;
			}
			return this.DataAsset;
		}

		protected bool RequiresUpdate()
		{
			return this._requiresUpdate;
		}

		public virtual void MarkInputDataRequiresUpdate()
		{
			this._requiresUpdate = true;
			this._currentDataVersion++;
			this.InputDataAvailable();
		}

		protected abstract void UpdateData();

		protected abstract TData DataAsset { get; }

		public void InjectAllDataSource(DataSource<TData>.UpdateModeFlags updateMode, IDataSource updateAfter)
		{
			this.InjectUpdateMode(updateMode);
			this.InjectUpdateAfter(updateAfter);
		}

		public void InjectUpdateMode(DataSource<TData>.UpdateModeFlags updateMode)
		{
			this._updateMode = updateMode;
		}

		public void InjectUpdateAfter(IDataSource updateAfter)
		{
			this._updateAfter = (updateAfter as Object);
			this.UpdateAfter = updateAfter;
		}

		protected bool _started;

		private bool _requiresUpdate = true;

		[Header("Update")]
		[SerializeField]
		private DataSource<TData>.UpdateModeFlags _updateMode;

		[SerializeField]
		[Interface(typeof(IDataSource), new Type[]
		{

		})]
		[Optional(OptionalAttribute.Flag.DontHide)]
		private Object _updateAfter;

		private IDataSource UpdateAfter;

		private int _currentDataVersion;

		[Flags]
		public enum UpdateModeFlags
		{
			Manual = 0,
			UnityUpdate = 1,
			UnityFixedUpdate = 2,
			UnityLateUpdate = 4,
			AfterPreviousStep = 8
		}
	}
}
