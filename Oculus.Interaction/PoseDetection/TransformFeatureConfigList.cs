using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	[Serializable]
	public class TransformFeatureConfigList
	{
		public List<TransformFeatureConfig> Values
		{
			get
			{
				return this._values;
			}
		}

		public static TransformFeatureConfigList Create(List<TransformFeatureConfig> values)
		{
			return new TransformFeatureConfigList
			{
				_values = values
			};
		}

		[SerializeField]
		private List<TransformFeatureConfig> _values;
	}
}
