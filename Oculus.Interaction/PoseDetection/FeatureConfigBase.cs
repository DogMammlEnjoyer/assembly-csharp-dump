using System;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[Serializable]
	public abstract class FeatureConfigBase<TFeature>
	{
		public FeatureStateActiveMode Mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				this._mode = value;
			}
		}

		public TFeature Feature
		{
			get
			{
				return this._feature;
			}
			set
			{
				this._feature = value;
			}
		}

		public string State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		[SerializeField]
		private FeatureStateActiveMode _mode;

		[SerializeField]
		private TFeature _feature;

		[SerializeField]
		private string _state;
	}
}
