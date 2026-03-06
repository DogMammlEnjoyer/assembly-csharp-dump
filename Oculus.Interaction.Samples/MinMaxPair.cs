using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Serializable]
	public struct MinMaxPair
	{
		public bool UseRandomRange
		{
			get
			{
				return this._useRandomRange;
			}
		}

		public float Min
		{
			get
			{
				return this._min;
			}
		}

		public float Max
		{
			get
			{
				return this._max;
			}
		}

		[SerializeField]
		private bool _useRandomRange;

		[SerializeField]
		private float _min;

		[SerializeField]
		private float _max;
	}
}
