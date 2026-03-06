using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class HandSourceInjector : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			HandSourceInjector.ActiveDataSource[] sources = this._sources;
			for (int i = 0; i < sources.Length; i++)
			{
				sources[i].Initialize();
			}
			this.UpdateActiveSource();
			if (this._activeDataSource == null)
			{
				this.ApplySource(this._sources[0]);
			}
			this.EndStart(ref this._started);
		}

		private void Update()
		{
			if (!this._activeDataSource.IsActive())
			{
				this.UpdateActiveSource();
			}
		}

		private void UpdateActiveSource()
		{
			foreach (HandSourceInjector.ActiveDataSource activeDataSource in this._sources)
			{
				if (activeDataSource.IsActive())
				{
					this.ApplySource(activeDataSource);
					return;
				}
			}
		}

		private void ApplySource(HandSourceInjector.ActiveDataSource activeDataSource)
		{
			this._activeDataSource = activeDataSource;
			this._targetHand.ResetSources(activeDataSource.Source, activeDataSource.ModifyAfter, this._targetHand.UpdateMode);
		}

		[SerializeField]
		private Hand _targetHand;

		[SerializeField]
		private HandSourceInjector.ActiveDataSource[] _sources;

		private HandSourceInjector.ActiveDataSource _activeDataSource;

		protected bool _started;

		[Serializable]
		private class ActiveDataSource
		{
			public IDataSource<HandDataAsset> Source { get; private set; }

			public IDataSource ModifyAfter { get; private set; }

			public void Initialize()
			{
				this.Source = (this._source as IDataSource<HandDataAsset>);
				this.ModifyAfter = (this._modifyAfter as IDataSource);
				HandSourceInjector.ActiveDataSource.<Initialize>g__AssertField|10_0(this.Source, "Source");
				HandSourceInjector.ActiveDataSource.<Initialize>g__AssertField|10_0(this.ModifyAfter, "ModifyAfter");
			}

			public bool IsActive()
			{
				return this.Source.GetData().IsDataValidAndConnected;
			}

			[CompilerGenerated]
			internal static void <Initialize>g__AssertField|10_0(object obj, string name)
			{
			}

			[SerializeField]
			[Interface(typeof(IDataSource<HandDataAsset>), new Type[]
			{

			})]
			private Object _source;

			[SerializeField]
			[Interface(typeof(IDataSource), new Type[]
			{

			})]
			private Object _modifyAfter;
		}
	}
}
