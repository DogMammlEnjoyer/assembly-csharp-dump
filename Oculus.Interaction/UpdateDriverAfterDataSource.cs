using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class UpdateDriverAfterDataSource : MonoBehaviour, IUpdateDriver
	{
		protected virtual void Awake()
		{
			this.UpdateDriver = (this._updateDriver as IUpdateDriver);
			this.DataSource = (this._dataSource as IDataSource);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.DataSource.InputDataAvailable += this.Drive;
				this.UpdateDriver.IsRootDriver = false;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.DataSource.InputDataAvailable -= this.Drive;
				this.UpdateDriver.IsRootDriver = true;
			}
		}

		public bool IsRootDriver { get; set; } = true;

		public void Drive()
		{
			this.UpdateDriver.Drive();
		}

		public void InjectAllUpdateDriverAfterDataSource(IUpdateDriver updateDriver, IDataSource dataSource)
		{
			this.InjectUpdateDriver(updateDriver);
			this.InjectDataSource(dataSource);
		}

		public void InjectUpdateDriver(IUpdateDriver updateDriver)
		{
			this.UpdateDriver = updateDriver;
			this._updateDriver = (updateDriver as Object);
		}

		public void InjectDataSource(IDataSource dataSource)
		{
			this.DataSource = dataSource;
			this._dataSource = (dataSource as Object);
		}

		[SerializeField]
		[Interface(typeof(IUpdateDriver), new Type[]
		{

		})]
		private Object _updateDriver;

		private IUpdateDriver UpdateDriver;

		[SerializeField]
		[Interface(typeof(IDataSource), new Type[]
		{

		})]
		private Object _dataSource;

		private IDataSource DataSource;

		protected bool _started;
	}
}
