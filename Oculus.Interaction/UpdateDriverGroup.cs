using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class UpdateDriverGroup : MonoBehaviour, IUpdateDriver
	{
		public bool IsRootDriver { get; set; } = true;

		public int Iterations
		{
			get
			{
				return this._iterations;
			}
			set
			{
				this._iterations = value;
			}
		}

		protected virtual void Awake()
		{
			this.Drivers = this._updateDrivers.ConvertAll<IUpdateDriver>((Object mono) => mono as IUpdateDriver);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			if (!this.IsRootDriver)
			{
				return;
			}
			this.Drive();
		}

		public void Drive()
		{
			for (int i = 0; i < this._iterations; i++)
			{
				foreach (IUpdateDriver updateDriver in this.Drivers)
				{
					updateDriver.Drive();
				}
			}
		}

		public void InjectAllUpdateDriverGroup(List<IUpdateDriver> updateDrivers)
		{
			this.InjectUpdateDrivers(updateDrivers);
		}

		public void InjectUpdateDrivers(List<IUpdateDriver> updateDrivers)
		{
			this.Drivers = updateDrivers;
			this._updateDrivers = updateDrivers.ConvertAll<Object>((IUpdateDriver driver) => driver as Object);
		}

		[SerializeField]
		[Interface(typeof(IUpdateDriver), new Type[]
		{

		})]
		private List<Object> _updateDrivers;

		protected List<IUpdateDriver> Drivers;

		[SerializeField]
		[Min(1f)]
		private int _iterations = 3;
	}
}
